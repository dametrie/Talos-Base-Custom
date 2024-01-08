using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Talos.Cryptography;
using Talos.Cryptography.Abstractions.Definitions;
using Talos.Enumerations;
using Talos.Forms;
using Talos.Maps;
using Talos.Networking;
using Talos.Player;
using Talos.Properties;
using Talos.Structs;

namespace Talos
{
    internal class Server
    {
        #region networking vars
        private Socket _clientSocket;

        private IPEndPoint _remoteEndPoint;
        internal int _port { get; set; }

        private ClientMessageHandler[] _clientMessage;
        internal ClientMessageHandler[] ClientMessage => _clientMessage;

        private ServerMessageHandler[] _serverMessage;
        internal ServerMessageHandler[] ServerMessage => _serverMessage;

        internal MainForm _mainForm;

        private Thread _serverThread;

        internal List<Client> _clientList;

        private bool _initialized;
        #endregion

        internal Dictionary<uint, WorldMap> _worldMaps = new Dictionary<uint, WorldMap>();
        internal Dictionary<short, Map> _maps = new Dictionary<short, Map>();
        private bool _isMapping;
        internal bool _stopWalking;

        public static object Lock { get; internal set; } = new object();

        internal Server(MainForm mainForm)
        {
            _mainForm = mainForm;
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientMessage = new ClientMessageHandler[256];
            _serverMessage = new ServerMessageHandler[256];
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse("52.88.55.94"), 2610);
            _clientList = new List<Client>();
            MessageHandlers();
            Initialize(2610);
            //LoadMapCache();

        }

