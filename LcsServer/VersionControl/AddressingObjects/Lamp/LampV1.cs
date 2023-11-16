using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.Lamp;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class LampV1 : BaseVC
{
    private const string ModelClassName = "LCAddressLamp";

    public int LampAddress { get; set; }
    public int PixelsCount { get; set; }
    public int LampId { get; set; }
    public int ColorsCount { get; set; }
    public bool IsReverse { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        LCAddressLamp addressLamp = new LCAddressLamp(Id, Name, ParentId, ColorsCount, LampAddress, LampId, PixelsCount);
        addressLamp.IsReverse = IsReverse;
        return addressLamp;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCAddressLamp addressLamp = (LCAddressLamp)o;

        LampV1 lampVc = new LampV1
        {
            Id = addressLamp.Id,
            Name = addressLamp.Name,
            ParentId = addressLamp.ParentId,
            ColorsCount = addressLamp.ColorsCount,
            LampAddress = addressLamp.LampAddress,
            LampId = addressLamp.LampId,
            PixelsCount = addressLamp.PixelsCount,
            IsReverse = addressLamp.IsReverse,
        };

        return lampVc;
    }
}