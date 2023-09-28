using Acn.Rdm;
using Acn.Rdm.Packets.Product;
using Acn.Sockets;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Models;
using Sensor =LcsServer.DevicePollingService.Models.Sensor;

namespace LcsServer.DevicePollingService.SerializationModels;

public class RdmDeviceDto : BaseObjectDto
    {
        DatabaseContext _dbOperations;

        private IServiceProvider _serviceProvider;
        // For json deserialization
        public RdmDeviceDto() { }

        public RdmDeviceDto(RdmDevice model, DatabaseContext databaseOperations = null, IServiceProvider serviceProvider = null)
        {
            _serviceProvider = serviceProvider;
            _dbOperations = databaseOperations;
            List<SensorDto> sensorDtos = new List<SensorDto>();
            foreach (DevicePollingService.Models.Sensor modelSensor in model.Sensors)
            {
                SensorDto sensorDto = new SensorDto(modelSensor);
                sensorDtos.Add(sensorDto);
            }

            List<ParameterInformationDto> parameterInformationDtos = new List<ParameterInformationDto>();
            foreach (ParameterInformation parameterInformation in model.Parameters)
            {
                ParameterInformationDto parameterInformationDto = new ParameterInformationDto(parameterInformation);
                parameterInformationDtos.Add(parameterInformationDto);
            }

            ParentId = model.ParentId;
            //StatusCheckerTimerInterval = model.StatusCheckerTimerInterval;
            UId = model.UId;
            Address = model.Address;
            DevicePort = model.DevicePort;
            SubDeviceId = model.SubDeviceId;
            Manufacturer = model.Manufacturer;
            Model = model.Model;
            Label = model.Label;
            Mode = model.Mode;
            PanInvert = model.PanInvert;
            TiltInvert = model.TiltInvert;
            PanTiltSwap = model.PanTiltSwap;
            DeviceHours = model.DeviceHours;
            LampHours = model.LampHours;
            PowerCycles = model.PowerCycles;
            LampStrikes = model.LampStrikes;
            DmxAddress = model.DmxAddress;
            DmxFootprint = model.DmxFootprint;
            RdmProtocolVersion = model.RdmProtocolVersion;
            DeviceModelId = model.DeviceModelId;
            ProductCategory = model.ProductCategory;
            SoftwareVersionId = model.SoftwareVersionId;
            SoftwareVersionLabel = model.SoftwareVersionLabel;
            BootSoftwareVersionId = model.BootSoftwareVersionId;
            BootSoftwareVersionLabel = model.BootSoftwareVersionLabel;
            DmxPersonality = model.DmxPersonality;
            DmxPersonalityCount = model.DmxPersonalityCount;
            LastSeen = model.LastSeen;
            SubDeviceCount = model.SubDeviceCount;
            SensorCount = model.SensorCount;
            Sensors = sensorDtos;
            Parameters = parameterInformationDtos;
        }

        public override BaseObject ToBaseObject()
        {
            List<Sensor> sensors = Sensors.Select(sensorDto => sensorDto.ToBaseObject() as Sensor).ToList();

            List<ParameterInformation> parameters = Parameters.Select(parameterInformationDto => parameterInformationDto.ToBaseObject() as ParameterInformation).ToList();

            RdmDevice rdmDevice = new RdmDevice(null, UId, Address, DevicePort, ParentId, sensors, parameters, _dbOperations, _serviceProvider)
            {
                //StatusCheckerTimerInterval = StatusCheckerTimerInterval,
                SubDeviceId = SubDeviceId,
                Manufacturer = Manufacturer,
                Model = Model,
                Label = Label,
                Mode = Mode,
                PanInvert = PanInvert,
                TiltInvert = TiltInvert,
                PanTiltSwap = PanTiltSwap,
                DeviceHours = DeviceHours,
                LampHours = LampHours,
                PowerCycles = PowerCycles,
                LampStrikes = LampStrikes,
                DmxAddress = DmxAddress,
                DmxFootprint = DmxFootprint,
                RdmProtocolVersion = RdmProtocolVersion,
                DeviceModelId = DeviceModelId,
                ProductCategory = ProductCategory,
                SoftwareVersionId = SoftwareVersionId,
                SoftwareVersionLabel = SoftwareVersionLabel,
                BootSoftwareVersionId = BootSoftwareVersionId,
                BootSoftwareVersionLabel = BootSoftwareVersionLabel,
                DmxPersonality = DmxPersonality,
                DmxPersonalityCount = DmxPersonalityCount,
                LastSeen = LastSeen,
                SubDeviceCount = SubDeviceCount,
                SensorCount = SensorCount,
            };

            return rdmDevice;
        }

        public override string Type => nameof(RdmDevice);

        public string ParentId { get; set; }

        //public int StatusCheckerTimerInterval { get; set; }

        public UId UId { get; set; }

        public RdmEndPoint Address { get; set; }

        public byte DevicePort { get; set; }

        public int SubDeviceId { get; set; }

        public string Manufacturer { get; set; }

        public string Model { get; set; }

        public string Label { get; set; }

        public string Mode { get; set; }

        public bool? PanInvert { get; set; }

        public bool? TiltInvert { get; set; }

        public bool? PanTiltSwap { get; set; }

        public int? DeviceHours { get; set; }

        public int? LampHours { get; set; }

        public int? PowerCycles { get; set; }

        public int? LampStrikes { get; set; }

        public int? DmxAddress { get; set; }

        public int? DmxFootprint { get; set; }

        public short? RdmProtocolVersion { get; set; }

        public short? DeviceModelId { get; set; }

        public ProductCategories ProductCategory { get; set; }

        public int? SoftwareVersionId { get; set; }

        public string SoftwareVersionLabel { get; set; }

        public int? BootSoftwareVersionId { get; set; }

        public string BootSoftwareVersionLabel { get; set; }

        public byte? DmxPersonality { get; set; }

        public byte? DmxPersonalityCount { get; set; }

        public short? SubDeviceCount { get; set; }

        public DateTime LastSeen { get; set; }

        public byte? SensorCount { get; set; }

        public List<SensorDto> Sensors { get; set; }

        public List<ParameterInformationDto> Parameters { get; set; }

    }