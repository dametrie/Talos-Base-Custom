using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talos.Forms;
using Talos.Cryptography;
using Talos.Networking;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using Talos.Enumerations;
using Talos.Player;

namespace Talos
{
    internal class Client
    {
        #region Networking Vars
        internal Server _server;
        internal Crypto _crypto;
        internal Socket _clientSocket;
        internal Socket _serverSocket;

        internal bool _connected = false;

        internal Queue<Packet> _sendQueue;
        internal Queue<Packet> _receiveQueue;

        internal byte _serverOrdinal;
        internal byte _clientOrdinal;
        internal byte[] _clientBuffer = new byte[4096];
        internal byte[] _serverBuffer = new byte[4096];
        internal List<byte> _fullClientBuffer = new List<byte>();
        internal List<byte> _fullServerBuffer = new List<byte>();

        private Thread _clientLoopThread = null;
        #endregion

        internal bool safeScreen;
        internal Cheats cheats;
        internal bool inArena = false;
        //internal bool inArena//Adam Fix
        //{
        
            //get
            //{
            //    string mapName = Map?.Name;

            //    if (mapName == "Balanced Arena" || mapName == "Coliseum Circuit")
            //    {
            //        return false;
            //    }

            //    if (mapName?.Contains("Arena") == true || mapName?.Contains("Loures Battle Ring") == true)
            //    {
            //        return true;
            //    }

            //    return mapName?.Contains("Coliseum") == true;
            //}
       // }

        #region Player vars
        internal Statistics _stats;
        internal string Name { get; set; }
        internal uint PlayerID { get; set; }
        internal byte Path { get; set; }
        internal ClientTab ClientTab { get; set; }
        internal bool DialogOn { get; set; }
        internal int Health
        {
            get
            {
                if (CurrentHP * 100 / MaximumHP <= 100)
                {
                    return (int)(CurrentHP * 100 / MaximumHP);
                }
                return 100;
            }
        }
        internal int Mana
        {
            get
            {
                if (CurrentMP * 100 / MaximumMP <= 100)
                {
                    return (int)(CurrentMP * 100 / MaximumMP);
                }
                return 100;
            }
        }
        internal byte Level
        {
            get
            {
                return _stats._level;
            }
            set
            {
                _stats._level = value;
            }
        }
        internal byte Ability
        {
            get
            {
                return _stats._ability;
            }
            set
            {
                _stats._ability = value;
            }
        }
        internal uint MaximumHP
        {
            get
            {
                return _stats._maximumHP;
            }
            set
            {
                _stats._maximumHP = value;
            }
        }
        internal uint MaximumMP
        {
            get
            {
                return _stats._maximumMP;
            }
            set
            {
                _stats._maximumMP = value;
            }
        }
        internal byte CurrentStr
        {
            get
            {
                return _stats._currentStr;
            }
            set
            {
                _stats._currentStr = value;
            }
        }
        internal byte CurrentInt
        {
            get
            {
                return _stats._currentInt;
            }
            set
            {
                _stats._currentInt = value;
            }
        }
        internal byte CurrentWis
        {
            get
            {
                return _stats._currentWis;
            }
            set
            {
                _stats._currentWis = value;
            }
        }
        internal byte CurrentCon
        {
            get
            {
                return _stats._currentCon;
            }
            set
            {
                _stats._currentCon = value;
            }
        }
        internal byte CurrentDex
        {
            get
            {
                return _stats._currentDex;
            }
            set
            {
                _stats._currentDex = value;
            }
        }
        internal bool HasUnspentPoints
        {
            get
            {
                return _stats._hasUnspentPoints;
            }
            set
            {
                _stats._hasUnspentPoints = value;
            }
        }
        internal byte UnspentPoints
        {
            get
            {
                return _stats._unspentPoints;
            }
            set
            {
                _stats._unspentPoints = value;
            }
        }
        internal short MaximumWeight
        {
            get
            {
                return _stats._maximumWeight;
            }
            set
            {
                _stats._maximumWeight = value;
            }
        }
        internal short CurrentWeight
        {
            get
            {
                return _stats._currentWeight;
            }
            set
            {
                _stats._currentWeight = value;
            }
        }
        internal uint CurrentHP
        {
            get
            {
                return _stats._currentHP;
            }
            set
            {
                _stats._currentHP = value;
            }
        }
        internal uint CurrentMP
        {
            get
            {
                return _stats._currentMP;
            }
            set
            {
                _stats._currentMP = value;
            }
        }
        internal uint Experience
        {
            get
            {
                return _stats._experience;
            }
            set
            {
                _stats._experience = value;
            }
        }
        internal uint ToNextLevel
        {
            get
            {
                return _stats._toNextLevel;
            }
            set
            {
                _stats._toNextLevel = value;
            }
        }
        internal uint AbilityExp
        {
            get
            {
                return _stats._abilityExp;
            }
            set
            {
                _stats._abilityExp = value;
            }
        }
        internal uint ToNextAbility
        {
            get
            {
                return _stats._toNextAbility;
            }
            set
            {
                _stats._toNextAbility = value;
            }
        }
        internal uint GamePoints
        {
            get
            {
                return _stats._gamePoints;
            }
            set
            {
                _stats._gamePoints = value;
            }
        }
        internal uint Gold
        {
            get
            {
                return _stats._gold;
            }
            set
            {
                _stats._gold = value;
            }
        }
        internal Statistics Stats
        {
            get
            {
                return _stats;
            }
            set
            {
                _stats = value;
            }
        }
        internal bool HasLetter => _stats._mail.HasFlag(Mail.HasLetter);
        internal bool HasParcel => _stats._mail.HasFlag(Mail.HasParcel);
        internal byte Blind
        {
            get
            {
                return _stats._blind;
            }
            set
            {
                _stats._blind = value;
            }
        }

