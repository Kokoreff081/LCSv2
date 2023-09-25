using System.Net;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;

namespace LcsServer.Models.DeviceModels;

public class OutputGatewayUniverseToWeb : GatewayUniverseToWeb
{
    private OutputStatuses _outputStatus;

    public OutputGatewayUniverseToWeb(
        string parentId,
        IPAddress address,
        int index,
        int portAddress,
        byte universe,
        PortTypes portType,
        OutputStatuses outputStatus) : base(parentId, address, index, portAddress, universe, false, portType)
    {
        OutputStatus = outputStatus;
    }

    public OutputStatuses OutputStatus
    {
        get => _outputStatus;
        set
        {
            _outputStatus = value;
        }
    }
}