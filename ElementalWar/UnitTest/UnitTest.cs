using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using DataModel;
using Util;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {
        //HU14
        [TestMethod]
        public void ComunicarTurno()
        {
            Juego objJuego = new Juego();

            int[,] fichas = new int[10, 10]
            {
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,1,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,1,0,0,0 },
                { 0,0,0,0,1,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,1,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 }
            };

            ReplicaMetodo replica = new ReplicaMetodo();

            //si aun hay piesas por voltear se comunica turno
            var resultado = replica.VerificarAunSePuedenVoltearFichas(objJuego, fichas);

            Assert.AreEqual(resultado, true);
        }

        //HU19
        [TestMethod]
        public void FinalizacionDelJuego()
        {
            int[,] fichas = new int[10, 10]
            {
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 }
            };

            //Si no hay fichas por voltear el juego termina
            bool terminaJuego = GameLogic.LogicaJuego.HayFichasParaVoltear(fichas);

            Assert.AreEqual(terminaJuego, false);
        }

        //HU18
        [TestMethod]
        public void CambioDeColorFicha()
        {
            Juego objJuego = new Juego();
            int jugadorId = 1;


            //Probando cambio elemento a agua
            objJuego.Jugadores[jugadorId].Elemento.ElementoId = Constantes.Elementos.Agua;

            GameLogic.LogicaMesaEnEspera.SetearElementoJugador(objJuego, jugadorId);

            var nombreAgua = objJuego.Jugadores[jugadorId].Elemento.Nombre;
            var rutaImagenAgua = objJuego.Jugadores[jugadorId].Elemento.RutaImagen;
            var rutaImagenVictoriaAgua = objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria;
            var rutaImagenDerrotaAgua = objJuego.Jugadores[jugadorId].Elemento.RutaImagenDerrota;
            var rutaImagenFondoAgua = objJuego.Jugadores[jugadorId].Elemento.RutaImagenFondo;

            Assert.AreEqual(nombreAgua, "Agua");
            Assert.AreEqual(rutaImagenAgua, Constantes.Elementos.Imagenes.Agua);
            Assert.AreEqual(rutaImagenVictoriaAgua, Constantes.Elementos.Imagenes.Victoria.Agua);
            Assert.AreEqual(rutaImagenDerrotaAgua, Constantes.Elementos.Imagenes.Derrota.Agua);
            Assert.AreEqual(rutaImagenFondoAgua, Constantes.Elementos.Imagenes.Fondo.Agua);


            //Probando cambio elemento a fuego
            objJuego.Jugadores[jugadorId].Elemento.ElementoId = Constantes.Elementos.Fuego;

            GameLogic.LogicaMesaEnEspera.SetearElementoJugador(objJuego, jugadorId);

            var nombreFuego = objJuego.Jugadores[jugadorId].Elemento.Nombre;
            var rutaImagenFuego = objJuego.Jugadores[jugadorId].Elemento.RutaImagen;
            var rutaImagenVictoriaFuego = objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria;
            var rutaImagenDerrotaFuego = objJuego.Jugadores[jugadorId].Elemento.RutaImagenDerrota;
            var rutaImagenFondoFuego = objJuego.Jugadores[jugadorId].Elemento.RutaImagenFondo;

            Assert.AreEqual(nombreFuego, "Fuego");
            Assert.AreEqual(rutaImagenFuego, Constantes.Elementos.Imagenes.Fuego);
            Assert.AreEqual(rutaImagenVictoriaFuego, Constantes.Elementos.Imagenes.Victoria.Fuego);
            Assert.AreEqual(rutaImagenDerrotaFuego, Constantes.Elementos.Imagenes.Derrota.Fuego);
            Assert.AreEqual(rutaImagenFondoFuego, Constantes.Elementos.Imagenes.Fondo.Fuego);


            //Probando cambio elemento a tierra
            objJuego.Jugadores[jugadorId].Elemento.ElementoId = Constantes.Elementos.Tierra;

            GameLogic.LogicaMesaEnEspera.SetearElementoJugador(objJuego, jugadorId);

            var nombreTierra = objJuego.Jugadores[jugadorId].Elemento.Nombre;
            var rutaImagenTierra = objJuego.Jugadores[jugadorId].Elemento.RutaImagen;
            var rutaImagenVictoriaTierra = objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria;
            var rutaImagenDerrotaTierra = objJuego.Jugadores[jugadorId].Elemento.RutaImagenDerrota;
            var rutaImagenFondoTierra = objJuego.Jugadores[jugadorId].Elemento.RutaImagenFondo;

            Assert.AreEqual(nombreTierra, "Tierra");
            Assert.AreEqual(rutaImagenTierra, Constantes.Elementos.Imagenes.Tierra);
            Assert.AreEqual(rutaImagenVictoriaTierra, Constantes.Elementos.Imagenes.Victoria.Tierra);
            Assert.AreEqual(rutaImagenDerrotaTierra, Constantes.Elementos.Imagenes.Derrota.Tierra);
            Assert.AreEqual(rutaImagenFondoTierra, Constantes.Elementos.Imagenes.Fondo.Tierra);


            //Probando cambio elemento a aire
            objJuego.Jugadores[jugadorId].Elemento.ElementoId = Constantes.Elementos.Aire;

            GameLogic.LogicaMesaEnEspera.SetearElementoJugador(objJuego, jugadorId);

            var nombreAire = objJuego.Jugadores[jugadorId].Elemento.Nombre;
            var rutaImagenAire = objJuego.Jugadores[jugadorId].Elemento.RutaImagen;
            var rutaImagenVictoriaAire = objJuego.Jugadores[jugadorId].Elemento.RutaImagenVictoria;
            var rutaImagenDerrotaAire = objJuego.Jugadores[jugadorId].Elemento.RutaImagenDerrota;
            var rutaImagenFondoAire = objJuego.Jugadores[jugadorId].Elemento.RutaImagenFondo;

            Assert.AreEqual(nombreAire, "Aire");
            Assert.AreEqual(rutaImagenAire, Constantes.Elementos.Imagenes.Aire);
            Assert.AreEqual(rutaImagenVictoriaAire, Constantes.Elementos.Imagenes.Victoria.Aire);
            Assert.AreEqual(rutaImagenDerrotaAire, Constantes.Elementos.Imagenes.Derrota.Aire);
            Assert.AreEqual(rutaImagenFondoAire, Constantes.Elementos.Imagenes.Fondo.Aire);
        }

        //HU15
        [TestMethod]
        public void ActivarDesactivarControles()
        {
            Juego objJuego = new Juego();

            int[,] fichas = new int[10, 10]
            {
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,1,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,1,0,0,0 },
                { 0,0,0,0,1,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 },
                { 0,0,0,0,1,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0,0,0 }
            };

            ReplicaMetodo replica = new ReplicaMetodo();

            //si aun hay piesas por voltear si activan/desactivan controles
            var resultado = replica.VerificarAunSePuedenVoltearFichas(objJuego, fichas);

            Assert.AreEqual(resultado, true);
        }

    }
}
