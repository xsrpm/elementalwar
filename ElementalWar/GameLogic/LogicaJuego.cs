using DataModel;
using Util;

namespace GameLogic
{
    public static class LogicaJuego
    {
        public static void AsignarTurnos(Juego objJuego)
        {
            objJuego.Jugadores.Shuffle();
            for (int i = 0; i < objJuego.Jugadores.Count; i++)
                objJuego.Jugadores[i].JugadorId = i;
        }

        public static bool MovimientoValido(int posX, int posY, int direccion)
        {
            bool valido = false;

            switch (direccion)
            {
                case Constantes.Mensajes.Juego.AccionMando.Izquierda:
                    if (posX - 1 >= 1)
                        valido = true;
                    break;
                case Constantes.Mensajes.Juego.AccionMando.Arriba:
                    if (posY - 1 >= 1)
                        valido = true;
                    break;
                case Constantes.Mensajes.Juego.AccionMando.Derecha:
                    if (posX + 1 <= 8)
                        valido = true;
                    break;
                case Constantes.Mensajes.Juego.AccionMando.Abajo:
                    if (posY + 1 <= 8)
                        valido = true;
                    break;
            }

            return valido;
        }

        public static int[,] FichasParaVoltear(Juego objJuego)
        {
            return FichasParaVoltear(objJuego, objJuego.PosXFicha, objJuego.PosYFicha);
        }

        public static int[,] FichasParaVoltear(Juego objJuego, int posX, int posY)
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

            var jugadorId = objJuego.JugadorIdTurno;
            var jugadorIdRival = jugadorId == 0 ? 1 : 0;
            var cont = 0;

            #region Horizontal Izquierda
            cont = 0;
            for (int i = posX - 1; i >= 0; i--)
            {
                if (objJuego.Fichas[i, posY].JugadorId == jugadorIdRival)
                    cont++;
                else
                {
                    if (objJuego.Fichas[i, posY].JugadorId != jugadorId)
                    {
                        cont = 0;
                    }
                    break;
                }
            }
            if (cont > 0)
            {
                for (int i = posX - 1; i >= posX - cont; i--)
                {
                    fichas[i, posY] = 1;
                }
            }
            #endregion

            #region Horizontal Derecha
            cont = 0;
            for (int i = posX + 1; i < 10; i++)
            {
                if (objJuego.Fichas[i, posY].JugadorId == jugadorIdRival)
                    cont++;
                else
                {
                    if (objJuego.Fichas[i, posY].JugadorId != jugadorId)
                    {
                        cont = 0;
                    }
                    break;
                }
            }
            if (cont > 0)
            {
                for (int i = posX + 1; i <= posX + cont; i++)
                {
                    fichas[i, posY] = 1;
                }
            }
            #endregion

            #region Vertical Arriba
            cont = 0;
            for (int i = posY - 1; i >= 0; i--)
            {
                if (objJuego.Fichas[posX, i].JugadorId == jugadorIdRival)
                    cont++;
                else
                {
                    if (objJuego.Fichas[posX, i].JugadorId != jugadorId)
                    {
                        cont = 0;
                    }
                    break;
                }
            }
            if (cont > 0)
            {
                for (int i = posY - 1; i >= posY - cont; i--)
                {
                    fichas[posX, i] = 1;
                }
            }
            #endregion

            #region Vertical Abajo
            cont = 0;
            for (int i = posY + 1; i < 10; i++)
            {
                if (objJuego.Fichas[posX, i].JugadorId == jugadorIdRival)
                    cont++;
                else
                {
                    if (objJuego.Fichas[posX, i].JugadorId != jugadorId)
                    {
                        cont = 0;
                    }
                    break;
                }
            }
            if (cont > 0)
            {
                for (int i = posY + 1; i <= posY + cont; i++)
                {
                    fichas[posX, i] = 1;
                }
            }
            #endregion

            #region Diagonal Abajo Derecha
            cont = 0;
            for (int i = posX + 1, k = posY + 1; i < 10 && k < 10; i++, k++)
            {
                if (objJuego.Fichas[i, k].JugadorId == jugadorIdRival)
                    cont++;
                else
                {
                    if (objJuego.Fichas[i, k].JugadorId != jugadorId)
                    {
                        cont = 0;
                    }
                    break;
                }
            }
            if (cont > 0)
            {
                for (int i = posX + 1, k = posY + 1; i <= posX + cont && k <= posY + cont; i++, k++)
                {
                    fichas[i, k] = 1;
                }
            }
            #endregion

            #region Diagonal Abajo Izquierda
            cont = 0;
            for (int i = posX - 1, k = posY + 1; i >= 0 && k < 10; i--, k++)
            {
                if (objJuego.Fichas[i, k].JugadorId == jugadorIdRival)
                    cont++;
                else
                {
                    if (objJuego.Fichas[i, k].JugadorId != jugadorId)
                    {
                        cont = 0;
                    }
                    break;
                }
            }
            if (cont > 0)
            {
                for (int i = posX - 1, k = posY + 1; i >= posX - cont && k <= posY + cont; i--, k++)
                {
                    fichas[i, k] = 1;
                }
            }
            #endregion

            #region Diagonal Arriba Derecha
            cont = 0;
            for (int i = posX + 1, k = posY - 1; i < 10 && k >= 0; i++, k--)
            {
                if (objJuego.Fichas[i, k].JugadorId == jugadorIdRival)
                    cont++;
                else
                {
                    if (objJuego.Fichas[i, k].JugadorId != jugadorId)
                    {
                        cont = 0;
                    }
                    break;
                }
            }
            if (cont > 0)
            {
                for (int i = posX + 1, k = posY - 1; i <= posX + cont && k >= posY - cont; i++, k--)
                {
                    fichas[i, k] = 1;
                }
            }
            #endregion

            #region Diagonal Arriba Izquierda
            cont = 0;
            for (int i = posX - 1, k = posY - 1; i >= 0 && k >= 0; i--, k--)
            {
                if (objJuego.Fichas[i, k].JugadorId == jugadorIdRival)
                    cont++;
                else
                {
                    if (objJuego.Fichas[i, k].JugadorId != jugadorId)
                    {
                        cont = 0;
                    }
                    break;
                }
            }
            if (cont > 0)
            {
                for (int i = posX - 1, k = posY - 1; i >= posX - cont && k >= posY - cont; i--, k--)
                {
                    fichas[i, k] = 1;
                }
            }
            #endregion

            return fichas;
        }

        public static bool HayFichasParaVoltear(int[,] fichas)
        {
            var hayFichasParaVoltear = false;

            for (int i = 0; i < 10; i++)
            {
                for (int k = 0; k < 10; k++)
                {
                    if (fichas[i, k] == 1)
                    {
                        hayFichasParaVoltear = true;
                        break;
                    }
                }
            }

            return hayFichasParaVoltear;
        }
    }
}
