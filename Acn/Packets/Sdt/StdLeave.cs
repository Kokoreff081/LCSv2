﻿using Acn.IO;

namespace Acn.Packets.Sdt
{
    public class StdLeave : AcnPdu
    {
        public StdLeave()
            : base((int) StdVectors.Leave,1)
        {
        }

        #region Packet Contents

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
        }

        #endregion
    }
}
