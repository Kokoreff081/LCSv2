using System.Net;
using LcsServer.Models.LCProjectModels.GlobalBase;

namespace LcsServer.Models.LCProjectModels.Models.Addressing;

public class LCArtNetDevice : LCDevice, IEquatable<LCArtNetDevice>
{
    public IPAddress IpAddress { get; }

    public LCArtNetDevice(string ipAddress)
    {
        IpAddress = IPAddress.Parse(ipAddress);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((LCArtNetDevice) obj);
    }

    public override int GetHashCode()
    {
        return (IpAddress != null ? IpAddress.GetHashCode() : 0);
    }

    public override string ToString()
    {
        return string.Format($"SystemType: {SystemType.ArtNet}, IPAddress: {IpAddress}, Ports: {_universes.Count}");
    }

    public static bool operator ==(LCArtNetDevice device1, LCArtNetDevice device2)
    {
        return Comparator.EqualsOperator(device1, device2);
    }

    public static bool operator !=(LCArtNetDevice device1, LCArtNetDevice device2)
    {
        return !(device1 == device2);
    }

    public bool Equals(LCArtNetDevice other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Equals(IpAddress, other.IpAddress);
    }
}
public enum SystemType
{
    ArtNet,
    SAcn,
    Ilcs
}