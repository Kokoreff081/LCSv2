using LcsServer.DevicePollingService.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scheduler;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LcsServer.Models.LCProjectModels.Models.Scheduler;
using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.Managers;

public class ScheduleManager : BaseLCObjectsManager
{
    public event EventHandler<UpdateObjectsEventArgs> ObjectsUpdated;
    public event Action ScheduleFileChanged;

    private readonly VersionControlManager _versionControlManagerEx;
    private readonly ScenarioManager _scenarioManager;
    private readonly ISettingsService _settingsService;

    public ScheduleManager(VersionControlManager versionControlManagerEx, ScenarioManager scenarioManager,
        ISettingsService settingsService)
    {
        _versionControlManagerEx = versionControlManagerEx;
        _scenarioManager = scenarioManager;
        _settingsService = settingsService;

    }

    public List<LCScheduleItem> GetSortedScheduleItems()
    {
        List<LCScheduleItem> scheduleItems = GetPrimitives<LCScheduleItem>().ToList();
        if (_project != null)
        {
            (double latitude, double longitude) coord = _project.ProjectInfo.GetCoordinates();
            foreach (LCScheduleItem scheduleItem in scheduleItems)
            {
                scheduleItem.UpdateTime(coord.latitude, coord.longitude);
            }
        }

        scheduleItems.Sort();
        //scheduleItems.ForEach(item =>
        //{
        //    item.Save(GetProjectPath());
        //});
        return scheduleItems;
    }


    public List<LCScheduleGroup> GetScheduleGroups(bool updateTime)
    {
        List<LCScheduleGroup> scheduleGroups = GetPrimitives<LCScheduleGroup>().ToList();
        if (updateTime && _project != null)
        {
            (double latitude, double longitude) = _project.ProjectInfo.GetCoordinates();
            foreach (var lcGroup in scheduleGroups)
            {
                var scheduleItems = lcGroup.Schedules.SelectMany(s => s.ScheduleItems).ToList();
                foreach (LCScheduleItem scheduleItem in scheduleItems)
                {
                    scheduleItem.UpdateTime(latitude, longitude);
                }
            }
        }

        return scheduleGroups;
    }

