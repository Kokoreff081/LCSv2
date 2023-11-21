using Acn.ArtNet.Packets;
using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.Models.DeviceModels;

public class ToWebInterface
{
    
    public bool DeviceScanning { get; set; }
    public List<RdmDeviceToWeb> OnlyRdmDevice { get; set; }
    public List<ArtnetGateWayToWeb> OnlyArtNetControllers { get; set; }
    public List<GatewayUniverseToWeb> OnlyArtNetUniverses { get; set; }
    public List<NewToWebDevices> ToTreeTable { get; set; }
}
public class NewToWebDevices
{
    public string key { get; set;}
    public ColumnsToWebDevices data { get; set; }
    public List<NewToWebDevices> children { get; set; }
}

public class ColumnsToWebDevices
{
    public string DeviceName { get; set; }
    public string Label { get; set; }
    public DeviceStatuses DeviceStatus { get; set; }
    public int? DmxAddress { get; set; }
    public string SoftwareVersionIdLabel { get; set; }
    public int? DmxFootprint { get; set; }
    //public bool IsInProject { get; set; }
    public string Id { get; set; }
    public string Type { get; set; }
        
}