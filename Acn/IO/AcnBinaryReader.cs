using System.Text;
using System.Net;

namespace Acn.IO
{
    public class AcnBinaryReader : BinaryReader
    {
        public AcnBinaryReader(Stream input)
            : base(input)
        { }

        public short ReadOctet2()
        {
            return IPAddress.NetworkToHostOrder(ReadInt16());
        }

        public int ReadOctet4()
        {
            return IPAddress.NetworkToHostOrder(ReadInt32());
        }

        public string ReadUtf8String(int size)
        {
            byte[] data = ReadBytes(size);
            return Encoding.UTF8.GetString(data).TrimEnd((char)0);
        }
    }
}
