using System.Xml;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scheduler;
using NLog;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Settings;

public class SettingsService : ISettingsService
{
    private readonly Dictionary<SettingsType, string> _typeToFileName;
    private string _projectSettingsPath;
    private readonly Dictionary<SettingsType, BaseSettings> _allSettings;


    public SettingsService()
    {
        _typeToFileName = new Dictionary<SettingsType, string>();
        _allSettings = new Dictionary<SettingsType, BaseSettings>();
    }

    public string ProjectSettingsPath
    {
        get => _projectSettingsPath;
        set
        {
            _projectSettingsPath = value;
            FileManager.CreateIfNotExist(_projectSettingsPath);
            //issue 254 - Ошибка в стартовой комнате
            //Ошибка не воспроизводится, поэтому точку возникновения кидаем в try{}catch{} и будем по логам потом пытаться понять что это.
            try
            {
                foreach (var setting in _allSettings)
                {
                    if (setting.Value.Location == SettingsLocation.Project)
                    {
                        Load(setting.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error($"Some {ex.GetType()} Error occured in {ex.StackTrace} with message {ex.Message}");
            }
        }
    }

    public bool IsRegister(SettingsType type)
    {
        return _allSettings.ContainsKey(type);
    }

    public void Register(SettingsType type, BaseSettings setting, string registerName)
    {
        if (_allSettings.ContainsKey(type))
        {
            return;
        }

        _allSettings[type] = setting;
        _typeToFileName[type] = $"{registerName}.xml";
        Load(type);
    }

    public BaseSettings GetSettings(SettingsType type)
    {
        return _allSettings.TryGetValue(type, out BaseSettings setting) ? setting : null;
    }

    private void Load(SettingsType type)
    {
        if (_allSettings.ContainsKey(type) && _typeToFileName.TryGetValue(type, out string typeFileName))
        {
            BaseSettings setting = _allSettings[type];
            if (setting.Location == SettingsLocation.Project && string.IsNullOrEmpty(_projectSettingsPath))
            {
                return;
            }

            string settingsFolderPath = setting.Location == SettingsLocation.Application ? 
                FileManager.ApplicationSettingsFolderPath : 
                _projectSettingsPath;

            string fileName = Path.Combine(settingsFolderPath, typeFileName);

            if (File.Exists(fileName))
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(fileName);
                }
                catch (Exception e)
                {
                    LogManager.GetCurrentClassLogger().Error(e);
                }
                _allSettings[type].Load(doc);
            }
            else
            {
                _allSettings[type].ToDefault();
            }
        }
    }

    public void ResetAllProjectSettings()
    {
        foreach (var pair in _allSettings)
        {
            var settings = pair.Value;
            if (settings.Location == SettingsLocation.Project)
            {
                settings.ToDefault();
            }
        }
    }

    public void SetAllSettingsAsUnchanged()
    {
        foreach (var pair in _allSettings)
        {
            var settings = pair.Value;

            settings.IsChanged = false;
        }
    }

    public void Save(SettingsType type)
    {
        if (!_allSettings.TryGetValue(type, out BaseSettings setting))
        {
            return;
        }

        string settingsFolderPath = setting.Location == SettingsLocation.Application ? 
            FileManager.ApplicationSettingsFolderPath : 
            _projectSettingsPath;
        SaveAs(type, settingsFolderPath);
    }

    private void SaveAs(SettingsType type, string settingsFolderPath)
    {
        if (string.IsNullOrEmpty(settingsFolderPath))
        {
            return;
        }

        if (!_typeToFileName.TryGetValue(type, out string typeFileName) ||
            !_allSettings.TryGetValue(type, out BaseSettings setting))
        {
            return;
        }

        FileManager.CreateIfNotExist(settingsFolderPath);

        string fileName = Path.Combine(settingsFolderPath, typeFileName);

        XmlDocument doc = setting.Save();

        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }

        doc.Save(fileName);
    }

    public void SaveAs(SettingsLocation settingsLocation, string settingsFolderPath)
    {
        foreach (var settings in _allSettings)
        {
            if (settings.Value.Location == settingsLocation)
            {
                SaveAs(settings.Key, settingsFolderPath);
            }
        }
    }
}