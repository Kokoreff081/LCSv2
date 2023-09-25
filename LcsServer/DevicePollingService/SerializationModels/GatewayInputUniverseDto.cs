using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.SerializationModels;

public class GatewayInputUniverseDto : GatewayUniverseDto
{
    // For json deserialization
    public GatewayInputUniverseDto() { }

    public GatewayInputUniverseDto(GatewayInputUniverse model):base(model)
    {
        InputStatus = model.InputStatus;
    }

    public InputStatuses InputStatus { get; set; }

    public override string Type => nameof(GatewayInputUniverse);

    public override BaseObject ToBaseObject()
    {
        GatewayInputUniverse gatewayUniverse = new GatewayInputUniverse(ParentId, IpAddress, Index, PortAddress, Universe, PortType, InputStatus);
        return gatewayUniverse;
    }
}