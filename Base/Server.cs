using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talos.Cryptography;
using Talos.Networking;

namespace Talos
{
    internal class Server
    {
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
        }

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

        private void MessageHandlers()
        {
            //Client Message handlers
            ClientMessage[0] = new ClientMessageHandler(ClientMessage_0x00_JoinServer);
            ClientMessage[2] = new ClientMessageHandler(ClientMessage_0x02_CreateChar1);
            ClientMessage[3] = new ClientMessageHandler(ClientMessage_0x03_Login);
            ClientMessage[4] = new ClientMessageHandler(ClientMessage_0x04_CreateChar2);
            ClientMessage[5] = new ClientMessageHandler(ClientMessage_0x05_MapDataRequest);
            ClientMessage[6] = new ClientMessageHandler(ClientMessage_0x06_Walk);
            ClientMessage[7] = new ClientMessageHandler(ClientMessage_0x07_Pickup);
            ClientMessage[8] = new ClientMessageHandler(ClientMessage_0x08_Drop);
            ClientMessage[11] = new ClientMessageHandler(ClientMessage_0x0B_ExitRequest);
            ClientMessage[12] = new ClientMessageHandler(ClientMessage_0x0C_DisplayEntityRequest);
            ClientMessage[13] = new ClientMessageHandler(ClientMessage_0x0D_Ignore);
            ClientMessage[14] = new ClientMessageHandler(ClientMessage_0x0E_PublicMessage);
            ClientMessage[15] = new ClientMessageHandler(ClientMessage_0x0F_UseSpell);
            ClientMessage[16] = new ClientMessageHandler(ClientMessage_0x10_JoinClient);
            ClientMessage[17] = new ClientMessageHandler(ClientMessage_0x11_Turn);
            ClientMessage[19] = new ClientMessageHandler(ClientMessage_0x13_Spacebar);
            ClientMessage[24] = new ClientMessageHandler(ClientMessage_0x18_WorldListRequest);
            ClientMessage[25] = new ClientMessageHandler(ClientMessage_0x19_Whisper);
            ClientMessage[27] = new ClientMessageHandler(ClientMessage_0x1B_UserOptionToggle);
            ClientMessage[28] = new ClientMessageHandler(ClientMessage_0x1C_UseItem);
            ClientMessage[29] = new ClientMessageHandler(ClientMessage_0x1D_Emote);
            ClientMessage[36] = new ClientMessageHandler(ClientMessage_0x24_GoldDrop);
            ClientMessage[38] = new ClientMessageHandler(ClientMessage_0x26_ChangePassword);
            ClientMessage[41] = new ClientMessageHandler(ClientMessage_0x29_ItemDroppedOnCreature);
            ClientMessage[42] = new ClientMessageHandler(ClientMessage_0x2A_GoldDropOnCreature);
            ClientMessage[45] = new ClientMessageHandler(ClientMessage_0x2D_ProfileRequest);
            ClientMessage[46] = new ClientMessageHandler(ClientMessage_0x2E_GroupRequest);
            ClientMessage[47] = new ClientMessageHandler(ClientMessage_0x2F_ToggleGroup);
            ClientMessage[48] = new ClientMessageHandler(ClientMessage_0x30_SwapSlot);
            ClientMessage[56] = new ClientMessageHandler(ClientMessage_0x38_RefreshRequest);
            ClientMessage[57] = new ClientMessageHandler(ClientMessage_0x39_PursuitRequest);
            ClientMessage[58] = new ClientMessageHandler(ClientMessage_0x3A_DialogResponse);
            ClientMessage[59] = new ClientMessageHandler(ClientMessage_0x3B_BoardRequest);
            ClientMessage[62] = new ClientMessageHandler(ClientMessage_0x3E_UseSkill);
            ClientMessage[63] = new ClientMessageHandler(ClientMessage_0x3F_WorldMapClick);
            ClientMessage[67] = new ClientMessageHandler(ClientMessage_0x43_ClickObject);
            ClientMessage[68] = new ClientMessageHandler(ClientMessage_0x44_RemoveEquipment);
            ClientMessage[69] = new ClientMessageHandler(ClientMessage_0x45_KeepAlive);
            ClientMessage[71] = new ClientMessageHandler(ClientMessage_0x47_ChangeStat);
            ClientMessage[74] = new ClientMessageHandler(ClientMessage_0x4A_Exchange);
            ClientMessage[75] = new ClientMessageHandler(ClientMessage_0x4B_RequestLoginMessage);
            ClientMessage[77] = new ClientMessageHandler(ClientMessage_0x4D_BeginChant);
            ClientMessage[78] = new ClientMessageHandler(ClientMessage_0x4E_DisplayChant);
            ClientMessage[79] = new ClientMessageHandler(ClientMessage_0x4F_Profile);
            ClientMessage[87] = new ClientMessageHandler(ClientMessage_0x57_RequestServerTable);
            ClientMessage[104] = new ClientMessageHandler(ClientMessage_0x68_RequestHomePage);
            ClientMessage[117] = new ClientMessageHandler(ClientMessage_0x75_SynchronizeTicks);
            ClientMessage[121] = new ClientMessageHandler(ClientMessage_0x79_SocialStatus);
            ClientMessage[123] = new ClientMessageHandler(ClientMessage_0x7B_MetaDataRequest);

            //Server Message Handlers
            ServerMessage[0] = new ServerMessageHandler(ServerMessage_0x00_ConnectionInfo);
            ServerMessage[2] = new ServerMessageHandler(ServerMessage_0x02_LoginMessage);
            ServerMessage[3] = new ServerMessageHandler(ServerMessage_0x03_Redirect);
            ServerMessage[4] = new ServerMessageHandler(ServerMessage_0x04_Location);
            ServerMessage[5] = new ServerMessageHandler(ServerMessage_0x05_UserID);
            ServerMessage[7] = new ServerMessageHandler(ServerMessage_0x07_DisplayVisibleEntities);
            ServerMessage[8] = new ServerMessageHandler(ServerMessage_0x08_Attributes);
            ServerMessage[10] = new ServerMessageHandler(ServerMessage_0x0A_ServerMessage);
            ServerMessage[11] = new ServerMessageHandler(ServerMessage_0x0B_ClientWalk);
            ServerMessage[12] = new ServerMessageHandler(ServerMessage_0x0C_EntityWalk);
            ServerMessage[13] = new ServerMessageHandler(ServerMessage_0x0D_PublicChat);
            ServerMessage[14] = new ServerMessageHandler(ServerMessage_0x0E_RemoveObject);
            ServerMessage[15] = new ServerMessageHandler(ServerMessage_0x0F_AddItem);
            ServerMessage[16] = new ServerMessageHandler(ServerMessage_0x10_RemoveItem);
            ServerMessage[17] = new ServerMessageHandler(ServerMessage_0x11_CreatureTurn);
            ServerMessage[19] = new ServerMessageHandler(ServerMessage_0x13_HealthBar);
            ServerMessage[21] = new ServerMessageHandler(ServerMessage_0x15_MapInfo);
            ServerMessage[23] = new ServerMessageHandler(ServerMessage_0x17_AddSpell);
            ServerMessage[24] = new ServerMessageHandler(ServerMessage_0x18_RemoveSpell);
            ServerMessage[25] = new ServerMessageHandler(ServerMessage_0x19_Sound);
            ServerMessage[26] = new ServerMessageHandler(ServerMessage_0x1A_AnimateUser);
            ServerMessage[31] = new ServerMessageHandler(ServerMessage_0x1F_MapChangeComplete);
            ServerMessage[32] = new ServerMessageHandler(ServerMessage_0x20_LightLevel);
            ServerMessage[34] = new ServerMessageHandler(ServerMessage_0x22_RefreshResponse);
            ServerMessage[41] = new ServerMessageHandler(ServerMessage_0x29_Animation);
            ServerMessage[44] = new ServerMessageHandler(ServerMessage_0x2C_AddSkill);
            ServerMessage[45] = new ServerMessageHandler(ServerMessage_0x2D_RemoveSkill);
            ServerMessage[46] = new ServerMessageHandler(ServerMessage_0x2E_WorldMap);
            ServerMessage[47] = new ServerMessageHandler(ServerMessage_0x2F_MerchantMenu);
            ServerMessage[48] = new ServerMessageHandler(ServerMessage_0x30_Dialog);
            ServerMessage[49] = new ServerMessageHandler(ServerMessage_0x31_BulletinBoard);
            ServerMessage[50] = new ServerMessageHandler(ServerMessage_0x32_Door);
            ServerMessage[51] = new ServerMessageHandler(ServerMessage_0x33_DisplayUser);
            ServerMessage[52] = new ServerMessageHandler(ServerMessage_0x34_Profile);
            ServerMessage[54] = new ServerMessageHandler(ServerMessage_0x36_WorldList);
            ServerMessage[55] = new ServerMessageHandler(ServerMessage_0x37_AddEquipment);
            ServerMessage[56] = new ServerMessageHandler(ServerMessage_0x38_RemoveEquipment);
            ServerMessage[57] = new ServerMessageHandler(ServerMessage_0x39_ProfileSelf);
            ServerMessage[58] = new ServerMessageHandler(ServerMessage_0x3A_EffectsBar);
            ServerMessage[59] = new ServerMessageHandler(ServerMessage_0x3B_HeartbeatA);
            ServerMessage[60] = new ServerMessageHandler(ServerMessage_0x3C_MapData);
            ServerMessage[63] = new ServerMessageHandler(ServerMessage_0x3F_Cooldown);
            ServerMessage[66] = new ServerMessageHandler(ServerMessage_0x42_Exchange);
            ServerMessage[72] = new ServerMessageHandler(ServerMessage_0x48_CancelCasting);
            ServerMessage[73] = new ServerMessageHandler(ServerMessage_0x49_RequestPersonal);
            ServerMessage[75] = new ServerMessageHandler(ServerMessage_0x4B_ForceClientPacket);
            ServerMessage[76] = new ServerMessageHandler(ServerMessage_0x4C_ConfirmExit);
            ServerMessage[86] = new ServerMessageHandler(ServerMessage_0x56_ServerTable);
            ServerMessage[88] = new ServerMessageHandler(ServerMessage_0x58_MapLoadComplete);
            ServerMessage[96] = new ServerMessageHandler(ServerMessage_0x60_LobbyNotification);
            ServerMessage[99] = new ServerMessageHandler(ServerMessage_0x63_GroupRequest);
            ServerMessage[102] = new ServerMessageHandler(ServerMessage_0x66_LobbyControls);
            ServerMessage[103] = new ServerMessageHandler(ServerMessage_0x67_MapChangePending);
            ServerMessage[104] = new ServerMessageHandler(ServerMessage_0x68_HeartbeatB);
            ServerMessage[111] = new ServerMessageHandler(ServerMessage_0x6F_MetaFile);
            ServerMessage[126] = new ServerMessageHandler(ServerMessage_0x7E_AcceptConnection);

        }

