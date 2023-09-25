using Acn.Rdm.Packets.Sensors;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.SerializationModels;

public class SensorDto :BaseObjectDto
    {
        // For json deserialization
        public SensorDto() { }

        public SensorDto(Sensor model)
        {
            Id = model.Id;
            ParentId = model.ParentId;
            SensorNumber = model.SensorNumber;
            SensorType = model.SensorType;
            Unit = model.Unit;
            Prefix = model.Prefix;
            RangeMinValue = model.RangeMinValue;
            RangeMaxValue = model.RangeMaxValue;
            NormalMinValue = model.NormalMinValue;
            NormalMaxValue = model.NormalMaxValue;
            RecordValueSupport = model.RecordValueSupport;
            Description = model.Description;
            PresentValue = model.PresentValue;
            MinValue = model.MinValue;
            MaxValue = model.MaxValue;
            RecordedValue = model.RecordedValue;
        }

        public override string Type => nameof(Sensor);

        public string Id { get; set; }

        public string ParentId { get; set; }

        public byte SensorNumber { get; set; }
        
        public SensorDefinition.SensorTypes SensorType { get; set; }

        public SensorDefinition.SensorUnit Unit { get; set; }

        public SensorDefinition.SensorPrefix Prefix { get; set; }

        public short RangeMinValue { get; set; }

         public short RangeMaxValue { get; set; }

        public short NormalMinValue { get; set; }

        public short NormalMaxValue { get; set; }

        public byte RecordValueSupport { get; set; }

        public string Description { get; set; }

        public short? PresentValue { get; set; }

        public short MinValue { get; set; }

        public short MaxValue { get; set; }

        public short RecordedValue { get; set; }
        
        public override BaseObject ToBaseObject()
        {
            Sensor sensor = new Sensor(Id, ParentId)
            {
                SensorNumber = SensorNumber,
                SensorType = SensorType,
                Unit = Unit,
                Prefix = Prefix,
                RangeMinValue = RangeMinValue,
                RangeMaxValue = RangeMaxValue,
                NormalMinValue = NormalMinValue,
                NormalMaxValue = NormalMaxValue,
                RecordValueSupport = RecordValueSupport,
                Description = Description,
                PresentValue = PresentValue,
                MinValue = MinValue,
                MaxValue = MaxValue,
                RecordedValue = RecordedValue
            };

            return sensor;
        }

    }