using System.Globalization;
using System.IO.Compression;
using System.Net.Mime;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.Managers;
using LcsServer.Models.LCProjectModels.Models.Addressing;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LcsServer.Models.ProjectModels;
using LLcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LcsServer.Models.LCProjectModels.Models.Project;

public class ProjectChanger
{
    private readonly IConfiguration Configuration;
    private readonly RasterManager _rasterManager;
    private readonly ScenarioManager _scenarioManager;
    private readonly ScheduleManager _scheduleManager;
    private readonly List<ScenarioNameId> scenarioNamesIds;
   // private readonly List<ScheduleItemFront> scheduleFrontItems;
    private readonly List<LCLampsFront> lcLampsFront;
    private readonly AddressingManager _addressingManager;
    public ProjectToWeb CurrentProject;
    //private readonly LCHub _lcHub;

    public ProjectChanger(IConfiguration _configuration,
        RasterManager rasterManager,
        ScenarioManager scenarioManager,
        ScheduleManager scheduleManager,
        AddressingManager addressingManager)
    {
        Configuration = _configuration;
        _rasterManager = rasterManager;
        _scenarioManager = scenarioManager;
        _scheduleManager = scheduleManager;
        _addressingManager = addressingManager;
        scenarioNamesIds = new List<ScenarioNameId>();
        /*scheduleFrontItems = new List<ScheduleItemFront>();
        schedulerFilesFront = new List<SchedulerFilesFront>();*/
        lcLampsFront = new List<LCLampsFront>();
        CurrentProject = new ProjectToWeb();
        
        InitProjectData();
        FillCurProject();
    }

    private void InitProjectData()
    {
        string projectFolder = Path.Combine(Configuration.GetValue<string>("LightCadProjectsFolder"),
            Configuration.GetValue<string>("DefaultProjectFolder"));
        string lcsFolderPath = FileManager.GetLCSProjectFolderPath(projectFolder);
        var files = Directory.GetFiles(lcsFolderPath);
        if (files.Length == 0)
        {
            throw new Exception("No lcs project files found!");
        }

        var tmpFile = new FileInfo(files[0]);
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            var stringTmpDateTime = tmpFile.Name.Split('_')[1].Split('.')[0];
            var stringDateTime = fileInfo.Name.Split('_')[1].Split('.')[0];
            var tmpDateAndTime = DateTime.ParseExact(stringTmpDateTime, "yyyy-MM-dd-HHmmss", CultureInfo.InvariantCulture);
            var dateAndTime = DateTime.ParseExact(stringDateTime, "yyyy-MM-dd-HHmmss", CultureInfo.InvariantCulture);
            if (tmpDateAndTime < dateAndTime)
                tmpFile = fileInfo;
        }

        var baseFolder = Path.Combine(AppContext.BaseDirectory, "LcsProject");
        var currentFolder = Path.Combine(baseFolder, Path.GetFileNameWithoutExtension(tmpFile.Name));
        if(!Directory.Exists(currentFolder))
            ZipFile.ExtractToDirectory(tmpFile.FullName, Path.Combine(baseFolder, Path.GetFileNameWithoutExtension(tmpFile.Name)));

