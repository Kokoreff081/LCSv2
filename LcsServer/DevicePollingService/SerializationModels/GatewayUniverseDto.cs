using System.Net;
using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.SerializationModels;

public abstract class GatewayUniverseDto : BaseObjectDto
{
    // For json deserialization
    public GatewayUniverseDto() { }

    public GatewayUniverseDto(GatewayUniverse model)
    {
        Index = model.Index;
        Universe = model.Universe;
        PortAddress = model.PortAddress;
        ParentId = model.ParentId;
        IpAddress = model.IpAddress;
    }
        
    public IPAddress IpAddress { get; }
        
    public int Index { get; set; }

    public byte Universe { get; set; }

    public int PortAddress { get; set; }

    public string ParentId { get; set; }

    public PortTypes PortType { get; set; }

}