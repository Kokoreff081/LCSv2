using Acn.Packets.sAcn;

namespace Acn
{
    public static class AcnPacketFactory
    {
        static AcnPacketFactory()
        {
            //Port List
            RegisterPacketType((int)ProtocolIds.sACN, typeof(StreamingAcnDmxPacket));
            RegisterPacketType((int)ProtocolIds.RdmNet, typeof(RdmNetPacket));
        }

        private struct PacketKey
        {
            public PacketKey(int protocolId)
            {
                ProtocolId = protocolId;
            }

            public int ProtocolId;
        }

        private static Dictionary<PacketKey, Type> packetStore = new Dictionary<PacketKey, Type>();

        public static void RegisterPacketType(int protocolId, Type packetType)
        {
            PacketKey key = new PacketKey();
            key.ProtocolId = protocolId;

            packetStore[key] = packetType;
        }

        public static AcnPacket Build(AcnRootLayer header)
        {
            Type packetType;
            if (packetStore.TryGetValue(new PacketKey(header.ProtocolId), out packetType))
            {
                AcnPacket packet = (AcnPacket)Activator.CreateInstance(packetType);
                return packet;
            }

            return null;
        }

    }
}
