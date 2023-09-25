using LcsServer.DevicePollingService.Interfaces;

namespace LcsServer.DevicePollingService.Models;

public abstract class BaseSettings
{
    public bool IsChanged { get; set; }

    public abstract void Load(ISerializationManagerRDM serializationManager, byte[] data);
}