using CoordinateSharp;
using GeoTimeZone;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Utils;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LcsServer.Models.LCProjectModels.Models.Scheduler.Enums;
using NLog;
using TimeZoneConverter;

namespace LcsServer.Models.LCProjectModels.Models.Scheduler;

public class LCScheduleItem : LCScheduleObject, ISaveLoad, IEquatable<LCScheduleItem>, IComparable<LCScheduleItem>
{
    private Scenario _scenario;
    private WeekDays _selectedWeekDays;
    private DateTime _specifiedDateTime;
    private DateTime _specifiedDateTimeFinish;
    private List<DateTime> _specifiedDateTimes; //TODO: LEX это свойство нигде не сохраняется
    private bool _isLooped = true;
    private bool _isFinishEnabled;
    private int _minutes;
    private int _minutesFinish;
    private StartTimeType _timeType;
    private FinishTimeType _timeTypeFinish;
    

    private static readonly WeekDays[] _weekDaysArray = {
        WeekDays.Monday, WeekDays.Tuesday, WeekDays.Wednesday, WeekDays.Thursday, WeekDays.Friday,
        WeekDays.Saturday, WeekDays.Sunday
    };

    public event Action<string> Changed;

    public string ScenarioNameForRestore { get; set; }

    public LCScheduleItem()
    {
        TimeType = StartTimeType.Time;
        Minutes = 0;
        ScenarioId = 0;
        SelectedWeekDays = WeekDays.EveryDay;
        SpecifiedDateTime = DateTime.MinValue;
        SpecifiedDateTimes = new List<DateTime>();
    }

    public LCScheduleItem(int id, int scenarioId, WeekDays selectedWeekDays, DateTime specifiedDateTime, List<DateTime> specifiedDateTimes, bool isLooped, StartTimeType timeType, int minutes)
    {
        Id = id;
        ScenarioId = scenarioId;
        SelectedWeekDays = selectedWeekDays;
        SpecifiedDateTime = specifiedDateTime;
        SpecifiedDateTimes = specifiedDateTimes; 
        IsLooped = isLooped;
        TimeType = timeType;
        Minutes = minutes;
    }

    public LCScheduleItem(int id, int scenarioId, string scenarioNameForRestore, WeekDays dayType, DateTime specifiedDateTime, List<DateTime> specifiedDateTimes, bool isLooped, StartTimeType timeType, int minutes):this(id, scenarioId, dayType, specifiedDateTime, specifiedDateTimes, isLooped, timeType, minutes)
    {
        ScenarioNameForRestore = scenarioNameForRestore;
    }

    /// <summary>
    /// Id сценария
    /// </summary>
    public int ScenarioId { get; set; }

    /// <summary>
    /// Тип даты
    /// </summary>
    public WeekDays SelectedWeekDays
    {
        get => _selectedWeekDays;
        set
        {
            if (_selectedWeekDays != value)
            {
                _selectedWeekDays = value;
                Changed?.Invoke(nameof(SelectedWeekDays));
            }
        }
    }
    
    /// <summary>
    /// Дополнительные минуты для Астрономического времени
    /// </summary>
    public int Minutes
    {
        get => _minutes;
        set
        {
            if (_minutes != value)
            {
                _minutes = value;
                Changed?.Invoke(nameof(Minutes));
            }
        }
    }

    public int MinutesFinish
    {
        get => _minutesFinish;
        set
        {
            if (_minutesFinish != value)
            {
                _minutesFinish = value;
                Changed?.Invoke(nameof(MinutesFinish));
            }
        }
    }

    /// <summary>
    /// Дополнительные минуты для Астрономического времени
    /// </summary>
    public StartTimeType TimeType
    {
        get => _timeType;
        set
        {
            if (_timeType != value)
            {
                _timeType= value;
                Changed?.Invoke(nameof(TimeType));
            }
        }
    }

