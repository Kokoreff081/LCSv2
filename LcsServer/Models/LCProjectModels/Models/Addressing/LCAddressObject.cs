using LcsServer.Models.LCProjectModels.GlobalBase;

namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public abstract class LCAddressObject : LCObject
{
    [SaveLoad]
    protected int SaveParentId { get; set; }

    public override int ParentId => SaveParentId;

    public abstract void UnsubscribeAllEvents();
}