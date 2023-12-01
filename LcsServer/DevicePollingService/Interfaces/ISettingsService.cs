using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.Interfaces;

public interface ISettingsService
{
    BaseSettings GetSettings(DevicePollSettingsTypes type);
    bool IsRegistered(DevicePollSettingsTypes type);
    void Register(DevicePollSettingsTypes type, BaseSettings setting);
    void Save(DevicePollSettingsTypes type);
    void Load(DevicePollSettingsTypes type);
    void Load();
}