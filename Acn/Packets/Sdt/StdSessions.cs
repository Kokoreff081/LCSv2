﻿using Acn.IO;

namespace Acn.Packets.Sdt
{
    public class StdSessions : AcnPdu
    {
        public StdSessions()
            : base((int) StdVectors.Sessions,1)
        {
        }

        #region Packet Contents
        
        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            throw new NotImplementedException();
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
