using System.Net;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;

namespace LcsServer.Models.DeviceModels;

public class InputGatewayUniverseToWeb : GatewayUniverseToWeb
{
    private InputStatuses _inputStatus;

    public InputGatewayUniverseToWeb(
        string parentId,
        IPAddress address,
        int index,
        int portAddress,
        byte universe,
        PortTypes portType,
        InputStatuses inputStatus) : base(parentId, address, index, portAddress, universe, true, portType)
    {
        InputStatus = inputStatus;
    }

    public InputStatuses InputStatus
    {
        get => _inputStatus;
        set => _inputStatus = value;
    }
}