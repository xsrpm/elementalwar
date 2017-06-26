using DataModel;
using SynapseSDK;
using System;
using Util;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Networking;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
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
    public sealed partial class ElegirMesa : Page
    {
        public ElegirMesa()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
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

        private void imgAtras_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.objSDK.setObjMetodoReceptorString = null;
            if (App.DetectPlatform() == Platform.WindowsPhone)
            {
                this.Frame.Navigate(typeof(MenuPrincipal));
            }
            else
            {
                this.Frame.Navigate(typeof(SeleccionarRol));
            }
        }

        private void panelInfoJugador_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GuardarDatosJugador), typeof(ElegirMesa));
        }

        private void txtSala_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                txtSala.IsEnabled = false;
                txtSala.IsEnabled = true;
                btnUnirme_Tapped(null, null);
            }
        }

        private void txtSala_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key.ToString().Equals("Back"))
            {
                e.Handled = false;
                return;
            }
            for (int i = 0; i < 10; i++)
            {
                if (e.Key.ToString() == string.Format("Number{0}", i))
                {
                    e.Handled = false;
                    return;
                }
            }
            e.Handled = true;
        }

        private async void btnUnirme_Tapped(object sender, TappedRoutedEventArgs e)
        {
            txtSala.IsEnabled = false;
            imgUnirme.Visibility = Visibility.Collapsed;
            panelConectando.Visibility = Visibility.Visible;
            prConectando.IsActive = true;
            try
            {
                string mesaSeleccionada = txtSala.Text;

                string strBytesImagen = "";
                if (App.objJugador.Imagen != null)
                    strBytesImagen = Convert.ToBase64String(App.objJugador.Imagen);
                else
                    strBytesImagen = Constantes.Imagenes.SIN_IMAGEN;

                App.objSDK.clearDeviceCollection();
                await App.objSDK.MulticastPing();
                var dispositivos = App.objSDK.getDeviceCollection();

                if (dispositivos != null)
                {
                    foreach (var objDevice in dispositivos)
                    {
                        var hn = new HostName(objDevice.IP);
                        await App.objSDK.ConnectStreamSocket(hn);
                        await App.objSDK.StreamPing(Constantes.Mensajes.UnirseMesa.SolicitudUnirse + Constantes.SEPARADOR +
                            mesaSeleccionada + Constantes.SEPARADOR +
                            App.objJugador.Ip + Constantes.SEPARADOR +
                            App.objJugador.Nombre + Constantes.SEPARADOR +
                            strBytesImagen);
                    }
                }
            }
            catch (Exception)
            {
            }
            prConectando.IsActive = false;
            panelConectando.Visibility = Visibility.Collapsed;
            imgUnirme.Visibility = Visibility.Visible;
            txtSala.IsEnabled = true;
        }

        #region Conexion SynapseSDK
        private void IniciarSDK()
        {
            try
            {
                App.UIDispatcher = this.Dispatcher;
                App.objSDK = MainCore.getInstance(Constantes.MULTICAST_ADDRESS, Constantes.MULTICAST_SERVICE_PORT, Constantes.UNICAST_SERVICE_PORT, Constantes.STREAM_SERVICE_PORT, MiReceptorElegirMesa, Constantes.DELAY);
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
                    App.objSDK.setObjMetodoReceptorString = MiReceptorElegirMesa;
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

        public async void MiReceptorElegirMesa(string strIp, string strMessage)
        {
            try
            {
                await App.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ReceptorElegirMesa(strIp, strMessage);
                });
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }
        #endregion

        public void ReceptorElegirMesa(string strIp, string strMensaje)
        {
            try
            {
                if (!string.IsNullOrEmpty(strIp) && !string.IsNullOrEmpty(strMensaje))
                {
                    var mensaje = strMensaje.Split(new string[] { Constantes.SEPARADOR }, StringSplitOptions.None);
                    //mensaje[0] => Accion
                    #region Jugador recibe la confirmacion que se ha unido a la mesa
                    if (mensaje[0] == Constantes.Mensajes.UnirseMesa.ConfirmacionUnirse)
                    {
                        //mensaje[1] => objJuego.Ip
                        //mensaje[2] => objJugador.JugadorId
                        //mensaje[3] => objJugador.Elemento
                        if (mensaje.Length != 4)
                            return;

                        txtSala.IsEnabled = false;
                        imgUnirme.Visibility = Visibility.Collapsed;
                        panelConectando.Visibility = Visibility.Visible;
                        prConectando.IsActive = true;

                        //Reenviar a la pantalla del mando, el juega ya inicio
                        App.objJugador.MesaIp = mensaje[1];
                        App.objJugador.JugadorId = int.Parse(mensaje[2]);
                        App.objJugador.Elemento = new Elemento { ElementoId = int.Parse(mensaje[3]) };

                        this.Frame.Navigate(typeof(MandoJugador));
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }
    }
}
