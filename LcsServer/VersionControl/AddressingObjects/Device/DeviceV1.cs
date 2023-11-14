using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.Device;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class DeviceV1 : BaseVC
{
    private const string ModelClassName = "LCAddressDevice";

    public string IpAddress { get; set; }
    public byte Protocol { get; set; }
    public int[] PortIds { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        LCArtNetAddressDevice addressDevice = new LCArtNetAddressDevice(Id, Name, ParentId, PortIds, IpAddress);

        return addressDevice;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCArtNetAddressDevice addressDevice = (LCArtNetAddressDevice)o;

        DeviceV1 addressDeviceVc = new DeviceV1
        {
            Id = addressDevice.Id,
            Name = addressDevice.Name,
            ParentId = addressDevice.ParentId,
            IpAddress = addressDevice.IpAddress,
            PortIds = addressDevice.Ports.Select(x => x.Id).ToArray(),
        };

        return addressDeviceVc;
    }
}