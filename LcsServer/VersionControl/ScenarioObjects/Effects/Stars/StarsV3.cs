using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Stars;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class StarsV3 : BaseVC
{
    private const string ModelClassName = "Stars";

    public long TotalTicks { get; set; }
    public string[] ColorStack { get; set; }
    public int Filling { get; set; }
    public int LifeTime { get; set; }
    public bool Modulation { get; set; }
    public bool IsColorMix { get; set; }
    public string Background { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        List<CompositeColor> colorStack = ColorStack.Select(x => CompositeColor.FromString(x)).ToList();

        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Stars stars = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Stars(Id, Name, ParentId,
            CompositeColor.FromString(Background), colorStack, Filling, IsColorMix, LifeTime, Modulation, TotalTicks,
            RiseTime, FadeTime, DimmingLevel);

        return stars;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Stars stars)
        {
            return null;
        }

        string[] saveColorStack = new string[stars.ColorStack.Count];
        for (int i = 0; i < stars.ColorStack.Count; i++)
        {
            saveColorStack[i] = stars.ColorStack[i].ToString();
        }

        StarsV3 starsVc = new StarsV3 {
            Id = stars.Id,
            Name = stars.Name,
            ParentId = stars.ParentId,
            Background = stars.Background.ToString(),
            ColorStack = saveColorStack,
            Filling = stars.Filling,
            IsColorMix = stars.IsColorMix,
            LifeTime = stars.LifeTime,
            Modulation = stars.Modulation,
            TotalTicks = stars.TotalTicks,
            RiseTime = stars.RiseTime,
            FadeTime = stars.FadeTime,
            DimmingLevel = stars.DimmingLevel
        };

        return starsVc;
    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not StarsV2 starsV2)
        {
            return;
        }

        base.FromPrevious(baseVC);

        Background = starsV2.Background;
        ColorStack = starsV2.ColorStack;
        Filling = starsV2.Filling;
        IsColorMix = starsV2.IsColorMix;
        LifeTime = starsV2.LifeTime;
        Modulation = starsV2.Modulation;
        TotalTicks = starsV2.TotalTicks;
        RiseTime = starsV2.RiseTime;
        FadeTime = starsV2.FadeTime;
    }
}