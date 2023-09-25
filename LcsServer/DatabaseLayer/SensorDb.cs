namespace LcsServer.DatabaseLayer;

public class SensorDb
{
    public int Id { get; set; }
    public string deviceId { get; set; }
    public string description { get; set; }
    public int SensorUnitId { get; set; }
    public string SensorId { get; set; }
    public int SensorNumber { get; set; }
    public DateTime LastPoll { get; set; }
}