        var dataFiles = Directory.GetFiles(currentFolder);
        foreach (var file in dataFiles)
        {
            var fi = new FileInfo(file);
            if (fi.Extension != FileManager.ProjectLCSFilenameExtension)
            {
                switch (fi.Name)
                {
                    case "rasters.json":
                        _rasterManager.Load(fi.FullName, true);
                        break;
                    case "scenarios.json":
                        _scenarioManager.LoadScenarios(fi.FullName, _rasterManager.GetPrimitives<Raster>().ToList());
                        break;
                    case "addressing.json":
                        _addressingManager.Load(fi.FullName, true);
                        var lamps = _addressingManager.GetPrimitives<LCAddressLamp>().ToList();
                        var json = string.Empty;
                        if (File.Exists(Path.Combine(currentFolder, "lampsIdNames.json")))
                        {
                            string text = File.ReadAllText(Path.Combine(currentFolder, "lampsIdNames.json"));
                            var lampsDict = JsonConvert.DeserializeObject<Dictionary<int, string>>(text);
                            foreach (var lamp in lamps)
                            {
                                lamp.Name = lampsDict[lamp.Id];
                            }
                        }

                        break;
                    case "scheduler.json":
                        _scheduleManager.Load(fi.FullName, false);
                        break;
                    case "projectInfo.json":
                        CurrentProject.ProjectInfo = JsonConvert.DeserializeObject<ProjectInfo>(File.ReadAllText(file));
                        break;
                    
                }
            }
        }
    }

    private void FillCurProject()
    {
        
        var projectRasters = _rasterManager.GetPrimitives<Raster>().ToList();
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
            CurrentProject.Rasters.Add(webRaster);
        }

        var scenarios = _scenarioManager.GetPrimitives<Scenario>().ToList();
        if (scenarioNamesIds.Count > 0)
            scenarioNamesIds.Clear();
        foreach (var scenario in scenarios)
        {
            scenarioNamesIds.Add(new ScenarioNameId()
            {
                ScenarioId = scenario.Id,
                ScenarioName = scenario.Name,
                TotalTicks = (float)Math.Round((float)scenario.TotalTicks / 1000, 2),
                ElapsedTicks = /*_scenarioPlayer.IsPlay ? (float)Math.Round((float)_scenarioPlayer.ElapsedTicks / 1000, 2) :*/0f
            });
        }
        CurrentProject.Scenarios = scenarioNamesIds;
        if (lcLampsFront.Count > 0)
                lcLampsFront.Clear();
        var addressObjects = _addressingManager.GetPrimitives<LCObject>().ToList();
        List<LCAddressLamp> addressLamps = addressObjects.OfType<LCAddressLamp>().ToList();
       // var fullInfoLamp = _sceneManager.GetPrimitives<LCLamp>().ToList();
        foreach (LCAddressDevice device in addressObjects.OfType<LCAddressDevice>())
        {
            LCDevice dcoDevice = device switch
            {
                LCArtNetAddressDevice artNetDevice => new LCArtNetDevice(artNetDevice.IpAddress),
                //LCAddressDevice lcsDevice => new LCDevice(lcsDevice.UsbInstance, lcsDevice.Speed.ToSpeedTypes()),
                _ => throw new NotSupportedException("Device is not supported")
            };
            string ipAddress = string.Empty;
            string longName = string.Empty;
            if (dcoDevice is LCArtNetDevice item)
            {
                ipAddress = item.IpAddress.ToString();
                longName = item.ToString();
            }
            LCLampsFront lclamp = new LCLampsFront()
            {
                Id = device.Id,
                IpAddress = ipAddress,
                Name = longName,
                Type = "ArtNetDevice",
                Children = new List<FrontPort>(),
            };
            foreach (LCAddressDevicePort devicePort in addressObjects.OfType<LCAddressDevicePort>()
                         .Where(x => x.ParentId == device.Id))
            {
                var adressedLamps = addressLamps.Where(x => x.ParentId == devicePort.Universe.Id).ToList();
                lclamp.Children.Add(new FrontPort()
                {
                    Id = devicePort.Universe.Id,
                    ParentId = devicePort.ParentId,
                    Type = "Port",
                    Name = devicePort.DisplayName,
                    DmxSize = devicePort.DmxSize,
                    /*Children = fullInfoLamp
                        .Where(w=>adressedLamps.Contains(w.AddressData))
                        .Select(s => new LCLampFront { 
                            Id = s.Id,
                            Name = s.Name,
                            AddressData = s.AddressData,
                            IpAddress=lclamp.IpAddress,
                            ParentPort= "Port_" + devicePort.PortNumber,
                            Type = s.LampDescriptor.Vendor.Series,
                            ColorsCount = s.AddressData.ColorsCount,
                            LampAddress = s.AddressData.LampAddress
                        })
                        .ToList()*/
                });
            }
            lcLampsFront.Add(lclamp);
        }
        CurrentProject.OnlyProjectLamps = new List<LCLampFront>();
        /*foreach(var item in lcLampsFront)
        {
            foreach(var child in item.Children)
            {
                CurrentProject.OnlyProjectLamps.AddRange(child.Children);
            }
        }*/
        CurrentProject.LCLamps = lcLampsFront;
        
        
    }
}