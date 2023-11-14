using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Flame;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class FlameV3 : BaseVC
{
    private const string ModelClassName = "Flame";

    public long TotalTicks { get; set; }
    public string[] ColorStack { get; set; }
    public int Height { get; set; }
    public int Speed { get; set; }
    public int Stability { get; set; }
    public bool Floating { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        IEnumerable<CompositeColor> colorStack = ColorStack.Select(CompositeColor.FromString);
        CompositeColorList compositeColorList = new CompositeColorList(colorStack);

        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flame flame = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flame(Id, Name, ParentId,
            compositeColorList, Floating, Height, Speed, Stability, TotalTicks, RiseTime, FadeTime, DimmingLevel);

        return flame;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flame flame)
        {
            return null;
        }

        IReadOnlyList<CompositeColor> colors = flame.ColorStack.Get;
        string[] saveColorStack = new string[colors.Count];
        for (int i = 0; i < colors.Count; i++)
        {
            saveColorStack[i] = colors[i].ToString();
        }

        FlameV3 flameVc = new FlameV3
        {
            Id = flame.Id,
            Name = flame.Name,
            ParentId = flame.ParentId,
            ColorStack = saveColorStack,
            Floating = flame.Floating,
            Height = flame.Height,
            Speed = flame.Speed,
            Stability = flame.Stability,
            TotalTicks = flame.TotalTicks,
            RiseTime = flame.RiseTime,
            FadeTime = flame.FadeTime,
            DimmingLevel = flame.DimmingLevel
        };

        return flameVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not FlameV2 flameV2)
        {
            return;
        }

        base.FromPrevious(baseVC);

        TotalTicks = flameV2.TotalTicks;
        ColorStack = flameV2.ColorStack;
        Height = flameV2.Height;
        Speed = flameV2.Speed;
        Stability = flameV2.Stability;
        Floating = flameV2.Floating;
        RiseTime = flameV2.RiseTime;
        FadeTime = flameV2.FadeTime;
    }
}