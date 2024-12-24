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
using Talos.Properties;
using Talos.Definitions;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Talos.Base;
using System.Collections.Concurrent;
using GroundItem = Talos.Objects.GroundItem;
using Talos.PInvoke;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;



namespace Talos.Base
{
    internal class Client
    {
        #region Process Memory
        internal int processId;
        internal IntPtr hWnd;

        internal byte[] PROCESS_DATA;

        internal int BASE_ADDRESS = 6281332;

        internal byte[] ADDRESS_BUFFER = new byte[5]
        {
        232,
        47,
        88,
        2,
        0
        };

        private bool IsOSCompatible => Environment.OSVersion.Version.Major >= 5;
        #endregion

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




        internal Location _clientLocation;
        internal Location _serverLocation;
        internal Location _routeDestination;
        internal Location _lastDestination;

        internal DateTime _lastWorldShout = DateTime.MinValue;
        internal DateTime _lastMapChange = DateTime.MinValue;
        internal DateTime _comboScrollLastUsed = DateTime.MinValue;
        internal DateTime _arenaAnnounceTimer = DateTime.MinValue;
        internal DateTime _lastHidden = DateTime.MinValue;
        private DateTime _lastClickedWorldMap = DateTime.MinValue;
        private static DateTime _lastWalkTime = DateTime.MinValue;

        internal Map _map;
        internal WorldMap _worldMap;
        internal Cheats _cheats;
        internal Direction _clientDirection;
        internal Direction _serverDirection;
        internal MapFlags _mapFlags;
        internal EffectsBar _status;
        internal TemuairClass _temuairClassFlag;
        internal MedeniaClass _medeniaClassFlag;
        internal DruidForm _druidFormFlag;
        internal PreviousClass _previousClass;
        internal Dugon _dugon;
        internal Element _element;
        internal Animation _lastAnimation;
        internal string _npcDialog;
        internal string _currentSkill = "";
        internal string _currentItem = "";
        internal string _offenseElement = "";
        internal string _action = "Current Action: ";
        internal string _itemToReEquip = "";
        internal bool _safeScreen;
        internal bool _clientWalkPending;
        internal bool _isCasting;
        internal bool _isWalking;
        internal bool _isBashing;
        internal int _isRefreshing;
        internal bool _mapChanged;
        internal bool _hasLabor;
        internal bool _bool_39;
        internal bool _overrideMapFlags;
        internal bool _mapChangePending;
        internal bool _isRegistered = true;
        internal bool _isCheckingBelt;
        internal bool _inventoryFull;
        internal bool _shouldEquipBow;
        internal bool _trainingGroundsMember;
        internal bool _hasWalked;
        internal bool _cancelPressed;
        internal bool _acceptPressed;
        internal bool _exchangeOpen;
        internal bool _exchangeClosing;
        internal bool _okToBubble = false;
        internal bool _confirmBubble;
        internal bool isStatusUpdated;
        internal bool _recentlyCrashered;
        internal bool _bashingSkillsLoaded = false;
        internal bool _unmaxedSpellsLoaded = false;
        internal bool _unmaxedSkillsLoaded = false;
        internal bool _comboOneSet = false;
        internal bool _comboTwoSet = false;
        internal bool _comboThreeSet = false;

        internal double _walkSpeed = 150;
        internal ushort _spriteOverride = 1;
        internal bool _deformNearStrangers = false;
        internal int _spellCounter;
        private int _customSpellLineCounter;
        private static int _walkCallCount = 0;
        internal int _identifier;
        internal int _stuckCounter;
        internal byte _comboScrollCounter;
        internal Spell CastedSpell { get; set; }
        internal System.Windows.Forms.Timer _spellTimer;
        internal Stack<Location> pathStack = new Stack<Location>();
        internal Stack<Location> routeStack = new Stack<Location>();




        internal readonly object Lock = new object();
        internal readonly object BashLock = new object();

        internal AutoResetEvent _walkSignal = new AutoResetEvent(false);

        internal List<Staff> _staffList = new List<Staff>();
        internal List<MeleeWeapon> _meleeList = new List<MeleeWeapon>();
        internal List<Bow> _bowList = new List<Bow>();
        internal List<SpellEntry> _spellHistory = new List<SpellEntry>();
        internal List<string> _inventoryList = new List<string>();

        internal BindingList<int> _worldObjectBindingList = new BindingList<int>();
        internal BindingList<int> _creatureBindingList = new BindingList<int>();

        internal BindingList<string> _strangerBindingList = new BindingList<string>();
        internal BindingList<string> _friendBindingList = new BindingList<string>();
        internal BindingList<string> _groupBindingList = new BindingList<string>();



        internal Dictionary<string, string> UserOptions { get; set; } = new Dictionary<string, string>();

        internal Dictionary<string, byte> AvailableSpellsAndCastLines { get; set; } = new Dictionary<string, byte>();
        internal Dictionary<string, DateTime> DictLastSeen { get; set; } = new Dictionary<string, DateTime>();
        internal ConcurrentDictionary<int, Location> LastSeenLocations { get; set; } = new ConcurrentDictionary<int, Location>();

        internal ConcurrentDictionary<int, WorldObject> WorldObjects { get; private set; } = new ConcurrentDictionary<int, WorldObject>();
        internal ConcurrentDictionary<int, Player> NearbyHiddenPlayers { get; private set; } = new ConcurrentDictionary<int, Player>();
        internal ConcurrentDictionary<string, Player> NearbyPlayers { get; private set; } = new ConcurrentDictionary<string, Player>();
        internal ConcurrentDictionary<string, Player> NearbyGhosts { get; private set; } = new ConcurrentDictionary<string, Player>();
        internal ConcurrentDictionary<string, Creature> NearbyNPC { get; private set; } = new ConcurrentDictionary<string, Creature>();
        internal ConcurrentDictionary<string, int> ObjectID { get; private set; } = new ConcurrentDictionary<string, int>();
        internal ConcurrentDictionary<string, Player> DeadPlayers { get; private set; } = new ConcurrentDictionary<string, Player>();
        internal ConcurrentDictionary<Location, Door> Doors { get; private set; } = new ConcurrentDictionary<Location, Door> { };

        internal HashSet<int> NearbyObjects { get; private set; } = new HashSet<int>();
        internal HashSet<int> NearbyGroundItems { get; private set; } = new HashSet<int>();
        internal HashSet<ushort> EffectsBar { get; set; } = new HashSet<ushort>();
        internal HashSet<string> GroupedPlayers { get; set; } = new HashSet<string> { };

        internal static HashSet<string> AoSuainHashSet = new HashSet<string>(new string[3]
        {
            "ao suain",
            "ao pramh",
            "Leafhopper Chirp"
        }, StringComparer.CurrentCultureIgnoreCase);

        internal bool _stopped = false;
        internal short _previousMapID;
        internal uint _exchangeID;
        internal bool _needsToRepairHammer = false;
        internal bool _assailNoise;
        internal bool _ladder;
        internal bool _chestToggle = false;
        internal bool _raffleToggle = false;

        internal Pathfinder Pathfinder { get; set; }
        internal RouteFinder RouteFinder { get; set; }

        internal Bot Bot { get; set; }
        internal BotBase BotBase { get; set; }
        internal Thread WalkThread { get; set; }
        internal ClientTab ClientTab { get; set; }
        internal Statistics Stats { get; set; }
        internal Dialog Dialog { get; set; }
        internal Inventory Inventory { get; set; } = new Inventory(60);
        internal Item[] EquippedItems { get; set; } = new Item[20];
        internal Spellbook Spellbook { get; set; } = new Spellbook();
        internal Skillbook Skillbook { get; set; } = new Skillbook();
        internal string Name { get; set; }
        internal string GuildName { get; set; }
        internal byte Path { get; set; }
        internal byte StepCount { get; set; }
        internal DateTime LastStep { get; set; } = DateTime.MinValue;
        internal DateTime LastMoved { get; set; } = DateTime.MinValue;
        internal DateTime LastTurned { get; set; } = DateTime.MinValue;
        internal uint PlayerID { get; set; }
        internal bool SpriteOverrideEnabled { get; set; }
        internal bool InArena
        {
            get
            {
                string mapName = _map?.Name;

                if (mapName == "Balanced Arena" || mapName == "Coliseum Circuit")
                {
                    return false;
                }

                if (mapName?.Contains("Arena") == true || mapName?.Contains("Loures Battle Ring") == true)
                {
                    return true;
                }

                return mapName?.Contains("Coliseum") == true;
            }
        }
        internal bool UnifiedGuildChat { get; set; }
        internal Player Player { get; set; }
        internal Creature CastedTarget { get; set; }
        internal Nation Nation { get; set; }

        internal uint HealthPct
        {
            get
            {
                if (Stats.CurrentHP * 100 / Stats.MaximumHP <= 100)
                {
                    return (Stats.CurrentHP * 100 / Stats.MaximumHP);
                }
                return 100;
            }
        }
        internal uint ManaPct
        {
            get
            {
                if (Stats.CurrentMP * 100 / Stats.MaximumMP <= 100)
                {
                    return (Stats.CurrentMP * 100 / Stats.MaximumMP);
                }
                return 100;
            }
        }

        internal uint MaximumHP { get { return Stats.MaximumHP; } set { Stats.MaximumHP = value; } }
        internal uint MaximumMP { get { return Stats.MaximumMP; } set { Stats.MaximumMP = value; } }
        internal uint CurrentHP { get { return Stats.CurrentHP; } set { Stats.CurrentHP = value; } }
        internal uint CurrentMP { get { return Stats.CurrentMP; } set { Stats.CurrentMP = value; } }
        internal byte UnspentPoints { get { return Stats.UnspentPoints; } set { Stats.UnspentPoints = value; } }
        internal uint Experience => Stats.Experience;
        internal uint AbilityExperience => Stats.AbilityExperience;
        internal uint Gold => Stats.Gold;
        internal byte Ability => Stats.Ability;
        internal byte Level => Stats.Level;
        internal uint ToNextLevel => Stats.ToNextLevel;
        internal bool DialogOn { get; set; }
        internal bool HasLetter => Stats.Mail.HasFlag(Mail.HasLetter);
        internal bool HasParcel => Stats.Mail.HasFlag(Mail.HasParcel);

        internal bool IsSkulled => Player != null && (EffectsBar.Contains((ushort)Enumerations.EffectsBar.Skull) || EffectsBar.Contains((ushort)Enumerations.EffectsBar.WormSkull) && Player.IsSkulled);

        public int CurrentWaypoint { get; internal set; }
        public uint BaseHP { get; internal set; }
        public uint BaseMP { get; internal set; }


        public readonly object CastLock = new object();

        internal Client(Server server, Socket socket)
        {
            _identifier = Utility.Random(int.MaxValue);
            _server = server;
            _crypto = new Crypto(0, "UrkcnItnI");
            _clientSocket = socket;
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _sendQueue = new Queue<Packet>();
            _receiveQueue = new Queue<Packet>();
            _spellTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _spellTimer.Tick += SpellTimerTick;
            RouteFinder = new RouteFinder(_server, this);
            Stats = new Statistics();
            Bot = new Bot(this, server);
        }
        internal void SpellTimerTick(object sender, EventArgs e)
        {
            _spellCounter = 0;
        }
        internal void Remove()
        {
            ClientTab.RemoveClient();
            ClientTab = null;
        }

