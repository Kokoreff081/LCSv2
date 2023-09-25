using System.Net;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;
using LcsServer.DevicePollingService.Enums;

namespace LcsServer.Models.DeviceModels;

public class GatewayUniverseToWeb
{
    private int _index;
    private int _portIndex;
    private byte _universe;
    private int _portAddress;
    private PortTypes _portType;
    protected GatewayUniverseToWeb(
        string parentId,
        IPAddress address,
        int index,
        int portAddress,
        byte universe,
        bool isInUniverse,
        PortTypes portType)
    {
        IpAddress = address;
        Index = index;
        PortAddress = portAddress;
        Universe = universe;
        PortType = portType;
       
    }
    public string Id { get; set; }
    public IPAddress IpAddress { get; }
    public DeviceStatuses deviceStatus { get; set; }
    public int Index
    {
        get { return _index; }
        set
        {
            _index = value;
        }
    }

    public byte Universe
    {
        get { return _universe; }
        set
        {
            _universe = value;
        }
    }

    /// <summary>
    /// Порядковый номер (физический номер порта)
    /// </summary>
    public int PortIndex
    {
        get { return _portIndex; }
        set
        {
            _portIndex = value;
        }
    }

    public int PortAddress
    {
        get { return _portAddress; }
        set
        {
            _portAddress = value;
        }
    }

    public PortTypes PortType
    {
        get { return _portType; }
        set
        {
            _portType = value;
        }
    }

    public List<RdmDeviceToWeb> children { get; set; }
    public bool IsInProject { get; set; }
    public string Name { get; set; }

    public string Type => nameof(GatewayUniverse);
}