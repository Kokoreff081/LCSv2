using System.Collections;
using Acn.ArtNet.IO;

namespace Acn.ArtNet.Packets
{
    public class ArtIpProgPacket : ArtNetPacket
    {
        public ArtIpProgPacket() : base(ArtNetOpCodes.IpProg)
        {
        }

        public ArtIpProgPacket(ArtNetReceiveData data) : base(data)
        {
        }

        #region Packet Properties

        /// <summary>
        /// Action this packet as follows:
        /// - Defines the how this packet is processed.If all bits
        ///     are clear, this is an enquiry only.
        /// 7 Set to enable any programming.
        /// 6 Set to enable DHCP (if set ignore lower bits).
        /// 5-4 Not used, transmit as zero
        /// 3 Set to return all three parameters to default
        /// 2 Program IP Address
        /// 1 Program Subnet Mask
        /// 0 Program Port
        /// </summary>
        public byte Command { get; private set; }
        
        public byte[] IPAddress { get; set; }

        public byte[] SubnetMask { get; set; }

        public short Port { get; set; }

        public void EnableAnyProgramming()
        {
            BitArray bit = new BitArray(new bool[]
            {
                false, false, false, false,
                false, false, false, true
            });

            byte[] bytes = new byte[1];
            bit.CopyTo(bytes, 0);

            Command = bytes[0];
        }

        public void EnableProgramIpAddress()
        {
            BitArray bit = new BitArray(new bool[]
            {
                false, false, true, false,
                false, false, false, true
            });

            byte[] bytes = new byte[1];
            bit.CopyTo(bytes, 0);

            Command = bytes[0];
        }

        public void EnableProgramSubnetMask()
        {
            BitArray bit = new BitArray(new bool[]
            {
                false, true, false, false,
                false, false, false, false
            });

            byte[] bytes = new byte[1];
            bit.CopyTo(bytes, 0);

            Command = bytes[0];
        }

        public void EnableProgramPort()
        {
            BitArray bit = new BitArray(new bool[]
            {
                true, false, false, false,
                false, false, false, false
            });

            byte[] bytes = new byte[1];
            bit.CopyTo(bytes, 0);

            Command = bytes[0];
        }

        #endregion

        protected override void WriteData(ArtNetBinaryWriter data)
        {
            base.WriteData(data);

            data.Write(new byte[2]); // Filler1, Filler2 - Pad length to match ArtPoll.
            data.Write(Command);
            data.Write(new byte());// Filler4 - Set to zero. Pads data structure for word alignment.
            
            foreach (byte ip in IPAddress) 
                data.Write(ip);

            foreach (byte sm in SubnetMask) 
                data.Write(sm);

            data.WriteNetwork(Port);
            
            data.Write(new byte[8]);// Spare1-8. Transmit as zero, receivers don’t test.
        }

        protected override void ReadData(ArtNetBinaryReader data)
        {
            base.ReadData(data);

            data.ReadBytes(2); 

            Command = data.ReadByte();
        }
    }
}