        #region networking

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        internal void Initialize(int port)
        {
            if (!_initialized)
            {
                _initialized = true;
                _port = port;
                _clientSocket.Bind(new IPEndPoint(IPAddress.Loopback, port));
                _clientSocket.Listen(10);
                _clientSocket.BeginAccept(new AsyncCallback(EndAccept), null);
                //_serverThread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void EndAccept(IAsyncResult ar)
        {
            try
            {
                Client client = new Client(this, _clientSocket.EndAccept(ar));
                client.Connect(_remoteEndPoint);
                _remoteEndPoint = new IPEndPoint(IPAddress.Parse("52.88.55.94"), 2610);
                _clientSocket.BeginAccept(new AsyncCallback(EndAccept), null);
                _clientList.Add(client);
            }
            catch (SocketException)
            {
                Console.WriteLine("Failed to establish a connection: Client");
            }
        }
        #endregion

        /// <summary>
        /// Message handler for the client and server messages based on what opcode is sent.
        /// </summary>
        private void MessageHandlers()
        {
            #region Client Message Handlers
            ClientMessage[(int)ClientOpCode.Version] = new ClientMessageHandler(ClientMessage_0x00_Version);
            ClientMessage[(int)ClientOpCode.CreateCharRequest] = new ClientMessageHandler(ClientMessage_0x02_CreateCharRequest);
            ClientMessage[(int)ClientOpCode.Login] = new ClientMessageHandler(ClientMessage_0x03_Login);
            ClientMessage[(int)ClientOpCode.CreateCharFinalize] = new ClientMessageHandler(ClientMessage_0x04_CreateCharFinalize);
            ClientMessage[(int)ClientOpCode.MapDataRequest] = new ClientMessageHandler(ClientMessage_0x05_MapDataRequest);
            ClientMessage[(int)ClientOpCode.ClientWalk] = new ClientMessageHandler(ClientMessage_0x06_ClientWalk);
            ClientMessage[(int)ClientOpCode.Pickup] = new ClientMessageHandler(ClientMessage_0x07_Pickup);
            ClientMessage[(int)ClientOpCode.ItemDrop] = new ClientMessageHandler(ClientMessage_0x08_ItemDrop);
            ClientMessage[(int)ClientOpCode.ExitRequest] = new ClientMessageHandler(ClientMessage_0x0B_ExitRequest);
            ClientMessage[(int)ClientOpCode.DisplayEntityRequest] = new ClientMessageHandler(ClientMessage_0x0C_DisplayEntityRequest);
            ClientMessage[(int)ClientOpCode.Ignore] = new ClientMessageHandler(ClientMessage_0x0D_Ignore);
            ClientMessage[(int)ClientOpCode.PublicMessage] = new ClientMessageHandler(ClientMessage_0x0E_PublicMessage);
            ClientMessage[(int)ClientOpCode.UseSpell] = new ClientMessageHandler(ClientMessage_0x0F_UseSpell);
            ClientMessage[(int)ClientOpCode.ClientJoin] = new ClientMessageHandler(ClientMessage_0x10_JoinClient);
            ClientMessage[(int)ClientOpCode.Turn] = new ClientMessageHandler(ClientMessage_0x11_Turn);
            ClientMessage[(int)ClientOpCode.SpaceBar] = new ClientMessageHandler(ClientMessage_0x13_Spacebar);
            ClientMessage[(int)ClientOpCode.WorldListRequest] = new ClientMessageHandler(ClientMessage_0x18_WorldListRequest);
            ClientMessage[(int)ClientOpCode.Whisper] = new ClientMessageHandler(ClientMessage_0x19_Whisper);
            ClientMessage[(int)ClientOpCode.UserOptionToggle] = new ClientMessageHandler(ClientMessage_0x1B_UserOptionToggle);
            ClientMessage[(int)ClientOpCode.UseItem] = new ClientMessageHandler(ClientMessage_0x1C_UseItem);
            ClientMessage[(int)ClientOpCode.Emote] = new ClientMessageHandler(ClientMessage_0x1D_Emote);
            ClientMessage[(int)ClientOpCode.SetNotepad] = new ClientMessageHandler(ClientMessage_0x23_SetNotepad);
            ClientMessage[(int)ClientOpCode.GoldDrop] = new ClientMessageHandler(ClientMessage_0x24_GoldDrop);
            ClientMessage[(int)ClientOpCode.PasswordChange] = new ClientMessageHandler(ClientMessage_0x26_PasswordChange);
            ClientMessage[(int)ClientOpCode.ItemDroppedOnCreature] = new ClientMessageHandler(ClientMessage_0x29_ItemDroppedOnCreature);
            ClientMessage[(int)ClientOpCode.GoldDroppedOnCreature] = new ClientMessageHandler(ClientMessage_0x2A_GoldDropOnCreature);
            ClientMessage[(int)ClientOpCode.ProfileRequest] = new ClientMessageHandler(ClientMessage_0x2D_ProfileRequest);
            ClientMessage[(int)ClientOpCode.GroupRequest] = new ClientMessageHandler(ClientMessage_0x2E_GroupRequest);
            ClientMessage[(int)ClientOpCode.ToggleGroup] = new ClientMessageHandler(ClientMessage_0x2F_ToggleGroup);
            ClientMessage[(int)ClientOpCode.SwapSlot] = new ClientMessageHandler(ClientMessage_0x30_SwapSlot);
            ClientMessage[(int)ClientOpCode.RefreshRequest] = new ClientMessageHandler(ClientMessage_0x38_RefreshRequest);
            ClientMessage[(int)ClientOpCode.PursuitRequest] = new ClientMessageHandler(ClientMessage_0x39_PursuitRequest);
            ClientMessage[(int)ClientOpCode.DialogResponse] = new ClientMessageHandler(ClientMessage_0x3A_DialogResponse);
            ClientMessage[(int)ClientOpCode.BoardRequest] = new ClientMessageHandler(ClientMessage_0x3B_BoardRequest);
            ClientMessage[(int)ClientOpCode.UseSkill] = new ClientMessageHandler(ClientMessage_0x3E_UseSkill);
            ClientMessage[(int)ClientOpCode.WorldMapClick] = new ClientMessageHandler(ClientMessage_0x3F_WorldMapClick);
            ClientMessage[(int)ClientOpCode.ClickObject] = new ClientMessageHandler(ClientMessage_0x43_ClickObject);
            ClientMessage[(int)ClientOpCode.Unequip] = new ClientMessageHandler(ClientMessage_0x44_Unequip);
            ClientMessage[(int)ClientOpCode.HeartBeat] = new ClientMessageHandler(ClientMessage_0x45_HeartBeat);
            ClientMessage[(int)ClientOpCode.RaiseStat] = new ClientMessageHandler(ClientMessage_0x47_RaiseStat);
            ClientMessage[(int)ClientOpCode.Exchange] = new ClientMessageHandler(ClientMessage_0x4A_Exchange);
            ClientMessage[(int)ClientOpCode.NoticeRequest] = new ClientMessageHandler(ClientMessage_0x4B_NoticeRequest);
            ClientMessage[(int)ClientOpCode.BeginChant] = new ClientMessageHandler(ClientMessage_0x4D_BeginChant);
            ClientMessage[(int)ClientOpCode.DisplayChant] = new ClientMessageHandler(ClientMessage_0x4E_DisplayChant);
            ClientMessage[(int)ClientOpCode.Profile] = new ClientMessageHandler(ClientMessage_0x4F_Profile);
            ClientMessage[(int)ClientOpCode.ServerTableRequest] = new ClientMessageHandler(ClientMessage_0x57_ServerTableRequest);
            ClientMessage[(int)ClientOpCode.SequenceChange] = new ClientMessageHandler(ClientMessage_0x62_SequenceChange);
            ClientMessage[(int)ClientOpCode.HomePageRequest] = new ClientMessageHandler(ClientMessage_0x68_HomePageRequest);
            ClientMessage[(int)ClientOpCode.SynchronizeTicks] = new ClientMessageHandler(ClientMessage_0x75_SynchronizeTicks);
            ClientMessage[(int)ClientOpCode.SocialStatus] = new ClientMessageHandler(ClientMessage_0x79_SocialStatus);
            ClientMessage[(int)ClientOpCode.MetaDataRequest] = new ClientMessageHandler(ClientMessage_0x7B_MetaDataRequest);
            #endregion

            #region Server Message Handlers
            ServerMessage[(int)ServerOpCode.ConnectionInfo] = new ServerMessageHandler(ServerMessage_0x00_ConnectionInfo);
            ServerMessage[(int)ServerOpCode.LoginMessage] = new ServerMessageHandler(ServerMessage_0x02_LoginMessage);
            ServerMessage[(int)ServerOpCode.Redirect] = new ServerMessageHandler(ServerMessage_0x03_Redirect);
            ServerMessage[(int)ServerOpCode.Location] = new ServerMessageHandler(ServerMessage_0x04_Location);
            ServerMessage[(int)ServerOpCode.UserID] = new ServerMessageHandler(ServerMessage_0x05_UserID);
            ServerMessage[(int)ServerOpCode.DisplayVisibleEntities] = new ServerMessageHandler(ServerMessage_0x07_DisplayVisibleEntities);
            ServerMessage[(int)ServerOpCode.Attributes] = new ServerMessageHandler(ServerMessage_0x08_Attributes);
            ServerMessage[(int)ServerOpCode.ServerMessage] = new ServerMessageHandler(ServerMessage_0x0A_ServerMessage);
            ServerMessage[(int)ServerOpCode.ConfirmClientWalk] = new ServerMessageHandler(ServerMessage_0x0B_ConfirmClientWalk);
            ServerMessage[(int)ServerOpCode.CreatureWalk] = new ServerMessageHandler(ServerMessage_0x0C_CreatureWalk);
            ServerMessage[(int)ServerOpCode.PublicMessage] = new ServerMessageHandler(ServerMessage_0x0D_PublicMessage);
            ServerMessage[(int)ServerOpCode.RemoveObject] = new ServerMessageHandler(ServerMessage_0x0E_RemoveObject);
            ServerMessage[(int)ServerOpCode.AddItemToPane] = new ServerMessageHandler(ServerMessage_0x0F_AddItemToPane);
            ServerMessage[(int)ServerOpCode.RemoveItemFromPane] = new ServerMessageHandler(ServerMessage_0x10_RemoveItemFromPane);
            ServerMessage[(int)ServerOpCode.CreatureTurn] = new ServerMessageHandler(ServerMessage_0x11_CreatureTurn);
            ServerMessage[(int)ServerOpCode.HealthBar] = new ServerMessageHandler(ServerMessage_0x13_HealthBar);
            ServerMessage[(int)ServerOpCode.MapInfo] = new ServerMessageHandler(ServerMessage_0x15_MapInfo);
            ServerMessage[(int)ServerOpCode.AddSpellToPane] = new ServerMessageHandler(ServerMessage_0x17_AddSpellToPane);
            ServerMessage[(int)ServerOpCode.RemoveSpellFromPane] = new ServerMessageHandler(ServerMessage_0x18_RemoveSpellFromPane);
            ServerMessage[(int)ServerOpCode.Sound] = new ServerMessageHandler(ServerMessage_0x19_Sound);
            ServerMessage[(int)ServerOpCode.BodyAnimation] = new ServerMessageHandler(ServerMessage_0x1A_BodyAnimation);
            ServerMessage[(int)ServerOpCode.Notepad] = new ServerMessageHandler(ServerMessage_0x1B_NotePad);
            ServerMessage[(int)ServerOpCode.MapChangeComplete] = new ServerMessageHandler(ServerMessage_0x1F_MapChangeComplete);
            ServerMessage[(int)ServerOpCode.LightLevel] = new ServerMessageHandler(ServerMessage_0x20_LightLevel);
            ServerMessage[(int)ServerOpCode.RefreshResponse] = new ServerMessageHandler(ServerMessage_0x22_RefreshResponse);
            ServerMessage[(int)ServerOpCode.Animation] = new ServerMessageHandler(ServerMessage_0x29_Animation);
            ServerMessage[(int)ServerOpCode.AddSkillToPane] = new ServerMessageHandler(ServerMessage_0x2C_AddSkillToPane);
            ServerMessage[(int)ServerOpCode.RemoveSkillFromPane] = new ServerMessageHandler(ServerMessage_0x2D_RemoveSkillFromPane);
            ServerMessage[(int)ServerOpCode.WorldMap] = new ServerMessageHandler(ServerMessage_0x2E_WorldMap);
            ServerMessage[(int)ServerOpCode.MerchantMenu] = new ServerMessageHandler(ServerMessage_0x2F_MerchantMenu);
            ServerMessage[(int)ServerOpCode.Dialog] = new ServerMessageHandler(ServerMessage_0x30_Dialog);
            ServerMessage[(int)ServerOpCode.Board] = new ServerMessageHandler(ServerMessage_0x31_Board);
            ServerMessage[(int)ServerOpCode.Door] = new ServerMessageHandler(ServerMessage_0x32_Door);
            ServerMessage[(int)ServerOpCode.DisplayAisling] = new ServerMessageHandler(ServerMessage_0x33_DisplayAisling);
            ServerMessage[(int)ServerOpCode.Profile] = new ServerMessageHandler(ServerMessage_0x34_Profile);
            ServerMessage[(int)ServerOpCode.WorldList] = new ServerMessageHandler(ServerMessage_0x36_WorldList);
            ServerMessage[(int)ServerOpCode.AddEquipment] = new ServerMessageHandler(ServerMessage_0x37_AddEquipment);
            ServerMessage[(int)ServerOpCode.Unequip] = new ServerMessageHandler(ServerMessage_0x38_Unequip);
            ServerMessage[(int)ServerOpCode.SelfProfile] = new ServerMessageHandler(ServerMessage_0x39_SelfProfle);
            ServerMessage[(int)ServerOpCode.Effect] = new ServerMessageHandler(ServerMessage_0x3A_Effect);
            ServerMessage[(int)ServerOpCode.HeartBeatResponse] = new ServerMessageHandler(ServerMessage_0x3B_HeartbeatResponse);
            ServerMessage[(int)ServerOpCode.MapData] = new ServerMessageHandler(ServerMessage_0x3C_MapData);
            ServerMessage[(int)ServerOpCode.Cooldown] = new ServerMessageHandler(ServerMessage_0x3F_Cooldown);
            ServerMessage[(int)ServerOpCode.Exchange] = new ServerMessageHandler(ServerMessage_0x42_Exchange);
            ServerMessage[(int)ServerOpCode.CancelCasting] = new ServerMessageHandler(ServerMessage_0x48_CancelCasting);
            ServerMessage[(int)ServerOpCode.ProfileRequest] = new ServerMessageHandler(ServerMessage_0x49_ProfileRequest);
            ServerMessage[(int)ServerOpCode.ForceClientPacket] = new ServerMessageHandler(ServerMessage_0x4B_ForceClientPacket);
            ServerMessage[(int)ServerOpCode.ConfirmExit] = new ServerMessageHandler(ServerMessage_0x4C_ConfirmExit);
            ServerMessage[(int)ServerOpCode.ServerTable] = new ServerMessageHandler(ServerMessage_0x56_ServerTable);
            ServerMessage[(int)ServerOpCode.MapLoadComplete] = new ServerMessageHandler(ServerMessage_0x58_MapLoadComplete);
            ServerMessage[(int)ServerOpCode.LoginNotice] = new ServerMessageHandler(ServerMessage_0x60_LoginNotice);
            ServerMessage[(int)ServerOpCode.GroupRequest] = new ServerMessageHandler(ServerMessage_0x63_GroupRequest);
            ServerMessage[(int)ServerOpCode.LoginControls] = new ServerMessageHandler(ServerMessage_0x66_LoginControls);
            ServerMessage[(int)ServerOpCode.MapChangePending] = new ServerMessageHandler(ServerMessage_0x67_MapChangePending);
            ServerMessage[(int)ServerOpCode.SynchronizeTicks] = new ServerMessageHandler(ServerMessage_0x68_SynchronizeTicks);
            ServerMessage[(int)ServerOpCode.MetaData] = new ServerMessageHandler(ServerMessage_0x6F_MetaData);
            ServerMessage[(int)ServerOpCode.AcceptConnection] = new ServerMessageHandler(ServerMessage_0x7E_AcceptConnection);
            #endregion
        }

        #region Client Messages

        private bool ClientMessage_0x00_Version(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x02_CreateCharRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x03_Login(Client client, ClientPacket clientPacket)
        {
            string login = clientPacket.ReadString8();
            string password = clientPacket.ReadString8();

            //fluff?
            clientPacket.ReadByte();
            clientPacket.ReadByte();
            clientPacket.ReadUInt32();
            clientPacket.ReadUInt16();
            clientPacket.ReadUInt32();
            clientPacket.ReadUInt16();
            clientPacket.ReadByte();
            return true;
        }

        private bool ClientMessage_0x04_CreateCharFinalize(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x05_MapDataRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x06_ClientWalk(Client client, ClientPacket clientPacket)
        {
            Direction facing = (Direction)clientPacket.ReadByte();
            Console.WriteLine("CLIENT Direction facing = " + facing);
            clientPacket.ReadByte();
            byte stepCount = client.StepCount;
            client.StepCount = stepCount++;
            clientPacket.Data[1] = stepCount;
            client._clientLocation.GetDirection(facing);
            Console.WriteLine("CLIENT Location facing = " + client._clientLocation);
            client.LastStep = DateTime.Now;
            Console.WriteLine("CLIENT Last step = " + client.LastStep);
            client._isCasting = false;
            client.LastMoved = DateTime.Now;
            Console.WriteLine("CLIENT Last moved = " + client.LastMoved);
            return true;
        }

        private bool ClientMessage_0x07_Pickup(Client client, ClientPacket clientPacket)
        {
            clientPacket.ReadByte();
            clientPacket.ReadStruct();
            return true;
        }

        private bool ClientMessage_0x08_ItemDrop(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x0B_ExitRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x0C_DisplayEntityRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x0D_Ignore(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x0E_PublicMessage(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x0F_UseSpell(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x10_JoinClient(Client client, ClientPacket clientPacket)
        {
            byte seed = clientPacket.ReadByte();
            string key = clientPacket.ReadString8();
            string name = clientPacket.ReadString8();
            uint id = clientPacket.ReadUInt32();
            client.Name = name;
            client._crypto = new Crypto(seed, key, name);
            return true;
        }

        private bool ClientMessage_0x11_Turn(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x13_Spacebar(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x18_WorldListRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x19_Whisper(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x1B_UserOptionToggle(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x1C_UseItem(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x1D_Emote(Client client, ClientPacket clientPacket)
        {
            return true;
        }
        private bool ClientMessage_0x23_SetNotepad(Client client, ClientPacket clientPacket)
        {
            return true;
        }
        private bool ClientMessage_0x24_GoldDrop(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x26_PasswordChange(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x29_ItemDroppedOnCreature(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x2A_GoldDropOnCreature(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x2D_ProfileRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x2E_GroupRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x2F_ToggleGroup(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x30_SwapSlot(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x38_RefreshRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x39_PursuitRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x3A_DialogResponse(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x3B_BoardRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x3E_UseSkill(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x3F_WorldMapClick(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x43_ClickObject(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x44_Unequip(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x45_HeartBeat(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x47_RaiseStat(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x4A_Exchange(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x4B_NoticeRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x4D_BeginChant(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x4E_DisplayChant(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x4F_Profile(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x57_ServerTableRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }
        private bool ClientMessage_0x62_SequenceChange(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x68_HomePageRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x75_SynchronizeTicks(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x79_SocialStatus(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x7B_MetaDataRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }
        #endregion

        #region Server Messages
        private bool ServerMessage_0x00_ConnectionInfo(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x02_LoginMessage(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x03_Redirect(Client client, ServerPacket serverPacket)
        {
            byte[] array = serverPacket.Read(4);
            short port = serverPacket.ReadInt16();
            serverPacket.ReadByte();
            serverPacket.ReadByte();
            serverPacket.Read(serverPacket.ReadByte());
            serverPacket.ReadString8();
            serverPacket.ReadInt32();
            Array.Reverse(array);
            _remoteEndPoint = new IPEndPoint(new IPAddress(array), port);
            serverPacket.Data[0] = 1;
            serverPacket.Data[1] = 0;
            serverPacket.Data[2] = 0;
            serverPacket.Data[3] = 127;
            serverPacket.Data[4] = (byte)(_port / 256);
            serverPacket.Data[5] = (byte)(_port % 256);
            return true;
        }

        private bool ServerMessage_0x04_Location(Client client, ServerPacket serverPacket)
        {
            Point location;
            try { location = serverPacket.ReadStruct(); } catch { return false; }

            client._clientLocation = location;
            client._serverLocation = location;

            return true;
        }

        private bool ServerMessage_0x05_UserID(Client client, ServerPacket serverPacket)
        {
            uint id = serverPacket.ReadUInt32();
            serverPacket.Read(2);
            byte path = serverPacket.ReadByte();
            client.PlayerID = id;
            client.Path = path;
            client.RequestProfile();
            _mainForm.AddClient(client);
            return true;
        }

        private bool ServerMessage_0x07_DisplayVisibleEntities(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x08_Attributes(Client client, ServerPacket serverPacket)
        {
            {
                byte num;
                Statistics stats = client.Stats;
                try
                {
                    num = serverPacket.ReadByte();
                    if ((num & 32) == 32)//StatUpdateFlags.Primary
                    {
                        serverPacket.Read(3);
                        stats.Level = serverPacket.ReadByte();
                        stats.Ability = serverPacket.ReadByte();
                        stats.MaximumHP = serverPacket.ReadUInt32();
                        stats.MaximumMP = serverPacket.ReadUInt32();
                        stats.CurrentStr = serverPacket.ReadByte();
                        stats.CurrentInt = serverPacket.ReadByte();
                        stats.CurrentWis = serverPacket.ReadByte();
                        stats.CurrentCon = serverPacket.ReadByte();
                        stats.CurrentDex = serverPacket.ReadByte();
                        stats.HasUnspentPoints = serverPacket.ReadBoolean();
                        stats.UnspentPoints = serverPacket.ReadByte();
                        stats.MaximumWeight = serverPacket.ReadInt16();
                        stats.CurrentWeight = serverPacket.ReadInt16();
                        serverPacket.Read(4);
                    }
                    if ((num & 16) == 16)//StatUpdateFlags.Current
                    {
                        stats.CurrentHP = serverPacket.ReadUInt32();
                        stats.CurrentMP = serverPacket.ReadUInt32();
                    }
                    if ((num & 8) == 8)//StatUpdateFlags.Experience
                    {
                        stats.Experience = serverPacket.ReadUInt32();
                        stats.ToNextLevel = serverPacket.ReadUInt32();
                        stats.AbilityExperience = serverPacket.ReadUInt32();
                        stats.ToNextAbility = serverPacket.ReadUInt32();
                        stats.GamePoints = serverPacket.ReadUInt32();
                        stats.Gold = serverPacket.ReadUInt32();
                    }
                    if ((num & 4) == 4)//StatUpdateFlags.Secondary
                    {
                        serverPacket.ReadByte();
                        stats.Blind = serverPacket.ReadByte();
                        if (client.getCheats(Cheats.NoBlind) && !client.inArena)
                        {
                            serverPacket.Position--;
                            serverPacket.WriteByte(0);
                        }
                        serverPacket.Read(3);
                        stats.Mail = (Mail)serverPacket.ReadByte();
                        stats.OffenseElement = (Element)serverPacket.ReadByte();
                        stats.DefenseElement = (Element)serverPacket.ReadByte();
                        stats.MagicResistance = serverPacket.ReadByte();
                        serverPacket.ReadByte();
                        stats.ArmorClass = serverPacket.ReadSByte();
                        stats.Damage = serverPacket.ReadByte();
                        stats.Hit = serverPacket.ReadByte();
                    }
                    if (stats.Mail.HasFlag(Mail.HasParcel) && (!client.HasParcel && !client.safeScreen))
                    {
                        client.ServerMessage(0, "{=qYou have received a parcel.");
                    }
                    if ((stats.Mail.HasFlag(Mail.HasLetter) && !client.HasLetter) && !client.safeScreen)
                    {
                        client.ServerMessage(0, "{=qYou've got mail.");
                    }
                }
                catch
                {
                    return false;
                }
                client.Stats = stats;
                if (client.getCheats(Cheats.GmMode))
                {
                    num = (byte)(num | 64);//StatUpdateFlags.GameMasterA
                    serverPacket.Data[0] = num;
                }
                if (client.ClientTab != null)
                {
                    //client.ClientTab.DisplaySessionStats();
                    client.ClientTab.DisplayHPMP();
                    Console.WriteLine("Health: " + stats.CurrentHP);
                    Console.WriteLine("Mana: " + stats.CurrentMP);
                }
                client.Attributes((StatUpdateFlags)num, stats);
                return false;
            }
        }

        private bool ServerMessage_0x0A_ServerMessage(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x0B_ConfirmClientWalk(Client client, ServerPacket serverPacket)
        {
            Point location;
            Direction direction = (Direction)serverPacket.ReadByte();
            try { location = serverPacket.ReadStruct().TranslatePointByDirection(direction); } catch { return false; }

            client._serverLocation = location;
            client._clientLocation = location;
            client._clientDirection = direction;
            client.LastMoved = DateTime.Now;

            Console.WriteLine("SERVER Direction facing = " + client._clientDirection);
            Console.WriteLine("SERVER Location facing = " + client._serverLocation);
            Console.WriteLine("SERVER Last moved = " + client.LastMoved);
            Console.WriteLine("SERVER client location = " + client._clientLocation);
            Console.WriteLine("SERVER server location = " + client._serverLocation);    

            return true;
        }

        private bool ServerMessage_0x0C_CreatureWalk(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x0D_PublicMessage(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x0E_RemoveObject(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x0F_AddItemToPane(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x10_RemoveItemFromPane(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x11_CreatureTurn(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x13_HealthBar(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x15_MapInfo(Client client, ServerPacket serverPacket)
        {
            short mapID = serverPacket.ReadInt16();
            byte sizeX = serverPacket.ReadByte();
            byte sizeY = serverPacket.ReadByte();
            byte flags = serverPacket.ReadByte();
            byte[] buffer = serverPacket.Read(2);
            ushort checksum = serverPacket.ReadUInt16();
            string name = serverPacket.ReadString8();

            Map map = new Map(mapID, sizeX, sizeY, flags, checksum, name);  

            if (_maps.TryGetValue(map.MapID, out map) && (map.Checksum == checksum))
            {
                map.Checksum = flags;
                map.Name = name;
            }
            else
            {
                map = new Map(mapID, sizeX, sizeY, flags, checksum, name);
                if (!_maps.ContainsKey(map.MapID))
                {
                    _maps.Add(map.MapID, map);

                }
            }
            if (_maps.ContainsKey(mapID))
            {
                Map temp = _maps[mapID];
                temp.Checksum = flags;
                map = temp;
            }
            object[] args = new object[] { mapID };
            string path = Program.WriteMapFiles(Environment.SpecialFolder.CommonApplicationData, @"maps\lod{0}.map", args);
            bool initialized = false;
            if (File.Exists(path))
            {
                byte[] buffer2 = File.ReadAllBytes(path);
                if (CRC.Calculate(buffer2) == checksum)
                {
                    map.Initialize(buffer2);
                    initialized = true;
                }
            }
            if (!initialized)
            {
                File.Delete(path);
                ClientPacket cp = new ClientPacket(5);//RequestMapData
                cp.WriteUInt32(0);
                cp.WriteByte(sizeX);
                cp.WriteByte(sizeY);
                cp.WriteUInt16(0);
                cp.WriteByte(0);
                Packet[] classArray1 = new Packet[] { cp };
                client.Enqueue(classArray1);
            }
            client._map = map;
            _maps[mapID].Tiles = client._map.Tiles;

            client._worldMap = null;

            Console.WriteLine("Map ID: " + map.MapID);
            Console.WriteLine("Map Name: " + map.Name);
            Console.WriteLine("Map Checksum: " + map.Checksum);
            Console.WriteLine("Map SizeX: " + map.SizeX);
            Console.WriteLine("Map SizeY: " + map.SizeY);
            Console.WriteLine("Map Flags: " + map.Flags);
            Console.WriteLine("Map tiles: " + client._map.Tiles);

            ServerPacket sp = new ServerPacket(21);
            sp.WriteInt16(mapID);
            sp.WriteByte(sizeX);
            sp.WriteByte(sizeY);
            sp.WriteByte(flags);
            sp.Write(buffer);
            sp.WriteUInt16(checksum);
            sp.WriteString8(map.Name);
            Packet[] array = new Packet[] { sp };
            client.Enqueue(array);
            return false;
        }

        private bool ServerMessage_0x17_AddSpellToPane(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x18_RemoveSpellFromPane(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x19_Sound(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x1A_BodyAnimation(Client client, ServerPacket serverPacket)
        {
            return true;
        }
        private bool ServerMessage_0x1B_NotePad(Client client, ServerPacket packet)
        {
            return true;
        }

        private bool ServerMessage_0x1F_MapChangeComplete(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x20_LightLevel(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x22_RefreshResponse(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x29_Animation(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x2C_AddSkillToPane(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x2D_RemoveSkillFromPane(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x2E_WorldMap(Client client, ServerPacket serverPacket)
        {
            string field = serverPacket.ReadString8();
            byte nodeCount = serverPacket.ReadByte();
            byte image = serverPacket.ReadByte();
            WorldMap worldMap = new WorldMap(field, new WorldMapNode[0]);
            for (int i = 0; i < nodeCount; i++)
            {

                Point position = serverPacket.ReadStruct();
                string name = serverPacket.ReadString8();
                int checksum = serverPacket.ReadInt16();
                short mapID = serverPacket.ReadInt16();
                Point spawnPoint = serverPacket.ReadStruct();
                worldMap.Nodes.Add(new WorldMapNode(position, name, mapID, spawnPoint));
            }
            client._worldMap = worldMap;
            uint crc = worldMap.GetCRC32();
            WorldMap newWorldMap = worldMap;
            _worldMaps[crc] = newWorldMap;
            if (_isMapping && (client._atDoor && _maps.ContainsKey(client._map.MapID)))
            {
                Point loc = client._clientLocation;
                loc.GetDirection(client._clientDirection);
                _maps[client._map.MapID].WorldMaps[loc] = newWorldMap;
            }
            client._atDoor = false;
            return true;
        }

        private bool ServerMessage_0x2F_MerchantMenu(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x30_Dialog(Client client, ServerPacket serverPacket)
        {
            bool flag3;
            byte inputLength = 0;
            string topCaption = string.Empty;
            string bottomCaption = string.Empty;
            List<string> options = new List<string>();
            try
            {
                byte dialogType = serverPacket.ReadByte();
                if (dialogType != 10)
                {
                    byte objType = serverPacket.ReadByte();
                    int objId = serverPacket.ReadInt32();
                    byte unknown = serverPacket.ReadByte();
                    ushort sprite1 = serverPacket.ReadUInt16();
                    byte color1 = serverPacket.ReadByte();
                    byte unknown2 = serverPacket.ReadByte();
                    ushort sprite2 = serverPacket.ReadUInt16();
                    byte color2 = serverPacket.ReadByte();
                    ushort pursuitID = serverPacket.ReadUInt16();
                    ushort dialogID = serverPacket.ReadUInt16();
                    bool prevButton = serverPacket.ReadBoolean();
                    bool nextButton = serverPacket.ReadBoolean();
                    byte unknown3 = serverPacket.ReadByte();
                    string objName = serverPacket.ReadString8();
                    int num1 = serverPacket.Position;
                    string npcDialog = serverPacket.ReadString16();
                    int num17 = serverPacket.Position;

                    client.ClientTab.UpdateNpcInfo(npcDialog, dialogID, pursuitID);
                    //client.ClientTab.npcText.Text = npcDialog;
                    //client.ClientTab.dialogIdLbl.Text = "Dialog ID:" + dialogID.ToString();
                    //client.ClientTab.pursuitLbl.Text = "Pursuit ID:" + pursuitID.ToString();
                    client._npcDialog = npcDialog;

                }
                else
                {
                    client.Dialog = null;
                    flag3 = true;
                }
            }
            catch
            {
                flag3 = false;
            }

            return true;
        }

        private bool ServerMessage_0x31_Board(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x32_Door(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x33_DisplayAisling(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x34_Profile(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x36_WorldList(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x37_AddEquipment(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x38_Unequip(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x39_SelfProfle(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x3A_Effect(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x3B_HeartbeatResponse(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        /// <summary>
        /// Packet sent to client whenever a map is not in the Darkages folder.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serverPacket"></param>
        /// <returns></returns>
        private bool ServerMessage_0x3C_MapData(Client client, ServerPacket serverPacket)
        {
            short num = serverPacket.ReadInt16();
            object[] mapID = new object[] { client._map.MapID };
            string path = Program.WriteMapFiles(System.Environment.SpecialFolder.CommonApplicationData, @"maps\lod{0}.map", mapID);
            FileStream output = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            output.Seek((long)((client._map.SizeX * 6) * num), SeekOrigin.Begin);
            BinaryWriter writer = new BinaryWriter(output);
            for (short i = 0; i < client._map.SizeX; i = (short)(i + 1))
            {
                writer.Write(serverPacket.ReadInt16());
                writer.Write(serverPacket.ReadInt16());
                writer.Write(serverPacket.ReadInt16());
            }
            writer.Close();
            if (num == (client._map.SizeY - 1))
            {
                byte[] buffer = File.ReadAllBytes(path);
                if (CRC.Calculate(buffer) == client._map.Checksum)
                {
                    client._map.Initialize(buffer);
                }
            }
            return true;
        }

        private bool ServerMessage_0x3F_Cooldown(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x42_Exchange(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x48_CancelCasting(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x49_ProfileRequest(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x4B_ForceClientPacket(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x4C_ConfirmExit(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x56_ServerTable(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x58_MapLoadComplete(Client client, ServerPacket serverPacket)
        {
            if (client._isRefreshing)
            {
                client._canRefresh = true;
            }
            return true;
        }

        private bool ServerMessage_0x60_LoginNotice(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x63_GroupRequest(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x66_LoginControls(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x67_MapChangePending(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x68_SynchronizeTicks(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x6F_MetaData(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x7E_AcceptConnection(Client client, ServerPacket serverPacket)
        {
            return true;
        }
        #endregion

        /// <summary>
        /// Function to load the map cache from the resources maps.dat
        /// Currently unused because this is being handled by the application called MapCacheEditor
        /// </summary>
        /// <returns></returns>
        private bool LoadMapCache()
        {
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(Resources.maps));
            binaryReader.ReadInt32();

            short worldMapCount = binaryReader.ReadInt16();
            for (int index1 = 0; index1 < (int)worldMapCount; ++index1)
            {
                WorldMap worldMap = new WorldMap(binaryReader.ReadString(), new WorldMapNode[0]);
                byte num2 = binaryReader.ReadByte();
                for (int index2 = 0; index2 < (int)num2; ++index2)
                {
                    short x1 = binaryReader.ReadInt16();
                    short y1 = binaryReader.ReadInt16();
                    string name = binaryReader.ReadString();
                    short mapId = binaryReader.ReadInt16();
                    byte x2 = binaryReader.ReadByte();
                    byte y2 = binaryReader.ReadByte();
                    worldMap.Nodes.Add(new WorldMapNode(new Point(x1, y1), name, mapId, new Point(x2, y2)));
                }
                this._worldMaps[worldMap.GetCRC32()] = worldMap;
            }

            short mapCount = binaryReader.ReadInt16();
            for (int index3 = 0; index3 < (int)mapCount; ++index3)
            {
                try
                {
                    short sourceMapId = binaryReader.ReadInt16();
                    byte sizeX = binaryReader.ReadByte();
                    byte sizeY = binaryReader.ReadByte();
                    string name = binaryReader.ReadString();
                    byte flags = binaryReader.ReadByte();
                    sbyte music = binaryReader.ReadSByte();
                    Map map = new Map(sourceMapId, sizeX, sizeY, flags, name, music);
                    short num5 = binaryReader.ReadInt16();
                    for (int index4 = 0; index4 < (int)num5; ++index4)
                    {
                        byte sourceX = binaryReader.ReadByte();
                        byte sourceY = binaryReader.ReadByte();
                        short targetMapId = binaryReader.ReadInt16();
                        byte targetX = binaryReader.ReadByte();
                        byte targetY = binaryReader.ReadByte();
                        Warp warp = new Warp(sourceX, sourceY, targetX, targetY, sourceMapId, targetMapId);
                        map.Exits[new Point((short)sourceX, (short)sourceY)] = warp;
                    }
                    byte num8 = binaryReader.ReadByte();
                    for (int index5 = 0; index5 < (int)num8; ++index5)
                    {
                        byte x = binaryReader.ReadByte();
                        byte y = binaryReader.ReadByte();
                        uint key = binaryReader.ReadUInt32();
                        if (this._worldMaps.ContainsKey(key))
                            map.WorldMaps[new Point((short)x, (short)y)] = this._worldMaps[key];
                    }
                    this._maps.Add(sourceMapId, map);
                }
                catch
                {
                }
            }
            binaryReader.Close();
            return true;
        }

    }
}

