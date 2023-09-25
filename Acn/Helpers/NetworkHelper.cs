using System.Net;
using System.Net.NetworkInformation;

namespace Acn.Helpers;

public static class NetworkHelper
{
    public static IPAddress GetIPMask(IPAddress ipAddress)
    {
        foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            IPInterfaceProperties ipProperties = adapter.GetIPProperties();

            for (int n = 0; n < ipProperties.UnicastAddresses.Count; n++)
            {
                if (ipProperties.UnicastAddresses[n].Address.AddressFamily ==
                    System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    if (Equals(ipAddress, adapter.GetIPProperties().UnicastAddresses[n].Address))
                    {
                        return adapter.GetIPProperties().UnicastAddresses[n].IPv4Mask;
                    }
                }
            }
        }

        return IPAddress.None;
    }
    
}