using System.Numerics;

namespace LcsServer.Models.ProjectModels;

public class WebRaster
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int DimensionX { get; set; }
    public int DimensionY { get; set; }
    public bool ManualSet { get; set; }
    public int Angle { get; set; }
    public bool IsFlipHorizontal { get; set; }
    public bool IsFlipVertical { get; set; }
    public Quaternion? Rotation { get; set; }

    public List<WebRasterProjection> Projections { get; set; }
}