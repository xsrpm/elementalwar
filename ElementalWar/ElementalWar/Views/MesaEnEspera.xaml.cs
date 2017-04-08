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
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Networking;
using Windows.UI.Core;
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
    public sealed partial class MesaEnEspera : Page
    {
        private Juego objJuego;

        public MesaEnEspera()
        {
            this.InitializeComponent();
            objJuego = new Juego();
            btnJugar.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            IniciarSDK();
        }

        private async void imgAtras_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Notificar a los dispositivos que se ha cerrado la mesa
            if (App.objSDK.SocketIsConnected)
            {
                foreach (var item in objJuego.Jugadores)
                {
                    if (item.Ip != "")
                    {
                        await App.objSDK.UnicastPing(new HostName(item.Ip),
                            Constantes.Mensajes.Juego.MesaIndicaSeCierra);
                    }
                }
                App.objSDK.setObjMetodoReceptorString = null;
            }
            this.Frame.Navigate(typeof(SeleccionarRol));
        }

        private void btnJugar_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MesaTablero), objJuego);
        }

        private void MostrarDatosJugadoresEnPantalla(int jugadorId)
        {
            Uri uri;
            BitmapImage imagenElemento;

            if (objJuego.Jugadores[jugadorId].Ip != "")
            {
                uri = new Uri(objJuego.Jugadores[jugadorId].Elemento.RutaImagen);
                imagenElemento = new BitmapImage(uri);
            }
            else
            {
                uri = new Uri(Constantes.Imagenes.Jugador);
                imagenElemento = new BitmapImage(uri);
            }

            ((Image)this.FindName("imgElementoJug" + jugadorId)).Source = imagenElemento;
            ((TextBlock)this.FindName("lblNombreJug" + jugadorId)).Text = objJuego.Jugadores[jugadorId].Nombre;
        }

        #region Conexion SynapseSDK
        private void IniciarSDK()
        {
            try
            {
                App.UIDispatcher = this.Dispatcher;
                App.objSDK = MainCore.getInstance(Constantes.MULTICAST_ADDRESS, Constantes.MULTICAST_SERVICE_PORT, Constantes.UNICAST_SERVICE_PORT, Constantes.STREAM_SERVICE_PORT, MiReceptorMesaEnEspera, Constantes.DELAY);
                int cont = 0;
                while (!App.objSDK.SocketIsConnected && cont < 3)
                {
                    App.objSDK.TearDownSockets();
                    App.objSDK.InitializeSockets();
                    cont++;
                }

                if (App.objSDK.SocketIsConnected)
                {
                    GameLogic.LogicaMesaEnEspera.SetearMesa(objJuego, App.objSDK.MyIP.ToString());
                    lblCodigoMesa.Text = objJuego.Codigo;
                    App.objSDK.setObjMetodoReceptorString = MiReceptorMesaEnEspera;
                }
                else
                {
                    //No hay conexión
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

        private async void MiReceptorMesaEnEspera(string strIp, string strMessage)
        {
            try
            {
                await App.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    ReceptorMesaEnEspera(strIp, strMessage);
                });
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }
        #endregion

        private async void ReceptorMesaEnEspera(string strIp, string strMensaje)
        {
            try
            {
                if (!string.IsNullOrEmpty(strIp) && !string.IsNullOrEmpty(strMensaje))
                {
                    var mensaje = strMensaje.Split(new string[] { Constantes.SEPARADOR }, StringSplitOptions.None);
                    //mensaje[0] => Accion
                    #region Jugador solicita unirse a la mesa
                    if (mensaje[0] == Constantes.Mensajes.UnirseMesa.SolicitudUnirse)
                    {
                        //mensaje[1] => objJuego.Codigo
                        //mensaje[2] => objJugador.Ip
                        //mensaje[3] => objJugador.Nombre
                        if (mensaje.Length != 4)
                            return;

                        //Verificar que es la mesa seleccionada
                        if (mensaje[1] != objJuego.Codigo)
                            return;

                        //Unir al jugador a la mesa
                        var jugador = new Jugador();
                        jugador.Ip = mensaje[2];
                        jugador.Nombre = mensaje[3];
                        //if (!mensaje[4].Equals(Constantes.SIN_IMAGEN))
                        //    jugador.Imagen = Convert.FromBase64String(mensaje[4]);
                        var jugadorUnido = GameLogic.LogicaMesaEnEspera.MesaAgregarJugador(objJuego, jugador);
                        if (jugadorUnido != null)
                        {
                            //Notificar al nuevo jugador que se ha unido a la mesa
                            await App.objSDK.UnicastPing(new HostName(jugadorUnido.Ip),
                                Constantes.Mensajes.UnirseMesa.ConfirmacionUnirse + Constantes.SEPARADOR +
                                objJuego.Ip + Constantes.SEPARADOR +
                                jugadorUnido.JugadorId + Constantes.SEPARADOR +
                                jugadorUnido.Elemento);

                            //Mostrar datos del jugadors en pantalla
                            MostrarDatosJugadoresEnPantalla(jugadorUnido.JugadorId);
                        }
                    }
                    #endregion
                    #region El jugador indica que se sale del mando mientras el juego aun no comienza
                    else if (mensaje[0] == Constantes.Mensajes.Juego.JugadorSaleMesa)
                    {
                        //mensaje[1] => objJugador.JugadorId
                        if (mensaje.Length != 2)
                            return;

                        //Desconectar al jugador
                        if (GameLogic.LogicaMesaEnEspera.MesaEliminarJugador(objJuego, objJuego.Jugadores[int.Parse(mensaje[1])].Ip))
                        {
                            MostrarDatosJugadoresEnPantalla(int.Parse(mensaje[1]));
                        }
                    }
                    #endregion
                    #region El jugador presiona un boton
                    else if (mensaje[0] == Constantes.Mensajes.Juego.MovimientoJugador)
                    {
                        //mensaje[1] => objJugador.JugadorId
                        //mensaje[2] => movimiento
                        if (mensaje.Length != 3)
                            return;

                        //En esta pantalla solo se realiza el cambio de elemento
                        CambiarElemento(int.Parse(mensaje[1]), int.Parse(mensaje[2]));
                    }
                    #endregion

                    //Habilitar el boton jugar
                    if (objJuego.Jugadores.Count(x => x.Ip != "") >= 2)
                        btnJugar.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }

        private void CambiarElemento(int jugadorId, int movimiento)
        {
            if (GameLogic.LogicaMesaEnEspera.CambiarFichaJugador(objJuego, jugadorId, jugadorId == 0 ? 1 : 0, movimiento))
            {
                GameLogic.LogicaMesaEnEspera.SetearElementoJugador(objJuego, jugadorId);
                MostrarDatosJugadoresEnPantalla(jugadorId);
            }
        }
    }
}
