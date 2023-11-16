using System.Net;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LightCAD.UI.Strings;

namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public class LCModbusAddressDevice : LCAddressObject, ISaveLoad
{
    private bool _isOnline;

    public LCModbusAddressDevice(int id, int parentId, IPAddress address, int port, byte modbusUnitId)
    {
        Id = id;
        SaveParentId = parentId;
        Address = address;
        Port = port;
        ModbusUnitId = modbusUnitId;
    }

    public LCModbusAddressDevice(int id, int parentId, string name, IPAddress address, int port, byte modbusUnitId)
    {
        Id = id;
        SaveParentId = parentId;
        Name = name;
        Address = address;
        Port = port;
        ModbusUnitId = modbusUnitId;
    }

    public IPAddress Address { get; set; }

    public int Port { get; set; }

    public byte ModbusUnitId { get; set; }


    public bool IsOnline
    {
        get => _isOnline;
        set
        {
            if (_isOnline !=value)
                return;

            _isOnline = value;
            OnNameChanged();
        }
    }

    public override void UnsubscribeAllEvents()
    { }

    public override string ToString()
    {
        string isOnline = IsOnline ? Resources.Online : Resources.Offline;

        return $"{DisplayName} [Modbus {Address}] ({isOnline})";
    }

    //private int[] _saveInputIds;

    public void Save(string projectFolderPath)
    {
    }

    public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
    {
            
    }
}