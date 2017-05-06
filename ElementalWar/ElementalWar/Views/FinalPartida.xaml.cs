﻿using DataModel;
using System;
using Util;
using Windows.Graphics.Display;
using Windows.Networking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ElementalWar.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FinalPartida : Page
    {
        private Juego objJuego;

        public FinalPartida()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            //if (e.Parameter != null)
            //{
            //    objJuego = (Juego)e.Parameter;
            //}

            //NotificarJugadoresResultado();
            //DibujarResultadosPartida();
        }

        private async void NotificarJugadoresResultado()
        {
            if (objJuego.JugadorIdGanador == Constantes.NO_ASIGNADO)
            {
                await App.objSDK.UnicastPing(new HostName(objJuego.Jugadores[0].Ip),
                    Constantes.Mensajes.Juego.FinJuegoEmpataste);
                await App.objSDK.UnicastPing(new HostName(objJuego.Jugadores[1].Ip),
                    Constantes.Mensajes.Juego.FinJuegoEmpataste);
            }
            else
            {
                var jugadorIdGanador = objJuego.JugadorIdGanador;
                var jugadorIdPerdedor = jugadorIdGanador == 0 ? 1 : 0;

                await App.objSDK.UnicastPing(new HostName(objJuego.Jugadores[jugadorIdGanador].Ip),
                    Constantes.Mensajes.Juego.FinJuegoGanaste);
                await App.objSDK.UnicastPing(new HostName(objJuego.Jugadores[jugadorIdPerdedor].Ip),
                    Constantes.Mensajes.Juego.FinJuegoPerdiste);
            }
        }

        private void DibujarResultadosPartida()
        {
            Uri uri;
            BitmapImage imagen;

            //Dibujar Fondos
            uri = new Uri(objJuego.Jugadores[0].Elemento.RutaImagenFondo);
            imagen = new BitmapImage(uri);
            //////////SETEAR FONDO 1
            uri = new Uri(objJuego.Jugadores[1].Elemento.RutaImagenFondo);
            imagen = new BitmapImage(uri);
            //////////SETEAR FONDO 2

            var mensajeResultado1 = "";
            var mensajeResultado2 = "";
            //Dibujar Elementos
            if (objJuego.JugadorIdGanador == 0)
            {
                uri = new Uri(objJuego.Jugadores[0].Elemento.RutaImagenVictoria);
                mensajeResultado1 = "GANASTE";
            }
            else
            {
                uri = new Uri(objJuego.Jugadores[0].Elemento.RutaImagenDerrota);
                mensajeResultado1 = "PERDISTE";
            }
            imagen = new BitmapImage(uri);
            //////////SETEAR IMAGEN JUGADOR 1
            if (objJuego.JugadorIdGanador == 1)
            {
                uri = new Uri(objJuego.Jugadores[1].Elemento.RutaImagenVictoria);
                mensajeResultado2 = "GANASTE";
            }
            else
            {
                uri = new Uri(objJuego.Jugadores[1].Elemento.RutaImagenDerrota);
                mensajeResultado2 = "PERDISTE";
            }
            imagen = new BitmapImage(uri);
            //////////SETEAR IMAGEN JUGADOR 2

            if (objJuego.JugadorIdGanador == Constantes.NO_ASIGNADO)
            {
                mensajeResultado1 = "EMPATE";
                mensajeResultado2 = "EMPATE";
            }
            //////////SETEAR TEXTO RESULTADO JUGADOR 1
            //////////SETEAR TEXTO RESULTADO JUGADOR 2
        }

        private void btnMenuPrincipal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MenuPrincipal));
        }
    }
}