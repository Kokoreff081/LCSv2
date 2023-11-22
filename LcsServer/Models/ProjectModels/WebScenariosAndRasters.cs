namespace LcsServer.Models.ProjectModels;

public class WebScenariosAndRasters
{
    public List<WebRaster> Rasters { get; set; }
    public List<ScenarioNameId> Scenarios { get; set; }
    public List<WebRasterProjection> RasterProjections { get; set; }
}