        internal bool HasEffect(EffectsBar effectID) => EffectsBar.Contains((ushort)effectID);
        internal void ClearEffect(EffectsBar effectID) => EffectsBar.Remove((ushort)effectID);
        internal bool GetMapFlags(MapFlags flagID) => _mapFlags.HasFlag(flagID);
        internal void SetMapFlags(MapFlags flagID) => _mapFlags |= flagID;
        internal void SetTemuairClass(TemuairClass temClass) => _temuairClassFlag |= temClass;
        internal void SetMedeniaClass(MedeniaClass medClass) => _medeniaClassFlag |= medClass;
        internal void SetPreviousClass(PreviousClass previousClass) => _previousClass |= previousClass;
        internal void SetDruidForm (DruidForm druidForm) => _druidFormFlag |= druidForm;
        internal void SetDugon(Dugon color) => _dugon |= color;
        internal bool GetCheats(Cheats value) => _cheats.HasFlag(value);
        internal void EnableCheats(Cheats value) => _cheats |= value;
        internal void DisableCheats(Cheats value) => _cheats &= (Cheats)(byte)(~(uint)value);
        internal void SetStatUpdateFlags(StatUpdateFlags flags) => Attributes(flags, Stats);
        internal bool HasItem(string itemName) => Inventory.Contains(itemName);
        internal bool HasSkill(string skillName) => Skillbook[skillName] != null;
        internal bool HasSpell(string spellName) => Spellbook[spellName] != null;
        internal int CalculateHealAmount(string spellName)
        {
            int num = (Stats.CurrentWis >= 99) ? Stats.CurrentWis : 99;
            uint fnvHash = Utility.CalculateFNV(spellName);

            switch (fnvHash)
            {
                case 301833330: //Spirit Essence
                    return 2000000;

                case 2360874113: //beag ioc
                    return 1056 - (255 - num) * 6;

                case 2431366422: //ioc
                case 1586538930: //ioc comlha
                    return 8800 - (255 - num) * 50;

                case 2358017708: //mor ioc comlha:
                case 1812078524: //mor ioc
                    return 26400 - (255 - num) * 150;

                case 1544215329: //ard ioc comlha
                case 4118165127: //ard ioc
                    return 52800 - (255 - num) * 300;

                case 3783935686: //nuadhaich
                case 1649558369: //Nuadhiach Le Cheile
                    return 88000 - (255 - num) * 500;

                case 1445356977: //Leigheas
                    return 264000 - (255 - num) * 1000;

                case 4167026527: //Cold Blood
                    return 500 * Ability;

                default:
                    return 0;
            }
        }
        internal int GetItemQuantity(string itemName)
        {
            int quantity = 0;
            foreach (Item item in Inventory)
            {
                if (item != null && item.Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase))
                {
                    quantity += item.Quantity;
                }
            }
            return quantity;
        }
        internal bool CanUseItem(Item item)
        {
            if (!EffectsBar.Contains((ushort)Enumerations.EffectsBar.Pramh) || !EffectsBar.Contains((ushort)Enumerations.EffectsBar.Suain))
                return true;

            return false;
        }
        internal bool CanUseSkill(Skill skill)
        {
            if (skill.CanUse && _map.CanUseSkills && !EffectsBar.Contains((ushort)Enumerations.EffectsBar.Pramh) && !EffectsBar.Contains((ushort)Enumerations.EffectsBar.Suain))
                return true;

            return false;
        }
        internal bool CanUseSpell(Spell spell, Creature creature = null)
        {
            if (spell.CanUse && _map.CanUseSpells)
            {
                if (EffectsBar.Contains((ushort)Enumerations.EffectsBar.Pramh) || EffectsBar.Contains((ushort)Enumerations.EffectsBar.Suain))
                    return AoSuainHashSet.Contains(spell.Name);

                return true; // If neither Pramh nor Suain is active, always return true
            }
            return false;
        }
        internal bool NumberedSkill(string skillName)
        {
            for (int i = 20; i > 0; i--)
            {
                if (UseSkill($"{skillName} {i}"))
                    return true;
            }
            return false;
        }
        internal void UseHammer()
        {
            Bot._dontWalk = true;
            Bot._dontCast = true;
            Bot._dontBash = true;

            var repairItem = Inventory.FirstOrDefault(i => i.Name.Equals("Equipment Repair", StringComparison.OrdinalIgnoreCase));

            if (repairItem == null)
            {
                ServerMessage((byte)ServerMessageType.OrangeBar1, "You do not have any Equipment Repair hammers!");
                ClientTab.equipmentrepairCbox.Checked = false;
                Bot._dontWalk = false;
                Bot._dontCast = false;
                Bot._dontBash = false;
            }
            else
            {
                if (!UseItem("Equipment Repair"))
                    return;

                Timer hammer = Timer.FromSeconds(5);

                while (Dialog == null)
                {
                    if (hammer.IsTimeExpired || Player.IsSuained || Player.IsAsleep)
                        return;
                    Thread.Sleep(25);
                }

                ReplyDialog(Dialog.ObjectType, Dialog.ObjectID, Dialog.PursuitID, (ushort)(Dialog.DialogID + 1U));
                _needsToRepairHammer = false;
                Bot._hammerTimer = DateTime.UtcNow;
                Bot._dontWalk = false;
                Bot._dontCast = false;
                Bot._dontBash = false;
            }
        }
        internal List<GroundItem> GetNearbyGroundItems(int distance = 12, params ushort[] sprites)
        {
            var spriteSet = new HashSet<ushort>(sprites);
            var nearbyGroundItems = new List<GroundItem>();

            if (Monitor.TryEnter(Server.SyncObj, 150))
            {
                try
                {
                    foreach (var objectId in NearbyGroundItems)
                    {
                        if (WorldObjects[objectId] is GroundItem groundItem
                            && this.WithinRange(groundItem, distance)
                            && spriteSet.Contains(groundItem.SpriteID))
                        {
                            nearbyGroundItems.Add(groundItem);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(Server.SyncObj);
                }
            }

            return nearbyGroundItems;
        }
        internal List<GroundItem> GetNearbyGroundItems(int distance = 12) //Adam
        {
            if (!Monitor.TryEnter(Server.SyncObj, 1000))
            {
                return new List<GroundItem>();
            }
            try
            {
                return WorldObjects.Values
                    .OfType<GroundItem>()
                    .Where(obj => obj.GetType() == typeof(Objects.GroundItem))
                    .Where(obj => _serverLocation.DistanceFrom(obj.Location) <= distance)
                    .ToList();
            }
            finally
            {
                Monitor.Exit(Server.SyncObj);
            }
        }
        internal Player GetNearbyPlayer(string name)
        {
            if (Monitor.TryEnter(Server.SyncObj, 300))
            {
                try
                {
                    return (!NearbyPlayers.ContainsKey(name)) ? null : NearbyPlayers[name];
                }
                finally
                {
                    Monitor.Exit(Server.SyncObj);
                }
            }
            return null;
        }
        internal List<Player> GetNearbyPlayers()
        {
            List<Player> nearbyPlayers = new List<Player>();
            if (Monitor.TryEnter(Server.SyncObj, 150))
            {
                try
                {
                    nearbyPlayers = NearbyPlayers.Values
                        .Where(p => !string.IsNullOrEmpty(p.Name))
                        .ToList();
                }
                finally
                {
                    Monitor.Exit(Server.SyncObj);
                }
            }
            //foreach (var p in nearbyPlayers)
            //{
            //    Console.WriteLine("NearbyPlayer: " + p.Name);
            //}
            return nearbyPlayers;
        }
        internal List<Player> GetNearbyAllies()
        {

            if (!Monitor.TryEnter(Server.SyncObj, 1000))
            {
                return new List<Player>();
            }
            try
            {
                return NearbyPlayers.Values
                    .Where(player => GroupedPlayers.Contains(player.Name))
                    .ToList();
            }
            finally
            {
                Monitor.Exit(Server.SyncObj);
            }
        }
        internal Creature GetNearbyNPC(string name)
        {
            if (Monitor.TryEnter(Server.SyncObj, 300))
            {
                try
                {
                    return (!NearbyNPC.ContainsKey(name)) ? null : NearbyNPC[name];
                }
                finally
                {
                    Monitor.Exit(Server.SyncObj);
                }
            }
            return null;
        }
        internal List<Creature> GetNearbyNPCs()
        {
            if (Monitor.TryEnter(Server.SyncObj, 150))
            {
                try
                {
                    return NearbyNPC.Values.ToList();
                }
                finally
                {
                    Monitor.Exit(Server.SyncObj);
                }
            }

            // Return an empty list if unable to acquire lock
            return new List<Creature>();
        }
        internal List<WorldObject> GetNearbyObjects() //Adam
        {
            return WorldObjects.Values.Where(wo => NearbyObjects.Contains(wo.ID)).ToList();
        }
        internal List<Location> GetWarpPoints(Location location)
        {
            if (!_server._maps.TryGetValue(location.MapID, out Map value))
            {
                return new List<Location>();
            }

            var exits = value.Exits
                .Where(item => !location.Point.Equals(item.Key))
                .Select(item => new Location(location.MapID, item.Key))
                .ToList();

            exits.AddRange(value.WorldMaps
                .Where(item => !location.Point.Equals(item.Key))
                .Select(item => new Location(location.MapID, item.Key)));

            return exits;
        }

        internal HashSet<Location> GetAllWarpPoints()
        {
            HashSet<Location> allWarpPoints = new HashSet<Location>();

            if (_server._maps.TryGetValue(_map.MapID, out Map map))
            {
                foreach (var exit in map.Exits)
                {
                    allWarpPoints.Add(new Location(_map.MapID, exit.Key));
                }

                foreach (var worldMap in map.WorldMaps)
                {
                    allWarpPoints.Add(new Location(_map.MapID, worldMap.Key));
                }
            }

            return allWarpPoints;
        }



        internal List<Creature> GetNearbyValidCreatures(int distance = 12)
        {
            var whiteList = new HashSet<ushort>();
            List<Creature> creatureList = new List<Creature>();
            if (!Monitor.TryEnter(Server.SyncObj, 1000))
            {
                return creatureList;
            }

            try
            {
                var validCreatures = GetNearbyObjects().OfType<Creature>().Where(creature => IsValidCreature(creature, distance));
                creatureList.AddRange(validCreatures);

                if (creatureList.Count == 0) return creatureList;

                if (CONSTANTS.WHITELIST_BY_MAP_ID.ContainsKey((ushort)_map.MapID) ||
                    CONSTANTS.WHITELIST_BY_MAP_NAME.Any(kv => _map.Name.StartsWith(kv.Key)))
                {
                    whiteList = GetWhiteLists();
                    creatureList = creatureList.Where(creature => IsCreatureAllowed(creature, whiteList)).ToList();
                }

                return creatureList;
            }
            finally
            {
                Monitor.Exit(Server.SyncObj);
            }
        }
        private HashSet<ushort> GetWhiteLists()
        {
            var whiteList = new HashSet<ushort>();
            if (CONSTANTS.WHITELIST_BY_MAP_ID.TryGetValue((ushort)_map.MapID, out var whiteListByMapID))
            {
                whiteList.UnionWith(whiteListByMapID);
            }

            var whiteListByMapName = CONSTANTS.WHITELIST_BY_MAP_NAME.FirstOrDefault(kv => _map.Name.StartsWith(kv.Key)).Value;
            if (whiteListByMapName != null)
            {
                whiteList.UnionWith(whiteListByMapName);
            }

            return whiteList;
        }
        private bool IsValidCreature(Creature creature, int distance)
        {
            int mapID = _map.MapID;

            // Allow sprite 492 only on map 6999
            if (creature.SpriteID == 492 && mapID == 6999)
            {
                return creature.Type < CreatureType.Merchant && creature.SpriteID > 0 && creature.SpriteID <= 1000 && WithinRange(creature, distance);
            }

            return creature.Type < CreatureType.Merchant && creature.SpriteID > 0 && creature.SpriteID <= 1000 &&
                !CONSTANTS.INVISIBLE_SPRITES.Contains(creature.SpriteID) &&
                !CONSTANTS.UNDESIRABLE_SPRITES.Contains(creature.SpriteID) && WithinRange(creature, distance);
        }
        internal List<Creature> GetAllNearbyMonsters(int distance = 12)
        {
            List<Creature> list = new List<Creature>();
            if (Monitor.TryEnter(Server.SyncObj, 1000))
            {
                try
                {
                    foreach (Creature creature in GetNearbyObjects().OfType<Creature>())
                    {
                        if (IsValidCreature(creature, distance))
                        {
                            list.Add(creature);
                        }
                    }
                    return list;
                }
                finally
                {
                    Monitor.Exit(Server.SyncObj);
                }
            }
            return list;
        }
        internal List<Location> GetAdjacentPoints(Creature creature)
        {
            return new List<Location>
            {
                new Location(creature.Location.MapID, (short)(creature.Location.X - 1), creature.Location.Y),
                new Location(creature.Location.MapID, creature.Location.X, (short)(creature.Location.Y + 1)),
                new Location(creature.Location.MapID, creature.Location.X, (short)(creature.Location.Y - 1)),
                new Location(creature.Location.MapID, (short)(creature.Location.X + 1), creature.Location.Y)
            };
        }
        private Location GetValidLocationNearTarget(Location targetLocation, short followDistance)
        {
            List<Location> potentialLocations = new List<Location>();

            // Check exactly followDistance tiles away in each direction (Up, Down, Left, Right)
            short[] dx = { followDistance, (short)-followDistance, 0, 0 };
            short[] dy = { 0, 0, followDistance, (short)-followDistance };

            //Console.WriteLine($"Checking cardinal directions around target: {targetLocation}, followDistance: {followDistance}");

            // Try to find valid locations in the cardinal directions
            for (int i = 0; i < 4; i++)
            {
                Location potentialLocation = new Location(
                    targetLocation.MapID,
                    (short)(targetLocation.X + dx[i]),
                    (short)(targetLocation.Y + dy[i])
                );


                //Console.WriteLine($"Checking cardinal direction location: {potentialLocation}");

                // Check if the location is valid and walkable
                if (IsWalkable(potentialLocation))
                {
                    //Console.WriteLine($"Location {potentialLocation} is walkable.");
                    potentialLocations.Add(potentialLocation);
                }
                else
                {
                    //Console.WriteLine($"Location {potentialLocation} is NOT walkable.");
                }
            }

            // Return the first valid location found in the cardinal directions
            if (potentialLocations.Any())
            {
                //Console.WriteLine($"Returning valid cardinal direction location: {potentialLocations.First()}");
                return potentialLocations.First();
            }

            // As a fallback, calculate diagonal movements exactly followDistance away
            //Console.WriteLine($"No valid cardinal locations found. Checking diagonals...");

            for (short xOffset = (short)-followDistance; xOffset <= followDistance; xOffset++)
            {
                for (short yOffset = (short)-followDistance; yOffset <= followDistance; yOffset++)
                {
                    if (Math.Abs(xOffset) == followDistance || Math.Abs(yOffset) == followDistance)
                    {
                        Location diagonalLocation = new Location(
                            targetLocation.MapID,
                            (short)(targetLocation.X + xOffset),
                            (short)(targetLocation.Y + yOffset)
                        );


                        //Console.WriteLine($"Checking diagonal location: {diagonalLocation}");

                        // Check if the diagonal location is walkable
                        if (IsWalkable(diagonalLocation))
                        {
                            //Console.WriteLine($"Diagonal location {diagonalLocation} is walkable.");
                            potentialLocations.Add(diagonalLocation);
                        }
                        else
                        {
                            //Console.WriteLine($"Diagonal location {diagonalLocation} is NOT walkable.");
                        }
                    }
                }
            }

            // If valid diagonal locations were found, return the first one
            if (potentialLocations.Any())
            {
                //Console.WriteLine($"Returning valid diagonal location: {potentialLocations.First()}");
                return potentialLocations.First();
            }

            // If no valid locations were found, return the original target location
            //Console.WriteLine($"No valid locations found. Returning original target location: {targetLocation}");
            return targetLocation;
        }
        internal List<Location> GetCreatureCoverage(Creature creature)
        {
            if (HasEffect(Enumerations.EffectsBar.Hide))
            {
                return new List<Location>();
            }
            return new List<Location>
            {
                new Location(creature.Location.MapID, new Point((short)(creature.Location.X - 2), creature.Location.Y)),
                new Location(creature.Location.MapID, new Point((short)(creature.Location.X - 1), (short)(creature.Location.Y + 1))),
                new Location(creature.Location.MapID, new Point((short)(creature.Location.X - 1), creature.Location.Y)),
                new Location(creature.Location.MapID, new Point((short)(creature.Location.X - 1), (short)(creature.Location.Y - 1))),
                new Location(creature.Location.MapID, new Point(creature.Location.X, (short)(creature.Location.Y + 2))),
                new Location(creature.Location.MapID, new Point(creature.Location.X, (short)(creature.Location.Y + 1))),
                new Location(creature.Location.MapID, new Point(creature.Location.X, (short)(creature.Location.Y - 1))),
                new Location(creature.Location.MapID, new Point(creature.Location.X, (short)(creature.Location.Y - 2))),
                new Location(creature.Location.MapID, new Point((short)(creature.Location.X + 1), (short)(creature.Location.Y + 1))),
                new Location(creature.Location.MapID, new Point((short)(creature.Location.X + 1), creature.Location.Y)),
                new Location(creature.Location.MapID, new Point((short)(creature.Location.X + 1), (short)(creature.Location.Y - 1))),
                new Location(creature.Location.MapID, new Point((short)(creature.Location.X + 2), creature.Location.Y))
            };
        }
        internal List<Creature> GetAllNearbyMonsters(int distance = 12, params ushort[] creatureArray)
        {
            var creatureList = new List<Creature>();
            var hashSet = new HashSet<ushort>(creatureArray);

            lock (Server.SyncObj)
            {
                foreach (var creature in GetNearbyObjects().OfType<Creature>())
                {
                    // Allow sprite 492 only on map 6999
                    if (creature.SpriteID == 492 && _map.MapID != 6999)
                    {
                        continue;
                    }

                    if ((creature.Type == CreatureType.Normal || creature.Type == CreatureType.WalkThrough)
                        && WithinRange(creature, distance)
                        && creature.SpriteID > 0 && creature.SpriteID <= 1000
                        && !CONSTANTS.INVISIBLE_SPRITES.Contains(creature.SpriteID)
                        && !CONSTANTS.UNDESIRABLE_SPRITES.Contains(creature.SpriteID)
                        && hashSet.Contains(creature.SpriteID))
                    {
                        creatureList.Add(creature);
                    }
                }
            }

            return creatureList;
        }


        internal void UseExperienceGem(byte choice)
        {
            Bot._dontWalk = true;
            if (!UseItem("Experience Gem"))
            {
                ServerMessage(1, "You do not have any experience gems.");
                ClientTab.autoGemCbox.Checked = false;
                return;
            }
            while (Dialog == null)
            {
                Thread.Sleep(25);
            }
            byte type = Dialog.ObjectType;
            int objID = Dialog.ObjectID;
            ushort pursuitID = Dialog.PursuitID;
            ushort dialogID = Dialog.DialogID;
            Dialog.DialogNext();
            ReplyDialog(type, objID, pursuitID, (ushort)(dialogID + 1));
            ReplyDialog(type, objID, pursuitID, (ushort)(dialogID + 1), choice);
            ReplyDialog(type, objID, pursuitID, (ushort)(dialogID + 1));
            ReplyDialog(type, objID, pursuitID, (ushort)(dialogID + 1));
            ReplyDialog(type, objID, pursuitID, (ushort)(dialogID + 1));
            Thread.Sleep(1000);
            while (Dialog == null)
            {
                Thread.Sleep(25);
            }
            type = Dialog.ObjectType;
            objID = Dialog.ObjectID;
            pursuitID = Dialog.PursuitID;
            dialogID = Dialog.DialogID;
            ReplyDialog(type, objID, pursuitID, (ushort)(dialogID + 1));
            ReplyDialog(type, objID, pursuitID, (ushort)(dialogID + 1), 2);
            ReplyDialog(type, objID, pursuitID, dialogID);
            Bot._lastUsedGem = DateTime.UtcNow;
            Bot._dontWalk = false;
        }
        private bool IsValidSpell(Client client, string spellName, Creature creature)
        {
            // Guard clause: Reject spell if Suain effect is active and spell is not allowed
            //Adam maybe we should check if pramh'd too?
            if (ShouldRejectSpellDueToSuain(client, spellName))
            {
                return false;
            }

            // Guard clause: Make sure we aren't in Dojo
            if (!_map.Name.Contains("Training Dojo"))
            {
                // Reject spell based on spell name
                //Adam check whether this is needed. We already check for spell == null in the parent method
                //if (ShouldRejectSpellDueToSpellName(spellName))
                //{
                //    return false;
                //}

                // Check if the creature already has the given spell
                if (DoesCreatureHaveSpellAlready(spellName, creature, client))
                {
                    return false;
                }
            }
            //Console.WriteLine($"[IsValidSpell] Spell {spellName} on Creature ID: {creature?.ID} is valid.");
            return true;
        }
        internal bool ReadyToSpell(string spell)
        {
            try
            {
                uint num = Utility.CalculateFNV(spell);
                switch (num)
                {
                    case 420187390: // beag fas nadur
                    case 2476745328: // fas nadur
                    case 1149628551: // mor fas nadur
                    case 107956092: // ard fas nadur
                        if (!CastedTarget.IsFassed) return true;
                        return false;

                    case 2848971440: // beag cradh
                    case 1154413499: // cradh
                    case 1281358573: // mor cradh
                    case 2118188214: // ard cradh
                    case 1928539694: // Dark Seal
                    case 219207967: // Darker Seal
                    case 4252225073: // Demon Seal
                    case 928817768: // Demise
                        if (!CastedTarget.IsCursed) return true;
                        return false;

                    case 2112563240: // beag naomh aite
                    case 291448073: // naomh aite
                    case 2761324515: // mor naomh aite
                    case 443271170: // ard naomh aite
                        if (!CastedTarget.IsAited) return true;
                        return false;

                    case 195270534: // Wake Scroll
                        if (spell == "Wake Scroll")
                        {
                            foreach (Player player in GetNearbyAllies())
                            {
                                player.LastAnimation[(ushort)SpellAnimation.Mesmerize] = DateTime.MinValue;
                                player.LastAnimation[(ushort)SpellAnimation.Pramh] = DateTime.MinValue;
                            }
                        }
                        return false;

                    case 810175405: // ao suain
                    case 894297607: // Leafhopper Chirp
                        CastedTarget.LastAnimation[(ushort)SpellAnimation.Suain] = DateTime.MinValue;
                        return false;

                    case 1046347411: // suain
                        if (CastedTarget.IsSuained)
                        {
                            Console.WriteLine("ReadyToSpell: Creature is already suained, returning false");
                            return false;
                        }
                        else
                        {
                            Console.WriteLine("ReadyToSpell: Creature is not suained, can cast Pramh, returning true");
                            return true;
                        }

                    case 2030226177: // armachd
                        //Adam check this
                        if (!CastedTarget.HasArmachd) return true;
                        return false;

                    case 3219892635: // beag pramh
                    case 2647647615: // pramh
                    case 2592944103: // Mesmerize
                        if (CastedTarget.IsAsleep)
                        {
                            Console.WriteLine("ReadyToSpell: Creature is already asleep, returning false");
                            return false;
                        }
                        else
                        {
                            Console.WriteLine("ReadyToSpell: Creature is not asleep, can cast Pramh, returning true");
                            return true;
                        }

                    case 2756163491: // Fungus Beetle Extract
                        foreach (Player player in GetNearbyAllies())
                        {
                            player.LastAnimation[(ushort)SpellAnimation.PinkPoison] = DateTime.MinValue;
                            player.LastAnimation[(ushort)SpellAnimation.GreenBubblePoison] = DateTime.MinValue;
                            player.LastAnimation[(ushort)SpellAnimation.MedeniaPoison] = DateTime.MinValue;
                        }
                        return false;

                    case 674409180: // Lyliac Plant
                    case 2793184655: // Lyliac Vineyard
                        Bot._needFasSpiorad = true;
                        return false;

                    case 2996522388: //ao puinsein
                        {
                            CastedTarget.LastAnimation[(ushort)SpellAnimation.PinkPoison] = DateTime.MinValue;
                            CastedTarget.LastAnimation[(ushort)SpellAnimation.GreenBubblePoison] = DateTime.MinValue;
                            CastedTarget.LastAnimation[(ushort)SpellAnimation.MedeniaPoison] = DateTime.MinValue;
                        }
                        return false;

                    case 3134427575: // Frost Arrow 1
                    case 3151205194: // Frost Arrow 2
                    case 3167982813: // Frost Arrow 3
                    case 3050539480: // Frost Arrow 4
                    case 3067317099: // Frost Arrow 5
                    case 3084094718: // Frost Arrow 6
                    case 3100872337: // Frost Arrow 7
                    case 2983429004: // Frost Arrow 8
                    case 3000206623: // Frost Arrow 9
                    case 2718832517: // Frost Arrow 10
                    case 2702054898: // Frost Arrow 11
                        if (CastedTarget.IsFrozen)
                        {
                            Console.WriteLine("ReadyToSpell: Creature is already asleep, returning false");
                            return false;
                        }
                        else
                        {
                            Console.WriteLine("ReadyToSpell: Creature is not asleep, can cast Frost Arrow, returning true");
                            return true;
                        }
                    case 2292268700: // Cursed Tune 1
                    case 2342601557: // Cursed Tune 2
                    case 2325823938: // Cursed Tune 3
                    case 2241935843: // Cursed Tune 4
                    case 2225158224: // Cursed Tune 5
                    case 2275491081: // Cursed Tune 6
                    case 2258713462: // Cursed Tune 7
                    case 2443267271: // Cursed Tune 8
                    case 2426489652: // Cursed Tune 9
                    case 3252005060: // Cursed Tune 10
                    case 3268782679: // Cursed Tune 11
                    case 3285560298: // Cursed Tune 12
                        if (CastedTarget.HasCursedTunes)
                        {
                            Console.WriteLine("ReadyToSpell: Creature already has CT, returning false");
                            return false;
                        }
                        else
                        {
                            Console.WriteLine("ReadyToSpell: Creature does not have CT, can cast CT, returning true");
                            return true;
                        }


                    default:
                        return true;
                }
            }
            catch
            {
                return true;
            }
        }
        private bool ShouldRejectSpellDueToSuain(Client client, string spellName)
        {
            return (EffectsBar.Contains((ushort)Enumerations.EffectsBar.Suain) &&
                   (client == null || !IsAllowedSuainSpell(spellName)));
        }
        private bool IsAllowedSuainSpell(string spellName)
        {
            return spellName.Equals("ao suain", StringComparison.CurrentCultureIgnoreCase) ||
                   spellName.Equals("Leafhopper Chirp", StringComparison.CurrentCultureIgnoreCase);
        }
        private bool ShouldRejectSpellDueToSpellName(string spellName)
        {
            uint spellHash = Utility.CalculateFNV(spellName);

            switch (spellHash)
            {
                case 107956092: // ard fas nadur
                case 219207967: // Darker Seal
                case 4252225073: //Demon Seal
                case 291448073: // naomh aite
                case 420187390: // beag fas nadur
                case 443271170: // ard naomh aite
                case 810175405: // ao suain
                case 928817768: // Demise

                case 3777649476: // beag pramh
                case 1046347411: // pramh

                case 2848971440: // beag cradh
                case 1149628551: // cradh
                case 2118188214: // ard cradh


                case 1154413499: // mor fas nadur
                case 1928539694: // Dark Seal
                case 2112563240: // beag naomh aite

                case 2454795333: // fas nadur
                case 2579487986: // Mesmerize
                case 2647647615: // pramh
                case 2761324515: // mor naomh aite

                case 3000206623: // Frost Arrow 9
                case 3050539480: // Frost Arrow 4
                case 3067317099: // Frost Arrow 5
                case 3084094718: // Frost Arrow 6
                case 3100872337: // Frost Arrow 7
                case 3134427575: // Frost Arrow 1
                case 3151205194: // Frost Arrow 2
                case 3635920463: // fas spiorad

                    return false;
                default:
                    return true;
            }
        }
        private bool DoesCreatureHaveSpellAlready(string spellName, Creature creature, Client client)
        {
            lock (Lock)
            {
                if (creature == null)
                {
                    return false;
                }

                switch (spellName)
                {
                    case "suain":
                        //Console.WriteLine($"[DoesCreatureHaveSpellAlready] Checking {spellName} for Creature ID: {creature.ID}, Creature Name: {creature.Name}, Hash: {creature.GetHashCode()}, Currently Suained: {creature.IsSuained}");
                        return creature.IsSuained;
                    case "beag cradh":
                    case "cradh":
                    case "mor cradh":
                    case "ard cradh":
                    case "Dark Seal":
                    case "Darker Seal":
                    case "Demise":
                    case "Demon Seal":
                        //Console.WriteLine($"[DoesCreatureHaveSpellAlready] Checking {spellName} for Creature ID: {creature.ID}, Creature Name: {creature.Name}, Hash: {creature.GetHashCode()} Currently Cursed: {creature.IsCursed}");
                        return creature.IsCursed;

                    case "Frost Arrow 1":
                    case "Frost Arrow 2":
                    case "Frost Arrow 3":
                    case "Frost Arrow 4":
                    case "Frost Arrow 5":
                    case "Frost Arrow 6":
                    case "Frost Arrow 7":
                    case "Frost Arrow 8":
                    case "Frost Arrow 9":
                    case "Frost Arrow 10":
                        return creature.IsFrozen;
                    case "beag pramh":
                    case "pramh":
                    case "Mesmerize":
                        //Console.WriteLine($"[DoesCreatureHaveSpellAlready] Checking {spellName} for Creature ID: {creature.ID}, Hash: {creature.GetHashCode()}, Currently Asleep: {creature.IsAsleep}");
                        return creature.IsAsleep;
                    case "fas spiorad":
                        return !Bot._needFasSpiorad && !Bot._manaLessThanEightyPct;
                    case "beag fas nadur":
                    case "fas nadur":
                    case "mor fas nadur":
                    case "ard fas nadur":
                        //Console.WriteLine($"[DoesCreatureHaveSpellAlready] Checking {spellName} for Creature ID: {creature.ID}, Currently Fassed: {creature.IsFassed}");
                        return creature.IsFassed;
                    case "ao suain":
                        return creature.IsSuained;
                    case "beag naomh aite":
                    case "naomh aite":
                    case "mor naomh aite":
                    case "ard naomh aite":
                        return creature.IsAited;
                    default:
                        return false;
                }
            }
        }
        internal bool WaitForSpellChant()
        {
            DateTime utcNow = DateTime.UtcNow;
            while (true)
            {
                if (CastedSpell != null)
                {
                    if (!(DateTime.UtcNow.Subtract(utcNow).TotalSeconds <= 1.5))
                    {
                        break;
                    }
                    //Thread.Sleep(5);
                    continue;
                }
                return true;
            }
            CastedSpell = null;
            return false;
        }
        internal void SeeGhosts(Player player)
        {
            player.NakedPlayer();
            player.HeadColor = 1;
            player.BodySprite = 48;
            DisplayAisling(player);
            DisplayEntityRequest(player.ID);
        }
        private void UpdateClientActionText(string text)
        {
            if (ClientTab.InvokeRequired)
            {
                ClientTab.Invoke((Action)(() => ClientTab.currentAction.Text = text));
            }
            else
            {
                ClientTab.currentAction.Text = text;
            }
        }
        private bool UseOptimalStaff(Spell spell, out byte castLines)
        {
            bool swappingWeapons = false;
            castLines = spell.CastLines;

            Item obj = EquippedItems[1];
            Bow bow = (obj != null && obj.IsBow) ? EquippedItems[1].Bow : new Bow();
            Staff staff = (obj != null && obj.IsStaff) ? EquippedItems[1].Staff : new Staff();
            MeleeWeapon meleeWeapon = (obj != null && obj.IsMeleeWeapon) ? EquippedItems[1].Melee : new MeleeWeapon();

            bool hasArcherSpells = CONSTANTS.ARCHER_SPELLS.Any(spellName => spell.Name.Contains(spellName) || spell.Name.Equals(spellName, StringComparison.InvariantCultureIgnoreCase));
            DateTime utcNow = DateTime.UtcNow;
            if (hasArcherSpells && int.TryParse(Skillbook.SkillbookDictionary.Keys.FirstOrDefault((string string_0) => string_0.Contains("Archery ")).Replace("Archery ", ""), out int currentArcherySkill))
            {
                var equippedBow = EquippedItems[1]?.Bow;
                var bestBow = Inventory.Where(i => i.IsBow && i.Bow.CanUse(Ability, currentArcherySkill) && i.Bow.AbilityRequired > equippedBow?.AbilityRequired)
                                       .Select(i => i.Bow)
                                       .OrderByDescending(b => b.AbilityRequired)
                                       .FirstOrDefault();

                if (bestBow != null)
                {
                    string bowName = bestBow.Name;
                    if (equippedBow?.Name.Equals(bowName, StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        return true;
                    }

                    RemoveShield();
                    //Console.WriteLine($"Equipping {bowName} to cast {spell.Name}");
                    UseItem(bowName);

                    while (EquippedItems[1]?.Name?.Equals(bowName, StringComparison.CurrentCultureIgnoreCase) != true)
                    {
                        double elapsedSeconds = DateTime.UtcNow.Subtract(utcNow).TotalSeconds;

                        if (elapsedSeconds <= 2.0)
                        {
                            UpdateClientActionText($"{_action} Swapping to {bestBow.Name}");
                            // Thread.Sleep(5);
                            continue;
                        }

                        return false;
                    }
                }
            }
            else
            {
                var bestStaff = Inventory
                    .Where(item => item.IsStaff && item.Staff.CanUse(Ability, Level, ToNextLevel, _temuairClassFlag))
                    .FirstOrDefault(item => item.Staff.CastLines[spell.Name] < spell.CastLines &&
                        (item.Staff.AbilityRequired > staff.AbilityRequired ||
                        item.Staff.InsightRequired >= staff.InsightRequired ||
                        (item.Staff.MasterRequired && !staff.MasterRequired)));

                if (bestStaff != null)
                {
                    staff = bestStaff.Staff;
                    swappingWeapons = true;
                }

                if (staff.Name == new Staff().Name)
                {
                    if (Spellbook[spell.Name].CastLines <= _staffList[0].CastLines[spell.Name])
                    {
                        return true;
                    }
                    staff = _staffList[0];
                }

                if (swappingWeapons)
                {
                    RemoveShield();

                    if (staff.Name != "Barehand")
                    {
                        Console.WriteLine($"Equipping {staff.Name} to cast {spell.Name}");
                        UseItem(staff.Name);
                    }

                    while (Spellbook[spell.Name]?.CastLines != staff.CastLines[spell.Name])
                    {
                        double elapsedSeconds = DateTime.UtcNow.Subtract(utcNow).TotalSeconds;

                        // Ensure the swap operation doesn't exceed 2 seconds
                        if (elapsedSeconds <= 2.0)
                        {
                            UpdateClientActionText($"{_action} Swapping to {staff.Name}");

                            // Thread.Sleep(5);
                            continue;
                        }

                        return false;
                    }

                    Thread.Sleep(100);
                }
            }
            castLines = Spellbook[spell.Name].CastLines;
            return true;
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
            _meleeList.Add(new MeleeWeapon("Stick", 0, 1, false, true));
            _meleeList.Add(new MeleeWeapon("Oak Stick", 0, 1, false, true));
            _meleeList.Add(new MeleeWeapon("Dirk", 0, 2, false, true));
            _meleeList.Add(new MeleeWeapon("Eppe", 0, 2, false, true));
            _meleeList.Add(new MeleeWeapon("Loures Saber", 0, 7, false, true));
            _meleeList.Add(new MeleeWeapon("Harpoon", 0, 11, false, true));
            _meleeList.Add(new MeleeWeapon("Hatchet", 0, 13, false, true));
            _meleeList.Add(new MeleeWeapon("Claidheamh", 0, 14, false, true));
            _meleeList.Add(new MeleeWeapon("Broad Sword", 0, 17, false, true));
            _meleeList.Add(new MeleeWeapon("Dragon Scale Sword", 0, 20, false, true));
            _meleeList.Add(new MeleeWeapon("Wooden Club", 0, 50, false, true));
            _meleeList.Add(new MeleeWeapon("Stone Axe", 0, 95, false, true));
            _meleeList.Add(new MeleeWeapon("Amber Saber", 0, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Diamond Saber", 0, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Emerald Saber", 0, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Ruby Saber", 0, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Sapphire Saber", 0, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Master Falcata", 0, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Master Saber", 0, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Eclipse", 0, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Crystal Saber", 65, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Yowien Hatchet", 80, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Defiled Ruby Saber", 95, 99, true, true, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Two-Handed Claidhmore", 0, 71, false, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Two-Handed Emerald Sword", 0, 77, false, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Two-Handed Gladius", 0, 86, false, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Two-Handed Kindjal", 0, 90, false, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Giant Stone Axe", 0, 93, false, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Giant Stone Club", 0, 95, false, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Giant Stone Hammer", 0, 97, false, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Two-Handed Dragon Slayer", 0, 97, false, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Hy-brasyl Battle Axe", 0, 99, false, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Gold Kindjal", 0, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Hy-brasy Escalon", 0, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Empowered Escalon", 0, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Enchanted Escalon", 0, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Master Battle Axe", 0, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Dane Blade", 1, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Astion Blade", 8, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Brune Blade", 15, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Veltain Sword", 15, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Tempered Veltain Sword", 15, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Tuned Veltain Sword", 15, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Blazed Veltain Sword", 15, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Grum Blade", 22, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Andor Saber", 30, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Hwarone Guandao", 45, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Empowered Hwarone Guandao", 55, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Hellreavers Blade", 90, 99, true, false, TemuairClass.Warrior));
            _meleeList.Add(new MeleeWeapon("Blackstar Night Claw", 95, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Eagles Grasp", 90, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Yowien's Fist", 80, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Yowien's Fist1", 80, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Yowien's Claw", 65, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Yowien's Claw1", 65, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Stone Fists", 0, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Phoenix Claws", 0, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Enchanted Kalkuri", 0, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Obsidian", 0, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Nunchaku", 0, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Tilian Claw", 0, 99, true, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Wolf Claw", 0, 50, false, true, TemuairClass.Monk));
            _meleeList.Add(new MeleeWeapon("Wolf Claws", 0, 50, false, true, TemuairClass.Monk));
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
                if (item6.Key == "beag cradh" || item6.Key == "cradh" || item6.Key == "mor cradh" || item6.Key == "ard cradh" || item6.Key == "Dark Seal" || item6.Key == "Darker Seal" || item6.Key == "Demise" || item6.Key == "Demon Seal")
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
            _bowList.Add(new Bow("Wooden Bow", 1, 1));
            _bowList.Add(new Bow("Royal Bow", 8, 2));
            _bowList.Add(new Bow("Jenwir Bow", 15, 3));
            _bowList.Add(new Bow("Sen Bow", 22, 4));
            _bowList.Add(new Bow("Andor Bow", 30, 4));
            _bowList.Add(new Bow("Yumi Bow", 45, 5));
            _bowList.Add(new Bow("Empowered Yumi Bow", 55, 6));
            _bowList.Add(new Bow("Thunderfury", 90, 6));
            _staffList.Add(new Staff("Barehand", new Dictionary<string, byte>(AvailableSpellsAndCastLines), 0, 0, false));
            _staffList.Add(new Staff("Magus Zeus", fourLinesBecome2Lines, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Ares", cradhOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Diana", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Deoch", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Gramail", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Luathas", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Glioca", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Cail", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Sgrios", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Ceannlaidir", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Magus Fiosachd", minusOneLine, 0, 19, false, TemuairClass.Wizard));
            _staffList.Add(new Staff("Holy Deoch", minusOneLine, 0, 19, false, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Gramail", minusOneLine, 0, 19, false, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Luathas", minusOneLine, 0, 19, false, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Glioca", minusOneLine, 0, 19, false, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Cail", minusOneLine, 0, 19, false, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Sgrios", minusOneLine, 0, 19, false, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Ceannlaidir", minusOneLine, 0, 19, false, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Fiosachd", minusOneLine, 0, 19, false, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Diana", minusTwoLines, 0, 19, false, TemuairClass.Priest));
            _staffList.Add(new Staff("Assassin's Cross", minusTwoLines, 27, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Veltain Staff", minusTwoLines, 15, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Andor Staff", minusTwoLines, 30, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Skylight Staff", minusTwoLines, 75, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Star Crafted Staff", minusTwoLines, 95, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Divinities Staff", minusTwoLines, 75, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Dark Star", minusTwoLines, 95, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Master Celestial Staff", masterOneLine, 0, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Enchanted Magus Orb", masterOneLine, 0, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Staff of Ages", masterOneLine, 0, 99, true));
            _staffList.Add(new Staff("Staff of Brilliance", masterOneLine, 0, 99, true));
            _staffList.Add(new Staff("Staff of Deliverance", masterOneLine, 25, 99, true));
            _staffList.Add(new Staff("Staff of Clarity", masterOneLine, 50, 99, true));
            _staffList.Add(new Staff("Staff of Eternity", masterOneLine, 75, 99, true));
            _staffList.Add(new Staff("Empowered Magus Orb", oneLine, 0, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Sphere", oneLine, 1, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Shaine Sphere", oneLine, 8, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Maron Sphere", oneLine, 15, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Chernol Sphere", oneLine, 22, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Serpant Sphere", oneLine, 45, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Empowered Serpant Sphere", oneLine, 55, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Glimmering Wand", oneLine, 65, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Glimmering Wand1", oneLine, 65, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Yowien Tree Staff", oneLine, 80, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Yowien Tree Staff1", oneLine, 80, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Dragon Infused Staff", oneLine, 90, 99, true, TemuairClass.Wizard));
            _staffList.Add(new Staff("Wooden Harp", oneLine, 1, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Goldberry Harp", oneLine, 8, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Rosewood Harp", oneLine, 15, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Ironwood Harp", oneLine, 22, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Hwarone Lute", oneLine, 45, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Empowered Hwarone Lute", oneLine, 55, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Hy-brasyl Baton", oneLine, 65, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Holy Hy-brasyl Baton1", oneLine, 65, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Brute's Quill", oneLine, 80, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Brute's Quill1", oneLine, 80, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Dragon Emberwood Staff", oneLine, 90, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Master Divine Staff", masterPriest, 0, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Empowered Holy Gnarl", cradhZeroLine, 0, 99, true, TemuairClass.Priest));
            _staffList.Add(new Staff("Eagles Grasp", AvailableSpellsAndCastLines, 90, 99, true, TemuairClass.Monk));
            _staffList.Add(new Staff("Yowien's Fist", AvailableSpellsAndCastLines, 80, 99, true, TemuairClass.Monk));
            _staffList.Add(new Staff("Yowien's Fist1", AvailableSpellsAndCastLines, 80, 99, true, TemuairClass.Monk));
            _staffList.Add(new Staff("Yowien's Claw", AvailableSpellsAndCastLines, 65, 99, true, TemuairClass.Monk));
            _staffList.Add(new Staff("Yowien's Claw1", AvailableSpellsAndCastLines, 65, 99, true, TemuairClass.Monk));
            _staffList.Add(new Staff("Blackstar Night Claw", AvailableSpellsAndCastLines, 95, 99, true, TemuairClass.Monk));
            //Adam Add Arsaid Aon weapons
        }
        internal void CheckWeaponType(Item item)
        {
            if (_staffList.Any(Staff => Staff.Name == item.Name))
            {
                item.IsStaff = true;
                item.Staff = _staffList.First(Staff => Staff.Name == item.Name);
            }
            if (_bowList.Any(Bow => Bow.Name == item.Name))
            {
                item.IsBow = true;
                item.Bow = _bowList.First(Bow => Bow.Name == item.Name);
            }
            if (_meleeList.Any(MeleeWeapon => MeleeWeapon.Name == item.Name))
            {
                item.IsMeleeWeapon = true;
                item.Melee = _meleeList.First(MeleeWeapon => MeleeWeapon.Name == item.Name);
            }
        }
        private string[] LoadSavedChants(string spellName)
        {
            string[] chants = new string[10];
            string spellBookPath = System.IO.Path.Combine(Settings.Default.DarkAgesPath.Replace("Darkages.exe", ""), Name, "SpellBook.cfg");

            if (!File.Exists(spellBookPath))
            {
                return chants;
            }

            string[] lines = File.ReadAllLines(spellBookPath);
            bool foundSpell = false;
            foreach (string line in lines)
            {
                if (line.Equals(spellName, StringComparison.CurrentCultureIgnoreCase))
                {
                    foundSpell = true;
                    continue;
                }

                if (foundSpell)
                {
                    if (line.StartsWith("Spell"))
                    {
                        int index = int.Parse(line.Substring("Spell".Length, line.IndexOf(':') - "Spell".Length));
                        chants[index] = line.Substring(line.IndexOf(':') + 1);
                        if (index == chants.Length - 1) // Stop if we've read all chants
                        {
                            break;
                        }
                    }
                    else
                    {
                        break; // Exit loop if line doesn't start with "Spell" prefix
                    }
                }
            }

            return chants;
        }
        private string ParseCustomSpellLines()
        {
            string[] array = Regex.Split(ClientTab.customLinesBox.Text, "\r\n");
            _customSpellLineCounter++;
            if (_customSpellLineCounter > array.Count() - 1)
            {
                _customSpellLineCounter = 0;
            }
            return array[_customSpellLineCounter];
        }
        internal bool TryUseAnySpell(string[] spellNames, Creature target, bool autoStaffSwitch, bool keepSpellAfterUse)
        {
            foreach (string spellName in spellNames)
            {
                // Check if the spell exists in the Spellbook
                if (Spellbook[spellName] != null)
                {
                    // If the spell exists, attempt to use it
                    if (UseSpell(spellName, target, autoStaffSwitch, keepSpellAfterUse))
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }
        internal void SleepFighting()
        {
            Thread.Sleep(3000);
            Dialog?.DialogNext(2);
        }
        internal void ReEquipItem(string itemName)
        {
            while (!Inventory.Contains(itemName))
            {
                Thread.Sleep(100);
            }
            UseItem(itemName);
        }
        internal bool EquipDarkNeck()
        {
            return EquipNecklace("Dark", CONSTANTS.DARK_NECKS);
        }
        internal bool EquipLightNeck()
        {
            return EquipNecklace("Light", CONSTANTS.LIGHT_NCEKS);
        }
        private bool EquipNecklace(string elementName, Dictionary<string, (int Ability, int Level)> necklaceList)
        {
            lock (BashLock)
            {
                if (necklaceList.ContainsKey(EquippedItems[6].Name))
                {
                    _offenseElement = elementName;
                    return true;
                }
                var bestNecklace = Inventory
                    .Where(item => necklaceList.ContainsKey(item.Name)) // Item exists in the list
                    .Where(item => necklaceList[item.Name].Ability <= Ability && necklaceList[item.Name].Level <= Level) // Usability check
                    .OrderByDescending(item => necklaceList[item.Name].Level)   // Prioritize higher levels
                    .ThenByDescending(item => necklaceList[item.Name].Ability)  // Break ties with ability
                    .FirstOrDefault();

                if (bestNecklace == null)
                    return false; // No usable necklace found

                UseItem(bestNecklace.Name);

                // Verify if the necklace is equipped
                Timer timer = Timer.FromMilliseconds(1500);
                while (!timer.IsTimeExpired)
                {
                    if (EquippedItems[6].Name == bestNecklace.Name)
                    {
                        _offenseElement = elementName;
                        return true;
                    }
                    Thread.Sleep(50);
                }

                return false;
            }
        }
        private bool IsCreatureAllowed(Creature creature, HashSet<ushort> whiteList)
        {

            int mapID = _map.MapID;

            // Allow sprite 492 only on map 6999
            if (creature.SpriteID == 492 && mapID == 6999)
            {
                return true;
            }

            if (creature.Type == CreatureType.WalkThrough || CONSTANTS.INVISIBLE_SPRITES.Contains(creature.SpriteID) || CONSTANTS.UNDESIRABLE_SPRITES.Contains(creature.SpriteID) || CONSTANTS.RED_BOROS.Contains(creature.SpriteID) || CONSTANTS.GREEN_BOROS.Contains(creature.SpriteID))
            {
                return false;
            }

            var blackListByMapName = CONSTANTS.BLACKLIST_BY_MAP_NAME.FirstOrDefault(kv => _map.Name.StartsWith(kv.Key)).Value;
            if (blackListByMapName != null && blackListByMapName.Contains(creature.SpriteID))
            {
                return false;
            }

            return whiteList.Contains(creature.SpriteID);
        }
        internal void ResetExchangeVars()
        {
            _exchangeClosing = true;
            Thread.Sleep(1500);
            _exchangeOpen = false;
            _exchangeClosing = false;
        }
        internal void LoadUnmaxedSkills()
        {
            short num = 15;
            short num2 = 20;
            foreach (KeyValuePair<string, Skill> entry in Skillbook)
            {
                Skill skill = entry.Value;
                if (skill != null && !CONSTANTS.DOJOBLACKLIST_1.Contains(skill.Name) && !skill.Name.Contains("Lore") && !skill.Name.Contains("Inner Beast") && !skill.Name.Contains("Item") && !skill.Name.Contains("Archery") && !skill.Name.Contains("Thrust Attack") && skill.CurrentLevel < skill.MaxLevel)
                {
                    ClientTab.RenderUnmaxedSkills(skill.Name, skill.Sprite, new System.Drawing.Point(num, num2));
                    num = (short)(num + 40);
                    if (num >= 480)
                    {
                        num = 15;
                        num2 = (short)(num2 + 40);
                    }
                }
            }
            _unmaxedSkillsLoaded = true;
        }
        internal void LoadBashingSkills()
        {
            short num = 15;
            short num2 = 20;
            foreach (KeyValuePair<string, Skill> entry in Skillbook)
            {
                Skill skill = entry.Value;
                if (skill != null && !CONSTANTS.DOJOBLACKLIST_2.Contains(skill.Name) && !skill.Name.Contains("Lore") && !skill.Name.Contains("Inner Beast") && !skill.Name.Contains("Item") && !skill.Name.Contains("Archery") && !skill.Name.Contains("Thrust Attack") && !skill.Name.Contains("Arrow Shot") && skill != null)
                {
                    ClientTab.RenderBashingSkills(skill.Name, skill.Sprite, new System.Drawing.Point(num, num2));
                    num = (short)(num + 40);
                    if (num >= 650)
                    {
                        num = 15;
                        num2 = (short)(num2 + 40);
                    }
                }
            }
            _bashingSkillsLoaded = true;
        }
        internal void LoadUnmaxedSpells()
        {
            short num = 15;
            short num2 = 20;
            foreach (Spell spell in Spellbook.Where((Spell sp) => sp != null))
            {
                if (!CONSTANTS.DOJOBLACKLIST_1.Contains(spell.Name) && !spell.Name.Contains("Lore") && !spell.Name.Contains("Inner Beast") && !spell.Name.Contains("Item") && !spell.Name.Contains("Archery") && !spell.Name.Contains("Thrust Attack") && spell.CurrentLevel < spell.MaximumLevel)
                {
                    ClientTab.RenderUnmaxedSpells(spell.Name, spell.Sprite, new System.Drawing.Point(num, num2));
                    num = (short)(num + 40);
                    if (num >= 480)
                    {
                        num = 15;
                        num2 = (short)(num2 + 40);
                    }
                }
            }
            _unmaxedSpellsLoaded = true;
        }
        internal void EscapeKey()
        {
            NativeMethods.PostMessage(Process.GetProcessById(processId).MainWindowHandle, 256, 27, MakeLParam(1, NativeMethods.MapVirtualKey(27, 0)));
            NativeMethods.PostMessage(Process.GetProcessById(processId).MainWindowHandle, 257, 27, MakeLParam(1, NativeMethods.MapVirtualKey(27, 0)));
        }
        internal void EnterKey()
        {
            NativeMethods.PostMessage(Process.GetProcessById(processId).MainWindowHandle, 256, 13, MakeLParam(1, NativeMethods.MapVirtualKey(13, 0)));
            NativeMethods.PostMessage(Process.GetProcessById(processId).MainWindowHandle, 257, 13, MakeLParam(1, NativeMethods.MapVirtualKey(13, 0)));
        }
        internal int MakeLParam(int LoWord, int HiWord)
        {
            return (HiWord << 16) | (LoWord & 0xFFFF);
        }


        internal bool Pathfind(Location destination, short distance = 1, bool shouldBlock = true, bool avoidWarps = true)
        {

            if (NearbyHiddenPlayers.Count > 0 || _isCasting)
            {
                Console.WriteLine($"[Pathfind] [{this.Name}] Cannot walk because NearbyHiddenPlayers count is {NearbyHiddenPlayers.Count} or is casting {_isCasting}");
                if (!_isWalking)
                {
                    Console.WriteLine($"[Pathfind] [{this.Name}] Not currently walking.");
                    return false;
                }
                _isCasting = false;
            }

            bool isWall = _map.IsWall(_clientLocation);
            bool isStuck = GetNearbyObjects().OfType<Creature>()
                .Any(creature => creature != Player && creature.Type != CreatureType.WalkThrough && creature.Location == _clientLocation);

            //Console.WriteLine($"[Pathfind] [{this.Name}] value of isStuck = " + isStuck);

            if ((isWall || isStuck) && (_hasWalked || _clientLocation.X == 0 && _clientLocation.Y == 0 || _serverLocation.X == 0 && _serverLocation.Y == 0))
            {
                Console.WriteLine($"[Pathfind] [{this.Name}] isWall or isStuck, refreshing. Setting _hasWalked to false and returning false");
                RefreshRequest();
                _hasWalked = false;
                return false;
            }


            if (_clientLocation == destination)
            {
                if (_hasWalked || DateTime.UtcNow.Subtract(LastStep).TotalSeconds > 2.0)
                {
                    if (_stuckCounter == 0)
                    {
                        Console.WriteLine($"[Pathfind] [{this.Name}] Refreshing client due to timeout or refresh needed.");
                        RefreshRequest();
                    }
                    _hasWalked = false;
                }
                return true;
            }

            double elapsedMilliseconds = DateTime.UtcNow.Subtract(LastMoved).TotalMilliseconds;
            double waitThreshold = (Bot.IsStrangerNearby() && !ClientTab.chkSpeedStrangers.Checked) || Bot._rangerNear
                ? 420.0
                : _walkSpeed;

            // Continue looping until elapsed time meets or exceeds the threshold
            while (elapsedMilliseconds < waitThreshold)
            {
                Thread.Sleep(10);
                elapsedMilliseconds = DateTime.UtcNow.Subtract(LastMoved).TotalMilliseconds;
            }

            if (Equals(_clientLocation, destination))
            {
                Console.WriteLine($"[Pathfind] [{this.Name}] Destination reached.");
                RefreshRequest();
                return true;
            }

            if (Location.NotEquals(destination, _lastDestination) || pathStack.Count == 0)
            {
                Console.WriteLine($"[Pathfind] [{this.Name}] New destination or empty path stack. Calculating new path.");
                _lastDestination = destination;
                pathStack = Pathfinder.FindPath(_clientLocation, destination, avoidWarps);

            }

            if (pathStack.Count == 0 && Location.NotEquals(_clientLocation, _serverLocation) && _hasWalked)
            {
                pathStack = Pathfinder.FindPath(_serverLocation, destination, avoidWarps);
                if (pathStack.Count == 0)
                {
                    return false;
                }
                Console.WriteLine($"[Pathfind] [{this.Name}] Location difference detected, requesting refresh.");
                RefreshRequest();
                _hasWalked = false;
                return false;
            }

            if (pathStack.Count == 0)
            {
                Console.WriteLine($"[Pathfind] [{this.Name}] Path stack is empty, no further movement possible.");
                RefreshRequest();
                return false;
            }


            List<Creature> nearbyCreatures = (from creature in GetNearbyObjects().OfType<Creature>()
                                              where creature.Type != CreatureType.WalkThrough && creature != Player && creature.Location.DistanceFrom(_serverLocation) <= 11
                                              select creature).ToList();

            foreach (Location loc in pathStack)
            {
                Door door = Doors.Values.FirstOrDefault(d => d.Location.Equals(loc));
                if (door != null)
                {
                    Console.WriteLine($"[Pathfind] [{this.Name}] Door at {loc}, Closed: {door.Closed}, RecentlyClicked: {door.RecentlyClicked}");
                    if (door.Closed && !door.RecentlyClicked)
                    {
                        Console.WriteLine($"[Pathfind] [{this.Name}] Attempting to click door.");
                        ClickObject(loc);
                        door.LastClicked = DateTime.UtcNow;
                    }
                }
                if (nearbyCreatures.Count > 0 && nearbyCreatures.Any(creature => Location.NotEquals(loc, destination) && Location.Equals(creature.Location, loc) || (!HasEffect(Enumerations.EffectsBar.Hide) && CONSTANTS.GREEN_BOROS.Contains(creature.SpriteID) && GetCreatureCoverage(creature).Contains(loc))))
                {
                    if (isStuck)
                    {
                        Console.WriteLine($"[Pathfind] [{this.Name}] Stuck in creature intreaction if statement.");
                        break;
                    }
                    Console.WriteLine($"[Pathfind] [{this.Name}] Creature interaction required at {loc}, recalculating path.");
                    pathStack = Pathfinder.FindPath(_clientLocation, destination, avoidWarps);
                    return false;
                }
            }

            Location nextPosition = pathStack.Peek();
            if (nextPosition.Equals(_clientLocation))
            {
                pathStack.Pop();
                if (pathStack.Count == 0)
                {
                    Console.WriteLine($"[Pathfind] [{this.Name}] Path complete. Destination reached.");
                    return true;
                }
                nextPosition = pathStack.Peek();
            }


            if (nextPosition.DistanceFrom(_clientLocation) != 1)
            {
                Console.WriteLine($"[Pathfind] [{this.Name}] Unexpected distance to next position {nextPosition}, recalculating path.");
                if (nextPosition.DistanceFrom(_clientLocation) > 2 && _hasWalked)
                {
                    if (_stuckCounter == 0)
                    {
                        Console.WriteLine($"[Pathfind] [{this.Name}] Refreshing client due to unexpected position distance.");
                        RefreshRequest();
                    }
                    _hasWalked = false;
                }
                pathStack = Pathfinder.FindPath(_clientLocation, destination, avoidWarps);
                return false;
            }

            Direction directionToWalk = nextPosition.GetDirection(_clientLocation);
            if (shouldBlock)
            {
                lock (CastLock)
                {
                    if (!HasEffect(Enumerations.EffectsBar.Pramh))
                    {
                        if (!HasEffect(Enumerations.EffectsBar.Suain))
                        {
                            if (HasEffect(Enumerations.EffectsBar.Skull))
                            {
                                if (ClientTab.ascendBtn.Text != "Ascending")
                                {
                                    return true;
                                }
                            }
                            Walk(directionToWalk);
                        }
                    }
                }
            }
            else if (!HasEffect(Enumerations.EffectsBar.Pramh) && !HasEffect(Enumerations.EffectsBar.Suain) && (!HasEffect(Enumerations.EffectsBar.Skull) || ClientTab.ascendBtn.Text == "Ascending"))
            {
                Walk(directionToWalk);
            }

            return true;
        }
        internal bool RouteFind(Location destination, short distance = 0, bool mapOnly = false, bool shouldBlock = true, bool avoidWarps = true)
        {
            try
            {
                Console.WriteLine($"[RouteFind] [{this.Name}] Starting RouteFind to {destination}");
                if (_server._stopWalking)
                {
                    Console.WriteLine($"[RouteFind] [{this.Name}] Not supposed to walk.");
                    return false;
                }

                Location currentLocation = new Location(_map.MapID, _clientLocation.X, _clientLocation.Y);
                Console.WriteLine($"[RouteFind] [{this.Name}] Current location: {currentLocation}");
                Location adjustedDestination;
                //adjust for followdistance?
                if (ClientTab.followCbox.Checked)
                {
                    adjustedDestination = GetValidLocationNearTarget(destination, (short)ClientTab.followDistanceNum.Value);
                }
                else
                {
                    adjustedDestination = destination;
                }

                if (Location.Equals(currentLocation, adjustedDestination))
                {
                    if (Location.Equals(adjustedDestination, new Location(395, 6, 6))) //Path Temple 1
                    {
                        Bot._circle1 = true;
                    }
                    else if (Location.Equals(adjustedDestination, new Location(344, 6, 6))) //Path Temple 6
                    {
                        Bot._circle2 = true;
                    }
                    Console.WriteLine($"[RouteFind] [{this.Name}] Already at destination.");
                    routeStack.Clear();
                    return false;
                }

                if (routeStack.Count == 1 && adjustedDestination.MapID == _map.MapID && mapOnly)
                {
                    routeStack.Clear();
                    return false;
                }

                if (Location.NotEquals(_routeDestination, adjustedDestination) || routeStack.Count == 0)
                {
                    Console.WriteLine($"[RouteFind] [{this.Name}] Finding new route.");
                    _routeDestination = adjustedDestination;
                    routeStack = RouteFinder.FindRoute(currentLocation, adjustedDestination);
                }

                if (_map.Name.Contains("Plamit"))
                {
                    routeStack = RouteFinder.FindRoute(currentLocation, adjustedDestination);
                }

                if (routeStack.Count == 0)
                {
                    Console.WriteLine($"[RouteFind] [{this.Name}] Route not found, initializing new RouteFinder.");
                    RouteFinder = new RouteFinder(_server, this);
                    _routeDestination = adjustedDestination;
                    _lastClickedWorldMap = DateTime.MinValue;
                    routeStack = RouteFinder.FindRoute(currentLocation, adjustedDestination);
                    return false;
                }

                Location nextLocation = routeStack.Peek();

                //if (routeStack.Count != 1)
                //{
                //Console.WriteLine("***routeStack.Count != 1");
                //    distance = 0;
                //}

                if (routeStack.Count > 1 && Location.Equals(nextLocation, _serverLocation))
                {
                    Console.WriteLine($"[RouteFind] [{this.Name}] routeStack.Count > 1 & nextLocaiton = _serverLocation");
                    routeStack.Pop();
                    nextLocation = routeStack.Peek();
                }

                if (_worldMap != null)
                {
                    Console.WriteLine($"[RouteFind] [{this.Name}] World map is not null, processing world map navigation.");
                    List<Location> list = RouteFinder.FindRoute(currentLocation, adjustedDestination).Reverse().ToList();
                    if (DateTime.UtcNow.Subtract(_lastClickedWorldMap).TotalSeconds < 1.0)
                    {
                        return false;
                    }
                    foreach (Location location in list)
                    {
                        Console.WriteLine($"[RouteFind] [{this.Name}] Checking world map node for location {location}");

                        foreach (WorldMapNode node in _worldMap.Nodes)
                        {
                            Console.WriteLine($"[RouteFind] [{this.Name}] Checking node {node.Location}");
                            if (node.MapID == location.MapID)
                            {
                                Console.WriteLine($"[RouteFind] [{this.Name}] Need to click world map");
                                _lastClickedWorldMap = DateTime.UtcNow;
                                ClickWorldMap(node.MapID, node.Location);
                                return true;
                            }
                        }
                    }
                    foreach (WorldMapNode node in _worldMap.Nodes)
                    {
                        Console.WriteLine($"[RouteFind] [{this.Name}] [2]Checking node {node.Location}");

                        if (node.MapID == nextLocation.MapID)
                        {
                            Console.WriteLine($"[RouteFind] [{this.Name}] [2]Need to click world map");
                            _lastClickedWorldMap = DateTime.UtcNow;
                            ClickWorldMap(node.MapID, node.Location);
                            return true;
                        }
                    }
                    return false;
                }
                if (nextLocation.MapID != _map.MapID)
                {
                    if (!_server._maps.TryGetValue(_map.MapID, out Map value))
                    {
                        Console.WriteLine($"[RouteFind] [{this.Name}] Map not found in server maps. Clearing routeStack.");
                        routeStack.Clear();
                        return false;
                    }
                    if (value.WorldMaps.TryGetValue(_clientLocation.Point, out WorldMap _worldMapNodeList))
                    {
                        if (!value.WorldMaps.TryGetValue(_serverLocation.Point, out _worldMapNodeList))
                        {
                            Console.WriteLine($"[RouteFind] [{this.Name}] Need to refresh");
                            RefreshRequest();
                            return false;
                        }
                        foreach (WorldMapNode worldMapNode in _worldMapNodeList.Nodes)
                        {
                            if (worldMapNode.MapID == nextLocation.MapID)
                            {
                                Console.WriteLine($"[RouteFind] [{this.Name}] maps are equal");
                                return false;
                            }
                        }
                    }
                    if (value.Exits.TryGetValue(_clientLocation.Point, out Warp warp) && warp.TargetMapID == nextLocation.MapID)
                    {
                        Console.WriteLine($"[RouteFind] [{this.Name}] Warping with NPC");
                        WarpWithNPC(nextLocation);
                        RefreshRequest();
                        return true;
                    }
                    routeStack.Clear();
                    return false;
                }
                Console.WriteLine($"[RouteFind] [{this.Name}] About to call TryWalkLocation with distance value of: {distance}");
                if (!Pathfind(nextLocation, distance, shouldBlock, avoidWarps = true))
                {
                    if (_map.Name.Contains("Threshold"))
                    {
                        Console.WriteLine($"[RouteFind] [{this.Name}] Threshold map detected, attempting to walk south.");
                        Walk(Direction.South);
                        Thread.Sleep(1000);
                        return true;
                    }
                    Console.WriteLine($"[RouteFind] [{this.Name}] TryWalkToLocation failed, returning false.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RouteFind] [{this.Name}] Exception in RouteFind. Refreshing.");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("***");
                RefreshRequest();
                return false;
            }
            if (routeStack.Count <= 0)
            {
                Console.WriteLine($"[RouteFind] [{this.Name}] Route stack empty, returning false.");
                return false;
            }

            return true;
        }
        private bool IsWalkable(Location location)
        {
            // Check if the location is walkable (not blocked)
            if (_map.Tiles.TryGetValue(location.Point, out Tile tile))
            {
                return !tile.IsWall;
            }
            return false;
        }
        private void WarpWithNPC(Location nextLocation)
        {
            if (ClientTab != null)
            {
                try
                {
                    if (Location.Equals(_clientLocation, new Location(5220, 0, 6)) && nextLocation.MapID == 5210 && Dialog != null)
                    {
                        Dialog.DialogNext(2);
                    }
                    if (Location.Equals(_clientLocation, new Location(6926, 8, 9)) && nextLocation.MapID == 10028)
                    {
                        Creature creature = GetNearbyNPC("Quard");
                        ClickNPCDialog(creature, "Express Ship", true);
                        ReplyDialog(1, creature.ID, 0, 2, 1);
                        ReplyDialog(1, creature.ID, 0, 2);

                        if (!string.IsNullOrEmpty(ClientTab.walkMapCombox.Text) && ClientTab.walkBtn.Text == "Stop" && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medWalk[Name] = ClientTab.walkMapCombox.Text;
                        }
                        if (!string.IsNullOrEmpty(ClientTab.followText.Text) && ClientTab.followCbox.Checked && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medTask[Name] = ClientTab.followText.Text;
                        }
                        if (((!string.IsNullOrEmpty(ClientTab.walkMapCombox.Text) && ClientTab.walkBtn.Text == "Stop") || (!string.IsNullOrEmpty(ClientTab.followText.Text) && ClientTab.followCbox.Checked)) && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medWalkSpeed[Name] = ClientTab.walkSpeedSldr.Value;
                        }
                        if (ClientTab.toggleBugBtn.Text == "Disable" && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medTask[Name] = "bugEvent";
                            _server._medWalkSpeed[Name] = ClientTab.walkSpeedSldr.Value;
                        }
                        if (ClientTab.toggleSeaonalDblBtn.Text == "Disable" && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medTask[Name] = "vDayEvent";
                            _server._medWalkSpeed[Name] = ClientTab.walkSpeedSldr.Value;
                        }
                    }
                    else if (Location.Equals(_clientLocation, new Location(706, 11, 13)) && nextLocation.MapID == 6591)
                    {
                        PublicMessage(3, "Enter Sewer Maze");
                    }
                    else if (Location.Equals(_clientLocation, new Location(10000, 29, 31)) && nextLocation.MapID == 10999)
                    {
                        Creature creature = GetNearbyNPC("Lenoa");
                        ClickNPCDialog(creature, "Caravan to Noam", true);
                        ReplyDialog(1, creature.ID, 0, 2);
                        ReplyDialog(1, creature.ID, 0, 2, 1);
                        ReplyDialog(1, creature.ID, 0, 2);
                    }
                    else if (Location.Equals(_clientLocation, new Location(10055, 46, 23)) && nextLocation.MapID == 10999)
                    {
                        Creature creature = GetNearbyNPC("Habab");
                        ClickNPCDialog(creature, "Caravan to Asilon or Hwarone", true);
                        ReplyDialog(1, creature.ID, 0, 2);
                        ReplyDialog(1, creature.ID, 0, 2, 1);
                        ReplyDialog(1, creature.ID, 0, 2);
                    }
                    else if (Location.Equals(_clientLocation, new Location(10265, 87, 47)) && nextLocation.MapID == 10998)
                    {
                        Creature creature = GetNearbyNPC("Mank");
                        ClickNPCDialog(creature, "Caravan to Noam", true);
                        ReplyDialog(1, creature.ID, 0, 2);
                        ReplyDialog(1, creature.ID, 0, 2, 1);
                        ReplyDialog(1, creature.ID, 0, 2);
                    }
                    else if (Location.Equals(_clientLocation, new Location(10055, 46, 24)) && nextLocation.MapID == 1960)
                    {
                        Creature creature = GetNearbyNPC("Habab");
                        ClickNPCDialog(creature, "Carpet Merchant", true);
                        ReplyDialog(1, creature.ID, 0, 2);
                        ReplyDialog(1, creature.ID, 0, 2, 1);
                        ReplyDialog(1, creature.ID, 0, 2, 1);
                        ReplyDialog(1, creature.ID, 0, 2);

                        if (!string.IsNullOrEmpty(ClientTab.walkMapCombox.Text) && ClientTab.walkBtn.Text == "Stop" && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medWalk[Name] = ClientTab.walkMapCombox.Text;
                        }
                        if (!string.IsNullOrEmpty(ClientTab.followText.Text) && ClientTab.followCbox.Checked && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medTask[Name] = ClientTab.followText.Text;
                        }
                        if (((!string.IsNullOrEmpty(ClientTab.walkMapCombox.Text) && ClientTab.walkBtn.Text == "Stop") || (!string.IsNullOrEmpty(ClientTab.followText.Text) && ClientTab.followCbox.Checked)) && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medWalkSpeed[Name] = ClientTab.walkSpeedSldr.Value;
                        }
                        if (ClientTab.toggleBugBtn.Text == "Disable" && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medTask[Name] = "bugEvent";
                            _server._medWalkSpeed[Name] = ClientTab.walkSpeedSldr.Value;
                        }
                        if (ClientTab.toggleSeaonalDblBtn.Text == "Disable" && ClientTab.startStrip.Text == "Stop")
                        {
                            _server._medTask[Name] = "vDayEvent";
                            _server._medWalkSpeed[Name] = ClientTab.walkSpeedSldr.Value;
                        }
                    }
                    else if (Location.Equals(_clientLocation, new Location(3634, 16, 6)) && nextLocation.MapID == 8420)
                    {
                        Creature creature = GetNearbyNPC("Fallen Soldier");
                        ClickNPCDialog(creature, "ChadulEntry", true);
                        ReplyDialog(1, creature.ID, 0, 2, 1);
                    }
                    else if (Location.Equals(_clientLocation, new Location(8318, 50, 95)) && nextLocation.MapID == 8345)
                    {
                        Creature class7 = GetNearbyNPC("Ashlee");
                        ClickObject(class7.ID);
                        ReplyDialog(1, class7.ID, 0, 2);
                    }
                    else if (Location.Equals(_clientLocation, new Location(8355, 32, 5)) && nextLocation.MapID == 8356)
                    {
                        Creature class8 = GetNearbyNPC("Norrie");
                        PublicMessage(0, "let me through");
                        while (Dialog == null)
                        {
                            Thread.Sleep(25);
                        }
                        if (!Dialog.Message.Contains("Sorry, I forgot..."))
                        {
                            ReplyDialog(1, class8.ID, 0, 2, 1);
                            ReplyDialog(1, class8.ID, 0, 2, 1);
                            Thread.Sleep(1000);
                            PublicMessage(0, "let me through");
                        }
                        ReplyDialog(1, class8.ID, 0, 2);
                        ReplyDialog(1, class8.ID, 0, 2);
                        ReplyDialog(1, class8.ID, 0, 2);
                        Thread.Sleep(1000);
                    }
                    else if (Location.Equals(_clientLocation, new Location(8361, 32, 7)) && nextLocation.MapID == 8362)
                    {
                        Creature class9 = GetNearbyNPC("Yowien Guard");
                        if (class9 != null && Inventory.Contains("Yowien Costume"))
                        {
                            UseSkill("Assail");
                            UseItem("Yowien Costume");
                            UseItem("Yowien Headgear");
                            Thread.Sleep(1000);
                        }
                        PublicMessage(0, "graauuloow");
                        DateTime utcNow = DateTime.UtcNow;
                        while (Dialog == null)
                        {
                            if (!(DateTime.UtcNow.Subtract(utcNow).TotalSeconds <= 2.0))
                            {
                                return;
                            }
                            Thread.Sleep(25);
                        }
                        if (Dialog.Message == "*The Yowien Guard sniffs you, then suddenly attacks you. You run back to a safe distance*")
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            ReplyDialog(1, class9.ID, 0, 2);
                            ReplyDialog(1, class9.ID, 0, 2);
                            ReplyDialog(1, class9.ID, 0, 2);
                            ReplyDialog(1, class9.ID, 0, 2);
                            ReplyDialog(1, class9.ID, 0, 2);
                            Thread.Sleep(1000);
                        }
                    }
                    else if (_map.MapID == 3052 && _clientLocation.X == 44 && _clientLocation.Y >= 18 && _clientLocation.Y <= 25)
                    {
                        Creature class10 = GetNearbyNPC("Celesta");
                        ClickNPCDialog(class10, "Enter Balanced Arena", true);
                        ReplyDialog(1, class10.ID, 0, 2);
                        while (Dialog == null)
                        {
                            Thread.Sleep(10);
                        }
                        Thread.Sleep(1000);
                        if (Dialog != null && Dialog.Message.Contains("Lumen Amulet"))
                        {
                            ServerMessage(0, "Deposit your lumen, then re-enable bot. damn nub");
                            Bot.Stop();
                        }
                    }
                    else if (_map.MapID == 393 && _clientLocation.DistanceFrom(new Location(393, 7, 6)) <= 1)
                    {
                        Creature npc = GetNearbyNPC("Aoife");
                        PursuitRequest(1, npc.ID, 1171);
                        if (WaitForDialog())
                        {
                            ReplyDialog(Dialog.ObjectType, Dialog.ObjectID, Dialog.PursuitID, 1);
                            ReplyDialog(Dialog.ObjectType, Dialog.ObjectID, Dialog.PursuitID, (ushort)((Player.BodySprite == 16) ? 6 : 3));
                            ReplyDialog(Dialog.ObjectType, Dialog.ObjectID, Dialog.PursuitID, 8, 1);
                            ReplyDialog(Dialog.ObjectType, Dialog.ObjectID, Dialog.PursuitID, 15, 1);
                            Thread.Sleep(1000);
                        }
                    }
                    else if (Location.Equals(_clientLocation, new Location(503, 41, 59)) && nextLocation.MapID == 3014)
                    {
                        Creature npc = GetNearbyNPC("Keane");
                        ClickNPCDialog(npc, "Suomi Help", true);
                        ReplyDialog(1, npc.ID, 0, 2, 1);
                        ReplyDialog(1, npc.ID, 0, 2);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WarpWithNPC] Exception occurred: {ex.ToString()}");
                }
            }
        }
        internal bool ClickNPCDialog(Creature creature, string dialogText, bool click)
        {
            DateTime utcNow = DateTime.UtcNow;
            if (creature != null)
            {
                if (!_server.PursuitIDs.Values.Contains(dialogText))
                {
                    bool flag = false;
                    ClickObject(creature.ID);
                    while (Dialog == null)
                    {
                        if (DateTime.UtcNow.Subtract(utcNow).TotalSeconds > 2.0)
                        {
                            if (flag)
                            {
                                return false;
                            }
                            ClickObject(creature.ID);
                            flag = true;
                        }
                        Thread.Sleep(10);
                    }
                    Dialog.Reply();
                }
                utcNow = DateTime.UtcNow;
                Dialog = null;
                PursuitRequest(1, creature.ID, _server.PursuitIDs.FirstOrDefault((KeyValuePair<ushort, string> keyValuePair_0) => keyValuePair_0.Value == dialogText).Key);
                if (click)
                {
                    while (Dialog == null)
                    {
                        if (DateTime.UtcNow.Subtract(utcNow).TotalSeconds <= 2.0)
                        {
                            Thread.Sleep(10);
                            continue;
                        }
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        internal bool WaitForDialog()
        {
            DateTime utcNow = DateTime.UtcNow;
            while (true)
            {
                if (Dialog == null)
                {
                    if (!(DateTime.UtcNow.Subtract(utcNow).TotalSeconds <= 3.0))
                    {
                        break;
                    }
                    Thread.Sleep(10);
                    continue;
                }
                return true;
            }
            return false;
        }
        internal bool RouteFindByMapID(short mapID)
        {
            return RouteFind(new Location(mapID, 0, 0), 0, true);
        }
        internal bool WithinRange(VisibleObject obj, int range = 12)
        {
            return NearbyObjects.Contains(obj.ID) && _serverLocation.DistanceFrom(obj.Location) <= range;
        }
        internal bool IsLocationSurrounded(Location location)
        {
            if (Player == null) return false;

            // Early return if the player is too close to the location.
            if (Player.Location.DistanceFrom(location) <= 1)
            {
                return false;
            }

            // Gather all relevant creatures' locations within the same map to avoid repeated enumeration.
            var creatureLocations = GetNearbyObjects()
                .OfType<Creature>()
                .Where(creature => (creature.Type == CreatureType.Aisling || creature.Type == CreatureType.Merchant || creature.Type == CreatureType.Normal) && !creature.Location.Equals(location))
                .Select(creature => creature.Location)
                .ToHashSet(); // Using HashSet for O(1) lookups.

            // Get obstacle locations within the same map once to avoid repeated calls.
            var obstacleLocations = GetWarpPoints(location).Where(obstacle => obstacle.MapID == location.MapID).ToHashSet(); // Using HashSet for O(1) lookups.

            // Define adjacent locations based on cardinal directions.
            var adjacentLocations = new[]
            {
                location.Offset(Direction.North),
                location.Offset(Direction.West),
                location.Offset(Direction.South),
                location.Offset(Direction.East)
            };

            // Check each adjacent location for being surrounded conditions.
            foreach (var loc in adjacentLocations)
            {
                bool isOccupiedOrBound = creatureLocations.Contains(loc) || obstacleLocations.Contains(loc) || _map.IsWall(loc);

                // If any adjacent location is not occupied or within bounds, the location is not surrounded.
                if (!isOccupiedOrBound)
                {
                    return false;
                }
            }

            // All adjacent locations are occupied or within bounds, so the location is surrounded.
            return true;
        }




        #region ClientPacket methods

        internal void Pickup(byte inventorySlot, Location location)
        {
            ClientPacket clientPacket = new ClientPacket(7);
            clientPacket.WriteByte(inventorySlot);
            clientPacket.WriteStruct(location.Point);
            Enqueue(clientPacket);
        }
        internal void Drop(byte inventorySlot, Location location, int count = 1)
        {
            ClientPacket clientPacket = new ClientPacket(8);
            clientPacket.WriteByte(inventorySlot);
            clientPacket.WriteStruct(location.Point);
            clientPacket.WriteInt32(count);
            Enqueue(clientPacket);
        }
        internal void DisplayEntityRequest(int id)
        {
            ClientPacket clientPacket = new ClientPacket(12);
            clientPacket.WriteInt32(id);
            Enqueue(clientPacket);
        }
        internal void PublicMessage(byte type, string message)
        {
            ClientPacket clientPacket = new ClientPacket(14);
            clientPacket.WriteByte(type);
            clientPacket.WriteString8(message);
            Enqueue(clientPacket);
        }
        internal void Turn(Direction direction)
        {
            LastTurned = DateTime.UtcNow;
            ClientPacket clientPacket = new ClientPacket(17);
            clientPacket.WriteByte((byte)direction);
            Enqueue(clientPacket);
            _serverDirection = direction;
        }
        internal void Whisper(string targetName, string message)
        {
            ClientPacket clientPacket = new ClientPacket(25);
            clientPacket.WriteString8(targetName);
            clientPacket.WriteString8(message);
            Enqueue(clientPacket);
        }
        internal bool UseItem(string itemName)
        {
            Item item = Inventory.FirstOrDefault((Item item2) => item2.Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase));
            if (item == null)
            {
                ServerMessage(0, $"Item {item} not found in inventory");
                return false;
            }

            if (!EffectsBar.Contains((ushort)Enumerations.EffectsBar.Pramh) && !EffectsBar.Contains((ushort)Enumerations.EffectsBar.Suain) && !_server._stopCasting)
            {
                ClientPacket clientPacket = new ClientPacket(28);
                clientPacket.WriteByte(item.Slot);
                item.LastUsed = DateTime.UtcNow;
                _currentItem = itemName;
                if (itemName == "Sprint Potion")
                {
                    Task.Run(() =>
                    {
                        ClientTab.DelayedUpdateStrangerList();
                    });
                }

                UpdateClientActionText($"{_action} Using {itemName}");
                ReadyToSpell(itemName);
                Enqueue(clientPacket);
                if (itemName == "Two Move Combo")
                {
                    if (_comboScrollCounter <= 1u)
                    {
                        _comboScrollCounter++;
                        _comboScrollLastUsed = DateTime.UtcNow;
                    }
                }
                else if (itemName == "Three Move Combo")
                {
                    if (_comboScrollCounter <= 2u)
                    {
                        _comboScrollCounter++;
                        _comboScrollLastUsed = DateTime.UtcNow;
                    }
                }

                return true;
            }
            return false;
        }
        internal void RequestProfile()
        {
            Enqueue(new ClientPacket(45));
        }
        internal void RequestGroup(string playerName)
        {
            ClientPacket clientPacket = new ClientPacket(46);
            clientPacket.WriteByte(2);
            clientPacket.WriteString8(playerName);
            Enqueue(clientPacket);
        }
        internal void RequestGroupForced(string playerName)
        {
            ClientPacket clientPacket = new ClientPacket(46);
            clientPacket.WriteByte(3);
            clientPacket.WriteString8(playerName);
            Enqueue(clientPacket);
        }
        internal void RefreshRequest(bool waitForCompletion = true)
        {
            if (Interlocked.CompareExchange(ref _isRefreshing, 1, 0) == 0)
            {
                this.Enqueue(new ClientPacket(56));
                this.Bot._lastRefresh = DateTime.UtcNow;
            }

            var expirationTime = DateTime.UtcNow.AddMilliseconds(1500);

            if (waitForCompletion)
            {
                try
                {
                    while (DateTime.UtcNow < expirationTime)
                    {
                        if (this._mapChanged || this._isRefreshing == 0)
                        {
                            this._mapChanged = false;
                            break;
                        }

                        Thread.Sleep(10);
                    }
                }
                catch (ThreadInterruptedException ex)
                {
                    Console.WriteLine("Thread was interrupted.");
                }
                finally
                {
                    this._isRefreshing = 0;
                }
            }
            else
                this._isRefreshing = 0;
        }
        internal void PursuitRequest(byte objType, int objID, ushort pursuitID, params object[] args)
        {
            ClientPacket clientPacket = new ClientPacket(57);
            clientPacket.WriteByte(objType);
            clientPacket.WriteInt32(objID);
            clientPacket.WriteUInt16(pursuitID);
            clientPacket.WriteArray(args);
            Enqueue(clientPacket);
        }
        internal void WithdrawMoney(int objID, int quantity)
        {
            ClientPacket clientPacket = new ClientPacket(57);
            clientPacket.WriteByte(1);
            clientPacket.WriteInt32(objID);
            clientPacket.WriteUInt16(85);
            clientPacket.WriteString8(quantity.ToString());
            Enqueue(clientPacket);
        }
        internal void DepositMoney(int objID, int quantity)
        {
            ClientPacket clientPacket = new ClientPacket(57);
            clientPacket.WriteByte(1);
            clientPacket.WriteInt32(objID);
            clientPacket.WriteInt16(82);
            clientPacket.WriteString8(quantity.ToString());
            Enqueue(clientPacket);
        }
        internal void WithdrawItem(int npcID, string item, int quantity = 0)
        {
            ClientPacket clientPacket = new ClientPacket(57);
            clientPacket.WriteByte(1);
            clientPacket.WriteInt32(npcID);
            clientPacket.WriteInt16(quantity > 0 ? (short)87 : (short)86);
            clientPacket.WriteString8(item);
            if (quantity > 0)
                clientPacket.WriteString8(quantity.ToString());
            Enqueue(clientPacket);
        }
        internal void DepositItem(int npcID, string item, int quantity = 1)
        {
            byte num = 0;
            if (!Inventory.Contains(item))
                ServerMessage((byte)ServerMessageType.ActiveMessage, "You don't own that item.");
            else
                num = Inventory[item].Slot;
            ClientPacket clientPacket = new ClientPacket((byte)57);
            clientPacket.WriteByte((byte)1);
            clientPacket.WriteInt32(npcID);
            clientPacket.WriteInt16(quantity > 1 ? (short)84 : (short)83);
            if (quantity > 1)
                clientPacket.WriteByte((byte)1);
            clientPacket.WriteByte(num);
            if (quantity > 1)
                clientPacket.WriteString8(quantity.ToString());
            Enqueue((Packet)clientPacket);
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
        internal void ReplyDialog(byte objType, int objId, ushort pursuitId, ushort dialogId, byte optionToClick)
        {
            ClientPacket clientPacket = new ClientPacket(58);
            clientPacket.WriteByte(objType);
            clientPacket.WriteInt32(objId);
            clientPacket.WriteUInt16(pursuitId);
            clientPacket.WriteUInt16(dialogId);
            clientPacket.WriteByte(1);
            clientPacket.WriteByte(optionToClick);
            Enqueue(clientPacket);
        }
        internal void ReplyDialog(byte objType, int objId, ushort pursuitId, ushort dialogId, string response)
        {
            ClientPacket clientPacket = new ClientPacket(58);
            clientPacket.WriteByte(objType);
            clientPacket.WriteInt32(objId);
            clientPacket.WriteUInt16(pursuitId);
            clientPacket.WriteUInt16(dialogId);
            clientPacket.WriteByte(2);
            clientPacket.WriteString8(response);
            Enqueue(clientPacket);
        }
        internal bool UseSkill(string skillName)
        {
            Skill skill = Skillbook[skillName];
            if (skill != null && CanUseSkill(skill))
            {
                ClientPacket clientPacket = new ClientPacket(62);
                clientPacket.WriteByte(skill.Slot);
                skill.LastUsed = DateTime.UtcNow;
                if (_currentSkill != skillName)
                {
                    _currentSkill = skillName;
                    if (skillName == "Charge")
                    {
                        ThreadPool.QueueUserWorkItem(_ => ClientTab.DelayedUpdateStrangerList());
                    }
                }
                Enqueue(clientPacket);
                return true;
            }
            return false;
        }
        internal void ClickWorldMap(short mapId, Point point)
        {
            ClientPacket clientPacket = new ClientPacket(63);
            clientPacket.WriteInt32(mapId);
            clientPacket.WriteStruct(point);
            Enqueue(clientPacket);
        }
        internal void RemoveShield()
        {
            ClientPacket clientPacket = new ClientPacket(68);
            clientPacket.WriteByte(3);
            Enqueue(clientPacket);
        }
        internal bool UseSpell(string spellName, Creature target = null, bool staffSwitch = true, bool wait = true)
        {

            if (Spellbook[spellName] == null)
            {
                //ServerMessage(0, $"Spell {spellName} not found in spellbook");
                //Console.WriteLine($"Spell {spellName} not found in spellbook");
                return false;
            }

            //if (target != null)
            //{
            //    int test = _clientLocation.DistanceFrom(target.Location);
            //    ServerMessage((byte)ServerMessageType.TopRight, $"Casting on {target.ID}, {test} spaces");
            //    DisplayTextOverTarget(2, target.ID, $"[Target]");
            //}


            Spell spell = Spellbook[spellName];
            byte castLines = spell.CastLines;

            //Console.WriteLine($"[UseSpell] Attempting to cast {spell.Name}, LastUsed: {spell.LastUsed}, Cooldown: {spell.Cooldown}, Ticks: {spell.Ticks}, Hash: {spell.GetHashCode()}");

            lock (BashLock)
            {

                if (spell == null || !CanUseSpell(spell, target) || ((spellName == "Hide" || spellName == "White Bat Form") && _server._stopCasting))
                {
                    //Console.WriteLine($"[UseSpell] Aborted casting {spellName} on Creature ID: {target?.ID}. Reason: Validation failed.");
                    _isCasting = false;
                    return false;
                }

                if (staffSwitch)
                {
                    if (!UseOptimalStaff(spell, out castLines))
                    {
                        //Console.WriteLine("Error in Client.cs UseSpell: staffSwitch was true but CheckWeaponCastLines returned false");
                        return false;
                    }
                }

                CastedTarget = (target ?? Player);

                //if (spellName.Contains("cradh"))
                //{
                //    Console.WriteLine($"[Casting] {spellName} on Creature ID: {CastedTarget.ID}, Name: {CastedTarget.Name}, HashCode: {CastedTarget.GetHashCode()}, IsCursed: {CastedTarget.IsCursed}, LastCursed: {CastedTarget.GetState<DateTime>(CreatureState.LastCursed)}, CurseDuration: {CastedTarget.GetState<double>(CreatureState.CurseDuration)}");
                //}
                //if (spellName.Contains("fas"))
                //{
                //    Console.WriteLine($"[Casting] {spellName} on Creature ID: {CastedTarget.ID}, Name: {CastedTarget.Name}, HashCode: {CastedTarget.GetHashCode()}, IsFassed: {CastedTarget.IsFassed}, LastFassed: {CastedTarget.GetState<DateTime>(CreatureState.LastFassed)}, FasDuration: {CastedTarget.GetState<double>(CreatureState.FasDuration)}");
                //}

                if (ReadyToSpell(spell.Name))
                {
                    var existingEntry = _spellHistory.FirstOrDefault(cts => cts.Creature.ID == CastedTarget.ID && cts.Spell.Name == spell.Name);

                    if (existingEntry != null)
                    {
                        if (existingEntry.CooldownEndTime > DateTime.UtcNow)
                        {
                            //Console.WriteLine($"[Debug] Skipped adding {CreatureTarget.ID} to _spellHistory due to cooldown.");
                        }
                        else
                        {
                            // Update cooldown end time for re-casting
                            existingEntry.CooldownEndTime = DateTime.UtcNow.AddSeconds(1);
                            //Console.WriteLine($"[Debug] Updated cooldown for {CreatureTarget.ID} in _spellHistory.");
                        }
                    }
                    else
                    {
                        var newEntry = new SpellEntry(spell, CastedTarget)
                        {
                            CooldownEndTime = DateTime.UtcNow.AddSeconds(1)
                        };
                        _spellHistory.Add(newEntry);
                        //Console.WriteLine($"[UseSpell] Casting '{spellName}' on Creature ID: {target?.ID}, CooldownEndTime: {newEntry.CooldownEndTime}");
                        //Console.WriteLine($"[Debug] Added to _spellHistory: Spell = {spell.Name}, Creature ID = {CreatureTarget.ID}, Time = {DateTime.UtcNow}");
                    }
                }
                //else
                //{
                //return false;
                //}


                if (CastedSpell != spell)
                {
                    UpdateClientActionText($"{_action} Casting {spellName}");
                }

                ClientPacket clientPacket = new ClientPacket(15);
                clientPacket.WriteByte(spell.Slot);

                if (target != null && (NearbyObjects.Contains(target.ID) || target is Player))
                {
                    clientPacket.WriteInt32(target.ID);
                    clientPacket.WriteStruct(target.Location);
                }



                if (castLines > 0)
                {
                    if (_isWalking)
                    {
                        _isCasting = false;
                        return false;
                    }
                    string[] chantArray = LoadSavedChants(spellName);
                    lock (CastLock)
                    {
                        _isCasting = true;
                        ClientPacket chantPacket = new ClientPacket(77);//begin chant
                        chantPacket.WriteByte(castLines);
                        Enqueue(chantPacket);
                        for (int i = 0; i < castLines; i++)
                        {
                            if (ClientTab.hideLinesCbox.Checked)
                            {
                                DisplayChant(" ");
                            }
                            else if (!string.IsNullOrWhiteSpace(ClientTab.customLinesBox.Text) && !Bot._rangerNear)
                            {
                                DisplayChant(ParseCustomSpellLines());
                            }
                            else if (!string.IsNullOrWhiteSpace(chantArray[i]))
                            {
                                DisplayChant(chantArray[i]);
                            }
                            DateTime utcNow = DateTime.UtcNow;
                            Client client = (target != null) ? _server.GetClient(target.Name) : this;
                            while (DateTime.UtcNow.Subtract(utcNow).TotalMilliseconds < 1000.0)
                            {
                                if (!IsValidSpell(client, spell.Name, target))
                                {
                                    _isCasting = false;
                                    return false;
                                }
                                Thread.Sleep(10);
                            }
                            if (!_isCasting || !CanUseSpell(spell, target))
                            {
                                _isCasting = false;
                                return false;
                            }
                        }
                        if (ClientTab.hideLinesCbox.Checked)
                        {
                            DisplayChant(" ");
                        }
                        else
                        {
                            if (spell.Name == "fas spiorad" && !Bot._needFasSpiorad && !Bot._manaLessThanEightyPct)
                            {
                                _isCasting = false;
                                return false;
                            }
                            DisplayChant(spell.Name);
                        }
                    }
                }
                if (spell.Name == "fas spiorad" && ClientTab.safeFSCbox.Checked)
                {
                    if (!int.TryParse(ClientTab.safeFSTbox.Text, out int result))
                    {
                        if (Stats.CurrentHP < Stats.MaximumMP * 0.5)
                        {
                            _isCasting = false;
                            return false;
                        }
                    }
                    else if (Stats.CurrentHP < result)
                    {
                        _isCasting = false;
                        return false;
                    }
                }
                //Console.WriteLine($"[UseSpell] Casting {spellName} on Creature ID: {target?.ID}, Name: {target?.Name}");

                Enqueue(clientPacket);
                _spellCounter++;
                Bot._spellTimer = DateTime.UtcNow;
                spell.LastUsed = DateTime.UtcNow;
                _isCasting = false;
                CastedSpell = (wait ? spell : null);
                if (spell.Name != "Gem Polishing" || spell.Name.Contains("Prayer"))
                {
                    CastedSpell = spell;
                }
                return !wait || WaitForSpellChant();
            }
        }
        internal void DisplayChant(string chant)
        {
            ClientPacket clientPacket = new ClientPacket(78);
            clientPacket.WriteString8(chant);
            Enqueue(clientPacket);
        }
        internal void ClickObject(int objectId)
        {
            ClientPacket clientPacket = new ClientPacket(67);
            clientPacket.WriteByte(1);
            clientPacket.WriteInt32(objectId);
            Enqueue(clientPacket);
        }
        internal void ClickObject(Location location)
        {
            Point point = new Point(location.X, location.Y);
            Console.WriteLine($"[ClickObject] Clicking object at {point}");
            ClientPacket clientPacket = new ClientPacket(67);
            clientPacket.WriteByte(3);
            clientPacket.WriteStruct(point);
            clientPacket.WriteByte(1);
            Enqueue(clientPacket);
        }
        internal void RaiseStrStat()
        {
            ClientPacket clientPacket = new ClientPacket((byte)71);
            clientPacket.WriteByte((byte)1);
            Enqueue((Packet)clientPacket);
        }


        #endregion

        #region SereverPacket methods
        internal void EnableMapZoom()
        {
            ServerPacket serverPacket = new ServerPacket(5);
            serverPacket.WriteUInt32(PlayerID);
            serverPacket.WriteUInt16(0);
            serverPacket.WriteByte((byte)(GetCheats(Cheats.ZoomableMap) ? 2 : Path));
            serverPacket.WriteUInt16(0);
            Enqueue(serverPacket);
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
                if (GetCheats(Cheats.NoBlind) && !InArena)
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
        internal void DisplayTextOverTarget(byte type, int id, string message)
        {
            ServerPacket serverPacket = new ServerPacket(13);
            serverPacket.WriteByte(type);
            serverPacket.WriteInt32(id);
            serverPacket.WriteString8(message);
            this.Enqueue(serverPacket);
        }
        internal void RemoveObject(int objId)
        {
            ServerPacket serverPacket = new ServerPacket(14);
            serverPacket.WriteUInt32((uint)objId);
            Enqueue(serverPacket);
        }
        internal void SendAnimation(ushort animation, short speed)
        {
            ServerPacket serverPacket = new ServerPacket(41);
            serverPacket.WriteUInt32(this.PlayerID);
            serverPacket.WriteUInt32(this.PlayerID);
            serverPacket.WriteUInt16(animation);
            serverPacket.WriteUInt16(animation);
            serverPacket.WriteInt16(speed);
            Enqueue(serverPacket);

        }
        internal void AddSkill(Skill skill)
        {
            ServerPacket serverPacket = new ServerPacket(44);
            serverPacket.WriteByte(skill.Slot);
            serverPacket.WriteUInt16(skill.Sprite);
            serverPacket.WriteString8(skill.Name);
            Enqueue(serverPacket);
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
            ushort spriteID = player.SpriteID;
            if (player == Player && SpriteOverrideEnabled)
            {
                spriteID = _spriteOverride;
            }
            ServerPacket serverPacket = new ServerPacket(51);
            serverPacket.WriteStruct(player.Location);
            serverPacket.WriteByte((byte)player.Direction);
            serverPacket.WriteInt32(player.ID);
            if (spriteID == 0)
            {
                serverPacket.WriteUInt16(player.HeadSprite);
                if (player.BodySprite == 0 && GetCheats(Cheats.SeeHidden) && !InArena)
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
                if (map != null && map.Name?.Contains("Arena") == false && !InArena)
                    serverPacket.WriteString8(string.Empty);
                else
                    serverPacket.WriteString8(player.Name);
            }
            else
            {
                serverPacket.WriteString8(player.Name);
            }
            serverPacket.WriteString8(player.GroupName);
            Enqueue(serverPacket);
        }
        internal void Walk(Direction dir)
        {

            //Console.WriteLine($"Attempting to walk in direction: {dir}"); // Log the direction

            if (Dialog == null || !_server._stopWalking || dir != Direction.Invalid || _isRefreshing == 1)
            {
                _walkCallCount++;
                //Console.WriteLine($"Walk method called {walkCallCount} times");

                //Console.WriteLine("Walk conditions met. Proceeding..."); // Debugging: Log that conditions are met

                LastStep = DateTime.UtcNow;
                //Console.WriteLine($"LastStep set to: {LastStep}"); // Debugging: Log LastStep update

                if (_serverDirection != dir)
                {
                    //Console.WriteLine($"Turning from {_serverDirection} to {dir}"); // Debugging: Log direction change
                    Turn(dir);
                }

                _hasWalked = true;
                //Console.WriteLine("shouldRefresh set to true"); // Debugging: Log shouldRefresh update


                //Console.WriteLine($"Preparing packets for PlayerID: {PlayerID} at {_clientLocation} moving {dir}"); // Debugging: Log enqueueing of server packet
                ClientPacket clientPacket = new ClientPacket(6); // walk
                clientPacket.WriteByte((byte)dir);
                clientPacket.WriteByte(StepCount++);
                //Console.WriteLine($"Client packet prepared. StepCount: {StepCount - 1}"); // Debugging: Log ClientPacket preparation
                Enqueue(clientPacket);


                ServerPacket serverPacket = new ServerPacket(12); // creaturewalk
                serverPacket.WriteUInt32(PlayerID);
                serverPacket.WriteStruct(_clientLocation);
                serverPacket.WriteByte((byte)dir);

                Enqueue(serverPacket);


                _clientLocation = _clientLocation.Offset(dir);
                //Console.WriteLine($"Client location updated to: {_clientLocation}"); // Debugging: Log client location update

                LastMoved = DateTime.UtcNow;
                //Console.WriteLine($"[WalkLastMoved set to: {LastMoved}"); // Debugging: Log LastMoved update

                UpdateClientActionText($"{_action} Walking {dir}");
            }
            else
            {
                _isWalking = false;
                //Console.WriteLine("Walk aborted due to conditions not met. _isWalking set to false."); // Debugging: Log abort and _isWalking update
                Thread.Sleep(100); // Consider logging this delay for clarity if necessary
            }
        }
        internal void Cooldown(bool isSkill, byte slot, uint ticks)
        {
            ServerPacket serverPacket = new ServerPacket(63);
            serverPacket.WriteBoolean(isSkill);
            serverPacket.WriteByte(slot);
            serverPacket.WriteUInt32(ticks);
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
                ProcessSendQueue();
                ProcessReceiveQueue();
                Thread.Sleep(5);
            }

            HandleDisconnect();
            RestoreWindow();
            CleanupClient();
        }

        private void ProcessSendQueue()
        {
            lock (_sendQueue)
            {
                while (_sendQueue.Count > 0)
                {
                    Packet packet = _sendQueue.Dequeue();
                    HandlePacketSending(packet);
                }
            }
        }

        private void HandlePacketSending(Packet packet)
        {
            Socket socket;
            byte[] array;

            if (packet is ClientPacket clientPacket)
            {
                LogClientPacket(clientPacket);
                PrepareClientPacketForSending(clientPacket);
                socket = _serverSocket;
            }
            else if (packet is ServerPacket serverPacket)
            {
                LogServerPacket(serverPacket);
                PrepareServerPacketForSending(serverPacket);
                socket = _clientSocket;
            }
            else
            {
                return;
            }

            array = packet.CreatePacket();

            try
            {
                socket.BeginSend(array, 0, array.Length, SocketFlags.None, EndSend, socket);
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }

        private void PrepareClientPacketForSending(ClientPacket clientPacket)
        {
            if (clientPacket.IsDialog) clientPacket.EncryptDialog();
            if (clientPacket.Opcode == 98) _serverOrdinal = clientPacket.Sequence;
            if (clientPacket.ShouldEncrypt)
            {
                clientPacket.Sequence = _serverOrdinal++;
                clientPacket.Encrypt(_crypto);
            }
        }

        private void PrepareServerPacketForSending(ServerPacket serverPacket)
        {
            if (serverPacket.ShouldEncrypt)
            {
                serverPacket.Sequence = _clientOrdinal++;
                serverPacket.Encrypt(_crypto);
            }
        }

        private void LogClientPacket(ClientPacket clientPacket)
        {
            ClientPacket? clientPacketToLog = clientPacket.Copy();
            if (clientPacketToLog != null && ClientTab != null && !ClientTab.IsDisposed)
            {
                ClientTab.LogPackets(clientPacketToLog);
            }
        }

        private void LogServerPacket(ServerPacket serverPacket)
        {
            ServerPacket? serverPacketToLog = serverPacket.Copy();
            if (serverPacketToLog != null && ClientTab != null && !ClientTab.IsDisposed)
            {
                ClientTab.LogPackets(serverPacketToLog);
            }
        }

        private void LogException(Exception ex)
        {
            string logDirectory = AppDomain.CurrentDomain.BaseDirectory + "CrashLogs\\";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            string logPath = logDirectory + DateTime.Now.ToString("MM-dd-HH-yyyy h mm tt") + ".log";
            File.WriteAllText(logPath, ex.ToString());
        }

        private void ProcessReceiveQueue()
        {
            lock (_receiveQueue)
            {
                while (_receiveQueue.Count > 0)
                {
                    Packet packet = _receiveQueue.Dequeue();
                    HandlePacketReceiving(packet);
                }
            }
        }

        private void HandlePacketReceiving(Packet packet)
        {
            if (packet.ShouldEncrypt) packet.Decrypt(_crypto);
            bool shouldEnqueue = true;

            if (packet is ClientPacket clientPacket)
            {
                shouldEnqueue = HandleClientPacket(clientPacket);
            }
            else if (packet is ServerPacket serverPacket)
            {
                shouldEnqueue = HandleServerPacket(serverPacket);
            }

            if (shouldEnqueue)
            {
                Enqueue(packet);
            }
        }

        private bool HandleClientPacket(ClientPacket clientPacket)
        {
            if (clientPacket.IsDialog) clientPacket.DecryptDialog();

            ClientMessageHandler clientHandler = _server.ClientMessage[clientPacket.Opcode];
            return TryHandleClientPacket(clientHandler, clientPacket);
        }

        private bool HandleServerPacket(ServerPacket serverPacket)
        {
            ServerMessageHandler serverHandler = _server.ServerMessage[serverPacket.Opcode];
            return TryHandleServerPacket(serverHandler, serverPacket);
        }

        private bool TryHandleClientPacket(ClientMessageHandler handler, ClientPacket packet)
        {
            bool result = true;

            if (handler != null)
            {
                lock (Server.SyncObj)
                {
                    try
                    {
                        result = handler(this, packet);
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        private bool TryHandleServerPacket(ServerMessageHandler handler, ServerPacket packet)
        {
            bool result = true;

            if (handler != null)
            {
                lock (Server.SyncObj)
                {
                    try
                    {
                        result = handler(this, packet);
                    }
                    catch
                    {
                        result = false;
                    }
                }
            }

            return result;
        }


        private void HandleDisconnect()
        {
            try
            {
                _clientSocket.Disconnect(false);
                _serverSocket.Disconnect(false);
            }
            catch
            {
            }
        }

        private void RestoreWindow()
        {
            try
            {
                NativeMethods.SetForegroundWindow((int)hWnd);
                NativeMethods.ShowWindow(hWnd, 1u);
                FlashWindowEx(Process.GetProcessById(processId).MainWindowHandle);
            }
            catch
            {
            }
        }

        private void CleanupClient()
        {
            Thread.Sleep(500);
            lock (_server._clientListLock)
            {
                _server._clientList.Remove(this);
            }
            Thread.Sleep(100);
            _server._mainForm.RemoveClientTab(this);
        }


        internal bool FlashWindowEx(IntPtr hWnd)
        {
            if (IsOSCompatible)
            {
                Interop.FLASHWINFO flashwinfo = NativeMethods.fInfo(hWnd, 15u, uint.MaxValue, 0u);
                return NativeMethods.FlashWindowEx(ref flashwinfo);
            }
            return false;
        }

        internal void CheckNetStat()
        {
            try
            {
                if (TryGetPortFromRemoteEndPoint(_clientSocket.RemoteEndPoint.ToString(), out int port))
                {
                    string netStatData = GetNetStatData();

                    if (TryGetProcessIdForPort(netStatData, port, out int procID))
                    {
                        processId = procID;
                        hWnd = Process.GetProcessById(processId).MainWindowHandle;

                        NativeMethods.SetWindowText(hWnd, Name);
                    }
                }
            }
            catch
            {
                DisconnectWait();
            }
        }

        private bool TryGetPortFromRemoteEndPoint(string remoteEndPoint, out int port)
        {
            return int.TryParse(remoteEndPoint.Replace("127.0.0.1:", ""), out port);
        }

        private string GetNetStatData()
        {
            string netStatData = string.Empty;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Arguments = "/c netstat -a -n -o";
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        netStatData += e.Data + Environment.NewLine;
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }

            return netStatData;
        }

        private bool TryGetProcessIdForPort(string netStatData, int port, out int processId)
        {
            processId = 0;
            string pattern = $@"TCP\s+127.0.0.1:{port}\s+127.0.0.1:2610\s+ESTABLISHED\s+([0-9]+)";

            var match = Regex.Match(netStatData, pattern);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int procID))
            {
                processId = procID;
                return true;
            }

            return false;
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


