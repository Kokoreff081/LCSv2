namespace LcsServer.DatabaseLayer
{
    public class SensorValue
    {
        public long Id { get; set; }
        public string SensorId { get; set; }
        public DateTime Timestamp { get; set; }
        public double? Value { get; set; }
        public short NormalMinValue { get; set; }
        public short NormalMaxValue { get; set; }
        public int SensorUnitId { get; set; }

    }
}
