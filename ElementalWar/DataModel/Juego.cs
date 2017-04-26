using System.Collections.Generic;

namespace DataModel
{
    public class Juego
    {
        public string Codigo { get; set; }
        public string Ip { get; set; }

        public List<Jugador> Jugadores { get; set; }

        public Ficha[,] Fichas { get; set; }

        public int NroFichasJugador1 { get; set; }
        public int NroFichasJugador2 { get; set; }

        public Juego()
        {
            Jugadores = new List<Jugador>
            {
                new Jugador { JugadorId = 0, Nombre = "Jugador 1"},
                new Jugador { JugadorId = 1, Nombre = "Jugador 2"}
            };
            Fichas = new Ficha[8, 8]
            {
                {new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha() },
                {new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha() },
                {new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha() },
                {new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha() },
                {new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha() },
                {new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha() },
                {new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha() },
                {new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha(), new Ficha() }
            };
        }

        public void ActualizarInfoFichas()
        {
            NroFichasJugador1 = 0;
            NroFichasJugador2 = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (Fichas[i, k].JugadorId == 0)
                        NroFichasJugador1++;
                    else if (Fichas[i, k].JugadorId == 1)
                        NroFichasJugador2++;
                }
            }
        }
    }
}
