using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using LcsServer.Hubs;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.Managers;
using LcsServer.Models.LCProjectModels.Models.Addressing;
using LcsServer.Models.LCProjectModels.Models.Project;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LcsServer.Models.ProjectModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace LcsServer.Controllers;

public class ProjectController : Controller
{
    private RasterManager _rasterManager;
    private AddressingManager _addressingManager;
    private ScenarioManager _scenarioManager;
    private ScheduleManager _scheduleManager;
    private readonly IConfiguration Configuration;
    private ProjectChanger _pChanger;
    private string baseFolder;
    private IWebHostEnvironment Environment;
    private readonly LCHub _lcHub;
    public ProjectController(IConfiguration _configuration,
        RasterManager rastMan, AddressingManager addressingManager,
        ScenarioManager scenarioManager, ScheduleManager scheduleManager,
        ProjectChanger pChanger, IWebHostEnvironment _environment, LCHub lcHub)
    {
        Configuration = _configuration;
        _rasterManager = rastMan;
        _addressingManager = addressingManager;
        _scenarioManager = scenarioManager;
        _scheduleManager = scheduleManager;
        _pChanger = pChanger;
        Environment = _environment;
        baseFolder = Path.Combine(Environment.WebRootPath, "LcsProject");
        _lcHub = lcHub;
    }
    [HttpGet]
    
    [Route("/[controller]/[action]")]
    public string Index()
    {
        return JsonConvert.SerializeObject(_pChanger.CurrentProject);
    }
    
    public async Task<string> UploadProject(IFormFile file)
    {
        if(file != null)
        {
            try
            {
                string filePath = Path.Combine(baseFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string path = Path.Combine(baseFolder, fileName);

                if (file.FileName.Contains(".rar"))
                {
                    using (var archive = RarArchive.Open(filePath))
                    {

                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            if (entry.Key.StartsWith(fileName))
                            {
                                path = baseFolder;
                            }
                            else
                            {
                                Directory.CreateDirectory(path);
                            }

                            entry.WriteToDirectory(path, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
                else
                {
                    if(!Directory.Exists(Path.Combine(baseFolder, fileName)))
                        ZipFile.ExtractToDirectory(filePath, Path.Combine(baseFolder, fileName));
                }

                path = Path.Combine(baseFolder, fileName);
                _pChanger.ReInitProjectData(file.FileName);
                await _lcHub.ProjectChanged(_pChanger.CurrentProject);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        return Index();
    }
    [HttpPost]
    [Authorize(Roles = "admin, user")]
    [Route("/[controller]/[action]")]
    public string ChangeProjectVersion([FromBody] LcsProjectVersion sv)
    {
        if(sv == null)
            return JsonConvert.SerializeObject(NotFound());
        var newVersion = _pChanger.CurrentProject.Versions.First(f => f.Id == sv.Id);
        _pChanger.ReInitProjectData(newVersion.Name);
        return Index();
    }
}

