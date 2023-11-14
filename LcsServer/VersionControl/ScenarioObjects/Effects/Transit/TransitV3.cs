using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Transit;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class TransitV3 : BaseVC
{
    private const string ModelClassName = "Transit";

    public int Repeat { get; set; }
    public int Speed { get; set; }
    public string[] SaveColorStack { get; set; }
    public long TotalTicks { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        List<CompositeColor> colorsPalette = SaveColorStack.Select(x => CompositeColor.FromString(x)).ToList();

        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Transit transit = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Transit(Id, Name, ParentId,
            Repeat, colorsPalette, Speed, TotalTicks, RiseTime, FadeTime, DimmingLevel);
        return transit;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Transit transit)
        {
            return null;
        }

        TransitV3 transitVc = new TransitV3 {
            Id = transit.Id,
            Name = transit.Name,
            ParentId = transit.ParentId,
            Repeat = transit.Repeat,
            SaveColorStack = transit.ColorStack.Select(x => x.ToString()).ToArray(),
            Speed = transit.Speed,
            TotalTicks = transit.TotalTicks,
            RiseTime = transit.RiseTime,
            FadeTime = transit.FadeTime,
            DimmingLevel = transit.DimmingLevel
        };

        return transitVc;
    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not TransitV2 transitV2)
        {
            return;
        }

        base.FromPrevious(baseVC);
        Repeat = transitV2.Repeat;
        SaveColorStack = transitV2.SaveColorStack;
        Speed = transitV2.Speed;
        TotalTicks = transitV2.TotalTicks;
        RiseTime = transitV2.RiseTime;
        FadeTime = transitV2.FadeTime;
    }
}