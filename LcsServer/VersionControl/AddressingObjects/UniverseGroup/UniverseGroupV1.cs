using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.UniverseGroup;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class UniverseGroupV1 : BaseVC
{
    private const string ModelClassName = "LCAddressUniverseGroup";

    public override ISaveLoad ToConcreteObject()
    {
        LCAddressUniverseGroup addressUniverseGroup = new LCAddressUniverseGroup(Id, Name,ParentId);

        return addressUniverseGroup;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCAddressUniverseGroup universeGroup = (LCAddressUniverseGroup)o;

        UniverseGroupV1 universeGroupVc = new UniverseGroupV1
        {
            Id = universeGroup.Id,
            Name = universeGroup.Name,
            ParentId = universeGroup.ParentId,
        };

        return universeGroupVc;
    }
}