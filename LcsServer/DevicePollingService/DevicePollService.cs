using System.Collections.Immutable;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Acn.ArtNet;
using Acn.ArtNet.Packets;
using Acn.ArtNet.Sockets;
using Acn.Rdm;
using Acn.Rdm.Packets.Net;
using Acn.Rdm.Routing;
using Acn.Sockets;
using LcsServer.CommandLayer;
using LcsServer.DatabaseLayer;
using LcsServer.DevicePollingService.Enums;
using LcsServer.DevicePollingService.Interfaces;
using LcsServer.DevicePollingService.Models;
using LightControlServiceV._2.DevicePollingService.Models;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.EntityFrameworkCore;
using DeviceFoundEventArgs = LcsServer.DevicePollingService.Models.DeviceFoundEventArgs;

namespace LcsServer.DevicePollingService;

public class DevicePollService : IDisposable
{
    private ArtNetSocket _artNetSocket;
    private IPAddress _localAdapter;
    private RdmReliableSocket _rdmSocket;
    private DiscoveryType _discoveryType;
    private RdmDeviceBroker _rdmDeviceBroker;
    private readonly IStorageManager _storageManager;
    private readonly ISettingsService _settingsService;
    private DatabaseContext _db;
    private IConfiguration Configuration;
    private Timer? _timer = null;
    private Timer? _commandTimer = null;
    private object locker = new object();
    private CancellationToken token;
    private readonly IBackgroundTaskQueue _taskQueue;
    private bool _isWorking = false;
    private bool _isCommandStopped = false;
    private IServiceProvider _serviceProvider;
    public bool IsWorking
    {
        get { return _isWorking;}
        set { _isWorking = value; }
    }

    public bool IsCommandStopped
    {
        get { return _isCommandStopped; }
        set { _isCommandStopped = value; }
    }
    public DevicePollService(IStorageManager storageManager, IConfiguration configuration, IBackgroundTaskQueue taskQueue, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        token = new CancellationToken();
        //_db = databaseOperations;
        //_db.SavedChanges += DbOnSavedChanges;
        //_db.ChangeTracker.DetectingEntityChanges
        var serManager = new JsonSerializationManagerRDM();
        var statChecker = new StatusChecker();
        _storageManager = storageManager;
        _settingsService = new SettingsService(serManager, _serviceProvider);
        _taskQueue = taskQueue;
        _taskQueue.NewCommandAdded += ProcessTaskQueueAsync;
        Configuration = configuration;

    }

    public IPAddress SubnetMask { get; set; }
    public IPAddress LocalAdapter
    {
        get => _localAdapter;
        set
        {
            _localAdapter = value;
            ReleaseSockets();
            InitSocket();
        }
    }
    public event Action<TotalSentReceivedInfo> TotalSentReceivedInfo;
    public event Action AllTransactionsCompleted;

   private async void DoWork()
    {
        if(_storageManager.Devices.Count == 0)
            await DiscoveryAll(DiscoveryType.DeviceDiscovery);//poll all devices
        else
            await DiscoveryAll(DiscoveryType.GatewayDiscovery);//poll only sensors
    }

