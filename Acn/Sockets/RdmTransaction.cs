using Acn.Rdm;

namespace Acn.Sockets;

internal class RdmTransaction : IEquatable<RdmTransaction>
{
    public RdmTransaction(byte transactionNumber, RdmPacket packet, RdmEndPoint address, UId targetId)
    {
        TransactionNumber = transactionNumber;
        Packet = packet;
        TargetAddress = address;
        TargetId = targetId;
        Attempts = 0;
        LastAttempt = DateTime.MinValue;
    }

    public readonly byte TransactionNumber;
    public readonly RdmPacket Packet;
    public readonly RdmEndPoint TargetAddress;
    public readonly UId TargetId;

    public int Attempts;
    public DateTime LastAttempt;

    public bool Completed;

    public bool Equals(RdmTransaction other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return TransactionNumber == other.TransactionNumber && Equals(TargetId, other.TargetId);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((RdmTransaction)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = TransactionNumber.GetHashCode();
            hashCode = (hashCode * 397) ^ (TargetId != null ? TargetId.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int)Packet.Header.ParameterId;
            return hashCode;
        }
    }
}