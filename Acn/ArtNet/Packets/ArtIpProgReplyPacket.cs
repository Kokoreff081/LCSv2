using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public class ArtIpProgReplyPacket : ArtNetPacket
    {
        public ArtIpProgReplyPacket() : base(ArtNetOpCodes.IpProgReply)
        {
        }

        public ArtIpProgReplyPacket(ArtNetReceiveData data) : base(data)
        {
        }

        #region Packet Properties

        public short Port { get; set; }

        public byte Status { get; set; }

        public byte[] IpAddress { get; set; }

        public byte[] SubnetMask { get; set; }

        #endregion

        protected override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            data.ReadBytes(2); // Filler1-1. Pad length to match ArtPoll.
            data.ReadBytes(2); // Filler3-4. Pad length to match ArtIpProg.

            IpAddress = data.ReadBytes(4);

            SubnetMask = data.ReadBytes(4);

            Port = (byte)data.ReadNetwork16();

            Status = data.ReadByte();
        }
        
    }
}
