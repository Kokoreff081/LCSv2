using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.SerializationModels;

public abstract class BaseObjectDto
{
    public abstract BaseObject ToBaseObject();
    public abstract string Type { get; }
}