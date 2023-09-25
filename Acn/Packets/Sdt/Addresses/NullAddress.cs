namespace Acn.Packets.Sdt.Addresses
{
    public class NullAddress:SdtAddress
    {
        public NullAddress():base(SdtAddressTypes.Null)
        {
        }

        public override void WriteData(IO.AcnBinaryWriter data)
        {
        }
    }
}
