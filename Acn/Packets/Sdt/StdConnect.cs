using Acn.IO;

namespace Acn.Packets.Sdt
{
    public class StdConnect : AcnPdu
    {
        public StdConnect()
            : base((int) StdVectors.Connect,1)
        {
        }

        #region Packet Contents

        public int ProtocolId { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            ProtocolId = data.ReadOctet4();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.WriteOctet(ProtocolId);
        }

        #endregion
    }
}
