using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using NLog;
using NModbus;

namespace LcsServer.Models.LCProjectModels.Managers;

public class ModbusMastersManager : IDisposable
{
    private readonly ModbusFactory _modbusFactory = new ModbusFactory();
    private readonly ConcurrentDictionary<IPEndPoint, (long interval, ushort[] registers)> _masters = new();
    private readonly ConcurrentDictionary<IPEndPoint, long> _failedSockets= new();

    public ushort[] ReadHoldingRegisters(IPEndPoint endPoint, byte unit, ushort startAddress, ushort numberOfPoints)
    {
        ushort[] result = Array.Empty<ushort>();

        long ms = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

        if (_failedSockets.TryGetValue(endPoint, out long interval))
        {
            if (ms - interval < 5000) // пять секунд не опрашиваем кривой порт
            {
                return result;
            }
        }

        if (_masters.TryGetValue(endPoint, out (long, ushort[]) result2) && ms - result2.Item1 < 100)
        {
            return result2.Item2;
        }

        try
        {
            using TcpClient tcpClient = new TcpClient();
            

            IAsyncResult res = tcpClient.BeginConnect( endPoint.Address, endPoint.Port, null, null );
 
            bool success = res.AsyncWaitHandle.WaitOne( 500, true );
 
            if ( !success )
            {
                _failedSockets[endPoint] = ms;
                throw new ApplicationException("Failed to connect server.");
            }

            _failedSockets.TryRemove(endPoint, out _);
            IModbusMaster modbusMaster = _modbusFactory.CreateMaster(tcpClient);
            result = modbusMaster.ReadHoldingRegisters(unit, startAddress, numberOfPoints);
            _masters[endPoint] = (ms,result);
        }
        catch (Exception e)
        {
            LogManager.GetCurrentClassLogger().Warn(e.Message);
        }


        return result;
    }

    private void ReleaseUnmanagedResources()
    {
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~ModbusMastersManager()
    {
        ReleaseUnmanagedResources();
    }
}