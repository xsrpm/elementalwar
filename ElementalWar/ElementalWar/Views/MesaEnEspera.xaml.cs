using DataModel;
using SynapseSDK;
using System;
using System.Linq;
using System.Threading;
using Util;
using Windows.Graphics.Display;
using Windows.Networking;
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
    public sealed partial class MesaEnEspera : Page
    {
        private Juego objJuego;
        private bool recibirComando;
        private Timer timerCuentaRegresivaInicioJuego;
        private int cuentaRegresiva;

        public MesaEnEspera()
        {
            this.InitializeComponent();

            InicializarVariables();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            IniciarSDK();
        }

        private void InicializarVariables()
        {
            objJuego = new Juego();
            recibirComando = true;

            timerCuentaRegresivaInicioJuego = new Timer(timerCuentaRegresivaInicioJuegoCallback, null, Timeout.Infinite, Timeout.Infinite);
            cuentaRegresiva = 4;

            contadorPanel.Visibility = Visibility.Collapsed;
            flechaizq0.Opacity = 0;
            flechader0.Opacity = 0;
            imglisto0.Opacity = 0;
            flechaizq1.Opacity = 0;
            flechader1.Opacity = 0;
            imglisto1.Opacity = 0;
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
                        await App.objSDK.ConnectStreamSocket(new HostName(item.Ip));
                        await App.objSDK.StreamPing(Constantes.Mensajes.Juego.MesaIndicaSeCierra);
                    }
                }
                App.objSDK.setObjMetodoReceptorString = null;
            }
            this.Frame.Navigate(typeof(SeleccionarRol));
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

                        var jugadorUnido = GameLogic.LogicaMesaEnEspera.MesaAgregarJugador(objJuego, jugador);
                        if (jugadorUnido != null)
                        {
                            //Notificar al nuevo jugador que se ha unido a la mesa
                            await App.objSDK.ConnectStreamSocket(new HostName(jugadorUnido.Ip));
                            await App.objSDK.StreamPing(Constantes.Mensajes.UnirseMesa.ConfirmacionUnirse + Constantes.SEPARADOR +
                                objJuego.Ip + Constantes.SEPARADOR +
                                jugadorUnido.JugadorId + Constantes.SEPARADOR +
                                jugadorUnido.Elemento.ElementoId);

                            //Mostrar datos del jugadors en pantalla
                            GameLogic.LogicaMesaEnEspera.SetearElementoJugador(objJuego, jugadorUnido.JugadorId);
                            //Dibujar UI Jugador
                            ((Image)FindName("flechaizq" + jugadorUnido.JugadorId)).Opacity = 1;
                            ((Image)FindName("flechader" + jugadorUnido.JugadorId)).Opacity = 1;
                            ((Image)FindName("imglisto" + jugadorUnido.JugadorId)).Opacity = 0;
                            MostrarDatosJugadoresEnPantalla(jugadorUnido.JugadorId);
                        }
                    }
                    #endregion
                    #region El jugador envia la imagen que le corresponde para ser representado en el juego
                    else if (mensaje[0] == Constantes.Mensajes.UnirseMesa.EnviarImagenJugador)
                    {
                        //mensaje[1] => objJugador.JugadorId
                        //mensaje[2] => strBytes (objJugador.Imagen)
                        if (mensaje.Length != 3)
                            return;

                        if (mensaje[2] == Constantes.Imagenes.SIN_IMAGEN)
                            objJuego.Jugadores[int.Parse(mensaje[1])].Imagen = null;
                        else
                            objJuego.Jugadores[int.Parse(mensaje[1])].Imagen = Convert.FromBase64String(mensaje[2]);
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
                            var jugadorId = int.Parse(mensaje[1]);
                            //ReDibujar UI Jugador
                            ((Image)FindName("flechaizq" + jugadorId)).Opacity = 0;
                            ((Image)FindName("flechader" + jugadorId)).Opacity = 0;
                            ((Image)FindName("imglisto" + jugadorId)).Opacity = 0;
                            MostrarDatosJugadoresEnPantalla(jugadorId);
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
                        EvaluarComandoJugador(int.Parse(mensaje[1]), int.Parse(mensaje[2]));
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }

        private void EvaluarComandoJugador(int jugadorId, int movimiento)
        {
            if (recibirComando)
            {
                switch (movimiento)
                {
                    case Constantes.Mensajes.Juego.AccionMando.Izquierda:
                    case Constantes.Mensajes.Juego.AccionMando.Derecha:
                        CambiarElemento(jugadorId, movimiento);
                        break;
                    case Constantes.Mensajes.Juego.AccionMando.Accion:
                        MarcarJugadorListo(jugadorId);
                        break;
                }
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

        private void MarcarJugadorListo(int jugadorId)
        {
            if (!objJuego.Jugadores[jugadorId].Listo)
            {
                objJuego.Jugadores[jugadorId].Listo = true;
                ((Image)FindName("flechaizq" + jugadorId)).Opacity = 0;
                ((Image)FindName("flechader" + jugadorId)).Opacity = 0;
                ((Image)FindName("imglisto" + jugadorId)).Opacity = 1;
            }
            else
            {
                objJuego.Jugadores[jugadorId].Listo = false;
                ((Image)FindName("flechaizq" + jugadorId)).Opacity = 1;
                ((Image)FindName("flechader" + jugadorId)).Opacity = 1;
                ((Image)FindName("imglisto" + jugadorId)).Opacity = 0;
            }

            if (objJuego.Jugadores[0].Listo && objJuego.Jugadores[1].Listo)
            {
                recibirComando = false;
                contadorPanel.Visibility = Visibility.Visible;
                timerCuentaRegresivaInicioJuego.Change(0, 1000 * 1);
            }
        }

        private async void timerCuentaRegresivaInicioJuegoCallback(object state)
        {
            await App.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                cuentaRegresiva--;

                if (cuentaRegresiva >= 0)
                {
                    lblContador.Text = cuentaRegresiva.ToString();
                }
                else
                {
                    timerCuentaRegresivaInicioJuego.Change(Timeout.Infinite, Timeout.Infinite);
                    this.Frame.Navigate(typeof(MesaTablero), objJuego);
                }
            });
        }
    }
}
