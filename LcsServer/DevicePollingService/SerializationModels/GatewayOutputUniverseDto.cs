using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.SerializationModels;

public class GatewayOutputUniverseDto : GatewayUniverseDto
{
    // For json deserialization
    public GatewayOutputUniverseDto() { }

    public GatewayOutputUniverseDto(GatewayOutputUniverse model) : base(model)
    {
        OutputStatus = model.OutputStatus;
    }

    public OutputStatuses OutputStatus { get; set; }

    public override string Type => nameof(GatewayOutputUniverse);

    public override BaseObject ToBaseObject()
    {
        GatewayOutputUniverse gatewayUniverse = new GatewayOutputUniverse(ParentId, IpAddress, Index, PortAddress, Universe, PortType, OutputStatus);
        return gatewayUniverse;
    }
}