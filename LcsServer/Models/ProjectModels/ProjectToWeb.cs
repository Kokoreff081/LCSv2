using LcsServer.Models.DeviceModels;
using LcsServer.Models.LCProjectModels.Models.Project;

namespace LcsServer.Models.ProjectModels;

public class ProjectToWeb
{

    public ProjectToWeb()
    {
        Rasters = new List<WebRaster>();
        RasterProjections = new List<WebRasterProjection>();
        
    }
    public string Name { get; set; }

    public ProjectInfo ProjectInfo { get; set; }

    public List<WebRaster> Rasters { get; set; }
    public List<ScenarioNameId> Scenarios { get; set; }
    public List<ScheduleFront> Scheduler { get; set; }
    public List<WebRasterProjection> RasterProjections { get; set; }
    public bool IsTasksPlaying { get; set; }
    public List<LCLampsFront> LCLamps { get; set; }
    public List<LCLampFront> OnlyProjectLamps { get; set; }
    public List<NewToWebDevices> ToTreeTable { get; set; }
    public DateTime LastModified { get; set; }
}