using LcsServer.Models.DeviceModels;
using LcsServer.Models.LCProjectModels.Models.Project;

namespace LcsServer.Models.ProjectModels;

public class ProjectToWeb
{

    public ProjectToWeb()
    {
        Rasters = new List<WebRaster>();
        RasterProjections = new List<WebRasterProjection>();
        Versions = new List<LcsProjectVersion>();
    }
    public string Name { get; set; }

    public ProjectInfo ProjectInfo { get; set; }

    public List<WebRaster> Rasters { get; set; }
    public List<ScenarioNameId> Scenarios { get; set; }
    public List<ScheduleGroupFront> Scheduler { get; set; }
    public List<WebRasterProjection> RasterProjections { get; set; }
    public bool IsTasksPlaying { get; set; }
    public List<LCLampsFront> LCLamps { get; set; }
    public List<LCLampFront> OnlyProjectLamps { get; set; }
    public List<NewToWebDevices> ToTreeTable { get; set; }
    public DateTime LastModified { get; set; }
    public string Path { get; set; }
    public List<LcsProjectVersion> Versions { get; set; }
}

public class LcsProjectVersion
{
    public int Id { get; set; }
    public string Name { get; set; }
}