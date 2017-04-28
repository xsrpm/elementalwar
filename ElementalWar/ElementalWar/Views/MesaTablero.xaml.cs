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
using Windows.UI.Xaml.Media;
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
        private bool recibirComando;
        private SolidColorBrush colorValido;
        private SolidColorBrush colorInvalido;
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
            IniciarVariables();
            DibujarInfoJugadores();
            InicializarFichas();
            DibujarFichas();
            ActualizarInfoFichas();
            ComunicarJugadoresJuegoInicia();
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

            //Asignar la ficha inicial que se mueve por el tablero
            objJuego.Fichas[objJuego.PosXFicha, objJuego.PosYFicha].ElementoId = objJuego.Jugadores[0].Elemento.ElementoId;

            //Asignar las referencias de las imagenes a cada ficha
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    objJuego.Fichas[i, k].Imagen = ((Image)this.FindName("elemento" + k + i));
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
                    if (objJuego.Fichas[i, k].JugadorId != Constantes.NO_ASIGNADO)
                    {
                        var jugadorId = objJuego.Fichas[i, k].JugadorId;
                        uri = new Uri(objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria);
                        imagen = new BitmapImage(uri);
                        objJuego.Fichas[i, k].Imagen.Source = imagen;
                        objJuego.Fichas[i, k].Imagen.Visibility = Visibility.Visible;
                    }
                }
            }

            uri = new Uri(objJuego.Jugadores[0].Elemento.RutaImagenVictoria);
            imagen = new BitmapImage(uri);
            objJuego.Fichas[objJuego.PosXFicha, objJuego.PosYFicha].Imagen.Source = imagen;
            objJuego.Fichas[objJuego.PosXFicha, objJuego.PosYFicha].Imagen.Visibility = Visibility.Visible;
            objJuego.Fichas[objJuego.PosXFicha, objJuego.PosYFicha].Imagen.Opacity = Constantes.Ficha.Propiedades.OPACITY;
            bordeFichaSeleccionada.Stroke = colorValido;
            bordeFichaSeleccionada.SetValue(Grid.RowProperty, objJuego.PosYFicha + 1);
            bordeFichaSeleccionada.SetValue(Grid.ColumnProperty, objJuego.PosXFicha + 1);
        }

        private void ActualizarInfoFichas()
        {
            objJuego.ActualizarInfoFichas();
            lblPuntosJugador1.Text = objJuego.NroFichasJugador1.ToString();
            lblPuntosJugador2.Text = objJuego.NroFichasJugador2.ToString();
        }

        private void IniciarVariables()
        {
            recibirComando = true;
            colorValido = new SolidColorBrush(Windows.UI.Colors.Green);
            colorInvalido = new SolidColorBrush(Windows.UI.Colors.Red);
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
                    #region El jugador presiona un boton
                    if (mensaje[0] == Constantes.Mensajes.Juego.MovimientoJugador)
                    {
                        //mensaje[1] => objJugador.JugadorId
                        //mensaje[2] => movimiento
                        if (mensaje.Length != 3)
                            return;

                        //En esta pantalla solo se realiza el cambio de elemento
                        var jugadorId = int.Parse(mensaje[1]);
                        if (recibirComando && jugadorId == objJuego.JugadorIdTurno)
                        {
                            EjecutarBotonPresionadoJugador(int.Parse(mensaje[2]));
                        }
                    }
                    #endregion
                    #region Jugador indica sale del juego
                    else if (mensaje[0] == Constantes.Mensajes.Juego.JugadorSaleMesa)
                    {
                        //mensaje[1] => objJugador.JugadorId
                        if (mensaje.Length != 2)
                            return;

                        if (recibirComando)
                        {
                            recibirComando = false;
                            FinalizarJuego(true);
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Helper.MensajeOk(ex.Message);
            }
        }

        #region Accion Mando Jugador
        private void EjecutarBotonPresionadoJugador(int botonPresionado)
        {
            switch (botonPresionado)
            {
                case Constantes.Mensajes.Juego.AccionMando.Izquierda:
                case Constantes.Mensajes.Juego.AccionMando.Arriba:
                case Constantes.Mensajes.Juego.AccionMando.Derecha:
                case Constantes.Mensajes.Juego.AccionMando.Abajo:
                    MoverFicha(botonPresionado);
                    break;
                case Constantes.Mensajes.Juego.AccionMando.Accion:
                    ColocarFicha();
                    break;
            }
        }
        #endregion

        #region Mover Ficha
        private void MoverFicha(int direccion)
        {
            if (GameLogic.LogicaJuego.MovimientoValido(objJuego.PosXFicha, objJuego.PosYFicha, direccion))
            {
                PintarFichaOrigen();
                RealizarMoverFichaJugador(direccion);
                PintarFichaDestino();
            }
        }

        private void PintarFichaOrigen()
        {
            var posX = objJuego.PosXFicha;
            var posY = objJuego.PosYFicha;

            if (objJuego.Fichas[posX, posY].JugadorId == Constantes.NO_ASIGNADO)
            {
                objJuego.Fichas[posX, posY].Imagen.Visibility = Visibility.Collapsed;
            }
            else
            {
                Uri uri;
                BitmapImage imagen;
                uri = new Uri(objJuego.Jugadores[objJuego.Fichas[posX, posY].JugadorId].Elemento.RutaImagenVictoria);
                imagen = new BitmapImage(uri);
                objJuego.Fichas[posX, posY].Imagen.Source = imagen;
                objJuego.Fichas[posX, posY].Imagen.Opacity = 1;
            }
        }

        private void RealizarMoverFichaJugador(int direccion)
        {
            switch (direccion)
            {
                case Constantes.Mensajes.Juego.AccionMando.Izquierda:
                    objJuego.PosXFicha--;
                    break;
                case Constantes.Mensajes.Juego.AccionMando.Arriba:
                    objJuego.PosYFicha--;
                    break;
                case Constantes.Mensajes.Juego.AccionMando.Derecha:
                    objJuego.PosXFicha++;
                    break;
                case Constantes.Mensajes.Juego.AccionMando.Abajo:
                    objJuego.PosYFicha++;
                    break;
            }
        }

        private void PintarFichaDestino()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    objJuego.Fichas[i, k].Imagen.Opacity = 1;
                }
            }

            Uri uri;
            BitmapImage imagen;

            var posX = objJuego.PosXFicha;
            var posY = objJuego.PosYFicha;

            //Mover Borde
            bordeFichaSeleccionada.SetValue(Grid.RowProperty, posY + 1);
            bordeFichaSeleccionada.SetValue(Grid.ColumnProperty, posX + 1);
            //Pintar Borde
            if (objJuego.Fichas[posX, posY].JugadorId == Constantes.NO_ASIGNADO)
            {
                //Pinta verde solo si cuando se coloca la ficha se logran puntos
                var fichas = GameLogic.LogicaJuego.FichasParaVoltear(objJuego);
                if (GameLogic.LogicaJuego.HayFichasParaVoltear(fichas))
                {
                    bordeFichaSeleccionada.Stroke = colorValido;
                    //Se translucen las fichas que podrian ser volteadas
                    for (int i = 0; i < 8; i++)
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            if (fichas[i, k] == 1)
                            {
                                objJuego.Fichas[i, k].Imagen.Opacity = Constantes.Ficha.Propiedades.OPACITY_FICHA_COLOCADA;
                            }
                        }
                    }
                }
                else
                    bordeFichaSeleccionada.Stroke = colorInvalido;
            }
            else
                bordeFichaSeleccionada.Stroke = colorInvalido;

            //PintarFicha
            if (objJuego.Fichas[posX, posY].JugadorId != Constantes.NO_ASIGNADO)
            {
                uri = new Uri(objJuego.Jugadores[objJuego.Fichas[posX, posY].JugadorId].Elemento.RutaImagenVictoria);
                imagen = new BitmapImage(uri);
                objJuego.Fichas[posX, posY].Imagen.Source = imagen;
            }
            else
            {
                uri = new Uri(objJuego.Jugadores[objJuego.JugadorIdTurno].Elemento.RutaImagenVictoria);
                imagen = new BitmapImage(uri);
                objJuego.Fichas[posX, posY].Imagen.Source = imagen;
            }
            objJuego.Fichas[posX, posY].Imagen.Visibility = Visibility.Visible;
            objJuego.Fichas[posX, posY].Imagen.Opacity = Constantes.Ficha.Propiedades.OPACITY;
        }
        #endregion

        #region ColocarFicha
        private void ColocarFicha()
        {
            if (objJuego.Fichas[objJuego.PosXFicha, objJuego.PosYFicha].JugadorId == Constantes.NO_ASIGNADO)
            {
                var fichasParaVoltear = GameLogic.LogicaJuego.FichasParaVoltear(objJuego);

                if (GameLogic.LogicaJuego.HayFichasParaVoltear(fichasParaVoltear))
                {
                    DeshabilitarControlesJugadorActual();
                    PonerFichaLugarAccion();
                    VoltearFichas(fichasParaVoltear);
                    ActualizarInfoFichas();
                    IniciarSiguienteTurno();
                }
            }
        }

        private void PonerFichaLugarAccion()
        {
            Uri uri;
            BitmapImage imagen;
            uri = new Uri(objJuego.Jugadores[objJuego.JugadorIdTurno].Elemento.RutaImagenVictoria);
            imagen = new BitmapImage(uri);
        }

        private void VoltearFichas(int[,] fichas)
        {
            Uri uri;
            BitmapImage imagen;
            uri = new Uri(objJuego.Jugadores[objJuego.JugadorIdTurno].Elemento.RutaImagenVictoria);
            imagen = new BitmapImage(uri);
            //Se pone la ficha en el lugar que selecciono el jugador
            objJuego.Fichas[objJuego.PosXFicha, objJuego.PosYFicha].JugadorId = objJuego.JugadorIdTurno;
            objJuego.Fichas[objJuego.PosXFicha, objJuego.PosYFicha].Imagen.Source = imagen;

            //Se voltean las fichas
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (fichas[i, k] == 1)
                    {
                        objJuego.Fichas[i, k].JugadorId = objJuego.JugadorIdTurno;
                        objJuego.Fichas[i, k].ElementoId = objJuego.Jugadores[objJuego.JugadorIdTurno].Elemento.ElementoId;
                        objJuego.Fichas[i, k].Imagen.Source = imagen;
                        objJuego.Fichas[i, k].Imagen.Opacity = 1;
                    }
                }
            }
        }

        private async void DeshabilitarControlesJugadorActual()
        {
            await App.objSDK.UnicastPing(new HostName(objJuego.Jugadores[objJuego.JugadorIdTurno].Ip),
                    Constantes.Mensajes.Juego.DeshabilitarControles);
        }
        #endregion

        #region Cambio de Turno
        private void IniciarSiguienteTurno()
        {
            //Verificar que aun se puedan voltear fichas
            objJuego.CambiarTurno();
            if (VerificarAunSePuedenVoltearFichas())
            {
                PonerFichaDeMovimiento();
                ComunicarJugadoresTurno();
            }
            else
            {
                FinalizarJuego();
            }
        }

        private bool VerificarAunSePuedenVoltearFichas()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (objJuego.Fichas[i, k].JugadorId == Constantes.NO_ASIGNADO)
                    {
                        var fichas = GameLogic.LogicaJuego.FichasParaVoltear(objJuego, i, k);
                        if (GameLogic.LogicaJuego.HayFichasParaVoltear(fichas))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void PonerFichaDeMovimiento()
        {
            objJuego.Fichas[objJuego.PosXFicha, objJuego.PosYFicha].Imagen.Opacity = Constantes.Ficha.Propiedades.OPACITY;
            bordeFichaSeleccionada.Stroke = colorInvalido;
        }

        private async void ComunicarJugadoresTurno()
        {
            var jugadorId = objJuego.JugadorIdTurno;
            var jugadorIdRival = jugadorId == 0 ? 1 : 0;

            //Se vuelve a enviar por si no llego a deshabilitarse los controles
            await App.objSDK.UnicastPing(new HostName(objJuego.Jugadores[jugadorIdRival].Ip),
                    Constantes.Mensajes.Juego.DeshabilitarControles);

            //Se envia dos veces para asegurarse que reciba el mensaje
            await App.objSDK.UnicastPing(new HostName(objJuego.Jugadores[jugadorId].Ip),
                    Constantes.Mensajes.Juego.HabilitarControles);
            await App.objSDK.UnicastPing(new HostName(objJuego.Jugadores[jugadorId].Ip),
                    Constantes.Mensajes.Juego.HabilitarControles);
        }
        #endregion

        #region FIN DEL JUEGO
        private void FinalizarJuego(bool jugadorSalio = false)
        {
            var ganador = objJuego.NroFichasJugador1 == objJuego.NroFichasJugador2 ? -1 : objJuego.NroFichasJugador1 >= objJuego.NroFichasJugador2 ? 1 : 0;

            Helper.MensajeOk((jugadorSalio ? "Uno de los jugadores ha salido del juego. Se da por terminada la partida. " : "") +
                "El juega ha finalizado. GANADOR: " + ganador);
        }
        #endregion
    }
}
