using LcsServer.Models.LCProjectModels.GlobalBase.Settings;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scheduler;

public class SchedulerSettings : BaseSettings
{
    public const string Name = nameof(SchedulerSettings);

    public SchedulerSettings()
    {
        Location = SettingsLocation.Project;
        
        Started = new BoolParam("Started", nameof(Started));
        SchedulerFile = new StringParam("File", "File");
        ScheduleFiles = new ListStringParam(nameof(ScheduleFiles), nameof(ScheduleFiles));

        List.AddRange(new UniParam[] {
            Started,
            SchedulerFile,
            ScheduleFiles,
        });

        ToDefault();

        SubscriptionСhanges();
    }

    /// <summary>
    /// Запущен или нет планировщик
    /// </summary>
    public BoolParam Started { get; }

    /// <summary>
    /// Имя файла планировщика
    /// </summary>
    public StringParam SchedulerFile { get; }

    /// <summary>
    /// Список файлов планировщика по порядку
    /// </summary>
    public ListStringParam ScheduleFiles { get; }

    /// <summary>
    /// Установить свойствам дефолтные значения 
    /// </summary>
    public override void ToDefault()
    {
        Started.Value = false;
        ScheduleFiles.Value = new List<string>();
    }

    public int GetCurrentIndex()
    {
        return ScheduleFiles.Value.IndexOf(SchedulerFile);
    }
}

public enum SettingsLocation
{
    Application,
    Project
}