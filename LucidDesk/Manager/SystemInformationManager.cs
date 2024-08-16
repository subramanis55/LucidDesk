using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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
        public static string HostName;
        public static string MacAddress;
        public static string IpAddresss;
        public static BitmapImage DesktopWallpaper;
        public static string GetHostName()
        {
            return Environment.MachineName;
        }
        public static string GetPcUserName(){
            return Environment.UserName;
        }
        public static string GetOsName(){
          if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
                return "Windows";
          }
           if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
                return "Linux";
            }
            if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX)){
                return "MacOs";
            }
            return "UnknownOs";
        }
        public static string GetMacAddress()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up)
                {
                    var macAddress = networkInterface.GetPhysicalAddress().ToString();
                    if (!string.IsNullOrEmpty(macAddress))
                    {
                        return macAddress;
                    }
                }
            }
            return null;
        }
        public static string GetPcIPAddress(string hostName)
        {
            try
            {
                IPAddress[] iparray = Dns.GetHostAddresses(hostName);
                foreach (IPAddress ip in iparray)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch(Exception e) { }
            return "";
        }
        public static string GetIpAddresss(string macAddress)
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                var physicalAddress = networkInterface.GetPhysicalAddress().ToString();
                if (string.Equals(physicalAddress, macAddress.Replace("-", ""), StringComparison.OrdinalIgnoreCase))
                {
                    var ipProperties = networkInterface.GetIPProperties();
                    foreach (var unicastAddress in ipProperties.UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return unicastAddress.Address.ToString();
                        }
                    }
                }
            }
            return "IP Address not found for the given MAC Address";
        }
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
