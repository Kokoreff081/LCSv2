using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public class LCModbusDIPlayingEntityLink : ISaveLoad
{
    public int PlayingEntityId { get; set; }

    public int ModbusDeviceId { get; set; }

    public ushort PortNumber { get; set; }

    public LCModbusDIPlayingEntityLink(int playingEntityId, int modbusDeviceId, ushort portNumber)
    {
        PlayingEntityId = playingEntityId;
        ModbusDeviceId = modbusDeviceId ;
        PortNumber = portNumber;
    }

    public void Save(string projectFolderPath)
    {
    }

    public void Load(List<ISaveLoad> primitives, int indexInPrimitives)
    {
    }
}