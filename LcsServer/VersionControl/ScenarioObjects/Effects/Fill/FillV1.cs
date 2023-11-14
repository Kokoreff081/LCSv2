using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Fill;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class FillV1 : BaseVC
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

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Fill fill = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Fill)o;

        FillV1 fillVc = new FillV1
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
        };

        return fillVc;
    }
}