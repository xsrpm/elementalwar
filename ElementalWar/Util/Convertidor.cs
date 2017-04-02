using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;

namespace Util
{
    public static class Convertidor
    {
        // read a text file from the app's local folder
        public static async Task<StorageFile> GetTextFile(string _filename)
        {
            // declare an empty variable to be filled later
            //string contents = null;
            // define where the file resides
            StorageFolder localfolder = ApplicationData.Current.LocalFolder;
            StorageFile textfile = null;
            // see if the file exists
            if (await localfolder.TryGetItemAsync(_filename) != null)
            {
                // open the file
                textfile = await localfolder.GetFileAsync(_filename);
                // read the file
                //contents = await FileIO.ReadTextAsync(textfile);
            }
            else
            {
                textfile = await localfolder.CreateFileAsync(_filename);
                //await FileIO.WriteTextAsync(textfile, "My text");
            }
            // return the contents of the file
            return textfile;
        }

        // read a text file from the app's local folder
        public static async Task<string> ReadTextFile(string _filename)
        {
            // declare an empty variable to be filled later
            string contents = null;
            // define where the file resides
            StorageFolder localfolder = ApplicationData.Current.LocalFolder;
            //StorageFile textfile = null;
            // see if the file exists
            if (await localfolder.TryGetItemAsync(_filename) != null)
            {
                // open the file
                StorageFile textfile = await localfolder.GetFileAsync(_filename);
                // read the file
                contents = await FileIO.ReadTextAsync(textfile);
            }
            // return the contents of the file
            return contents;
        }

        // read a text file from the app's local folder
        public static async Task<IBuffer> ReadBufferFile(string _filename)
        {
            // declare an empty variable to be filled later
            IBuffer contents = null;
            // define where the file resides
            StorageFolder localfolder = ApplicationData.Current.LocalFolder;
            //StorageFile textfile = null;
            // see if the file exists
            if (await localfolder.TryGetItemAsync(_filename) != null)
            {
                // open the file
                StorageFile textfile = await localfolder.GetFileAsync(_filename);
                // read the file
                contents = await FileIO.ReadBufferAsync(textfile);
            }
            // return the contents of the file
            return contents;
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static async Task<byte[]> ConvertImageToBytes(IRandomAccessStream myMemoryStream)
        {
            var reader = new DataReader(myMemoryStream.GetInputStreamAt(0));
            byte[] bytes = new byte[myMemoryStream.Size];
            await reader.LoadAsync((uint)myMemoryStream.Size);
            reader.ReadBytes(bytes);

            return bytes;
        }

        public static async Task<InMemoryRandomAccessStream> ConvertImageToStream(byte[] arr)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            await randomAccessStream.WriteAsync(arr.AsBuffer());
            randomAccessStream.Seek(0);
            return randomAccessStream;
        }

        public static SolidColorBrush GetSolidColorBrush(string hex)
        {
            hex = hex.Replace("#", string.Empty);
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            return myBrush;
        }
    }
}
