using Acn.ArtNet.Packets;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.SerializationModels;

public class ArtNetGatewayDto :BaseObjectDto
    {
        // For json deserialization
        public ArtNetGatewayDto() { }

        public ArtNetGatewayDto(ArtNetGateway artNetGateway)
        {
            IpAddress = artNetGateway.IpAddress;
            Port = artNetGateway.Port;
            FirmwareVersion = artNetGateway.FirmwareVersion;
            Oem = artNetGateway.Oem;
            UbeaVersion = artNetGateway.UbeaVersion;
            Status = artNetGateway.Status;
            EstaCode = artNetGateway.EstaCode;
            LongName = artNetGateway.LongName;
            NodeReport = artNetGateway.NodeReport;
            GoodInput = artNetGateway.GoodInput;
            GoodOutput = artNetGateway.GoodOutput;
            SwVideo = artNetGateway.SwVideo;
            SwMacro = artNetGateway.SwMacro;
            SwRemote = artNetGateway.SwRemote;
            Style = artNetGateway.Style;
            MacAddress = artNetGateway.MacAddress;
            Status2 = artNetGateway.Status2;
        }
        
        public override BaseObject ToBaseObject()
        {
            ArtNetGateway artNetGateway = new ArtNetGateway(IpAddress, Port)
            {
                FirmwareVersion = FirmwareVersion,
                Oem = Oem,
                UbeaVersion = UbeaVersion,
                Status = Status,
                EstaCode = EstaCode,
                LongName = LongName,
                NodeReport = NodeReport,
                GoodInput = GoodInput,
                GoodOutput = GoodOutput,
                SwVideo = SwVideo,
                SwMacro = SwMacro,
                SwRemote = SwRemote,
                Style = Style,
                MacAddress = MacAddress,
                Status2 = Status2,
            };

            return artNetGateway;
        }

        public byte[] IpAddress { get; set; }

        public short Port { get; set; }

        public short FirmwareVersion { get; set; }

        public byte Net { get; set; }

        public byte SubNet { get; set; }

        public short Oem { get; set; }

        public byte UbeaVersion { get; set; }

        public PollReplyStatus Status { get; set; }

        public short EstaCode { get; set; }

        public string LongName { get; set; }

        public string NodeReport { get; set; }

        public byte[] GoodInput { get; set; }

        public byte[] GoodOutput { get; set; }

        public byte SwVideo { get; set; }

        public byte SwMacro { get; set; }

        public byte SwRemote { get; set; }

        public byte Style { get; set; }

        public byte[] MacAddress { get; set; }

        public byte[] BindIpAddress { get; set; }

        public PollReplyStatus2 Status2 { get; set; }

        public override string Type => nameof(ArtNetGateway);
    }