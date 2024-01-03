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

        #region Player vars
        internal string Name { get; set; }
        internal uint PlayerID { get; set; }
        internal byte Path { get; set; }
        internal ClientTab ClientTab { get; set; }
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

        #region Packet methods
        internal void RequestProfile()
        {
            Enqueue(new ClientPacket(45));
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


