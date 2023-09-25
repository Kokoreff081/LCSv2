namespace LcsServer.Models.RequestModels;

public class ParamToChange
{
    public string id { get; set; }
    public int parameterId { get; set; }
    public string newValue { get; set; }
}