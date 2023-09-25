namespace Acn.ArtNet
{
    public enum ArtNetOpCodes
    {
        None = 0,

        Poll = 0x20,
        PollReply = 0x21,

        Dmx = 0x50,
        Sync = 0x52,

        Address = 0x60,

        Input = 0x70,

        TodRequest = 0x80,
        TodData = 0x81,
        TodControl = 0x82,
        Rdm = 0x83,
        RdmSub = 0x84,

        ArtTrigger = 0x99,

        IpProg = 0xf8,
        IpProgReply = 0xf9
    }

    public enum ArtNetStyles
    {
        StNode = 0x00,
        StServer = 0x01,
        StMedia = 0x02,
        StRoute = 0x03,
        StBackup = 0x04,
        StConfig = 0x05
    }

    public enum ArtAddressCommand : byte
    {
        AcNone = 0x00, // No Action
        AcCancelMerge = 0x01, // The next ArtDmx packet cancels Node's merge mode
        AcLedNormal = 0x02, // Node front panel indicators operate normally
        AcLedMute = 0x03, // Node front panel indicators are muted
        AcLedLocate = 0x04, // Fast flash all indicators for locate
        AcResetRxFlags = 0x05, // Reset the receive DMX flags for errors, SI's, Text & Test packets
        AcAnalysisOn = 0x06, // Product signal analysis enabled
        AcAnalysisOff = 0x07, // Product signal analysis disabled

        AcFailHold = 0x08, // Failsafe mode = hold last state
        AcFailZero = 0x09, // Failsafe mode = clear outputs
        AcFailFull = 0x0a, // Failsafe mode = outputs to full
        AcFailScene = 0x0b, // Failsafe mode = playback failsafe scene
        AcFailRecord = 0x0c, // Failsafe mode = record current output as failsafe scene

        AcMergeLtp0 = 0x10, // Set Port 0 to merge in LTP.
        AcMergeLtp1 = 0x11, // Set Port 1 to merge in LTP.
        AcMergeLtp2 = 0x12, // Set Port 2 to merge in LTP.
        AcMergeLtp3 = 0x13, // Set Port 3 to merge in LTP.

        AcMergeHtp0 = 0x50, // Set Port 0 to merge in HTP. (Default Mode)
        AcMergeHtp1 = 0x51, // Set Port 1 to merge in HTP.
        AcMergeHtp2 = 0x52, // Set Port 2 to merge in HTP.
        AcMergeHtp3 = 0x53, // Set Port 3 to merge in HTP.

        AcArtNetSel0 = 0x60, // Set Port 0 to output DMX and RDM from Art-Net. (Default Mode)
        AcArtNetSel1 = 0x61, // Set Port 1 to output DMX and RDM from Art-Net.
        AcArtNetSel2 = 0x62, // Set Port 2 to output DMX and RDM from Art-Net.
        AcArtNetSel3 = 0x63, // Set Port 3 to output DMX and RDM from Art-Net.

        AcAcnSel0 = 0x70, // Set Port 0 to output DMX from sACN.
        AcAcnSel1 = 0x71, // Set Port 1 to output DMX from sACN.
        AcAcnSel2 = 0x72, // Set Port 2 to output DMX from sACN.
        AcAcnSel3 = 0x73, // Set Port 3 to output DMX from sACN.

        AcClearOp0 = 0x90, // Clear all data buffers associated with output port 0
        AcClearOp1 = 0x91, // Clear all data buffers associated with output port 1
        AcClearOp2 = 0x92, // Clear all data buffers associated with output port 2
        AcClearOp3 = 0x93, // Clear all data buffers associated with output port 3

        AcStyleDelta0 = 0xa0, // Set output style to delta on output port 0
        AcStyleDelta1 = 0xa1, // Set output style to delta on output port 1
        AcStyleDelta2 = 0xa2, // Set output style to delta on output port 2
        AcStyleDelta3 = 0xa3, // Set output style to delta on output port 3

        AcStyleConstant0 = 0xb0, // Set output style to continuous on output port 0
        AcStyleConstant1 = 0xb1, // Set output style to continuous on output port 1
        AcStyleConstant2 = 0xb2, // Set output style to continuous on output port 2
        AcStyleConstant3 = 0xb3, // Set output style to continuous on output port 3

        AcRdmEnable0 = 0xc0, // Enable RDM on output port 0
        AcRdmEnable1 = 0xc1, // Enable RDM on output port 1
        AcRdmEnable2 = 0xc2, // Enable RDM on output port 2
        AcRdmEnable3 = 0xc3, // Enable RDM on output port 3

        AcRdmDisable0 = 0xd0, // Disable RDM on output port 0
        AcRdmDisable1 = 0xd1, // Disable RDM on output port 1
        AcRdmDisable2 = 0xd2, // Disable RDM on output port 2
        AcRdmDisable3 = 0xd3 // Disable RDM on output port 3
    }

    public enum ArtNetOemCodes
    {
        OemOarwSm1 =
            0x09d0 //Company Name: Oarw, Product Name: Screen Monkey, Number of DMX Inputs: 0, Number of DMX Outputs: 1, Are dmx ports physical or emulated: Emulated, Is RDM Supported: Not at this time
    }
}