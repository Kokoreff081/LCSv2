using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public abstract class ArtNetPacket
    {
        public ArtNetPacket(ArtNetOpCodes opCode)
        {
            OpCode = opCode;
        }

        public ArtNetPacket(ArtNetReceiveData data)
        {
            DataLength = data.DataLength - 12;  // Subtract ArtNet header
            ArtNetBinaryReader packetReader = new ArtNetBinaryReader(new MemoryStream(data.buffer));
            ReadData(packetReader);
        }
        
        public int DataLength { get; }

        public byte[] ToArray()
        {
            using MemoryStream stream = new MemoryStream();
            using var writeStream = new ArtNetBinaryWriter(stream);
            WriteData(writeStream);
            return stream.ToArray();
        }

        #region Packet Properties

        private string _protocol = "Art-Net";

        public string Protocol
        {
            get { return _protocol; }
            protected set { _protocol = value.Length > 8 ? value[..8] : value; }
        }


        private short _version = 14;

        public short Version
        {
            get { return _version; }
            protected set { _version = value; }
        }

        private ArtNetOpCodes _opCode = ArtNetOpCodes.None;

        public ArtNetOpCodes OpCode
        {
            get { return _opCode; }
            protected set { _opCode = value; }
        }

        #endregion

        protected virtual void ReadData(ArtNetBinaryReader data)
        {
            Protocol = data.ReadNetworkString(8);
            OpCode = (ArtNetOpCodes)data.ReadNetwork16();

            //For some reason the poll packet header does not include the version.
            if (OpCode != ArtNetOpCodes.PollReply)
                Version = data.ReadNetwork16();

        }

        protected virtual void WriteData(ArtNetBinaryWriter data)
        {
            data.WriteNetwork(Protocol, 8);
            data.WriteNetwork((short)OpCode);

            //For some reason the poll packet header does not include the version.
            if (OpCode != ArtNetOpCodes.PollReply)
                data.WriteNetwork(Version);

        }

        public static ArtNetPacket Create(ArtNetReceiveData data)
        {
            return (ArtNetOpCodes)data.OpCode switch
            {
                ArtNetOpCodes.Poll => new ArtPollPacket(data),
                ArtNetOpCodes.PollReply => new ArtPollReplyPacket(data),
                ArtNetOpCodes.Dmx => new ArtNetDmxPacket(data),
                ArtNetOpCodes.Sync => new ArtSyncPacket(data),
                ArtNetOpCodes.TodRequest => new ArtTodRequestPacket(data),
                ArtNetOpCodes.TodData => new ArtTodDataPacket(data),
                ArtNetOpCodes.TodControl => new ArtTodControlPacket(data),
                ArtNetOpCodes.Rdm => new ArtRdmPacket(data),
                ArtNetOpCodes.RdmSub => new ArtRdmSubPacket(data),
                ArtNetOpCodes.IpProgReply => new ArtIpProgReplyPacket(data),
                ArtNetOpCodes.IpProg => new ArtIpProgPacket(data),
                ArtNetOpCodes.Address => new ArtAddressPacket(data),
                ArtNetOpCodes.Input => new ArtInputPacket(data),
                ArtNetOpCodes.ArtTrigger => new ArtTriggerPacket(data),
                _ => new ArtNetUnknownPacket(data)
            };
        }
    }
}
