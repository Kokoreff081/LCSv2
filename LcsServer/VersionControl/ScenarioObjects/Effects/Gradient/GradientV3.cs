using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Gradient;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class GradientV3 : BaseVC
{
    private const string ModelClassName = "Gradient";

    public byte FillStyle { get; set; }
    public byte Direction { get; set; }
    public byte Rotation { get; set; }
    public string[] ColorStack { get; set; }
    public int Angle { get; set; }
    public bool IsRandomAngle { get; set; }
    public long TotalTicks { get; set; }
    public int Speed { get; set; }
    public float Scale { get; set; } = 1;
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        List<CompositeColor> colorsPalette = ColorStack.Select(x => CompositeColor.FromString(x)).ToList();
        InOutDirection direction = (InOutDirection)Enum.Parse(typeof(InOutDirection), Direction.ToString());
        FillStyles fillStyle = (FillStyles)Enum.Parse(typeof(FillStyles), FillStyle.ToString());
        Rotation rotation = (Rotation)Enum.Parse(typeof(Rotation), Rotation.ToString());

        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Gradient gradient = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Gradient(Id, Name,
            ParentId, Angle, colorsPalette, direction, TotalTicks, fillStyle,
            IsRandomAngle, rotation, Speed, Scale, RiseTime, FadeTime, DimmingLevel);

        return gradient;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Gradient gradient)
        {
            return null;
        }

        string[] saveColorStack = new string[gradient.ColorsPalette.Count];
        for (int i = 0; i < gradient.ColorsPalette.Count; i++)
        {
            saveColorStack[i] = gradient.ColorsPalette[i].ToString();
        }

        GradientV3 gradientVc = new GradientV3
        {
            Id = gradient.Id,
            Name = gradient.Name,
            ParentId = gradient.ParentId,
            Angle = gradient.Angle,
            ColorStack = saveColorStack,
            Direction = (byte)gradient.Direction,
            TotalTicks = gradient.TotalTicks,
            FillStyle = (byte)gradient.FillStyle,
            IsRandomAngle = gradient.IsRandomAngle,
            Rotation = (byte)gradient.Rotation,
            Speed = gradient.Speed,
            Scale = gradient.Scale,
            RiseTime = gradient.RiseTime,
            FadeTime = gradient.FadeTime,
            DimmingLevel = gradient.DimmingLevel
        };

        return gradientVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not GradientV2 gradientV2)
        {
            return;
        }

        base.FromPrevious(baseVC);

        FillStyle = gradientV2.FillStyle;
        Direction = gradientV2.Direction;
        Rotation = gradientV2.Rotation;
        ColorStack = gradientV2.ColorStack;
        Angle = gradientV2.Angle;
        IsRandomAngle = gradientV2.IsRandomAngle;
        TotalTicks = gradientV2.TotalTicks;
        Speed = gradientV2.Speed;
        Scale = gradientV2.Scale;
        RiseTime = gradientV2.RiseTime;
        FadeTime = gradientV2.FadeTime;
    }
}