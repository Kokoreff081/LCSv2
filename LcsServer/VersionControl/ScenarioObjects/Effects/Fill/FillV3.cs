using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Fill;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]

public class FillV3 : BaseVC
{
    private const string ModelClassName = "Fill";

    public byte FillStyle { get; set; }
    public byte Direction { get; set; }
    public byte Rotation { get; set; }
    public string CompColor { get; set; }
    public string Background { get; set; }
    public int Angle { get; set; }
    public bool IsRandomAngle { get; set; }
    public long TotalTicks { get; set; }
    public bool IsRandomColor { get; set; }
    public int Repeat { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public long CycleRiseTime { get; set; }
    public long CycleFadeTime { get; set; }
    public float DimmingLevel { get; set; } = 1.0f;
    public int BlurFactor { get; set; }

    public bool IsRevertEven { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        FillStyles fillStyle = (FillStyles)Enum.Parse(typeof(FillStyles), FillStyle.ToString());
        InOutDirection direction = (InOutDirection)Enum.Parse(typeof(InOutDirection), Direction.ToString());
        Rotation rotation = (Rotation)Enum.Parse(typeof(Rotation), Rotation.ToString());

        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Fill fill = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Fill(Id, Name, ParentId,
            fillStyle, Angle, CompositeColor.FromString(Background),
            CompositeColor.FromString(CompColor), direction, IsRandomAngle, rotation, TotalTicks, IsRandomColor, Repeat,
            RiseTime, FadeTime, CycleRiseTime, CycleFadeTime, DimmingLevel, BlurFactor, IsRevertEven);

        return fill;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Fill fill)
        {
            return null;
        }

        FillV3 fillVc = new FillV3
        {
            Id = fill.Id,
            Name = fill.Name,
            ParentId = fill.ParentId,
            FillStyle = (byte)fill.FillStyle,
            Angle = fill.Angle,
            Background = fill.BackgroundColor.ToString(),
            CompColor = fill.CompColor.ToString(),
            Direction = (byte)fill.Direction,
            IsRandomAngle = fill.IsRandomAngle,
            Rotation = (byte)fill.Rotation,
            TotalTicks = fill.TotalTicks,
            IsRandomColor = fill.IsRandomColor,
            Repeat = fill.Repeat,
            RiseTime = fill.RiseTime,
            FadeTime = fill.FadeTime,
            CycleRiseTime = fill.CyclicRiseTime,
            CycleFadeTime = fill.CyclicFadeTime,
            DimmingLevel = fill.DimmingLevel,
            BlurFactor = fill.BlurFactor,
            IsRevertEven = fill.IsRevertEven
        };

        return fillVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not FillV2 fillV2)
        {
            return;
        }

        base.FromPrevious(baseVC);
        Background = fillV2.Background;
        CompColor = fillV2.CompColor;
        IsRandomColor = fillV2.IsRandomColor;
        Repeat = fillV2.Repeat;
        TotalTicks = fillV2.TotalTicks;
        FillStyle = fillV2.FillStyle;
        Direction = fillV2.Direction;
        Rotation = fillV2.Rotation;
        Angle = fillV2.Angle;
        IsRandomAngle = fillV2.IsRandomAngle;
        RiseTime = fillV2.RiseTime;
        FadeTime = fillV2.FadeTime;
    }
}