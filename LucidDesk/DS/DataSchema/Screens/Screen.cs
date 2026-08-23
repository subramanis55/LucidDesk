using System.Drawing;

namespace LucidDesK.DS.DataSchema.Screens
{
    public class Screen
    {
        public bool IsPrimaryScreen { set; get; }
        public Rectangle Bounds { set; get; }
        public string DeviceName { set; get; }
    }
}
