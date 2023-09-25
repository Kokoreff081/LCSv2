using Acn.ArtNet.Packets;
using LcsServer.DevicePollingService.Enums;

namespace LcsServer.Models.DeviceModels;

public class ArtnetGateWayToWeb
{
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
    
    public string Id { get; set; }
    public string IpAddress { get; set; }

    public short Port { get; set; }
    public string Manufacturer
    {
        get { return _manufacturer; }
        set
        {
            _manufacturer = value;
        }
    }

    public short FirmwareVersion
    {
        get => _firmwareVersion;
        set
        {
            _firmwareVersion = value;
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
        }
    }

    public byte UbeaVersion
    {
        get => _ubeaVersion;
        set
        {
            _ubeaVersion = value;
        }
    }
    public int StatusId { get; set; }
    public PollReplyStatus Status
    {
        get => _status;
        set
        {
            _status = value;
        }
    }
    public DeviceStatuses deviceStatus { get; set; }
    public short EstaCode
    {
        get => _estaCode;
        set
        {
            _estaCode = value;
        }
    }

    public string LongName
    {
        get => _longName;
        set
        {
            _longName = value;
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
        }
    }

    public PollReplyStatus2 Status2
    {
        get => _status2;
        set => _status2 = value;
    }

    public string Type { get; set; }

    public List<GatewayUniverseToWeb> children { get; set; }

}