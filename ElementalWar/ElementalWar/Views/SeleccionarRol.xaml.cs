using Util;
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
    public sealed partial class SeleccionarRol : Page
    {
        bool perfilCreado = false;

        public SeleccionarRol()
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
                    perfilCreado = true;
                }
                if (App.objJugador.Imagen != null)
                {
                    BitmapImage bimgBitmapImage = new BitmapImage();
                    IRandomAccessStream fileStream = await Convertidor.ConvertImageToStream(App.objJugador.Imagen);
                    bimgBitmapImage.SetSource(fileStream);
                    imgJugador.Source = bimgBitmapImage;
                }
            }
        }

        private void imgAtras_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MenuPrincipal));
        }

        private void panelInfoJugador_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GuardarDatosJugador), typeof(SeleccionarRol));
        }

        private void panelTablero_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MesaEnEspera));
        }

        private void panelJugador_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (perfilCreado)
            {
                this.Frame.Navigate(typeof(ElegirMesa));
            }
            else
            {
                this.Frame.Navigate(typeof(GuardarDatosJugador), typeof(ElegirMesa));
            }
        }
    }
}
