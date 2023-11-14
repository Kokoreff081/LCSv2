using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.ModbusDIPlayingEntityLink;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class ModbusDIPlayingEntityLinkV1 : BaseVC
{
    private const string ModelClassName = "LCModbusDIPlayingEntityLink";

    public int PlayingEntityId { get; set; }

    public int ModbusInputId { get; set; }

    public ushort PortNumber { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        LCModbusDIPlayingEntityLink link = new LCModbusDIPlayingEntityLink(PlayingEntityId, ModbusInputId, PortNumber);

        return link;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCModbusDIPlayingEntityLink link = (LCModbusDIPlayingEntityLink)o;

        ModbusDIPlayingEntityLinkV1 linkVc = new ModbusDIPlayingEntityLinkV1
        {
            PlayingEntityId = link.PlayingEntityId,
            ModbusInputId = link.ModbusDeviceId,
            PortNumber = link.PortNumber,
        };

        return linkVc;
    }
}