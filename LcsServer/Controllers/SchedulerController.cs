using LcsServer.Models.LCProjectModels.Managers;
using LcsServer.Models.LCProjectModels.Models.Project;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace LcsServer.Controllers;

public class SchedulerController: Controller
{
    private Project _project;
    private ScheduleManager _scheduleManager;
    private readonly IConfiguration Configuration;

    public SchedulerController(IConfiguration _configuration, ScheduleManager scheduleManager)
    {
        Configuration = _configuration;
        _scheduleManager = scheduleManager;
        var projectPath = Path.Combine(Configuration.GetValue<string>("LightCadProjectsFolder"),
            Configuration.GetValue<string>("DefaultProjectFolder"));

        string unzippedString = System.IO.File.ReadAllText(projectPath);
        _project = JsonConvert.DeserializeObject<Project>(unzippedString);
    }

    public string Index()
    {
        return "hello";
    }
}