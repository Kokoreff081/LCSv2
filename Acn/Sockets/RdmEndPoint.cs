using Acn.Rdm;
using System.Net;

namespace Acn.Sockets
{
    public class RdmEndPoint : IPEndPoint
    {
        public RdmEndPoint(IPAddress ipAddress)
            : this(ipAddress, 0, 0)
        {
        }

        public RdmEndPoint(IPAddress ipAddress, int universe)
            : this(ipAddress, 0, universe)
        {
        }

        public RdmEndPoint(IPEndPoint ipEndPoint)
            : this(ipEndPoint.Address, ipEndPoint.Port, 0)
        {
        }

        public RdmEndPoint(IPEndPoint ipEndPoint, int universe)
            : this(ipEndPoint.Address, ipEndPoint.Port, universe)
        {
        }

        public RdmEndPoint(IPAddress ipAddress, int port, int universe)
            : this(ipAddress, port, universe, 0)
        {
            IpAddress = ipAddress;
            Universe = universe;
        }

        public RdmEndPoint(IPAddress ipAddress, int port, int universe, byte net) : base(ipAddress, port)
        {
            IpAddress = ipAddress;
            Universe = universe;
            Net = net;
        }

        public UId Id { get; set; } = UId.Empty;

        public IPAddress IpAddress { get; }

        public int Universe { get; }

        public byte Net { get; }

        public override string ToString()
        {
            return IpAddress.ToString();
        }

        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = (hashCode * 397) ^ Universe;
            hashCode = (hashCode * 397) ^ Net;
            return hashCode;
        }
    }

    public class RdmEndpointComparer : IEqualityComparer<RdmEndPoint>
    {

        public bool Equals(RdmEndPoint x, RdmEndPoint y)
        {
            return y != null && x != null && x.Id.Equals(y.Id) && x.Universe.Equals(y.Universe);
        }

        public int GetHashCode(RdmEndPoint obj)
        {
            return obj.GetHashCode();
        }
    }
}
