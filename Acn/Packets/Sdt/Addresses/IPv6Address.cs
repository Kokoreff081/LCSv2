namespace Acn.Packets.Sdt.Addresses
{
    public class IPv6Address:SdtAddress
    {
        public IPv6Address():base(SdtAddressTypes.IPv6)
        {
        }

        public override void WriteData(IO.AcnBinaryWriter data)
        {
            throw new NotImplementedException();
        }
    }
}
