using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.Interfaces;

public interface IStatusChecker
{
    /// <summary>
    /// Reset the checker
    /// </summary>
    /// <param name="devices">The devices we are waiting for</param>
    void Reset(Dictionary<string, BaseDevice> devices);

    /// <summary>
    /// All request finished. No other devices expected
    /// </summary>
    void CheckWhichDevicesLost();

    /// <summary>
    /// New devices received. Update their statuses
    /// </summary>
    /// <param name="devices">New received devices</param>
    void UpdateDeviceStatuses(params BaseDevice[] devices);
}