        #endregion




        internal Client(Server server, Socket socket)
        {
            _server = server;
            _crypto = new Crypto(0, "UrkcnItnI");
            _clientSocket = socket;
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _sendQueue = new Queue<Packet>();
            _receiveQueue = new Queue<Packet>();

        }
        internal void Remove()
        {
            ClientTab.RemoveClient();
            ClientTab = null;
        }

        internal bool getCheats(Cheats value)
        {
            return cheats.HasFlag(value);
        }

        #region Packet methods
        internal void RequestProfile()
        {
            Enqueue(new ClientPacket(45));
        }

        internal void ServerMessage(byte type, string message)
        {
            if (!safeScreen)
            {
                ServerPacket serverPacket = new ServerPacket(10);
                serverPacket.WriteByte(type);
                serverPacket.WriteString16(message);
                Enqueue(serverPacket);
            }
        }

        internal void Attributes(StatUpdateFlags value, Statistics stats)
        {
            ServerPacket serverPacket = new ServerPacket(8);
            if (getCheats(Cheats.GmMode))
            {
                value |= StatUpdateFlags.GameMasterA;
            }
            if (stats._mail != 0)
            {
                value |= (StatUpdateFlags.UnreadMail | StatUpdateFlags.Secondary);
            }
            serverPacket.WriteByte((byte)value);
            if (value.HasFlag(StatUpdateFlags.Primary))
            {
                serverPacket.Write(new byte[3]);
                serverPacket.WriteByte(stats._level);
                serverPacket.WriteByte(stats._ability);
                serverPacket.WriteUInt32(stats._maximumHP);
                serverPacket.WriteUInt32(stats._maximumMP);
                serverPacket.WriteByte(stats._currentStr);
                serverPacket.WriteByte(stats._currentInt);
                serverPacket.WriteByte(stats._currentWis);
                serverPacket.WriteByte(stats._currentCon);
                serverPacket.WriteByte(stats._currentDex);
                serverPacket.WriteBoolean(stats._hasUnspentPoints);
                serverPacket.WriteByte(stats._unspentPoints);
                serverPacket.WriteInt16(stats._maximumWeight);
                serverPacket.WriteInt16(stats._currentWeight);
                serverPacket.Write(new byte[4]);
            }
            if (value.HasFlag(StatUpdateFlags.Current))
            {
                serverPacket.WriteUInt32(stats._currentHP);
                serverPacket.WriteUInt32(stats._currentMP);
            }
            if (value.HasFlag(StatUpdateFlags.Experience))
            {
                serverPacket.WriteUInt32(stats._experience);
                serverPacket.WriteUInt32(stats._toNextLevel);
                serverPacket.WriteUInt32(stats._abilityExp);
                serverPacket.WriteUInt32(stats._toNextAbility);
                serverPacket.WriteUInt32(stats._gamePoints);
                serverPacket.WriteUInt32(stats._gold);
            }
            if (value.HasFlag(StatUpdateFlags.Secondary))
            {
                serverPacket.Write(new byte[1]);
                if (getCheats(Cheats.NoBlind) && !inArena)
                {
                    serverPacket.WriteByte(0);
                }
                else
                {
                    serverPacket.WriteByte(stats._blind);
                }
                serverPacket.Write(new byte[4]);
                serverPacket.WriteByte((byte)stats._offenseElement);
                serverPacket.WriteByte((byte)stats._defenseElement);
                serverPacket.WriteByte(stats._magicResistance);
                serverPacket.Write(new byte[1]);
                serverPacket.WriteSByte(stats._armorClass);
                serverPacket.WriteByte(stats._damage);
                serverPacket.WriteByte(stats._hit);
            }
            Enqueue(serverPacket);
        }

