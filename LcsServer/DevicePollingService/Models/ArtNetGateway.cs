using System.Net;
using System.Text;
using Acn.ArtNet.Packets;
using LcsServer.DatabaseLayer;

namespace LcsServer.DevicePollingService.Models;

public class ArtNetGateway : BaseDevice
    {
        private DatabaseContext _db;
        private short _firmwareVersion;
        private short _oem = 0xff;
        private byte _ubeaVersion;
        private PollReplyStatus _status;
        private short _estaCode;
        private string _longName;
        private string _nodeReport;
        private byte[] _goodInput = new byte[4];
        private byte[] _goodOutput = new byte[4];
        private byte _swVideo;
        private byte _swMacro;
        private byte _swRemote;
        private byte _style;
        private byte[] _macAddress = new byte[6];
        private PollReplyStatus2 _status2;
        private string _manufacturer;
        private IServiceProvider _serviceProvider;
        //public ArtNetGateway()
        //{

        //}

        // Уникальное ID art-Net устройства присваивается как IpAddress:Port (например: 192.168.76.240:6454)
        public ArtNetGateway(byte[] ipAddress, short port, IServiceProvider serviceProvider = null) : base(string.Join(".", ipAddress) + $":{port}", string.Empty)
        {
            _serviceProvider = serviceProvider;
            IpAddress = ipAddress;
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var device = _db.Devices.First(f => f.deviceId == Id);
                var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                var param = new DeviceParam();
                if (list.Any(a => a.ParamName == nameof(IpAddress)))
                {
                    param = list.First(f => f.ParamName == nameof(IpAddress));
                    param.ParamValue = new IPAddress(ipAddress).ToString();
                    //_db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                    _db.SaveChanges();
                }
                else
                {
                    param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = nameof(IpAddress), LastPoll = DateTime.Now };
                    param.ParamValue = new IPAddress(ipAddress).ToString();
                    _db.DeviceParams.Add(param);
                    _db.SaveChanges();
                }

                Port = port;
                list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();

                if (list.Any(a => a.ParamName == nameof(Port)))
                {
                    param = list.First(f => f.ParamName == nameof(Port));
                    param.ParamValue = port.ToString();
                    //_db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                    _db.SaveChanges();
                }
                else
                {
                    param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = nameof(Port), LastPoll = DateTime.Now };
                    param.ParamValue = port.ToString();
                    _db.DeviceParams.Add(param);
                    _db.SaveChanges();
                }
            }
        }

        public byte[] IpAddress { get; }

        public short Port { get; }
        
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
        public string Manufacturer
        {
            get { return _manufacturer; }
            set
            {
                _manufacturer = value;
                AddParamToDb(value.ToString(), nameof(Manufacturer));
            }
        }

        public short FirmwareVersion
        {
            get => _firmwareVersion;
            set
            {
                _firmwareVersion = value;
                AddParamToDb(value.ToString(), nameof(FirmwareVersion));
            }
        }

        public string FirmwareVersionLabel
        {
            get
            {
                byte ver1 = (byte)FirmwareVersion;
                byte ver2 = (byte)((uint)FirmwareVersion >> 8);

                return $"{ver2}.{ver1}";
            }
        }

        public short Oem
        {
            get => _oem;
            set
            {
                _oem = value;
                AddParamToDb(value.ToString(), nameof(Oem));
            }
        }

        public byte UbeaVersion
        {
            get => _ubeaVersion;
            set
            {
                _ubeaVersion = value;
                AddParamToDb(value.ToString(), nameof(UbeaVersion));
            }
        }

        public PollReplyStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                var valParam = (int)value;
                AddParamToDb(valParam.ToString(), nameof(Status));
            }
        }

        public short EstaCode
        {
            get => _estaCode;
            set
            {
                _estaCode = value;
                AddParamToDb(value.ToString(), nameof(EstaCode));
            }
        }

        public string LongName
        {
            get => _longName;
            set
            {
                _longName = value;
                var valParam = value.Contains("\0") ? value.Substring(0, value.IndexOf('\0')) : value;
                AddParamToDb(valParam.ToString(), nameof(LongName));
            }
        }

        public string DisplayLongName
        {
            get
            {
                if (string.IsNullOrEmpty(LongName))
                    return string.Empty;

                return LongName.Contains("\0") ? LongName.Substring(0, LongName.IndexOf('\0')) : LongName;
            }
        }

        public string NodeReport
        {
            get => _nodeReport;
            set
            {
                _nodeReport = value;
                var valParam = value.Contains("\0") ? value.Substring(0, value.IndexOf('\0')) : value;
                AddParamToDb(valParam.ToString(), nameof(NodeReport));
            }
        }

        public byte[] GoodInput
        {
            get => _goodInput;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The good input must be an array of 4 bytes.");

                _goodInput = value;
            }
        }

        public byte[] GoodOutput
        {
            get => _goodOutput;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The good output must be an array of 4 bytes.");

                _goodOutput = value;
            }
        }

        public byte SwVideo
        {
            get => _swVideo;
            set => _swVideo = value;
        }

        public byte SwMacro
        {
            get => _swMacro;
            set => _swMacro = value;
        }

        public byte SwRemote
        {
            get => _swRemote;
            set => _swRemote = value;
        }

        public byte Style
        {
            get => _style;
            set => _style = value;
        }

        public byte[] MacAddress
        {
            get => _macAddress;
            set
            {
                if (value.Length != 6)
                    throw new ArgumentException("The mac address must be an array of 6 bytes.");

                _macAddress = value;
                
                var sb = new StringBuilder();
                for(int i=0;i<value.Length;i++)
                {
                    var item = value[i];
                    sb.Append(item.ToString("X2"));
                    if(i<value.Length-1)
                        sb.Append(":");
                }

                var valParam = sb.ToString();
                AddParamToDb(valParam, nameof(MacAddress));
            }
        }

        public PollReplyStatus2 Status2
        {
            get => _status2;
            set => _status2 = value;
        }

        /// <summary>
        /// Defines whether the device has been allocated an IP address via DHCP. The field is only relevant when Is Dhcp enabled
        /// </summary>
        public bool IsIpStatic => !(Status2.HasFlag(PollReplyStatus2.IpIsDhcpConfigured) && Status2.HasFlag(PollReplyStatus2.DhcpEnabled));

        public override string Type => nameof(ArtNetGateway);

        protected override void OnDeviceLost()
        {
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                _db.Events.Add(new Event()
                {
                    deviceId = Id, level = "DeviceLost", dateTime = DateTime.Now,
                    Description = $"Device {DisplayLongName} has been losted!"
                });
                _db.SaveChanges();
            }
        }
    }