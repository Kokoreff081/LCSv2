using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.ArtNetDevice;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class ArtNetDeviceV1 : BaseVC
{
    private const string ModelClassName = "LCArtNetAddressDevice";

    public string IpAddress { get; set; }
    public int[] PortIds { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        LCArtNetAddressDevice addressDevice = new LCArtNetAddressDevice(Id, Name, ParentId, PortIds, IpAddress);

        return addressDevice;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCArtNetAddressDevice addressDevice = (LCArtNetAddressDevice)o;

        ArtNetDeviceV1 addressDeviceVc = new ArtNetDeviceV1
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