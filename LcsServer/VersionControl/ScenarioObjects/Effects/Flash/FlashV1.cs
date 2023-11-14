using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Flash;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class FlashV1 : BaseVC
{
    private const string ModelClassName = "Flash";

    public long TotalTicks { get; set; }
    public float RisePercent { get; set; }
    public float FadePercent { get; set; }
    public int Repeat { get; set; }
    public bool IsRandomColor { get; set; }
    public string CompColor { get; set; }
    public string Background { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        throw new Exception("It's old class version");
    }
}