        private bool ClientMessage_0x00_JoinServer(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x02_CreateChar1(Client client, ClientPacket clientPacket)
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

        private bool ClientMessage_0x04_CreateChar2(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x05_MapDataRequest(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x06_Walk(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x07_Pickup(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x08_Drop(Client client, ClientPacket clientPacket)
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

        private bool ClientMessage_0x24_GoldDrop(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x26_ChangePassword(Client client, ClientPacket clientPacket)
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

        private bool ClientMessage_0x44_RemoveEquipment(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x45_KeepAlive(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x47_ChangeStat(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x4A_Exchange(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x4B_RequestLoginMessage(Client client, ClientPacket clientPacket)
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

        private bool ClientMessage_0x57_RequestServerTable(Client client, ClientPacket clientPacket)
        {
            return true;
        }

        private bool ClientMessage_0x68_RequestHomePage(Client client, ClientPacket clientPacket)
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
            return true;
        }

        private bool ServerMessage_0x0A_ServerMessage(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x0B_ClientWalk(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x0C_EntityWalk(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x0D_PublicChat(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x0E_RemoveObject(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x0F_AddItem(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x10_RemoveItem(Client client, ServerPacket serverPacket)
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
            return true;
        }

        private bool ServerMessage_0x17_AddSpell(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x18_RemoveSpell(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x19_Sound(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x1A_AnimateUser(Client client, ServerPacket serverPacket)
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

        private bool ServerMessage_0x2C_AddSkill(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x2D_RemoveSkill(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x2E_WorldMap(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x2F_MerchantMenu(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x30_Dialog(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x31_BulletinBoard(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x32_Door(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x33_DisplayUser(Client client, ServerPacket serverPacket)
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

        private bool ServerMessage_0x38_RemoveEquipment(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x39_ProfileSelf(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x3A_EffectsBar(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x3B_HeartbeatA(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x3C_MapData(Client client, ServerPacket serverPacket)
        {
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

        private bool ServerMessage_0x49_RequestPersonal(Client client, ServerPacket serverPacket)
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
            return true;
        }

        private bool ServerMessage_0x60_LobbyNotification(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x63_GroupRequest(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x66_LobbyControls(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x67_MapChangePending(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x68_HeartbeatB(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x6F_MetaFile(Client client, ServerPacket serverPacket)
        {
            return true;
        }

        private bool ServerMessage_0x7E_AcceptConnection(Client client, ServerPacket serverPacket)
        {
            return true;
        }
    }
}
