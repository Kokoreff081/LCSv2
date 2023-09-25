using System.Net;

namespace Acn.Sockets
{
    public class NewPacketEventArgs<TPacketType> : EventArgs
    {
        public NewPacketEventArgs(IPEndPoint source, TPacketType packet)
        {
            Source = source;
            Packet = packet;
        }

        public IPEndPoint Source { get; }

        public TPacketType Packet { get; }
    }
}
