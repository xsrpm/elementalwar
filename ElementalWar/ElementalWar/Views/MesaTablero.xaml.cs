using DataModel;
using SynapseSDK;
using System;
using System.Collections.Generic;
using Util;
using Windows.Graphics.Display;
using Windows.Networking;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
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
            }

            IniciarSDK();
            //#region Pruebas
            //IniciarDataDePrueba();
            //#endregion
            Inicializar();
        }

        #region Pruebas
        private void IniciarDataDePrueba()
        {
            objJuego = new Juego
            {
                Codigo = "138",
                Ip = "192.168.1.38",
                Jugadores = new List<Jugador>
                {
                    new Jugador
                    {
                        JugadorId = 0,
                        Ip = "192.168.1.39",
                        Nombre = "Roy",
                        Imagen = null,
                        Elemento = new Elemento{ ElementoId = Constantes.Elementos.Agua }
                    },
                    new Jugador
                    {
                        JugadorId = 0,
                        Ip = "192.168.1.40",
                        Nombre = "Cesar",
                        Imagen = null,
                        Elemento = new Elemento{ ElementoId = Constantes.Elementos.Fuego }
                    }
                }
            };
            GameLogic.LogicaMesaEnEspera.SetearElementoJugador(objJuego, 0);
            GameLogic.LogicaMesaEnEspera.SetearElementoJugador(objJuego, 1);
        }
        #endregion

        #region Inicializacion
        private void Inicializar()
        {
            GameLogic.LogicaJuego.AsignarTurnos(objJuego);
            ComunicarJugadoresJuegoInicia();
            DibujarInfoJugadores();
            InicializarFichas();
            DibujarFichas();
            ActualizarInfoFichas();
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

        private async void DibujarInfoJugadores()
        {
            Uri uri;
            BitmapImage imagen;
            IRandomAccessStream fileStream;

            //Jugador 1
            if (objJuego.Jugadores[0].Imagen != null)
            {
                imagen = new BitmapImage();
                fileStream = await Convertidor.ConvertImageToStream(objJuego.Jugadores[0].Imagen);
                imagen.SetSource(fileStream);
                imgJugador1.Source = imagen;
            }
            lblJugador1.Text = objJuego.Jugadores[0].Nombre;
            uri = new Uri(objJuego.Jugadores[0].Elemento.RutaImagen);
            imagen = new BitmapImage(uri);
            imgElementoJugador1.Source = imagen;

            //Jugador 2
            if (objJuego.Jugadores[1].Imagen != null)
            {
                imagen = new BitmapImage();
                fileStream = await Convertidor.ConvertImageToStream(objJuego.Jugadores[1].Imagen);
                imagen.SetSource(fileStream);
                imgJugador2.Source = imagen;
            }
            lblJugador2.Text = objJuego.Jugadores[1].Nombre;
            uri = new Uri(objJuego.Jugadores[1].Elemento.RutaImagen);
            imagen = new BitmapImage(uri);
            imgElementoJugador2.Source = imagen;
        }

        private void InicializarFichas()
        {
            //Asignar las fichas inciales del centro
            objJuego.Fichas[4, 3].JugadorId = 0;
            objJuego.Fichas[4, 3].ElementoId = objJuego.Jugadores[0].Elemento.ElementoId;
            objJuego.Fichas[3, 4].JugadorId = 0;
            objJuego.Fichas[3, 4].ElementoId = objJuego.Jugadores[0].Elemento.ElementoId;

            objJuego.Fichas[3, 3].JugadorId = 1;
            objJuego.Fichas[3, 3].ElementoId = objJuego.Jugadores[1].Elemento.ElementoId;
            objJuego.Fichas[4, 4].JugadorId = 1;
            objJuego.Fichas[4, 4].ElementoId = objJuego.Jugadores[1].Elemento.ElementoId;

            //Asignar las referencias de las imagenes a cada ficha
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    objJuego.Fichas[i, k].Imagen = ((Image)this.FindName("elemento" + i + k));
                }
            }
        }

        private void DibujarFichas()
        {
            Uri uri;
            BitmapImage imagen;
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (objJuego.Fichas[i, k].ElementoId != Constantes.Elementos.SIN_ELEMENTO)
                    {
                        var jugadorId = objJuego.Fichas[i, k].JugadorId;
                        uri = new Uri(objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria);
                        imagen = new BitmapImage(uri);
                        objJuego.Fichas[i, k].Imagen.Source = imagen;
                        objJuego.Fichas[i, k].Imagen.Visibility = Visibility.Visible;
                    }   
                }
            }
        }

        private void ActualizarInfoFichas()
        {
            objJuego.ActualizarInfoFichas();
            lblPuntosJugador1.Text = objJuego.NroFichasJugador1.ToString();
            lblPuntosJugador2.Text = objJuego.NroFichasJugador2.ToString();
        }
        #endregion

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
