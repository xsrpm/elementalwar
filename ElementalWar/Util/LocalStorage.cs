using DataModel;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;

namespace Util
{
    public static class LocalStorage
    {
        public static async Task<bool> GuardarDatosJugador(Jugador objJugador)
        {
            try
            {
                StorageFile file = await Convertidor.GetTextFile(Constantes.FILE_NOMBRE_JUGADOR);
                if (file != null)
                {
                    var cadena = "";
                    if (objJugador.Nombre != null && !objJugador.Nombre.Equals(""))
                        cadena = objJugador.Nombre;
                    await FileIO.WriteTextAsync(file, cadena);
                }
                StorageFile imageFile = await Convertidor.GetTextFile(Constantes.FILE_IMAGEN_JUGADOR);
                if (imageFile != null)
                {
                    byte[] imagen = null;
                    if (objJugador.Imagen != null)
                        imagen = objJugador.Imagen;
                    await FileIO.WriteBytesAsync(imageFile, imagen);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<Jugador> ObtenerDatosJugador()
        {
            try
            {
                string fileContent = await Convertidor.ReadTextFile(Constantes.FILE_NOMBRE_JUGADOR);
                if (fileContent != null)
                {
                    var jugador = new Jugador();
                    jugador.Nombre = fileContent;

                    try
                    {
                        var imageContent = await Convertidor.ReadBufferFile(Constantes.FILE_IMAGEN_JUGADOR);
                        if (imageContent != null)
                        {
                            jugador.Imagen = imageContent.ToArray();
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return jugador;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
