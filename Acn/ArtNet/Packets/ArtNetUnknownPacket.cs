using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public class ArtNetUnknownPacket : ArtNetPacket
    {
        public ArtNetUnknownPacket(int opCode)
            : base((ArtNetOpCodes)opCode)
        {
        }

        public ArtNetUnknownPacket(ArtNetReceiveData data)
            : base(data)
        {
        }

        public byte[] Data { get; set; }

        protected override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            Data = data.ReadBytes(DataLength);
        }

        protected override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(Data);
        }
    }
}
