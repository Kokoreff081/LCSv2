using System.Diagnostics;
using System.Text;
using Acn.Rdm;
using Acn.Rdm.Packets.Control;
using Acn.Rdm.Packets.DMX;
using Acn.Rdm.Packets.Power;
using Acn.Rdm.Packets.Product;
using Acn.Sockets;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;
using LLcsServer.DevicePollingService.Models;
using Sensor = LcsServer.DevicePollingService.Models.Sensor;

namespace LcsServer.Models.DeviceModels;

public class RdmDeviceToWeb
{
    private int _subDeviceId;
    private string _id;
    private string _manufacturer;
    private string _model;
    private string _label;
    private int? _dmxAddress;
    private int? _dmxFootprint;
    private string _mode;
    private bool? _panInvert;
    private bool? _tiltInvert;
    private bool? _panTiltSwap;
    private int? _deviceHours;
    private int? _lampHours;
    private int? _powerCycles;
    private int? _lampStrikes;
    private short? _rdmProtocolVersion;
    private short? _deviceModelId;
    private ProductCategories _productCategory;
    private int? _softwareVersionId;
    private string _softwareVersionLabel;
    private int? _bootSoftwareVersionId;
    private string _bootSoftwareVersionLabel;
    private bool? _isIdentifyOn;
    private byte? _dmxPersonality;
    private byte? _dmxPersonalityCount;
    private short? _subDeviceCount;
    private byte? _sensorCount;
    private readonly Dictionary<string, Sensor> _sensors;
    private readonly Dictionary<string, ParameterInformation> _parameters;
    private readonly Dictionary<RdmParameters, string> _unsupportedParameters = new Dictionary<RdmParameters, string>();
    private readonly Dictionary<byte, PersonalityDescription> _personalityDescriptions = new Dictionary<byte, PersonalityDescription>();
    private DateTime _lastSeen;
    private DatabaseContext _db;

    public RdmDeviceToWeb(string id) 
    {
        Id = id;
        _sensors = new Dictionary<string, Sensor>();
        _parameters = new Dictionary<string, ParameterInformation>();
        
    }

    public DeviceStatuses deviceStatus { get; set; }

    //public bool ForceUpdate { get; internal set; }

    public RdmDeviceBroker RdmDeviceBroker { get; set; }

    public string Type => nameof(RdmDevice);

    public string Id{
        get { return _id; }
        set { _id = value; }
    }
    public UId UId { get; }

    public RdmEndPoint Address { get; }

    public byte DevicePort { get; }

    public int SubDeviceId
    {
        get => _subDeviceId;
        set
        {
            _subDeviceId = value;
        }
    }

    public string Manufacturer
    {
        get => _manufacturer;
        set
        {
            _manufacturer = value;
        }
    }

    public string Model
    {
        get => _model;
        set
        {
            _model = value;
        }
    }

    public string DisplayName
    {
        get
        {
            if (string.IsNullOrEmpty(Model))
                return UId.ToString();

            return Model;
        }
    }
    public string DeviceName { get; set; }
    public string Label
    {
        get => _label;
        set { 
            _label = value;
        }
    }

    public bool IsInProject { get; set; }

    public void ChangeLabelRequest(string newValue)
    {
        DeviceLabel.Set setLabel = new DeviceLabel.Set();
        setLabel.Label = newValue;
        RdmDeviceBroker.ForceSendRdm(setLabel, Address, UId);

        Label = null;

        // Запускаем задачу на получение лейбла через 1 сек., чтобы запрос на изменение лейбла успел отработать
        Task.Delay(1000).ContinueWith((x) =>
        {
            RdmDeviceBroker.ForceRequestLabel(Address, UId);
        });

    }

    public void ChangeDmxAddress(int dmxAddress)
    {
        var logInfo = new Event();
        logInfo.deviceId = Id;
        logInfo.level = "Info";
        logInfo.Description = $"device {DisplayName}-{Address.Address} dmx address changed from {DmxAddress} to {dmxAddress}";
        logInfo.dateTime = DateTime.Now;
            _db.Events.Add(logInfo);
            _db.SaveChanges();
        

        DmxStartAddress.Set setDmxStartAddress = new DmxStartAddress.Set();
        setDmxStartAddress.DmxAddress = (short)dmxAddress;
        RdmDeviceBroker.ForceSendRdm(setDmxStartAddress, Address, UId);

        DmxAddress = null;

        // Запускаем задачу на получение dmxAddress через 1 сек., чтобы запрос на изменение dmxAddress успел отработать
        Task.Delay(1000).ContinueWith((x) =>
        {
            RdmDeviceBroker.ForceRequestDmxAddress(Address, UId);
        });
    }


