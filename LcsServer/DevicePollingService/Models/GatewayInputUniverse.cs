using System.Net;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;

namespace LcsServer.DevicePollingService.Models;

public class GatewayInputUniverse : GatewayUniverse
{
    private InputStatuses _inputStatus;
    private IServiceProvider _serviceProvider;
    public GatewayInputUniverse(
        string parentId,
        IPAddress address,
        int index,
        int portAddress,
        byte universe,
        PortTypes portType,
        InputStatuses inputStatus, IServiceProvider serviceProvider = null) : base(parentId, address, index, portAddress, universe, true, portType, serviceProvider)
    {
        _serviceProvider = _serviceProvider;
        InputStatus = inputStatus;
    }

    public InputStatuses InputStatus
    {
        get => _inputStatus;
        set => _inputStatus = value;
    }
}