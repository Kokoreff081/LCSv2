using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Interfaces;

namespace LcsServer.DevicePollingService.Models;

 public class StorageManager : IStorageManager
    {
        private ConcurrentDictionary<string, BaseDevice> _devices;
        private readonly IStatusChecker _statusChecker;
        public DatabaseContext _db { get; set; }
        private object locker = new object();
        private const string RdmScan = "RdmDiscoveryForbidden";
        private IServiceProvider _serviceProvider;
        public event Action<ActionTypes, BaseDevice[]> DeviceListUpdated;

        private Dictionary<string, BaseDevice> _snapshot;

        public StorageManager(IStatusChecker statusChecker, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _statusChecker = statusChecker;
            _devices = new ConcurrentDictionary<string, BaseDevice>();
            
           // _db = db;
            
        }
        public ConcurrentDictionary<string, BaseDevice> Devices
        {
            get { return _devices; }
            set { _devices = value; }
        }
        public string SnapshotFilePath { get; set; }
        public bool scanActive { 
            get { lock (locker) 
                {
                    /*using (var db = _db.CreateDbContext(null))
                    {*/
                        return _db.Settings.Where(w => w.Name == RdmScan).First().IsEnabled; 
                    //}
                } 
            } 
            set {
                /*using (var db = _db.CreateDbContext(null))
                {*/
                    var current = _db.Settings.Where(w => w.Name == RdmScan).First();
                    if (current.IsEnabled == value)
                        return;
                    else
                    {
                        current.IsEnabled = value;
                        _db.Update(current);
                        _db.SaveChanges();
                    }
                //}
            } 
        }
        public BaseDevice GetDeviceById(string id)
        {
            return Devices[id];
        }

        public IEnumerable<BaseDevice> GetDevices()
        {
            return Devices.Values;
        }

        public Dictionary<string, BaseDevice> GetDevicesDictionary()
        {
            return Devices.ToDictionary(x => x.Key, x => x.Value);
        }

        public void AddDevices(BaseDevice[] devices)
        {
            
            foreach (var device in devices)
            {
                if (Devices.TryGetValue(device.Id, out BaseDevice existenDevice))
                {
                    if (existenDevice is RdmDevice rdmDevice)
                        rdmDevice.LastSeen = DateTime.Now;
                    else if (existenDevice is GatewayOutputUniverse outputUniverse)
                        outputUniverse.OutputStatus = ((GatewayOutputUniverse)device).OutputStatus;
                    else if (existenDevice is GatewayInputUniverse inputUniverse)
                        inputUniverse.InputStatus = ((GatewayInputUniverse)device).InputStatus;
                }
                else
                {
                    Devices.AddOrUpdate(device.Id, device, (key, baseDevice) => baseDevice);
                    WriteDevicesToDb();
                    DeviceListUpdated?.Invoke(ActionTypes.Added, new BaseDevice[] { device });

                    if (device is RdmDevice rdmDevice)
                        rdmDevice.LastSeen = DateTime.Now;
                }
                _statusChecker.UpdateDeviceStatuses(device);
            }
            
        }
        private async void WriteDevicesToDb()
        {
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                foreach (var device in Devices.Values)
                {
                    _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                    var list = _db.Devices.ToList();
                    if (!list.Any(a => a.deviceId == device.Id))
                    {
                        var status = _db.DeviceStatuses.First(f => f.Status == (int)device.DeviceStatus);
                        var newDevice = new Device()
                        {
                            deviceId = device.Id, Type = device.Type, StatusId = status.Id, ParentId = device.ParentId
                        };
                        _db.Devices.Add(newDevice);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //_db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                        var existenDevice = _db.Devices.First(f => f.deviceId == device.Id);
                        switch (existenDevice.Type)
                        {
                            case "RdmDevice":
                                var param = _db.DeviceParams.First(f =>
                                    f.ParamName == "LastSeen" && f.DeviceId == existenDevice.Id);
                                param.ParamValue = DateTime.Now.ToString();
                                param.LastPoll = DateTime.Now;
                                break;
                            case "GatewayOutputUniverse":
                            case "GatewayInputUniverse":
                                existenDevice.StatusId =
                                    _db.DeviceStatuses.First(f => f.Status == (int)device.DeviceStatus).Id;
                                break;
                        }

                        await _db.SaveChangesAsync();
                        
                    }

                }
            }
        }
        public void GetAllChildren(BaseDevice parent, List<BaseDevice> devices)
        {
            foreach (var children in _devices.Where(x => x.Value.ParentId == parent.Id))
            {
                devices.Add(children.Value);
                GetAllChildren(children.Value, devices);
            }
        }

        public void RefreshAllRdmDevices(string parentId)
        {
            foreach (RdmDevice rdmDevice in _devices.Values.OfType<RdmDevice>())
            {
                if (!rdmDevice.ParentId.Contains(parentId))
                    break;

                rdmDevice.UpdateAll();
            }
        }

        public void CheckWhichDevicesLost()
        {
            _statusChecker.CheckWhichDevicesLost();
        }

        public void ClearDevices()
        {
            Devices.Clear();
            DeviceListUpdated?.Invoke(ActionTypes.Cleared, null);
        }

        
    }