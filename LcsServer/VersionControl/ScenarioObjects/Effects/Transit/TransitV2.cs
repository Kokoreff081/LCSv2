using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Transit;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 2)]
public class TransitV2 : BaseVC
{
    private const string ModelClassName = "Transit";

    public int Repeat { get; set; }
    public int Speed { get; set; }
    public string[] SaveColorStack { get; set; }
    public long TotalTicks { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("Cannot instantiate old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Transit transit)
        {
            return null;
        }

        TransitV2 transitVc = new TransitV2
        {
            Id = transit.Id,
            Name = transit.Name,
            ParentId = transit.ParentId,
            Repeat = transit.Repeat,
            SaveColorStack = transit.ColorStack.Select(x => x.ToString()).ToArray(),
            Speed = transit.Speed,
            TotalTicks = transit.TotalTicks,
            RiseTime = transit.RiseTime,
            FadeTime = transit.FadeTime,
        };

        return transitVc;

    }
    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not TransitV1 transitV1)
        {
            return;
        }

        base.FromPrevious(baseVC);
        Repeat = transitV1.Repeat;
        SaveColorStack = transitV1.SaveColorStack;
        Speed = transitV1.Speed;
        TotalTicks = transitV1.TotalTicks;
    }
}