    public FinishTimeType FinishTimeType
    {
        get => _timeTypeFinish;
        set
        {
            if (_timeTypeFinish != value)
            {
                _timeTypeFinish = value;
                Changed?.Invoke(nameof(FinishTimeType));
            }
        }
    }

    /// <summary>
    /// Время и дата, если тип даты на выбранную дату
    /// </summary>
    public DateTime SpecifiedDateTime
    {
        get => _specifiedDateTime;
        set
        {
            if (_specifiedDateTime != value)
            {
                _specifiedDateTime = value;
                Changed?.Invoke(nameof(SpecifiedDateTime));
            }
        }
    }

    public DateTime SpecifiedDateTimeFinish
    {
        get => _specifiedDateTimeFinish;
        set
        {
            if (_specifiedDateTimeFinish != value)
            {
                _specifiedDateTimeFinish = value;
                Changed?.Invoke(nameof(SpecifiedDateTimeFinish));
            }
        }
    }

    /// <summary>
    /// Времена и даты, если тип даты на выбранную дату
    /// </summary>
    public List<DateTime> SpecifiedDateTimes
    {
        get => _specifiedDateTimes;
        set
        {
            if (_specifiedDateTimes != value)
            {
                _specifiedDateTimes = value;
                Changed?.Invoke(nameof(SpecifiedDateTimes));
            }
        }
    }

    /// <summary>
    /// Сценарий найденый по ScenarioId
    /// </summary>
    public Scenario Scenario
    {
        get => _scenario;
        set
        {
            if (_scenario == value)
            {
                return;
            }

            if (_scenario != null)
            {
                _scenario.TotalTicksChanged -= OnScenarioTotalTicksChanged;
            }

            _scenario = value;

            if (_scenario != null)
            {
                _scenario.TotalTicksChanged += OnScenarioTotalTicksChanged;
            }

            ScenarioId = _scenario?.Id ?? 0;
            Changed?.Invoke(nameof(Scenario));
        }
    }

    private void OnScenarioTotalTicksChanged(object sender, EventArgs e)
    {
        Changed?.Invoke(nameof(Scenario));
    }

    /// <summary>
    /// Задание зациклиненое до следующего задания
    /// </summary>
    public bool IsLooped
    {
        get => _isLooped;
        set
        {
            if (_isLooped == value)
            {
                return;
            }

            _isLooped = value;
            Changed?.Invoke(nameof(IsLooped));
        }
    }

    public bool IsFinishEnabled
    {
        get => _isFinishEnabled;
        set
        {
            if (_isFinishEnabled == value)
            {
                return;
            }

            _isFinishEnabled = value;
            Changed?.Invoke(nameof(IsFinishEnabled));
        }
    }

    /// <summary>
    /// Расчитывает время старта задания
    /// </summary>
    /// <returns>Дата и время старта</returns>
    public (DateTime begin, DateTime end, double diff) GetScenarioBeginEnd(DateTime currentDateTime, bool next)
    {
        TimeSpan time = SpecifiedDateTime.TimeOfDay;
        
        List<DateTime> dates = new List<DateTime>();

        if (SelectedWeekDays == WeekDays.SpecifiedDate)
        {
            foreach (DateTime dateTime1 in SpecifiedDateTimes)
            {
                dates.Add(dateTime1);
            }
        }
        else
        {
            foreach (WeekDays value in _weekDaysArray)
            {
                if (SelectedWeekDays.HasFlag(value))
                {
                    DateTime nextDate = currentDateTime.NextDayOfWeek(value.DateTypeToDayOfWeek()).Date;
                    dates.Add(nextDate.AddDays(-7));
                    dates.Add(nextDate);
                    dates.Add(nextDate.AddDays(7));
                }
            }
        }

        DateTime nearestDay = currentDateTime.AddYears(500);
        double tempdiff = next ? -double.MaxValue : double.MaxValue;
        foreach (DateTime date in dates)
        {
            DateTime datetime = date.Date.Add(time);
            double diff = (currentDateTime - datetime).TotalSeconds;//Math.Abs((dateTime1 - currentDateTime.Date).TotalDays);
            
            switch (next)
            {
                // if (diff == 0)
                // {
                //     nearestDay = datetime;
                //     break;
                // }
                case false when diff > 0 && diff < tempdiff:
                    nearestDay = datetime;
                    tempdiff = diff;
                    break;
                case true when diff <= 0 && diff > tempdiff:
                    nearestDay = datetime;
                    tempdiff = diff;
                    break;
            }
        }

        DateTime begin = nearestDay;//.Date.Add(time);
        DateTime end = IsLooped ? begin.AddYears(1000): begin.AddMilliseconds(Scenario.TotalTicks);// + one day in ms
        
        LogManager.GetCurrentClassLogger().Debug($"{(Scenario == null ? "NONAME" : Scenario.Name)} begin:{begin}, end:{end}");
        if (next)
        {
            Start2 = begin.ToString("dd.MM.yyyy HH:mm:ss");
        }
        else
        {
            Start1 = begin.ToString("dd.MM.yyyy HH:mm:ss");
        }
        return (begin, end, tempdiff);
    }