    public void SetDeviceInformation(DeviceInfo.GetReply deviceInfo)
    {
        DmxAddress = deviceInfo.DmxStartAddress;

        DmxFootprint = deviceInfo.DmxFootprint;

        RdmProtocolVersion = deviceInfo.RdmProtocolVersion;

        DeviceModelId = deviceInfo.DeviceModelId;

        ProductCategory = deviceInfo.ProductCategory;

        SoftwareVersionId = deviceInfo.SoftwareVersionId;

        DmxPersonality = deviceInfo.DmxPersonality;

        DmxPersonalityCount = deviceInfo.DmxPersonalityCount;

        SubDeviceCount = deviceInfo.SubDeviceCount;

        SensorCount = deviceInfo.SensorCount;
    }

    public string Mode
    {
        get => _mode;
        set
        {
            _mode = value;
        }
    }

    public List<PersonalitySlotInformation> PersonalitySlots { get; } = new List<PersonalitySlotInformation>();

    public IEnumerable<PersonalityDescription> PersonalityDescriptions => _personalityDescriptions.Values;

    public bool? PanInvert
    {
        get => _panInvert;
        set
        {
            _panInvert = value;
        }
    }

    public bool? TiltInvert
    {
        get => _tiltInvert;
        set
        {
            _tiltInvert = value;
        }
    }

    public bool? PanTiltSwap
    {
        get => _panTiltSwap;
        set
        {
            _panTiltSwap = value;
        }
    }

    public int? DeviceHours
    {
        get => _deviceHours;
        set
        {
            _deviceHours = value;
        }
    }

    public int? LampHours
    {
        get => _lampHours;
        set
        {
            _lampHours = value;
        }
    }

    public void ChangeLampHours(int lampHours)
    {
        LampHours.Set setLampHours = new LampHours.Set();
        setLampHours.LampHours = lampHours;
        RdmDeviceBroker.ForceSendRdm(setLampHours, Address, UId);

        LampHours = null;

        // Запускаем задачу на получение LampHours через 1 сек., чтобы запрос на изменение LampHours успел отработать
        Task.Delay(1000).ContinueWith((x) =>
        {
            RdmDeviceBroker.ForceRequestLampHours(Address, UId);
        });
    }

    public int? PowerCycles
    {
        get => _powerCycles;
        set
        {
            _powerCycles = value;
        }
    }

    public int? LampStrikes
    {
        get => _lampStrikes;
        set
        {
            _lampStrikes = value;
        }
    }

    public bool? IsIdentifyOn
    {
        get => _isIdentifyOn;
        internal set => _isIdentifyOn = value;
    }


    #region Device Information

    public int? DmxAddress
    {
        get => _dmxAddress;
        set
        {
            _dmxAddress = value;
        }
    }

    public int? DmxFootprint
    {
        get => _dmxFootprint;
        set
        {
            _dmxFootprint = value;
        }
    }

    public short? RdmProtocolVersion
    {
        get => _rdmProtocolVersion;
        set
        {
            _rdmProtocolVersion = value;
        }
    }

    public short? DeviceModelId
    {
        get => _deviceModelId;
        set
        {
            _deviceModelId = value;
        }
    }

    public ProductCategories ProductCategory
    {
        get => _productCategory;
        set
        {
            _productCategory = value;
        }
    }

    public int? SoftwareVersionId
    {
        get => _softwareVersionId;
        set
        {
            _softwareVersionId = value;
        }
    }

    public string SoftwareVersionIdLabel
    {
        get
        {
            if (SoftwareVersionId == null)
                return null;

            byte ver1 = (byte)SoftwareVersionId;
            byte ver2 = (byte)((uint)SoftwareVersionId >> 8);

            return $"{ver2}.{ver1}";
        }
    }

    public string SoftwareVersionLabel
    {
        get => _softwareVersionLabel;
        set
        {
            _softwareVersionLabel = value;
        }
    }

    public int? BootSoftwareVersionId
    {
        get => _bootSoftwareVersionId;
        set
        {
            _bootSoftwareVersionId = value;
        }
    }

    public string BootSoftwareVersionLabel
    {
        get => _bootSoftwareVersionLabel;
        set
        {
            _bootSoftwareVersionLabel = value;
        }
    }

