using LcsServer.Models.LCProjectModels.Managers;
using LcsServer.Models.LCProjectModels.Models.Project;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using LcsServer.Models.ProjectModels;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace LcsServer.Controllers;

public class SchedulerController: Controller
{

    private ScheduleManager _scheduleManager;
    private readonly IConfiguration Configuration;
    private ProjectChanger _pChanger;

    public SchedulerController(IConfiguration _configuration, ScheduleManager scheduleManager, ProjectChanger pChanger)
    {
        Configuration = _configuration;
        _scheduleManager = scheduleManager;
        _pChanger = pChanger;
        _scheduleManager._pChanger = _pChanger;
    }
    [HttpGet]
    [Authorize]
    [Route("/[controller]/[action]")]
    public string Index()
    {
        var scheduler = _pChanger.CurrentProject.Scheduler;
        var scenarios = _pChanger.CurrentProject.Scenarios;
        var result = new WebScheduler() { Schedule = scheduler, Scenarios = scenarios };
        return JsonConvert.SerializeObject(result);
    }
}