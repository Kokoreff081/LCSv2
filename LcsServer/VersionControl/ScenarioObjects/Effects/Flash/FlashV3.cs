using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Flash;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class FlashV3 : BaseVC
{
    private const string ModelClassName = "Flash";

    public long TotalTicks { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public long CycleRiseTime { get; set; }
    public long CycleFadeTime { get; set; }
    public int Repeat { get; set; }
    public bool IsRandomColor { get; set; }
    public string CompColor { get; set; }
    public string Background { get; set; }
    
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flash flash = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flash(Id, Name, ParentId,
            CompositeColor.FromString(Background),
            CompositeColor.FromString(CompColor), FadeTime, IsRandomColor, Repeat, RiseTime, TotalTicks, CycleRiseTime,
            CycleFadeTime, DimmingLevel);

        return flash;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flash flash)
        {
            return null;
        }

        FlashV3 flashVc = new FlashV3
        {
            Id = flash.Id,
            Name = flash.Name,
            ParentId = flash.ParentId,
            Background = flash.BackgroundColor.ToString(),
            CompColor = flash.CompColor.ToString(),
            FadeTime = flash.FadeTime,
            IsRandomColor = flash.IsRandomColor,
            Repeat = flash.Repeat,
            RiseTime = flash.RiseTime,
            TotalTicks = flash.TotalTicks,
            CycleRiseTime = flash.CyclicRiseTime,
            CycleFadeTime = flash.CyclicFadeTime,
            DimmingLevel = flash.DimmingLevel
        };

        return flashVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not FlashV2 flashV2)
        {
            return;
        }

        base.FromPrevious(baseVC);
        Background = flashV2.Background;
        CompColor = flashV2.CompColor;
        IsRandomColor = flashV2.IsRandomColor;
        Repeat = flashV2.Repeat;
        TotalTicks = flashV2.TotalTicks;
        CycleRiseTime = flashV2.CycleRiseTime;
        CycleFadeTime = flashV2.CycleFadeTime;
        RiseTime = flashV2.RiseTime;
        FadeTime = flashV2.FadeTime;
    }
}