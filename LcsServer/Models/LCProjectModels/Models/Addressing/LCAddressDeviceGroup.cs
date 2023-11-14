using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public class LCAddressDeviceGroup: LCAddressObject, ISaveLoad
{
    public LCAddressDeviceGroup(int parentId)
    {
        SaveParentId = parentId;
    }

    public LCAddressDeviceGroup(int id, string name, int parentId)
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