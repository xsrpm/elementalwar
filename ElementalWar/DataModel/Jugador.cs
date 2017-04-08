namespace DataModel
{
    public class Jugador
    {
        public int JugadorId { get; set; }
        public string Ip { get; set; }
        public string Nombre { get; set; }
        public byte[] Imagen { get; set; }
        public Elemento Elemento { get; set; }

        public string MesaIp { get; set; }

        public Jugador()
        {
            LimpiarData();
        }

        public void LimpiarData()
        {
            Ip = "";
            Nombre = "";
            Imagen = null;
            Elemento = new Elemento { ElementoId = -1 };
        }
    }
}
