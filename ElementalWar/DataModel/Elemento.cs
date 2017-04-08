using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Elemento
    {
        public int ElementoId { get; set; }
        public string Nombre { get; set; }
        public string RutaImagen { get; set; }
        public string RutaImagenVictoria { get; set; }
        public string RutaImagenDerrota { get; set; }
        public string RutaImagenFondo { get; set; }
    }
}
