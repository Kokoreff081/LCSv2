using LcsServer.DevicePollingService.Enums;

namespace LcsServer.DevicePollingService.Models;

public abstract class BaseDevice : BaseObject
{
    private DeviceStatuses _deviceStatus;


    public BaseDevice(string id, string parentId) : base(id, parentId)
    {
        _deviceStatus = DeviceStatuses.New;
    }

    public abstract string Type { get; }

    public DeviceStatuses DeviceStatus
    {
        get => _deviceStatus;
        set
        {
            if (!(_deviceStatus == DeviceStatuses.Warning && value == DeviceStatuses.Loaded)) // Состояние из Warning в Loaded перейти не может
                _deviceStatus = value;

            if (value == DeviceStatuses.Lost)
                OnDeviceLost();
        }
    }

    protected abstract void OnDeviceLost();

}