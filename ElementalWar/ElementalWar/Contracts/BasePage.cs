using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ElementalWar.Contracts
{
    public abstract class BasePage : Page
    {
        public Jugador objJugador { get; set; }
    }
}
