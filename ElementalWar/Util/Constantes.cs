namespace Util
{
    public static class Constantes
    {
        public static string FILE_NOMBRE_JUGADOR = "nombreJugador.txt";
        public static string FILE_IMAGEN_JUGADOR = "imagenJugador.txt";
        public static string SEPARADOR = ";#;#";
        public static string MessageDialogTitle = "Elemental War";

        public static string MULTICAST_ADDRESS = "239.10.2.78";
        public static string MULTICAST_SERVICE_PORT = "54330";
        public static string UNICAST_SERVICE_PORT = "22111";
        public static string STREAM_SERVICE_PORT = "22110";
        public static int DELAY = 500;

        public const int NO_ASIGNADO = -1;

        public struct Colores
        {
            public struct PanelTurno
            {
                public const string COLORMANDOACTIVO = "#FF01FE0F";
                public const string COLORMANDOINACTIVO = "#FFFF0437";
            }
        }

        public struct Imagenes
        {
            public const string SIN_IMAGEN = "Ninguno";
            public const string Jugador = "ms-appx:///Assets/PC/Roles/Jugador.png";
        }

        #region Mensajes en el Juego
        public struct Mensajes
        {
            public struct UnirseMesa
            {
                public const string SolicitudUnirse = "SolicitudUnirse";
                public const string ConfirmacionUnirse = "ConfirmacionUnirse";
                public const string EnviarImagenJugador = "EnviarImagenJugador";
            }

            public struct Juego
            {
                public const string JugadorSaleMesa = "JugadorSaleMesa";
                public const string MesaIndicaSeCierra = "MesaIndicaSeCierra";

                public const string MovimientoJugador = "MovimientoJugador";
                public const string DeshabilitarControles = "InhabilitarControles";
                public const string HabilitarControles = "HabilitarControles";

                public const string MesaIndicaJuegoInicia = "MesaIndicaJuegoInicia";

                public const string FinJuegoGanaste = "FinJuegoGanaste";
                public const string FinJuegoPerdiste = "FinJuegoPerdiste";
                public const string FinJuegoEmpataste = "FinJuegoEmpataste";

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

        public struct Ficha
        {
            public struct Propiedades
            {
                public const double OPACITY = 0.7;
                public const double OPACITY_FICHA_COLOCADA = 0.3;
            }
        }

        public struct Elementos
        {
            public const int SIN_ELEMENTO = -1;
            public const int Agua = 0;
            public const int Fuego = 1;
            public const int Tierra = 2;
            public const int Aire = 3;

            public struct Imagenes
            {
                public const string Agua = "ms-appx:///Assets/Elementos/Agua.png";
                public const string Fuego = "ms-appx:///Assets/Elementos/Fuego.png";
                public const string Tierra = "ms-appx:///Assets/Elementos/Tierra.png";
                public const string Aire = "ms-appx:///Assets/Elementos/Aire.png";

                public struct Victoria
                {
                    public const string Agua = "ms-appx:///Assets/Elementos/Victoria/Agua.png";
                    public const string Fuego = "ms-appx:///Assets/Elementos/Victoria/Fuego.png";
                    public const string Tierra = "ms-appx:///Assets/Elementos/Victoria/Tierra.png";
                    public const string Aire = "ms-appx:///Assets/Elementos/Victoria/Aire.png";
                }
                public struct Derrota
                {
                    public const string Agua = "ms-appx:///Assets/Elementos/Derrota/Agua.png";
                    public const string Fuego = "ms-appx:///Assets/Elementos/Derrota/Fuego.png";
                    public const string Tierra = "ms-appx:///Assets/Elementos/Derrota/Tierra.png";
                    public const string Aire = "ms-appx:///Assets/Elementos/Derrota/Aire.png";
                }
                public struct Fondo
                {
                    public const string Agua = "ms-appx:///Assets/Elementos/Fondo/Agua.png";
                    public const string Fuego = "ms-appx:///Assets/Elementos/Fondo/Fuego.png";
                    public const string Tierra = "ms-appx:///Assets/Elementos/Fondo/Tierra.png";
                    public const string Aire = "ms-appx:///Assets/Elementos/Fondo/Aire.png";
                }
            }
        }
    }
}
