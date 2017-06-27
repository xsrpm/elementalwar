using DataModel;
using Windows.Networking;
using Util;
using System.Threading.Tasks;

namespace UnitTest
{
    public class ReplicaMetodo
    {
        //Replica del método de notificación de turnos acomodado para poder ser probado
        public async Task ComunicarJugadoresTurno(Juego objJuego)
        {
            var jugadorId = objJuego.JugadorIdTurno;
            var jugadorRivalId = jugadorId == 0 ? 1 : 0;

            await ElementalWar.App.objSDK.ConnectStreamSocket(new HostName(objJuego.Jugadores[jugadorRivalId].Ip));
            await ElementalWar.App.objSDK.StreamPing(Constantes.Mensajes.Juego.DeshabilitarControles);

            await ElementalWar.App.objSDK.ConnectStreamSocket(new HostName(objJuego.Jugadores[jugadorId].Ip));
            await ElementalWar.App.objSDK.StreamPing(Constantes.Mensajes.Juego.HabilitarControles);
        }

        public bool VerificarAunSePuedenVoltearFichas(Juego objJuego, int[,] fichas)
        {
            for (int i = 1; i < 9; i++)
            {
                for (int k = 1; k < 9; k++)
                {
                    if (objJuego.Fichas[i, k].JugadorId == Constantes.NO_ASIGNADO)
                    {
                        if (GameLogic.LogicaJuego.HayFichasParaVoltear(fichas))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
