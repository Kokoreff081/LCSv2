using Acn.IO;

namespace Acn.Packets.Sdt
{
    public class StdAck : AcnPdu
    {
        public StdAck()
            : base((int) StdVectors.Ack,1)
        {
        }

        #region Packet Contents

        public int ReliableSequenceNumber { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            ReliableSequenceNumber = data.ReadOctet4();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.WriteOctet(ReliableSequenceNumber);
        }

        #endregion
    }
}
