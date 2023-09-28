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
    private IServiceProvider _serviceProvider;
    protected GatewayUniverse(
        string parentId,
        IPAddress address,
        int index,
        int portAddress,
        byte universe,
        bool isInUniverse,
        PortTypes portType, IServiceProvider serviceProvider = null) : base($"{parentId}:{isInUniverse}:{index}", parentId)
    {
        _serviceProvider = serviceProvider;
        IpAddress = address;
        Index = index;
        PortAddress = portAddress;
        Universe = universe;
        PortType = portType;
       
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
    public IPAddress IpAddress { get; }
    
    public int Index
    {
        get { return _index; }
        set
        {
            _index = value;
            AddParamToDb(value.ToString(), nameof(Index));
        }
    }

    public byte Universe
    {
        get { return _universe; }
        set
        {
            _universe = value;
            AddParamToDb(value.ToString(), nameof(Universe));
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
            AddParamToDb(value.ToString(), nameof(PortIndex));
        }
    }

    public int PortAddress
    {
        get { return _portAddress; }
        set
        {
            _portAddress = value;
            AddParamToDb(value.ToString(), nameof(PortAddress));
        }
    }

    public PortTypes PortType
    {
        get { return _portType; }
        set
        {
            _portType = value;
            var valParam = (int)value;
            AddParamToDb(valParam.ToString(), nameof(PortType));
        }
    }

    public List<RdmDevice> children { get; set; }
    public bool IsInProject { get; set; }
    public string Name { get; set; }

    public override string Type => nameof(GatewayUniverse);

    protected override void OnDeviceLost()
    {
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            _db.Events.Add(new Event()
            {
                deviceId = Id, level = "DeviceLost", dateTime = DateTime.Now,
                Description = $"Device {Name} has been losted!"
            });
            _db.SaveChanges();
        }
    }
}