    public void Save(string projectFolderPath)
    {
        ScenarioNameForRestore = Scenario?.Name;
    }

    public void Load(List<ISaveLoad> primitives, int indexInPrimitives)
    {

    }

    public int CompareTo(LCScheduleItem other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (ReferenceEquals(null, other))
        {
            return 1;
        }

        var dayTypeComparison = DateTimeExtensions.CompareTo(SelectedWeekDays, other.SelectedWeekDays);
        if (dayTypeComparison != 0)
        {
            return dayTypeComparison;
        }

        var specifiedDateTimeComparison = SpecifiedDateTime.CompareTo(other.SpecifiedDateTime);
        if (specifiedDateTimeComparison != 0)
        {
            return specifiedDateTimeComparison;
        }

        return IsLooped.CompareTo(other.IsLooped);
    }

    public bool Equals(LCScheduleItem other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ScenarioId == other.ScenarioId && SelectedWeekDays == other.SelectedWeekDays && SpecifiedDateTime.Equals(other.SpecifiedDateTime) && IsLooped == other.IsLooped;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((LCScheduleItem)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Id;
            hashCode = (hashCode * 397) ^ (int)SelectedWeekDays;
            hashCode = (hashCode * 397) ^ SpecifiedDateTime.GetHashCode();
            hashCode = (hashCode * 397) ^ IsLooped.GetHashCode();
            return hashCode;
        }
    }
    
    public string Start1 { get; private set; }
    public string Start2 { get; private set; }

    public void UpdateTime(double latitude, double longitude)
    {
        if (TimeType == StartTimeType.Time)
        {
            return;
        }

        var aa = GetScenarioBeginEnd(DateTime.Now, true);
        // sunrise/sunset calculations
        //Celestial cel = Celestial.CalculateCelestialTimes(latitude, longitude, aa.begin);
        TimeSpan time =
            TimeType == StartTimeType.Sunrize ? 
            Celestial.Get_Last_SunRise(latitude, longitude, aa.begin).TimeOfDay: 
            Celestial.Get_Last_SunSet(latitude, longitude, aa.begin).TimeOfDay;

        string tzIana = TimeZoneLookup.GetTimeZone(latitude, longitude).Result;
        TimeZoneInfo tzInfo = TZConvert.GetTimeZoneInfo(tzIana);
        //DateTimeOffset convertedTime = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, tzInfo);
        time = time.Add(tzInfo.BaseUtcOffset);
        UTCTime = tzInfo.DisplayName.Substring(0, tzInfo.DisplayName.IndexOf(" ", StringComparison.Ordinal));
        if (Minutes != 0)
        {
            time = time.Add(new TimeSpan(0, Minutes, 0)); 
        }
        SpecifiedDateTime = SpecifiedDateTime.Date.Add(time);
    }
    
    public string UTCTime { get; set; }
}