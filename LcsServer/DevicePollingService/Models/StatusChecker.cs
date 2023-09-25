using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Interfaces;

namespace LcsServer.DevicePollingService.Models;

public class StatusChecker : IStatusChecker
{
    private Dictionary<string, BaseDevice> _devicesSnapshot;
    private readonly Dictionary<string, BaseDevice> _receivedDevices;

    public StatusChecker()
    {
        _devicesSnapshot = new Dictionary<string, BaseDevice>();
        _receivedDevices = new Dictionary<string, BaseDevice>();
    }

    public void Reset(Dictionary<string, BaseDevice> devices)
    {
        _devicesSnapshot = devices;
        _receivedDevices.Clear();
    }

    public void UpdateDeviceStatuses(params BaseDevice[] devices)
    {
        foreach (var device in devices)
        {
            if (_devicesSnapshot.TryGetValue(device.Id, out BaseDevice currentDevice))
            {
                currentDevice.DeviceStatus = DeviceStatuses.Loaded;
            }
            else
            {
                device.DeviceStatus = DeviceStatuses.New;
                _devicesSnapshot.Add(device.Id, device);
            }

            _receivedDevices[device.Id] = device;
        }
    }

    public void CheckWhichDevicesLost()
    {
        foreach (var device in _devicesSnapshot)
        {
            if (!_receivedDevices.ContainsKey(device.Key))
                device.Value.DeviceStatus = DeviceStatuses.Lost;
        }

    }
}