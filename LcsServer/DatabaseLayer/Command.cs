namespace LcsServer.DatabaseLayer;

public class Command
{
    public int Id { get; set; }
    public string DeviceId { get; set; }
    public int ParamId { get; set; }
    public string ParamNewValue { get; set; }
    public int CommandType { get; set; }//values in enum CommandTypes in WebInterface.Models namespace
    public int State { get; set; }//0 - new, 1 - done, 2 - cancelled
    public string UserLogin { get; set; }
    
}