using System.Drawing;
using LcsServer.Models.LCProjectModels.Managers;
using LcsServer.Models.LCProjectModels.Models.Project;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LcsServer.Models.ProjectModels;
using LightControlServiceV._2.DevicePollingService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LcsServer.Controllers;

public class RendererController: Controller
{
    private RasterManager _rasterManager;
    private ScenarioManager _scenarioManager;
    private readonly IConfiguration Configuration;
    private WebScenariosAndRasters _webModel;
    private readonly List<ScenarioNameId> scenarioNamesIds;
    private ProjectChanger _pChanger;
    public RendererController(IConfiguration _configuration, RasterManager rastMan, ScenarioManager scenarioManager, ProjectChanger pChanger)
    {
        Configuration = _configuration;
        _pChanger = pChanger;
        /*_rasterManager = rastMan;
        _scenarioManager = scenarioManager;
        scenarioNamesIds = new List<ScenarioNameId>();
        string projectFolder = Path.Combine(Configuration.GetValue<string>("LightCadProjectsFolder"),
            Configuration.GetValue<string>("DefaultProjectFolder"));
        if(!_rasterManager.GetPrimitives<Raster>().Any())
            _rasterManager.Load(projectFolder, true);
        if(!_scenarioManager.GetPrimitives<Scenario>().Any())
            _scenarioManager.LoadScenarios(projectFolder, _rasterManager.GetPrimitives<Raster>().ToList());*/
        _webModel = GetScenariosAndRasters();
    }

    private WebScenariosAndRasters GetScenariosAndRasters()
    {
        var result = new WebScenariosAndRasters();
        result.Rasters = _pChanger.CurrentProject.Rasters;
        result.Scenarios = _pChanger.CurrentProject.Scenarios;
        return result;
    }
    
    /// <summary>
    /// Get all rasters and scenarios from project
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    [Route("/[controller]/[action]")]
    public string Index()
    {
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new IPEndPointConverter());
        settings.Converters.Add(new IPAddressConverter());
        //settings.Formatting = Formatting.Indented;
        var result = JsonConvert.SerializeObject(_webModel, settings);
        
        return result;
    }
}