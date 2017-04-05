namespace Util
{
    public static class Constantes
    {
        public static string FILE_NOMBRE_JUGADOR = "nombreJugador.txt";
        public static string FILE_IMAGEN_JUGADOR = "imagenJugador.txt";
        public static string SEPARADOR = ";#;#";
        public static string MessageDialogTitle = "Elemental War";
        public static string SIN_IMAGEN = "Ninguno";

        public static string MULTICAST_ADDRESS = "239.10.2.78";
        public static string MULTICAST_SERVICE_PORT = "54330";
        public static string UNICAST_SERVICE_PORT = "22111";
        public static string STREAM_SERVICE_PORT = "22110";
        public static int DELAY = 500;

        #region Mensajes para unirse a la mesa
        public static string UnirseEnviameConfirmacion = "UnirseEnviameConfirmacion";
        public static string ConfirmacionUnirseMesa = "ConfirmacionUnirseMesa";
        public static string JugadorSaleMesa = "JugadorSaleMesa";
        public static string MesaIndicaSeCierra = "MesaIndicaSeCierra";
        //public static string MesaIndicaJuegoInicia = "MesaIndicaJuegoInicia";
        #endregion
    }
}
