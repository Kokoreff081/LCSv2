using System.IO;
using LcsServer.Models.LCProjectModels.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LcsServer.Controllers;

public class ProjectController : Controller
{
    private RasterManager _rasterManager;
    private readonly IConfiguration Configuration;
    public ProjectController(IConfiguration _configuration, RasterManager rastMan)
    {
        Configuration = _configuration;
        _rasterManager = rastMan;
        string projectFolder = Path.Combine(Configuration.GetValue<string>("LightCadProjectsFolder"),
            Configuration.GetValue<string>("DefaultProjectFolder"));
        _rasterManager.Load(projectFolder, true);
        int point = 0;
    }

    public string Index()
    {
        return "hello";
    }
}