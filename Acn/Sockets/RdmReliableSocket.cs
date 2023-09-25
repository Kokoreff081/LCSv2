using System;
using Acn.Rdm;
using System.ComponentModel;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Acn.Sockets
{
    /// <summary>
    /// This RDM socket provides a reliable means of transporting RDM packets over an unreliable network. It
    /// will wrap an unreliable RDM socket.
    /// </summary>
    /// <remarks>
    /// Ensures that a transaction is completed by re-requesting packets for which no response has been received.
    /// </remarks>
    public class RdmReliableSocket : IRdmSocket, INotifyPropertyChanged, IDisposable
    {
        private readonly IRdmSocket _socket;

        /// <summary>
        /// Dictionary of transactions where key is a hashCode of IpAddress and universe address (function GetAddressHashCode). It is a high-priority queue
        /// </summary>
        private readonly ConcurrentDictionary<int, ConcurrentQueue<Transaction>> _forceTransactionsQueue = new ConcurrentDictionary<int, ConcurrentQueue<Transaction>>();

        /// <summary>
        /// Dictionary of transactions where key is a hashCode of IpAddress and universe address (function GetAddressHashCode)
        /// </summary>
        private readonly ConcurrentDictionary<int, ConcurrentQueue<Transaction>> _transactionsQueue = new ConcurrentDictionary<int, ConcurrentQueue<Transaction>>();

        /// <summary>
        /// Dictionary of retryTransactions where key is a hashCode of IpAddress and universe address (function GetAddressHashCode). It is a high-priority retry queue
        /// </summary>
        private readonly ConcurrentDictionary<int, ConcurrentQueue<Transaction>> _forceRetryTransactionsQueue = new ConcurrentDictionary<int, ConcurrentQueue<Transaction>>();

        /// <summary>
        /// Dictionary of retryTransactions where key is a hashCode of IpAddress and universe address (function GetAddressHashCode).
        /// </summary>
        private readonly ConcurrentDictionary<int, ConcurrentQueue<Transaction>> _retryTransactionsQueue = new ConcurrentDictionary<int, ConcurrentQueue<Transaction>>();

        /// <summary>
        /// Dictionary of transactions where key is a hashCode of transaction. The dictionary is needed to match the received transaction
        /// </summary>
        private readonly ConcurrentDictionary<int, Transaction> _allTransactions = new ConcurrentDictionary<int, Transaction>();

        private Timer _nextTransactionTimer;
        private bool _timerRunning;

        public event EventHandler<NewPacketEventArgs<RdmPacket>> NewRdmPacket;
        public event EventHandler<NewPacketEventArgs<RdmPacket>> RdmPacketSent;

        public event Action AllTransactionsCompleted;

        private readonly object _timerLockObj = new object();

        private readonly object _counterLockObj = new object();

        private class Transaction : IEquatable<Transaction>
        {
            public Transaction(int id, RdmPacket packet, RdmEndPoint address, UId targetId)
            {
                Id = id;
                TransactionNumber = (byte)(id % 255);
                Packet = packet;
                TargetAddress = address;
                TargetId = targetId;
                Attempts = 0;
                LastAttempt = DateTime.MinValue;
            }

            public int Id;
            public byte TransactionNumber;
            public RdmPacket Packet;
            public RdmEndPoint TargetAddress;
            public UId TargetId;

            public int Attempts = 0;
            public DateTime LastAttempt = DateTime.MinValue;

            public bool Completed;

            public bool Equals(Transaction other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return TransactionNumber == other.TransactionNumber && Equals(TargetId, other.TargetId);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Transaction)obj);
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

        public RdmReliableSocket(IRdmSocket socket)
        {
            _socket = socket;

            socket.NewRdmPacket += socket_NewRdmPacket;
            socket.RdmPacketSent += socket_RdmPacketSent;

            _nextTransactionTimer = new Timer(OnNextTransaction);
        }

        private TimeSpan RetryInterval => TimeSpan.FromSeconds(10);
        private TimeSpan TransmitInterval => TimeSpan.FromMilliseconds(60);

        public int RetryAttempts => 3;

        private int _transactionNumber = 1;

        private int AllocateTransactionNumber()
        {
            return Interlocked.Increment(ref _transactionNumber);
        }

        private int _packetsSent;

        public int PacketsSent
        {
            get => _packetsSent;
            private set
            {
                if (_packetsSent != value)
                {
                    _packetsSent = value;
                    RaisePropertyChanged("PacketsSent");
                }
            }
        }

        private int _packetsReceived;

        public int PacketsReceived
        {
            get => _packetsReceived;
            private set
            {
                if (_packetsReceived != value)
                {
                    _packetsReceived = value;
                    RaisePropertyChanged("PacketsReceived");
                }
            }
        }

        private int _packetsDropped;

        public int PacketsDropped
        {
            get => _packetsDropped;
            private set
            {
                if (_packetsDropped != value)
                {
                    _packetsDropped = value;
                    RaisePropertyChanged("PacketsDropped");
                }
            }
        }

        private int _transactionsStarted;

        public int TransactionsStarted
        {
            get => _transactionsStarted;
            private set
            {
                if (_transactionsStarted != value)
                {
                    _transactionsStarted = value;
                    RaisePropertyChanged("TransactionsStarted");
                }
            }
        }

        private int _transactionsFailed;

        public int TransactionsFailed
        {
            get => _transactionsFailed;
            private set
            {
                if (_transactionsFailed != value)
                {
                    _transactionsFailed = value;
                    RaisePropertyChanged("TransactionsFailed");
                }
            }
        }

        private int _transactionsReceived;

        public int TransactionsReceived
        {
            get => _transactionsReceived;
            private set
            {
                if (_transactionsReceived != value)
                {
                    _transactionsReceived = value;
                    RaisePropertyChanged("TransactionsReceived");
                }
            }
        }

        private void RegisterTransaction(ConcurrentDictionary<int, ConcurrentQueue<Transaction>> transactionsQueue, RdmPacket packet, RdmEndPoint address, UId id)
        {
            if (packet.Header.Command != RdmCommands.Get && packet.Header.Command != RdmCommands.Set)
                return;

            int transactionId = AllocateTransactionNumber();
            Transaction transaction = new Transaction(transactionId, packet, address, id);

            if (_allTransactions.ContainsKey(transaction.GetHashCode())) // If it's already exist then reset the transaction
            {
                transaction = _allTransactions[transaction.GetHashCode()];
                transaction.Completed = false;
                transaction.Attempts = 0;
            }

            int addressHashCode = GetAddressHashCode(transaction.TargetAddress);

            // Add to the _transactionsQueue
            if (transactionsQueue.TryGetValue(addressHashCode, out ConcurrentQueue<Transaction> transactions))
            {
                transactions.Enqueue(transaction);
            }
            else
            {
                ConcurrentQueue<Transaction> nTransactions = new ConcurrentQueue<Transaction>();
                nTransactions.Enqueue(transaction);
                transactionsQueue.TryAdd(addressHashCode, nTransactions);
            }

            packet.Header.TransactionNumber = transaction.TransactionNumber;

            //Debug.WriteLine($"Sent - {packet.Header}, UId = {id}");
            // Add to the _allTransactions dictionary 
            _allTransactions.TryAdd(transaction.GetHashCode(), transaction);

            if (!_timerRunning) // if timer is not running, start it
            {
                lock (_timerLockObj)
                {
                    _nextTransactionTimer?.Change(TransmitInterval, TimeSpan.Zero);
                    _timerRunning = true;
                }
            }

            lock (_counterLockObj)
                TransactionsStarted++;
        }

        /// <summary>
        /// Returns hashCode of the address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private int GetAddressHashCode(RdmEndPoint address)
        {
            return address.IpAddress.GetHashCode() << 8 ^ address.Universe.GetHashCode();
        }

        private DateTime _lastSentRequest;
        /// <summary>
        /// Processes the transaction queue and determines what transactions can be sent to their destination.
        /// </summary>
        /// <remarks>
        /// This function ensures that only a single transaction is sent to each DMX port every TransmitInterval. Any more 
        /// transactions per port and the device might become flooded.
        /// </remarks>
        /// <param name="state">Thread State</param>
        private void OnNextTransaction(object state)
        {
            bool isAnySent = false;
            var transactionsQueueTuples = new[]
            {
                new Tuple<ConcurrentDictionary<int, ConcurrentQueue<Transaction>>, ConcurrentDictionary<int, ConcurrentQueue<Transaction>>>(_forceTransactionsQueue, _forceRetryTransactionsQueue),
                new Tuple<ConcurrentDictionary<int, ConcurrentQueue<Transaction>>, ConcurrentDictionary<int, ConcurrentQueue<Transaction>>>(_transactionsQueue, _retryTransactionsQueue)
            };

            foreach (var transactionsQueue in transactionsQueueTuples)
            {
                isAnySent = SendTransactionsInQueue(transactionsQueue.Item1, transactionsQueue.Item2);

                if (isAnySent)
                    break;
            }

            if (!isAnySent)
            {
                foreach (var transactionsQueue in new[] { _forceRetryTransactionsQueue, _retryTransactionsQueue })
                {
                    isAnySent = SendTransactionsInRetryQueue(transactionsQueue);

                    if (isAnySent)
                        break;
                }
                if (!isAnySent &&  DateTime.Now.Subtract(_lastSentRequest) >= RetryInterval)
                {
                    _timerRunning = false;
                    lock (_timerLockObj)
                    {
                        _nextTransactionTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    }

                    AllTransactionsCompleted?.Invoke();
                    return;
                }
            }

            lock (_timerLockObj)
            {
                _nextTransactionTimer?.Change(TransmitInterval, TimeSpan.Zero);
            }
        }

        private bool SendTransactionsInQueue(ConcurrentDictionary<int, ConcurrentQueue<Transaction>> transactionsQueue, ConcurrentDictionary<int, ConcurrentQueue<Transaction>> retryTransactionsQueue)
        {
            bool isAnySent = false;

            DateTime timeStamp = DateTime.Now;

            foreach (var transactionsByAddress in transactionsQueue)
            {
                if (transactionsByAddress.Value.TryDequeue(out Transaction transaction))
                {
                    int addressHashCode = GetAddressHashCode(transaction.TargetAddress);

                    transaction.LastAttempt = timeStamp;

                    if (retryTransactionsQueue.ContainsKey(addressHashCode))
                    {
                        retryTransactionsQueue[addressHashCode].Enqueue(transaction);
                    }
                    else
                    {
                        ConcurrentQueue<Transaction> transactions = new ConcurrentQueue<Transaction>();
                        transactions.Enqueue(transaction);
                        retryTransactionsQueue.TryAdd(addressHashCode, transactions);
                    }

                    try
                    {
                        _socket.SendRdm(transaction.Packet, transaction.TargetAddress, transaction.TargetId);
                        _lastSentRequest = DateTime.Now;
                    }
                    catch (SocketException)
                    {
                        lock (_counterLockObj)
                            TransactionsFailed++;
                    }

                    isAnySent = true;
                }
            }
            return isAnySent;
        }

        private bool SendTransactionsInRetryQueue(ConcurrentDictionary<int, ConcurrentQueue<Transaction>> retryTransactionQueue)
        {
            bool isAnySent = false;

            DateTime timeStamp = DateTime.Now;

            foreach (var transactionsByAddress in retryTransactionQueue)
            {
                Transaction transaction = GetNextRetryTransaction(transactionsByAddress.Value);

                if (transaction == null)
                    continue;

                // If RetryAttempts is limited then return
                if (transaction.Attempts > RetryAttempts)
                {
                    lock (_counterLockObj)
                        TransactionsFailed++;
                }
                else
                {
                    try
                    {
                        Debug.WriteLine($"SendTransactionsInRetryQueue - {transaction.Packet}");

                        transaction.Attempts++;
                        transaction.LastAttempt = timeStamp;

                        transactionsByAddress.Value.Enqueue(transaction); // Add queue again

                        lock (_counterLockObj)
                            PacketsDropped++;

                        isAnySent = true;

                        _socket.SendRdm(transaction.Packet, transaction.TargetAddress, transaction.TargetId);

                        _lastSentRequest = DateTime.Now;
                    }
                    catch (SocketException)
                    {
                        lock (_counterLockObj)
                            TransactionsFailed++;
                    }
                }
            }

            return isAnySent;
        }

        private Transaction GetNextRetryTransaction(ConcurrentQueue<Transaction> transactions)
        {
            DateTime timeStamp = DateTime.Now;

            while (!transactions.IsEmpty)
            {
                if (!transactions.TryPeek(out Transaction peekedTransaction) || timeStamp.Subtract(peekedTransaction.LastAttempt) < RetryInterval)
                    return null;

                if (transactions.TryDequeue(out Transaction transaction))
                {
                    if (!transaction.Completed)
                        return transaction;
                }
            }

            return null;
        }

        void socket_RdmPacketSent(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            lock (_counterLockObj) 
                PacketsSent++;

            RdmPacketSent?.Invoke(sender, e);
        }

        void socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            lock (_counterLockObj)
                PacketsReceived++;

            if (e.Packet.Header.Command == RdmCommands.GetResponse || e.Packet.Header.Command == RdmCommands.SetResponse)
            {
                Transaction tr = new Transaction(e.Packet.Header.TransactionNumber, e.Packet, new RdmEndPoint(e.Source.Address, e.Source.Port, 0), e.Packet.Header.SourceId);
                if (_allTransactions.TryGetValue(tr.GetHashCode(), out Transaction transaction))
                {
                    transaction.Completed = true; 
                    lock (_counterLockObj)
                        TransactionsReceived++;
                }
                else
                {
                    Debug.WriteLine($"Not found. TransactionNumber = {e.Packet.Header.TransactionNumber}, Type = {e.Packet}");
                }
            }

            NewRdmPacket?.Invoke(sender, e);
        }

        #region Communications

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId)
        {
            //Queue this packet for sending.
            RegisterTransaction(_transactionsQueue, packet, targetAddress, targetId);
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId, UId sourceId)
        {
            //Queue this packet for sending.
            RegisterTransaction(_transactionsQueue, packet, targetAddress, targetId);
        }

        public void ForceSendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId)
        {
            //Queue this packet for sending. (High-priority queue)
            RegisterTransaction(_forceTransactionsQueue, packet, targetAddress, targetId);

            //lock (_timerLockObj)
            //{
            //    _nextTransactionTimer.Change(Timeout.Infinite, Timeout.Infinite);
            //}

            //Thread.Sleep(TransmitInterval);

            //_socket.SendRdm(packet, targetAddress, targetId);

            //OnNextTransaction(null);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void Dispose()
        {
            _allTransactions.Clear();
            _transactionsQueue.Clear();
            _retryTransactionsQueue.Clear();

            _forceRetryTransactionsQueue.Clear();
            _forceTransactionsQueue.Clear();

            if (_nextTransactionTimer != null)
            {
                ManualResetEvent waitHandle = new ManualResetEvent(false);
                if (_nextTransactionTimer.Dispose(waitHandle))
                {
                    // Timer has not been disposed by someone else
                    if (!waitHandle.WaitOne(1000))
                        throw new TimeoutException("Timeout waiting for timer to stop");
                }
                waitHandle.Close();   // Only close if Dispose has completed successful
                _nextTransactionTimer = null;
            }
        }
    }
}
