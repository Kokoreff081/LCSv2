using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public class ArtInputPacket : ArtNetPacket
    {
        private byte[] inputs = { 0, 0, 0, 0 };
        public byte BindIndex { get; set; } = 0;

        public short NumPorts { get; set; } = 0;

        public byte[] Inputs
        {
            get => inputs;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The input must be an array of 4 bytes.");

                inputs = value;
            }
        }

        public ArtInputPacket() : base(ArtNetOpCodes.Input)
        {
        }

        public ArtInputPacket(ArtNetReceiveData data) : base(data)
        {
        }

        protected override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);
            data.ReadByte();
            BindIndex = data.ReadByte();
            NumPorts = data.ReadInt16();
            Inputs = data.ReadBytes(4);
        }

        protected override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);
            data.Write((byte)0);
            data.Write(BindIndex);
            data.WriteNetwork(NumPorts);
            data.Write(Inputs);
        }
    }
}