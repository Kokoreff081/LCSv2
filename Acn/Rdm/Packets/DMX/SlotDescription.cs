﻿namespace Acn.Rdm.Packets.DMX
{
    /// <summary>
    /// This parameter is used for requesting an ASCII text description for DMX512 slot offsets.
    /// </summary>
    /// <remarks>
    /// If the responder does not support the Slot number requested, or cannot provide a text description
    /// for a slot number that it does support, the responder shall respond with
    /// NR_DATA_OUT_OF_RANGE.
    /// </remarks>
    public class SlotDescription
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.SlotDescription)
            {
            }

            public short SlotOffset { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SlotOffset = data.ReadNetwork16();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(SlotOffset);
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.SlotDescription)
            {
            }

            public short SlotOffset { get; set; }

            public string Description { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                SlotOffset = data.ReadNetwork16();
                Description = data.ReadNetworkString(Header.ParameterDataLength - 2);
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(SlotOffset);
                data.WriteNetwork(Description);
            }

            #endregion
        }
    }
}
