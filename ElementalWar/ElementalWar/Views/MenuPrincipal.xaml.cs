using SynapseSDK;
using System;
using Util;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
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
    public sealed partial class MenuPrincipal : Page
    {
        public MenuPrincipal()
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
            RevisarConexionSDK();
        }

        private void panelInfoJugador_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GuardarDatosJugador), typeof(MenuPrincipal));
        }

        private void btnJugar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (App.DetectPlatform() == Platform.WindowsPhone)
            {
                this.Frame.Navigate(typeof(ElegirMesa));
            }
            else
            {
                this.Frame.Navigate(typeof(SeleccionarRol));
            }
        }

        private void btnComoJugar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ComoJugar));
        }

        private void btnCreditos_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Creditos));
        }

        private void RevisarConexionSDK()
        {
            try
            {
                App.UIDispatcher = this.Dispatcher;
                App.objSDK = MainCore.getInstance(Constantes.MULTICAST_ADDRESS, Constantes.MULTICAST_SERVICE_PORT, Constantes.UNICAST_SERVICE_PORT, Constantes.STREAM_SERVICE_PORT, null, Constantes.DELAY);
                if (App.objSDK.SocketIsConnected)
                {
                    App.objSDK.TearDownSockets();
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }
    }
}
