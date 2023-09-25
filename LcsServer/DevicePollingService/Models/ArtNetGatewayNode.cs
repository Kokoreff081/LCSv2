using Acn.ArtNet.Packets;

namespace LcsServer.DevicePollingService.Models;

public class ArtNetGatewayNode : BaseDevice
    {
        private short _portCount;
        private string _shortName;
        private byte[] _swIn = new byte[4];
        private byte[] _swOut = new byte[4];
        private PollReplyPortTypes[] _portTypes = new PollReplyPortTypes[4];
        private byte _subNet;
        private byte _net;
        private byte[] _bindIpAddress = new byte[4];

        //public ArtNetGatewayNode() { }

        //Уникальное ID узла art-Net устройства присваивается как IpAddress:Port:ByteIndex (например: 192.168.76.240:6454:1)
        //Родителем является ArtNetGateway
        public ArtNetGatewayNode(byte[] ipAddress, short port, byte bindIndex)
            : base(string.Join(".", ipAddress) + $":{port}:{bindIndex}", string.Join(".", ipAddress) + $":{port}")
        {
            IpAddress = ipAddress;
            Port = port;
            BindIndex = bindIndex;
        }

        public byte[] IpAddress { get; }

        public short Port { get; }

        public byte BindIndex { get; }

        public short PortCount
        {
            get => _portCount;
            set => _portCount = value;
        }

        public string ShortName
        {
            get => _shortName;
            set => _shortName = value;
        }
        public string Name { get { return ShortName.Contains("\0") ? ShortName.Substring(0, ShortName.IndexOf('\0')) : ShortName; } }
        public string DisplayShortName
        {
            get
            {
                if (ShortName == null)
                    return null;

                return ShortName.Contains("\0") ? ShortName.Substring(0, ShortName.IndexOf('\0')) : ShortName;
            }
        }

        public PollReplyPortTypes[] PortTypes
        {
            get => _portTypes;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The port types must be an array of 4 bytes.");

                _portTypes = value;
            }
        }

        public byte[] SwIn
        {
            get => _swIn;
            set => _swIn = value;
        }

        public byte[] SwOut
        {
            get => _swOut;
            set => _swOut = value;
        }

        public byte SubNet
        {
            get => _subNet;
            set => _subNet = value;
        }

        public byte Net
        {
            get => _net;
            set => _net = value;
        }

        public byte[] BindIpAddress
        {
            get => _bindIpAddress;
            set
            {
                if (value.Length != 4)
                    throw new ArgumentException("The bind IP address must be an array of 4 bytes.");

                _bindIpAddress = value;
            }
        }
        public List<GatewayUniverse> children { get; set; }

        public bool IsInProject { get; set; }

        public override string Type => nameof(ArtNetGatewayNode);

        protected override void OnDeviceLost()
        {

        }

        /// Interprets the universe address to ensure compatibility with ArtNet I, II and III devices.
        /// <returns>The 15 Bit universe address</returns>
        public int GetPortAddress(int universe)
        {
            int portAddress = 0;

            if (Net > 0 || SubNet > 0)
            {
                portAddress = (Net & 0x7F00);
                portAddress += (Net & 0xFF) << 8;

                portAddress += (SubNet & 0x7F00);
                portAddress += (SubNet & 0x0F) << 4;

                portAddress += universe & 0xF;
            }
            else
            {
                portAddress = universe;
            }

            return portAddress;
        }

    }