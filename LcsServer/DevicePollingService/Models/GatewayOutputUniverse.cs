using System.Net;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;

namespace LcsServer.DevicePollingService.Models;

public class GatewayOutputUniverse : GatewayUniverse
{
    private OutputStatuses _outputStatus;
    private DatabaseContext _db;
    private IServiceProvider _serviceProvider;
    public GatewayOutputUniverse(
        string parentId,
        IPAddress address,
        int index,
        int portAddress,
        byte universe,
        PortTypes portType,
        OutputStatuses outputStatus, IServiceProvider serviceProvider = null) : base(parentId, address, index, portAddress, universe, false, portType, serviceProvider)
    {
        _serviceProvider = serviceProvider;
        OutputStatus = outputStatus;
    }

    public OutputStatuses OutputStatus
    {
        get => _outputStatus;
        set
        {
            _outputStatus = value;
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var valParam = (int)value;
                var device = _db.Devices.First(f => f.deviceId == Id);
                var list = _db.DeviceParams.Where(w => w.DeviceId == device.Id).ToList();
                var param = new DeviceParam();
                if (list.Any(a => a.ParamName == nameof(OutputStatus)))
                {
                    param = list.First(f => f.ParamName == nameof(OutputStatus));
                    param.ParamValue = valParam.ToString();
                    _db.Entry(param).Property(p => p.ParamValue).IsModified = true;
                    _db.SaveChanges();
                }
                else
                {
                    param = new DeviceParam()
                        { DeviceId = device.Id, ParamName = nameof(OutputStatus), LastPoll = DateTime.Now };
                    param.ParamValue = valParam.ToString();
                    _db.DeviceParams.Add(param);
                    _db.SaveChanges();
                }
            }
        }
    }
}