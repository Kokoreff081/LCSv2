using System.Net;
using Acn.Rdm;

namespace LcsServer.DevicePollingService.Models;

public class DeviceFoundEventArgs
{
    public UId Id { get; set; }

    public IPAddress Address { get; set; }

    public byte DevicePort { get; set; }

    public byte BindIndex { get; set; }

    public byte Universe { get; set; }

    public byte Net { get; set; }
}