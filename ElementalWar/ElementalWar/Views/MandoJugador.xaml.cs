using SynapseSDK;
using System;
using System.Threading;
using Util;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Networking;
using Windows.Phone.Devices.Notification;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
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
        private Timer timerMantenerConexion;
        private int contReconexion;

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
            lblTurno.Text = Constantes.MensajesMando.MANDOMESAENESPERA;
            panelTurno.Background = Convertidor.GetSolidColorBrush(Constantes.Colores.COLORENESPERA);
            IniciarSDK();

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            PanelReconexionOcultar();
            timerMantenerConexion = new Timer(timerMantenerConexionCallback, null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(Constantes.Reconexion.KeepAlive));
        }

        #region Revision de conexion
        private async void timerMantenerConexionCallback(object state)
        {
            await App.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                VerificarConexion();
            });
        }

        private void VerificarConexion()
        {
            IniciarSDK();

            if (App.objSDK.SocketIsConnected)
                contReconexion = 0;
            else
                contReconexion++;

            if (contReconexion == Constantes.Reconexion.CountTimeoutConnection)
            {
                contReconexion = 0;
                PanelReconexionMostrar();
            }
            else
            {
                PanelReconexionOcultar();
            }
        }

        #region Panel reconexion
        private void PanelReconexionMostrar()
        {
            prConectando.IsActive = true;
            panelReconexion.Visibility = Visibility.Visible;
        }

        private void PanelReconexionOcultar()
        {
            panelReconexion.Visibility = Visibility.Collapsed;
            prConectando.IsActive = false;
        }
        #endregion
        #endregion

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
                timerMantenerConexion.Change(Timeout.Infinite, Timeout.Infinite);

                //Notificar a la mesa que se esta saliendo de la mesa
                await App.objSDK.StreamPing(Constantes.Mensajes.Juego.JugadorSaleMesa + Constantes.SEPARADOR +
                    App.objJugador.JugadorId);
                App.objSDK.setObjMetodoReceptorString = null;

                if (App.DetectPlatform() == Platform.WindowsPhone)
                {
                    this.Frame.Navigate(typeof(ElegirMesa));
                }
                else
                {
                    this.Frame.Navigate(typeof(SeleccionarRol));
                }
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
                    App.objSDK.MulticastPing();
                    App.objSDK.ConnectStreamSocket(new HostName(App.objJugador.MesaIp));
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
                    #region Jugador recibe la instruccion que la mesa se ha cerrado
                    if (mensaje[0] == Constantes.Mensajes.Juego.MesaIndicaSeCierra)
                    {
                        if (mensaje.Length != 1)
                            return;

                        App.objSDK.setObjMetodoReceptorString = null;
                        Helper.MensajeOk("La partida en curso ha sido cerrada.");

                        this.Frame.Navigate(typeof(ElegirMesa));
                    }
                    #endregion
                    #region Mesa indica que el juego acaba de iniciar
                    else if (mensaje[0] == Constantes.Mensajes.Juego.MesaIndicaJuegoInicia)
                    {
                        //mensaje[1] => objJugador.Ip
                        if (mensaje.Length != 2)
                            return;

                        if (mensaje[1] == App.objJugador.Ip)
                        {
                            App.objJugador.JugadorId = 0;
                            mandoActivo = false;
                            HabilitarControles();
                        }
                        else
                        {
                            App.objJugador.JugadorId = 1;
                            mandoActivo = true;
                            DeshabilitarControles();
                        }
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
                    #region Jugador Gano
                    else if (mensaje[0] == Constantes.Mensajes.Juego.FinJuegoGanaste)
                    {
                        if (mensaje.Length != 1)
                            return;
                        DeshabilitarControles();
                        MostrarResultadoFinalJuego("GANASTE");
                    }
                    #endregion
                    #region Jugador Perdio
                    else if (mensaje[0] == Constantes.Mensajes.Juego.FinJuegoPerdiste)
                    {
                        if (mensaje.Length != 1)
                            return;
                        DeshabilitarControles();
                        MostrarResultadoFinalJuego("PERDISTE");
                    }
                    #endregion
                    #region Jugador Empato
                    else if (mensaje[0] == Constantes.Mensajes.Juego.FinJuegoEmpataste)
                    {
                        if (mensaje.Length != 1)
                            return;
                        DeshabilitarControles();
                        MostrarResultadoFinalJuego("EMPATASTE");
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
            if (!mandoActivo)
            {
                mandoActivo = true;
                lblTurno.Text = Constantes.MensajesMando.MANDOACTIVO;
                panelTurno.Background = Convertidor.GetSolidColorBrush(Constantes.Colores.COLORACTIVO);
                if (App.DetectPlatform() == Platform.WindowsPhone)
                {
                    var vibration = VibrationDevice.GetDefault();
                    vibration.Vibrate(TimeSpan.FromMilliseconds(500));
                }
            }
        }

        private void DeshabilitarControles()
        {
            if (mandoActivo)
            {
                mandoActivo = false;
                lblTurno.Text = Constantes.MensajesMando.MANDOINACTIVO;
                panelTurno.Background = Convertidor.GetSolidColorBrush(Constantes.Colores.COLORINACTIVO);
            }
        }

        private async void EnviarMovimiento(int movimiento)
        {
            pageMando.IsEnabled = false;
            pageMando.IsEnabled = true;

            if (mandoActivo)
            {
                await App.objSDK.StreamPing(Constantes.Mensajes.Juego.MovimientoJugador + Constantes.SEPARADOR +
                    App.objJugador.JugadorId + Constantes.SEPARADOR +
                    movimiento);
            }
        }

        private void MostrarResultadoFinalJuego(string mensaje)
        {
            if (App.DetectPlatform() == Platform.WindowsPhone)
            {
                var vibration = VibrationDevice.GetDefault();
                vibration.Vibrate(TimeSpan.FromMilliseconds(500));
            }

            btnMensaje.Content = mensaje;

            panelRegresar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            Contenido.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            panelTurno.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            mensajeFinJuego.Visibility = Windows.UI.Xaml.Visibility.Visible;
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

        private void btnMenuPrincipal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MenuPrincipal));
        }

        private void imgJugador_Tapped(object sender, TappedRoutedEventArgs e)
        {
            IniciarSDK();
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.Left:
                    EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Izquierda);
                    break;
                case Windows.System.VirtualKey.Up:
                    EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Arriba);
                    break;
                case Windows.System.VirtualKey.Right:
                    EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Derecha);
                    break;
                case Windows.System.VirtualKey.Down:
                    EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Abajo);
                    break;
                case Windows.System.VirtualKey.Space:
                    EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Accion);
                    break;
                case Windows.System.VirtualKey.Enter:
                    EnviarMovimiento(Constantes.Mensajes.Juego.AccionMando.Accion);
                    break;
            }
        }
    }
}
