using DataModel;
using System;
using Util;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
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
    public sealed partial class GuardarDatosJugador : Page
    {
        Type paginaRedirect;
        public static BitmapImage bimgBitmapImage = new BitmapImage();
        byte[] fotoBytes;

        public GuardarDatosJugador()
        {
            this.InitializeComponent();
            paginaRedirect = typeof(MenuPrincipal);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            //Pagina a redireccionar
            if (e.Parameter != null)
                paginaRedirect = (Type)e.Parameter;

            if (App.objJugador != null)
            {
                if (App.objJugador.Nombre != null)
                {
                    txtNombre.Text = App.objJugador.Nombre;
                }
                if (App.objJugador.Imagen != null)
                {
                    fotoBytes = App.objJugador.Imagen;
                    bimgBitmapImage = new BitmapImage();
                    IRandomAccessStream fileStream = await Convertidor.ConvertImageToStream(fotoBytes);
                    bimgBitmapImage.SetSource(fileStream);
                    imgFoto.Source = bimgBitmapImage;
                }
            }
            else
            {
                if (paginaRedirect == typeof(ElegirMesa))
                {
                    panelRegresar.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void imgAtras_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(paginaRedirect);
        }

        private async void imgFoto_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                IRandomAccessStream fileStream;
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.FileTypeFilter.Clear();
                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".jpeg");
                openPicker.FileTypeFilter.Add(".jpg");

                StorageFile file = await openPicker.PickSingleFileAsync();

                if (file != null)
                {
                    fileStream = await file.OpenAsync(FileAccessMode.Read);
                    fotoBytes = await Convertidor.ConvertImageToBytes(fileStream);
                    bimgBitmapImage.SetSource(fileStream);
                    imgFoto.Source = bimgBitmapImage;
                }
                lblTexto.Focus(FocusState.Programmatic);
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }

        private void txtNombre_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                txtNombre.IsEnabled = false;
                txtNombre.IsEnabled = true;
                btnGrabar_Tapped(null, null);
            }
        }

        private async void btnGrabar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string strMensaje;
            try
            {
                if (string.IsNullOrEmpty(txtNombre.Text.Trim()))
                {
                    strMensaje = "No ha ingresado su nombre";
                    Helper.MensajeOk(strMensaje);
                    return;
                }

                var objJugador = new Jugador();
                objJugador.Nombre = txtNombre.Text.TrimStart().TrimEnd();
                objJugador.Imagen = fotoBytes;
                await LocalStorage.GuardarDatosJugador(objJugador);
                App.objJugador = objJugador;

                this.Frame.Navigate(paginaRedirect);
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }
    }
}
