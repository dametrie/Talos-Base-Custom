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
        internal Statistics stats;
        internal string Name { get; set; }
        internal uint PlayerID { get; set; }
        internal byte Path { get; set; }
        internal ClientTab ClientTab { get; set; }
        internal bool DialogOn { get; set; }
        internal int Health
        {
            get
            {
                if (CurrentHP * 100 / MaxHP <= 100)
                {
                    return (int)(CurrentHP * 100 / MaxHP);
                }
                return 100;
            }
        }
        internal int Mana
        {
            get
            {
                if (CurrentMP * 100 / MaxMP <= 100)
                {
                    return (int)(CurrentMP * 100 / MaxMP);
                }
                return 100;
            }
        }
        internal byte Level
        {
            get
            {
                return stats.Level;
            }
            set
            {
                stats.Level = value;
            }
        }
        internal byte Ability
        {
            get
            {
                return stats.Ability;
            }
            set
            {
                stats.Ability = value;
            }
        }
        internal uint MaxHP
        {
            get
            {
                return stats.MaxHP;
            }
            set
            {
                stats.MaxHP = value;
            }
        }
        internal uint MaxMP
        {
            get
            {
                return stats.MaxMP;
            }
            set
            {
                stats.MaxMP = value;
            }
        }
        internal byte Str
        {
            get
            {
                return stats.Str;
            }
            set
            {
                stats.Str = value;
            }
        }
        internal byte Int
        {
            get
            {
                return stats.Int;
            }
            set
            {
                stats.Int = value;
            }
        }
        internal byte Wis
        {
            get
            {
                return stats.Wis;
            }
            set
            {
                stats.Wis = value;
            }
        }
        internal byte Con
        {
            get
            {
                return stats.Con;
            }
            set
            {
                stats.Con = value;
            }
        }
        internal byte Dex
        {
            get
            {
                return stats.Dex;
            }
            set
            {
                stats.Dex = value;
            }
        }
        internal bool UnspentPoints
        {
            get
            {
                return stats.HasUnspentPoints;
            }
            set
            {
                stats.HasUnspentPoints = value;
            }
        }
        internal byte AvailablePoints
        {
            get
            {
                return stats.AvailablePoints;
            }
            set
            {
                stats.AvailablePoints = value;
            }
        }
        internal short MaxWeight
        {
            get
            {
                return stats.MaxWeight;
            }
            set
            {
                stats.MaxWeight = value;
            }
        }
        internal short CurrentWeight
        {
            get
            {
                return stats.CurrentWeight;
            }
            set
            {
                stats.CurrentWeight = value;
            }
        }
        internal uint CurrentHP
        {
            get
            {
                return stats.CurrentHP;
            }
            set
            {
                stats.CurrentHP = value;
            }
        }
        internal uint CurrentMP
        {
            get
            {
                return stats.CurrentMP;
            }
            set
            {
                stats.CurrentMP = value;
            }
        }
        internal uint Experience
        {
            get
            {
                return stats.Experience;
            }
            set
            {
                stats.Experience = value;
            }
        }
        internal uint ToNextLevel
        {
            get
            {
                return stats.ToNextLevel;
            }
            set
            {
                stats.ToNextLevel = value;
            }
        }
        internal uint AbilityExp
        {
            get
            {
                return stats.AbilityExp;
            }
            set
            {
                stats.AbilityExp = value;
            }
        }
        internal uint ToNextAbility
        {
            get
            {
                return stats.ToNextAbility;
            }
            set
            {
                stats.ToNextAbility = value;
            }
        }
        internal uint GamePoints
        {
            get
            {
                return stats.GamePoints;
            }
            set
            {
                stats.GamePoints = value;
            }
        }
        internal uint Gold
        {
            get
            {
                return stats.Gold;
            }
            set
            {
                stats.Gold = value;
            }
        }
        internal Statistics Stats
        {
            get
            {
                return stats;
            }
            set
            {
                stats = value;
            }
        }
        internal bool HasLetter => stats.Mail.HasFlag(Mail.HasLetter);
        internal bool HasParcel => stats.Mail.HasFlag(Mail.HasParcel);
        internal byte Byte_10//Figure this out
        {
            get
            {
                return stats.byte_8;
            }
            set
            {
                stats.byte_8 = value;
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

        internal void Attributes(StatusType value, Statistics stats)
        {
            ServerPacket serverPacket = new ServerPacket(8);
            if (getCheats(Cheats.GmMode))
            {
                value |= StatusType.GameMasterA;
            }
            if (stats.Mail != 0)
            {
                value |= (StatusType.UnreadMail | StatusType.Secondary);
            }
            serverPacket.WriteByte((byte)value);
            if (value.HasFlag(StatusType.Primary))
            {
                serverPacket.Write(new byte[3]);
                serverPacket.WriteByte(stats.Level);
                serverPacket.WriteByte(stats.Ability);
                serverPacket.WriteUInt32(stats.MaxHP);
                serverPacket.WriteUInt32(stats.MaxMP);
                serverPacket.WriteByte(stats.Str);
                serverPacket.WriteByte(stats.Int);
                serverPacket.WriteByte(stats.Wis);
                serverPacket.WriteByte(stats.Con);
                serverPacket.WriteByte(stats.Dex);
                serverPacket.WriteBoolean(stats.HasUnspentPoints);
                serverPacket.WriteByte(stats.AvailablePoints);
                serverPacket.WriteInt16(stats.MaxWeight);
                serverPacket.WriteInt16(stats.CurrentWeight);
                serverPacket.Write(new byte[4]);
            }
            if (value.HasFlag(StatusType.Current))
            {
                serverPacket.WriteUInt32(stats.CurrentHP);
                serverPacket.WriteUInt32(stats.CurrentMP);
            }
            if (value.HasFlag(StatusType.Experience))
            {
                serverPacket.WriteUInt32(stats.Experience);
                serverPacket.WriteUInt32(stats.ToNextLevel);
                serverPacket.WriteUInt32(stats.AbilityExp);
                serverPacket.WriteUInt32(stats.ToNextAbility);
                serverPacket.WriteUInt32(stats.GamePoints);
                serverPacket.WriteUInt32(stats.Gold);
            }
            if (value.HasFlag(StatusType.Secondary))
            {
                serverPacket.Write(new byte[1]);
                if (getCheats(Cheats.NoBlind) && !inArena)
                {
                    serverPacket.WriteByte(0);
                }
                else
                {
                    serverPacket.WriteByte(stats.byte_8);
                }
                serverPacket.Write(new byte[4]);
                serverPacket.WriteByte((byte)stats.AttackElement);
                serverPacket.WriteByte((byte)stats.DefenseElement);
                serverPacket.WriteByte(stats.MagicResistance);
                serverPacket.Write(new byte[1]);
                serverPacket.WriteSByte(stats.ArmorClass);
                serverPacket.WriteByte(stats.Damage);
                serverPacket.WriteByte(stats.Hit);
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


