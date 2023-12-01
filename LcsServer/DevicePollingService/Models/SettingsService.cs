using System.Diagnostics;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Interfaces;

namespace LcsServer.DevicePollingService.Models;

public class SettingsService : ISettingsService
{
    private readonly Dictionary<DevicePollSettingsTypes, BaseSettings> _settings;

    private DatabaseContext _db;
    private readonly ISerializationManagerRDM _serializationManager;
    private IServiceProvider _serviceProvider;

    public SettingsService(ISerializationManagerRDM serializationManager, IServiceProvider serviceProvider)
    {
        _serializationManager = serializationManager;
        _serviceProvider = serviceProvider;
        _settings = new Dictionary<DevicePollSettingsTypes, BaseSettings>();
    }

    public BaseSettings GetSettings(DevicePollSettingsTypes type)
    {
        return _settings.ContainsKey(type) ? _settings[type] : new ParametersSettings();
    }

    public bool IsRegistered(DevicePollSettingsTypes type)
    {
        return _settings.ContainsKey(type);
    }

    public void Register(DevicePollSettingsTypes type, BaseSettings setting)
    {
        _settings[type] = setting;
    }

    public void Save(DevicePollSettingsTypes type)
    {
        byte[] settingsData = _serializationManager.Serialize(_settings[type]);
        FileManager.WriteAllBytes(GetSettingsFileName(type), settingsData);
    }

    public void Load(DevicePollSettingsTypes type)
    {
        try
        {
            _settings[type].Load(_serializationManager, File.ReadAllBytes(GetSettingsFileName(type)));
        }
        catch (FileNotFoundException e)
        {
            Debug.WriteLine($"Settings file was not found. Exception - {e}");
        }
        catch (Exception ex)
        {
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                _db.Events.Add(new Event() { level = "Error", dateTime = DateTime.Now, Description = ex.Message });
                _db.SaveChanges();
            }
        }
    }

    public void Load()
    {
        foreach (var setting in _settings)
        {
            Load(setting.Key);
        }
    }

    private string GetSettingsFileName(DevicePollSettingsTypes settingsType)
    {
        return Path.Combine(FileManager.ApplicationDataFolderPath, settingsType + ".json");
    }
}