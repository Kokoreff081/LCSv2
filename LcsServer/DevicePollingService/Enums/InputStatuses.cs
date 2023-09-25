namespace LcsServer.DevicePollingService.Enums;

[Flags]
public enum InputStatuses
{
    IsErrorDetected = 4, // – Receive errors detected.
    IsDisabled = 8, //  – Input is disabled.
    DmxTextPackets = 16, //  – Channel includes DMX512 text packets
    DmxSips = 32, //  – Channel includes DMX512 SIP’s
    DmxTest = 64, //  – Channel includes DMX512 test packets
    IsDataReceived = 128, // Data received
}