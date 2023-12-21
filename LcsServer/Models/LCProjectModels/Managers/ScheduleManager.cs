using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scheduler;
using LcsServer.Models.LCProjectModels.GlobalBase.Settings;
using LcsServer.Models.LCProjectModels.Models.Project;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LcsServer.Models.LCProjectModels.Models.Scheduler;
using LcsServer.Models.ProjectModels;
using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.Managers;

public class ScheduleManager : BaseLCObjectsManager
{
    public event EventHandler<UpdateObjectsEventArgs> ObjectsUpdated;
    public event Action ScheduleFileChanged;

    private readonly VersionControlManager _versionControlManagerEx;
    private readonly ScenarioManager _scenarioManager;
    private readonly LCProjectModels.GlobalBase.Interfaces.ISettingsService _settingsService;
    public ProjectChanger _pChanger;
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
        if (_pChanger.CurrentProject != null)
        {
            (double latitude, double longitude) coord = _pChanger.CurrentProject.ProjectInfo.GetCoordinates();
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


    public List<LCScheduleGroup> GetScheduleGroups(bool updateTime, ProjectToWeb CurrentProject)
    {
        List<LCScheduleGroup> scheduleGroups = GetPrimitives<LCScheduleGroup>().ToList();
        if (updateTime && CurrentProject != null)
        {
            (double latitude, double longitude) = CurrentProject.ProjectInfo.GetCoordinates();
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

    /*public string GetCurrentFilePath()
    {
        string projectPath = GetProjectPath();
        if (string.IsNullOrEmpty(projectPath))
            return string.Empty;

        return GetSchedulerFilePath(projectPath);
    }*/

    /*public string GetProjectPath()
    {
        if (_pChanger.CurrentProject == null)
            return string.Empty;

        return /*string.IsNullOrEmpty(_project.Path) ? _project.TempPath :#1# _pChanger.CurrentProject.Path;
    }*/

    /*public void ChangeSchedulerFile(string fileName)
    {
        string projectPath = GetProjectPath();
        Save(projectPath); // Сохраняет текущий файл планировщика

        RemoveAllObjects();

        if (_settingsService.GetSettings(LCProjectModels.GlobalBase.Settings.SettingsType.Scheduler) is SchedulerSettings schedulerSettings)
        {
            schedulerSettings.SchedulerFile.Value = fileName;
        }
        _settingsService.Save(SettingsType.Scheduler);

        string schedulerFolderPath = FileManager.GetScheduleFolderPath(projectPath);
        string schedulerFile = Path.Combine(schedulerFolderPath, fileName);
        if (!File.Exists(schedulerFile))
            Save(projectPath); // Создает новый файл планировщика (пустой)

        //Load(projectPath, _project);

        ScheduleFileChanged?.Invoke();
    }*/

    /*public void AddScheduleItems(string fileName)
    {
        //string schedulerFolderPath = FileManager.GetScheduleFolderPath(GetProjectPath());
        //FileManager.CreateIfNotExist(schedulerFolderPath);

        string schedulerFile = Path.Combine(schedulerFolderPath, fileName);

        Load(schedulerFile, true);

        ScheduleFileChanged?.Invoke();
    }*/
    
    public void NewSaveMethod(string projectPath, string projectName, bool autoSave, ProjectToWeb CurrentProject)
    {
        var schedulerFileName = FileManager.GetSchedulerFilePath(projectPath);
        FileManager.CreateIfNotExist(Path.GetDirectoryName(schedulerFileName));
        List<LCScheduleGroup> scheduleGroups = GetPrimitives<LCScheduleGroup>().ToList();
        if (!scheduleGroups.Any(a => a.DisplayName == "default"))
        {
            int counter = 1;
            var defaultGroup = new LCScheduleGroup(GetNewCurrentId(), counter, "default", 100, 0, true);
            defaultGroup.Description = "Default schedule group";
            defaultGroup.Schedules = new List<LCSchedule>() { new LCSchedule(GetNewCurrentId(), defaultGroup.Id, "Scheduler", 0, true) };
            AddObjects(new List<LCScheduleGroup>() { defaultGroup }.Cast<LCObject>().ToArray());
        }
        //            _versionControlManagerEx.ConvertToVCAndSave(GetPrimitives<LCObject>().OfType<ISaveLoad>(), projectPath, schedulerFileName, false);
        var groups = GetScheduleGroups(false, CurrentProject);
        var objectsToSave = new List<LCScheduleObject>();
        foreach (var group in groups)
        {
            objectsToSave.Add(group);
            objectsToSave.AddRange(group.GetGroupObjects());
        }

        _versionControlManagerEx.ConvertToVCAndSave(objectsToSave.OfType<ISaveLoad>()/*GetPrimitives<LCObject>().OfType<ISaveLoad>()*/, projectPath, schedulerFileName, false);

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
    /*public void Load(string schedulerFile)
    {
        
        
        /*ConvertLCTT();
        ConvertLcsked();#1#

        //string schedulerFile = FileManager.GetSchedulerFilePath(projectPath);//GetSchedulerFilePath(projectPath);

        if (!File.Exists(schedulerFile))
        {
            return;
        }

        Load(schedulerFile);
    }*/

    public void Load(string schedulerFile, bool generateNewId = false)
    {
        //List<ISaveLoad> schedulerObjects = _versionControlManagerEx.LoadAndConvertFromVC(schedulerFile, Path.GetExtension(schedulerFile).EndsWith("lctt"));
        List<ISaveLoad> schedulerObjects = _versionControlManagerEx.LoadAndConvertFromVC(schedulerFile, false);
        foreach(var item in schedulerObjects)
        {
            item.Load(GetPrimitives<LCScheduleObject>().OfType<ISaveLoad>().ToList(), 0);
        }
        //var groups = RestoreScheduleGroups(schedulerObjects);
        RestoreScheduleItems(schedulerObjects.OfType<LCScheduleGroup>().ToList(), generateNewId);

        AddObjects(schedulerObjects.Cast<LCObject>().ToArray());
    }
    private List<LCScheduleGroup> RestoreScheduleGroups(List<ISaveLoad> scheduleObjects)
    {
        var result = scheduleObjects.OfType<LCScheduleGroup>().ToList();
        for(int i=0; i<result.Count;i++)
        {
            var lcGroup = result[i];
            lcGroup.Schedules = new List<LCSchedule>();
            foreach(var lcSchedule in scheduleObjects.OfType<LCSchedule>().ToList())
            {
                lcSchedule.ScheduleItems = new List<LCScheduleItem>();
                foreach(var scheduleItem in scheduleObjects.OfType<LCScheduleItem>().ToList())
                {
                    if (scheduleItem.ParentId == lcSchedule.Id)
                        lcSchedule.ScheduleItems.Add(scheduleItem);
                }
                if(lcSchedule.ParentId == lcGroup.Id)
                    lcGroup.Schedules.Add(lcSchedule);
            }
        }

        return result;
    }
    /*private void LoadScheduleFileList()
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
    }*/

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
