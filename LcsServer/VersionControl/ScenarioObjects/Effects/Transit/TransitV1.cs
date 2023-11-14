using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Transit;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class TransitV1 : BaseVC
{
    private const string ModelClassName = "Transit";

    public int Repeat { get; set; }
    public int Speed { get; set; }
    public string[] SaveColorStack { get; set; }
    public long TotalTicks { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Transit transit = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Transit)o;
        TransitV1 transitVc = new TransitV1
        {
            Id = transit.Id,
            Name = transit.Name,
            ParentId = transit.ParentId,
            Repeat = transit.Repeat,
            SaveColorStack = transit.ColorStack.Select(x => x.ToString()).ToArray(),
            Speed = transit.Speed,
            TotalTicks = transit.TotalTicks,
        };

        return transitVc;
    }
}