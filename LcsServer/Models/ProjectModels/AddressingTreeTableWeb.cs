namespace LcsServer.Models.ProjectModels;

public class AddressingTreeTableWeb
{
    public string key { get; set;}

    public ColumnsToAddressingTT data { get; set; }

    public List<AddressingTreeTableWeb> children { get; set; }
}

public class ColumnsToAddressingTT
{
    public string Name { get; set; }
    public int? LampAddress { get; set; }
    public int? ColorsCount { get; set; }
    public string? IpAddress { get; set; }
}