    private async void RunCommand(Command cmd)
    {
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            if (cmd.CommandType == 4)
            {
                bool flag = bool.Parse(cmd.ParamNewValue);
                try
                {
                    string msg = "";
                    if (!flag)
                    {
                        Stop();
                        IsCommandStopped = true;
                        msg = $"Device polling was successfully stopped by user {cmd.UserLogin}";
                    }
                    else
                    {
                        Start();
                        IsCommandStopped = false;
                        msg = $"Device polling was successfully started by user {cmd.UserLogin}";
                    }
                    _db.Events.Add(new Event()
                    {
                        level = "Info", Description = msg, dateTime = DateTime.Now
                    });
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _db.Events.Add(new Event()
                    {
                        level = "Error", Description = ex.Message, dateTime = DateTime.Now
                    });
                    await _db.SaveChangesAsync();
                }

            }
            else
            {
                var device = _storageManager.GetDeviceById(cmd.DeviceId) as RdmDevice;
                if (device != null)
                {
                    switch (cmd.CommandType)
                    {
                        case 0:
                            try
                            {
                                string msg =
                                    $"Dmx address of device {device.DisplayName} was successfully changed from {device.DmxAddress} to {cmd.ParamNewValue} by user {cmd.UserLogin}";
                                device.ChangeDmxAddress(int.Parse(cmd.ParamNewValue));
                                cmd.State = 1;
                                _db.Events.Add(new Event()
                                {
                                    level = "Info", Description = msg, dateTime = DateTime.Now
                                });
                                await _db.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                _db.Events.Add(new Event()
                                {
                                    level = "Error", Description = ex.Message, dateTime = DateTime.Now
                                });
                                await _db.SaveChangesAsync();
                            }
                            break;
                        case 1:
                            try
                            {
                                string msg =
                                    $"Label of device {device.DisplayName} was successfully changed from {device.Label} to {cmd.ParamNewValue} by user {cmd.UserLogin}";
                                device.ChangeLabelRequest(cmd.ParamNewValue);
                                var logEntry = new Event()
                                {
                                    level = "Info", Description = msg, dateTime = DateTime.Now
                                };
                                
                                    _db.Events.Add(logEntry);
                                    await _db.SaveChangesAsync();
                                
                            }
                            catch (Exception ex)
                            {
                                _db.Events.Add(new Event()
                                {
                                    level = "Error", Description = ex.Message, dateTime = DateTime.Now
                                });
                                await _db.SaveChangesAsync();
                            }
                            break;
                        case 2:
                            try
                            {
                                string msg = "";
                                var flag = bool.Parse(cmd.ParamNewValue);
                                if(flag)
                                    msg = $"Identify of device {device.DisplayName} was successfully switch on by user {cmd.UserLogin}";
                                else
                                    msg = $"Identify of device {device.DisplayName} was successfully switch off by user {cmd.UserLogin}";
                                device.IdentifyOnOff(flag);
                                _db.Events.Add(new Event()
                                {
                                    level = "Info", Description = msg, dateTime = DateTime.Now
                                });
                                await _db.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                _db.Events.Add(new Event()
                                {
                                    level = "Error", Description = ex.Message, dateTime = DateTime.Now
                                });
                                await _db.SaveChangesAsync();
                            }
                            break;
                        case 3:
                            try
                            {
                                string msg = "";
                                var rdmParams = device.Parameters;
                                if (rdmParams != null)
                                {
                                    byte[] byteValue;
                                    var changingParam = rdmParams.Where(w => w.ParameterId.ToString() == cmd.ParamId.ToString()).FirstOrDefault();
                                    switch (changingParam.DataType)
                                    {
                                        case ParameterInformation.ParameterDefinition.SDWord:
                                        case ParameterInformation.ParameterDefinition.SWord:
                                        case ParameterInformation.ParameterDefinition.UDWord:
                                        case ParameterInformation.ParameterDefinition.UWord:
                                            byteValue = new[] { (byte)int.Parse(cmd.ParamNewValue, System.Globalization.NumberStyles.HexNumber) };
                                            break;
                                        case ParameterInformation.ParameterDefinition.ASCII:
                                            byteValue = Encoding.ASCII.GetBytes(cmd.ParamNewValue);
                                            break;
                                        default:
                                            if (int.TryParse(cmd.ParamNewValue, out int intValue))
                                                byteValue = new[] { (byte)intValue };
                                            else
                                                throw new Exception("Data type is not valid");

                                            break;
                                    }
                                    msg = $"Param {changingParam.Description} of device {device.DisplayName} was successfully changed from {changingParam.Value.ToString()} to {cmd.ParamNewValue} by user {cmd.UserLogin}";
                                    await Task.Run(() =>changingParam.ChangeValueRequest(byteValue));
                                    _db.Events.Add(new Event()
                                    {
                                        level = "Info", Description = msg, dateTime = DateTime.Now
                                    });
                                    await _db.SaveChangesAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                _db.Events.Add(new Event()
                                {
                                    level = "Error", Description = ex.Message, dateTime = DateTime.Now
                                });
                                await _db.SaveChangesAsync();
                            }
                            break;
                    }
                }
            }
        }
    }
    private async void ProcessTaskQueueAsync()
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                Command workItem =
                    await _taskQueue.DequeueAsync();

