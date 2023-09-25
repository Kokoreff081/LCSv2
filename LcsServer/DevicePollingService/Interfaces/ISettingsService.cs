using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.Interfaces;

public interface ISettingsService
{
    BaseSettings GetSettings(SettingsTypes type);
    bool IsRegistered(SettingsTypes type);
    void Register(SettingsTypes type, BaseSettings setting);
    void Save(SettingsTypes type);
    void Load(SettingsTypes type);
    void Load();
}