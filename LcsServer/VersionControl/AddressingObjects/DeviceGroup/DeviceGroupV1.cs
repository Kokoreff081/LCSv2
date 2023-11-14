using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.DeviceGroup;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class DeviceGroupV1 : BaseVC
{
    private const string ModelClassName = "LCAddressDeviceGroup";

    public override ISaveLoad ToConcreteObject()
    {
        LCAddressDeviceGroup deviceGroup = new LCAddressDeviceGroup(Id, Name, ParentId);

        return deviceGroup;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCAddressDeviceGroup deviceGroup = (LCAddressDeviceGroup)o;
        DeviceGroupV1 addressDeviceGroupVc = new DeviceGroupV1
        {
            Id = deviceGroup.Id,
            Name = deviceGroup.Name,
            ParentId = deviceGroup.ParentId,
        };

        return addressDeviceGroupVc;
    }
}