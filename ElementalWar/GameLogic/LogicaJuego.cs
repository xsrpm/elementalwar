using DataModel;
using Util;

namespace GameLogic
{
    public static class LogicaJuego
    {
        public static void AsignarTurnos(Juego objJuego)
        {
            objJuego.Jugadores.Shuffle();
            for(int i = 0; i < objJuego.Jugadores.Count; i++)
                objJuego.Jugadores[i].JugadorId = i;
        }
    }
}
