using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.ModbusStatic;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]

public class ModbusStaticV1 : BaseVC
{
    private const string ModelClassName = "ModbusStaticColor";

    public string CompColor { get; set; }
    public string Background { get; set; }
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
        SensorUnits sensorUnits = (SensorUnits)Enum.Parse(typeof(SensorUnits), SensorUnits.ToString()); 

        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.ModbusStaticColor fill = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.ModbusStaticColor(Id, Name, ParentId,
            CompositeColor.FromString(Background), CompositeColor.FromString(CompColor), 
            TotalTicks, RiseTime, FadeTime,DimmingLevel, IpAddress, Port, UnitId, Register, Interval, Minimum, Maximum, sensorUnits);

        return fill;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not ModbusStaticColor eff)
        {
            return null;
        }

        ModbusStaticV1 fillVc = new ModbusStaticV1
        {
            Id = eff.Id,
            Name = eff.Name,
            ParentId = eff.ParentId,
            Background = eff.BackgroundColor.ToString(),
            CompColor = eff.Color.ToString(),
            TotalTicks = eff.TotalTicks,
            RiseTime = eff.RiseTime,
            FadeTime =  eff.FadeTime,
            DimmingLevel = eff.DimmingLevel,
            //====
            IpAddress = eff.IpAddress,
            Port = eff.Port,
            UnitId = eff.UnitId,
            Register = eff.Register,
            Interval = eff.Interval,
            Minimum = eff.Minimum,
            Maximum = eff.Maximum,
            SensorUnits = (byte)eff.SensorUnits
        };

        return fillVc;
    }
}