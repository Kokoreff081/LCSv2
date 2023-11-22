namespace LcsServer.Models.ProjectModels;

public class WebRasterProjection
{
    public int LampId { get; set; }

    /// <summary>
    /// Кол-во источников в светильнике
    /// </summary>
    public int PixelsCount { get; set; }

    /// <summary>
    /// Кол-во цветов подерживает лампа
    /// </summary>
    public int ColorsCount { get; set; }
    public int RasterX { get; set; }
    public int RasterY { get; set; }
    public string? Color { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}