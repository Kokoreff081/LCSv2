using System.Text;
using System.Net;

namespace Acn.ArtNet.IO
{
    public class ArtNetBinaryReader:BinaryReader
    {
        public ArtNetBinaryReader(Stream input)
            : base(input)
        {
        }

        public short ReadNetwork16()
        {
            return IPAddress.NetworkToHostOrder(ReadInt16());
        }
        
        public int ReadNetwork32()
        {
            return IPAddress.NetworkToHostOrder(ReadInt32());
        }

        public string ReadNetworkString(int length)
        {
            return Encoding.ASCII.GetString(ReadBytes(length));
        }
    }
}
