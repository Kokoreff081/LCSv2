

using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public class LCAddressUniverseGroup: LCAddressObject, ISaveLoad
{
    public LCAddressUniverseGroup(int parentId)
    {
        SaveParentId = parentId;
    }

    public LCAddressUniverseGroup(int id, string name, int parentId)
    {
        Id = id;
        Name = name;
        SaveParentId = parentId;
    }

    public void Save(string projectFolderPath)
    {
    }

    public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
    {
    }

    public override void UnsubscribeAllEvents()
    {
            
    }
}