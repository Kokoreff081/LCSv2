using LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;
using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.Universe;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class UniverseV1 : BaseVC
{
    private const string ModelClassName = "LCAddressUniverse";

    public int UId { get; set; }
    public int DmxSize { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        DmxSizeTypes dmxSizeType = (DmxSizeTypes)Enum.Parse(typeof(DmxSizeTypes), DmxSize.ToString());

        LCAddressUniverse addressUniverse = new LCAddressUniverse(Id, Name, ParentId, dmxSizeType, UId);

        return addressUniverse;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCAddressUniverse addressUniverse = (LCAddressUniverse)o;

        UniverseV1 universeVc = new UniverseV1()
        {
            Id = addressUniverse.Id,
            Name = addressUniverse.Name,
            ParentId = addressUniverse.ParentId,
            DmxSize = (int)addressUniverse.DmxSize,
            UId = addressUniverse.UId,
        };

        return universeVc;
    }
}