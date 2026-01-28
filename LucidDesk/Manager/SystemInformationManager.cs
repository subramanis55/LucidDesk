using LucidDesk.Manager.Enum;
using LucidDesk.Settings;
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
using Microsoft.Win32;
using System.Windows.Forms;
namespace LucidDesk.Manager
{
    public static class SystemInformationManager
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);
        private static string macAddress, ipAddress, hostname;
        private const int SPI_GETDESKWALLPAPER = 0x0073;
        private const int MAX_PATH = 260;
        private static BitmapImage desktopWallpaper;

        public static double ScreenWidth = SystemInformation.VirtualScreen.Width;

        public static double ScreenHeight = SystemInformation.VirtualScreen.Height;

        public static string ApplicationName => SettingsManager.Settings.ApplicationName;
        public static string HostName
        {
            get
            {
                if (hostname == null)
                    hostname = GetHostName();
                return hostname;
            }
        }
        public static string MacAddress
        {
            get
            {
                if (macAddress == null)
                    macAddress = GetMacAddress();
                return macAddress;
            }
        }
        public static string IpAddresss
        {
            get
            {
                if (ipAddress == null)
                    ipAddress = GetLocalIPAddress();
                return ipAddress;
            }
        }

        public static BitmapImage DesktopWallpaper
        {
            get
            {
                if (desktopWallpaper == null)
                    desktopWallpaper = GetDesktopWallpaper();
                return desktopWallpaper;
            }
        }


        public static string GetHostName()
        {
            return Environment.MachineName;
        }
        public static string GetPcUserName()
        {
            return Environment.UserName;
        }
        public static string GetOsName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "MacOs";
            }
            return "UnknownOs";
        }

        public static string GetLocalIPAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up)
                    continue;

                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                        !IPAddress.IsLoopback(ip.Address))
                    {
                        return ip.Address.ToString();
                    }
                }
            }
            return IPAddress.Loopback.ToString();
        }

        public static string GetMacAddress()
        {
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Skip virtual, tunnel, loopback interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                    nic.NetworkInterfaceType == NetworkInterfaceType.Tunnel ||
                    nic.Description.ToLower().Contains("virtual") ||
                    nic.Description.ToLower().Contains("vmware") ||
                    nic.Description.ToLower().Contains("hyper-v"))
                    continue;

                string mac = nic.GetPhysicalAddress().ToString();

                // Physical MAC is always 12+ characters
                if (!string.IsNullOrEmpty(mac) && mac.Length >= 12)
                {
                    return mac;
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
            catch (Exception e) { }
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

                }
            }
            return null;
        }

        public static string GetFromRegistry()
        {
            var value = Registry.CurrentUser
                .OpenSubKey($"Software\\{SystemInformationManager.ApplicationName}\\")
                ?.GetValue("DeviceGuid");

            if (value != null && Guid.TryParse(value.ToString(), out Guid guid))
                return guid.ToString();

            Guid newGuid = Guid.NewGuid();
            Registry.CurrentUser
                .CreateSubKey($"Software\\{SystemInformationManager.ApplicationName}\\")
                .SetValue("DeviceGuid", newGuid.ToString());
            return newGuid.ToString();
        }

        internal static void Refresh()
        {
            ipAddress = null;
            macAddress = null;
            hostname = null;    
        }
    }
}
