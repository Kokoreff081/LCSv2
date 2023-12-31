using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using Acn.Rdm;
using System.IO;
using System.Threading.Tasks;
using Acn.Packets.sAcn;
using Acn.Sockets;
using Acn.ArtNet.Packets;
using Acn.ArtNet.IO;

namespace Acn.ArtNet.Sockets
{
    public class ArtNetSocket : Socket, IRdmSocket
    {
        public const int Port = 6454;


        private IAsyncResult _receiveResult;

        public event UnhandledExceptionEventHandler UnhandledException;
        public event EventHandler<NewPacketEventArgs<ArtNetPacket>> NewPacket;
        public event EventHandler<NewPacketEventArgs<RdmPacket>> NewRdmPacket;
        public event EventHandler<NewPacketEventArgs<RdmPacket>> RdmPacketSent;

        public ArtNetSocket(UId rdmId)
            : base(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp)
        {
            RdmId = rdmId;
        }

        public ArtNetSocket()
            : this(UId.Empty)
        {
        }
        #region Information

        /// <summary>
        /// Gets or sets the RDM Id to use when sending packets.
        /// </summary>
        public UId RdmId { get; protected set; }

        private bool portOpen = false;

        public bool PortOpen
        {
            get { return portOpen; }
            set { portOpen = value; }
        }

        public IPAddress LocalIP { get; protected set; }

        public IPAddress LocalSubnetMask { get; protected set; }

        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public IPAddress BroadcastAddress
        {
            get
            {
                if (LocalSubnetMask == null)
                    return IPAddress.Broadcast;
                return GetBroadcastAddress(LocalIP, LocalSubnetMask);
            }
        }

        private DateTime? lastPacket = null;

        public DateTime? LastPacket
        {
            get { return lastPacket; }
            protected set { lastPacket = value; }
        }

        #endregion




        public void Open(IPAddress localIp, IPAddress localSubnetMask)
        {
            LocalIP = localIp;
            LocalSubnetMask = localSubnetMask;

            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(new IPEndPoint(LocalIP, Port));
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            PortOpen = true;

            StartReceive();
        }

        private void StartReceive()
        {
            try
            {
                EndPoint localPort = new IPEndPoint(IPAddress.Any, Port);
                ArtNetReceiveData receiveState = new ArtNetReceiveData();
                _receiveResult = BeginReceiveFrom(receiveState.buffer, 0, receiveState.bufferSize, SocketFlags.None, ref localPort, new AsyncCallback(OnReceive), receiveState);
            }
            catch (Exception ex)
            {
                OnUnhandledException(new ApplicationException("An error ocurred while trying to start recieving ArtNet.", ex));
            }
        }

        public void StopReceive()
        {
            if (_receiveResult == null)
                return;

            EndPoint localPort = new IPEndPoint(IPAddress.Any, Port);

            Task.Factory.StartNew(() =>
            {
                EndReceiveFrom(_receiveResult, ref localPort);
            });
        }

