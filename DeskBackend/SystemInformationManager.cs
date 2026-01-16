using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using Settings;
using Settings.Enum;
namespace DeskBackend
{
    public static class SystemInformationManager
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);
        private static string macAddress, ipAddress, hostname;
        private const int SPI_GETDESKWALLPAPER = 0x0073;
        private const int MAX_PATH = 260;
        private static byte[] desktopWallpaper;
        private static double screenWidth = 0, screenHeight = 0;
        public static double ScreenWidth
        {
            get
            {
                if (screenWidth == 0)
                    screenWidth = GetSystemMetrics(78);
                return screenWidth;
            }
        }
        public static double ScreenHeight
        {
            get
            {
                if (screenHeight == 0)
                    screenHeight = GetSystemMetrics(79);
                return screenHeight;
            }
        }
       

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
        public static string ApplicationName => SettingsManager.Settings.ApplicationMode == ApplicationMode.Online ? "Desk" : "Local Desk";
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
                    ipAddress = GetIpAddresss(MacAddress);
                return ipAddress;
            }
        }

        public static byte[] DesktopWallpaper
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
        public static byte[]? GetDesktopWallpaper()
        {
            var wallpaper = new StringBuilder(MAX_PATH);

            if (SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, wallpaper, 0) == 0)
                return null;

            string path = wallpaper.ToString();

            if (string.IsNullOrWhiteSpace(path))
                return null;

            if (!File.Exists(path))
                return null;

            return File.ReadAllBytes(path);
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

    }
}
