using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Addressing.Enums;
using LightCAD.UI.Strings;

namespace LcsServer.Models.LCProjectModels.Models.Addressing;

/// <summary>
/// Модель управляющего устройства (контроллера)
/// </summary>
public class LCArtNetAddressDevice : LCAddressDevice
{
    private bool _isOnline;

    private string _ipAddress;

    public LCArtNetAddressDevice(int id, int parentId, List<LCAddressDevicePort> ports) : base(id, parentId, ports)
    {
        IpAddress = "192.168.0.1";
    }

    public LCArtNetAddressDevice(int id, string name, int parentId, int[] ports, string ipAddress) : base(id, parentId, ports)
    {
        Name = name;
        IpAddress = ipAddress;
    }

    /// <summary>
    /// Адрес
    /// </summary>
    public string IpAddress
    {
        get => _ipAddress;
        set
        {
            _ipAddress = value;
            OnNameChanged();
        }
    }

    public bool IsOnline
    {
        get => _isOnline;
        set
        {
            if (_isOnline == value)
                return;

            _isOnline = value;
            OnNameChanged();
        }
    }

    public override string ToString()
    {
        string isOnline = IsOnline ? Resources.Online: Resources.Offline;

        if (string.IsNullOrEmpty(IpAddress))
            return $"{DisplayName} [{Protocol}] ({isOnline})";

        return $"{DisplayName}_{IpAddress} [{Protocol}] ({isOnline})";
    }

    /// <summary>
    /// Тип протокола
    /// </summary>
    public override ProtocolType Protocol => ProtocolType.ArtNet;

    #region SaveLoad

    [SaveLoad]
    [Obsolete]
    private string _saveIpAddress;
    [SaveLoad]
    [Obsolete]
    private byte _saveProtocol;

    #endregion

}