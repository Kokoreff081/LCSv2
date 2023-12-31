using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public enum ArtTodControlCommand
    {
        AtcNone = 0,
        AtcFlush = 1
    }

    public class ArtTodControlPacket : ArtNetPacket
    {
        public ArtTodControlPacket()
            : base(ArtNetOpCodes.TodControl)
        {
        }

        public ArtTodControlPacket(ArtNetReceiveData data)
            : base(data)
        {
            
        }

        #region Packet Properties

        public byte Net { get; set; }

        public ArtTodControlCommand Command { get; set; }

        public byte Address { get; set; }
	
	
        #endregion

        protected override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            data.BaseStream.Seek(9, SeekOrigin.Current);
            Net = data.ReadByte();
            Command = (ArtTodControlCommand) data.ReadByte();
            Address = data.ReadByte();
        }

        protected override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(new byte[9]);
            data.Write(Net);
            data.Write((byte) Command);
            data.Write(Address);
        }
	

    }
}
