using System;
using System.Threading.Tasks;
using Util;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ElementalWar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashScreen : Page
    {
        public SplashScreen()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            //Inicializando
            App.objJugador = await LocalStorage.ObtenerDatosJugador();

            await Task.Delay(TimeSpan.FromSeconds(3));

            if (App.DetectPlatform() == Platform.WindowsPhone)
            {
                if (App.objJugador == null)
                {
                    Frame.Navigate(typeof(GuardarDatosJugador), typeof(MenuPrincipal));
                }
                else
                {
                    Frame.Navigate(typeof(MenuPrincipal));
                }
            }
            else
            {
                Frame.Navigate(typeof(MenuPrincipal));
            }
        }
    }
}
