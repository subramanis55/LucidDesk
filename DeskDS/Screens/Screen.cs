using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskDS.Screens
{
    public class Screen
    {
        public bool IsPrimaryScreen { set; get; }
        public Rectangle Bounds { set; get; }
        public string DeviceName { set; get; }
    }
}
