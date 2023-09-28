using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Acn.Rdm;
using Acn.Rdm.Packets.Control;
using Acn.Rdm.Packets.DMX;
using Acn.Rdm.Packets.Power;
using Acn.Rdm.Packets.Product;
using Acn.Sockets;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;
using LLcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.Models;

public class RdmDevice : BaseDevice
    {
        public event Action<string> UnsupportedParameterAdded;

        private int _subDeviceId;
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
        private object locker = new object();
        private IServiceProvider _serviceProvider;

        public RdmDevice(RdmDeviceBroker rdmDeviceBroker, UId uId, RdmEndPoint address, byte devicePort, string parentId, IServiceProvider serviceProvider) : base(uId.ToString(), parentId)
        {
            _serviceProvider = serviceProvider;
            RdmDeviceBroker = rdmDeviceBroker;
            UId = uId;
            Address = address;
            DevicePort = devicePort;
            _sensors = new Dictionary<string, Sensor>();
            _parameters = new Dictionary<string, ParameterInformation>();
            
        }

        public RdmDevice(RdmDeviceBroker rdmDeviceBroker, UId uId, RdmEndPoint address, byte devicePort, string parentId,
            List<Sensor> sensors, List<ParameterInformation> parameters, DatabaseContext databaseOperations, IServiceProvider serviceProvider) :
            this(rdmDeviceBroker, uId, address, devicePort, parentId, serviceProvider)
        {
            _sensors = sensors.ToDictionary(x => x.Id, x => x);
            _parameters = parameters.ToDictionary(x => x.Id, x => x);
        }

        //public bool ForceUpdate { get; internal set; }

        public RdmDeviceBroker RdmDeviceBroker { get; set; }

        public override string Type => nameof(RdmDevice);

        protected override async void OnDeviceLost()
        {
            /*
            LogManager.GetInstance().WriteDeviceMessage(new RdmDeviceLogMessage()
            {
                UID = UId.ToString(),
                IpAddress = Address.IpAddress.ToString(),
                Universe = Address.Universe,
                DmxAddress = DmxAddress == null ? "null" : DmxAddress.Value.ToString(),
            });
            */

            var logInfo = new Event();
            logInfo.deviceId = Id;
            logInfo.level = "Error";
            logInfo.Description = $"device {DisplayName}-{Address.Address} lost connection";
            logInfo.dateTime = DateTime.Now;
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                _db.Events.Add(logInfo);
                await _db.SaveChangesAsync();
            }
        }

        private async void AddParamToDb(string val, string paramName)
        {
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var device = _db.Devices.First(f => f.deviceId == Id);
                var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                var param = new DeviceParam();
                if (list.Any(a => a.ParamName == paramName))
                {
                    param = list.First(f => f.ParamName == paramName);
                    if (param.ParamValue == val)
                        param.LastPoll = DateTime.Now;
                    else
                    {
                        param.ParamValue = val;
                        param.LastPoll = DateTime.Now;
                        _db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                    }

                    _db.Entry(param).Property(p => p.LastPoll).IsModified = true;
                }
                else
                {
                    param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = paramName, LastPoll = DateTime.Now };
                    param.ParamValue = val;
                    _db.DeviceParams.Add(param);
                }

                await _db.SaveChangesAsync();
            }


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
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(SubDeviceId));
            }
        }

        public string Manufacturer
        {
            get => _manufacturer;
            set
            {
                _manufacturer = value;
                if (value == null)
                    return;
                AddParamToDb(value, nameof(Manufacturer));
            }
        }

        public string Model
        {
            get => _model;
            set
            {
                _model = value;
                if (value == null)
                    return;
                AddParamToDb(value, nameof(Model));
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
                if (value == null)
                    return;
                AddParamToDb(value, nameof(Label));
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
                if (value == null)
                    return;
                AddParamToDb(value, nameof(Mode));
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
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(PanInvert));
            }
        }

        public bool? TiltInvert
        {
            get => _tiltInvert;
            set
            {
                _tiltInvert = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(TiltInvert));
            }
        }

        public bool? PanTiltSwap
        {
            get => _panTiltSwap;
            set
            {
                _panTiltSwap = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(PanTiltSwap));
            }
        }

        public int? DeviceHours
        {
            get => _deviceHours;
            set
            {
                _deviceHours = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(DeviceHours));
            }
        }

        public int? LampHours
        {
            get => _lampHours;
            set
            {
                _lampHours = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(LampHours));
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
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(PowerCycles));
            }
        }

        public int? LampStrikes
        {
            get => _lampStrikes;
            set
            {
                _lampStrikes = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(LampStrikes));
            }
        }

        public bool? IsIdentifyOn
        {
            get => _isIdentifyOn;
            internal set
            {
                _isIdentifyOn = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(IsIdentifyOn));
            }
        }

        public void IdentifyOnOff(bool isOn)
        {
            IdentifyDevice.Set identifySet = new IdentifyDevice.Set();
            identifySet.IdentifyEnabled = isOn;
            RdmDeviceBroker.ForceSendRdm(identifySet, Address, UId);

            // Запускаем задачу на получение IsIdentifyOn через 1 сек., чтобы запрос на изменение IsIdentifyOn успел отработать
            Task.Delay(1000).ContinueWith(x => ForceRequestIdentifyOn());

        }


        #region Device Information

        public int? DmxAddress
        {
            get => _dmxAddress;
            set
            {
                _dmxAddress = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(DmxAddress));
            }
        }

        public int? DmxFootprint
        {
            get => _dmxFootprint;
            set
            {
                _dmxFootprint = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(DmxFootprint));
            }
        }

        public short? RdmProtocolVersion
        {
            get => _rdmProtocolVersion;
            set
            {
                _rdmProtocolVersion = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(RdmProtocolVersion));
            }
        }

        public short? DeviceModelId
        {
            get => _deviceModelId;
            set
            {
                _deviceModelId = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(DeviceModelId));
            }
        }

        public ProductCategories ProductCategory
        {
            get => _productCategory;
            set
            {
                _productCategory = value;
                if (value == null)
                    return;
                var valParam = (int)value;
                AddParamToDb(valParam.ToString(), nameof(ProductCategory));
            }
        }

        public int? SoftwareVersionId
        {
            get => _softwareVersionId;
            set
            {
                _softwareVersionId = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(SoftwareVersionId));
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
                if (value == null)
                    return;
                AddParamToDb(value, nameof(SoftwareVersionLabel));
            }
        }

        public int? BootSoftwareVersionId
        {
            get => _bootSoftwareVersionId;
            set
            {
                _bootSoftwareVersionId = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(BootSoftwareVersionId));
            }
        }

        public string BootSoftwareVersionLabel
        {
            get => _bootSoftwareVersionLabel;
            set
            {
                _bootSoftwareVersionLabel = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(BootSoftwareVersionLabel));
            }
        }

        public byte? DmxPersonality
        {
            get => _dmxPersonality;
            set
            {
                _dmxPersonality = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(DmxPersonality));
            }
        }

        public void ChangeDmxPersonality(byte personalityIndex)
        {
            DmxPersonality.Set setDmxPersonality = new DmxPersonality.Set();
            setDmxPersonality.PersonalityIndex = personalityIndex;
            RdmDeviceBroker.ForceSendRdm(setDmxPersonality, Address, UId);

            DmxPersonality = null;

            // Запускаем задачу на получение dmxAddress через 1 сек., чтобы запрос на изменение dmxAddress успел отработать
            Task.Delay(1000).ContinueWith((x) =>
            {
                RequestPersonality();
            });

        }

        public byte? DmxPersonalityCount
        {
            get => _dmxPersonalityCount;
            set
            {
                _dmxPersonalityCount = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(DmxPersonalityCount));
            }
        }

        public short? SubDeviceCount
        {
            get => _subDeviceCount;
            set
            {
                _subDeviceCount = value;
                if (value == null)
                    return;
                AddParamToDb(value.ToString(), nameof(SubDeviceCount));
            }
        }

        public byte? SensorCount
        {
            get => _sensorCount;
            set
            {
                if (value == null)
                    return;
                _sensorCount = value;
                AddParamToDb(value.ToString(), nameof(SensorCount));
            }
        }

        public DateTime LastSeen
        {
            get => _lastSeen;
            set
            {
                _lastSeen = value;
                AddParamToDb(value.ToString(), nameof(LastSeen));
            }
        }

        public string IpAddress { get; set; }

        public string ParentPort { get; set; }

        #endregion

        #region Sensors

        public IEnumerable<Sensor> Sensors => _sensors.Values;

        public void SensorFound(Sensor sensor)
        {
            AddSensor(sensor);
        }

        private async void AddSensor(Sensor sensor)
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
                var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var sensorDb = _db.Sensors.First(f => f.SensorId == sensor.Id);
                    sensorDb.LastPoll = DateTime.Now;
                    _db.Update(sensorDb);
                    await _db.SaveChangesAsync();
                }
            }
            else
            {
                _sensors.Add(sensor.Id, sensor);
                var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var sensorToDb = new SensorDb()
                    {
                        deviceId = Id, SensorUnitId = (int)sensor.Unit, description = sensor.Description,
                        SensorId = sensor.Id, SensorNumber = sensor.SensorNumber, LastPoll = DateTime.Now
                    };
                    _db.Sensors.Add(sensorToDb);
                    await _db.SaveChangesAsync();
                }
            }
            
        }

        public async void SensorValueReceived(string sensorId, short presentValue, short minValue, short maxValue, short recordedValue)
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
                DeviceStatus = DeviceStatuses.Warning;
            }
            var sensorVal = new SensorValue()
            {
                SensorId = sensor.Id,
                NormalMinValue = sensor.NormalMinValue,
                NormalMaxValue = sensor.NormalMaxValue,
                Value = sensor.SensorValue,
                SensorUnitId = (int)sensor.Unit,
                Timestamp = DateTime.Now,
            };
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                _db.SensorValues.Add(sensorVal);
                await _db.SaveChangesAsync();
            }

        }

        #endregion

        #region Parameters
        public IEnumerable<ParameterInformation> Parameters => _parameters.Values;

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

        [SuppressMessage("ReSharper.DPA", "DPA0005: Database issues")]
        public async void ParameterValueReceived(string pid, byte[] value)
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

                var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
                using (var scope = scopeFactory.CreateScope())
                {
                    _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    if (!list.Any(a => a.ParamId == pid))
                    {
                        var param = new DeviceParam()
                        {
                            DeviceId = device.Id, ParamName = currentParam.Description, LastPoll = DateTime.Now,
                            ParamId = pid
                        };
                        param.ParamValue = currentParam.Value.ToString();
                        _db.DeviceParams.Add(param);
                    }
                    else
                    {
                        var param = list.First(f => f.ParamId == pid);
                        if (param.ParamValue != currentParam.Value.ToString())
                        {
                            param.DeviceId = device.Id;
                            param.ParamName = currentParam.Description;
                            param.ParamValue = currentParam.Value.ToString();
                            param.LastPoll = DateTime.Now;
                            param.ParamId = pid;
                            _db.DeviceParams.Add(param);
                        }
                        else
                        {
                            param.LastPoll = DateTime.Now;
                            _db.Entry(param).Property(p => p.LastPoll).IsModified = true;
                        }
                    }
                    await _db.SaveChangesAsync();
                }
            }
        }

        #endregion

        public void AddUnsupportedParameter(RdmParameters rdmParameter, string parameterName)
        {
            _unsupportedParameters[rdmParameter] = parameterName;
            UnsupportedParameterAdded?.Invoke(parameterName);
        }

        public HashSet<RdmParameters> GetUnsupportedParameters()
        {
            return _unsupportedParameters.Keys.ToHashSet();
        }

        public bool IsUnsupportedParameter(string name)
        {
            return _unsupportedParameters.Values.Contains(name);
        }

        #region RequestsRetranslation

        public void RequestModelDescription()
        {
            Model = null;
            RdmDeviceBroker.ForceRequestModelDescription(Address, UId);
        }

        public void RequestSoftwareVersionLabel()
        {
            SoftwareVersionLabel = null;
            RdmDeviceBroker.ForceRequestSoftwareVersionLabel(Address, UId);
        }

        public void RequestBootSoftwareVersionId()
        {
            BootSoftwareVersionId = null;
            RdmDeviceBroker.ForceRequestBootSoftwareVersionId(Address, UId);
        }

        public void RequestBootSoftwareVersionLabel()
        {
            BootSoftwareVersionLabel = null;
            RdmDeviceBroker.ForceRequestBootSoftwareVersionLabel(Address, UId);
        }

        public void RequestDmxAddress()
        {
            DmxAddress = null;
            RdmDeviceBroker.ForceRequestDmxAddress(Address, UId);
        }

        public void RequestLabel()
        {
            Label = null;
            RdmDeviceBroker.ForceRequestLabel(Address, UId);
        }

        public void RequestManufacturer()
        {
            Manufacturer = null;
            RdmDeviceBroker.ForceRequestManufacture(Address, UId);
        }

        public void RequestPowerCycles()
        {
            PowerCycles = null;
            RdmDeviceBroker.ForceRequestPowerCycles(Address, UId);
        }

        public void RequestDeviceHours()
        {
            DeviceHours = null;
            RdmDeviceBroker.ForceRequestDeviceHours(Address, UId);
        }

        public void RequestLampHours()
        {
            LampHours = null;
            RdmDeviceBroker.ForceRequestLampHours(Address, UId);
        }

        public void RequestLampStrikes()
        {
            LampStrikes = null;
            RdmDeviceBroker.ForceRequestLampStrikes(Address, UId);
        }

        public void RequestPanInvert()
        {
            PanInvert = null;
            RdmDeviceBroker.ForceRequestPanInvert(Address, UId);
        }

        public void RequestTiltInvert()
        {
            TiltInvert = null;
            RdmDeviceBroker.ForceRequestTiltInvert(Address, UId);
        }

        public void RequestPanTiltSwap()
        {
            PanTiltSwap = null;
            RdmDeviceBroker.ForceRequestPanTiltSwap(Address, UId);
        }

        public void ForceRequestIdentifyOn()
        {
            IsIdentifyOn = null;
            RdmDeviceBroker.ForceRequestIsIdentifyOn(Address, UId);
        }

        public void RequestDeviceInfo()
        {
            DmxFootprint = null;
            RdmProtocolVersion = null;
            DeviceModelId = null;
            SoftwareVersionId = null;
            DmxPersonality = null;
            DmxPersonalityCount = null;
            SubDeviceCount = null;
            SensorCount = null;

            RdmDeviceBroker.ForceRequestDeviceInfo(Address, UId);
        }

        public void RequestPersonality()
        {
            _personalityDescriptions.Clear();

            RequestDeviceInfo();
            RdmDeviceBroker.ForceRequestPersonality(Address, this);


        }

        public void RequestParameters()
        {
            _parameters.Clear();
            RdmDeviceBroker.ForceRequestParameters(Address, UId);
        }

        public void RequestSensors()
        {
            _sensors.Clear();
            if (SensorCount != null)
                RdmDeviceBroker.ForceRequestSensors(Address, UId, SensorCount.Value);
        }

        public void UpdateAll()
        {
            //ForceUpdate = true;
            ResetProperties();
            RdmDeviceBroker.ForceRequestDeviceInfo(Address, UId);
            RdmDeviceBroker.ForceUpdateRdmDevice(this, SensorCount ?? (byte)0, SubDeviceCount ?? 0);
        }

        #endregion

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