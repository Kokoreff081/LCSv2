using System.ComponentModel;
using str = LightCAD.UI.Strings.Resources;


namespace LcsServer.Models.LCProjectModels.Models.Scheduler.Enums;

/// <summary>
/// Дни недели
/// </summary>
[Flags]
public enum WeekDays
{
    [Description(nameof(str.WeekDays_Monday))]
    Monday = 1,

    [Description(nameof(str.WeekDays_Tuesday))]
    Tuesday = 2,

    [Description(nameof(str.WeekDays_Wednesday))]
    Wednesday = 4,

    [Description(nameof(str.WeekDays_Thursday))]
    Thursday = 8,

    [Description(nameof(str.WeekDays_Friday))]
    Friday = 16,

    [Description(nameof(str.WeekDays_Saturday))]
    Saturday = 32,
     
    [Description(nameof(str.WeekDays_Sunday))]
    Sunday = 64,

    [Description(nameof(str.SelectDate))]
    SpecifiedDate = 0,

    [Description(nameof(str.WeekDays_WeekendDays))]
    WeekendDays = Sunday | Saturday,

    [Description(nameof(str.WeekDays_WorkDays))]
    WorkDays = Monday | Tuesday | Wednesday | Thursday | Friday,

    [Description(nameof(str.WeekDays_EveryDay))]
    EveryDay = WeekendDays | WorkDays
}