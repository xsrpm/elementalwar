using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            objJuego.Jugadores[jugadorId].Elemento = objJuego.Jugadores[jugadorRivalId].Elemento + 1;
            if (objJuego.Jugadores[jugadorId].Elemento == Constantes.Elementos.Aire)
                objJuego.Jugadores[jugadorId].Elemento = Constantes.Elementos.Agua;

            return objJuego.Jugadores[jugadorId];
        }

        public static bool MesaEliminarJugador(Juego objJuego, int jugadorId)
        {
            //Verificar que el jugador esta en la mesa
            if (objJuego.Jugadores.FirstOrDefault(x => x.JugadorId == jugadorId) == null)
                return false;

            //Eliminar al jugadir de la mesa
            objJuego.Jugadores.First(x => x.JugadorId == jugadorId).LimpiarData();
            return true;
        }
    }
}
