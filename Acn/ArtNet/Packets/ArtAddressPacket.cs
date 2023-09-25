using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public class ArtAddressPacket : ArtNetPacket
    {
     
        public ArtAddressPacket() : base(ArtNetOpCodes.Address)
        {
        }

        public ArtAddressPacket(ArtNetReceiveData data) : base(data)
        {
        }

        #region Packet Properties

        public byte NetSwitch { get; set; }

        public byte BindIndex { get; set; }

        public string ShortName { get; set; } = "";

        public string LongName { get; set; } = "";

        public byte[] SwIn { get; set; }

        public byte[] SwOut { get; set; }

        public byte SubSwitch { get; set; }
        
        public byte AcnPriority { get; set; } = 255;

        public ArtAddressCommand Command { get; set; }

        #endregion


        protected override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(NetSwitch);
            data.Write(BindIndex);
            data.WriteNetwork(ShortName, 18);
            data.WriteNetwork(LongName, 64);
            data.Write(SwIn);
            data.Write(SwOut);
            data.Write(SubSwitch);
            data.Write(AcnPriority);
            data.Write((byte)Command);
            //data.Write(new byte());
        }

        protected override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            NetSwitch = data.ReadByte();
            BindIndex = data.ReadByte();
            ShortName = data.ReadNetworkString(18);
            LongName = data.ReadNetworkString(64);
            SwIn = data.ReadBytes(4);
            SwOut = data.ReadBytes(4);
            SubSwitch = data.ReadByte();
            AcnPriority = data.ReadByte();
            Command = (ArtAddressCommand)data.ReadByte();
        }
    }
}
