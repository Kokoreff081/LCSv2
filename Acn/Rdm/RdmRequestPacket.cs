namespace Acn.Rdm
{
    public abstract class RdmRequestPacket : RdmPacket
    {
        public RdmRequestPacket(RdmCommands command, RdmParameters parameterId):base(command, parameterId)
        {
        }

        public byte PortId { get; set; }
    }
}
