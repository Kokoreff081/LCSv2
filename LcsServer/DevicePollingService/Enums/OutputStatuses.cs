namespace LcsServer.DevicePollingService.Enums;

[Flags]
public enum OutputStatuses
{
    //Clr – Output is selected to transmit Art-Net. 
    SACN = 1,   //– Output is selected to transmit sACN.
    IsLtp = 2, // – Merge Mode is LTP.
    Dmx = 4, //  – DMX output short detected on power up
    ArtNet = 8, // Output is merging ArtNet data.
    DmxText = 16, // – Channel includes DMX512 text packets
    DmxSip = 32, // – Channel includes DMX512 SIP’s
    DmxTest = 64, // – Channel includes DMX512 test packets
    IsDataTransmitted = 128, // – Data is being transmitted
}