    public override void AddObjects(params LCObject[] objects)
    {
        var result = AddRangeToLcObjectsList(objects);
        ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(result, InformAction.Add));
    }

    public override void RemoveObjects(LCObject[] objects)
    {
        RemoveRangeFromLcObjectsList(objects);
        ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(new List<LCObject>(objects), InformAction.Remove));
    }

    public override void RemoveAllObjects()
    {
        ClearLcObjectsList();
        ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(null, InformAction.RemoveAll));
    }

    public string GetCurrentFilePath()
    {
        string projectPath = GetProjectPath();
        if (string.IsNullOrEmpty(projectPath))
            return string.Empty;

        return GetSchedulerFilePath(projectPath);
    }

    public string GetProjectPath()
    {
        if (_project == null)
            return string.Empty;

        return string.IsNullOrEmpty(_project.Path) ? _project.TempPath : _project.Path;
    }

    public void ChangeSchedulerFile(string fileName)
    {
        string projectPath = GetProjectPath();
        Save(projectPath); // Сохраняет текущий файл планировщика

        RemoveAllObjects();

        if (_settingsService.GetSettings(SettingsType.Scheduler) is SchedulerSettings schedulerSettings)
        {
            schedulerSettings.SchedulerFile.Value = fileName;
        }
        _settingsService.Save(SettingsType.Scheduler);

        string schedulerFolderPath = FileManager.GetScheduleFolderPath(projectPath);
        string schedulerFile = Path.Combine(schedulerFolderPath, fileName);
        if (!File.Exists(schedulerFile))
            Save(projectPath); // Создает новый файл планировщика (пустой)

        Load(projectPath, _project);

        ScheduleFileChanged?.Invoke();
    }

    public void AddScheduleItems(string fileName)
    {
        string schedulerFolderPath = FileManager.GetScheduleFolderPath(GetProjectPath());
        FileManager.CreateIfNotExist(schedulerFolderPath);

        string schedulerFile = Path.Combine(schedulerFolderPath, fileName);

        Load(schedulerFile, true);

        ScheduleFileChanged?.Invoke();
    }
    /// <summary>
    /// Аналог ConvertLCTT но для файлов с расширением .lcsked
    /// </summary>
    private void ConvertLcsked() {
        var projectPath = GetProjectPath();
        if (File.Exists(FileManager.GetSchedulerFilePath(projectPath)))
        {
            return;
        }
        var scheduleFiles = FileManager.GetFilesByExtensions(FileManager.GetScheduleFolderPath(projectPath), FileManager.SchedulerFileExtension).ToList();
        string schedulerFolderPath = FileManager.GetScheduleFolderPath(projectPath);
        var scheduleSettings = (SchedulerSettings)_settingsService.GetSettings(SettingsType.Scheduler);
        int counter = 1;
        var defaultGroup = new LCScheduleGroup(counter, counter, "default", 100, 0, true);
        defaultGroup.Description = "Default schedule group";
        counter++;
        foreach(string scheduleFile in scheduleFiles)
        {
            var index = scheduleFiles.IndexOf(scheduleFile);
            bool flag = scheduleSettings.SchedulerFile.Value.Equals(scheduleFile, StringComparison.InvariantCultureIgnoreCase);
            string name = Path.GetFileNameWithoutExtension(scheduleFile);
            var lcSchedule = new LCSchedule(counter,defaultGroup.ParentId,name, index, flag);
            counter++;
            List<ISaveLoad> schedulerObjects = _versionControlManagerEx.LoadAndConvertFromVC(scheduleFile, false);
            lcSchedule.ScheduleItems = schedulerObjects.OfType<LCScheduleItem>().ToList();
            defaultGroup.Schedules.Add(lcSchedule);
        }
        string schedulerFile = Path.Combine(schedulerFolderPath, FileManager.SchedulerFileNameNew);
        RestoreScheduleItems(new List<LCScheduleGroup>() { defaultGroup });
        AddObjects(new List<LCScheduleGroup>() { defaultGroup }.Cast<LCObject>().ToArray());
        _versionControlManagerEx.ConvertToVCAndSave(GetPrimitives<LCObject>().OfType<ISaveLoad>(), projectPath, schedulerFile, false);
        ClearLcObjectsList();
    }

    /// <summary>
    /// Конвертирует lctt файлы в lcschedule
    /// лютый костыль изза того что файл перезаписывается при кажлом чихе
    /// </summary>
    /// <param name="fileName"></param>
    private void ConvertLCTT()
    {
        if (FileManager.GetFilesByExtensions(FileManager.GetScheduleFolderPath(GetProjectPath()),
                FileManager.SchedulerFileExtension).Any())
        {
            return;
        }
        
        var projectPath = GetProjectPath();
        var scheduleFiles = FileManager.GetFilesByExtensions(FileManager.GetScheduleFolderPath(projectPath), ".lctt");

        string schedulerFolderPath = FileManager.GetScheduleFolderPath(projectPath);

        foreach (string scheduleFile in scheduleFiles)
        {
            List<ISaveLoad> schedulerObjects = _versionControlManagerEx.LoadAndConvertFromVC(scheduleFile, true);
            RestoreScheduleItems(schedulerObjects.OfType<LCScheduleGroup>().ToList());
            AddObjects(schedulerObjects.Cast<LCObject>().ToArray());
            string schedulerFile = Path.Combine(schedulerFolderPath, $"{Path.GetFileNameWithoutExtension(scheduleFile)}{FileManager.SchedulerFileExtension}");
            _versionControlManagerEx.ConvertToVCAndSave(GetPrimitives<LCObject>().OfType<ISaveLoad>(), projectPath, schedulerFile, false);
            ClearLcObjectsList();
        }
    }
    public void NewSaveMethod(string projectPath, string projectName, bool autoSave)
    {
        var schedulerFileName = FileManager.GetSchedulerFilePath(projectPath);
        FileManager.CreateIfNotExist(Path.GetDirectoryName(schedulerFileName));
        _versionControlManagerEx.ConvertToVCAndSave(GetPrimitives<LCObject>().OfType<ISaveLoad>(), projectPath, schedulerFileName, false);
    }

    public void Save(string projectPath)
    {
        var schedulerFileName = ((SchedulerSettings)_settingsService.GetSettings(SettingsType.Scheduler)).SchedulerFile.Value;

        string schedulerFolderPath = FileManager.GetScheduleFolderPath(projectPath);
        string schedulerFile = Path.Combine(schedulerFolderPath, schedulerFileName);

        FileManager.CreateIfNotExist(schedulerFolderPath);

        _versionControlManagerEx.ConvertToVCAndSave(GetPrimitives<LCObject>().OfType<ISaveLoad>(), projectPath, schedulerFile, false);
    }

    /// <summary>
    /// Загрузить Файл планировщика
    /// </summary>
    /// <param name="projectPath">Путь до папки проекта</param>
    /// <param name="project">Проект</param>
    /// <returns>Список объектов планировщика</returns>
    public void Load(string projectPath)
    {
        
        
        ConvertLCTT();
        ConvertLcsked();

        string schedulerFile = FileManager.GetSchedulerFilePath(projectPath);//GetSchedulerFilePath(projectPath);

        if (!File.Exists(schedulerFile))
        {
            return;
        }

        Load(schedulerFile);
    }

    private void Load(string schedulerFile, bool generateNewId = false)
    {
        //List<ISaveLoad> schedulerObjects = _versionControlManagerEx.LoadAndConvertFromVC(schedulerFile, Path.GetExtension(schedulerFile).EndsWith("lctt"));
        List<ISaveLoad> schedulerObjects = _versionControlManagerEx.LoadAndConvertFromVC(schedulerFile, false);
        foreach(var item in schedulerObjects)
        {
            item.Load(GetPrimitives<LCScheduleObject>().OfType<ISaveLoad>().ToList(), 0, GetProjectPath());
        }
        RestoreScheduleItems(schedulerObjects.OfType<LCScheduleGroup>().ToList(), generateNewId);

        AddObjects(schedulerObjects.Cast<LCObject>().ToArray());
    }

    private void LoadScheduleFileList()
    {
        SchedulerSettings schedulerSettings = (SchedulerSettings)_settingsService.GetSettings(SettingsType.Scheduler);
        List<string> scheduleFilesList = schedulerSettings.ScheduleFiles.Value;
        var scheduleFiles = FileManager.GetFilesByExtensions(FileManager.GetScheduleFolderPath(GetProjectPath()), FileManager.SchedulerFileExtension).Select(x => Path.GetFileName(x)).ToList();

        foreach (string scheduleFile in scheduleFiles)
        {
            if (!scheduleFilesList.Contains(scheduleFile))
                scheduleFilesList.Add(scheduleFile);
        }

        foreach (string scheduleFile in scheduleFilesList.ToList())
        {
            if (!scheduleFiles.Contains(scheduleFile))
                scheduleFilesList.Remove(scheduleFile);
        }

        if (scheduleFilesList.Count == 0)
            scheduleFilesList.Add(FileManager.SchedulerFileName);

        schedulerSettings.ScheduleFiles.Value = scheduleFilesList;

        if (!scheduleFilesList.Contains(schedulerSettings.SchedulerFile.Value))
        {
            schedulerSettings.SchedulerFile.Value = scheduleFilesList.FirstOrDefault();
        }
    }

    private string GetSchedulerFilePath(string projectPath)
    {
        string schedulerFolderPath = FileManager.GetScheduleFolderPath(projectPath);
        FileManager.CreateIfNotExist(schedulerFolderPath);

        SchedulerSettings schedulerSettings = (SchedulerSettings)_settingsService.GetSettings(SettingsType.Scheduler);

        if (schedulerSettings == null)
        {
            return string.Empty;
        }

        string schedulerFileName = string.IsNullOrEmpty(schedulerSettings.SchedulerFile?.Value) ? 
            FileManager.GetSchedulerFilePath(projectPath) : 
            schedulerSettings.SchedulerFile.Value;

        string schedulerFile = Path.Combine(schedulerFolderPath, schedulerFileName);

        if (!File.Exists(schedulerFile))
        {
            schedulerFile = FileManager.GetFilesByExtensions(schedulerFolderPath, FileManager.SchedulerFileExtension).FirstOrDefault();
            if (string.IsNullOrEmpty(schedulerFile))
            {
                schedulerFile = Path.Combine(schedulerFolderPath, FileManager.GetSchedulerFilePath(projectPath));
                schedulerSettings.SchedulerFile.Value = FileManager.SchedulerFileName;
            }
        }

        return schedulerFile;
    }

    private void RestoreScheduleItems(List<LCScheduleGroup> scheduleGroups, bool generateNewId = false)
    {
        foreach (var scheduleGroup in scheduleGroups)
        {
            if (generateNewId)
                scheduleGroup.Id = GetNewCurrentId();
            foreach (var lcSchedule in scheduleGroup.Schedules)
            {
                foreach (var scheduleItem in lcSchedule.ScheduleItems)
                {
                    if (string.IsNullOrEmpty(scheduleItem.ScenarioNameForRestore))
                        // Неправильный вариант из предыдущей версии (id сценариям присваиваются новые так как они могут быть в разных файлах)
                        scheduleItem.Scenario = _scenarioManager.GetLCObjectById(scheduleItem.ScenarioId) as Scenario;
                    else
                        scheduleItem.Scenario = _scenarioManager.GetPrimitives<Scenario>().FirstOrDefault(x => x.Name.Equals(scheduleItem.ScenarioNameForRestore, StringComparison.InvariantCultureIgnoreCase));

                    scheduleItem.SaveParentId = lcSchedule.Id;
                }
            }
        }
    }

}
public enum SettingsType: byte
{
    Application,
    Engineering,
    LightSettings,
    LightSettingsViewerOnly,
    //RenderingSettings_Application,
    RenderingSettings_Project,
    Project,
    Converter,
    ViewPortSettings,
    RenderTool,
    ReportSettings,
    GameModeSettings,
    VideoMaker,
    Hardware,
    Scheduler,
    //RemoteButtons,
    Modbus,
    REST,
}