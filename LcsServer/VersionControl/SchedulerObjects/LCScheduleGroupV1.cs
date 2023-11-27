using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.Models.Scheduler;
using LCSVersionControl.Converters;
using Newtonsoft.Json;

namespace LCSVersionControl.SchedulerObjects;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class LCScheduleGroupV1 : BaseVC
{
    private const string ModelClassName = "LCScheduleGroup";
    public int Index { get; set; }
    public int DimmingLevel { get; set; }
    public bool IsCurrent { get; set; }
    public string Description { get; set; }
    public bool IsAutoStart { get; set; }
    public List<LCScheduleToSave> Schedules { get; set; }
    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCScheduleGroup lcGroup = (LCScheduleGroup)o;
        LCScheduleGroupV1 lcGroupVc = new LCScheduleGroupV1()
        {
            Id = lcGroup.Id,
            ParentId = lcGroup.ParentId,
            Name = lcGroup.Name,
            DimmingLevel = lcGroup.DimmingLevel,
            Index = lcGroup.Index,
            IsCurrent = lcGroup.IsCurrent,
            Schedules = lcGroup.SchedulesToSave,
            Description = lcGroup.Description,
            IsAutoStart = lcGroup.IsAutoStart
        };

        return lcGroupVc;
    }

    public override ISaveLoad ToConcreteObject()
    {
        LCScheduleGroup lcGroup = new LCScheduleGroup(Id, ParentId, Name, DimmingLevel, Index, IsCurrent, Description);
        lcGroup.SchedulesToSave = Schedules;
        lcGroup.IsAutoStart = IsAutoStart;
        return lcGroup;
    }
}