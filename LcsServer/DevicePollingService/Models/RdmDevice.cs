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
        private DesignTimeDbContextFactory _db;

        public RdmDevice(RdmDeviceBroker rdmDeviceBroker, UId uId, RdmEndPoint address, byte devicePort, string parentId, DesignTimeDbContextFactory dbOperations) : base(uId.ToString(), parentId)
        {
            RdmDeviceBroker = rdmDeviceBroker;
            UId = uId;
            Address = address;
            DevicePort = devicePort;
            _sensors = new Dictionary<string, Sensor>();
            _parameters = new Dictionary<string, ParameterInformation>();
            _db = dbOperations;
        }

        public RdmDevice(RdmDeviceBroker rdmDeviceBroker, UId uId, RdmEndPoint address, byte devicePort, string parentId,
            List<Sensor> sensors, List<ParameterInformation> parameters, DesignTimeDbContextFactory databaseOperations) :
            this(rdmDeviceBroker, uId, address, devicePort, parentId, databaseOperations)
        {
            _sensors = sensors.ToDictionary(x => x.Id, x => x);
            _parameters = parameters.ToDictionary(x => x.Id, x => x);
        }

        //public bool ForceUpdate { get; internal set; }

        public RdmDeviceBroker RdmDeviceBroker { get; set; }

        public override string Type => nameof(RdmDevice);

        protected override void OnDeviceLost()
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
            using (var db = _db.CreateDbContext(null))
            {
                db.Events.Add(logInfo);
                db.SaveChanges();
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(SubDeviceId)))
                    {
                        param = list.First(f => f.ParamName == nameof(SubDeviceId));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(SubDeviceId), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(Manufacturer)))
                    {
                        param = list.First(f => f.ParamName == nameof(Manufacturer));
                        param.ParamValue = value;
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(Manufacturer), LastPoll = DateTime.Now };
                        param.ParamValue = value;
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(Model)))
                    {
                        param = list.First(f => f.ParamName == nameof(Model));
                        param.ParamValue = value;
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(Model), LastPoll = DateTime.Now };
                        param.ParamValue = value;
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(Label)))
                    {
                        param = list.First(f => f.ParamName == nameof(Label));
                        param.ParamValue = value;
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(Label), LastPoll = DateTime.Now };
                        param.ParamValue = value;
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(Mode)))
                    {
                        param = list.First(f => f.ParamName == nameof(Mode));
                        param.ParamValue = value;
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(Mode), LastPoll = DateTime.Now };
                        param.ParamValue = value;
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(PanInvert)))
                    {
                        param = list.First(f => f.ParamName == nameof(PanInvert));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(PanInvert), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(TiltInvert)))
                    {
                        param = list.First(f => f.ParamName == nameof(TiltInvert));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(TiltInvert), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(PanTiltSwap)))
                    {
                        param = list.First(f => f.ParamName == nameof(PanTiltSwap));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(PanTiltSwap), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(DeviceHours)))
                    {
                        param = list.First(f => f.ParamName == nameof(DeviceHours));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(DeviceHours), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(LampHours)))
                    {
                        param = list.First(f => f.ParamName == nameof(LampHours));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(LampHours), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(PowerCycles)))
                    {
                        param = list.First(f => f.ParamName == nameof(PowerCycles));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(PowerCycles), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(LampStrikes)))
                    {
                        param = list.First(f => f.ParamName == nameof(LampStrikes));
                        param.ParamValue = value != null ? value.ToString() : "0";
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(LampStrikes), LastPoll = DateTime.Now };
                        param.ParamValue = value != null ? value.ToString() : "0";
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(IsIdentifyOn)))
                    {
                        param = list.First(f => f.ParamName == nameof(IsIdentifyOn));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(IsIdentifyOn), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(DmxAddress)))
                    {
                        param = list.First(f => f.ParamName == nameof(DmxAddress));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(DmxAddress), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(DmxFootprint)))
                    {
                        param = list.First(f => f.ParamName == nameof(DmxFootprint));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(DmxFootprint), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(RdmProtocolVersion)))
                    {
                        param = list.First(f => f.ParamName == nameof(RdmProtocolVersion));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(RdmProtocolVersion), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(DeviceModelId)))
                    {
                        param = list.First(f => f.ParamName == nameof(DeviceModelId));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(DeviceModelId), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var valParam = (int)value;
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(ProductCategory)))
                    {
                        param = list.First(f => f.ParamName == nameof(ProductCategory));
                        param.ParamValue = valParam.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(ProductCategory), LastPoll = DateTime.Now };
                        param.ParamValue = valParam.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(SoftwareVersionId)))
                    {
                        param = list.First(f => f.ParamName == nameof(SoftwareVersionId));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(SoftwareVersionId), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(SoftwareVersionLabel)))
                    {
                        param = list.First(f => f.ParamName == nameof(SoftwareVersionLabel));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(SoftwareVersionLabel), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(BootSoftwareVersionId)))
                    {
                        param = list.First(f => f.ParamName == nameof(BootSoftwareVersionId));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                        {
                            DeviceId = device.Id, ParamName = nameof(BootSoftwareVersionId), LastPoll = DateTime.Now
                        };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(BootSoftwareVersionLabel)))
                    {
                        param = list.First(f => f.ParamName == nameof(BootSoftwareVersionLabel));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                        {
                            DeviceId = device.Id, ParamName = nameof(BootSoftwareVersionLabel), LastPoll = DateTime.Now
                        };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(DmxPersonality)))
                    {
                        param = list.First(f => f.ParamName == nameof(DmxPersonality));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(DmxPersonality), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(DmxPersonalityCount)))
                    {
                        param = list.First(f => f.ParamName == nameof(DmxPersonalityCount));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(DmxPersonalityCount), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(SubDeviceCount)))
                    {
                        param = list.First(f => f.ParamName == nameof(SubDeviceCount));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(SubDeviceCount), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
            }
        }

        public byte? SensorCount
        {
            get => _sensorCount;
            set
            {
                _sensorCount = value;
                if (value == null)
                    return;
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(SensorCount)))
                    {
                        param = list.First(f => f.ParamName == nameof(SensorCount));
                        param.ParamValue = value.ToString();
                        db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(SensorCount), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
                    }
                }
            }
        }

        public DateTime LastSeen
        {
            get => _lastSeen;
            set
            {
                _lastSeen = value;
                if (value == null)
                    return;
                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = nameof(LastSeen), LastPoll = DateTime.Now };
                    param.ParamValue = value.ToString();
                    db.DeviceParams.Add(param);
                    db.SaveChanges();
                }
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
                using (var db = _db.CreateDbContext(null))
                {
                    var sensorDb = db.Sensors.First(f => f.SensorId == sensor.Id);
                    sensorDb.LastPoll = DateTime.Now;
                    db.Update(sensorDb);
                    db.SaveChanges();
                }
            }
            else
            {
                _sensors.Add(sensor.Id, sensor);
                using (var db = _db.CreateDbContext(null))
                {
                    var sensorToDb = new SensorDb()
                    {
                        deviceId = Id, SensorUnitId = (int)sensor.Unit, description = sensor.Description,
                        SensorId = sensor.Id, SensorNumber = sensor.SensorNumber, LastPoll = DateTime.Now
                    };
                    db.Sensors.Add(sensorToDb);
                    db.SaveChanges();
                }
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
                DeviceStatus = DeviceStatuses.Warning;
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
            var sensorVal = new SensorValue()
            {
                SensorId = sensor.Id,
                NormalMinValue = sensor.NormalMinValue,
                NormalMaxValue = sensor.NormalMaxValue,
                Value = sensor.SensorValue,
                SensorUnitId = (int)sensor.Unit,
                Timestamp = DateTime.Now,
            };
            using (var db = _db.CreateDbContext(null))
            {
                db.SensorValues.Add(sensorVal);
                db.SaveChanges();
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

                using (var db = _db.CreateDbContext(null))
                {
                    var device = db.Devices.First(f => f.deviceId == Id);
                    var list = db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    if (!list.Any(a => a.ParamId == pid))
                    {
                        var param = new DeviceParam()
                        {
                            DeviceId = device.Id, ParamName = currentParam.Description, LastPoll = DateTime.Now,
                            ParamId = pid
                        };
                        param.ParamValue = currentParam.Value.ToString();
                        db.DeviceParams.Add(param);
                        db.SaveChanges();
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
                            db.DeviceParams.Add(param);
                            db.SaveChanges();
                        }
                        else
                        {
                            param.LastPoll = DateTime.Now;
                            db.Entry(param).Property(p => p.LastPoll).IsModified = true;
                            db.SaveChanges();
                        }
                    }
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