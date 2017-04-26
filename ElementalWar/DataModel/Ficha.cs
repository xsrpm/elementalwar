using Windows.UI.Xaml.Controls;

namespace DataModel
{
    public class Ficha
    {
        public int JugadorId { get; set; }
        public int ElementoId { get; set; }
        public Image Imagen { get; set; }

        public Ficha()
        {
            JugadorId = -1;
            ElementoId = -1;
            Imagen = null;
        }
    }
}
