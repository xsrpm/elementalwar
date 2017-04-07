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

        public struct Colores
        {
            public struct PanelTurno
            {
                public const string COLORMANDOACTIVO = "#FF01FE0F";
                public const string COLORMANDOINACTIVO = "#FFFF0437";
            }
        }

        #region Mensajes para unirse a la mesa
        public struct Mensajes
        {
            public struct UnirseMesa
            {
                public const string SolicitudUnirse = "SolicitudUnirse";
                public const string ConfirmacionUnirse = "ConfirmacionUnirse";
            }

            public struct Juego
            {
                public const string JugadorSaleMesa = "JugadorSaleMesa";
                public const string MesaIndicaSeCierra = "MesaIndicaSeCierra";

                public const string MovimientoJugador = "MovimientoJugador";
                public const string DeshabilitarControles = "InhabilitarControles";
                public const string HabilitarControles = "HabilitarControles";

                public struct AccionMando
                {
                    public const int Izquierda = 1;
                    public const int Arriba = 2;
                    public const int Derecha = 3;
                    public const int Abajo = 4;
                    public const int Accion = 5;
                }
            }
        }
        #endregion

        public struct Elementos
        {
            public const int SIN_ELEMENTO = -1;
            public const int Agua = 0;
            public const int Fuego = 1;
            public const int Tierra = 2;
            public const int Aire = 3;
        }
    }
}
