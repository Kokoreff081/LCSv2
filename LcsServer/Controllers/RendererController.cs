using System.Drawing;
using LcsServer.Models.LCProjectModels.Managers;
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
    public RendererController(IConfiguration _configuration, RasterManager rastMan, ScenarioManager scenarioManager)
    {
        Configuration = _configuration;
        _rasterManager = rastMan;
        _scenarioManager = scenarioManager;
        scenarioNamesIds = new List<ScenarioNameId>();
        string projectFolder = Path.Combine(Configuration.GetValue<string>("LightCadProjectsFolder"),
            Configuration.GetValue<string>("DefaultProjectFolder"));
        if(!_rasterManager.GetPrimitives<Raster>().Any())
            _rasterManager.Load(projectFolder, true);
        if(!_scenarioManager.GetPrimitives<Scenario>().Any())
            _scenarioManager.LoadScenarios(projectFolder, _rasterManager.GetPrimitives<Raster>().ToList());
        _webModel = GetScenariosAndRasters();
    }

    private WebScenariosAndRasters GetScenariosAndRasters()
    {
        var result = new WebScenariosAndRasters();
        result.Rasters = new List<WebRaster>();
        var projectRasters = _rasterManager.GetPrimitives<Raster>().ToList();
        //var frame = _scenarioPlayer.Frame;
        //var lst = new List<WebScenarios>();
        /*if (frame != null)
        {
            foreach (var item in frame.GetDictionary())
            {
                Color frameColor = Color.FromArgb(item.Value[0].Red, item.Value[0].Green, item.Value[0].Blue);
                string hexColor = $"#{frameColor.R:X2}{frameColor.G:X2}{frameColor.B:X2}";
                lst.Add(new WebScenarios() { LampId = item.Key, Color = hexColor });//, Color=item.Value });
            }
        }*/
        foreach (var raster in projectRasters)
        {
            var webRaster = new WebRaster()
            {
                Id = raster.Id,
                Name = raster.DisplayName,
                Angle = raster.Angle,
                DimensionX = raster.DimensionX,
                DimensionY = raster.DimensionY,
                IsFlipHorizontal = raster.IsFlipHorizontal,
                IsFlipVertical = raster.IsFlipVertical,
                ManualSet = raster.ManualSet,
                Rotation = raster.Rotation,
            };
            webRaster.Projections = new List<WebRasterProjection>();
            foreach (var projection in raster.Projection)
            {
                var currentcolor = /*lst.Count != 0 ? lst.Find(f => f.LampId == projection.LampId)?.Color :*/ "#3cc5e7";
                if (projection.Points.Count == 1)
                {
                    webRaster.Projections.Add(new WebRasterProjection()
                    {
                        LampId = projection.LampId,
                        ColorsCount = projection.ColorsCount,
                        PixelsCount = projection.PixelsCount,
                        RasterX = projection.Points[0].X,
                        RasterY = projection.Points[0].Y,
                        Color = currentcolor,
                        width = 10,
                        height=10,
                    });
                }
                else
                {
                    int pixelCounter = projection.Points.Count;
                    int x = 1, y = 1;   
                    int xMin = projection.Points.Min(m => m.X);
                    int yMin = projection.Points.Min(m => m.Y);
                    int pointsYZero = projection.Points.Count(w => w.Y == yMin);
                    int pointsXZero = projection.Points.Count(w => w.X == xMin);
                    if (pointsYZero > 1)
                    {
                        x = pixelCounter;
                        y = 10;
                    }
                    else if (pointsXZero > 1)
                    {
                        y = pixelCounter;
                        x = 10;
                    }
                    webRaster.Projections.Add(new WebRasterProjection()
                    {
                        LampId = projection.LampId,
                        ColorsCount = projection.ColorsCount,
                        PixelsCount = projection.PixelsCount,
                        RasterX = projection.Points[0].X,
                        RasterY = projection.Points[0].Y,
                        Color = currentcolor,
                        width = x,
                        height = y,
                    });
                }
            }


            result.Rasters.Add(webRaster);

        }

        var scenarios = _scenarioManager.GetPrimitives<Scenario>().ToList();
        if (scenarioNamesIds.Count > 0)
            scenarioNamesIds.Clear();
        foreach (var scenario in scenarios)
        {
            scenarioNamesIds.Add(new ScenarioNameId() { ScenarioId = scenario.Id, ScenarioName = scenario.Name, TotalTicks = (float)Math.Round((float)scenario.TotalTicks / 1000, 2), ElapsedTicks = /*_scenarioPlayer.IsPlay ? (float)Math.Round((float)_scenarioPlayer.ElapsedTicks / 1000, 2) :*/0f});
        }
        result.Scenarios = scenarioNamesIds;

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