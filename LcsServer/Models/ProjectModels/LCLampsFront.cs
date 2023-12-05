using LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;

namespace LcsServer.Models.ProjectModels;

public class LCLampsFront
{
    public int Id { get; set; }
    public string? IpAddress { get; set; }
    public string? Name { get; set; }
    public string Type { get; set; }

    public List<FrontPort> Children { get; set; }
}

public class FrontPort
{
    public int Id { get; set; }

    public int ParentId { get; set; }

    public string Type { get; set; }

    public List<LCLampFront> Children { get; set; }

    public string Name { get; set; }
    public DmxSizeTypes DmxSize { get; set; }

}