using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LucidDesk.Manager.Database
{
    internal static class UnIdentifiedLocalDeskManager
    {

        public static List<Desk> UnIdentifiedDesk;
        public static void Intialize()
        {
            UnIdentifiedDesk = getUnIdentifiedDesk();
        }

        public static List<Desk> getUnIdentifiedDesk()
        {
            List<Desk> desks = new List<Desk>();
            List<(string IpAddress, string HostName)> desk = GetLocalNetworkDevices();
            foreach (var desk_ in desk)
            {
                 if(!DeskProfileManager.ContainsDisplayID(desk_.IpAddress))
                desks.Add(new Desk() { IPAddress = desk_.IpAddress, HostName = desk_.HostName });
            }
            return desks;
        }

        private static List<(string IpAddress, string HostName)> GetLocalNetworkDevices()
        {
            var results = new ConcurrentBag<(string IpAddress, string HostName)>();
            // Get local IPv4
            var localIp = Dns.GetHostAddresses(Dns.GetHostName())
                             .First(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            // Assume /24 subnet (most home routers)
            var subnet = localIp.ToString().Substring(0, localIp.ToString().LastIndexOf('.') + 1);
            Parallel.For(1, 255, i =>
            {
                string ip = subnet + i;
                try
                {
                    var ping = new Ping();
                    var reply = ping.Send(ip, 100);
                    if (reply.Status == IPStatus.Success)
                    {
                        string hostName = string.Empty;
                        try
                        {
                            var entry = Dns.GetHostEntry(ip);
                            hostName = entry.HostName;
                        }
                        catch
                        {
                            hostName = "Unknown";
                        }
                        results.Add((IpAddress: ip, HostName: hostName));
                    }
                }
                catch
                {
                    // ignore unreachable hosts
                }
            });
            return results.ToList();
        }
    }
}
