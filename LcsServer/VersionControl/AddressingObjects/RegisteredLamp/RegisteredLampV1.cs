using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.RegisteredLamp;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class RegisteredLampV1 : BaseVC
{
    private const string ModelClassName = "LCAddressRegisteredLamp";

    public int LampId { get; set; }
    public int ArrayIndex { get; set; }
    public int ArraySize { get; set; }
    public bool IsReverse { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        LCAddressRegisteredLamp registeredLamp = new LCAddressRegisteredLamp()
        {
            ArrayIndex = ArrayIndex,
            ArraySize = ArraySize,
            IsReverse = IsReverse,
            LampId = LampId,
        };

        return registeredLamp;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCAddressRegisteredLamp registeredLamp = (LCAddressRegisteredLamp) o;

        RegisteredLampV1 registeredLampVc = new RegisteredLampV1
        {
            LampId = registeredLamp.LampId,
            ArrayIndex = registeredLamp.ArrayIndex,
            ArraySize = registeredLamp.ArraySize,
            IsReverse = registeredLamp.IsReverse,
        };

        return registeredLampVc;
    }
}