    public byte? DmxPersonality
    {
        get => _dmxPersonality;
        set
        {
            _dmxPersonality = value;
        }
    }

  

    public byte? DmxPersonalityCount
    {
        get => _dmxPersonalityCount;
        set
        {
            _dmxPersonalityCount = value;
        }
    }

    public short? SubDeviceCount
    {
        get => _subDeviceCount;
        set
        {
            _subDeviceCount = value;
        }
    }

    public byte? SensorCount
    {
        get => _sensorCount;
        set
        {
            _sensorCount = value;
        }
    }

    public DateTime LastSeen
    {
        get => _lastSeen;
        set { _lastSeen = value; }
    }

    public string IpAddress { get; set; }

    public string ParentPort { get; set; }

    #endregion

    #region Sensors

    public IEnumerable<Sensor> Sensors { get; set; }//=> _sensors.Values;

    public void SensorFound(Sensor sensor)
    {
        AddSensor(sensor);
    }

    private void AddSensor(Sensor sensor)
    {
        
        if (_sensors.ContainsKey(sensor.Id))
        {
            Sensor currentSensor = _sensors[sensor.Id];
            currentSensor.SensorNumber = sensor.SensorNumber;
            currentSensor.Description = sensor.Description;
            currentSensor.NormalMaxValue = sensor.NormalMaxValue;
            currentSensor.NormalMinValue = sensor.NormalMinValue;
            currentSensor.Prefix = sensor.Prefix;
            currentSensor.RangeMaxValue = sensor.RangeMaxValue;
            currentSensor.RangeMinValue = sensor.RangeMinValue;
            currentSensor.RecordValueSupport = sensor.RecordValueSupport;
            currentSensor.SensorType = sensor.SensorType;
            currentSensor.Unit = sensor.Unit;
            
        }
        else
        {
            _sensors.Add(sensor.Id, sensor);
        }
        
    }

    public void SensorValueReceived(string sensorId, short presentValue, short minValue, short maxValue, short recordedValue)
    {
        if (!_sensors.ContainsKey(sensorId))
        {
            Debug.WriteLine("The sensor was not found");
            return;
        }

        Sensor sensor = _sensors[sensorId];
        sensor.PresentValue = presentValue;
        sensor.MinValue = minValue;
        sensor.MaxValue = maxValue;
        sensor.RecordedValue = recordedValue;

        if (sensor.PresentValueIsOutOfNormalRange)
        {
            //DeviceStatus = DeviceStatuses.Warning;
            /*SensorLogMessage logMessage = new SensorLogMessage
            {
                UID = UId.ToString(),
                IpAddress = Address.IpAddress.ToString(),
                Universe = Address.Universe,
                DmxAddress = DmxAddress == null ? "null" : DmxAddress.Value.ToString(),
                SensorDescription = $"{sensor.Description} ({sensor.Unit})",
                NormalMinRange = sensor.NormalMinValue,
                NormalMaxRange = sensor.NormalMaxValue,
                PresentValue = sensor.PresentValue,
            };
            LogManager.GetInstance().WriteDeviceMessage(logMessage);*/
        }


            var sensorUnitName = sensor.Unit.ToString();
            var sensorUnitId =
                _db.SensorUnits.ToList().First(f => f.Name == sensorUnitName).Id; //GetSensorUnitId(sensor);
            var sensorVal = new SensorValue()
            {
                SensorId = sensor.Id,
                NormalMinValue = sensor.NormalMinValue,
                NormalMaxValue = sensor.NormalMaxValue,
                Value = sensor.SensorValue,
                SensorUnitId = (int)sensorUnitId,
                Timestamp = DateTime.Now,
            };
            _db.SensorValues.Add(sensorVal);
            _db.SaveChanges();
        
    }

    #endregion

    #region Parameters
    public IEnumerable<ParameterInformationToWeb> Parameters { get; set; }//=> _parameters.Values;

    public void AddParameterIfNotExists(ParameterInformation parameterInformation)
    {
        if (_parameters.ContainsKey(parameterInformation.Id))
            return;

        _parameters.Add(parameterInformation.Id, parameterInformation);
    }

