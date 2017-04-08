using DataModel;
using SynapseSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Util;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Networking;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ElementalWar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MesaTablero : Page
    {
        private Juego objJuego;
        public MesaTablero()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            if (e.Parameter != null)
            {
                objJuego = (Juego)e.Parameter;
            }
            else
            {
                Helper.MensajeOk("No se pudo iniciar el juego.");
                this.Frame.Navigate(typeof(SeleccionarRol));
            }
            IniciarSDK();
            Inicializar();
        }

        private void Inicializar()
        {
            GameLogic.LogicaJuego.AsignarTurnos(objJuego);
            ComunicarJugadoresJuegoInicia();
            DibujarInfoJugadores();
        }

        private async void ComunicarJugadoresJuegoInicia()
        {
            //Indica a los jugadores que inicio el juego y quien es el primer
            for (int i = 0; i < objJuego.Jugadores.Count; i++)
            {
                await App.objSDK.UnicastPing(new HostName(objJuego.Jugadores[i].Ip),
                    Constantes.Mensajes.Juego.MesaIndicaJuegoInicia + Constantes.SEPARADOR +
                    objJuego.Jugadores[0].JugadorId);
            }
        }

        private void DibujarInfoJugadores()
        {

        }

        #region Conexion SynapseSDK
        private void IniciarSDK()
        {
            try
            {
                App.UIDispatcher = this.Dispatcher;
                App.objSDK = MainCore.getInstance(Constantes.MULTICAST_ADDRESS, Constantes.MULTICAST_SERVICE_PORT, Constantes.UNICAST_SERVICE_PORT, Constantes.STREAM_SERVICE_PORT, MiReceptorJuego, Constantes.DELAY);
                int cont = 0;
                while (!App.objSDK.SocketIsConnected && cont < 3)
                {
                    App.objSDK.TearDownSockets();
                    App.objSDK.InitializeSockets();
                    cont++;
                }

                if (App.objSDK.SocketIsConnected)
                {
                    App.objSDK.setObjMetodoReceptorString = MiReceptorJuego;
                }
                else
                {
                    //No hay conexion
                    string strMensaje = "Lo sentimos, no se pudo establecer la conexión vía Wi-Fi. Intente nuevamente.";
                    Helper.MensajeOk(strMensaje);
                    this.Frame.Navigate(typeof(SeleccionarRol));
                    return;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }

        private async void MiReceptorJuego(string strIp, string strMessage)
        {
            try
            {
                await App.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ReceptorJuego(strIp, strMessage);
                });
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }
        #endregion

        private void ReceptorJuego(string strIp, string strMensaje)
        {
            try
            {
                if (!string.IsNullOrEmpty(strIp) && !string.IsNullOrEmpty(strMensaje))
                {
                    var mensaje = strMensaje.Split(new string[] { Constantes.SEPARADOR }, StringSplitOptions.None);
                    //mensaje[0] => Accion
                    //#region Jugador solicita unirse a la mesa
                    //if (mensaje[0] == Constantes.Mensajes.UnirseMesa.SolicitudUnirse)
                    //{
                    //    //mensaje[1] => objJuego.Codigo
                    //    //mensaje[2] => objJugador.Ip
                    //    //mensaje[3] => objJugador.Nombre
                    //    if (mensaje.Length != 4)
                    //        return;

                    //}
                    //#endregion
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }
    }
}
