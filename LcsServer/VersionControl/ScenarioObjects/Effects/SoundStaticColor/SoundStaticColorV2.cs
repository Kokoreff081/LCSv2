using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.SoundStaticColor;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 2)]
public class SoundStaticColorV2 : BaseVC
{
    private const string ModelClassName = "SoundStaticColor";

    public string Background { get; set; }
    public long TotalTicks { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    
    public float DimmingLevel { get; set; } = 1.0f;
        
    public override ISaveLoad ToConcreteObject()
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.SoundStaticColor soundStaticColor =
            new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.SoundStaticColor(Id, Name, ParentId,
                CompositeColor.FromString(Background), TotalTicks, RiseTime, FadeTime, DimmingLevel);

        return soundStaticColor;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.SoundStaticColor soundStaticColor)
        {
            return null;
        }

        SoundStaticColorV2 soundStaticColorVc = new SoundStaticColorV2
        {
            Id = soundStaticColor.Id,
            Name = soundStaticColor.Name,
            ParentId =  soundStaticColor.ParentId,
            Background = soundStaticColor.BackgroundColor.ToString(),
            TotalTicks = soundStaticColor.TotalTicks,
            RiseTime = soundStaticColor.RiseTime,
            FadeTime = soundStaticColor.FadeTime,
            DimmingLevel = soundStaticColor.DimmingLevel
        };

        return soundStaticColorVc;
    }
    
    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not SoundStaticColorV1 soundStaticColorV1)
        {
            return;
        }

        base.FromPrevious(baseVC);

        RiseTime = soundStaticColorV1.RiseTime;
        FadeTime = soundStaticColorV1.FadeTime;
        Background = soundStaticColorV1.Background;
        TotalTicks = soundStaticColorV1.TotalTicks;
    }
}