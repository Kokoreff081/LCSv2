﻿using System.IO;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.Managers;
using LcsServer.Models.LCProjectModels.Models.Addressing;
using LcsServer.Models.LCProjectModels.Models.Project;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LcsServer.Controllers;

public class ProjectController : Controller
{
    private RasterManager _rasterManager;
    private AddressingManager _addressingManager;
    private ScenarioManager _scenarioManager;
    private ScheduleManager _scheduleManager;
    private readonly IConfiguration Configuration;
    private ProjectChanger _pChanger;
    public ProjectController(IConfiguration _configuration, RasterManager rastMan, AddressingManager addressingManager, ScenarioManager scenarioManager, ScheduleManager scheduleManager, ProjectChanger pChanger)
    {
        Configuration = _configuration;
        _rasterManager = rastMan;
        _addressingManager = addressingManager;
        _scenarioManager = scenarioManager;
        _scheduleManager = scheduleManager;
        _pChanger = pChanger;
        string projectFolder = Path.Combine(Configuration.GetValue<string>("LightCadProjectsFolder"),
            Configuration.GetValue<string>("DefaultProjectFolder"));
        if(!_rasterManager.GetPrimitives<Raster>().Any())
            _rasterManager.Load(projectFolder, true);
        if(!_scenarioManager.GetPrimitives<Scenario>().Any())
            _scenarioManager.LoadScenarios(projectFolder, _rasterManager.GetPrimitives<Raster>().ToList());
       // if(!_addressingManager.GetPrimitives<Scenario>().Any())
        _addressingManager.Load(projectFolder, true);
        int point = 0;
    }

    public string Index()
    {

        var jObject = new JObject()
        {
            ["rasters"] = JsonConvert.SerializeObject(_rasterManager.GetPrimitives<Raster>(),  
                new JsonSerializerSettings { 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
                }),
            ["scenarios"] = JsonConvert.SerializeObject(_scenarioManager.GetPrimitives<Scenario>(), 
                new JsonSerializerSettings { 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
                }),
            ["adressing"] = JsonConvert.SerializeObject(_addressingManager.GetPrimitives<LCAddressObject>(), 
                new JsonSerializerSettings { 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
                })   
        };
        return
            jObject.ToString(); //JsonConvert.SerializeObject(json, Formatting.Indented, new JsonSerializerSettings { 
        //ReferenceLoopHandling = ReferenceLoopHandling.Ignore 
        //});
    }
}