        private void OnReceive(IAsyncResult state)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            if (PortOpen)
            {
                try
                {
                    ArtNetReceiveData receiveState = (ArtNetReceiveData)(state.AsyncState);

                    if (receiveState != null)
                    {
                        receiveState.DataLength = EndReceiveFrom(state, ref remoteEndPoint);

                        //Protect against UDP loopback where we receive our own packets.
                        if (!LocalEndPoint.Equals(remoteEndPoint) && receiveState.Valid)
                        {
                            LastPacket = DateTime.Now;
                            ProcessPacket((IPEndPoint)remoteEndPoint, ArtNetPacket.Create(receiveState));
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnUnhandledException(ex);
                }
                finally
                {
                    //Attempt to receive another packet.
                    StartReceive();
                }
            }
        }

        private void ProcessPacket(IPEndPoint source, ArtNetPacket packet)
        {
            if (packet != null)
            {
                if (NewPacket != null)
                    NewPacket(this, new NewPacketEventArgs<ArtNetPacket>(source, packet));

                ArtRdmPacket rdmPacket = packet as ArtRdmPacket;
                if (rdmPacket != null && NewRdmPacket != null)
                {
                    using (var ms = new MemoryStream(rdmPacket.RdmData))
                    {
                        using (var readerStream = new RdmBinaryReader(ms))
                        {
                            RdmPacket rdm = RdmPacket.ReadPacket(readerStream);
                            NewRdmPacket(this, new NewPacketEventArgs<RdmPacket>(source, rdm));
                        }
                    }

                }
            }
        }

        protected void OnUnhandledException(Exception ex)
        {
            if (UnhandledException != null) UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        #region Sending

        public void Send(ArtNetPacket packet)
        {
            SendTo(packet.ToArray(), new IPEndPoint(BroadcastAddress, Port));
        }

        public void Send(ArtNetPacket packet, IPEndPoint endPoint)
        {
            SendTo(packet.ToArray(), endPoint);
        }

        public void Send(ArtNetPacket packet, RdmEndPoint address)
        {
            SendTo(packet.ToArray(), new IPEndPoint(address.IpAddress, Port));
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId)
        {
            SendRdm(packet, targetAddress, targetId, RdmId);
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId, UId sourceId)
        {
            //Fill in addition details
            packet.Header.SourceId = sourceId;
            packet.Header.DestinationId = targetId;

            //Sub Devices
            if (targetId is SubDeviceUId)
                packet.Header.SubDevice = ((SubDeviceUId)targetId).SubDeviceId;

            //Create Rdm Packet
            using (var rdmData = new MemoryStream())
            {
                using (var rdmWriter = new RdmBinaryWriter(rdmData))
                {
                    //Write the RDM packet
                    RdmPacket.WritePacket(packet, rdmWriter);

                    //Write the checksum
                    rdmWriter.WriteNetwork((short)(RdmPacket.CalculateChecksum(rdmData.GetBuffer()) + (int)RdmVersions.SubMessage + (int)DmxStartCodes.RDM));

                    //Create sACN Packet
                    ArtRdmPacket rdmPacket = new ArtRdmPacket();
                    rdmPacket.Address = (byte)targetAddress.Universe;
                    rdmPacket.SubStartCode = (byte)RdmVersions.SubMessage;
                    rdmPacket.Net = targetAddress.Net;
                    rdmPacket.RdmData = rdmData.GetBuffer();
                    Send(rdmPacket, targetAddress);
                }
            }

            RdmPacketSent?.Invoke(this, new NewPacketEventArgs<RdmPacket>(new IPEndPoint(targetAddress.IpAddress, Port), packet));
        }

        public void SendRdm(List<RdmPacket> packets, RdmEndPoint targetAddress, UId targetId)
        {
            if (packets.Count < 1)
                throw new ArgumentException("Rdm packets list is empty.");

            RdmPacket primaryPacket = packets[0];

            //Create sACN Packet
            ArtRdmSubPacket rdmPacket = new ArtRdmSubPacket();
            rdmPacket.DeviceId = targetId;
            rdmPacket.RdmVersion = (byte)RdmVersions.SubMessage;
            rdmPacket.Command = primaryPacket.Header.Command;
            rdmPacket.ParameterId = primaryPacket.Header.ParameterId;
            rdmPacket.SubDevice = (short)primaryPacket.Header.SubDevice;
            rdmPacket.SubCount = (short)packets.Count;

            using (MemoryStream rdmData = new MemoryStream())
            {
                using (RdmBinaryWriter dataWriter = new RdmBinaryWriter(rdmData))
                {
                    foreach (RdmPacket item in packets)
                        RdmPacket.WritePacket(item, dataWriter, true);

                    rdmPacket.RdmData = rdmData.ToArray();
                }
            }

            Send(rdmPacket, targetAddress);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;

            base.Dispose(disposing);
        }
    }
}
