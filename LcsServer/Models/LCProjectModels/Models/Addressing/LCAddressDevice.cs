using System.Collections.ObjectModel;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;


namespace LcsServer.Models.LCProjectModels.Models.Addressing;

/// <summary>
/// Модель управляющего устройства (контроллера)
/// </summary>
public abstract class LCAddressDevice : LCAddressObject, ISaveLoad
{
    private List<LCAddressDevicePort> _ports;

    protected LCAddressDevice(int id, int parentId, List<LCAddressDevicePort> ports)
    {
        SaveParentId = parentId;
        Id = id;
        _ports = ports;
    }


    protected LCAddressDevice(int id, int parentId, int[] ports)
    {
        SaveParentId = parentId;
        Id = id;
        _savePortIds = ports;
    }

    /// <summary>
    /// Тип протокола
    /// </summary>
    public virtual ProtocolType Protocol { get; }

    /// <summary>
    /// Порты на этом устройстве
    /// </summary>
    public ReadOnlyCollection<LCAddressDevicePort> Ports => new ReadOnlyCollection<LCAddressDevicePort>(_ports);

    public override void UnsubscribeAllEvents()
    {
            
    }

    #region SaveLoad

    [SaveLoad]
    private int[] _savePortIds;

    public virtual void Save(string projectFolderPath)
    {
        _savePortIds = Ports.Select(x => x.Id).ToArray();
    }

    public virtual void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
    {
        List<LCAddressDevicePort> allPorts = primitives.OfType<LCAddressDevicePort>().ToList();

        _ports = new List<LCAddressDevicePort>();
        if (_savePortIds != null)
        {
            foreach (var savePortId in _savePortIds)
            {
                LCAddressDevicePort port = allPorts.FirstOrDefault(x => x.Id == savePortId);
                if (port != null)
                    _ports.Add(port);
            }
        }
    }

    #endregion

}