using LcsServer.Models.LCProjectModels.GlobalBase;

namespace LcsServer.Models.LCProjectModels.Models.Scheduler;

public class LCScheduleObject : LCObject
{
    [SaveLoad]
    public int SaveParentId { get; set; }

    public override int ParentId => SaveParentId;

}