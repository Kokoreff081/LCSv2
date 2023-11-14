using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Flame;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class FlameV1 : BaseVC
{
    private const string ModelClassName = "Flame";

    public long TotalTicks { get; set; }
    public string[] ColorStack { get; set; }
    public int Height { get; set; }
    public int Speed { get; set; }
    public int Stability { get; set; }
    public bool Floating { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flame flame = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flame)o;

        var colors = flame.ColorStack.Get;
        string[] saveColorStack = new string[colors.Count];
        for (int i = 0; i < colors.Count; i++)
        {
            saveColorStack[i] = colors[i].ToString();
        }

        FlameV1 flameVc = new FlameV1
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
        };

        return flameVc;
    }
}