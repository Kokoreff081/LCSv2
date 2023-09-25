namespace Acn.Rdm.Broker
{
    public class RdmMessageAttribute:Attribute
    {
        public RdmMessageAttribute(RdmCommands command, RdmParameters parameterId)
        {
            Command = command;
            ParameterId = parameterId;
        }

        public RdmCommands Command { get; private set; }
        
        public RdmParameters ParameterId { get; private set; }      
    }
}
