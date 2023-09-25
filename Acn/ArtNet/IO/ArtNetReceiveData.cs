namespace Acn.ArtNet.IO
{
    public class ArtNetReceiveData
    {
        public byte[] buffer = new byte[1500];
        public int bufferSize = 1500;
        public int DataLength = 0;

        public bool Valid
        {
            get { return DataLength > 12; }
        }

        public ushort OpCode
        {
            get
            {
                return (ushort)(buffer[9] + (buffer[8] << 8));
            }
        }
	
    }
}
