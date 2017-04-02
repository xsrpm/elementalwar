using System;
using System.Collections.Generic;
using Windows.UI.Popups;

namespace Util
{
    public static class Helper
    {
        public static async void MensajeOk(string strMensaje)
        {
            MessageDialog msgDialog = new MessageDialog(strMensaje, Constantes.MessageDialogTitle);
            UICommand okBtn = new UICommand("OK");
            msgDialog.Commands.Add(okBtn);
            await msgDialog.ShowAsync();
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random((int)DateTime.Now.Ticks);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static class IntUtil
        {
            private static Random random;

            private static void Init()
            {
                if (random == null) random = new Random();
            }

            public static int Random(int min, int max)
            {
                Init();
                return random.Next(min, max);
            }
        }
    }
}
