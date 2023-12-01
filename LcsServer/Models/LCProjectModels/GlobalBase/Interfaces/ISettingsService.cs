using LcsServer.Models.LCProjectModels.GlobalBase.Scheduler;
using LcsServer.Models.LCProjectModels.GlobalBase.Settings;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

public interface ISettingsService
{
    //BaseSetting GetSettings(Guid settingsId);
    BaseSettings GetSettings(SettingsType type);
    bool IsRegister(SettingsType type);
    //bool IsRegister(Guid settingsId);
    void Register(SettingsType type, BaseSettings setting, string registerName);
    void Save(SettingsType type);
    //void Save(SettingsLocation settingsLocation);
    //void SaveAs(SettingsType type, string settingsFolderPath);
    void SaveAs(SettingsLocation settingsLocation, string settingsFolderPath);
    string ProjectSettingsPath { get; set; }
    void ResetAllProjectSettings();
    void SetAllSettingsAsUnchanged();
}