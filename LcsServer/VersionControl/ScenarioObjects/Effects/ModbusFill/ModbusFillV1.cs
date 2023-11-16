using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.ModbusFill;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]

public class ModbusFillV1 : BaseVC
{
    private const string ModelClassName = "ModbusFill";

    public byte FillStyle { get; set; }
    public byte Direction { get; set; }
    public byte Rotation { get; set; }
    public string CompColor { get; set; }
    public string Background { get; set; }
    public int Angle { get; set; }
    public long TotalTicks { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public float DimmingLevel { get; set; } = 1.0f;
    public int BlurFactor { get; set; }
    
    public string IpAddress { get; set; }
    
    public ushort Port { get; set; }
    
    public byte UnitId { get; set; }
    
    public ushort Register { get; set; }
    
    public int Interval { get; set; }
    
    public float Minimum { get; set; }
    
    public float Maximum { get; set; }
    
    public byte SensorUnits { get; set; }
    
    public override ISaveLoad ToConcreteObject()
    {
        FillStyles fillStyle = (FillStyles)Enum.Parse(typeof(FillStyles), FillStyle.ToString());
        InOutDirection direction = (InOutDirection)Enum.Parse(typeof(InOutDirection), Direction.ToString());
        Rotation rotation = (Rotation)Enum.Parse(typeof(Rotation), Rotation.ToString());
        SensorUnits sensorUnits = (SensorUnits)Enum.Parse(typeof(SensorUnits), SensorUnits.ToString()); 

        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.ModbusFill fill = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.ModbusFill(Id, Name, ParentId,
            fillStyle, Angle, CompositeColor.FromString(Background), CompositeColor.FromString(CompColor), 
            direction, rotation, TotalTicks, RiseTime, FadeTime,DimmingLevel, BlurFactor,
            IpAddress, Port, UnitId, Register, Interval, Minimum, Maximum, sensorUnits);

        return fill;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.ModbusFill fill)
        {
            return null;
        }

        ModbusFillV1 fillVc = new ModbusFillV1
        {
            Id = fill.Id,
            Name = fill.Name,
            ParentId = fill.ParentId,
            FillStyle = (byte)fill.FillStyle,
            Angle = fill.Angle,
            Background = fill.BackgroundColor.ToString(),
            CompColor = fill.CompColor.ToString(),
            Direction = (byte)fill.Direction,
            Rotation = (byte)fill.Rotation,
            TotalTicks = fill.TotalTicks,
            RiseTime = fill.RiseTime,
            FadeTime =  fill.FadeTime,
            DimmingLevel = fill.DimmingLevel,
            BlurFactor = fill.BlurFactor,
            //====
            IpAddress = fill.IpAddress,
            Port = fill.Port,
            UnitId = fill.UnitId,
            Register = fill.Register,
            Interval = fill.Interval,
            Minimum = fill.Minimum,
            Maximum = fill.Maximum,
            SensorUnits = (byte)fill.SensorUnits
        };

        return fillVc;
    }
}