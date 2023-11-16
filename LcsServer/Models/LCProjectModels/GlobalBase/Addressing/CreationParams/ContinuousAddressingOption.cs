namespace LcsServer.Models.LCProjectModels.GlobalBase.Addressing.CreationParams;

public class ContinuousAddressingOption : BaseOption
{
    public int StartDmxAddress { get; set; }

    public int EndDmxAddress { get; set; }
}