﻿namespace Acn.Packets.Sdt.Addresses
{
    public class IPv4Address:SdtAddress
    {
        public IPv4Address()
            : base(SdtAddressTypes.IPv4)
        {
        }

        public override void WriteData(IO.AcnBinaryWriter data)
        {
            throw new NotImplementedException();
        }
    }
}
