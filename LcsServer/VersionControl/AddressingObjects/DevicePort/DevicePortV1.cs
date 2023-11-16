using LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;
using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.DevicePort;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class DevicePortV1 : BaseVC
{
    private const string ModelClassName = "LCAddressDevicePort";

    public int UniverseId { get; set; }
    public int PortNumber { get; set; }
    public int DmxSize { get; set; } = (int) DmxSizeTypes.Dmx512;

    public override ISaveLoad ToConcreteObject()
    {
        DmxSizeTypes dmxSizeType = (DmxSizeTypes)Enum.Parse(typeof(DmxSizeTypes), DmxSize.ToString());

        LCAddressDevicePort devicePort = new LCAddressDevicePort(Id, Name, ParentId, PortNumber, UniverseId, dmxSizeType);

        return devicePort;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCAddressDevicePort devicePort = (LCAddressDevicePort)o;

        DevicePortV1 devicePortVc = new DevicePortV1
        {
            Id = devicePort.Id,
            Name = devicePort.Name,
            ParentId = devicePort.ParentId,
            PortNumber = devicePort.PortNumber,
            UniverseId = devicePort.Universe?.Id ?? 0,
            DmxSize = (int)devicePort.DmxSize,
        };

        return devicePortVc;
    }
}