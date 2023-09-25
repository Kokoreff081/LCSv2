namespace Acn
{
    public class InvalidPacketException:InvalidOperationException
    {
        public InvalidPacketException(string message)
            : base(message)
        {
        }
    }
}
