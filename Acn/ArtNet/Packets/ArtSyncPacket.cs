using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public class ArtSyncPacket : ArtNetPacket
    {
        public ArtSyncPacket() : base(ArtNetOpCodes.Sync)
        {
        }

        public ArtSyncPacket(ArtNetReceiveData data) : base(data)
        {
        }

        #region Packet Properties

        private short _aux = 0;

        public short Aux
        {
            get { return _aux; }
            set { _aux = value; }
        }

        #endregion

        protected override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);
            Aux = data.ReadInt16();
        }

        protected override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);
            data.WriteNetwork(Aux);
        }
    }
}
