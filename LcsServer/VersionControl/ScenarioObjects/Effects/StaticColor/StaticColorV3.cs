using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.StaticColor;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class StaticColorV3 : BaseVC
{
    private const string ModelClassName = "StaticColor";

    public string Background { get; set; }
    public long TotalTicks { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.StaticColor staticColor = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.StaticColor(Id,
            Name, ParentId, CompositeColor.FromString(Background), TotalTicks, RiseTime, FadeTime, DimmingLevel);

        return staticColor;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.StaticColor staticColor)
        {
            return null;
        }

        StaticColorV3 staticColorVc = new StaticColorV3 {
            Id = staticColor.Id,
            Name = staticColor.Name,
            ParentId = staticColor.ParentId,
            Background = staticColor.BackgroundColor.ToString(),
            TotalTicks = staticColor.TotalTicks,
            RiseTime = staticColor.RiseTime,
            FadeTime = staticColor.FadeTime,
            DimmingLevel = staticColor.DimmingLevel
        };

        return staticColorVc;
    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not StaticColorV2 staticColorV2)
        {
            return;
        }

        base.FromPrevious(baseVC);

        Background = staticColorV2.Background;
        TotalTicks = staticColorV2.TotalTicks;
        RiseTime = staticColorV2.RiseTime;
        FadeTime = staticColorV2.FadeTime;
    }
}