using LcsServer.Models.LCProjectModels.Models.Addressing;

namespace LcsServer.Models.ProjectModels;

public class LCLampFront
{
    public int Id { get; set; }
    public string Name { get; set; }
    public LCAddressLamp AddressData { get; set; }
    public string IpAddress { get; set; }
    public string ParentPort { get; set; }
    public string Type { get; set; }
    public int LampAddress { get; set; }
    public int ColorsCount { get; set; }

}