using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskUI.Manager.Classes.DataSchema.Screens

{
    public class ScreenInformation
    {
        public int Count { get => Screens.Count; }

        public List<Screen> Screens = new List<Screen>();

        public void AddScreen(Screen screen)
        {
            Screens.Add(screen);
        }

    }
}
