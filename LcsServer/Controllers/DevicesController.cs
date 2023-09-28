using System.Data;
using System.Net;
using System.Text;
using Acn.ArtNet.Packets;
using Acn.Rdm;
using Acn.Rdm.Packets.Product;
using Acn.Rdm.Packets.Sensors;
using Acn.Sockets;
using LcsServer.CommandLayer;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft;
using Newtonsoft.Json;
using LcsServer.Models.DeviceModels;
using LcsServer.Models.RequestModels;
using LightControlServiceV._2.DevicePollingService.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebInterface.Controllers;

public class DevicesController : Controller
{
    private DatabaseContext _db;
    private readonly IConfiguration Configuration;
    private ToWebInterface toWeb;
    private const string RdmScan = "DeviceScanning";
    private readonly IBackgroundTaskQueue _taskQueue;
    private IServiceProvider _serviceProvider;
    public DevicesController(IConfiguration _configuration, IServiceProvider serviceProvider, IBackgroundTaskQueue taskQueue)
    {
        _serviceProvider = serviceProvider;
        //_db = context;
        Configuration = _configuration;
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            toWeb = new ToWebInterface()
            {
                DeviceScanning = _db.Settings.Where(w => w.Name == RdmScan).First().IsEnabled,
                OnlyArtNetControllers = new List<ArtnetGateWayToWeb>(),
                OnlyArtNetUniverses = new List<GatewayUniverseToWeb>()
            };
        }
        _taskQueue = taskQueue;
        FillDevices();
    }
    
    private void FillDevices()
    {
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var devices = _db.Devices.OrderBy(o => o.Type).ToList();
            var gatewayUniverseId = new List<string>();
            var artNet = new ArtnetGateWayToWeb();
            foreach (var device in devices)
            {
                var deviceParams = _db.DeviceParams
                    .Where(w => w.DeviceId == device.Id)
                    .OrderByDescending(o => o.LastPoll)
                    .ToList();

                if (device.Type == "ArtNetGateway")
                {
                    artNet.Id = device.deviceId;
                    artNet.IpAddress = deviceParams.First(f => f.ParamName == "IpAddress").ParamValue;
                    //toWeb.ParentId = item.Value.ParentId;
                    artNet.StatusId = device.StatusId; //item.Value.DeviceStatus;
                    artNet.Type = device.Type;
                    artNet.EstaCode =
                        short.Parse(deviceParams.First(f => f.ParamName == "EstaCode")
                            .ParamValue); //artnetGateway.EstaCode;
                    artNet.FirmwareVersion =
                        short.Parse(deviceParams.First(f => f.ParamName == "FirmwareVersion")
                            .ParamValue); //artnetGateway.FirmwareVersion;
                    //toWeb.GoodInput = artnetGateway.GoodInput;
                    //toWeb.GoodOutput = artnetGateway.GoodOutput;
                    artNet.LongName =
                        deviceParams.First(f => f.ParamName == "LongName")
                            .ParamValue; //artnetGateway.LongName.TrimEnd('\0');
                    artNet.MacAddress = deviceParams.First(f => f.ParamName == "MacAddress").ParamValue.Split(':')
                        .Select(x => Convert.ToByte(x, 16)).ToArray(); //artnetGateway.MacAddress;
                    artNet.NodeReport =
                        deviceParams.First(f => f.ParamName == "NodeReport")
                            .ParamValue; //artnetGateway.NodeReport.TrimEnd('\0');
                    artNet.Oem =
                        short.Parse(deviceParams.First(f => f.ParamName == "Oem").ParamValue); //artnetGateway.Oem;
                    artNet.Status =
                        (PollReplyStatus)int.Parse(deviceParams.First(f => f.ParamName == "Status")
                            .ParamValue); //artnetGateway.Status);
                    //toWeb.Status2 = artnetGateway.Status2;
                    artNet.UbeaVersion =
                        byte.Parse(deviceParams.First(f => f.ParamName == "UbeaVersion")
                            .ParamValue); //artnetGateway.UbeaVersion;
                    artNet.children = new List<GatewayUniverseToWeb>();
                    toWeb.OnlyRdmDevice = new List<RdmDeviceToWeb>();
                    toWeb.OnlyArtNetControllers.Add(artNet);
                }

                if (device.Type == "GatewayOutputUniverse" || device.Type == "GatewayInputUniverse")
                {
                    if (device.Type == "GatewayOutputUniverse")
                    {
                        var universeIndex = int.Parse(deviceParams.First(f => f.ParamName == "Index").ParamValue);
                        var universePortAddress =
                            int.Parse(deviceParams.First(f => f.ParamName == "PortAddress").ParamValue);
                        var universe = byte.Parse(deviceParams.First(f => f.ParamName == "Universe").ParamValue);
                        var universePortType =
                            (PortTypes)int.Parse(deviceParams.First(f => f.ParamName == "PortType").ParamValue);
                        var universeOutStatus =
                            (OutputStatuses)int.Parse(deviceParams.First(f => f.ParamName == "PortType").ParamValue);
                        GatewayUniverseToWeb outputUniverse = new OutputGatewayUniverseToWeb(artNet.Id,
                            IPAddress.Parse(artNet.IpAddress), universeIndex, universePortAddress, universe,
                            universePortType, universeOutStatus);
                        outputUniverse.Id = device.deviceId;
                        outputUniverse.children = new List<RdmDeviceToWeb>();
                        outputUniverse.deviceStatus = (DeviceStatuses)device.StatusId;
                        artNet.children.Add(outputUniverse);
                        gatewayUniverseId.Add(device.deviceId);
                        toWeb.OnlyArtNetUniverses.Add(outputUniverse);
                    }
                    else
                    {
                        var universeIndex = int.Parse(deviceParams.First(f => f.ParamName == "Index").ParamValue);
                        var universePortAddress =
                            int.Parse(deviceParams.First(f => f.ParamName == "PortAddress").ParamValue);
                        var universe = byte.Parse(deviceParams.First(f => f.ParamName == "Universe").ParamValue);
                        var universePortType =
                            (PortTypes)int.Parse(deviceParams.First(f => f.ParamName == "PortType").ParamValue);
                        var universeInStatus =
                            (InputStatuses)int.Parse(deviceParams.First(f => f.ParamName == "PortType").ParamValue);
                        GatewayUniverseToWeb inputUniverse = new InputGatewayUniverseToWeb(artNet.Id,
                            IPAddress.Parse(artNet.IpAddress), universeIndex, universePortAddress, universe,
                            universePortType, universeInStatus);
                        inputUniverse.Id = device.deviceId;
                        inputUniverse.children = new List<RdmDeviceToWeb>();
                        inputUniverse.deviceStatus = (DeviceStatuses)device.StatusId;
                        artNet.children.Add(inputUniverse);
                        gatewayUniverseId.Add(device.deviceId);
                        toWeb.OnlyArtNetUniverses.Add(inputUniverse);
                    }
                }

                if (device.Type == "RdmDevice")
                {
                    var rdmDev = new RdmDeviceToWeb(device.deviceId);
                    rdmDev.BootSoftwareVersionId =
                        int.Parse(deviceParams.First(f => f.ParamName == "BootSoftwareVersionId").ParamValue);
                    rdmDev.BootSoftwareVersionLabel =
                        deviceParams.First(f => f.ParamName == "BootSoftwareVersionLabel").ParamValue;
                    rdmDev.DeviceModelId =
                        short.Parse(deviceParams.First(f => f.ParamName == "DeviceModelId").ParamValue);
                    rdmDev.DmxAddress =
                        int.Parse(deviceParams.First(f => f.ParamName == "DmxAddress").ParamValue);
                    rdmDev.DmxFootprint = int.Parse(deviceParams.First(f => f.ParamName == "DmxFootprint").ParamValue);
                    rdmDev.DmxPersonality =
                        byte.Parse(deviceParams.First(f => f.ParamName == "DmxPersonality").ParamValue);
                    rdmDev.DmxPersonalityCount =
                        byte.Parse(deviceParams.First(f => f.ParamName == "DmxPersonalityCount").ParamValue);
                    rdmDev.Label = deviceParams.First(f => f.ParamName == "Label").ParamValue;
                    rdmDev.LampHours = int.Parse(deviceParams.First(f => f.ParamName == "LampHours").ParamValue);
                    rdmDev.LastSeen = DateTime.Parse(deviceParams.First(f => f.ParamName == "LastSeen").ParamValue);
                    try
                    {
                        rdmDev.LampStrikes =
                            int.Parse(deviceParams.First(f => f.ParamName == "LampStrikes").ParamValue);
                    }
                    catch (Exception e)
                    {
                        rdmDev.LampStrikes = 0;
                    }

                    rdmDev.Manufacturer = deviceParams.First(f => f.ParamName == "Manufacturer").ParamValue;
                    rdmDev.Model = deviceParams.First(f => f.ParamName == "Model").ParamValue;
                    rdmDev.PowerCycles = int.Parse(deviceParams.First(f => f.ParamName == "PowerCycles").ParamValue);
                    rdmDev.ProductCategory =
                        (ProductCategories)int.Parse(deviceParams.First(f => f.ParamName == "ProductCategory")
                            .ParamValue);
                    rdmDev.RdmProtocolVersion =
                        short.Parse(deviceParams.First(f => f.ParamName == "RdmProtocolVersion").ParamValue);
                    rdmDev.SensorCount = byte.Parse(deviceParams.First(f => f.ParamName == "SensorCount").ParamValue);
                    if (rdmDev.SensorCount > 0)
                    {
                        var sensorsInDb = _db.Sensors.Where(w => w.deviceId == rdmDev.Id).OrderByDescending(o=>o.LastPoll).ToList();
                        var lst = new List<Sensor>();
                        foreach (var _dbSensor in sensorsInDb)
                        {
                            var lastValues = _db.SensorValues.Where(w => w.SensorId == _dbSensor.SensorId)
                                .OrderByDescending(o => o.Timestamp).First();
                            lst.Add(new Sensor(_dbSensor.SensorId, rdmDev.Id)
                            {
                                Description = _dbSensor.description,
                                Unit = (SensorDefinition.SensorUnit)_dbSensor.SensorUnitId,
                                NormalMaxValue = lastValues.NormalMaxValue, NormalMinValue = lastValues.NormalMinValue,
                                PresentValue = (short)lastValues.Value,
                                SensorNumber = (byte)_dbSensor.SensorNumber
                            });
                        }

                        rdmDev.Sensors = lst;
                    }

                    rdmDev.SoftwareVersionId =
                        int.Parse(deviceParams.First(f => f.ParamName == "SoftwareVersionId").ParamValue);
                    rdmDev.SoftwareVersionLabel =
                        deviceParams.First(f => f.ParamName == "SoftwareVersionLabel").ParamValue;
                    rdmDev.SubDeviceCount =
                        short.Parse(deviceParams.First(f => f.ParamName == "SubDeviceCount").ParamValue);
                    rdmDev.deviceStatus = (DeviceStatuses)device.StatusId;
                    var pars = deviceParams.Where(w => w.ParamId != "").ToList();
                    var listPars = new List<ParameterInformationToWeb>();
                    foreach (var par in pars)
                    {
                        listPars.Add(new ParameterInformationToWeb((RdmParameters)int.Parse(par.ParamId), device.deviceId){Description = par.ParamName, Value = par.ParamValue});
                    }

                    rdmDev.Parameters = listPars;
                    var gatewayUniverse = artNet.children.Where(w => device.ParentId == w.Id).FirstOrDefault();
                    gatewayUniverse.children.Add(rdmDev);
                    
                    toWeb.OnlyRdmDevice.Add(rdmDev);
                }
            }

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new IPEndPointConverter());
            settings.Converters.Add(new IPAddressConverter());
            settings.Formatting = Formatting.Indented;
            
            toWeb.ToTreeTable = MakeToTreeTableList(artNet);//new List<NewToWebDevices>();
        }

        
    }
    [Authorize(Roles = "admin, user")]
    public string Index()
    {
        var rdmDisabled = Configuration.GetValue<bool>("RdmDiscoveryForbiddenGlobal");
        if (rdmDisabled)
        {
            var json = new JObject
                { ["status"] = StatusCodes.Status400BadRequest };
               
            return JsonConvert.SerializeObject(NotFound());
        }
        var settings = new JsonSerializerSettings();
        settings.Converters.Add(new IPEndPointConverter());
        settings.Converters.Add(new IPAddressConverter());
        settings.Formatting = Formatting.Indented;
        var result = JsonConvert.SerializeObject(toWeb,settings);
        
        return result;
    }
    private List<NewToWebDevices> MakeToTreeTableList(ArtnetGateWayToWeb artNet)
    {
        var result = new List<NewToWebDevices>();
        var listUniverses = artNet.children;
        var firstLevel = new NewToWebDevices()
        {
            key = "0",
            data = new ColumnsToWebDevices()
            {
                name = artNet.LongName,
                label = "",
                deviceStatus = (DeviceStatuses)artNet.StatusId,
                dmxAddress = null,
                DmxFootprint = null,
                //IsInProject = artNet.IsInProject,
                softwareVersionIdLabel = "",
                Id = artNet.Id,
                Type = artNet.Type

            },
            children = new List<NewToWebDevices>()
        };
        for(int i = 0; i<listUniverses.Count; i++)
        {
            var item = listUniverses[i];
            var rdmList = new List<NewToWebDevices>();
            int counter = 0;
            string port = item.PortType == PortTypes.IsInputPort ? "Input" : "Output";
            string name = "DMX "+port+" "+item.PortIndex + " Address: "+item.PortAddress;
            foreach(var rdm in item.children)
            {
                rdmList.Add(new NewToWebDevices()
                {
                    key = "0-" + i.ToString() + "-" + counter.ToString(),
                    data = new ColumnsToWebDevices() { 
                        name = rdm.DisplayName,
                        label = rdm.Label,
                        deviceStatus = rdm.deviceStatus,
                        dmxAddress = rdm.DmxAddress,
                        DmxFootprint = rdm.DmxFootprint,
                        softwareVersionIdLabel = rdm.SoftwareVersionIdLabel,
                        Id = rdm.Id,
                        Type = rdm.Type
                    }
                }) ;
                counter++;
            }
            firstLevel.children.Add(new NewToWebDevices()
            {
                key = "0-" + i.ToString(),
                data = new ColumnsToWebDevices()
                {
                    name = name,
                    label = "",
                    deviceStatus = item.deviceStatus,
                    dmxAddress = null,
                    DmxFootprint = null,
                    softwareVersionIdLabel = "",
                    Id = item.Id,
                    Type = item.Type
                },
                children = rdmList
            }) ;
        }
        result.Add(firstLevel);
        return result;
    }
    
    [HttpPost]
    [Authorize(Roles = "admin")]
    public  void StartStopDiscovery([FromBody] StartStopScheduler s3)
    {
        var cmd = new Command()
        {
            CommandType = (int)CommandTypes.StartStopDiscovering, State = 0, DeviceId = "", ParamNewValue = s3.action.ToString(),
            ParamId = -1, UserLogin = User.Identity.Name
        };
        _taskQueue.QueueBackgroundWorkItemAsync(cmd);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<string> ChangeDmxAddress([FromBody] NewDmxAddress nda)
    {
        if (string.IsNullOrEmpty(nda.id)){
            var json = new JObject
                { ["status"] = StatusCodes.Status400BadRequest };
               
            return JsonConvert.SerializeObject(json);
        }
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var device = _db.Devices.First(f => f.deviceId == nda.id);
            var deviceParam = _db.DeviceParams
                .Where(w => w.DeviceId == device.Id && w.ParamName == "DmxAddress")
                .OrderByDescending(o => o.LastPoll)
                .First();
            var cmd = new Command()
            {
                CommandType = (int)CommandTypes.ChangeDmxAddress,
                State = 0,
                DeviceId = nda.id,
                ParamId = deviceParam.Id,
                ParamNewValue = nda.newDmxAddress,
                UserLogin = User.Identity.Name
            };
            _taskQueue.QueueBackgroundWorkItemAsync(cmd);
        }

        return Index();
    }
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<string> ChangeLabel([FromBody] ChangeRdmLabel crl)
    {
        if (string.IsNullOrEmpty(crl.id)){
            var json = new JObject
                { ["status"] = StatusCodes.Status400BadRequest };
               
            return JsonConvert.SerializeObject(json);
        }
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var device = _db.Devices.First(f => f.deviceId == crl.id);
            var deviceParam = _db.DeviceParams
                .Where(w => w.DeviceId == device.Id && w.ParamName == "Label")
                .OrderByDescending(o => o.LastPoll)
                .First();
            var cmd = new Command()
            {
                CommandType = (int)CommandTypes.ChangeLabel,
                State = 0,
                DeviceId = crl.id,
                ParamId = deviceParam.Id,
                ParamNewValue = crl.newLabel,
                UserLogin = User.Identity.Name
            };
            _taskQueue.QueueBackgroundWorkItemAsync(cmd);
        }

        return Index();
    }
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<string> LampIdentity([FromBody]LampHighlight lamp)
    {
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var device = _db.Devices.First(f => f.deviceId == lamp.id);
            var deviceParam = _db.DeviceParams
                .Where(w => w.DeviceId == device.Id && w.ParamName == "Label")
                .OrderByDescending(o => o.LastPoll)
                .First();
            //var id = HttpContext.Request.Body;
            if (string.IsNullOrEmpty(lamp.id))
            {
                var json = new JObject
                    { ["status"] = StatusCodes.Status400BadRequest };
               
                return JsonConvert.SerializeObject(json);
            }
               
            var cmd = new Command();
            cmd.CommandType = (int)CommandTypes.IdentifyOnOff;
            cmd.DeviceId = lamp.id;
            cmd.ParamNewValue = lamp.isOn.ToString();
            cmd.ParamId = deviceParam.Id;
            cmd.State = 0;
            cmd.UserLogin = User.Identity.Name;

            _taskQueue.QueueBackgroundWorkItemAsync(cmd);
        }

        return Index();
    }
    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<string> ChangeParameter([FromBody] ParamToChange cp)
    {
        if (string.IsNullOrEmpty(cp.id)){
            var json = new JObject
                { ["status"] = StatusCodes.Status400BadRequest };
           
            return JsonConvert.SerializeObject(json);
        }

        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var device = _db.Devices.First(f => f.deviceId == cp.id);
            var deviceParam = _db.DeviceParams
                .Where(w => w.DeviceId == device.Id && w.ParamId == cp.parameterId.ToString())
                .OrderByDescending(o => o.LastPoll)
                .First();
            var cmd = new Command()
            {
                CommandType = (int)CommandTypes.ChangeLabel,
                State = 0,
                DeviceId = cp.id,
                ParamId = deviceParam.Id,
                ParamNewValue = cp.newValue,
                UserLogin = User.Identity.Name
            };
            _taskQueue.QueueBackgroundWorkItemAsync(cmd);
        }

        return Index();
    }
}