using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Scheduler;
using LCSVersionControl.Converters;
using Newtonsoft.Json;

namespace LCSVersionControl.SchedulerObjects;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class LCScheduleV1 : BaseVC
{
    private const string ModelClassName = "LCSchedule";

    public bool IsCurrentScheduleItem { get; set; }
    public int Index { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsPlaying { get; set; }
    public List<LCScheduleItem> ScheduleItems { get; set; }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCSchedule lcSchedule = o as LCSchedule;
        LCScheduleV1 lcScheduleVc = new LCScheduleV1()
        {
            Name = lcSchedule.Name,
            Id = lcSchedule.Id,
            ParentId = lcSchedule.ParentId,
            Index = lcSchedule.Index,
            IsCurrent = lcSchedule.IsCurrent,
            ScheduleItems = lcSchedule.ScheduleItems
        };

        return lcScheduleVc;
    }

    public override ISaveLoad ToConcreteObject()
    {
        LCSchedule lcSchedule = new LCSchedule(Id, ParentId, Name, Index,IsCurrent);
        lcSchedule.ScheduleItems = ScheduleItems;

        return lcSchedule;
    }
}