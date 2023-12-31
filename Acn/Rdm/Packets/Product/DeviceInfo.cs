﻿namespace Acn.Rdm.Packets.Product
{
    /// <summary>
    /// This parameter is used to retrieve a variety of information about the device that is normally
    /// required by a controller.
    /// </summary>
    public class DeviceInfo
    {
        public class Get : RdmRequestPacket
        {
            public Get()
                : base(RdmCommands.Get,RdmParameters.DeviceInfo)
            {
            }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
            }

            #endregion
        }

        public class GetReply : RdmResponsePacket
        {
            public GetReply()
                : base(RdmCommands.GetResponse, RdmParameters.DeviceInfo)
            {
            }

            public short RdmProtocolVersion { get; set; }

            public short DeviceModelId { get; set; }

            public ProductCategories ProductCategory { get; set; }

            public int SoftwareVersionId { get; set; }

            public short DmxFootprint { get; set; }

            public byte DmxPersonality { get; set; }

            public byte DmxPersonalityCount { get; set; }

            public short DmxStartAddress { get; set; }

            public short SubDeviceCount { get; set; }

            public byte SensorCount { get; set; }

            #region Read and Write

            protected override void ReadData(RdmBinaryReader data)
            {
                RdmProtocolVersion = data.ReadNetwork16();
                DeviceModelId = data.ReadNetwork16();
                ProductCategory = (ProductCategories) data.ReadNetwork16();
                SoftwareVersionId = data.ReadNetwork32();
                DmxFootprint = data.ReadNetwork16();
                DmxPersonality = data.ReadByte();
                DmxPersonalityCount = data.ReadByte();
                DmxStartAddress = data.ReadNetwork16();
                SubDeviceCount = data.ReadNetwork16();
                SensorCount = data.ReadByte();
            }

            protected override void WriteData(RdmBinaryWriter data)
            {
                data.WriteNetwork(RdmProtocolVersion);
                data.WriteNetwork(DeviceModelId);
                data.WriteNetwork((short) ProductCategory);
                data.WriteNetwork(SoftwareVersionId);
                data.WriteNetwork(DmxFootprint);
                data.Write(DmxPersonality);
                data.Write(DmxPersonalityCount);
                data.WriteNetwork(DmxStartAddress);
                data.WriteNetwork(SubDeviceCount);
                data.Write(SensorCount);
            }

            #endregion

            public override string ToString()
            {
                return string.Format("{0}",ProductCategory.ToString());
            }
        }
    }
}
