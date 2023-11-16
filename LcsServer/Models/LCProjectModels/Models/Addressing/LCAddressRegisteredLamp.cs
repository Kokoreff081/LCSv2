using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;


namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public class LCAddressRegisteredLamp : ISaveLoad
{
    [SaveLoad]
    public int LampId { get; set; }
    [SaveLoad]
    public int ArrayIndex { get; set; }
    [SaveLoad]
    public bool IsReverse { get; set; }
    [SaveLoad]
    public int ArraySize { get; set; }

    public void Save(string projectFolderPath)
    {
    }

    public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
    {
    }
}