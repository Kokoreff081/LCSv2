using System.Net;
using System.Text.Json;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;

namespace LcsServer.DevicePollingService.Models;

public abstract class GatewayUniverse : BaseDevice
{

    //Уникальное ID юниверса.
    //Присваивается как ParentId(это ArtNetGatewayNode.Id - IpAddress:Port:ByteIndex):isInUniverse(тип выхода Input/ Output):index(порядковый номер выхода порта)):universe(номер юниверса)
    //Родителем является ArtNetGatewayNode
    private DatabaseContext _db;
    private int _index;
    private int _portIndex;
    private byte _universe;
    private int _portAddress;
    private PortTypes _portType;
    protected GatewayUniverse(
        string parentId,
        IPAddress address,
        int index,
        int portAddress,
        byte universe,
        bool isInUniverse,
        PortTypes portType, DatabaseContext context = null) : base($"{parentId}:{isInUniverse}:{index}", parentId)
    {
        _db = context;
        IpAddress = address;
        Index = index;
        PortAddress = portAddress;
        Universe = universe;
        PortType = portType;
       
    }

    public IPAddress IpAddress { get; }

    public int Index
    {
        get { return _index; }
        set
        {
            _index = value;
                var device = _db.Devices.First(f => f.deviceId == Id);
                var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                var param = new DeviceParam();
                if (list.Any(a => a.ParamName == nameof(Index)))
                {
                    param = list.First(f => f.ParamName == nameof(Index));
                    param.ParamValue = value.ToString();
                    _db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                    _db.SaveChanges();
                }
                else
                {
                    param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = nameof(Index), LastPoll = DateTime.Now };
                    param.ParamValue = value.ToString();
                    _db.DeviceParams.Add(param);
                    _db.SaveChanges();
                }
            
        }
    }

    public byte Universe
    {
        get { return _universe; }
        set
        {
            _universe = value;
                var device = _db.Devices.First(f => f.deviceId == Id);
                var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                var param = new DeviceParam();
                if (list.Any(a => a.ParamName == nameof(Universe)))
                {
                    param = list.First(f => f.ParamName == nameof(Universe));
                    param.ParamValue = value.ToString();
                    _db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                    _db.SaveChanges();
                }
                else
                {
                    param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = nameof(Universe), LastPoll = DateTime.Now };
                    param.ParamValue = value.ToString();
                    _db.DeviceParams.Add(param);
                    _db.SaveChanges();
                }
            
        }
    }

    /// <summary>
    /// Порядковый номер (физический номер порта)
    /// </summary>
    public int PortIndex
    {
        get { return _portIndex; }
        set
        {
            _portIndex = value;
                var device = _db.Devices.First(f => f.deviceId == Id);
                var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                var param = new DeviceParam();
                if (list.Any(a => a.ParamName == nameof(PortIndex)))
                {
                    param = list.First(f => f.ParamName == nameof(PortIndex));
                    param.ParamValue = value.ToString();
                    _db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                    _db.SaveChanges();
                }
                else
                {
                    param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = nameof(PortIndex), LastPoll = DateTime.Now };
                    param.ParamValue = value.ToString();
                    _db.DeviceParams.Add(param);
                    _db.SaveChanges();
                }
            
        }
    }

    public int PortAddress
    {
        get { return _portAddress; }
        set
        {
            _portAddress = value;

                var device = _db.Devices.First(f => f.deviceId == Id);
                var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                var param = new DeviceParam();
                if (list.Any(a => a.ParamName == nameof(PortAddress)))
                {
                    param = list.First(f => f.ParamName == nameof(PortAddress));
                    param.ParamValue = value.ToString();
                    _db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                    _db.SaveChanges();
                }
                else
                {
                    param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = nameof(PortAddress), LastPoll = DateTime.Now };
                    param.ParamValue = value.ToString();
                    _db.DeviceParams.Add(param);
                    _db.SaveChanges();
                }
            
        }
    }

    public PortTypes PortType
    {
        get { return _portType; }
        set
        {
            _portType = value;
                var valParam = (int)value;
                var device = _db.Devices.First(f => f.deviceId == Id);
                var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                var param = new DeviceParam();
                if (list.Any(a => a.ParamName == nameof(PortType)))
                {
                    param = list.First(f => f.ParamName == nameof(PortType));
                    param.ParamValue = valParam.ToString();
                    _db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                    _db.SaveChanges();
                }
                else
                {
                    param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = nameof(PortType), LastPoll = DateTime.Now };
                    param.ParamValue = valParam.ToString();
                    _db.DeviceParams.Add(param);
                    _db.SaveChanges();
                }
            
        }
    }

    public List<RdmDevice> children { get; set; }
    public bool IsInProject { get; set; }
    public string Name { get; set; }

    public override string Type => nameof(GatewayUniverse);

    protected override void OnDeviceLost()
    {
            _db.Events.Add(new Event()
            {
                deviceId = Id, level = "DeviceLost", dateTime = DateTime.Now,
                Description = $"Device {Name} has been losted!"
            });
            _db.SaveChanges();
        
    }
}