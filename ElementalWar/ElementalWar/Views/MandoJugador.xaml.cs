using SynapseSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Util;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Networking;
using Windows.Phone.Devices.Notification;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ElementalWar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MandoJugador : Page
    {
        private bool mandoActivo = true;
        public MandoJugador()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            if (App.objJugador != null)
            {
                if (App.objJugador.Nombre != null)
                {
                    lblBienvenido.Text = App.objJugador.Nombre;
                }
                if (App.objJugador.Imagen != null)
                {
                    BitmapImage bimgBitmapImage = new BitmapImage();
                    IRandomAccessStream fileStream = await Convertidor.ConvertImageToStream(App.objJugador.Imagen);
                    bimgBitmapImage.SetSource(fileStream);
                    imgJugador.Source = bimgBitmapImage;
                }
            }
            IniciarSDK();
        }

        private async void imgAtras_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MessageDialog msgDialog = new MessageDialog("¿Está seguro que desea salir de la partida actual?", Constantes.MessageDialogTitle);
            UICommand btnSi = new UICommand("Si") { Id = 1 };
            UICommand btnNo = new UICommand("No") { Id = 0 };
            msgDialog.Commands.Add(btnSi);
            msgDialog.Commands.Add(btnNo);
            msgDialog.DefaultCommandIndex = 1;
            msgDialog.CancelCommandIndex = 0;

            var result = await msgDialog.ShowAsync();

            //Confirma que desea salir de la mesa
            if ((int)result.Id == 1)
            {
                //Notificar a la mesa que se esta saliendo de la mesa
                await App.objSDK.UnicastPing(new HostName(App.objJugador.MesaIp),
                    Constantes.Mensajes.Juego.JugadorSaleMesa + Constantes.SEPARADOR +
                    App.objJugador.JugadorId);
                App.objSDK.setObjMetodoReceptorString = null;

                this.Frame.Navigate(typeof(SeleccionarRol));
            }
        }

        #region Conexion SynapseSDK
        private void IniciarSDK()
        {
            try
            {
                App.UIDispatcher = this.Dispatcher;
                App.objSDK = MainCore.getInstance(Constantes.MULTICAST_ADDRESS, Constantes.MULTICAST_SERVICE_PORT, Constantes.UNICAST_SERVICE_PORT, Constantes.STREAM_SERVICE_PORT, MiReceptorMandoJugador, Constantes.DELAY);
                int cont = 0;
                while (!App.objSDK.SocketIsConnected && cont < 3)
                {
                    App.objSDK.TearDownSockets();
                    App.objSDK.InitializeSockets();
                    cont++;
                }

                if (App.objSDK.SocketIsConnected)
                {
                    App.objJugador.Ip = App.objSDK.MyIP.ToString();
                    App.objSDK.setObjMetodoReceptorString = MiReceptorMandoJugador;
                }
                else
                {
                    //No hay conexión
                    string strMensaje = "Lo sentimos, no se pudo establecer la conexión vía Wi-Fi. Intente nuevamente.";
                    Helper.MensajeOk(strMensaje);
                    if (App.DetectPlatform() == Platform.WindowsPhone)
                        this.Frame.Navigate(typeof(MenuPrincipal));
                    else
                        this.Frame.Navigate(typeof(SeleccionarRol));
                    return;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }

        public async void MiReceptorMandoJugador(string strIp, string strMessage)
        {
            try
            {
                await App.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ReceptorMandoJugador(strIp, strMessage);
                });
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }
        #endregion

        public void ReceptorMandoJugador(string strIp, string strMensaje)
        {
            try
            {
                if (!string.IsNullOrEmpty(strIp) && !string.IsNullOrEmpty(strMensaje))
                {
                    var mensaje = strMensaje.Split(new string[] { Constantes.SEPARADOR }, StringSplitOptions.None);
                    //mensaje[0] => Accion
                    #region Jugador recibe la confirmacion que se ha unido a la mesa
                    if (mensaje[0] == Constantes.Mensajes.Juego.MesaIndicaSeCierra)
                    {
                        if (mensaje.Length != 1)
                            return;

                        App.objSDK.setObjMetodoReceptorString = null;
                        Helper.MensajeOk("La partida en curso ha sido cerrada.");

                        this.Frame.Navigate(typeof(ElegirMesa));
                    }
                    #endregion
                    #region La mesa indica Habilitar controles
                    else if (mensaje[0] == Constantes.Mensajes.Juego.HabilitarControles)
                    {
                        if (mensaje.Length != 1)
                            return;
                        HabilitarControles();
                    }
                    #endregion
                    #region La mesa indica Deshabilitar controles
                    else if (mensaje[0] == Constantes.Mensajes.Juego.DeshabilitarControles)
                    {
                        if (mensaje.Length != 1)
                            return;
                        DeshabilitarControles();
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }

        private void HabilitarControles()
        {
            mandoActivo = true;
            lblTurno.Text = "TURNO";
            panelTurno.Background = Convertidor.GetSolidColorBrush(Constantes.Colores.PanelTurno.COLORMANDOACTIVO);
            if (App.DetectPlatform() == Platform.WindowsPhone)
            {
                var vibration = VibrationDevice.GetDefault();
                vibration.Vibrate(TimeSpan.FromMilliseconds(500));
            }
        }

        private void DeshabilitarControles()
        {
            mandoActivo = false;
            lblTurno.Text = "";
            panelTurno.Background = Convertidor.GetSolidColorBrush(Constantes.Colores.PanelTurno.COLORMANDOINACTIVO);
        }

        private async void EnviarMovimiento(int movimiento)
        {
            if (mandoActivo)
            {
                await App.objSDK.UnicastPing(new HostName(App.objJugador.MesaIp),
                    Constantes.Mensajes.Juego.MovimientoJugador + Constantes.SEPARADOR +
                    App.objJugador.JugadorId + Constantes.SEPARADOR +
                    movimiento);
            }
        }

        #region Eventos botones
        private void btnIzquierda_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Izquierda);
        }

        private void btnArriba_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Arriba);
        }

        private void btnDerecha_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Derecha);
        }

        private void btnAbajo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Abajo);
        }

        private void btnAccion_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Accion);
        }
        #endregion
    }
}
