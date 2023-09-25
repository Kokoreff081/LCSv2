using Acn.IO;
using System.Net;

namespace Acn
{
    public interface IProtocolFilter
    {
        int ProtocolId { get; }

        void ProcessPacket(IPEndPoint source, AcnRootLayer header, AcnBinaryReader data);
    }
}
