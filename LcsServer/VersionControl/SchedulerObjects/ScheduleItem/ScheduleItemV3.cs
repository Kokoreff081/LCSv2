using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Scheduler;
using LcsServer.Models.LCProjectModels.Models.Scheduler.Enums;
using LCSVersionControl.Converters;
using Newtonsoft.Json;

namespace LCSVersionControl.SchedulerObjects.ScheduleItem;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]

public class ScheduleItemV3 : BaseVC
{
    private const string ModelClassName = "LCScheduleItem";

    /// <summary>
    /// Id сценария
    /// </summary>
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

    /// <summary>
    /// Время и дата, если тип даты на выбранную дату
    /// </summary>
    public List<DateTime> SpecifiedDateTimes { get; set; }

    /// <summary>
    /// Задание зациклиненое до следующего задания
    /// </summary>
    public bool IsLooped { get; set; }
    
    public StartTimeType TimeType { get; set; }
    
    public int Minutes { get; set; }


    public override ISaveLoad ToConcreteObject()
    {
        WeekDays weekDays = (WeekDays)Enum.Parse(typeof(WeekDays), DayType.ToString());

        LCScheduleItem scheduleItem = new LCScheduleItem(Id, ScenarioId,ScenarioName, weekDays, SpecifiedDateTime, SpecifiedDateTimes, IsLooped, TimeType, Minutes);

        return scheduleItem;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCScheduleItem scheduleItem = (LCScheduleItem)o;

        ScheduleItemV3 scheduleItemVc = new ScheduleItemV3
        {
            Id = scheduleItem.Id,
            Name = scheduleItem.Name,
            ParentId = scheduleItem.ParentId,
            ScenarioId = scheduleItem.ScenarioId,
            ScenarioName = scheduleItem.ScenarioNameForRestore,
            DayType = (int)scheduleItem.SelectedWeekDays,
            SpecifiedDateTime = scheduleItem.SpecifiedDateTime,
            SpecifiedDateTimes = scheduleItem.SpecifiedDateTimes,
            IsLooped = scheduleItem.IsLooped,
            TimeType = scheduleItem.TimeType,
            Minutes = scheduleItem.Minutes
        };

        return scheduleItemVc;
    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not ScheduleItemV2 scheduleItemV2)
        {
            return;
        }

        base.FromPrevious(baseVC);
        ScenarioId = scheduleItemV2.ScenarioId;
        DayType = scheduleItemV2.DayType;
        SpecifiedDateTime = scheduleItemV2.SpecifiedDateTime;
        SpecifiedDateTimes = new List<DateTime> {scheduleItemV2.SpecifiedDateTime};
        IsLooped = scheduleItemV2.IsLooped;
        TimeType = StartTimeType.Time;
        Minutes = 0;
    }
}