        #endregion

        #region Networking
        /// <summary>
        /// Establishes a connection and begins receiving data. Begins a new _clientLoopThread
        /// and starts it.
        /// </summary>
        /// <param name="endPoint">The remote endpoint to connect to.</param>
        internal void Connect(EndPoint endPoint)
        {
            _serverSocket.Connect(endPoint);
            _connected = true;
            _clientSocket.BeginReceive(_clientBuffer, 0, _clientBuffer.Length, SocketFlags.None, new AsyncCallback(ClientEndReceive), this);
            _serverSocket.BeginReceive(_serverBuffer, 0, _serverBuffer.Length, SocketFlags.None, new AsyncCallback(ServerEndReceive), this);
            _clientLoopThread = new Thread(ClientLoop);
            _clientLoopThread.Start();
        }

        /// <summary>
        /// Main client loop that handles sending and receiving packets.
        /// </summary>
        private void ClientLoop()
        {
            Thread.GetDomain().UnhandledException += Program.ExceptionHandler;
            while (_connected)
            {
                //Console.WriteLine("Connected");
                lock (_sendQueue)
                {
                    while (_sendQueue.Count > 0)
                    {
                        Packet packet = _sendQueue.Dequeue();
                        ClientPacket? clientPacket = packet as ClientPacket;
                        Socket socket;
                        if (clientPacket != null)
                        {
                            Console.WriteLine(clientPacket.ToString());
                            if (ClientTab != null && !ClientTab.IsDisposed)
                                ClientTab.LogPackets(clientPacket);
                            if (clientPacket.IsDialog)
                            {
                                clientPacket.EncryptDialog();
                            }
                            if (clientPacket.Opcode == 98)
                            {
                                _serverOrdinal = clientPacket.Sequence;
                            }
                            if (clientPacket.ShouldEncrypt)
                            {
                                clientPacket.Sequence = _serverOrdinal++;
                                clientPacket.Encrypt(_crypto);
                            }
                            socket = _serverSocket;
                        }
                        else
                        {
                            ServerPacket? serverPacket = packet as ServerPacket;
                            Console.WriteLine(serverPacket.ToString());
                            if (ClientTab != null && !ClientTab.IsDisposed)
                                ClientTab.LogPackets(serverPacket);
                            if (serverPacket.ShouldEncrypt)
                            {
                                serverPacket.Sequence = _clientOrdinal++;
                                serverPacket.Encrypt(_crypto);
                            }
                            socket = _clientSocket;
                        }
                        byte[] array = packet.CreatePacket();
                        try
                        {
                            socket.BeginSend(array, 0, array.Length, SocketFlags.None, EndSend, socket);
                        }
                        catch (Exception ex)
                        {
                            string text = AppDomain.CurrentDomain.BaseDirectory + "CrashLogs\\";
                            if (!Directory.Exists(text))
                            {
                                Directory.CreateDirectory(text);
                            }
                            text = text + DateTime.Now.ToString("MM-dd-HH-yyyy h mm tt") + ".log";
                            File.WriteAllText(text, ex.ToString());
                        }
                    }
                }
                lock (_receiveQueue)
                {
                    while (_receiveQueue.Count > 0)
                    {
                        Packet packet = _receiveQueue.Dequeue();
                        if (packet.ShouldEncrypt)
                        {
                            packet.Decrypt(_crypto);
                        }
                        bool flag = true;
                        ClientPacket? clientPacket = packet as ClientPacket;
                        if (clientPacket != null)
                        {
                            if (clientPacket.IsDialog)
                            {
                                clientPacket.DecryptDialog();
                            }
                            ClientMessageHandler clientHandle = _server.ClientMessage[clientPacket.Opcode];
                            if (clientHandle != null)
                            {
                                lock (Server.Lock)
                                {
                                    try
                                    {
                                        flag = clientHandle(this, clientPacket);
                                    }
                                    catch
                                    {
                                        flag = false;
                                    }
                                }
                            }
                        }
                        else if (packet is ServerPacket)
                        {
                            ServerPacket? serverPacket = packet as ServerPacket;
                            ServerMessageHandler serverHandle = _server.ServerMessage[serverPacket.Opcode];
                            if (serverHandle != null)
                            {
                                lock (Server.Lock)
                                {
                                    try
                                    {
                                        flag = serverHandle(this, (ServerPacket)packet);
                                    }
                                    catch
                                    {
                                        flag = false;
                                    }

                                }
                            }
                        }
                        if (flag)
                        {
                            Enqueue(packet);
                        }
                    }
                }
                Thread.Sleep(5);
            }
            try
            {
                _clientSocket.Disconnect(false);
                _serverSocket.Disconnect(false);
            }
            catch
            {
            }
            //no longer connected
            _server._clientList.Remove(this);
            Thread.Sleep(100);
            _server._mainForm.RemoveClient(this);

        }

