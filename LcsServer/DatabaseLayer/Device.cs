namespace LcsServer.DatabaseLayer;

public class Device
{
    public int Id { get; set; }
    public string deviceId { get; set; }
    public string Type { get; set; }
    public int StatusId { get; set; }
    public string ParentId { get; set; }

    public List<DeviceParam> DeviceParams { get; set; }
}