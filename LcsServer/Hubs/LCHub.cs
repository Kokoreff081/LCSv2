using LcsServer.Models.ProjectModels;
using Microsoft.AspNetCore.SignalR;
using NLog;

namespace LcsServer.Hubs;

public class LCHub: Hub
{
    private static List<string> connectedClients = new List<string>();
    public override Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;
        connectedClients.Add(connectionId);
        return base.OnConnectedAsync();
    }
    public override Task OnDisconnectedAsync(Exception ex)
    {
        string connectionId = Context.ConnectionId;
        try
        {
            connectedClients.Remove(connectionId);
        }
        catch(Exception e)
        {
            //LogManager.GetInstance().ApplicationException(e.ToString());
        }
        return base.OnDisconnectedAsync(ex);
    }
    
    public async Task ProjectChanged(ProjectToWeb ptw)
    {
        if (connectedClients.Count > 0)
            await Clients.All.SendAsync("ProjectChanged", ptw);
    }
}