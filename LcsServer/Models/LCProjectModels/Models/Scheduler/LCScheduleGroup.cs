using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Scheduler.Enums;

namespace LcsServer.Models.LCProjectModels.Models.Scheduler;

public class LCScheduleGroup : LCScheduleObject, ISaveLoad
{
    private int _index;
    private int _dimmingLevel;
    private bool _isCurrent;
    private string _description;
    private bool _isAutoStart;

    public event EventHandler DescriptionChanged;

    public LCScheduleGroup(int id, int parentId, string name, int dimmingLevel, int index, bool isCurrent, string description = "")
    {
        Id = id;
        SaveParentId = parentId;
        Name = name;
        DimmingLevel = dimmingLevel;
        Index = index;
        IsCurrent = isCurrent;
        Schedules = new List<LCSchedule>();
        _description = description;
    }
    public List<LCScheduleToSave> SchedulesToSave;
    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }
    public int DimmingLevel
    {
        get { return _dimmingLevel; }
        set { _dimmingLevel = value; }
    }
    public bool IsCurrent
    {
        get { return _isCurrent; }
        set { _isCurrent = value; }
    }

    public string Description
    {
        get { return _description; }
        set 
        {
            _description = value;
            OnDescriptionChanged();
        }
    }

    public bool IsAutoStart
    {
        get { return _isAutoStart; }
        set { _isAutoStart = value; }
    }
    public List<LCSchedule> Schedules { get; set; }

    private void OnDescriptionChanged()
    {
        DescriptionChanged?.Invoke(this, EventArgs.Empty);
    }
    public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
    {
        if (SchedulesToSave != null)
        {
            foreach (var item in SchedulesToSave) {
                var lcSchedule = new LCSchedule(item.Id, item.ParentId, item.Name, item.Index, item.IsCurrent);
                lcSchedule.IsCurrent = item.IsCurrent;
                lcSchedule.ScheduleItems = new List<LCScheduleItem>();
                foreach(var scheduleItem in item.lCScheduleItems)
                {
                    var loadedScheduleItem = new LCScheduleItem(scheduleItem.Id, scheduleItem.ScenarioId, (WeekDays)scheduleItem.DayType, scheduleItem.SpecifiedDateTime, scheduleItem.SpecifiedDateTimes, scheduleItem.IsLooped, scheduleItem.TimeType, scheduleItem.Minutes);
                    loadedScheduleItem.MinutesFinish = scheduleItem.MinutesFinish;
                    loadedScheduleItem.SpecifiedDateTimeFinish = scheduleItem.SpecifiedDateTimeFinish;
                    loadedScheduleItem.IsFinishEnabled = scheduleItem.IsFinishEnabled;
                    loadedScheduleItem.FinishTimeType = scheduleItem.FinishTT;
                    lcSchedule.ScheduleItems.Add(loadedScheduleItem);

                }
                Schedules.Add(lcSchedule);
            }
        }
    }

    public void Save(string projectFolderPath)
    {
        SchedulesToSave = new List<LCScheduleToSave>();
        foreach (var item in Schedules)
        {
            var saveItem = new LCScheduleToSave() { Index = item.Index, IsCurrent = item.IsCurrent, SaveParentId = item.ParentId, Id = item.Id, Name= item.Name };
            saveItem.IsCurrent = item.IsCurrent;
            saveItem.lCScheduleItems = new List<LCScheduleItemToSave>();
            foreach(var scheduleItem in item.ScheduleItems)
            {
                saveItem.lCScheduleItems.Add(new LCScheduleItemToSave(scheduleItem.Id, scheduleItem.ParentId, scheduleItem.ScenarioId, scheduleItem.ScenarioNameForRestore, (int)scheduleItem.SelectedWeekDays, scheduleItem.SpecifiedDateTime, scheduleItem.SpecifiedDateTimes, scheduleItem.IsLooped, scheduleItem.Minutes, scheduleItem.TimeType, scheduleItem.SpecifiedDateTimeFinish, scheduleItem.MinutesFinish, scheduleItem.IsFinishEnabled, scheduleItem.FinishTimeType));
            }
            SchedulesToSave.Add(saveItem);
        }
    }
}
public class LCScheduleToSave : LCScheduleObject
{
    public bool IsCurrentScheduleItem { get; set; }
    public int Index { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsPlaying { get; set; }
    public List<LCScheduleItemToSave> lCScheduleItems { get; set; }
}

public class LCScheduleItemToSave : LCScheduleObject
{
    public LCScheduleItemToSave(int id, int parentId, int scenarioId, string scenarioName, int dayType, DateTime specDateTime, List<DateTime> specDateTimes, bool isLooped, int minutes, StartTimeType timeType, DateTime specDateTimeFinish, int minutesFinish, bool IsFinish, FinishTimeType FinishTimeType)
    {
        Id = id;
        SaveParentId = parentId;
        ScenarioId = scenarioId;
        ScenarioName = scenarioName;
        DayType = dayType;
        SpecifiedDateTime = specDateTime;
        SpecifiedDateTimes = specDateTimes;
        IsLooped = isLooped;
        Minutes = minutes;
        TimeType = timeType;
        SpecifiedDateTimeFinish = specDateTimeFinish;
        MinutesFinish = minutesFinish;
        IsFinishEnabled = IsFinish;
        FinishTT = FinishTimeType;
    }
    public int ScenarioId { get; set; }

    /// <summary>
    /// Id сценария
    /// </summary>
    public string ScenarioName { get; set; }

    /// <summary>
    /// Тип даты
    /// </summary>
    public int DayType { get; set; }

    /// <summary>
    /// Время и дата, если тип даты на выбранную дату
    /// </summary>
    public DateTime SpecifiedDateTime { get; set; }

    public DateTime SpecifiedDateTimeFinish { get; set; }

    /// <summary>
    /// Время и дата, если тип даты на выбранную дату
    /// </summary>
    public List<DateTime> SpecifiedDateTimes { get; set; }

    /// <summary>
    /// Задание зациклиненое до следующего задания
    /// </summary>
    public bool IsLooped { get; set; }
    public bool IsFinishEnabled { get; set; }

    public StartTimeType TimeType { get; set; }

    public int Minutes { get; set; }
    public int MinutesFinish { get; set; }
    public FinishTimeType FinishTT { get; set; }
}