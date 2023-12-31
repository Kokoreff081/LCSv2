using Acn.Rdm;
using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public class ArtTodDataPacket : ArtNetPacket
    {
        public ArtTodDataPacket()
            : base(ArtNetOpCodes.TodData)
        {
            RdmVersion = 1;
            Devices = new List<UId>();
        }

        public ArtTodDataPacket(ArtNetReceiveData data)
            : base(data)
        {
            
        }

        #region Packet Properties

        public byte RdmVersion { get; set; }

        public byte Port { get; set; }

        public byte Net { get; set; }

        public byte BindIndex { get; set; }

        public byte Command { get; set; }

        public byte Universe { get; set; }

        public short UIdTotal { get; set; }

        public byte BlockCount { get; set; }

        public List<UId> Devices { get; set; }
	
	
        #endregion

        protected override void ReadData(ArtNetBinaryReader data)
        {
            RdmBinaryReader rdmReader = new RdmBinaryReader(data.BaseStream);

            base.ReadData(data);

            RdmVersion = data.ReadByte();
            Port = data.ReadByte();
            data.BaseStream.Seek(6, SeekOrigin.Current);
            BindIndex = data.ReadByte();
            Net = data.ReadByte();
            Command = data.ReadByte();
            Universe = data.ReadByte();
            UIdTotal = rdmReader.ReadNetwork16();
            BlockCount = data.ReadByte();

            Devices = new List<UId>();
            int count = data.ReadByte();
            for (int n = 0; n < count; n++)
                Devices.Add(rdmReader.ReadUId());
        }

        protected override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            RdmBinaryWriter rdmWriter = new RdmBinaryWriter(data.BaseStream);

            data.Write(RdmVersion);
            data.Write(Port);
            data.Write(new byte[6]);
            data.Write(BindIndex);
            data.Write(Net);
            data.Write(Command);
            data.Write(Universe);
            rdmWriter.WriteNetwork(UIdTotal);
            data.Write(BlockCount);
            data.Write((byte) Devices.Count);

            foreach (UId id in Devices)
                rdmWriter.Write(id);
        }
	

    }
}
