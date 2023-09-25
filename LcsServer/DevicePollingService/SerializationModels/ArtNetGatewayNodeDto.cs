using Acn.ArtNet.Packets;
using LcsServer.DevicePollingService.Models;

namespace LcsServer.DevicePollingService.SerializationModels;

public class ArtNetGatewayNodeDto : BaseObjectDto
{

    // For json deserialization
    public ArtNetGatewayNodeDto() { }
        
    public ArtNetGatewayNodeDto(ArtNetGatewayNode model)
    {
        IpAddress = model.IpAddress;
        Port = model.Port;
        BindIndex = model.BindIndex;
        PortCount = model.PortCount;
        ShortName = model.ShortName;
        PortTypes = model.PortTypes;
        SwIn = model.SwIn;
        SwOut = model.SwOut;
        SubNet = model.SubNet;
        Net = model.Net;
        BindIpAddress = model.BindIpAddress;
    }
        
    public override BaseObject ToBaseObject()
    {
        ArtNetGatewayNode artNetGatewayNode = new ArtNetGatewayNode(IpAddress, Port, BindIndex)
        {
            PortCount = PortCount,
            ShortName = ShortName,
            PortTypes = PortTypes,
            SwIn = SwIn,
            SwOut = SwOut,
            SubNet = SubNet,
            Net = Net,
            BindIpAddress = BindIpAddress,
        };

        return artNetGatewayNode;
    }

    public override string Type => nameof(ArtNetGatewayNode);

    public byte[] IpAddress { get; set; }

    public short Port { get; set; }

    public byte BindIndex { get; set; }

    public short PortCount { get; set; }

    public string ShortName { get; set; }

    public PollReplyPortTypes[] PortTypes { get; set; }

    public byte[] SwIn { get; set; }

    public byte[] SwOut { get; set; }

    public byte SubNet { get; set; }

    public byte Net { get; set; }

    public byte[] BindIpAddress { get; set; }
}