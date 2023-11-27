using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Scheduler;
using LCSVersionControl.Converters;
using Newtonsoft.Json;

namespace LCSVersionControl.SchedulerObjects.ScheduleItem;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 2)]

public class ScheduleItemV2 : BaseVC
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
    /// Задание зациклиненое до следующего задания
    /// </summary>
    public bool IsLooped { get; set; }


    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCScheduleItem scheduleItem = (LCScheduleItem)o;

        ScheduleItemV2 scheduleItemVc = new ScheduleItemV2
        {
            Id = scheduleItem.Id,
            Name = scheduleItem.Name,
            ParentId = scheduleItem.ParentId,
            ScenarioId = scheduleItem.ScenarioId,
            ScenarioName = scheduleItem.ScenarioNameForRestore,
            DayType = (int)scheduleItem.SelectedWeekDays,
            SpecifiedDateTime = scheduleItem.SpecifiedDateTime,
            IsLooped = scheduleItem.IsLooped,
        };

        return scheduleItemVc;
    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not ScheduleItemV1 scheduleItemV1)
        {
            return;
        }

        base.FromPrevious(baseVC);
        ScenarioId = scheduleItemV1.ScenarioId;
        DayType = scheduleItemV1.DayType;
        SpecifiedDateTime = scheduleItemV1.SpecifiedDateTime;
        IsLooped = scheduleItemV1.IsLooped;
    }
}