using LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;
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
    public int? LampAddress { get; set; }
    public int ColorsCount { get; set; }
    public DmxSizeTypes? DmxSize { get; set; }
    public int? ParentId { get; set; }
    public List<LCLampFront> Children { get; set; }
}