    public void SetParameterInformation(ParameterInformation parameterInformation)
    {
        if (!_parameters.ContainsKey(parameterInformation.Id))
            return;

        ParameterInformation currentParam = _parameters[parameterInformation.Id];
        currentParam.Description = parameterInformation.Description;
        currentParam.Prefix = parameterInformation.Prefix;
        currentParam.Unit = parameterInformation.Unit;
        currentParam.ParameterType = parameterInformation.ParameterType;
        currentParam.CommandClass = parameterInformation.CommandClass;
        currentParam.DataType = parameterInformation.DataType;
        currentParam.DefaultValue = parameterInformation.DefaultValue;
        currentParam.MaxValidValue = parameterInformation.MaxValidValue;
        currentParam.MinValidValue = parameterInformation.MinValidValue;
        currentParam.PdlSize = parameterInformation.PdlSize;
    }

    public void ParameterValueReceived(string pid, byte[] value)
    {
        if (_parameters.ContainsKey(pid))
        {
            ParameterInformation currentParam = _parameters[pid];
            if (currentParam.DataType == ParameterInformation.ParameterDefinition.ASCII)
            {
                currentParam.Value = Encoding.ASCII.GetString(value);
            }
            else
            {
                int intValue = 0;

                if (value.Length == 1)
                {
                    intValue = value.First();
                }
                else if (value.Length == 4)
                {
                    intValue = BitConverter.ToInt32(value, 0);
                }

                currentParam.Value = intValue;
            }
        }
    }

    #endregion

    public void AddUnsupportedParameter(RdmParameters rdmParameter, string parameterName)
    {
        _unsupportedParameters[rdmParameter] = parameterName;
        
    }

    public HashSet<RdmParameters> GetUnsupportedParameters()
    {
        return _unsupportedParameters.Keys.ToHashSet();
    }

    public bool IsUnsupportedParameter(string name)
    {
        return _unsupportedParameters.Values.Contains(name);
    }

    private void ResetProperties()
    {
        Manufacturer = null;
        Model = null;
        Label = null;
        DmxAddress = null;
        DmxFootprint = null;
        Mode = null;
        PanInvert = null;
        TiltInvert = null;
        PanTiltSwap = null;
        DeviceHours = null;
        LampHours = null;
        PowerCycles = null;
        LampStrikes = null;
        RdmProtocolVersion = null;
        DeviceModelId = null;
        SoftwareVersionId = null;
        SoftwareVersionLabel = null;
        BootSoftwareVersionId = null;
        BootSoftwareVersionLabel = null;
        DmxPersonality = null;
        DmxPersonalityCount = null;
        SubDeviceCount = null;
        SensorCount = null;
        IsIdentifyOn = null;

        _sensors.Clear();

        _parameters.Clear();
    }

    public void Copy(RdmDevice device)
    {
        _subDeviceId = device.SubDeviceId;
        _manufacturer = device.Manufacturer;
        _model = device.Model;
        _label = device.Label;
        _dmxAddress = device.DmxAddress;
        _dmxFootprint = device.DmxFootprint;
        _mode = device.Mode;
        _panInvert = device.PanInvert;
        _tiltInvert = device.TiltInvert;
        _panTiltSwap = device.PanTiltSwap;
        _deviceHours = device.DeviceHours;
        _lampHours = device.LampHours;
        _powerCycles = device.PowerCycles;
        _lampStrikes = device.LampStrikes;
        _rdmProtocolVersion = device.RdmProtocolVersion;
        _deviceModelId = device.DeviceModelId;
        _productCategory = device.ProductCategory;
        _softwareVersionId = device.SoftwareVersionId;
        _softwareVersionLabel = device.SoftwareVersionLabel;
        _bootSoftwareVersionId = device.BootSoftwareVersionId;
        _bootSoftwareVersionLabel = device.BootSoftwareVersionLabel;
        _dmxPersonality = device.DmxPersonality;
        _dmxPersonalityCount = device.DmxPersonalityCount;
        _subDeviceCount = device.SubDeviceCount;
        _sensorCount = device.SensorCount;
        _isIdentifyOn = device.IsIdentifyOn;
    }

    public void AddPersonalityDescriptionIfNotExists(PersonalityDescription personalityDescription)
    {
        if (_personalityDescriptions.TryGetValue(personalityDescription.PersonalityIndex, out PersonalityDescription existenDescription))
        {
            existenDescription.DmxSlotsRequired = personalityDescription.DmxSlotsRequired;
            existenDescription.Description = personalityDescription.Description;
        }
        else
        {
            _personalityDescriptions.Add(personalityDescription.PersonalityIndex, personalityDescription);
        }
    }
}
