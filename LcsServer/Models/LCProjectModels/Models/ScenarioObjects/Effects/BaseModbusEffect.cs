using System.Globalization;
using System.Net;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Managers;
using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public abstract class BaseModbusEffect : Effect //, IDisposable
{
    public ModbusMastersManager ModbusManager = null;

    private string _ipAddress;
    private ushort _port;
    private byte _unitId;
    private ushort _register;
    private float _minimum;
    private float _maximum;
    private int _interval;
    private SensorUnits _sensorUnits; // единицы измерения
    private string _stringValue;

    //private TcpClient _tcpClient;

    public event EventHandler AddressChanged;

    public event EventHandler PortChanged;
    public event EventHandler UnitIdChanged;
    public event EventHandler RegisterChanged;
    public event EventHandler MinimumChanged;
    public event EventHandler MaximumChanged;
    public event EventHandler IntervalChanged;
    public event EventHandler StringValueChanged;
    public event EventHandler SensorUnitsChanged;

    protected BaseModbusEffect()
    {
        _interval = 1000;
        _minimum = 0;
        _maximum = 100;
        _ipAddress = "192.168.0.1";
        _port = 502;
        _unitId = 1;
        _register = 1;
        _sensorUnits = SensorUnits.Float;
        _endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
        // _modbusMaster = GetModbusMaster(_ipAddress);
    }

    private IPEndPoint _endPoint;

    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            if (_ipAddress != value)
            {
                _ipAddress = value;
                _endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
                AddressChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public ushort Port
    {
        get => _port;
        set
        {
            if (_port != value)
            {
                _port = value;
                _endPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), _port);
                PortChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public byte UnitId
    {
        get => _unitId;
        set
        {
            if (_unitId != value)
            {
                _unitId = value;
                UnitIdChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public ushort Register
    {
        get => _register;
        set
        {
            if (value != _register)
            {
                _register = value;
                RegisterChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public int Interval
    {
        get => _interval;
        set
        {
            if (value != _interval)
            {
                _interval = value;
                IntervalChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }


    public float Minimum
    {
        get => _minimum;
        set
        {
            if (!value.NearEqual(_minimum))
            {
                _minimum = value;
                MinimumChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public float Maximum
    {
        get => _maximum;
        set
        {
            if (!value.NearEqual(_maximum))
            {
                _maximum = value;
                MaximumChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public SensorUnits SensorUnits
    {
        get => _sensorUnits;
        set
        {
            if (value != _sensorUnits)
            {
                _sensorUnits = value;
                SensorUnitsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    protected float FloatValue { get; private set; }
    protected ushort ShortValue { get; private set; }
    protected int IntValue { get; private set; }

    public string StringValue
    {
        get => _stringValue;
        set
        {
            if (value != _stringValue)
            {
                _stringValue = value;
                StringValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }


    private long _previousUpdate;

    protected void ReadValue()
    {
        if (_ipAddress == string.Empty)
        {
            FloatValue = 0;
            StringValue = "no connection";
            return;
        }

        long ms = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

        if (ms - _previousUpdate <= _interval)
        {
            return;
        }

        try
        {
            var result = ModbusManager.ReadHoldingRegisters(_endPoint, _unitId, _register, 2);

            if (result.Length == 0)
            {
                return;
            }

            switch (_sensorUnits)
            {
                case SensorUnits.Int16:
                    FloatValue = result[0];
                    IntValue = result[0];
                    StringValue = result[0].ToString(CultureInfo.InvariantCulture);
                    break;
                case SensorUnits.Int32:
                    uint registerUInt32 = GetUInt32(result[1], result[0]);
                    FloatValue = registerUInt32;
                    IntValue = (int)registerUInt32;
                    StringValue = registerUInt32.ToString(CultureInfo.InvariantCulture);
                    break;
                case SensorUnits.Float:
                    float registerValue = GetSingle(result[1], result[0]);
                    FloatValue = registerValue;
                    IntValue = (int)registerValue;
                    StringValue = registerValue.ToString(CultureInfo.InvariantCulture);
                    break;
                default:
                    StringValue = "read error";
                    FloatValue = 0.0f;
                    break;
            }


        }
        catch (Exception)
        {
            StringValue = "no connection";
        }
        finally
        {
            _previousUpdate = ms;
        }
    }


    /// <summary>
    /// Converts four UInt16 values into a IEEE 64 floating point format.
    /// </summary>
    /// <param name="b3">Highest-order ushort value.</param>
    /// <param name="b2">Second-to-highest-order ushort value.</param>
    /// <param name="b1">Second-to-lowest-order ushort value.</param>
    /// <param name="b0">Lowest-order ushort value.</param>
    /// <returns>IEEE 64 floating point value.</returns>
    public static double GetDouble(ushort b3, ushort b2, ushort b1, ushort b0)
    {
        byte[] value = BitConverter.GetBytes(b0)
            .Concat(BitConverter.GetBytes(b1))
            .Concat(BitConverter.GetBytes(b2))
            .Concat(BitConverter.GetBytes(b3))
            .ToArray();
        return BitConverter.ToDouble(value, 0);
    }

    /// <summary>
    /// Converts two UInt16 values into a IEEE 32 floating point format
    /// </summary>
    /// <param name="highOrderValue">High order ushort value</param>
    /// <param name="lowOrderValue">Low order ushort value</param>
    /// <returns>IEEE 32 floating point value</returns>
    public static float GetSingle(ushort highOrderValue, ushort lowOrderValue)
    {
        return BitConverter.ToSingle(
            BitConverter.GetBytes(lowOrderValue).Concat(BitConverter.GetBytes(highOrderValue)).ToArray(), 0);
    }

    /// <summary>
    /// Converts two UInt16 values into a UInt32
    /// </summary>
    public static uint GetUInt32(ushort highOrderValue, ushort lowOrderValue)
    {
        return BitConverter.ToUInt32(
            BitConverter.GetBytes(lowOrderValue).Concat(BitConverter.GetBytes(highOrderValue)).ToArray(), 0);
    }

    // private void ReleaseUnmanagedResources()
    // {
    //     // TODO release unmanaged resources here
    // }

    // protected void Dispose(bool disposing)
    // {
    //     ReleaseUnmanagedResources();
    //     if (disposing)
    //     {
    //         _tcpClient?.Dispose();
    //     }
    // }
    //
    // public void Dispose()
    // {
    //     Dispose(true);
    //     GC.SuppressFinalize(this);
    // }
    //
    // ~BaseModbusEffect()
    // {
    //     Dispose(false);
    // }
}