using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace iiFramework.Util
{
    public class DNSHelper
    {
        public static IPAddress[] GetAddressList()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList;
        }

        public static IPAddress[] GetInterNetworks()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            List<IPAddress> interNetworkList = new List<IPAddress>();
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    interNetworkList.Add(ip);
                }
            }
            return interNetworkList.ToArray();
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
