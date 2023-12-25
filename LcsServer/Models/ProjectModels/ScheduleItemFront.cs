using LcsServer.Models.LCProjectModels.Models.Scheduler.Enums;

namespace LcsServer.Models.ProjectModels;

public class ScheduleItemFront
{
    public int Id { get; set; }
    public int ScenarioId { get; set; }
    public string ScenarioName { get; set; }
    public bool IsLooped { get; set; }
    public bool IsSelected { get; set; }
    public StartTimeType StartTimeType { get; set; }
    public FinishTimeType FinishTimeType { get; set; }
    public bool IsFinishEnabled { get; set; }
    public int MinutesStart { get; set; }
    public int MinutesFinish { get; set; }
    public WeekDays SelectedWeekDays { get; set; }
    public DateTime SpecifiedDateTime { get; set; }
    public DateTime SpecifiedDateTimeFinish { get; set; }
    public int DimmingLevel { get; set; }
    public DateTime? StartDate { get
        {
            if (SpecifiedDateTimes.Count > 0)
            {
                return SpecifiedDateTimes[0].Date.ToLocalTime();
            }
            else
            {
                if (SpecifiedDateTime != null)
                {
                    if (SpecifiedDateTime.Year >= DateTime.Now.Year)
                        return SpecifiedDateTime.Date.ToLocalTime();
                    else 
                        return DateTime.Now.AddDays(1).Date.ToLocalTime();
                }
                return null;
            }
            return null;
        } 
    }
    public DateTime? FinishDate { get
        {
            if (!IsFinishEnabled)
                return null;
            if (SpecifiedDateTimes.Count > 0)
            {
                return SpecifiedDateTimes[0].Date.ToLocalTime();
            }
            else
            {
                if (SpecifiedDateTimeFinish != null)
                {
                    if (SpecifiedDateTime.Year >= DateTime.Now.Year)
                        return SpecifiedDateTimeFinish.Date.ToLocalTime();
                    else
                        return DateTime.Now.AddDays(1).Date.ToLocalTime();
                }

                return null;
            }
        } 
    }
    public TimeSpan? StartTime { 
        get {
            if (SpecifiedDateTimes.Count > 0)
            {
                return SpecifiedDateTimes[0].ToLocalTime().TimeOfDay;
            }
            else
            {
                if (SpecifiedDateTime != null)
                {
                    return SpecifiedDateTime.ToLocalTime().TimeOfDay;
                }
                return null;
            }
            return null;
        } 
    }
    public TimeSpan? FinishTime { 
        get {
            if (SpecifiedDateTimes.Count > 0)
            {
                return SpecifiedDateTimes[0].ToLocalTime().TimeOfDay;
            }
            else
            {
                if (SpecifiedDateTimeFinish != null)
                {
                    return SpecifiedDateTimeFinish.ToLocalTime().TimeOfDay;
                }
                return null;
            }
            return null;
        } 
    }
    public List<DateTime> SpecifiedDateTimes { get; set; }

    public string? Duration { get; set; }
    public bool TaskChanged { get; set; }
    
}