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
using Talos.Enumerations;
using Talos.Objects;
using Talos.Structs;
using Talos.Maps;
using Talos.AStar;


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


        internal Location _clientLocation;
        internal Location _serverLocation;
        internal Map _map;
        internal WorldMap _worldMap;
        internal Cheats cheats;
        internal Direction _clientDirection;
        internal Direction _serverDirection;
        internal string _npcDialog;
        internal AutoResetEvent _walkSignal = new AutoResetEvent(false);

        internal Dictionary<int, WorldObject> WorldObjects { get; private set; } = new Dictionary<int, WorldObject>();
        internal Dictionary<string, Player> NearbyPlayers { get; private set; } = new Dictionary<string, Player>();
        internal Dictionary<string, Player> NearbyGhosts{ get; private set; } = new Dictionary<string, Player>();
        internal Dictionary<string, Creature> NearbyNPC { get; private set; } = new Dictionary<string, Creature>();
        internal Dictionary<string, int> ObjectID { get; private set; } = new Dictionary<string, int>();
       
        internal HashSet<int> CreatureHashSet { get; private set; } = new HashSet<int>();
        internal HashSet<int> ObjectHashSet { get; private set; } = new HashSet<int>();
        internal HashSet<Door> Doors { get; private set; } = new HashSet<Door> { };

        internal bool _safeScreen;
        internal bool _inArena = false;
        internal bool _atDoor;
        internal bool _isCasting;
        internal bool _isWalking;
        internal bool _isRefreshing;
        internal bool _canRefresh;
        internal bool _hasLabor;
        internal bool _bool_39;
        internal double _walkSpeed = 420.0;
        internal ushort _monsterFormID = 1;
        internal DateTime _lastWorldShout = DateTime.MinValue;
        internal Thread WalkThread { get; set; }    
        internal ClientTab ClientTab { get; set; }
        internal Statistics Stats { get; set; }
        internal Dialog Dialog { get; set; }
        internal string Name { get; set; }
        internal byte Path { get; set; }
        internal byte StepCount { get; set; }
        internal DateTime LastStep { get; set; }
        internal DateTime LastMoved { get; set; }
        internal DateTime LastTurned { get; set; }
        internal uint PlayerID { get; set; }
        internal bool InMonsterForm { get; set; }
        internal Player Player { get; set; }
        internal int Health
        {
            get
            {
                if (Stats.CurrentHP * 100 / Stats.MaximumHP <= 100)
                {
                    return (int)(Stats.CurrentHP * 100 / Stats.MaximumHP);
                }
                return 100;
            }
        }
        internal int Mana
        {
            get
            {
                if (Stats.CurrentMP * 100 / Stats.MaximumMP <= 100)
                {
                    return (int)(Stats.CurrentMP * 100 / Stats.MaximumMP);
                }
                return 100;
            }
        }
        internal bool DialogOn { get; set; }
        internal bool HasLetter => Stats.Mail.HasFlag(Mail.HasLetter);
        internal bool HasParcel => Stats.Mail.HasFlag(Mail.HasParcel);




        internal Client(Server server, Socket socket)
        {
            _server = server;
            _crypto = new Crypto(0, "UrkcnItnI");
            _clientSocket = socket;
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _sendQueue = new Queue<Packet>();
            _receiveQueue = new Queue<Packet>();
            Stats = new Statistics();
            WorldObjects = new Dictionary<int, WorldObject>();
            NearbyPlayers = new Dictionary<string, Player>();

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

        internal bool IsCreatureNearby(VisibleObject sprite, int dist = 12)
        {
            if (!CreatureHashSet.Contains(sprite.ID))
                return false;
            return _serverLocation.DistanceFrom(sprite.Location) <= dist;
        }

        internal void NewAisling(byte objType, int objID, ushort pursuitID)
        {
            ReplyDialog(objType, objID, pursuitID, 52);
            Thread.Sleep(500);
            ReplyDialog(objType, objID, pursuitID, 66);
            Thread.Sleep(500);
            ReplyDialog(objType, objID, pursuitID, 69);
            Thread.Sleep(500);
            ReplyDialog(objType, objID, pursuitID, 72);
            Thread.Sleep(500);
            ReplyDialog(objType, objID, pursuitID, 75);
            //this.client.Tasks.Stop();
            Thread.Sleep(3000);
            //this.client.Tasks.Start();
        }

        #region Packet methods
        internal void RequestProfile()
        {
            Enqueue(new ClientPacket(45));
        }

        internal void ServerMessage(byte type, string message)
        {
            if (!_safeScreen)
            {
                ServerPacket serverPacket = new ServerPacket(10);
                serverPacket.WriteByte(type);
                serverPacket.WriteString16(message);
                Enqueue(serverPacket);
            }
        }
        internal void ClickObject(int objectId)
        {
            ClientPacket clientPacket = new ClientPacket(67);
            clientPacket.WriteByte(1);
            clientPacket.WriteInt32(objectId);
            Enqueue(clientPacket);
        }

        internal void ServerDialog(byte dialogType, byte objectType, int objectID, byte unknown1, ushort sprite1, byte color1, byte unknown2, ushort sprite2, byte color2,
            ushort pursuitID, ushort dialogID, bool previousButton, bool nextButton, byte unknown3, string objectName, string message)
        {
            ServerPacket serverPacket = new ServerPacket(48);
            serverPacket.WriteByte(dialogType);
            serverPacket.WriteByte(objectType);
            serverPacket.WriteInt32(objectID);
            serverPacket.WriteByte(unknown1);
            serverPacket.WriteUInt16(sprite1);
            serverPacket.WriteByte(color1);
            serverPacket.WriteByte(unknown2);
            serverPacket.WriteUInt16(sprite2);
            serverPacket.WriteByte(color2);
            serverPacket.WriteUInt16(pursuitID);
            serverPacket.WriteUInt16(dialogID);
            serverPacket.WriteBoolean(previousButton);
            serverPacket.WriteBoolean(nextButton);
            serverPacket.WriteByte(unknown3);
            serverPacket.WriteString8(objectName);
            serverPacket.WriteString16(message);
            Enqueue(serverPacket);
        }

        internal void DisplayAisling(Player player)
        {
            _ = player.Location;
            ushort spriteID = player.Sprite;
            if (player == this.Player && InMonsterForm)
            {
                spriteID = _monsterFormID;
            }
            ServerPacket serverPacket = new ServerPacket(51);
            serverPacket.WriteStruct(player.Location);
            serverPacket.WriteByte((byte)player.Direction);
            serverPacket.WriteInt32(player.ID);
            if (spriteID == 0)
            {
                serverPacket.WriteUInt16(player.HeadSprite);
                if (player.BodySprite == 0 && getCheats(Cheats.SeeHidden) && !_inArena)
                {
                    serverPacket.WriteByte(80);
                }
                else
                {
                    serverPacket.WriteByte(player.BodySprite);
                }
                serverPacket.WriteUInt16(player.ArmorSprite1);
                serverPacket.WriteByte(player.BootsSprite);
                serverPacket.WriteUInt16(player.ArmorSprite2);
                serverPacket.WriteByte(player.ShieldSprite);
                serverPacket.WriteUInt16(player.WeaponSprite);
                serverPacket.WriteByte(player.HeadColor);
                serverPacket.WriteByte(player.BootColor);
                serverPacket.WriteByte(player.AccessoryColor1);
                serverPacket.WriteUInt16(player.AccessorySprite1);
                serverPacket.WriteByte(player.AccessoryColor2);
                serverPacket.WriteUInt16(player.AccessorySprite2);
                serverPacket.WriteByte(player.AccessoryColor3);
                serverPacket.WriteUInt16(player.AccessorySprite3);
                serverPacket.WriteByte(player.LanternSize);
                serverPacket.WriteByte(player.RestPosition);
                serverPacket.WriteUInt16(player.OvercoatSprite);
                serverPacket.WriteByte(player.OvercoatColor);
                serverPacket.WriteByte(player.BodyColor);
                serverPacket.WriteBoolean(player._isHidden);
                serverPacket.WriteByte(player.FaceSprite);
            }
            else
            {
                serverPacket.WriteUInt16(ushort.MaxValue);
                serverPacket.WriteUInt16((ushort)(spriteID + 16384));
                serverPacket.WriteByte(player.HeadColor);
                serverPacket.WriteByte(player.BootColor);
                serverPacket.WriteUInt16(0);
                serverPacket.WriteUInt32(0u);
            }
            serverPacket.WriteByte(player.NameTagStyle);
            if (player.BodySprite == 0 && !getCheats(Cheats.SeeHidden))
            {
                Map map = _map;
                if (map != null && map.Name?.Contains("Arena") == false && !_inArena)
                    serverPacket.WriteString8(string.Empty);
                else
                    serverPacket.WriteString8(player.Name);
            }
            serverPacket.WriteString8(player.GroupName);
            Enqueue(serverPacket);
        }

        internal void Walk(Direction dir)
        {
            if (Dialog == null && !_server._stopWalking && dir != Direction.Invalid && !_isRefreshing)
            {
                _walkSignal.Set(); // Always signal at the beginning

                LastStep = DateTime.UtcNow;

                if (_serverDirection != dir)
                {
                    Turn(dir);
                }

                ClientPacket clientPacket = new ClientPacket(6); // walk
                clientPacket.WriteByte((byte)dir);
                clientPacket.WriteByte(StepCount++);
                Enqueue(clientPacket);

                ServerPacket serverPacket = new ServerPacket(12); // creaturewalk
                serverPacket.WriteUInt32(PlayerID);
                serverPacket.WriteStruct(_clientLocation);
                serverPacket.WriteByte((byte)dir);
                Enqueue(serverPacket);

                _clientLocation.TranslateLocationByDirection(dir);
                LastMoved = DateTime.UtcNow;
            }
            else
            {
                _isWalking = false;
                Thread.Sleep(1000);
            }
        }

        internal void WalkToLocation(Location targetLocation)
        {

            _isWalking = true;
            _walkSignal.Set(); // Ensure the WalkToLocation thread starts

            // Get the target map and path directions
            var (pathDirections, targetMapID) = Pathfinding.FindPath(_server._maps[_clientLocation.MapID], _clientLocation, targetLocation);

            Task.Run(async () => // Run on a separate thread
            {
                foreach (var pathDir in pathDirections)
                {
                    _walkSignal.WaitOne(); // Wait for the signal before each step
                    Walk(pathDir);
                    await Task.Delay((int)_walkSpeed); // Use asynchronous delay
                }

                // Check if the destination is on a different map
                if (targetMapID != _clientLocation.MapID)
                {
                    // Handle map transition logic here
                    ChangeMap(targetMapID, targetLocation);
                }
            });
        }


        private void ChangeMap(short targetMapID, Location targetLocation)
        {
            if (targetMapID != _clientLocation.MapID)
            {
                // We are on a different map, find the corresponding exit
                Map currentMap = _server._maps[_clientLocation.MapID];

                if (currentMap.Exits.TryGetValue(_clientLocation.Point, out Warp exitWarp))
                {
                    // Walk to the exit warp location
                    //WalkToExitWarp(exitWarp);
                }
                else
                {
                    // Handle the case where there is no exit warp to the target map
                    // You might want to implement some fallback logic or handle this case differently
                    Console.WriteLine($"No exit warp found from MapID {_clientLocation.MapID} at Point {_clientLocation.Point} to {targetMapID}");
                }
            }

            // Update the player's location to the target location
            _clientLocation = targetLocation;
        }



        internal void Turn(Direction direction)
        {
            LastTurned = DateTime.UtcNow;
            ClientPacket clientPacket = new ClientPacket(17);
            clientPacket.WriteByte((byte)direction);
            Enqueue(clientPacket);
            _serverDirection = direction;
        }

        internal void RequestRefresh(bool waitForCompletion = true)
        {
            // Check if a refresh is already in progress
            if (_isRefreshing)
            {
                return;
            }

            // Set flag to indicate refresh is in progress
            _isRefreshing = true;

            // Enqueue the refresh request
            Enqueue(new ClientPacket(56));

            // Optionally wait for the refresh to complete
            if (waitForCompletion)
            {
                WaitForRefreshCompletion();
            }

            // Reset the flag to indicate refresh is completed
            _isRefreshing = false;
        }

        private void WaitForRefreshCompletion()
        {
            DateTime startTime = DateTime.UtcNow;

            // Wait for up to 1500 milliseconds for the refresh to complete
            while (DateTime.UtcNow.Subtract(startTime).TotalMilliseconds < 1500.0)
            {
                // Check if the refresh can proceed
                if (!_canRefresh)
                {
                    // If not, wait for a short duration before checking again
                    Thread.Sleep(10);
                    continue;
                }

                // Allow the refresh to proceed and exit the loop
                _canRefresh = false;
                break;
            }
        }

        internal void Attributes(StatUpdateFlags value, Statistics stats)
        {
            ServerPacket serverPacket = new ServerPacket(8);
            if (getCheats(Cheats.GmMode))
            {
                value |= StatUpdateFlags.GameMasterA;
            }
            if (stats.Mail != 0)
            {
                value |= (StatUpdateFlags.UnreadMail | StatUpdateFlags.Secondary);
            }
            serverPacket.WriteByte((byte)value);
            if (value.HasFlag(StatUpdateFlags.Primary))
            {
                serverPacket.Write(new byte[3]);
                serverPacket.WriteByte(stats.Level);
                serverPacket.WriteByte(stats.Ability);
                serverPacket.WriteUInt32(stats.MaximumHP);
                serverPacket.WriteUInt32(stats.MaximumMP);
                serverPacket.WriteByte(stats.CurrentStr);
                serverPacket.WriteByte(stats.CurrentInt);
                serverPacket.WriteByte(stats.CurrentWis);
                serverPacket.WriteByte(stats.CurrentCon);
                serverPacket.WriteByte(stats.CurrentDex);
                serverPacket.WriteBoolean(stats.HasUnspentPoints);
                serverPacket.WriteByte(stats.UnspentPoints);
                serverPacket.WriteInt16(stats.MaximumWeight);
                serverPacket.WriteInt16(stats.CurrentWeight);
                serverPacket.Write(new byte[4]);
            }
            if (value.HasFlag(StatUpdateFlags.Current))
            {
                serverPacket.WriteUInt32(stats.CurrentHP);
                serverPacket.WriteUInt32(stats.CurrentMP);
            }
            if (value.HasFlag(StatUpdateFlags.Experience))
            {
                serverPacket.WriteUInt32(stats.Experience);
                serverPacket.WriteUInt32(stats.ToNextLevel);
                serverPacket.WriteUInt32(stats.AbilityExperience);
                serverPacket.WriteUInt32(stats.ToNextAbility);
                serverPacket.WriteUInt32(stats.GamePoints);
                serverPacket.WriteUInt32(stats.Gold);
            }
            if (value.HasFlag(StatUpdateFlags.Secondary))
            {
                serverPacket.Write(new byte[1]);
                if (getCheats(Cheats.NoBlind) && !_inArena)
                {
                    serverPacket.WriteByte(0);
                }
                else
                {
                    serverPacket.WriteByte(stats.Blind);
                }
                serverPacket.Write(new byte[4]);
                serverPacket.WriteByte((byte)stats.OffenseElement);
                serverPacket.WriteByte((byte)stats.DefenseElement);
                serverPacket.WriteByte(stats.MagicResistance);
                serverPacket.Write(new byte[1]);
                serverPacket.WriteSByte(stats.ArmorClass);
                serverPacket.WriteByte(stats.Damage);
                serverPacket.WriteByte(stats.Hit);
            }
            Enqueue(serverPacket);
        }

        internal void ReplyDialog(byte objType, int objId, ushort pursuitId, ushort dialogId)
        {
            ClientPacket clientPacket = new ClientPacket(58);
            clientPacket.WriteByte(objType);
            clientPacket.WriteInt32(objId);
            clientPacket.WriteUInt16(pursuitId);
            clientPacket.WriteUInt16(dialogId);
            Enqueue(clientPacket);
        }

        internal void ReplyDialog(byte objType, int objId, ushort pursuitId, ushort dialogId, byte byte_10)
        {
            ClientPacket clientPacket = new ClientPacket(58);
            clientPacket.WriteByte(objType);
            clientPacket.WriteInt32(objId);
            clientPacket.WriteUInt16(pursuitId);
            clientPacket.WriteUInt16(dialogId);
            clientPacket.WriteByte(1);
            clientPacket.WriteByte(byte_10);
            Enqueue(clientPacket);
        }

        internal void ReplyDialog(byte objType, int objId, ushort pursuitId, ushort dialogId, string string_8)
        {
            ClientPacket clientPacket = new ClientPacket(58);
            clientPacket.WriteByte(objType);
            clientPacket.WriteInt32(objId);
            clientPacket.WriteUInt16(pursuitId);
            clientPacket.WriteUInt16(dialogId);
            clientPacket.WriteByte(2);
            clientPacket.WriteString8(string_8);
            Enqueue(clientPacket);
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
                            //Console.WriteLine(clientPacket.ToString());
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
                            //Console.WriteLine(serverPacket.ToString());
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


