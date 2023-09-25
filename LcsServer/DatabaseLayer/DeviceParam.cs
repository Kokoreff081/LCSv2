namespace LcsServer.DatabaseLayer;

public class DeviceParam
{
    public int Id { get; set; }
    public string? ParamId { get; set; }
    public int DeviceId { get; set; }
    public string ParamName { get; set; }
    public string ParamValue { get; set; }
    public DateTime LastPoll { get; set; }
    
    public Device Device { get; set; }
}