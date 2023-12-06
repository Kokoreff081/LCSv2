using System.Globalization;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;


namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public class LCAddressUniverse : LCAddressObject, ISaveLoad
{
    private int _uId;
    public event Action UniverseChanged;

    [Obsolete ("Используется в старом формате загрузке проекта")]
    public LCAddressUniverse() { }

    public LCAddressUniverse(int parentId, int uid)
    {
        SaveParentId = parentId;
        DmxSize = DmxSizeTypes.Dmx512;
        UId = uid;
    }

    public LCAddressUniverse(int id, string name, int parentId, DmxSizeTypes dmxSize, int uid)
    {
        Id = id;
        Name = name;
        SaveParentId = parentId;
        DmxSize = dmxSize;
        UId = uid;
    }

    [SaveLoad]
    public int UId
    {
        get => _uId;
        set
        {
            _uId = value;
            RaiseUniverseChanged();
        }
    }

    /// <summary>
    /// Размер пространства в одном порту
    /// </summary>
    public DmxSizeTypes DmxSize { get; set; }

    public override string DisplayName => $"Universe_{UId.ToString(CultureInfo.InvariantCulture)}";

    public void RaiseUniverseChanged()
    {
        UniverseChanged?.Invoke();
    }

    [SaveLoad]
    [Obsolete]
    private int _saveDmxSize;

    public void Save(string projectFolderPath)
    {
    }

    public void Load(List<ISaveLoad> primitives, int indexInPrimitives)
    {
    }

    private bool CheckLamp(LCAddressLamp addressLamp)
    {
        return addressLamp.LampAddress >= 0 && addressLamp.LampAddress + addressLamp.PixelsCount <= (int)DmxSize;
    }

    public override void UnsubscribeAllEvents()
    {
            
    }
}