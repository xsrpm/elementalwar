using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Juego
    {
        public string Codigo { get; set; }
        public string Ip { get; set; }

        public List<Jugador> Jugadores { get; set; }

        public Juego()
        {
            Jugadores = new List<Jugador>
            {
                new Jugador(),
                new Jugador()
            };
        }
    }
}
