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

        //public ArtNetGateway()
        //{

        //}

        // Уникальное ID art-Net устройства присваивается как IpAddress:Port (например: 192.168.76.240:6454)
        public ArtNetGateway(byte[] ipAddress, short port, DatabaseContext context = null) : base(string.Join(".", ipAddress) + $":{port}", string.Empty)
        {
            _db = context;
            IpAddress = ipAddress;
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

        public byte[] IpAddress { get; }

        public short Port { get; }
        public string Manufacturer
        {
            get { return _manufacturer; }
            set
            {
                _manufacturer = value;

                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(Manufacturer)))
                    {
                        param = list.First(f => f.ParamName == nameof(Manufacturer));
                        param.ParamValue = value;
                        //_db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(Manufacturer), LastPoll = DateTime.Now };
                        param.ParamValue = value;
                        _db.DeviceParams.Add(param);
                        _db.SaveChanges();
                    }
                
            }
        }

        public short FirmwareVersion
        {
            get => _firmwareVersion;
            set
            {
                _firmwareVersion = value;

                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(FirmwareVersion)))
                    {
                        param = list.First(f => f.ParamName == nameof(FirmwareVersion));
                        param.ParamValue = value.ToString();
                        //_db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(FirmwareVersion), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        _db.DeviceParams.Add(param);
                        _db.SaveChanges();
                    }
                
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

                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(Oem)))
                    {
                        param = list.First(f => f.ParamName == nameof(Oem));
                        param.ParamValue = value.ToString();
                        //_db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(Oem), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        _db.DeviceParams.Add(param);
                        _db.SaveChanges();
                    }
                
            }
        }

        public byte UbeaVersion
        {
            get => _ubeaVersion;
            set
            {
                _ubeaVersion = value;

                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(UbeaVersion)))
                    {
                        param = list.First(f => f.ParamName == nameof(UbeaVersion));
                        param.ParamValue = value.ToString();
                        //_db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(UbeaVersion), LastPoll = DateTime.Now };
                        param.ParamValue = value.ToString();
                        _db.DeviceParams.Add(param);
                        _db.SaveChanges();
                    }
                
            }
        }

        public PollReplyStatus Status
        {
            get => _status;
            set
            {
                _status = value;

                    var valParam = (int)value;
                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(Status)))
                    {
                        param = list.First(f => f.ParamName == nameof(Status));
                        param.ParamValue = valParam.ToString();
                        //_db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(Status), LastPoll = DateTime.Now };
                        param.ParamValue = valParam.ToString();
                        _db.DeviceParams.Add(param);
                        _db.SaveChanges();
                    }
               
            }
        }

        public short EstaCode
        {
            get => _estaCode;
            set
            {
                _estaCode = value;

                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(EstaCode)))
                    {
                        param = list.First(f => f.ParamName == nameof(EstaCode));
                        param.ParamValue = value.ToString();
                       // _db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(EstaCode), LastPoll = DateTime.Now, ParamId = ""};
                        param.ParamValue = value.ToString();
                        _db.DeviceParams.Add(param);
                        _db.SaveChanges();
                    }
            }
        }

        public string LongName
        {
            get => _longName;
            set
            {
                _longName = value;

                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(LongName)))
                    {
                        param = list.First(f => f.ParamName == nameof(LongName));
                        param.ParamValue = value.Contains("\0") ? value.Substring(0, value.IndexOf('\0')) : value;
                        //_db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(LongName), LastPoll = DateTime.Now, ParamId = ""};
                        param.ParamValue = value.Contains("\0") ? value.Substring(0, value.IndexOf('\0')) : value;
                        _db.DeviceParams.Add(param);
                        _db.SaveChanges();
                    }
               
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

                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(NodeReport)))
                    {
                        param = list.First(f => f.ParamName == nameof(NodeReport));
                        param.ParamValue = value.Contains("\0") ? value.Substring(0, value.IndexOf('\0')) : value;
                        //_db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(NodeReport), LastPoll = DateTime.Now };
                        param.ParamValue = value.Contains("\0") ? value.Substring(0, value.IndexOf('\0')) : value;
                        _db.DeviceParams.Add(param);
                        _db.SaveChanges();
                    }
                
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

                    var device = _db.Devices.First(f => f.deviceId == Id);
                    var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                    var param = new DeviceParam();
                    if (list.Any(a => a.ParamName == nameof(MacAddress)))
                    {
                        param = list.First(f => f.ParamName == nameof(MacAddress));
                        param.ParamValue = valParam;
                        _db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        param = new DeviceParam()
                            { DeviceId = device.Id, ParamName = nameof(MacAddress), LastPoll = DateTime.Now };
                        param.ParamValue = valParam;
                        _db.DeviceParams.Add(param);
                        _db.SaveChanges();
                    }
                
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
            /*LogManager.GetInstance().WriteDeviceMessage(new GatewayLogMessage()
            {
                IpAddress = string.Join(".", IpAddress),
                MacAdddress = string.Join(":", MacAddress),
            });*/

                _db.Events.Add(new Event()
                {
                    deviceId = Id, level = "DeviceLost", dateTime = DateTime.Now,
                    Description = $"Device {DisplayLongName} has been losted!"
                });
                _db.SaveChanges();
            
        }
    }