using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LucidDesk.Manager
{
    public static class SystemInformationManager
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);
        private const int SPI_GETDESKWALLPAPER = 0x0073;
        private const int MAX_PATH = 260;
        public static BitmapImage GetDesktopWallpaper()
        {
            StringBuilder wallpaper = new StringBuilder(MAX_PATH);
            if (SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, wallpaper, 0) != 0)
            {
                string wallpaperPath = wallpaper.ToString();
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(wallpaperPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load wallpaper image: " + ex.Message);
                    return null;
                }
            }
            else
            {
                MessageBox.Show("Failed to retrieve wallpaper path.");
                return null;
            }
        }
    }
}