                RunCommand(workItem);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error occurred executing task work item.");
                /*using (var db = _db.CreateDbContext(null))
                {*/
                _db.Events.Add(new Event() { level = "Error", dateTime = DateTime.Now, Description = ex.Message });
                    await _db.SaveChangesAsync();
                //}
            }
        }
    }

    public async void Start()
    {
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var scanning = _db.Settings.First(f => f.Name == "DeviceScanning");
            scanning.IsEnabled = true;
            _db.Update(scanning);
            await _db.SaveChangesAsync();
        }

        IsWorking = true;
        DoWork();
    }

    public async void Stop()
    {
       
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var scanning = _db.Settings.First(f => f.Name == "DeviceScanning");
            scanning.IsEnabled = false;
            _db.Update(scanning);
            await _db.SaveChangesAsync();
        }

        IsWorking = false;
        ReleaseSockets();
    }
    public void Dispose()
    {
        _timer?.Dispose();
    }
    
    public static IPAddress GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip;
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
    public async Task DiscoveryAll(DiscoveryType type, IPEndPoint targetController = null) 
    {
        _discoveryType = type;
        LocalAdapter = IPAddress.Parse(Configuration.GetValue<string>("DefaultIpForDiscovery"));//GetLocalIPAddress();
        InitSocket();
        SendArtPoll(targetController);
    }

    private void InitSocket()
    {
        if (_artNetSocket == null || !_artNetSocket.PortOpen)
        {
            _artNetSocket = new ArtNetSocket(UId.NewUId(1206, 1, 0x0FFF)); // 1206 == IntiLED, 4096 первых адресов никогда не будут светильниками

            _artNetSocket.NewPacket += NewPacketReceived;
            _artNetSocket.NewRdmPacket += OnSentReceiveNewRdmPacket;
            _artNetSocket.RdmPacketSent += OnSentReceiveNewRdmPacket;

            _artNetSocket.Open(LocalAdapter, SubnetMask);

            _rdmSocket = new RdmReliableSocket(_artNetSocket);
            _rdmSocket.PropertyChanged += RdmReliableSocket_PropertyChanged;
            _rdmSocket.AllTransactionsCompleted += RdmReliableSocketNew_AllTransactionsCompleted;
        }

        var paramSettings = _settingsService.GetSettings(SettingsTypes.Parameters) as ParametersSettings;

        _rdmDeviceBroker = new RdmDeviceBroker(_rdmSocket, _storageManager, paramSettings);
        _rdmDeviceBroker.NewRdmDeviceFound += OnNewRdmDeviceFound;
    }
    private void ReleaseSockets()
    {
        if (_rdmSocket != null)
        {
            _rdmSocket.PropertyChanged -= RdmReliableSocket_PropertyChanged;
            _rdmSocket.AllTransactionsCompleted -= RdmReliableSocketNew_AllTransactionsCompleted;
            _rdmSocket.Dispose();
            _rdmSocket = null;
        }

        if (_artNetSocket != null)
        {
            _artNetSocket.NewPacket -= NewPacketReceived;
            _artNetSocket.NewRdmPacket -= OnSentReceiveNewRdmPacket;
            _artNetSocket.RdmPacketSent -= OnSentReceiveNewRdmPacket;

            _artNetSocket?.Close();
            _artNetSocket?.Dispose();
            _artNetSocket = null;
        }

        //_rdmDeviceBroker?.Unsubscribe();
    }

    private void SendArtPoll(IPEndPoint targetController = null)
    {
        ArtPollPacket packet = new ArtPollPacket();
        packet.TalkToMe = 6;

        if (targetController == null)
            _artNetSocket.Send(packet);
        else
            _artNetSocket.Send(packet, targetController);
    }

    #region eventHandlers
    private void NewPacketReceived(object sender, NewPacketEventArgs<ArtNetPacket> e)
    {
        switch (e.Packet.OpCode)
        {
            case ArtNetOpCodes.PollReply:
                 ProcessPollReply((ArtPollReplyPacket)e.Packet, e.Source);
                break;
            case ArtNetOpCodes.TodData:
                ProcessTodData((ArtTodDataPacket)e.Packet, e.Source);
                break;
            case ArtNetOpCodes.IpProgReply:
                break;
            case ArtNetOpCodes.Address:
                break;
        }
    }

    private async void ProcessPollReply(ArtPollReplyPacket packet, IPEndPoint endPoint)
    {
        //Does device support RDM?
        if ((packet.Status /*& PollReplyStatus.RdmCapable*/) > 0)
        {

            List<BaseDevice> devices = new List<BaseDevice>();
            List<byte> outPortAddresses = new List<byte>();
            var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
                var list = _db.Devices.ToList();
                var idArtnet = string.Join(".", endPoint.Address.GetAddressBytes()) + $":{packet.Port}";
                if (!list.Any(a => a.deviceId == idArtnet))
                {
                    var newDevice = new Device()
                    {
                        deviceId = idArtnet, Type = "ArtNetGateway", StatusId = (int)packet.Status, ParentId = ""
                    };
                    _db.Devices.Add(newDevice);
                    await _db.SaveChangesAsync();
                }


                ArtNetGateway artNetGateway = new ArtNetGateway(endPoint.Address.GetAddressBytes(), packet.Port, _serviceProvider);
                devices.Add(artNetGateway);

                artNetGateway.EstaCode = packet.EstaCode;
                artNetGateway.FirmwareVersion = packet.FirmwareVersion;
                artNetGateway.GoodInput = packet.GoodInput;
                artNetGateway.GoodOutput = packet.GoodOutput;
                artNetGateway.LongName = packet.LongName;
                artNetGateway.MacAddress = packet.MacAddress;
                artNetGateway.NodeReport = packet.NodeReport;
                artNetGateway.Oem = packet.Oem;
                artNetGateway.Status = packet.Status;
                artNetGateway.Status2 = packet.Status2;
                artNetGateway.Style = packet.Style;
                artNetGateway.SwMacro = packet.SwMacro;
                artNetGateway.SwRemote = packet.SwRemote;
                artNetGateway.SwVideo = packet.SwVideo;
                artNetGateway.UbeaVersion = packet.UbeaVersion;
                artNetGateway.Manufacturer = Acn.Helpers.ESTACodes.GetManufacturerByCode(packet.EstaCode);
                ArtNetGatewayNode artNetGatewayNode =
                    new ArtNetGatewayNode(endPoint.Address.GetAddressBytes(), packet.Port, packet.BindIndex)
                    {
                        PortCount = packet.PortCount,
                        PortTypes = packet.PortTypes,
                        ShortName = packet.ShortName,
                        SwIn = packet.SwIn,
                        SwOut = packet.SwOut,
                        BindIpAddress = packet.BindIpAddress,
                        Net = packet.Net,
                        SubNet = packet.SubNet,
                    };
                devices.Add(artNetGatewayNode);

                // This array defines the low byte of the Port-Address of
                // the Output Gateway nodes that must respond to this
                // packet.The high nibble is the Sub - Net switch.The
                // low nibble corresponds to the Universe. This is
                // combined with the 'Net' field above to form the 15
                // bit address.
                for (int n = 0; n < 4; n++)
                {

                    if (packet.PortTypes[n].HasFlag(PollReplyPortTypes.IsOutputPort))
                    {
                        byte swOut = packet.SwOut[n];

                        outPortAddresses.Add(swOut);

                        byte universe = (byte)(swOut & 0xF);

                        var dbDevice = new Device()
                        {
                            deviceId = $"{artNetGatewayNode.Id}:{false}:{n + 1}",
                            Type = "GatewayOutputUniverse",
                            StatusId = (int)(OutputStatuses)packet.GoodOutput[n],
                            ParentId = artNetGatewayNode.Id
                        };
                        /*using (var db = _db.CreateDbContext(null))
                        {*/
                        if (!_db.Devices.Any(a => a.deviceId == dbDevice.deviceId))
                        {
                            _db.Devices.Add(dbDevice);
                            await _db.SaveChangesAsync();
                        }
                        //}

                        GatewayUniverse gatewayUniverse = new GatewayOutputUniverse(artNetGatewayNode.Id,
                            endPoint.Address, n + 1, artNetGatewayNode.GetPortAddress(universe), universe,
                            (PortTypes)packet.PortTypes[n], (OutputStatuses)packet.GoodOutput[n], _serviceProvider);

                        devices.Add(gatewayUniverse);
                    }

                    if (packet.PortTypes[n].HasFlag(PollReplyPortTypes.IsInputPort))
                    {

                        byte swIn = packet.SwIn[n];

                        byte universe = (byte)(swIn & 0xF);
                        var dbDevice = new Device()
                        {
                            deviceId = $"{artNetGatewayNode.Id}:{true}:{n + 1}",
                            Type = "GatewayInputUniverse",
                            StatusId = (int)(InputStatuses)packet.GoodInput[n],
                            ParentId = artNetGatewayNode.Id
                        };
                        /*using (var db = _db.CreateDbContext(null))
                        {*/
                        if (!_db.Devices.Any(a => a.deviceId == dbDevice.deviceId))
                        {
                            _db.Devices.Add(dbDevice);
                            await _db.SaveChangesAsync();
                        }
                        //}

                        GatewayUniverse gatewayUniverse = new GatewayInputUniverse(artNetGatewayNode.Id,
                            endPoint.Address, n + 1, artNetGatewayNode.GetPortAddress(universe), universe,
                            (PortTypes)packet.PortTypes[n], (InputStatuses)packet.GoodInput[n], _serviceProvider);

                        devices.Add(gatewayUniverse);
                    }
                }
            }

            //DevicesController.Devices = devices;
            Debug.WriteLine("ProcessPollReply");

            _storageManager.AddDevices(devices.ToArray());
            SendTodRequest(endPoint, packet.Net, outPortAddresses);/**/
           // 
            //Request TOD for each input universe.
           /* if (_discoveryType == DiscoveryType.DeviceDiscovery)
                SendFlushTodControl(endPoint, packet.Net, outPortAddresses);
           // else
                //TODO разобраться!!!*/
            
           // _storageManager.SaveDevices(_storageManager.Devices);
        }
    }
    private void SendFlushTodControl(IPEndPoint address, byte net, List<byte> subNetUniverses)
    {
        foreach (var subNetUniverse in subNetUniverses)
        {
            ArtTodControlPacket packet = new ArtTodControlPacket();
            packet.Address = subNetUniverse;
            packet.Net = net;
            packet.Command = ArtTodControlCommand.AtcFlush;
            _artNetSocket.Send(packet, new RdmEndPoint(address.Address, address.Port, subNetUniverse, net));
        }
    }
    private void SendTodRequest(IPEndPoint address, byte net, List<byte> universes)
    {
        ArtTodRequestPacket packet = new ArtTodRequestPacket
        {
            RequestedUniverses = universes,
            Net = net
        };

        _artNetSocket.Send(packet, new RdmEndPoint(address.Address, address.Port, 0, net));
    }
    private async void OnNewRdmDeviceFound(Models.DeviceFoundEventArgs deviceFoundArgs)
    {
        RdmEndPoint deviceAddress = new RdmEndPoint(deviceFoundArgs.Address, 0, deviceFoundArgs.Universe, deviceFoundArgs.Net);

        string parentId = $"{deviceFoundArgs.Address}:{ArtNetSocket.Port}:{deviceFoundArgs.BindIndex}:{false}:{deviceFoundArgs.DevicePort}";
        var scopeFactory = _serviceProvider.GetService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            _db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            RdmDevice rdmDevice = new RdmDevice(_rdmDeviceBroker, deviceFoundArgs.Id, deviceAddress,
                deviceFoundArgs.DevicePort, parentId, _serviceProvider);

            _storageManager.AddDevices(new[] { rdmDevice });

            _rdmDeviceBroker.RequestParameters(rdmDevice.Address, rdmDevice.UId);
            await Task.Delay(1000);
            RdmDevice currentDevice = _storageManager.GetDeviceById(rdmDevice.Id) as RdmDevice;
            if (currentDevice == null)
                return;

            if (currentDevice.SensorCount != null && currentDevice.SensorCount > 0)
                _rdmDeviceBroker.RequestSensors(currentDevice.Address, currentDevice.UId, (int)currentDevice.SensorCount);

            // Request deviceInfo
            _rdmDeviceBroker.RequestDeviceInfo(rdmDevice.Address, rdmDevice.UId);
            await Task.Delay(1000);
            //Only for RDMNet Endpoint Zero devices.
            if (rdmDevice.Address.Universe == 0)
            {
                //Get a list of endpoints supported by this device.
                EndpointList.Get getPorts = new EndpointList.Get();
                _rdmDeviceBroker.SendRdm(getPorts, rdmDevice.Address, rdmDevice.UId);
            }
        }
    }

    private void ProcessTodData(ArtTodDataPacket packet, IPEndPoint endPoint)
    {
        Debug.WriteLine($"ProcessTodData: bindIndex ={packet.BindIndex}. Port={packet.Port}. Net={packet.Net}. Universe={packet.Universe}. Devices count = {packet.Devices.Count} ");
        foreach (var deviceId in packet.Devices)
        {
            DeviceFoundEventArgs args = new DeviceFoundEventArgs()
            {
                Id = deviceId,
                Address = endPoint.Address,
                DevicePort = packet.Port,
                BindIndex = packet.BindIndex,
                Universe = packet.Universe,
                Net = packet.Net,
            };
            OnNewRdmDeviceFound(args);
        }
    }
    private void OnSentReceiveNewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
    {
        /*LogManager.GetInstance().WriteResponseMessage(new ResponseMessage()
        {
            Address = e.Source.Address.ToString(),
            Command = e.Packet.Header.Command.ToString(),
            DateTime = DateTime.Now,
            DestinationId = e.Packet.Header.DestinationId.ToString(),
            MessageCount = e.Packet.Header.MessageCount,
            ParameterId = e.Packet.Header.ParameterId.ToString(),
            ResponseType = ((RdmResponseTypes)e.Packet.Header.PortOrResponseType).ToString(),
            SourceId = e.Packet.Header.SourceId.ToString(),
            SubDevice = e.Packet.Header.SubDevice,
            TransactionNumber = e.Packet.Header.TransactionNumber
        });*/
    }

    private void RdmReliableSocket_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is RdmReliableSocket rdmReliableSocket)
        {
            var info = new TotalSentReceivedInfo
            {
                PacketsDropped = rdmReliableSocket.PacketsDropped,
                PacketsReceived = rdmReliableSocket.PacketsReceived,
                PacketsSent = rdmReliableSocket.PacketsSent,
                TransactionsFailed = rdmReliableSocket.TransactionsFailed,
                TransactionsStarted = rdmReliableSocket.TransactionsStarted,
                TransactionsReceived = rdmReliableSocket.TransactionsReceived,
            };
            TotalSentReceivedInfo?.Invoke(info);
        }
    }

    private void RdmReliableSocketNew_AllTransactionsCompleted()
    {
        AllTransactionsCompleted?.Invoke();
    }
    #endregion
}