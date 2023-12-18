using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Scheduler;
using LcsServer.Models.LCProjectModels.Models.Scheduler.Enums;
using LCSVersionControl.Converters;
using Newtonsoft.Json;

namespace LCSVersionControl.SchedulerObjects.ScheduleItem;

    [JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 4)]
    public class ScheduleItemV4 : BaseVC
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
        /// Время и дата окончания, если тип даты на выбранную дату
        /// </summary>
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
        public FinishTimeType FinishTimeType { get; set; }
        public int Minutes { get; set; }

        public int MinutesFinish { get; set; }
        public int DimmingLevel { get; set; }

        public override ISaveLoad ToConcreteObject()
        {
            WeekDays weekDays = (WeekDays)Enum.Parse(typeof(WeekDays), DayType.ToString());
            LCScheduleItem scheduleItem = new LCScheduleItem(Id, ScenarioId, ScenarioName, weekDays, SpecifiedDateTime, SpecifiedDateTimes, IsLooped, TimeType, Minutes);
            scheduleItem.MinutesFinish = MinutesFinish;
            scheduleItem.SpecifiedDateTimeFinish = SpecifiedDateTimeFinish;
            scheduleItem.IsFinishEnabled = IsFinishEnabled;
            scheduleItem.FinishTimeType = FinishTimeType;
            scheduleItem.DimmingLevel = DimmingLevel;
            return scheduleItem;
        }
        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            LCScheduleItem scheduleItem = (LCScheduleItem)o;

            ScheduleItemV4 scheduleItemVc = new ScheduleItemV4
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
                Minutes = scheduleItem.Minutes,
                MinutesFinish = scheduleItem.MinutesFinish,
                SpecifiedDateTimeFinish = scheduleItem.SpecifiedDateTimeFinish,
                DimmingLevel = scheduleItem.DimmingLevel,
            };

            return scheduleItemVc;
        }
        public override void FromPrevious(BaseVC baseVC)
        {
            if (baseVC is not ScheduleItemV3 scheduleItemV3)
            {
                return;
            }

            base.FromPrevious(baseVC);
            ScenarioId = scheduleItemV3.ScenarioId;
            DayType = scheduleItemV3.DayType;
            SpecifiedDateTime = scheduleItemV3.SpecifiedDateTime;
            SpecifiedDateTimes = new List<DateTime> { scheduleItemV3.SpecifiedDateTime };
            IsLooped = scheduleItemV3.IsLooped;
            TimeType = StartTimeType.Time;
            Minutes = 0;
            MinutesFinish = 0;
            DimmingLevel = 100;
        }
    }
