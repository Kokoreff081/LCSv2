namespace LcsServer.DevicePollingService.Models;

public class BaseObject
{
    public string Id { get; }

    public string ParentId { get; }

    public BaseObject(string id, string parentId)
    {
        Id = id;
        ParentId = parentId;
    }
}