using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.SoundStaticColor;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class SoundStaticColorV1 : BaseVC
{
    private const string ModelClassName = "SoundStaticColor";

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
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.SoundStaticColor soundStaticColor = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.SoundStaticColor) o;

        SoundStaticColorV1 soundStaticColorVc = new SoundStaticColorV1
        {
            Id = soundStaticColor.Id,
            Name = soundStaticColor.Name,
            ParentId =  soundStaticColor.ParentId,
            Background = soundStaticColor.BackgroundColor.ToString(),
            TotalTicks = soundStaticColor.TotalTicks,
            RiseTime = soundStaticColor.RiseTime,
            FadeTime = soundStaticColor.FadeTime,
        };

        return soundStaticColorVc;
    }
}