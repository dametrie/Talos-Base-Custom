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
        internal DateTime _lastWorldShout = DateTime.MinValue;
        internal DateTime _lastMapChange = DateTime.MinValue;
        internal Map _map;
        internal WorldMap _worldMap;
        internal Cheats _cheats;
        internal Direction _clientDirection;
        internal Direction _serverDirection;
        internal MapFlags _mapFlags;
        internal string _npcDialog;
        internal bool _safeScreen;
        internal bool _inArena = false;
        internal bool _atDoor;
        internal bool _isCasting;
        internal bool _isWalking;
        internal bool _isRefreshing;
        internal bool _canRefresh;
        internal bool _hasLabor;
        internal bool _bool_39;
        internal bool _overrideMapFlags;
        internal bool _mapChangePending;
        internal double _walkSpeed = 420.0;
        internal ushort _monsterFormID = 1;

        internal AutoResetEvent _walkSignal = new AutoResetEvent(false); 
        
        internal List<Staff> staffList = new List<Staff>();

        internal List<MeleeWeapon> meleeList = new List<MeleeWeapon>();

        internal List<Bow> bowList = new List<Bow>();

        internal Dictionary<int, WorldObject> WorldObjects { get; private set; } = new Dictionary<int, WorldObject>();
        internal Dictionary<string, Player> NearbyPlayers { get; private set; } = new Dictionary<string, Player>();
        internal Dictionary<string, Player> NearbyGhosts{ get; private set; } = new Dictionary<string, Player>();
        internal Dictionary<string, Creature> NearbyNPC { get; private set; } = new Dictionary<string, Creature>();
        internal Dictionary<string, int> ObjectID { get; private set; } = new Dictionary<string, int>();
        internal Dictionary<string, byte> AvailableSpellsAndCastLines { get; set; } = new Dictionary<string, byte>();
       
        internal HashSet<int> CreatureHashSet { get; private set; } = new HashSet<int>();
        internal HashSet<int> ObjectHashSet { get; private set; } = new HashSet<int>();
        internal HashSet<ushort> EffectsBarHashSet { get; set; } = new HashSet<ushort>();
        internal HashSet<Door> Doors { get; private set; } = new HashSet<Door> { };


        internal Thread WalkThread { get; set; }    
        internal ClientTab ClientTab { get; set; }
        internal Statistics Stats { get; set; }
        internal Dialog Dialog { get; set; }
        internal Inventory Inventory { get; set; } = new Inventory(60);
        internal Spellbook Spellbook { get; set; } = new Spellbook();
        internal Skillbook Skillbook { get; set; } = new Skillbook();
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


        }


        internal void Remove()
        {
            ClientTab.RemoveClient();
            ClientTab = null;
        }

        internal bool GetCheats(Cheats value)
        {
            return _cheats.HasFlag(value);
        }

        internal void SeeGhosts(Player player)
        {
            player.NakedPlayer();
            player.HeadColor = 1;
            player.BodySprite = 48;
            DisplayAisling(player);
            DisplayEntityRequest(player.ID);
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

        internal void LoadMeleeWeapons()
        {
            meleeList.Add(new MeleeWeapon("Stick", 0, 1, false, true));
            meleeList.Add(new MeleeWeapon("Oak Stick", 0, 1, false, true));
            meleeList.Add(new MeleeWeapon("Dirk", 0, 2, false, true));
            meleeList.Add(new MeleeWeapon("Eppe", 0, 2, false, true));
            meleeList.Add(new MeleeWeapon("Loures Saber", 0, 7, false, true));
            meleeList.Add(new MeleeWeapon("Harpoon", 0, 11, false, true));
            meleeList.Add(new MeleeWeapon("Hatchet", 0, 13, false, true));
            meleeList.Add(new MeleeWeapon("Claidheamh", 0, 14, false, true));
            meleeList.Add(new MeleeWeapon("Broad Sword", 0, 17, false, true));
            meleeList.Add(new MeleeWeapon("Dragon Scale Sword", 0, 20, false, true));
            meleeList.Add(new MeleeWeapon("Wooden Club", 0, 50, false, true));
            meleeList.Add(new MeleeWeapon("Stone Axe", 0, 95, false, true));
            meleeList.Add(new MeleeWeapon("Amber Saber", 0, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Diamond Saber", 0, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Emerald Saber", 0, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Ruby Saber", 0, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Sapphire Saber", 0, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Master Falcata", 0, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Master Saber", 0, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Eclipse", 0, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Crystal Saber", 65, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Yowien Hatchet", 80, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Defiled Ruby Saber", 95, 99, true, true, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Two-Handed Claidhmore", 0, 71, false, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Two-Handed Emerald Sword", 0, 77, false, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Two-Handed Gladius", 0, 86, false, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Two-Handed Kindjal", 0, 90, false, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Giant Stone Axe", 0, 93, false, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Giant Stone Club", 0, 95, false, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Giant Stone Hammer", 0, 97, false, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Two-Handed Dragon Slayer", 0, 97, false, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Hy-brasyl Battle Axe", 0, 99, false, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Gold Kindjal", 0, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Hy-brasy Escalon", 0, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Empowered Escalon", 0, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Enchanted Escalon", 0, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Master Battle Axe", 0, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Dane Blade", 1, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Astion Blade", 8, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Brune Blade", 15, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Veltain Sword", 15, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Tempered Veltain Sword", 15, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Tuned Veltain Sword", 15, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Blazed Veltain Sword", 15, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Grum Blade", 22, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Andor Saber", 30, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Hwarone Guandao", 45, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Empowered Hwarone Guandao", 55, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Hellreavers Blade", 90, 99, true, false, TemuairClass.Warrior));
            meleeList.Add(new MeleeWeapon("Blackstar Night Claw", 95, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Eagles Grasp", 90, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Yowien's Fist", 80, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Yowien's Fist1", 80, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Yowien's Claw", 65, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Yowien's Claw1", 65, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Stone Fists", 0, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Phoenix Claws", 0, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Enchanted Kalkuri", 0, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Obsidian", 0, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Nunchaku", 0, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Tilian Claw", 0, 99, true, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Wolf Claw", 0, 50, false, true, TemuairClass.Monk));
            meleeList.Add(new MeleeWeapon("Wolf Claws", 0, 50, false, true, TemuairClass.Monk));
            foreach (Item item in new List<Item>(Inventory))
            {
                if (meleeList.Any((MeleeWeapon melee) => melee.Name == item.Name))
                {
                    item.IsMeleeWeapon = true;
                }
            }
        }

        internal void LoadStavesAndBows()
        {
            Dictionary<string, byte> minusOneLine = new Dictionary<string, byte>(AvailableSpellsAndCastLines);
            Dictionary<string, byte> minusTwoLines = new Dictionary<string, byte>(AvailableSpellsAndCastLines);
            Dictionary<string, byte> masterOneLine = new Dictionary<string, byte>(AvailableSpellsAndCastLines);
            Dictionary<string, byte> oneLine = new Dictionary<string, byte>(AvailableSpellsAndCastLines);
            Dictionary<string, byte> masterPriest = new Dictionary<string, byte>(AvailableSpellsAndCastLines);
            Dictionary<string, byte> cradhZeroLine = new Dictionary<string, byte>(AvailableSpellsAndCastLines);
            Dictionary<string, byte> fourLinesBecome2Lines = new Dictionary<string, byte>(AvailableSpellsAndCastLines);
            Dictionary<string, byte> cradhOneLine = new Dictionary<string, byte>(AvailableSpellsAndCastLines);
            foreach (KeyValuePair<string, byte> item in new List<KeyValuePair<string, byte>>(minusOneLine))
            {
                if (item.Value <= 1)
                {
                    minusOneLine[item.Key] = 0;
                }
                else
                {
                    minusOneLine[item.Key]--;
                }
            }
            foreach (KeyValuePair<string, byte> item2 in new List<KeyValuePair<string, byte>>(minusTwoLines))
            {
                if (item2.Value <= 2)
                {
                    minusTwoLines[item2.Key] = 0;
                }
                else
                {
                    minusTwoLines[item2.Key] -= 2;
                }
            }
            foreach (KeyValuePair<string, byte> item3 in new List<KeyValuePair<string, byte>>(masterOneLine))
            {
                masterOneLine[item3.Key] = 1;
            }
            foreach (KeyValuePair<string, byte> item4 in new List<KeyValuePair<string, byte>>(oneLine))
            {
                oneLine[item4.Key] = (byte)((item4.Value > 0) ? 1 : 0);
            }
            foreach (KeyValuePair<string, byte> item5 in new List<KeyValuePair<string, byte>>(masterPriest))
            {
                if (item5.Value <= 3)
                {
                    masterPriest[item5.Key] = 0;
                }
                else
                {
                    masterPriest[item5.Key] -= 3;
                }
                if (item5.Key == "cradh" || item5.Key == "mor cradh" || item5.Key == "ard cradh")
                {
                    masterPriest[item5.Key] = 1;
                }
            }
            foreach (KeyValuePair<string, byte> item6 in new List<KeyValuePair<string, byte>>(cradhZeroLine))
            {
                if (item6.Key == "beag cradh" || item6.Key == "cradh" || item6.Key == "mor cradh" || item6.Key == "ard cradh" || item6.Key == "Dark Seal" || item6.Key == "Darker Seal" || item6.Key == "Demise")
                {
                    cradhZeroLine[item6.Key] = 0;
                }
            }
            foreach (KeyValuePair<string, byte> item7 in new List<KeyValuePair<string, byte>>(fourLinesBecome2Lines))
            {
                if (item7.Value == 4)
                {
                    fourLinesBecome2Lines[item7.Key] = 2;
                }
            }
            foreach (KeyValuePair<string, byte> item8 in new List<KeyValuePair<string, byte>>(cradhOneLine))
            {
                if (item8.Key == "beag cradh" || item8.Key == "cradh" || item8.Key == "mor cradh" || item8.Key == "ard cradh")
                {
                    cradhOneLine[item8.Key] = 1;
                }
            }
            bowList.Add(new Bow("Wooden Bow", 1, 1));
            bowList.Add(new Bow("Royal Bow", 8, 2));
            bowList.Add(new Bow("Jenwir Bow", 15, 3));
            bowList.Add(new Bow("Sen Bow", 22, 4));
            bowList.Add(new Bow("Andor Bow", 30, 4));
            bowList.Add(new Bow("Yumi Bow", 45, 5));
            bowList.Add(new Bow("Empowered Yumi Bow", 55, 6));
            bowList.Add(new Bow("Thunderfury", 90, 6));
            staffList.Add(new Staff("Barehand", new Dictionary<string, byte>(AvailableSpellsAndCastLines), 0, 0, false));
            staffList.Add(new Staff("Magus Zeus", fourLinesBecome2Lines, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Ares", cradhOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Diana", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Deoch", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Gramail", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Luathas", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Glioca", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Cail", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Sgrios", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Ceannlaidir", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Magus Fiosachd", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            staffList.Add(new Staff("Holy Deoch", minusOneLine, 0, 19, false, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Gramail", minusOneLine, 0, 19, false, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Luathas", minusOneLine, 0, 19, false, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Glioca", minusOneLine, 0, 19, false, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Cail", minusOneLine, 0, 19, false, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Sgrios", minusOneLine, 0, 19, false, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Ceannlaidir", minusOneLine, 0, 19, false, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Fiosachd", minusOneLine, 0, 19, false, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Diana", minusTwoLines, 0, 19, false, TemuairClass.Priest));
            staffList.Add(new Staff("Assassin's Cross", minusTwoLines, 27, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Veltain Staff", minusTwoLines, 15, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Andor Staff", minusTwoLines, 30, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Skylight Staff", minusTwoLines, 75, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Star Crafted Staff", minusTwoLines, 95, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Divinities Staff", minusTwoLines, 75, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Dark Star", minusTwoLines, 95, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Master Celestial Staff", masterOneLine, 0, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Enchanted Magus Orb", masterOneLine, 0, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Staff of Ages", masterOneLine, 0, 99, true));
            staffList.Add(new Staff("Staff of Brilliance", masterOneLine, 0, 99, true));
            staffList.Add(new Staff("Staff of Deliverance", masterOneLine, 25, 99, true));
            staffList.Add(new Staff("Staff of Clarity", masterOneLine, 50, 99, true));
            staffList.Add(new Staff("Staff of Eternity", masterOneLine, 75, 99, true));
            staffList.Add(new Staff("Empowered Magus Orb", oneLine, 0, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Sphere", oneLine, 1, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Shaine Sphere", oneLine, 8, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Maron Sphere", oneLine, 15, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Chernol Sphere", oneLine, 22, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Serpant Sphere", oneLine, 45, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Empowered Serpant Sphere", oneLine, 55, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Glimmering Wand", oneLine, 65, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Glimmering Wand1", oneLine, 65, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Yowien Tree Staff", oneLine, 80, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Yowien Tree Staff1", oneLine, 80, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Dragon Infused Staff", oneLine, 90, 99, true, TemuairClass.Wizard));
            staffList.Add(new Staff("Wooden Harp", oneLine, 1, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Goldberry Harp", oneLine, 8, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Rosewood Harp", oneLine, 15, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Ironwood Harp", oneLine, 22, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Hwarone Lute", oneLine, 45, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Empowered Hwarone Lute", oneLine, 55, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Hy-brasyl Baton", oneLine, 65, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Holy Hy-brasyl Baton1", oneLine, 65, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Brute's Quill", oneLine, 80, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Brute's Quill1", oneLine, 80, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Dragon Emberwood Staff", oneLine, 90, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Master Divine Staff", masterPriest, 0, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Empowered Holy Gnarl", cradhZeroLine, 0, 99, true, TemuairClass.Priest));
            staffList.Add(new Staff("Eagles Grasp", AvailableSpellsAndCastLines, 90, 99, true, TemuairClass.Monk));
            staffList.Add(new Staff("Yowien's Fist", AvailableSpellsAndCastLines, 80, 99, true, TemuairClass.Monk));
            staffList.Add(new Staff("Yowien's Fist1", AvailableSpellsAndCastLines, 80, 99, true, TemuairClass.Monk));
            staffList.Add(new Staff("Yowien's Claw", AvailableSpellsAndCastLines, 65, 99, true, TemuairClass.Monk));
            staffList.Add(new Staff("Yowien's Claw1", AvailableSpellsAndCastLines, 65, 99, true, TemuairClass.Monk));
            staffList.Add(new Staff("Blackstar Night Claw", AvailableSpellsAndCastLines, 95, 99, true, TemuairClass.Monk));
            //Adam Add Arsaid Aon weapons
            foreach (Item item in new List<Item>(Inventory))
            {
                if (staffList.Any((Staff staff) => staff.Name == item.Name))
                {
                    item.IsStaff = true;
                }
                else if (bowList.Any((Bow bow) => bow.Name == item.Name))
                {
                    item.IsBow = true;
                }
            }
        }

        #region Packet methods

        internal void DisplayEntityRequest(int id)
        {
            ClientPacket clientPacket = new ClientPacket(12);
            clientPacket.WriteInt32(id);
            Enqueue(clientPacket);
        }

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
                if (player.BodySprite == 0 && GetCheats(Cheats.SeeHidden) && !_inArena)
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
            if (player.BodySprite == 0 && !GetCheats(Cheats.SeeHidden))
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
            if (GetCheats(Cheats.GmMode))
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
                if (GetCheats(Cheats.NoBlind) && !_inArena)
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