        /// <summary>
        /// Asynchronously finalizes the sending of a packet.
        /// </summary>
        /// <param name="ar">Result of the async operation.</param>
        private void EndSend(IAsyncResult ar)
        {
            ((Socket)ar.AsyncState).EndSend(ar);
        }

        /// <summary>
        /// Asynchronously receives data from the client, and processes the information.
        /// </summary>
        /// <param name="ar">Result of the async operation.</param>
        private void ClientEndReceive(IAsyncResult ar)
        {
            try
            {
                int length = _clientSocket.EndReceive(ar);
                if (length == 0)
                {
                    DisconnectWait();
                }
                else
                {
                    byte[] data = new byte[length];
                    Buffer.BlockCopy(_clientBuffer, 0, data, 0, length);
                    _fullClientBuffer.AddRange(data);
                    while (_fullClientBuffer.Count > 3)
                    {
                        int count = (_fullClientBuffer[1] << 8) + _fullClientBuffer[2] + 3;
                        if (count > _fullClientBuffer.Count)
                        {
                            break;
                        }
                        List<byte> range = _fullClientBuffer.GetRange(0, count);
                        _fullClientBuffer.RemoveRange(0, count);
                        ClientPacket clientPacket = new ClientPacket(range.ToArray());
                        lock (_receiveQueue)
                        {
                            _receiveQueue.Enqueue(clientPacket);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CLIENT ENDRECEIVE EXCEPTION + ", ex.ToString());
                DisconnectWait();
            }
            finally
            {
                _clientSocket.BeginReceive(_clientBuffer, 0, _clientBuffer.Length, SocketFlags.None, ClientEndReceive, null);
            }
        }

        /// <summary>
        /// Asynchronously receives data from the client, and processes the information.
        /// </summary>
        /// <param name="ar">Result of the async operation.</param>
        private void ServerEndReceive(IAsyncResult ar)
        {
            try
            {
                int length = _serverSocket.EndReceive(ar);
                if (length == 0)
                {
                    DisconnectWait();
                }
                else
                {
                    byte[] data = new byte[length];
                    Buffer.BlockCopy(_serverBuffer, 0, data, 0, length);
                    _fullServerBuffer.AddRange(data);
                    while (_fullServerBuffer.Count > 3)
                    {
                        int count = (_fullServerBuffer[1] << 8) + _fullServerBuffer[2] + 3;
                        if (count > _fullServerBuffer.Count)
                        {
                            break;
                        }
                        List<byte> range = _fullServerBuffer.GetRange(0, count);
                        _fullServerBuffer.RemoveRange(0, count);
                        ServerPacket serverPacket = new ServerPacket(range.ToArray());
                        lock (_receiveQueue)
                        {
                            _receiveQueue.Enqueue(serverPacket);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SERVER ENDRECEIVE EXCEPTION + ", ex.ToString());
                DisconnectWait();
            }
            finally
            {
                _serverSocket.BeginReceive(_serverBuffer, 0, _serverBuffer.Length, SocketFlags.None, ServerEndReceive, null);
            }
        }

        /// <summary>
        /// Enqueues a packet to be sent to the server.
        /// </summary>
        /// <param name="packetArray">The packet array to be sent</param> 
        internal void Enqueue(params Packet[] packetArray)
        {
            if (Thread.CurrentThread.ManagedThreadId == Program.MainForm.ThreadID)
                ThreadPool.QueueUserWorkItem(delegate { Enqueue(packetArray); });
            else
            {
                lock (_sendQueue)
                {
                    Packet[] array = packetArray;
                    foreach (Packet packet in array)
                    {
                        _sendQueue.Enqueue(packet);
                    }
                }
            }
        }

        /// <summary>
        /// Disconnect from the server.
        /// </summary>
        /// <param name="wait">True or false depending on whether we are disconnecting</param>
        internal void DisconnectWait(bool wait = false)
        {
            _connected = false;
            if (wait)
            {
                _clientLoopThread.Join(1000);
                if (_clientLoopThread.IsAlive)
                {
                    _clientLoopThread.Abort();
                }
            }
        }

       
        #endregion

    }
}


