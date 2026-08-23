using System.Collections.Generic;

namespace LucidDesK.DS.DataSchema.Screens

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
