using LcsServer.Models.LCProjectModels.Models.Scheduler.Enums;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Utils;

public static class DateTimeExtensions
{
    private static readonly Dictionary<WeekDays, int> WeekDaysOrder = new Dictionary<WeekDays, int>
    {
        {WeekDays.SpecifiedDate, 0},
        {WeekDays.Monday, 1},
        {WeekDays.Tuesday, 2},
        {WeekDays.Wednesday, 3},
        {WeekDays.Thursday, 4},
        {WeekDays.Friday, 5},
        {WeekDays.Saturday, 6},
        {WeekDays.Sunday, 7},
        {WeekDays.WorkDays, 8},
        {WeekDays.WeekendDays, 9},
        {WeekDays.EveryDay, 10},
    };

    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// Возвращает ближайшую дату по дню недели. Если текущий день = дню недели, то вернет текущий день
    /// </summary>
    /// <param name="dt">Текущая дата</param>
    /// <param name="dayOfWeek">День недели</param>
    /// <returns></returns>
    public static DateTime NextDayOfWeek(this DateTime dt, DayOfWeek dayOfWeek)
    {
        // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
        int daysUntilTuesday = ((int)dayOfWeek - (int)dt.DayOfWeek + 7) % 7;
        DateTime nextDayOfWeek = dt.AddDays(daysUntilTuesday);

        return nextDayOfWeek;
    }

    public static DayOfWeek DateTypeToDayOfWeek(this WeekDays weekDays)
    {
        switch (weekDays)
        {
            case WeekDays.Sunday:
                return DayOfWeek.Sunday;
            case WeekDays.Monday:
                return DayOfWeek.Monday;
            case WeekDays.Tuesday:
                return DayOfWeek.Tuesday;
            case WeekDays.Wednesday:
                return DayOfWeek.Wednesday;
            case WeekDays.Thursday:
                return DayOfWeek.Thursday;
            case WeekDays.Friday:
                return DayOfWeek.Friday;
            case WeekDays.Saturday:
                return DayOfWeek.Saturday;
            case WeekDays.SpecifiedDate:
            case WeekDays.WeekendDays:
            case WeekDays.WorkDays:
            case WeekDays.EveryDay:
            default:
                throw new ArgumentOutOfRangeException(nameof(weekDays), weekDays, null);
        }
    }
    
    public static WeekDays DayOfWeekToWeekDays(this DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch {
            DayOfWeek.Sunday => WeekDays.Sunday,
            DayOfWeek.Monday => WeekDays.Monday,
            DayOfWeek.Tuesday => WeekDays.Tuesday,
            DayOfWeek.Wednesday => WeekDays.Wednesday,
            DayOfWeek.Thursday => WeekDays.Thursday,
            DayOfWeek.Friday => WeekDays.Friday,
            DayOfWeek.Saturday => WeekDays.Saturday,
            _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
        };
    }

    public static int CompareTo(this WeekDays x, WeekDays y)
    {
        WeekDays x1 = GetSingleWeekDays(x);
        WeekDays y1 = GetSingleWeekDays(y);
        return WeekDaysOrder[x1].CompareTo(WeekDaysOrder[y1]);
    }

    private static WeekDays GetSingleWeekDays(WeekDays input)
    {
        if (input.HasFlag(WeekDays.EveryDay))
        {
            return WeekDays.EveryDay;
        }
        
        if (input.HasFlag(WeekDays.Monday))
        {
            return WeekDays.Monday;
        }
        
        if (input.HasFlag(WeekDays.Tuesday))
        {
            return WeekDays.Tuesday;
        }
        
        if (input.HasFlag(WeekDays.Wednesday))
        {
            return WeekDays.Wednesday;
        }
        
        if (input.HasFlag(WeekDays.Thursday))
        {
            return WeekDays.Thursday;
        }
        
        if (input.HasFlag(WeekDays.Friday))
        {
            return WeekDays.Friday;
        }
        
        if (input.HasFlag(WeekDays.Saturday))
        {
            return WeekDays.Saturday;
        }
        
        if (input.HasFlag(WeekDays.Sunday))
        {
            return WeekDays.Sunday;
        }

        return WeekDays.EveryDay;
    }
}