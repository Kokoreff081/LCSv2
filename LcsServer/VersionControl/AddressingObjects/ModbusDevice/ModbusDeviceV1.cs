using LcsServer.Models.LCProjectModels.Models.Addressing;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.AddressingObjects.ModbusDevice;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class ModbusDeviceV1 : BaseVC
{
    private const string ModelClassName = "LCModbusAddressDevice";

    public string IpAddress { get; set; }

    public int Port { get; set; }

    public byte ModbusUnitId { get; set; }
        
    public override ISaveLoad ToConcreteObject()
    {
        System.Net.IPAddress.TryParse(IpAddress, out System.Net.IPAddress ipAddress);
        LCModbusAddressDevice modbusDevice = new LCModbusAddressDevice(Id, ParentId, Name, ipAddress, Port, ModbusUnitId);

        return modbusDevice;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LCModbusAddressDevice modbusDevice = (LCModbusAddressDevice)o;

        ModbusDeviceV1 modbusDeviceVc = new ModbusDeviceV1
        {
            Id = modbusDevice.Id,
            ParentId = modbusDevice.ParentId,
            Name = modbusDevice.Name,
            IpAddress = modbusDevice.Address.ToString(),
            Port = modbusDevice.Port,
            ModbusUnitId = modbusDevice.ModbusUnitId,
        };

        return modbusDeviceVc;
    }
}