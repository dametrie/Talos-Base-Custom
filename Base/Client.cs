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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;
using System.Xml.Linq;
using Talos.Properties;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Talos.Base;


namespace Talos.Base
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

   


        internal Location _clientLocation;
        internal Location _serverLocation;
        internal DateTime _lastWorldShout = DateTime.MinValue;
        internal DateTime _lastMapChange = DateTime.MinValue;
        internal DateTime _comboScrollLastUsed = DateTime.MinValue;
        internal DateTime _arenaAnnounceTimer = DateTime.MinValue;
        internal Map _map;
        internal WorldMap _worldMap;
        internal Cheats _cheats;
        internal Direction _clientDirection;
        internal Direction _serverDirection;
        internal MapFlags _mapFlags;
        internal EffectsBar _status;
        internal TemuairClass _temuairClass;
        internal MedeniaClass _medeniaClass;
        internal PreviousClass _previousClass;
        internal Dugon _dugon;
        internal Element _element;
        internal string _npcDialog;
        internal string _currentSkill = "";
        internal string _currentItem = "";
        internal bool _safeScreen;
        internal bool _atDoor;
        internal bool _isCasting;
        internal bool _isWalking;
        internal bool _isRefreshing;
        internal bool _canRefresh;
        internal bool _hasLabor;
        internal bool _bool_39;
        internal bool _overrideMapFlags;
        internal bool _mapChangePending;
        internal bool _isRegistered = true;
        internal bool _isCheckingBelt;
        internal double _walkSpeed = 420.0;
        internal ushort _monsterFormID = 1;
        internal int _spellCounter;
        private int _customSpellLineCounter;
        internal byte _comboScrollCounter;
        internal Spell _currentSpell;
        internal System.Windows.Forms.Timer _spellTimer;


        internal readonly object Lock = new object();

        internal AutoResetEvent _walkSignal = new AutoResetEvent(false);

        internal List<Staff> _staffList = new List<Staff>();
        internal List<MeleeWeapon> _meleeList = new List<MeleeWeapon>();
        internal List<Bow> _bowList = new List<Bow>();
        internal List<CreatureToSpell> _creatureToSpellList = new List<CreatureToSpell>();

        internal BindingList<int> _worldObjectBindingList = new BindingList<int>();
        internal BindingList<int> _creatureBindingList = new BindingList<int>();

        private readonly List<string> _archerSpells = new List<string>
        {
        "Star Arrow",
        "Frost Arrow",
        "Shock Arrow",
        "Volley",
        "Barrage"
        };

        internal Dictionary<string, string> UserOptions { get; set; } = new Dictionary<string, string>();
        internal Dictionary<int, WorldObject> WorldObjects { get; private set; } = new Dictionary<int, WorldObject>();
        internal Dictionary<string, Player> NearbyPlayers { get; private set; } = new Dictionary<string, Player>();
        internal Dictionary<string, Player> NearbyGhosts { get; private set; } = new Dictionary<string, Player>();
        internal Dictionary<string, Creature> NearbyNPC { get; private set; } = new Dictionary<string, Creature>();
        internal Dictionary<string, int> ObjectID { get; private set; } = new Dictionary<string, int>();
        internal Dictionary<string, byte> AvailableSpellsAndCastLines { get; set; } = new Dictionary<string, byte>();

        internal HashSet<int> CreatureHashSet { get; private set; } = new HashSet<int>();
        internal HashSet<int> ObjectHashSet { get; private set; } = new HashSet<int>();
        internal HashSet<ushort> EffectsBarHashSet { get; set; } = new HashSet<ushort>();
        internal HashSet<Door> Doors { get; private set; } = new HashSet<Door> { };
        internal HashSet<string> AllyListHashSet { get; set; } = new HashSet<string> { };

        internal static HashSet<string> AoSuainHashSet = new HashSet<string>(new string[3]
        {
            "ao suain",
            "ao pramh",
            "Leafhopper Chirp"
        }, StringComparer.CurrentCultureIgnoreCase);
        internal int _stuckCounter;
        internal DateTime _lastFrozen;
        internal int stuckCounter;
        internal bool _inventoryFull;
        internal string _offenseElement;
        internal bool _shouldEquipBow;

        internal Bot Bot { get; set; }
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
        internal DateTime LastStep { get; set; }
        internal DateTime LastMoved { get; set; }
        internal DateTime LastTurned { get; set; }
        internal uint PlayerID { get; set; }
        internal bool InMonsterForm { get; set; }
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
        internal Creature CreatureTarget { get; set; }
        internal Nation Nation { get; set; }

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

        internal byte Ability => Stats.Ability;
        internal byte Level => Stats.Level;
        internal uint ToNextLevel => Stats.ToNextLevel;
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
            _spellTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _spellTimer.Tick += SpellTimerTick;
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
        internal bool HasEffect(EffectsBar effectID) => _status.HasFlag(effectID);
        internal void ClearEffect(EffectsBar effectID) => _status &= ~effectID;
        internal void AddEffect(EffectsBar effectID)
        {
            if (!HasEffect(effectID))
                _status |= effectID;
        }
        internal bool QueryEffectsBar(ushort effectID) => EffectsBarHashSet.Contains(effectID);
        internal bool GetMapFlags(MapFlags flagID) => _mapFlags.HasFlag(flagID);
        internal void SetMapFlags(MapFlags flagID) => _mapFlags |= flagID;
        internal void SetTemuairClass(TemuairClass temClass) => _temuairClass |= temClass;
        internal void SetMedeniaClass(MedeniaClass medClass) => _medeniaClass |= medClass;
        internal void SetPreviousClass(PreviousClass previousClass) => _previousClass |= previousClass;
        internal void SetDugon(Dugon color) => _dugon |= color;
        internal bool GetCheats(Cheats value) => _cheats.HasFlag(value);
        internal void SetCheats(Cheats value) => _cheats |= value;
        internal void setCheats2(Cheats value) => _cheats &= (Cheats)(byte)(~(uint)value);
        internal void SetStatUpdateFlags(StatUpdateFlags flags) => Attributes(flags, Stats);
        internal bool HasItem(string itemName) => Inventory.HasItem(itemName);
        internal bool HasSkill(string skillName) => Skillbook[skillName] != null;
        internal bool HasSpell(string spellName) => Spellbook[spellName] != null;
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
            if (!HasEffect(EffectsBar.Pramh) || !HasEffect(EffectsBar.Suain))
                return true;

            return false;
        }
        internal bool CanUseSkill(Skill skill)
        {
            if (skill.CanUse && _map.CanUseSkills && !HasEffect(EffectsBar.Pramh) && !HasEffect(EffectsBar.Suain))
                return true;

            return false;
        }
        internal bool CanUseSpell(Spell spell, Creature creature = null)
        {
            if (spell.CanUse && _map.CanUseSpells)
            {
                if (HasEffect(EffectsBar.Pramh) || HasEffect(EffectsBar.Suain))
                    return AoSuainHashSet.Contains(spell.Name);

                return true; // If neither Pramh nor Suain is active, always return true
            }
            return false;
        }
        internal List<Player> GetNearbyAllies()
        {
            List<Player> list = new List<Player>();
            if (Monitor.TryEnter(Server.Lock, 1000))
            {
                try
                {
                    foreach (Player value in NearbyPlayers.Values)
                    {
                        if (AllyListHashSet.Contains(value.Name))
                        {
                            list.Add(value);
                        }
                    }
                    return list;
                }
                finally
                {
                    Monitor.Exit(Server.Lock);
                }
            }
            return list;
        }
        private bool IsValidSpell(Client client, string spellName, Creature creature)
        {
            // Guard clause: Reject spell if Suain effect is active and spell is not allowed
            if (ShouldRejectSpellDueToSuain(client, spellName))
            {
                return false;
            }

            // Guard clause: Make sure we aren't in Dojo
            if (!_map.Name.Contains("Training Dojo"))
            {
                // Reject spell based on spell name
                if (ShouldRejectSpellDueToSpellName(spellName))
                {
                    return false;
                }

                // Reject spell based on special conditions
                if (ShouldRejectSpellDueToConflict(spellName, creature, client))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ReadyToSpell(string spell)
        {
            try
            {
                uint num = Utility.CalculateFNV(spell);
                switch (num)
                {
                    case 72064257: // mor dion
                    case 249286892: // Iron Skin
                    case 3740145550: // asgall faileas
                    case 3777649476: // Stone Skin
                    case 2579487986: // Wings of Protection
                    case 2454795333: // draco stance
                    case 1484963323: // deireas faileas
                    case 1645955527: // dion
                        return true;

                    case 420187390: // beag fas nadur
                    case 2476745328: // fas nadur
                    case 1149628551: // mor fas nadur
                    case 107956092: // ard fas nadur
                        if (!CreatureTarget.IsFassed)
                        {
                            CreatureTarget.LastFassed = DateTime.UtcNow;
                            CreatureTarget.FasDuration = 2.0;
                        }
                        return true;

                    case 195270534: // Wake Scroll
                        if (spell == "Wake Scroll")
                        {
                            foreach (Player player in GetNearbyAllies())
                            {
                                player.SpellAnimationHistory[117] = DateTime.MinValue;
                                player.SpellAnimationHistory[32] = DateTime.MinValue;
                            }
                        }
                        return false;

                    case 2848971440: // beag cradh
                    case 1154413499: // cradh
                    case 1281358573: // mor cradh
                    case 2118188214: // ard cradh
                    case 1928539694: // Dark Seal
                    case 219207967: // Darker Seal
                    case 928817768: // Demise
                        if (!CreatureTarget.IsCursed)
                        {
                            CreatureTarget.LastCursed = DateTime.UtcNow;
                            CreatureTarget.CurseDuration = 0.3;
                            CreatureTarget.Curse = spell;
                        }
                        return true;

                    case 2112563240: // beag naomh aite
                    case 291448073: // naomh aite
                    case 2761324515: // mor naomh aite
                    case 443271170: // ard naomh aite
                        if (!CreatureTarget.IsAited)
                        {
                            CreatureTarget.LastAited = DateTime.UtcNow;
                            CreatureTarget.AiteDuration = 2.0;
                        }
                        return true;

                    case 810175405: // ao suain
                    case 894297607: // Leafhopper Chirp
                        CreatureTarget.SpellAnimationHistory[40] = DateTime.MinValue;
                        return false;

                    case 1046347411: // suain
                        if (spell == "suain")
                            if (!CreatureTarget.IsWFF)
                                CreatureTarget.SpellAnimationHistory[40] = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 0, 3, 500));
                        return true;

                    case 2030226177: // armachd
                        if (CreatureTarget.SpellAnimationHistory.ContainsKey(20) && DateTime.UtcNow.Subtract(CreatureTarget.SpellAnimationHistory[20]).TotalMinutes < 2.5)
                            CreatureTarget.SpellAnimationHistory[20] = DateTime.UtcNow.Subtract(new TimeSpan(0, 2, 25));
                        return true;

                    case 2592944103: // Mesmerize
                        if (!CreatureTarget.IsAsleep)
                            CreatureTarget.SpellAnimationHistory[117] = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 0, 3, 500));
                        return true;

                    case 3219892635: // beag pramh
                    case 2647647615: // pramh
                        if (!CreatureTarget.IsAsleep)
                            CreatureTarget.SpellAnimationHistory[32] = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 0, 3, 500));
                        return true;

                    case 2756163491: // Fungus Beetle Extract
                        foreach (Player player in GetNearbyAllies())
                        {
                            player.SpellAnimationHistory[25] = DateTime.MinValue;
                            player.SpellAnimationHistory[247] = DateTime.MinValue;
                            player.SpellAnimationHistory[295] = DateTime.MinValue;
                        }
                        return false;

                    case 674409180: // Lyliac Plant
                    case 2793184655: // Lyliac Vineyard
                        Bot._needFasSpiorad = true;
                        return false;

                    case 2996522388: //ao puinsein
                        {
                            CreatureTarget.SpellAnimationHistory[25] = DateTime.MinValue;
                            CreatureTarget.SpellAnimationHistory[247] = DateTime.MinValue;
                            CreatureTarget.SpellAnimationHistory[295] = DateTime.MinValue;
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
                        if (!CreatureTarget.IsFrozen)
                        {
                            CreatureTarget.SpellAnimationHistory[235] = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 0, 1));
                        }
                        return true;

                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }
        private bool ShouldRejectSpellDueToSuain(Client client, string spellName)
        {
            return HasEffect(EffectsBar.Suain) &&
                   (client == null || !IsAllowedSuainSpell(spellName));
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
                case 291448073: // naomh aite
                case 420187390: // beag fas nadur
                case 443271170: // ard naomh aite
                case 810175405: // ao suain
                case 928817768: // Demise
                case 1046347411: // pramh
                case 1149628551: // cradh
                case 1154413499: // mor fas nadur
                case 1928539694: // Dark Seal
                case 2112563240: // beag naomh aite
                case 2118188214: // ard cradh
                case 2454795333: // fas nadur
                case 2579487986: // Mesmerize
                case 2647647615: // pramh
                case 2761324515: // mor naomh aite
                case 2848971440: // beag cradh
                case 3000206623: // Frost Arrow 9
                case 3050539480: // Frost Arrow 4
                case 3067317099: // Frost Arrow 5
                case 3084094718: // Frost Arrow 6
                case 3100872337: // Frost Arrow 7
                case 3134427575: // Frost Arrow 1
                case 3151205194: // Frost Arrow 2
                case 3635920463: // fas spiorad
                case 3777649476: // beag pramh
                    return false;
                default:
                    return true;
            }
        }
        private bool ShouldRejectSpellDueToConflict(string spellName, Creature creature, Client client)
        {
            switch (spellName)
            {
                case "suain":
                    return creature != null && creature.IsWFF;
                case "beag cradh":
                case "cradh":
                case "mor cradh":
                case "ard cradh":
                case "Dark Seal":
                case "Darker Seal":
                case "Demise":
                    return creature != null && creature.IsCursed;
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
                    return creature != null && creature.IsFrozen;
                case "beag pramh":
                case "pramh":
                case "Mesmerize":
                    return client != null && client.HasEffect(EffectsBar.Pramh);
                case "fas spiorad":
                    return !Bot._needFasSpiorad && !Bot._manaLessThanEightyPct;
                case "beag fas nadur":
                case "fas nadur":
                case "mor fas nadur":
                case "ard fas nadur":
                    return client != null && client.HasEffect(EffectsBar.FasNadur);
                case "Leafhopper Chirp":
                    return false; // Always allowed
                case "ao suain":
                    return client != null && client.HasEffect(EffectsBar.Suain);
                case "beag naomh aite":
                case "naomh aite":
                case "mor naomh aite":
                case "ard naomh aite":
                    return client != null && client.HasEffect(EffectsBar.Aite);
                default:
                    return false;
            }
        }
        internal bool ClearSpell()
        {
            DateTime utcNow = DateTime.UtcNow;
            while (true)
            {
                if (_currentSpell != null)
                {
                    if (!(DateTime.UtcNow.Subtract(utcNow).TotalSeconds <= 1.5))
                    {
                        break;
                    }
                    Thread.Sleep(5);
                    continue;
                }
                return true;
            }
            _currentSpell = null;
            return false;
        }

        internal void UpdateCurseTargets(Client invokingClient, int creatureID, string curseName)
        {
            if (!invokingClient._map.Name.Contains("Plamit"))
            {
                return;
            }

            try
            {
                foreach (Client targetClient in _server._clientList)
                {
                    if (targetClient.WorldObjects.TryGetValue(creatureID, out WorldObject worldObject) && worldObject is Creature creature)
                    {
                        creature.CurseDuration = Spell.GetSpellDuration(curseName);
                        creature.LastCursed = DateTime.UtcNow;
                        creature.Curse = curseName;
                    }
                    else if (invokingClient.WorldObjects.TryGetValue(creatureID, out WorldObject originalObject) && originalObject is Creature originalCreature)
                    {
                        Creature newCreature = new Creature(originalCreature.ID, originalCreature.Name, originalCreature.Sprite, (byte)originalCreature.Type, originalCreature.Location, originalCreature.Direction);
                        newCreature.CurseDuration = Spell.GetSpellDuration(curseName);
                        newCreature.LastCursed = DateTime.UtcNow;
                        newCreature.Curse = curseName;
                        targetClient.WorldObjects[creatureID] = newCreature;
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred when trying to update curse targets: {ex.Message}");
            }
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

        private bool CheckWeaponCastLines(Spell spell, out byte castLines)
        {
            bool swappingWeapons = false;
            castLines = spell.CastLines;

            Item obj = EquippedItems[1];
            Bow bow = (obj != null && obj.IsBow) ? EquippedItems[1].Bow : new Bow();
            Staff staff = (obj != null && obj.IsStaff) ? EquippedItems[1].Staff : new Staff();
            MeleeWeapon meleeWeapon = (obj != null && obj.IsMeleeWeapon) ? EquippedItems[1].Melee : new MeleeWeapon();

            bool hasArcherSpells = _archerSpells.Any(spellName => spell.Name.Contains(spellName) || spell.Name.Equals(spellName, StringComparison.InvariantCultureIgnoreCase));
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
                    Console.WriteLine($"Equipping {bowName} to cast {spell.Name}");
                    UseItem(bowName);

                    while (EquippedItems[1]?.Name.Equals(bowName, StringComparison.CurrentCultureIgnoreCase) != true)
                    {
                        if (DateTime.UtcNow.Subtract(utcNow).TotalSeconds <= 2.0)
                        {
                            ClientTab.Invoke((Action)delegate
                            {
                                //ClientTab.currentAction.Text = action + "Swapping to " + Bow.Name;//Adam
                            });
                            Thread.Sleep(5);
                            continue;
                        }
                        return false;
                    }
                }
            }
            else
            {
                var bestStaff = Inventory
                    .Where(item => item.IsStaff && item.Staff.CanUse(Ability, Level, ToNextLevel, _temuairClass))
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
                        if (DateTime.UtcNow.Subtract(utcNow).TotalSeconds <= 2.0)
                        {
                            ClientTab.Invoke((Action)delegate
                            {
                                //ClientTab.currentAction.Text = action + "Swapping to " + staff.Name;//Adam
                            });
                            Thread.Sleep(5);
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
                item.Melee = _meleeList.First(MeleeWeapon =>  MeleeWeapon.Name == item.Name);
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

        #region Packet methods

        internal void DisplayEntityRequest(int id)
        {
            ClientPacket clientPacket = new ClientPacket(12);
            clientPacket.WriteInt32(id);
            Enqueue(clientPacket);
        }

        internal bool UseItem(string itemName)
        {
            Item item = Inventory.FirstOrDefault((Item item2) => item2.Name.Equals(itemName, StringComparison.CurrentCultureIgnoreCase));
            if (item != null && !HasEffect(EffectsBar.Pramh) && !HasEffect(EffectsBar.Suain) && !_server._stopCasting)
            {
                ClientPacket clientPacket = new ClientPacket(28);
                clientPacket.WriteByte(item.Slot);
                item.LastUsed = DateTime.UtcNow;
                _currentItem = itemName;
                if (itemName == "Sprint Potion")
                {
                    Task.Run(() =>
                    {
                        ClientTab.UpdateStrangerListAfterCharge();
                    });
                }
                ClientTab.Invoke((Action)delegate
                {
                    //ClientTab.currentAction.Text = action + "Using " + itemName;//ADAM
                });
                Enqueue(clientPacket);
                if (!(itemName == "Fungus Beetle Extract"))
                {
                    if (itemName == "Wake Scroll")
                    {
                        foreach (Player player in GetNearbyAllies())
                        {
                            if (player != Player)
                            {
                                player.SpellAnimationHistory[117] = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 5));//Mesmerize
                                player.SpellAnimationHistory[32] = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 5));//Pramh
                            }
                        }
                    }
                }
                else
                {
                    foreach (Player player in GetNearbyAllies())
                    {
                        player.SpellAnimationHistory[25] = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 5));//pink swirl
                        player.SpellAnimationHistory[247] = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 5));//green bubble
                        player.SpellAnimationHistory[295] = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 5));//blue bubble cloud
                    }
                }
                if (itemName == "Two Move Combo")
                {
                    if ((uint)_comboScrollCounter <= 1u)
                    {
                        _comboScrollCounter++;
                        _comboScrollLastUsed = DateTime.UtcNow;
                    }
                }
                else if (itemName == "Three Move Combo")
                {
                    if ((uint)_comboScrollCounter <= 2u)
                    {
                        _comboScrollCounter++;
                        _comboScrollLastUsed = DateTime.UtcNow;
                    }
                }
                ReadyToSpell(itemName);
                return true;
            }
            return false;
        }
   
        internal bool UseSpell(string spellName, Creature creature = null, bool staffSwitch = true, bool keepSpellAfterUse = true)
        {

            Spell sp = Spellbook[spellName];
            byte castLines = sp.CastLines;

            lock (Lock)
            {
  
                if (sp == null || !CanUseSpell(sp, creature) || ((spellName == "Hide" || spellName == "White Bat Form") && _server._stopCasting))
                {
                    return false;
                }

                if (staffSwitch)
                {
                    if (!CheckWeaponCastLines(sp, out castLines))
                    {
                        Console.WriteLine("Error in Client.cs UseSpell: staffSwitch was true but CheckWeaponCastLines returned false");
                        return false;
                    }
                }

                if (_currentSpell != sp)
                {
                    ClientTab.Invoke((Action)delegate
                    {
                        //ClientTab.currentAction.Text = action + "Casting " + spellName;//ADAM
                    });
                }

                ClientPacket clientPacket = new ClientPacket(15);
                clientPacket.WriteByte(sp.Slot);

                if (creature != null && (CreatureHashSet.Contains(creature.ID) || creature is Player))
                {
                    clientPacket.WriteInt32(creature.ID);
                    clientPacket.WriteStruct(creature.Location);
                }

                if (castLines > 0)
                {
                    if (_isWalking)
                    {
                        _isCasting = false;
                        return false;
                    }
                    string[] chantArray = LoadSavedChants(spellName);
                    lock (Lock)
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
                            else if (!string.IsNullOrWhiteSpace(ClientTab.customLinesBox.Text) && !Bot._shouldBotStop)
                            {
                                DisplayChant(ParseCustomSpellLines());
                            }
                            else if (!string.IsNullOrWhiteSpace(chantArray[i]))
                            {
                                DisplayChant(chantArray[i]);
                            }
                            DateTime utcNow = DateTime.UtcNow;
                            //Client client = (creature != null) ? Server.method_76(creature.Name) : this;
                            while (DateTime.UtcNow.Subtract(utcNow).TotalMilliseconds < 1000.0)
                            {
                                if (!IsValidSpell(this, sp.Name, creature))
                                {
                                    _isCasting = false;
                                    return false;
                                }
                                Thread.Sleep(10);
                            }
                            if (!_isCasting || !CanUseSpell(sp, creature))
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
                            if (sp.Name == "fas spiorad" && !Bot._needFasSpiorad && !Bot._manaLessThanEightyPct)
                            {
                                _isCasting = false;
                                return false;
                            }
                            DisplayChant(sp.Name);
                        }
                    }
                }
                if (sp.Name == "fas spiorad" && ClientTab.safeFSCbox.Checked)
                {
                    if (!int.TryParse(ClientTab.safeFSTbox.Text, out int result))
                    {
                        if ((double)Stats.CurrentHP < (double)Stats.MaximumMP * 0.5)
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
                this.CreatureTarget = (creature ?? Player);
                if (ReadyToSpell(sp.Name))
                {
                    _creatureToSpellList.Add(new CreatureToSpell(sp, CreatureTarget));
                }
                Console.WriteLine($"Casting: {sp.Name}");
                Enqueue(clientPacket);
                _spellCounter++;
                //Tasks.dateTime_5 = DateTime.UtcNow;ADAM
                sp.LastUsed = DateTime.UtcNow;
                _isCasting = false;
                _currentSpell = (keepSpellAfterUse ? sp : null);
                if (sp.Name != "Gem Polishing" || sp.Name.Contains("Prayer"))
                {
                    _currentSpell = sp;
                }
                return !keepSpellAfterUse || ClearSpell();
            }
        }

        internal void RequestProfile()
        {
            Enqueue(new ClientPacket(45));
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
                        ThreadPool.QueueUserWorkItem(_ => ClientTab.UpdateStrangerListAfterCharge());
                    }
                }
                Enqueue(clientPacket);
                return true;
            }
            return false;
        }

        internal void RemoveShield()
        {
            ClientPacket clientPacket = new ClientPacket(68);
            clientPacket.WriteByte(3);
            Enqueue(clientPacket);
        }


        internal void DisplayChant(string chant)
        {
            ClientPacket clientPacket = new ClientPacket(78);
            clientPacket.WriteString8(chant);
            Enqueue(clientPacket);
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


