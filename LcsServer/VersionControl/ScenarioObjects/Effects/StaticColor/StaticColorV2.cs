using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.StaticColor;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 2)]
public class StaticColorV2 : BaseVC
{
    private const string ModelClassName = "StaticColor";

    public string Background { get; set; }
    public long TotalTicks { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("Cannot instantiate old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.StaticColor staticColor)
        {
            return null;
        }

        StaticColorV2 staticColorVc = new StaticColorV2
        {
            Id = staticColor.Id,
            Name = staticColor.Name,
            ParentId =  staticColor.ParentId,
            Background = staticColor.BackgroundColor.ToString(),
            TotalTicks = staticColor.TotalTicks,
            RiseTime = staticColor.RiseTime,
            FadeTime = staticColor.FadeTime,
        };

        return staticColorVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not StaticColorV1 staticColorV1)
        {
            return;
        }

        base.FromPrevious(baseVC);

        Background = staticColorV1.Background;
        TotalTicks = staticColorV1.TotalTicks;
    }
}