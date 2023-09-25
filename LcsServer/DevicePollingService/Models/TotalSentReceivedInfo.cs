namespace LcsServer.DevicePollingService.Models;

public class TotalSentReceivedInfo
{
    public int PacketsSent { get; set; }

    public int PacketsReceived { get; set; }

    public int PacketsDropped { get; set; }

    public int TransactionsStarted { get; set; }

    public int TransactionsFailed { get; set; }

    public int TransactionsReceived { get; set; }
}