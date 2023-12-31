﻿using Acn.IO;

namespace Acn.Packets.Sdt
{
    public class StdGetSessions:AcnPdu
    {
        public StdGetSessions()
            : base((int) StdVectors.GetSessions,1)
        {
        }

        #region Packet Contents

        public Guid ComponentId { get; set; }

        #endregion

        #region Read/Write

        protected override void ReadData(AcnBinaryReader data)
        {
            ComponentId = new Guid(data.ReadBytes(16));
        }

        protected override void WriteData(AcnBinaryWriter data)
        {
            data.Write(ComponentId.ToByteArray());
        }

        #endregion
    }
}
