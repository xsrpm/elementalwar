using DataModel;
using System.Linq;
using Util;

namespace GameLogic
{
    public static class LogicaMesaEnEspera
    {
        public static void SetearMesa(Juego objJuego, string Ip)
        {
            objJuego.Ip = Ip;
            var numeroMesa = Ip.Split('.');
            if (numeroMesa.Length == 4)
            {
                objJuego.Codigo = numeroMesa[2] + numeroMesa[3];
            }
        }

        public static Jugador MesaAgregarJugador(Juego objJuego, Jugador objJugador)
        {
            //Verificar si el jugador ya está en la mesa
            if (objJuego.Jugadores.FirstOrDefault(x => x.Ip == objJugador.Ip) != null)
                return objJuego.Jugadores.Where(x => x.Ip == objJugador.Ip).First();

            //Verificar que la mesa no esté llena
            if (objJuego.Jugadores.Count(x => x.Ip != "") >= 2)
                return null;

            int jugadorId = -1;
            int jugadorRivalId = -1;
            //Unir al jugador a la mesa
            if (objJuego.Jugadores[0].Ip == "")
            {
                jugadorId = 0;
                jugadorRivalId = 1;
            }
            else if (objJuego.Jugadores[1].Ip == "")
            {
                jugadorId = 1;
                jugadorRivalId = 0;
            }
            objJugador.JugadorId = jugadorId;
            objJuego.Jugadores[jugadorId] = objJugador;

            //Seleccionar el elemento incial del jugador
            objJuego.Jugadores[jugadorId].Elemento.ElementoId = Constantes.Elementos.Agua;
            if (objJuego.Jugadores[jugadorRivalId].Elemento.ElementoId == Constantes.Elementos.Agua)
                objJuego.Jugadores[jugadorId].Elemento.ElementoId = Constantes.Elementos.Fuego;

            return objJuego.Jugadores[jugadorId];
        }

        public static bool MesaEliminarJugador(Juego objJuego, string jugadorIp)
        {
            //Verificar que el jugador esta en la mesa
            if (objJuego.Jugadores.FirstOrDefault(x => x.Ip == jugadorIp) == null)
                return false;

            //Eliminar al jugador de la mesa
            objJuego.Jugadores.First(x => x.Ip == jugadorIp).LimpiarData();
            return true;
        }

        public static bool CambiarFichaJugador(Juego objJuego, int jugadorId, int jugadorRivalId, int direccion)
        {
            bool huboCambio = false;

            if (direccion == Constantes.Mensajes.Juego.AccionMando.Izquierda)
            {
                objJuego.Jugadores[jugadorId].Elemento.ElementoId--;
                if (objJuego.Jugadores[jugadorId].Elemento.ElementoId < 0)
                {
                    objJuego.Jugadores[jugadorId].Elemento.ElementoId = 3;
                }
                if (objJuego.Jugadores[jugadorId].Elemento.ElementoId == objJuego.Jugadores[jugadorRivalId].Elemento.ElementoId)
                {
                    objJuego.Jugadores[jugadorId].Elemento.ElementoId--;
                }
                if (objJuego.Jugadores[jugadorId].Elemento.ElementoId < 0)
                {
                    objJuego.Jugadores[jugadorId].Elemento.ElementoId = 3;
                }
                huboCambio = true;
            }
            else if (direccion == Constantes.Mensajes.Juego.AccionMando.Derecha)
            {
                objJuego.Jugadores[jugadorId].Elemento.ElementoId++;
                if (objJuego.Jugadores[jugadorId].Elemento.ElementoId > 3)
                {
                    objJuego.Jugadores[jugadorId].Elemento.ElementoId = 0;
                }
                if (objJuego.Jugadores[jugadorId].Elemento.ElementoId == objJuego.Jugadores[jugadorRivalId].Elemento.ElementoId)
                {
                    objJuego.Jugadores[jugadorId].Elemento.ElementoId++;
                }
                if (objJuego.Jugadores[jugadorId].Elemento.ElementoId > 3)
                {
                    objJuego.Jugadores[jugadorId].Elemento.ElementoId = 0;
                }
                huboCambio = true;
            }

            return huboCambio;
        }

        public static void SetearElementoJugador(Juego objJuego, int jugadorId)
        {
            var elementoId = objJuego.Jugadores[jugadorId].Elemento.ElementoId;

            switch (elementoId)
            {
                case Constantes.Elementos.Agua:
                    objJuego.Jugadores[jugadorId].Elemento.Nombre = "Agua";
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagen = Constantes.Elementos.Imagenes.Agua;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria = Constantes.Elementos.Imagenes.Victoria.Agua;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenDerrota = Constantes.Elementos.Imagenes.Derrota.Agua;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenFondo = Constantes.Elementos.Imagenes.Fondo.Agua;
                    break;
                case Constantes.Elementos.Fuego:
                    objJuego.Jugadores[jugadorId].Elemento.Nombre = "Fuego";
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagen = Constantes.Elementos.Imagenes.Fuego;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria = Constantes.Elementos.Imagenes.Victoria.Fuego;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenDerrota = Constantes.Elementos.Imagenes.Derrota.Fuego;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenFondo = Constantes.Elementos.Imagenes.Fondo.Fuego;
                    break;
                case Constantes.Elementos.Tierra:
                    objJuego.Jugadores[jugadorId].Elemento.Nombre = "Tierra";
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagen = Constantes.Elementos.Imagenes.Tierra;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria = Constantes.Elementos.Imagenes.Victoria.Tierra;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenDerrota = Constantes.Elementos.Imagenes.Derrota.Tierra;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenFondo = Constantes.Elementos.Imagenes.Fondo.Tierra;
                    break;
                case Constantes.Elementos.Aire:
                    objJuego.Jugadores[jugadorId].Elemento.Nombre = "Aire";
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagen = Constantes.Elementos.Imagenes.Aire;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria = Constantes.Elementos.Imagenes.Victoria.Aire;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenDerrota = Constantes.Elementos.Imagenes.Derrota.Aire;
                    objJuego.Jugadores[jugadorId].Elemento.RutaImagenFondo = Constantes.Elementos.Imagenes.Fondo.Aire;
                    break;
            }
        }
    }
}
