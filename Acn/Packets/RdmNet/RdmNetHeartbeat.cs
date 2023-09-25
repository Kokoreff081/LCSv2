namespace Acn.Packets.RdmNet
{
    public class RdmNetHeartbeat : AcnPacket
    {
        public RdmNetHeartbeat()
            : base(ProtocolIds.Null)
        {
        }

        protected override void ReadData(IO.AcnBinaryReader data)
        {

        }

        protected override void WriteData(IO.AcnBinaryWriter data)
        {

        }
    }
}
