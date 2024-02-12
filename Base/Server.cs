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
using Talos.Objects;
using Talos.Properties;
using Talos.Structs;
using Talos.AStar;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;


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

        const int CREATURE_SPRITE_OFFSET = 16384;

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
            LoadMapCache();

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
            ServerMessage[(int)ServerOpCode.DisplayVisibleObjects] = new ServerMessageHandler(ServerMessage_0x07_DisplayVisibleObjects);
            ServerMessage[(int)ServerOpCode.Attributes] = new ServerMessageHandler(ServerMessage_0x08_Attributes);
            ServerMessage[(int)ServerOpCode.ServerMessage] = new ServerMessageHandler(ServerMessage_0x0A_ServerMessage);
            ServerMessage[(int)ServerOpCode.ConfirmClientWalk] = new ServerMessageHandler(ServerMessage_0x0B_ConfirmClientWalk);
            ServerMessage[(int)ServerOpCode.CreatureWalk] = new ServerMessageHandler(ServerMessage_0x0C_CreatureWalk);
            ServerMessage[(int)ServerOpCode.PublicMessage] = new ServerMessageHandler(ServerMessage_0x0D_PublicMessage);
            ServerMessage[(int)ServerOpCode.RemoveVisibleObjects] = new ServerMessageHandler(ServerMessage_0x0E_RemoveVisibleObjects);
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
            client._clientLocation = client._clientLocation.TranslateLocationByDirection(facing);
            Console.WriteLine("CLIENT Location-- Map ID: " + client._clientLocation.MapID + " X,Y: " + client._clientLocation.Point);
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
            if (clientPacket.ReadByte() == 0)
                client._connected = false;

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
            string text;
            clientPacket.ReadByte();
            text = clientPacket.ReadString8();

            if (!text.StartsWith("/"))
                return true;
            //
            //ThreadPool.QueueUserWorkItem(new WaitCallback(ParseSlashCommands));
            return false;
        }

        private bool ClientMessage_0x0F_UseSpell(Client client, ClientPacket clientPacket)
        {
            byte num = clientPacket.ReadByte();
            Spell spell = client.Spellbook[num];
            if (spell != null)
            {
                client._spell = spell;
                if (clientPacket.Data.Length <= 2)
                {
                    client.CreatureTarget = null;
                }
                else
                {
                    try
                    {
                        client.CreatureTarget = client.WorldObjects[(int)clientPacket.ReadUInt32()] as Creature;
                    }
                    catch (KeyNotFoundException)
                    {
                        client.CreatureTarget = client.Player;
                    }
                }
                if (!(spell is Caster))
                {
                    return true;
                }
                Caster caster = spell as Caster;
                switch (caster.Type)
                {
                    case 1:
                        caster.SpellCastDelegate(client, 0, clientPacket.ReadString());
                        break;

                    case 2:
                        caster.SpellCastDelegate(client, clientPacket.ReadUInt32(), string.Empty);
                        break;

                    case 5:
                        caster.SpellCastDelegate(client, 0, string.Empty);
                        break;
                    default:
                        break;
                }
            }
            return false;
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
            Direction direction = (Direction)clientPacket.ReadByte();
            client._serverDirection = direction;
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
            clientPacket.ReadByte();
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
            clientPacket.ReadByte();
            clientPacket.ReadByte();
            clientPacket.ReadByte();
            return true;
        }

        private bool ClientMessage_0x38_RefreshRequest(Client client, ClientPacket clientPacket)
        {

            return true;
        }

        private bool ClientMessage_0x39_PursuitRequest(Client client, ClientPacket clientPacket)
        {
            clientPacket.ReadByte();
            clientPacket.ReadInt32();
            clientPacket.ReadUInt16();
            return true;
        }

        private bool ClientMessage_0x3A_DialogResponse(Client client, ClientPacket clientPacket)
        {
            byte objType = clientPacket.ReadByte();
            int objID = clientPacket.ReadInt32();
            ushort pursuitID = clientPacket.ReadUInt16();
            ushort dialogID = clientPacket.ReadUInt16();
            if ((client.Dialog != null) && ((client.Dialog.ObjectType == objType) && ((client.Dialog.ObjectID == objID) && ((pursuitID == 0) && (dialogID == 1)))))
                client.Dialog = null;

            return true;
        }

        private bool ClientMessage_0x3B_BoardRequest(Client client, ClientPacket clientPacket)
        {
            byte num = clientPacket.ReadByte();
            if ((num != 1) && (num == 2))
            {
                clientPacket.ReadUInt16();
                clientPacket.ReadUInt16();
            }
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
            byte num = clientPacket.ReadByte();
            if (num == 1)
            {
                int key = clientPacket.ReadInt32();
                if (client.WorldObjects.ContainsKey(key))
                    client.ClientTab.DisplayObject(client.WorldObjects[key]);
            }
            if (num == 3)
                try { clientPacket.ReadStruct(); } catch { };
            
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

            client._clientLocation.X = location.X;
            client._clientLocation.Y = location.Y;
            client._serverLocation.X = location.X;
            client._serverLocation.Y = location.Y;

            if (client.staffList.Count == 0)
                client.LoadStavesAndBows();
            if (client.meleeList.Count == 0)
                client.LoadMeleeWeapons();

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

        private bool ServerMessage_0x07_DisplayVisibleObjects(Client client, ServerPacket serverPacket)
        {
            List<Creature> creatures = new List<Creature>();
            List<Objects.Object> objects = new List<Objects.Object>();
            ushort length = serverPacket.ReadUInt16();
            int count = 0;

            try
            {
                while (true)
                {
                    if (count >= length)
                    {
                        foreach (Creature c in creatures)
                        {
                            if (!client.WorldObjects.ContainsKey(c.ID))
                            {
                                client.WorldObjects.Add(c.ID, c);
                            }
                            else
                            {
                                Creature creature = client.WorldObjects[c.ID] as Creature;
                                if (creature == null)
                                {
                                    client.WorldObjects[c.ID] = c;
                                }
                                else
                                {
                                    creature.Location = c.Location;
                                    creature.Sprite = c.Sprite;
                                    creature.Type = c.Type;
                                    creature.Direction = c.Direction;
                                    creature.Created(c.Creation);
                                }
                            }
                            if (!client.CreatureHashSet.Contains(c.ID))
                            {
                                client.CreatureHashSet.Add(c.ID);
                            }
                            if (c.Type == CreatureType.Merchant)
                            {
                                if (!client.NearbyNPC.ContainsKey(c.Name))
                                {
                                    client.NearbyNPC.Add(c.Name, c);
                                }
                                else
                                {
                                    client.NearbyNPC[c.Name] = c;
                                }
                            }
                            //else if ((client.Tasks.EnemyPage != null) && !client.Tasks.method_41(c.Sprite))
                            //{
                            //    Enemy class1 = new Enemy(c.Sprite);
                            //    class1.EnemyPage = client.Tasks.EnemyPage;
                            //    Enemy class4 = class1;
                            //    client.Tasks.method_39(class4);
                            //}
                            //else if (client.ClientTab != null)
                            //{
                            //    TabPage selectedTab;
                            //    ClientTab clientTab = client.ClientTab;
                            //    if (clientTab != null)
                            //    {
                            //        selectedTab = clientTab.monsterTabControl.SelectedTab;
                            //    }
                            //    else
                            //    {
                            //        ClientTab local1 = clientTab;
                            //        selectedTab = null;
                            //    }
                            //    if (ReferenceEquals(selectedTab, client.ClientTab.nearbyEnemyTab) && ReferenceEquals(client.ClientTab.clientTabControl.SelectedTab, client.ClientTab.mainMonstersTab))
                            //    {
                            //        client.ClientTab.AddNearbyEnemy(c);
                            //    }
                            //}
                        }

                        foreach (Objects.Object obj in objects)
                        {
                            if (client.WorldObjects.ContainsKey(obj.ID))
                            {
                                obj.Created(client.WorldObjects[obj.ID].Creation);
                                client.WorldObjects[obj.ID] = obj;
                            }
                            else if (!client.WorldObjects.ContainsKey(obj.ID))
                            {
                                client.WorldObjects[obj.ID] = obj;
                            }
                            if (!client.ObjectHashSet.Contains(obj.ID))
                            {
                                client.ObjectHashSet.Add(obj.ID);
                            }
                            if (!client.CreatureHashSet.Contains(obj.ID))
                            {
                                client.CreatureHashSet.Add(obj.ID);
                            }
                        }
                        break;
                    }

                    Point point = serverPacket.ReadStruct();
                    Location location = new Location(client._clientLocation.MapID, point);
                    int id = serverPacket.ReadInt32();
                    ushort sprite = serverPacket.ReadUInt16();
                    if (sprite >= 32768) // Is not a creature
                    {
                        serverPacket.Read(3);
                        objects.Add(new Objects.Object(id, (ushort)(sprite - 32768), location, true)); 
                    }
                    else // Is a creature
                    {
                        sprite = (ushort)(sprite - CREATURE_SPRITE_OFFSET);
                        serverPacket.Read(4);
                        byte direction = serverPacket.ReadByte();
                        serverPacket.ReadByte();
                        byte type = serverPacket.ReadByte();
                        string name = "";
                        if (type == (byte)CreatureType.Merchant)
                        {
                            name = serverPacket.ReadString8();
                        }
                        creatures.Add(new Creature(id, name, sprite, type, location, (Direction)direction));
                    }
                    count++;
                }
            }
            catch
            {
                client.RequestRefresh(false);
            }

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
                    if ((num & (byte)StatUpdateFlags.Primary) == (byte)StatUpdateFlags.Primary)
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
                    if ((num & (byte)StatUpdateFlags.Current) == (byte)StatUpdateFlags.Current)
                    {
                        stats.CurrentHP = serverPacket.ReadUInt32();
                        stats.CurrentMP = serverPacket.ReadUInt32();
                    }
                    if ((num & (byte)StatUpdateFlags.Experience) == (byte)StatUpdateFlags.Experience)
                    {
                        stats.Experience = serverPacket.ReadUInt32();
                        stats.ToNextLevel = serverPacket.ReadUInt32();
                        stats.AbilityExperience = serverPacket.ReadUInt32();
                        stats.ToNextAbility = serverPacket.ReadUInt32();
                        stats.GamePoints = serverPacket.ReadUInt32();
                        stats.Gold = serverPacket.ReadUInt32();
                    }
                    if ((num & (byte)StatUpdateFlags.Secondary) == (byte)StatUpdateFlags.Secondary)
                    {
                        serverPacket.ReadByte();
                        stats.Blind = serverPacket.ReadByte();
                        if (client.GetCheats(Cheats.NoBlind) && !client._inArena)
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
                    if (stats.Mail.HasFlag(Mail.HasParcel) && (!client.HasParcel && !client._safeScreen))
                    {
                        client.ServerMessage(0, "{=qYou have received a parcel.");
                    }
                    if ((stats.Mail.HasFlag(Mail.HasLetter) && !client.HasLetter) && !client._safeScreen)
                    {
                        client.ServerMessage(0, "{=qYou've got mail.");
                    }
                }
                catch
                {
                    return false;
                }
                client.Stats = stats;
                if (client.GetCheats(Cheats.GmMode))
                {
                    num = (byte)(num | (byte)StatUpdateFlags.GameMasterA);
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

            client._clientLocation.X = location.X;
            client._clientLocation.Y = location.Y;
            client._serverLocation.X = location.X;
            client._serverLocation.Y = location.Y;
            client._clientDirection = direction;
            client.LastMoved = DateTime.Now;

            //Console.WriteLine("SERVER Direction facing = " + client._clientDirection);
            //Console.WriteLine("SERVER Last moved = " + client.LastMoved);
            //Console.WriteLine("SERVER Client Location-- Map ID: " + client._clientLocation.MapID + " X,Y: " + client._clientLocation.Point);
            //Console.WriteLine("SERVER Server Location-- Map ID: " + client._serverLocation.MapID + " X,Y: " + client._serverLocation.Point);

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

        private bool ServerMessage_0x0E_RemoveVisibleObjects(Client client, ServerPacket serverPacket)
        {
            int id = serverPacket.ReadInt32();

            if (client.CreatureHashSet.Contains(id))
                client.CreatureHashSet.Remove(id);

            if (client.WorldObjects.ContainsKey(id))
            {
                if ((client.WorldObjects[id] is Objects.Object) && (client.WorldObjects[id] as Objects.Object).Exists)
                {
                    client.WorldObjects.Remove(id);

                    if (client.ObjectHashSet.Contains(id))
                        client.ObjectHashSet.Remove(id);
                }

                else if (!(client.WorldObjects[id] is Creature) || (client.WorldObjects[id] is Player))
                {
                    if (!(client.WorldObjects[id] is Player))
                    {
                        client.WorldObjects.Remove(id);
                    }
                    else
                    {
                        client.NearbyPlayers.Remove(client.WorldObjects[id].Name);
                        client.NearbyGhosts.Remove(client.WorldObjects[id].Name);
                        ClientTab clientTab = client.ClientTab;
                        if (clientTab != null)
                        {
                            //clientTab.UpdateStrangerList();//ADAM

                            TabPage selectedTab = clientTab.aislingTabControl.SelectedTab;

                            if (ReferenceEquals(selectedTab, clientTab.nearbyAllyTab) &&
                                ReferenceEquals(clientTab.clientTabControl.SelectedTab, clientTab.mainAislingsTab))
                            {
                                string name = client.WorldObjects[id].Name;
                                //clientTab.UpdateNearbyAllyTable(name);//ADAM
                            }
                        }
                    }
                }

                else
                {
                    Creature npc = client.WorldObjects[id] as Creature;
                    if ((npc.Type == CreatureType.Merchant) && client.NearbyNPC.ContainsKey(npc.Name))
                    {
                        client.NearbyNPC.Remove(npc.Name);
                    }

                    if (client.WorldObjects.Count == 0 ||
                        (client.WorldObjects.Count > 0 &&
                         !client.WorldObjects.Values.Any<WorldObject>(worldObject =>
                            worldObject is Creature &&
                            client.IsCreatureNearby(worldObject as VisibleObject, 12) &&
                            (worldObject as Creature).Sprite == npc.Sprite)))
                    {
                        ClientTab clientTab = client.ClientTab;

                        if (clientTab != null)
                        {
                            TabPage selectedTab = clientTab.monsterTabControl.SelectedTab;

                            if (ReferenceEquals(selectedTab, clientTab.nearbyEnemyTab) &&
                                ReferenceEquals(clientTab.clientTabControl.SelectedTab, clientTab.mainMonstersTab))
                            {
                                //client.ClientTab.UpdateNearbyEnemyTable(npc.Sprite);ADAM
                            }
                        }

                        //if (client.Tasks.EnemyPage != null && client.Tasks.method_41(npc.Sprite))
                        //{
                        //    client.Tasks.ClearEnemyLists(npc.Sprite.ToString());
                        //}
                    }
                }
            }

            return true;
        }

        private bool ServerMessage_0x0F_AddItemToPane(Client client, ServerPacket serverPacket)
        {
            byte slot = serverPacket.ReadByte();
            ushort sprite = serverPacket.ReadUInt16();
            byte color = serverPacket.ReadByte();
            string name = serverPacket.ReadString8();
            int quantity = serverPacket.ReadInt32();
            bool stackable = serverPacket.ReadBoolean();
            int maximumDurability = serverPacket.ReadInt32();
            int currentDurability = serverPacket.ReadInt32();

            if (name == "World Shout")
            {
                Item existingWorldShout = client.Inventory[name];
                if (existingWorldShout != null && quantity == existingWorldShout.Quantity - 1)
                {
                    client._lastWorldShout = DateTime.UtcNow;
                }
            }

            Item item = new Item(slot, sprite, color, name, quantity, stackable, maximumDurability, currentDurability);

            client.Inventory[slot] = item;
            return true;
        }

        private bool ServerMessage_0x10_RemoveItemFromPane(Client client, ServerPacket serverPacket)
        {
            byte num = serverPacket.ReadByte();
            if ((client.Inventory[num] != null) && (client.Inventory[num].Name == "World Shout"))
                client._lastWorldShout = DateTime.UtcNow;

            client.Inventory[num] = null;
            return true;
        }

        private bool ServerMessage_0x11_CreatureTurn(Client client, ServerPacket serverPacket)
        {
            int id = serverPacket.ReadInt32();
            Direction direction = (Direction)serverPacket.ReadByte();
            if (client.WorldObjects.ContainsKey(id))
            {
                (client.WorldObjects[id] as Creature).Direction = direction;

                Player player = client.WorldObjects[id] as Player;
                if (player != null && !client.NearbyPlayers.ContainsKey(player.Name) && !client.NearbyGhosts.ContainsKey(player.Name))
                {
                    client.NearbyGhosts.Add(player.Name, player);
                    if (client.GetCheats(Cheats.None | Cheats.SeeGhosts))
                        client.SeeGhosts(player);
                }

            }
            if (id == client.PlayerID)
            {
                client._clientDirection = direction;
                client._serverDirection = direction;
            }

            return true;
        }

        private bool ServerMessage_0x13_HealthBar(Client client, ServerPacket serverPacket)
        {
            int id = serverPacket.ReadInt32();
            byte who = serverPacket.ReadByte();
            byte percent = serverPacket.ReadByte();
            if (client.WorldObjects.ContainsKey(id)) 
            { 
                Creature creature = client.WorldObjects[id] as Creature;
                if (creature == null)
                    return true;
                if ((creature is Player) || (percent == 100) || creature.IsDioned)
                    return true;

                creature.Health = percent;
                creature._hitCounter++;
                creature.SpellAnimationHistory[114] = DateTime.MinValue;//net
                creature.SpellAnimationHistory[32] = DateTime.MinValue;//pramh
            }
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

            if (client._overrideMapFlags)
                serverPacket.Data[4] = (byte)client._mapFlags;

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
            object[] obj = new object[] { mapID };
            string path = Program.WriteMapFiles(Environment.SpecialFolder.CommonApplicationData, @"maps\lod{0}.map", obj);
            bool initialized = false;
            if (File.Exists(path))
            {
                byte[] savedMap = File.ReadAllBytes(path);
                if (CRC.Calculate(savedMap) == checksum)
                {
                    map.Initialize(savedMap);
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
                Packet[] requestMapData = new Packet[] { cp };
                client.Enqueue(requestMapData);
            }
            client._map = map;
            client._clientLocation.MapID = mapID;
            client._serverLocation.MapID = mapID;
            Console.WriteLine("Client Location Map ID: " + map.MapID);
            Console.WriteLine("Server Location Map ID: " + map.MapID);
            _maps[mapID].Tiles = client._map.Tiles;

            client._mapChangePending = false;
            client._worldMap = null;
            client._atDoor = false;
            client.Doors.Clear();
            client.NearbyPlayers.Clear();
            client.NearbyNPC.Clear();
            client.NearbyGhosts.Clear();
            client.CreatureHashSet.Clear();

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
            byte slot = serverPacket.ReadByte();
            ushort sprite = serverPacket.ReadUInt16();
            byte type = serverPacket.ReadByte();
            string name = serverPacket.ReadString8();
            string prompt = serverPacket.ReadString8();
            byte castLines = serverPacket.ReadByte();
            byte currentLevel = 0;
            byte maximumLevel = 0;
            Match match = Regex.Match(name, @"^(.*?) \([a-zA-Z]+:(\d+)/(\d+)\)$");
            if (match.Success)
            {
                name = match.Groups[1].Value;
                byte.TryParse(match.Groups[2].Value, out currentLevel);
                byte.TryParse(match.Groups[3].Value, out maximumLevel);
            }
            Spell spell = new Spell(slot, name, type, sprite, prompt, castLines, currentLevel, maximumLevel);
            client.Spellbook.AddOrUpdateSpell(spell);
            //if ((client.ClientTab != null) && ((CurrentLevel == MaximumLevel) && (client.Map.Name.Contains("Dojo") && ((client.ClientTab.toggleDojoBtn.Text == "Disable") && client.ClientTab.list_3.Contains(spell.Name)))))
            //{
            //    client.ClientTab.method_54(client.ClientTab.unmaxedSpellsGroup.Controls[spell.Name], new EventArgs());
            //    client.ClientTab.unmaxedSpellsGroup.Controls[spell.Name].Dispose();
            //}
            if (!client.AvailableSpellsAndCastLines.ContainsKey(name))
            {
                client.AvailableSpellsAndCastLines.Add(name, castLines);
            }
            return true;
        }

        private bool ServerMessage_0x18_RemoveSpellFromPane(Client client, ServerPacket serverPacket)
        {
            byte slot = serverPacket.ReadByte();
            client.Spellbook.RemoveSpell(slot);
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
            client._mapChangePending = false;
            client._lastMapChange = DateTime.UtcNow;
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
            byte slot = serverPacket.ReadByte();
            ushort sprite = serverPacket.ReadUInt16();
            string name = serverPacket.ReadString8();
            byte currentLevel = 0;
            byte maximumLevel = 0;
            Match match = Regex.Match(name, @"^(.*?) \([a-zA-Z]+:(\d+)/(\d+)\)$");
            if (match.Success)
            {
                name = match.Groups[1].Value;
                byte.TryParse(match.Groups[2].Value, out currentLevel);
                byte.TryParse(match.Groups[3].Value, out maximumLevel);
            }
            Skill skill = new Skill(slot, name, sprite, currentLevel, maximumLevel);
            //if ((client.ClientTab != null) && ((currentLevel == maximumLevel) && (client.Map.Name.Contains("Dojo") && ((client.ClientTab.toggleDojoBtn.Text == "Disable") && client.ClientTab.list_2.Contains(skill.Name)))))
            //{
            //    client.ClientTab.method_53(client.ClientTab.unmaxedSkillsGroup.Controls[skill.Name], new EventArgs());
            //    client.ClientTab.unmaxedSkillsGroup.Controls[skill.Name].Dispose();
            //}
            client.Skillbook.AddOrUpdateSkill(skill);
            return true;
        }

        private bool ServerMessage_0x2D_RemoveSkillFromPane(Client client, ServerPacket serverPacket)
        {
            byte num = serverPacket.ReadByte();
            client.Skillbook.RemoveSkill(num);
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
                Location loc = client._clientLocation;
                loc.TranslateLocationByDirection(client._clientDirection);
                _maps[client._map.MapID].WorldMaps[loc.Point] = newWorldMap;
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
            bool dialogSuccess;
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
                    int objID = serverPacket.ReadInt32();
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

                    if (npcDialog == "You are going to be arrested for sleep-fighting ((Auto Hunting)) in five minutes unless you state that you do not wish to.")
                    {
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(method_0));
                    }
                    if (npcDialog != "You have already sent a world shout in the last 5 minutes.")
                    {
                        client.ClientTab.UpdateNpcInfo(npcDialog, dialogID, pursuitID);
                        client._npcDialog = npcDialog;
                        int num12 = dialogType - 2;
                        if (num12 != 0)
                        {
                            if (num12 == 2)
                            {
                                topCaption = serverPacket.ReadString8();
                                inputLength = serverPacket.ReadByte();
                                bottomCaption = serverPacket.ReadString8();
                            }
                        }
                        else
                        {
                            byte length = serverPacket.ReadByte();
                            for (int i = 0; i < length; i++)
                            {
                                options.Add(serverPacket.ReadString8());
                            }
                        }
                        client.Dialog = new Dialog(dialogType, objType, objID, unknown, sprite1, color1, unknown2, sprite2, color2, pursuitID, dialogID, prevButton, nextButton, unknown3, objName, npcDialog, options, topCaption, inputLength, bottomCaption, client);
                        if ((npcDialog == "Do you wish to go back to Chaos 1?") || (npcDialog == "You are about to enter a hostile area. Do you wish to proceed?"))
                        {
                            client.Dialog.DialogNext(1);
                            return false;
                        }
                        if (npcDialog == "Lying in bed, your eyes become heavy. The cold and weary day has finally caught up to you. Do you want to sleep?")
                        {
                            client.ReplyDialog(objType, objID, pursuitID, (ushort)(dialogID + 1), 1);
                            client.ReplyDialog(objType, objID, pursuitID, (ushort)(dialogID + 1));
                            client.ReplyDialog(objType, objID, pursuitID, (ushort)(dialogID + 1));
                            return false;
                        }
                        if ((npcDialog == "((You are entering a role-playing town where role-playing is strictly enforced. Please do not speak words that are not appropriate to the theme of Dark Ages. Any act of heresy can result in a temporary banishment from the town.))") || (npcDialog == "Congratulations in finding a Golden Starfish!"))
                        {
                            client.Dialog.DialogNext();
                            return false;
                        }
                        if (npcDialog == "You throw the Braided Vine around the rocks hoping it gets lodged in between them.")
                        {
                            client.ReplyDialog(objType, objID, pursuitID, (ushort)(dialogID + 1));
                            client.ReplyDialog(objType, objID, pursuitID, (ushort)(dialogID + 1));
                            client.ReplyDialog(objType, objID, pursuitID, (ushort)(dialogID + 1));
                            return false;
                        }
                        if ((npcDialog == "The Braided Vine unravels from the rocks and you loose your footing.") || (npcDialog == "I don't think this is the correct order for the story. I better look at the other Wall Tablets."))
                        {
                            client.Dialog.Reply();
                            return false;
                        }
                        if (npcDialog == "The Braided Vine starts to unravel, but you quickly jumped to dry land.")
                        {
                            client.ReplyDialog(objType, objID, pursuitID, (ushort)(dialogID + 1));
                            client.ReplyDialog(objType, objID, pursuitID, (ushort)(dialogID + 1));
                            return false;
                        }
                        if (npcDialog == "*The Yowien Guard sniffs you, then suddenly attacks you. You run back to a safe distance*")
                        {
                            client.ClientTab.walkBtn.Text = "Walk";
                        }
                        else if (npcDialog == "This will reset your labor to one hour. You can only do this once.")
                        {
                            client._hasLabor = true;
                        }
                        else if (npcDialog == "Will you be respectful of the meditations and not reveal the secrets of meditation?")
                        {
                            client.Dialog.DialogNext(2);
                        }
                        else if (client.ClientTab.toggleHubaeBtn.Text == "Disable")
                        {
                            string str5;
                            if (npcDialog == "Welcome to Dark Ages : Online Roleplaying. This tutorial will give you the facts and skills you need to begin.")
                            {
                                for (int i = 0; i < 6; i++)
                                {
                                    client.ReplyDialog(objType, objID, pursuitID, (ushort)(dialogID + 1));
                                }
                                Thread.Sleep(1000);
                                return false;
                            }
                            if (npcDialog == "You are about to leave the tutorial. Heres a hint when starting, speak with all the mundanes (merchants) in Mileth (the starting town). Remember, this is an online world, you set your own goals and missions for your character.")
                            {
                                client.ReplyDialog(objType, objID, pursuitID, 2);
                                client.ReplyDialog(objType, objID, pursuitID, 3);
                                client.ReplyDialog(objType, objID, pursuitID, 11, 1);
                                client.ReplyDialog(objType, objID, pursuitID, 0x13);
                                client.ReplyDialog(objType, objID, pursuitID, 0x31);
                                client.ReplyDialog(objType, objID, pursuitID, 0x6d, 2);
                                client.ReplyDialog(objType, objID, pursuitID, 0x2a);
                                Thread.Sleep(0x3e8);
                                client.Dialog = null;
                                return false;
                            }
                            if (npcDialog == "You rub your eyes...")
                            {
                                ThreadPool.QueueUserWorkItem(state =>
                                {
                                    // Cast the state parameter back to the appropriate types
                                    var tuple = (Tuple<byte, int, ushort>)state;
                                    client.NewAisling(tuple.Item1, tuple.Item2, tuple.Item3);
                                },
                                Tuple.Create(client, objType, objID, pursuitID));
                                return false;
                            }
                            if (npcDialog == "You legs are the strongest.  You enemies will be scattered.")
                            {
                                //client.Tasks.bool_36 = true;
                                return false;
                            }
                            if (npcDialog == "Over that left door, you can be born a Monk.  You will be strong in working with the Nature.  You will save the people in danger, and only you will be able to increase other peoples' mana.")
                            {
                                //client.Tasks.bool_37 = true;
                                return false;
                            }
                            //if (!client.Tasks.bool_40 && client.Tasks.dictionary_3.TryGetValue(npcDialog, out str5))
                            //{
                            //    int index = 2147483647;
                            //    index = options.IndexOf(str5);
                            //    if (index != 2147483647)
                            //    {
                            //        client.Dialog.DialogNext((byte)(index + 1));
                            //    }
                            //}
                            //else
                            //{
                            //    string str6;
                            //    if (client.Tasks.bool_40 && client.Tasks.dictionary_4.TryGetValue(npcDialog, out str6))
                            //    {
                            //        int index = 2147483647;
                            //        index = options.IndexOf(str6);
                            //        if (index != 2147483647)
                            //        {
                            //            client.Dialog.DialogNext((byte)(index + 1));
                            //        }
                            //    }
                            //}
                        }
                        if (objType == 2)
                        {
                            client.ObjectID[objName] = objID;
                        }
                        return true;//may be return false but it freezes on dialog...
                    }
                    else
                    {
                        TimeSpan span = new TimeSpan(0, 5, 0) - DateTime.UtcNow.Subtract(client._lastWorldShout);
                        client.ServerDialog(dialogType, objType, objID, unknown, sprite1, color1, unknown2, sprite2, color2, pursuitID, dialogID, false, false, unknown3, objName, "You may World Shout again in " + ((client._lastWorldShout != DateTime.MinValue) ? (((span.Minutes > 1) ? $"{span.Minutes} minutes and " : ((span.Minutes > 0) ? $"{span.Minutes} minutes and " : string.Empty)) + $"{span.Seconds} seconds.") : "unkown value."));
                        dialogSuccess = false;
                    }
                }
                else
                {
                    client.Dialog = null;
                    dialogSuccess = true;
                }
            }
            catch
            {
                dialogSuccess = false;
            }

            return dialogSuccess;

        }

        private bool ServerMessage_0x31_Board(Client client, ServerPacket serverPacket)
        {
            byte num = serverPacket.ReadByte();
            serverPacket.ReadByte();
            if (num.Equals((byte)2))
            {
                serverPacket.ReadUInt16();
                serverPacket.ReadString8();
                byte num2 = serverPacket.ReadByte();
                for (int i = 0; i < num2; i++)
                {
                    serverPacket.Read(1);
                    serverPacket.ReadUInt16();
                    serverPacket.ReadString8();
                    serverPacket.ReadByte();
                    serverPacket.ReadByte();
                    serverPacket.ReadString8();
                }
            }
            if (num.Equals((byte)3))
            {
                serverPacket.ReadUInt16();
                serverPacket.ReadByte();
                serverPacket.ReadString8();
                serverPacket.ReadByte();
                serverPacket.ReadByte();
                serverPacket.ReadString8();
                serverPacket.ReadString16();
            }
            return true;
        }

        private bool ServerMessage_0x32_Door(Client client, ServerPacket serverPacket)
        {
            try
            {
                byte length = serverPacket.ReadByte();
                for (int i = 0; i < length; i++)
                {
                    client._atDoor = true;
                    byte x = serverPacket.ReadByte();
                    byte y = serverPacket.ReadByte();
                    bool closed = serverPacket.ReadBoolean();
                    bool openedRight = serverPacket.ReadBoolean();

                    Location currentLocation = new Location(client._map.MapID, x, y);
                    Console.WriteLine($"Door found at: {client._map.MapID}, {x},{y}, closed: {closed}, openedRight: {openedRight}");
                    Door door = new Door(currentLocation, closed);
                    if (!client.Doors.Contains(door))
                    {
                        client.Doors.Add(door);
                    }
                }
            }
            catch
            {
            }
            return true;
        }

        private bool ServerMessage_0x33_DisplayAisling(Client client, ServerPacket serverPacket)
        {
            bool isHidden = false;
            ushort armorSprite1 = 0;
            ushort armorSprite2 = 0;
            ushort weaponSprite = 0;
            ushort accessorySprite1 = 0;
            ushort accessorySprite2 = 0;
            ushort accessorySprite3 = 0;
            ushort overcoatSprite = 0;
            ushort form = 0;
            byte bodySprite = 0;
            byte bootsSprite = 0;
            byte shieldSprite = 0;
            byte headColor = 0;
            byte bootsColor = 0;
            byte accessoryColor = 0;
            byte accessorryColor2 = 0;
            byte accessoryColor3 = 0;
            byte lanternSize = 0;
            byte restPosition = 0;
            byte overcoatColor = 0;
            byte bodyColor = 0;
            byte faceSprite = 0;
            byte nameTagStyle = 0;
            string groupName = "";
            string name = "";


            Point point = serverPacket.ReadStruct();
            Location location = new Location(client._clientLocation.MapID, point);  
            Direction direction = (Direction)serverPacket.ReadByte();
            int id = serverPacket.ReadInt32();
            ushort headSprite = serverPacket.ReadUInt16();
            if (headSprite == 65535)
            {
                form = (ushort)(serverPacket.ReadUInt16() - CREATURE_SPRITE_OFFSET);
                headColor = serverPacket.ReadByte();
                bootsColor = serverPacket.ReadByte();
                serverPacket.Read(6);
            }
            else
            {
                bodySprite = serverPacket.ReadByte();
                if ((bodySprite == 0) && (client.GetCheats(Cheats.None | Cheats.SeeHidden) && !client._inArena))
                {
                    serverPacket.Position--;
                    serverPacket.WriteByte(80);
                }
                armorSprite1 = serverPacket.ReadUInt16();
                bootsSprite = serverPacket.ReadByte();
                armorSprite2 = serverPacket.ReadUInt16();
                shieldSprite = serverPacket.ReadByte();
                weaponSprite = serverPacket.ReadUInt16();
                headColor = serverPacket.ReadByte();
                bootsColor = serverPacket.ReadByte();
                accessoryColor = serverPacket.ReadByte();
                accessorySprite1 = serverPacket.ReadUInt16();
                accessorryColor2 = serverPacket.ReadByte();
                accessorySprite2 = serverPacket.ReadUInt16();
                accessoryColor3 = serverPacket.ReadByte();
                accessorySprite3 = serverPacket.ReadUInt16();
                lanternSize = serverPacket.ReadByte();
                restPosition = serverPacket.ReadByte();
                overcoatSprite = serverPacket.ReadUInt16();
                overcoatColor = serverPacket.ReadByte();
                bodyColor = serverPacket.ReadByte();
                isHidden = serverPacket.ReadBoolean() || (bodySprite == 0);
                faceSprite = serverPacket.ReadByte();
            }
            nameTagStyle = serverPacket.ReadByte();
            name = serverPacket.ReadString8();
            groupName = serverPacket.ReadString8();
            if ((bodySprite == 0) && (client.WorldObjects.ContainsKey(id) && (client.GetCheats(Cheats.None | Cheats.SeeHidden) && !client._inArena)))
            {
                serverPacket.Position -= (name.Length + groupName.Length) + 3;
                nameTagStyle = ((id == client.PlayerID) || (form != 0)) ? nameTagStyle : ((byte)3);
                serverPacket.WriteByte(nameTagStyle);
                serverPacket.WriteString8(client.WorldObjects[id].Name);
                serverPacket.WriteString8(groupName);
            }
            Player player = new Player(id, name, location, direction);
            if (!string.IsNullOrEmpty(player.Name))
            {
                var playersToRemove = client.WorldObjects
                    .Values
                    .OfType<Player>()
                    .Where(p => p.Name.Equals(player.Name) && p.ID != id)
                    .ToList();

                foreach (var playerToRemove in playersToRemove)
                {
                    client.WorldObjects.Remove(playerToRemove.ID);
                }
            }
            if (!client.WorldObjects.ContainsKey(id))
            {
                client.WorldObjects[id] = player;
                if (string.IsNullOrEmpty(name))
                    client.ClickObject(id);
            }
            else if (!(client.WorldObjects[id] is Player))
                client.WorldObjects[id] = player;
            else
            {
                Player p = client.WorldObjects[id] as Player;
                p.Location = location;
                p.Direction = direction;
                if (string.IsNullOrEmpty(p.Name))
                    p.Name = name;
            }
            player.Sprite = form;
            player.BodySprite = bodySprite;
            player.ArmorSprite1 = armorSprite1;
            player.ArmorSprite2 = armorSprite2;
            player.BootColor = bootsColor;
            player.LanternSize = lanternSize;
            player.RestPosition = restPosition;
            player.OvercoatColor = overcoatColor;
            player._isHidden = isHidden;
            player.NameTagStyle = nameTagStyle;
            player.GroupName = groupName;
            if ((bodySprite == 0) || ((id == client.PlayerID) || client._inArena))
            {
                player.HeadSprite = headSprite;
                player.BootsSprite = bootsSprite;
                player.ShieldSprite = shieldSprite;
                player.WeaponSprite = weaponSprite;
                player.HeadColor = headColor;
                player.AccessoryColor1 = accessoryColor;
                player.AccessorySprite1 = accessorySprite1;
                player.AccessoryColor2 = accessorryColor2;
                player.AccessorySprite2 = accessorySprite2;
                player.AccessoryColor3 = accessoryColor3;
                player.AccessorySprite3 = accessorySprite3;
                player.OvercoatSprite = overcoatSprite;
                player.BodyColor = bodyColor;
                player.FaceSprite = faceSprite;
            }
            if (string.IsNullOrEmpty(player.Name))
            {
                //client.playersWithNoName[id] = player;
            }
            else
            {
                //if (client.playersWithNoName.ContainsKey(id))
                //{
                //    client.playersWithNoName.Remove(id);
                //}
                //client.Dictionary_7[player.Name] = player;
                client.NearbyPlayers[player.Name] = player;
                //client.dictGhostList.Remove(player.Name);
            }
            if (id == client.PlayerID)
            {
                client.Player = player;
                client._clientDirection = direction;
                client._clientLocation = location;
                if (client.InMonsterForm)
                {
                    serverPacket.Clear();
                    serverPacket.WriteStruct(location);
                    serverPacket.WriteByte((byte)direction);
                    serverPacket.WriteUInt32((uint)id);
                    serverPacket.WriteUInt16(65535);//maxvalue
                    serverPacket.WriteUInt16((ushort)(client._monsterFormID + CREATURE_SPRITE_OFFSET));
                    serverPacket.WriteByte(headColor);
                    serverPacket.WriteByte(bootsColor);
                    serverPacket.Write(new byte[6]);
                    serverPacket.WriteByte(nameTagStyle);
                    serverPacket.WriteString8(name);
                    serverPacket.WriteString8(groupName);
                }
            }

            client.DisplayAisling(player);

            return true;
        }

        /// <summary>
        /// Message received when a player profile is requested, including legend details
        /// </summary>
        /// <param name="client"></param>
        /// <param name="serverPacket"></param>
        /// <returns></returns>
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
            ushort effect = serverPacket.ReadUInt16();
            if (serverPacket.ReadByte() != 0)
            {
                if (!client.EffectsBarHashSet.Contains(effect))
                {
                    client.EffectsBarHashSet.Add(effect);
                }
            }
            else if (client.EffectsBarHashSet.Contains(effect))
            {
                client.EffectsBarHashSet.Remove(effect);
                if ((effect == 19) && !client._inArena)//bday or incapacitate
                {
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(client.ReEquipItem));//ADAM
                }
            }
            return true;
        }

        private bool ServerMessage_0x3B_HeartbeatResponse(Client client, ServerPacket serverPacket)
        {
            ClientPacket clientPacket = new ClientPacket(0x45);
            clientPacket.WriteByte(serverPacket.ReadByte());
            clientPacket.WriteByte(serverPacket.ReadByte());
            Packet[] classArray1 = new Packet[] { clientPacket };
            client.Enqueue(classArray1);
            return false;
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
            byte slot = serverPacket.ReadByte();
            uint ticks = serverPacket.ReadUInt32();
            if (serverPacket.ReadByte() == 0)
            {
                Spell spell = client.Spellbook[slot];
                if (spell != null)
                {
                    spell.Cooldown = DateTime.UtcNow;
                    spell.Ticks = ticks;
                }
            }
            else
            {
                Skill skill = client.Skillbook[slot];
                if (skill != null)
                {
                    skill.Cooldown = DateTime.UtcNow;
                    skill.Ticks = ticks;
                }
            }
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
                client._canRefresh = true;

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
            if (serverPacket.ReadByte() == 3)
            {
                ServerPacket newServerPacket = new ServerPacket(0x66);
                newServerPacket.WriteByte(4);
                serverPacket.WriteByte(1);
                Packet[] toSend = new Packet[] { newServerPacket };
                client.Enqueue(toSend);
            }
            return true;
        }

        private bool ServerMessage_0x67_MapChangePending(Client client, ServerPacket serverPacket)
        {
            client._mapChangePending = true;
            return true;
        }

        private bool ServerMessage_0x68_SynchronizeTicks(Client client, ServerPacket serverPacket)
        {
            ClientPacket clientPacket = new ClientPacket(0x75);
            clientPacket.WriteInt32(serverPacket.ReadInt32());
            clientPacket.WriteInt32(Environment.TickCount);
            Packet[] toSend = new Packet[] { clientPacket };
            client.Enqueue(toSend);
            return false;
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
            Pathfinding.SetMaps(this._maps);
            binaryReader.Close();
            return true;
        }

    }
}

