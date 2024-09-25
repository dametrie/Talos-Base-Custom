using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using System.Threading.Tasks;
using Talos.Definitions;
using Talos.Enumerations;
using Talos.Forms;
using Talos.Forms.UI;
using Talos.Maps;
using Talos.Objects;
using Talos.Properties;
using Talos.Structs;

namespace Talos.Base
{
    internal class Bot : BotBase
    {
        private static object _lock { get; set; } = new object();
        private Dictionary<int, bool> routeFindPerformed = new Dictionary<int, bool>();
        private int _dropCounter;


        internal Client _client;
        internal Server _server;
        internal Creature creature;

        private bool _autoStaffSwitch;
        private bool _fasSpiorad;
        private bool _isSilenced;
        internal bool _needFasSpiorad = true;
        internal bool _manaLessThanEightyPct = true;
        internal bool _shouldBotStop = false;
        internal bool _shouldAlertItemCap;
        internal bool _recentlyAoSithed;
        internal bool[] itemDurabilityAlerts = new bool[5];

        internal bool _reddingOtherPlayer;
        internal bool bool_12;
        private bool bool_13;
        private bool bool_32;
        internal bool _hasRescue;

        internal byte _fowlCount;

        internal DateTime _lastKill = DateTime.MinValue;
        internal DateTime _lastDisenchanterCast = DateTime.MinValue;
        internal DateTime _lastGrimeScentCast = DateTime.MinValue;
        internal DateTime _skullTime = DateTime.MinValue;
        internal DateTime _lastRefresh = DateTime.MinValue;
        internal DateTime _lastVineCast = DateTime.MinValue;
        internal DateTime _botChecks = DateTime.MinValue;
        internal DateTime _lastBonusAppliedTime = DateTime.MinValue;
        internal DateTime _lastCast = DateTime.MinValue;
        internal DateTime _lastUsedGem = DateTime.MinValue;
        internal TimeSpan _bonusElapsedTime = TimeSpan.Zero;
        private DateTime _lastUsedFungusBeetle = DateTime.MinValue;
        private DateTime _lastUsedBeetleAid = DateTime.MinValue;

        internal List<Ally> _allyList = new List<Ally>();
        internal List<Enemy> _enemyList = new List<Enemy>();
        internal List<Player> _playersExistingOver250ms = new List<Player>();
        internal List<Player> _playersNeedingRed = new List<Player>();
        internal List<Player> _nearbyAllies = new List<Player>();
        internal List<Creature> _nearbyValidCreatures = new List<Creature>();

        internal HashSet<string> _allyListName = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        internal HashSet<ushort> _enemyListID = new HashSet<ushort>();



        internal System.Windows.Forms.Label currentAction;
        private Location _lastBubbleLocation;
        private string _bubbleType;

        private SoundPlayer soundPlayer = new SoundPlayer();
        private bool _swappingNecklace;
        private DateTime _lastUsedMonsterCall = DateTime.MinValue;
        internal bool _hasWhiteDugon;
        private List<Player> _nearbyPlayers;
        internal bool _circle1;
        internal bool _circle2;

        public bool RecentlyUsedGlowingStone { get; set; } = false;
        public bool RecentlyUsedDragonScale { get; set; } = false;
        public bool RecentlyUsedFungusExtract { get; set; } = false;
        internal AllyPage AllyPage { get; set; }
        internal EnemyPage EnemyPage { get; set; }


        internal Bot(Client client, Server server) : base(client, server)
        {
            _client = client;
            _server = server;
            AddTask(new TaskDelegate(BotLoop));
            AddTask(new TaskDelegate(Sounds));
            AddTask(new TaskDelegate(Walker));
        }

        private void Walker()
        {
            _shouldBotStop = IsRangerNearBy();
            if(!Client._exchangeOpen && Client.ClientTab != null)
            {
                HandleDialog();
                HandleDumbMTGWarp();
                WalkActions();
            }
        }

        private void WalkActions()
        {
            while (!_shouldThreadStop)
            {
                _nearbyPlayers = Client.GetNearbyPlayerList();
                _nearbyValidCreatures = Client.GetNearbyValidCreatures(12);
                var shouldWalk = !this._reddingOtherPlayer &&
                    (!Client.ClientTab.rangerStopCbox.Checked || !_shouldBotStop);

                if (shouldWalk)
                {
                    HandleWalkingCommand();
                }
            }

        }

        private void HandleWalkingCommand()
        {
            string comboBoxText = Client.ClientTab.walkMapCombox.Text;
            bool followChecked = Client.ClientTab.followCbox.Checked;
            string followName = Client.ClientTab.followText.Text;

            if (followChecked && !string.IsNullOrEmpty(followName))
            {
                FollowWalking(followName);
            }
            else if (Client.ClientTab.walkBtn.Text == "Stop")
            {

                if (comboBoxText == "SW Lure")
                {
                    SWLure();
                }
                else if (comboBoxText == "WayPoints")
                {
                    WayPointWalking();
                }

                else
                {
                    if (short.TryParse(comboBoxText, out short mapID))
                    {
                        Client.RouteFindByMapID(mapID);
                    }
                    else
                    {
                        Location destination = GetDestinationBasedOnComboBoxText(comboBoxText);
                        if (destination != default)
                        {
                            HandleExtraMapActions(destination);
                            UpdateWalkButton(destination);
                        }
                    }
                }
            }
        }

        private void FollowWalking(string playerName)
        {
            throw new NotImplementedException();
        }

        private void WayPointWalking()
        {
            throw new NotImplementedException();
        }

        private void SWLure()
        {
            throw new NotImplementedException();
        }

        private void HandleExtraMapActions(Location destination)
        {
            Location location = Client._serverLocation;
           
            if (Client._map.MapID == 6525) // Oren Island Ruins0
            {
                if (location.AbsoluteXY(32, 23) > 2)
                {
                    Client.RouteFind(new Location(6525, 32, 23), 0, false, true, true);
                    return;
                }
                else
                {
                    Client.PublicChat(3, "Welcome Aisling");
                    Thread.Sleep(500);
                    return;
                }

            }
            if (Client._map.MapID == 3938) // Loures Storage 12
            {
                if (location.AbsoluteXY(7, 13) > 2)
                {
                    Client.TryWalkToLocation(new Location(3938, 7, 13), 0, true, false);
                    return;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                }
            }
            if (Client._map.MapID == 3920) // Gladiator Arena Entrance
            {
                if (location.AbsoluteXY(13, 12) > 2)
                {
                    Client.TryWalkToLocation(new Location(3920, 13, 12), 0, true, false);
                    return;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                }
            }
            if (Client._map.MapID == 3012)
            {
                if (location.AbsoluteXY(15, 0) > 2)
                {
                    Client.TryWalkToLocation(new Location(3012, 15, 0), 0, true, false);
                    return;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                }
            }
            if (Client._map.MapID == 10265)
            {
                if (location.AbsoluteXY(93, 48) > 2)
                {
                    Client.TryWalkToLocation(new Location(10265, 93, 48), 0, true, false);
                    return;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                }
            }
            if (Client._map.MapID == 424)
            {
                if (location.AbsoluteXY(6, 6) > 2)
                {
                    Client.TryWalkToLocation(new Location(424, 6, 6), 0, true, false);
                    return;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                }
            }


            if (destination.MapID == 6537)
            {
                if (Client._map.MapID == 6530 && !Client.RouteFind(new Location(6537, 65, 1), 0, false, true, true))
                {
                    Client.TryWalkToLocation(new Location(6530, 10, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537 && Client._clientLocation.X < 31 && Client._clientLocation.Y < 43)
                {
                    Client.TryWalkToLocation(new Location(6537, 0, 31), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6535 && Client._clientLocation.X > 68 && Client._clientLocation.Y < 56)
                {
                    Client.TryWalkToLocation(new Location(6535, 74, 49), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537 && Client._clientLocation.X < 43 && Client._clientLocation.Y > 43)
                {
                    Client.TryWalkToLocation(new Location(6537, 14, 74), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6536 && Client._clientLocation.X < 25 && Client._clientLocation.Y < 24)
                {
                    Client.TryWalkToLocation(new Location(6536, 0, 15), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6534 && Client._clientLocation.X > 45)
                {
                    Client.TryWalkToLocation(new Location(6534, 74, 28), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6536 && (Client._clientLocation.X > 25 || Client._clientLocation.Y > 26) && Client._clientLocation.X < 69)
                {
                    Client.TryWalkToLocation(new Location(6536, 72, 4), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6536 && Client._clientLocation.X > 68)
                {
                    Client.TryWalkToLocation(new Location(6536, 69, 0), 0, true, false);
                    return;
                }
                if (!Client.RouteFind(new Location(6537, 65, 1), 3, false, true, true) && !Client.RouteFind(new Location(6537, 65, 1), 3, false, true, true))
                {
                    if (Client._map.MapID == 6537)
                    {
                        Location serverLocation = Client._serverLocation;
                        if (serverLocation.DistanceFrom(new Location(6537, 65, 1)) <= 3)
                        {
                            Client.ClientTab.walkBtn.Text = "Walk";
                        }
                    }
                    //Client.SetMapID00(6525);
                    Client.RouteFind(new Location(6525, 0, 0), 0, true); //problem because continuously recalls routefind
                }
            }
            if (destination.MapID == 6541)
            {
                if (Client._map.MapID == 6530 && !Client.RouteFind(new Location(6541, 73, 4), 0, false, true, true))
                {
                    Client.TryWalkToLocation(new Location(6530, 10, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537)
                {
                    Client.TryWalkToLocation(new Location(6537, 0, 4), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6539)
                {
                    Client.TryWalkToLocation(new Location(6539, 53, 74), 0, true, true);
                    return;
                }
                if (Client._map.MapID == 6538)
                {
                    Client.TryWalkToLocation(new Location(6538, 74, 16), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6540 && Client._clientLocation.X < 4)
                {
                    Client.TryWalkToLocation(new Location(6540, 3, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6541 && Client._clientLocation.X < 28 && Client._clientLocation.Y > 63)
                {
                    Client.TryWalkToLocation(new Location(6541, 23, 74), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6540)
                {
                    Client.TryWalkToLocation(new Location(6540, 35, 0), 0, true, false);
                    return;
                }
                if (!Client.RouteFind(new Location(6541, 73, 4), 3, false, true, true) && !Client.RouteFind(new Location(6541, 73, 4), 3, false, true, true))
                {
                    if (Client._map.MapID == 6541)
                    {
                        Location serverLocation = Client._serverLocation;
                        if (serverLocation.DistanceFrom(new Location(6541, 73, 4)) <= 3)
                        {
                            Client.ClientTab.walkBtn.Text = "Walk";
                        }
                    }
                    Client.RouteFind(new Location(6525, 0, 0), 0, true);
                }
            }
            if (destination.MapID == 6538)
            {
                if (Client._map.MapID == 6530 && !Client.RouteFind(new Location(6538, 58, 73), 0, false, true, true))
                {
                    Client.TryWalkToLocation(new Location(6530, 10, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537)
                {
                    Client.TryWalkToLocation(new Location(6537, 0, 4), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6539 && Client._clientLocation.Y < 8)
                {
                    Client.TryWalkToLocation(new Location(6539, 1, 8), 1, true, false);
                    return;
                }
                if (Client._map.MapID == 6539)
                {
                    Client.TryWalkToLocation(new Location(6539, 4, 74), 0, true, true);
                    return;
                }
                if (Client._map.MapID == 6538 && Client._clientLocation.Y < 50)
                {
                    Client.TryWalkToLocation(new Location(6538, 74, 47), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6540)
                {
                    Client.TryWalkToLocation(new Location(6540, 0, 67), 0, true, false);
                    return;
                }
                if (!Client.RouteFind(new Location(6538, 58, 73), 3, false, true, true) && !Client.RouteFind(new Location(6538, 58, 73), 3, false, true, true))
                {
                    if (Client._map.MapID == 6538)
                    {
                        Location serverLocation = Client._serverLocation;
                        if (serverLocation.DistanceFrom(new Location(6538, 58, 73)) <= 3)
                        {
                            Client.ClientTab.walkBtn.Text = "Walk";
                        }
                    }
                    Client.RouteFind(new Location(6525, 0, 0), 0, true);
                }
            }
            if (destination.MapID == 6534)
            {
                if (Client._map.MapID == 6530 && !Client.RouteFind(new Location(6534, 1, 36), 0, false, true, true))
                {
                    Client.TryWalkToLocation(new Location(6530, 10, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537)
                {
                    Client.TryWalkToLocation(new Location(6537, 0, 4), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6535)
                {
                    Client.TryWalkToLocation(new Location(6535, 20, 74), 0, true, false);
                    return;
                }
                if (!Client.RouteFind(new Location(6534, 1, 36), 3, false, true, true) && !Client.RouteFind(new Location(6534, 1, 36), 3, false, true, true))
                {
                    if (Client._map.MapID == 6534)
                    {
                        Location serverLocation = Client._serverLocation;
                        if (serverLocation.DistanceFrom(new Location(6534, 1, 36)) <= 3)
                        {
                            Client.ClientTab.walkBtn.Text = "Walk";
                        }
                    }
                    Client.RouteFind(new Location(6525, 0, 0), 0, true);
                }
            }
           
        }

        private Location GetDestinationBasedOnComboBoxText(string text)
        {
            uint fnvHash = Utility.CalculateFNV(text);
            switch (fnvHash)
            {
                case 104857644U: // Tavaly
                    return new Location(11500, 76, 90);
                case 42952968U: // Lost Ruins
                    return new Location(8995, 41, 36);
                case 197647056U: // Aman Skills/Spells
                    return new Location(8295, 12, 7);
                case 140575268U: // Noam
                    return new Location(10055, 0, 0);
                case 122196308U: // Andor 140
                    return new Location(10240, 15, 15);
                case 362565633U: // Nobis Storage
                    return new Location(6718, 8, 8);
                case 270108063U: // Crystal Caves
                    return new Location(8314, 9, 95);
                case 705191458U: // Succi Hair
                    return new Location(72, 23, 19);
                case 656194412U: // Blackstar
                    return new Location(3210, 69, 34);
                case 390284915U: // Nobis 2-11
                    return new Location(6537, 65, 1); //Actually 2-4
                case 3045472732U: // Nobis 2-5
                    return new Location(6534, 1, 36); // Actually 2-1
                case 1434418274U: // Nobis 3-11
                    return new Location(6541, 73, 4); //Actually 3-4
                case 3002224923U: // Nobis 3-5
                    return new Location(6538, 58, 73); //Actually 3-1
                case 1054692617U: // Canals
                    return new Location(3938, 7, 13); //Loures Storage 12
                case 1917735196U: // Glad Arena
                    return new Location(3950, 13, 12);
                case 1570042694U: // YT 24
                    return new Location(8368, 48, 24);
                case 2122966770U: // Water Dungeon
                    return new Location(6998, 11, 9);
                case 1936185911U: // Andor Lobby
                    return new Location(10101, 15, 10);
                case 2510239379U: // Shinewood
                    return new Location(559, 43, 26);
                case 2487385400U: //YT Vine Rooms
                    return new Location(8358, 58, 1);
                case 2199457723U: // Loures
                    return new Location(3012, 15, 0);
                case 2543647522U: // Shinewood 36
                    return new Location(566, 28, 24);
                case 2529604651U: // Fire Canyon
                    return new Location(10265, 93, 48); //Hwarone City
                case 2728795543U: // Plamit Boss
                    return new Location(9376, 42, 47);
                case 2628668450U: // Shinewood 43
                    return new Location(573, 22, 26);
                case 2577202760U: // Shinewood 38
                    return new Location(568, 28, 38);
                case 2911405393U: // Aman Jungle
                    return new Location(8300, 121, 33);
                case 3381421134U: // Plamit Lobby
                    return new Location(9378, 40, 20);
                case 3033542801U: // Andor 80
                    return new Location(10180, 20, 20);
                case 3560321112U: // MTG 16
                    return new Location(2092, 79, 6);
                case 3660986826U: // MTG 10
                    return new Location(2096, 4, 7);
                case 3644209207U: // MTG 13
                    return new Location(2095, 55, 94);
                case 3610506874U: // MTG 25
                    return new Location(2092, 57, 93);
                case 3390820287U: // Mines
                    return new Location(2901, 15, 15);
                case 3826339036U: // Yowien Territory
                    return new Location(8318, 50, 93);
                case 3791705852U: // Black Market
                    return new Location(424, 6, 6);
                case 3770643204U: // Chadul Mileth 1
                    return new Location(8432, 5, 8);
                case 4189239892U: // Chaos 1
                    return new Location(3634, 18, 10);
                case 3848419112U: // CR 31
                    return new Location(5031, 6, 34);
                default:
                    return default;
            }
        }

        private void UpdateWalkButton(Location targetLocation)
        {
            if (!Client.ClientTab.walkMapCombox.Text.Contains("Nobis") && ShouldProceedWithNavigation(targetLocation))
            {
                UpdateButtonStateBasedOnProximity(targetLocation);
            }
        }

        private bool ShouldProceedWithNavigation(Location targetLocation)
        {
            bool isRouteAvailable = Client.RouteFind(targetLocation, 3, false, true, true);
            return !isRouteAvailable && Client._map.MapID == targetLocation.MapID;
        }

        private void UpdateButtonStateBasedOnProximity(Location targetLocation)
        {
            Location currentLocation = Client._serverLocation;
            int proximityThreshold = 4;
            if (currentLocation.DistanceFrom(targetLocation) <= proximityThreshold)
            {
                Client.ClientTab.walkBtn.Text = "Walk";
            }
        }

        private void HandleDumbMTGWarp()
        {
            //The warp into Mt. Giragan 1 is stupid and sometimes drops you in the warp to the world server
            if (Client._map.MapID.Equals(2120)) //Giragan
            {
                Location location = Client._serverLocation;
                if (location.X.Equals(39) && (location.Y.Equals(8) || location.Y.Equals(7)))
                {
                    Client.Walk(Direction.West);
                    Thread.Sleep(800);
                }
            }
        }

        private void HandleDialog()
        {
            if (Client._npcDialog != null && Client._npcDialog.Equals("You see strange dark fog upstream. Curiosity overcomes you and you take a small raft up the river through the black mist."))
            {
                Client.EnterKey();
                Client._npcDialog = "";
            }
        }
        private void Sounds()
        {
            if (Client.ClientTab == null || Client == null)
            {
                return;
            }
            while (!_shouldThreadStop)  // Assuming _shouldStop is a volatile bool that is set to true when you want to stop all threads
            {
                if (_server._disableSound)
                {
                    Thread.Sleep(250);
                    return;
                }
                if (Client.ClientTab.alertSkulledCbox.Checked && Client.IsSkulled)
                {
                    soundPlayer.Stream = Resources.skull;
                    soundPlayer.PlaySync();
                }
                else if (Client.ClientTab.alertRangerCbox.Checked && IsRangerNearBy())
                {
                    soundPlayer.Stream = Resources.ranger;
                    soundPlayer.PlaySync();
                }
                else if (Client.ClientTab.alertStrangerCbox.Checked && IsStrangerNearby())
                {
                    soundPlayer.Stream = Resources.detection;
                    soundPlayer.PlaySync();
                }
                else if (Client.ClientTab.alertItemCapCbox.Checked && _shouldAlertItemCap)
                {
                    soundPlayer.Stream = Resources.itemCap;
                    soundPlayer.PlaySync();
                    _shouldAlertItemCap = false;
                }
                else
                {
                    if (Client.ClientTab.alertDuraCbox.Checked && itemDurabilityAlerts.Contains(true))
                    {
                        for (int i = 0; i < itemDurabilityAlerts.Length; i++)
                        {
                            if (itemDurabilityAlerts[i])
                            {
                                soundPlayer.Stream = Resources.durability;
                                soundPlayer.PlaySync();
                                itemDurabilityAlerts[i] = false; // Set the current alert as handled
                                break; // Exit the loop after handling the first alert
                            }
                        }
                        Thread.Sleep(2000);
                    }
                    if (Client.ClientTab.alertEXPCbox.Checked && Client.Experience >= 4290000000U)
                    {
                        soundPlayer.Stream = Resources.expmaxed;
                        soundPlayer.PlaySync();
                    }
                }

                Thread.Sleep(2000);  // Delay before the next iteration to prevent high CPU usage
            }

            
        }
        private void BotLoop()
        {
            while (!_shouldThreadStop)
            {
                try
                {
                    //Console.WriteLine("BotLoop running");

                    if (Client.InArena)
                    {
                        //Console.WriteLine("Sleeping for 1 second");
                        Thread.Sleep(1000);
                        return;
                    }
                    if (currentAction == null)
                    {
                        currentAction = Client.ClientTab.currentAction;
                    }

                    _shouldBotStop = IsRangerNearBy();

                    //Console.WriteLine("Checking for stop conditions");
                    if (CheckForStopConditions())
                    {
                        //Console.WriteLine("Sleeping for 100ms");
                        Thread.Sleep(100);
                        return;
                    }

                    //Console.WriteLine("Processing players");
                    ProcessPlayers();

                    if (Client.CurrentHP <= 1U && Client.IsSkulled)
                    {
                        HandleSkullStatus();
                        return;
                    }

                    if (ShouldRequestRefresh())
                    {
                        Console.WriteLine("Requesting refresh");
                        Client.RequestRefresh(false);
                        _lastRefresh = DateTime.UtcNow;
                        continue;
                    }

                    //Console.WriteLine("Checking and handling spells");
                    //CheckAndHandleSpells();

                    //Console.WriteLine("Checking for autoRed conditions");
                    if (AutoRedConditionsMet())
                    {
                        if (GetSkulledPlayers().Count > 0)
                        {
                            RedSkulledPlayers();
                        }   
                    }

                    //Console.WriteLine("Checking for strangers");
                    if (IsStrangerNearby())
                    {
                        FilterStrangerPlayers();
                    }

                    if (DateTime.UtcNow.Subtract(_botChecks).TotalSeconds < 2.5)
                    {
                        Console.WriteLine("Botcheck");
                        continue;//adam return?
                    }

                    PerformSpellActions();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in BotLoop: {ex.Message}");
                }
            }

        }
        internal bool IsRangerNearBy()
        {
            if (!Settings.Default.paranoiaMode)
            {
                return Client.GetNearbyPlayerList().Any(new Func<Player, bool>(RangerListContains));
            }
            return IsStrangerNearby();
        }
        private bool RangerListContains(Player player)
        {
            return CONSTANTS.KNOWN_RANGERS.Contains(player.Name, StringComparer.OrdinalIgnoreCase);
        }
        private bool CheckForStopConditions()
        {
            return (Client._inventoryFull && Client.ClientTab.toggleFarmBtn.Text == "Farming") ||
                   (Client.Bot.bool_32 && Client.ClientTab.toggleFarmBtn.Text == "Farming") ||
                   Client._exchangeOpen || Client.ClientTab == null || Client.Dialog != null || bool_12;
        }
        private void ProcessPlayers()
        {
            _nearbyAllies = Client.GetNearbyAllies();
            _nearbyValidCreatures = Client.GetNearbyValidCreatures(11);
            var nearbyPlayers = Client.GetNearbyPlayerList();
            _playersExistingOver250ms = nearbyPlayers?.Where(Delegates.HasPlayerExistedForOver250ms).ToList() ?? new List<Player>();
            _playersNeedingRed.Clear();
        }
        private void HandleSkullStatus()
        {
            currentAction.Text = Client._action + "Nothing, you're dead";
            _skullTime = (_skullTime == DateTime.MinValue) ? DateTime.UtcNow : _skullTime;
            LogIfSkulled();
            LogIfSkulledAndSurrounded();
            if (!Client._safeScreen)
            {
                Client.ServerMessage((byte)ServerMessageType.TopRight, "Skulled for: " + DateTime.UtcNow.Subtract(_skullTime).Seconds.ToString() + " seconds.");
            }
        }
        private void LogIfSkulled()
        {
            bool isOptionsSkullCboxChecked = Client.ClientTab.optionsSkullCbox.Checked;

            if (isOptionsSkullCboxChecked && DateTime.UtcNow.Subtract(_skullTime).TotalSeconds > 6.0)
            {
                string text = AppDomain.CurrentDomain.BaseDirectory + "\\skull.txt";
                File.WriteAllText(text, string.Concat(new string[]
                {
                    DateTime.UtcNow.ToLocalTime().ToString(),
                    "\n",
                    Client._map.Name,
                    ": (",
                    Client._serverLocation.ToString(),
                    ")"
                }));
                Process.Start(text);
                Client.DisconnectWait(false);
            }
        }
        private void LogIfSkulledAndSurrounded()
        {
            bool isOptionsSkullSurrboxChecked = Client.ClientTab.optionsSkullSurrbox.Checked;
            if (isOptionsSkullSurrboxChecked && DateTime.UtcNow.Subtract(_skullTime).TotalSeconds > 4.0 && Client.IsLocationSurrounded(Client._serverLocation))
            {
                string text2 = AppDomain.CurrentDomain.BaseDirectory + "\\skull.txt";
                File.WriteAllText(text2, string.Concat(new string[]
                {
                    DateTime.UtcNow.ToLocalTime().ToString(),
                    "\n",
                    Client._map.Name,
                    ": (",
                    Client._serverLocation.ToString(),
                    ")"
                }));
                Process.Start(text2);
                Client.DisconnectWait(false);
            }
        }
        private bool ShouldRequestRefresh()
        {
            return DateTime.UtcNow.Subtract(Client.LastStep).TotalSeconds > 20.0 &&
                   DateTime.UtcNow.Subtract(_lastKill).TotalSeconds > 20.0 &&
                   DateTime.UtcNow.Subtract(_lastRefresh).TotalSeconds > 30.0;
        }
        private void CheckAndHandleSpells()
        {
            if (Client.HasSpell("Lyliac Vineyard") && !Client.Spellbook["Lyliac Vineyard"].CanUse)
            {
                _lastVineCast = DateTime.UtcNow;
            }
        }
        private bool AutoRedConditionsMet()
        {
           return Client.ClientTab != null && _playersExistingOver250ms != null && Client.ClientTab.autoRedCbox.Checked;
        }
        private List<Player> GetSkulledPlayers()
        {
            if (_playersExistingOver250ms.Any(Delegates.PlayerIsSkulled))
            {
                _playersExistingOver250ms.RemoveAll(Delegates.PlayerIsSkulled);
                foreach (Player player in Client.GetNearbyPlayerList().Where(IsSkulledFriendOrGroupMember))
                {
                    _playersNeedingRed.Add(player);
                }
                return _playersNeedingRed = _playersNeedingRed.OrderBy(DistanceFromPlayer).ToList();
            }
            return new List<Player>();
        }

        private bool RedSkulledPlayers()
        {
            if (_playersNeedingRed.Count > 0 && Client.ClientTab.autoRedCbox.Checked)
            {
                Player player = _playersNeedingRed[0];

                var inventory = Client.Inventory;
                bool canUseBeetleAid = inventory.HasItem("Beetle Aid") && Client._isRegistered &&
                                       DateTime.UtcNow.Subtract(_lastUsedBeetleAid).TotalMinutes > 2.0;
                bool canUseOtherItems = inventory.HasItem("Komadium") || inventory.HasItem("beothaich deum");

                if (canUseBeetleAid || canUseOtherItems)
                {
                    _reddingOtherPlayer = true;
                    Direction direction = player.Location.Point.GetDirection(Client._serverLocation.Point);

                    if (Client._serverLocation.DistanceFrom(player.Location) > 1)
                    {
                        if (Client._clientLocation.DistanceFrom(player.Location) == 1)
                        {
                            Client.RequestRefresh(true);
                        }
                        Client.TryWalkToLocation(player.Location, 1, true, true);
                    }
                    else if (direction != Client._clientDirection)
                    {
                        Client.Turn(direction);
                    }
                    else
                    {
                        if (canUseBeetleAid && Client.UseItem("Beetle Aid"))
                        {
                            _lastUsedBeetleAid = DateTime.UtcNow;
                            player.SpellAnimationHistory[(ushort)SpellAnimation.Skull] = DateTime.UtcNow.AddSeconds(-2);
                        }
                        else if (canUseOtherItems && (Client.UseItem("Komadium") || Client.UseItem("beothaich deum")))
                        {
                            player.SpellAnimationHistory[(ushort)SpellAnimation.Skull] = DateTime.UtcNow.AddSeconds(-2);
                            Thread.Sleep(1000); // Consider async/await pattern if possible
                        }

                        Client.UseSkill("Transferblood");

                        return false;
                    }
                }
                else if (player == null || !Client.GetNearbyPlayerList().Contains(player) || player.HealthPercent > 30 || DateTime.UtcNow.Subtract(player.SpellAnimationHistory[(ushort)SpellAnimation.Skull]).TotalSeconds > 5.0)
                {
                    player = null;
                    _reddingOtherPlayer = false;

                    return false;
                }
            }

            return true;
        }

        private bool IsSkulledFriendOrGroupMember(Player player)
        {
            return Client._groupBindingList.Concat(Client._friendBindingList).Contains(player.Name, StringComparer.CurrentCultureIgnoreCase) && player.IsSkulled;
        }

        private int DistanceFromPlayer(Player player)
        {
            return Client._serverLocation.DistanceFrom(player.Location);
        }

        private void FilterStrangerPlayers()
        {
            var duplicateOrHiddenPlayers = new List<Player>();
            foreach (var player in _playersExistingOver250ms)
            {
                foreach (var otherPlayer in _playersExistingOver250ms)
                {
                    if (player != otherPlayer && (player.Location.Equals(otherPlayer.Location) || otherPlayer._isHidden) && !duplicateOrHiddenPlayers.Contains(player))
                    {
                        duplicateOrHiddenPlayers.Add(player);
                    }
                }
                foreach (var creature in _nearbyValidCreatures)
                {
                    if (creature != null && player.Location.Equals(creature.Location) && !duplicateOrHiddenPlayers.Contains(player))
                    {
                        duplicateOrHiddenPlayers.Add(player);
                    }
                }
            }
            _playersExistingOver250ms = _playersExistingOver250ms.Except(duplicateOrHiddenPlayers).ToList();
        }

        private void PerformSpellActions()
        {
            Loot();
            //ManageSpellCasting(); //Adam rework this, it breaks casting

            _autoStaffSwitch = Client.ClientTab.autoStaffCbox.Checked;
            _fasSpiorad = Client.HasEffect(EffectsBar.FasSpiorad) || (Client.HasSpell("fas spiorad") && DateTime.UtcNow.Subtract(Client.Spellbook["fas spiorad"].LastUsed).TotalSeconds < 1.5);
            _isSilenced = Client.HasEffect(EffectsBar.Silenced);

            AoSuain();
            WakeScroll();
            AutoGem();

            UpdatePlayersListBasedOnStrangers();

            CheckFasSpioradRequirement();

            try
            {
                if (CastDefensiveSpells())
                {
                    CastOffensiveSpells();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in PerformSpellActions: {ex.Message}");
            }

            //ManageSpellCastingDelay();


            Client currentClient = Client;
            byte? castLinesCountNullable;

            if (currentClient == null)
            {
                castLinesCountNullable = null;
            }
            else
            {
                Spell currentSpell = currentClient._currentSpell;
                castLinesCountNullable = (currentSpell != null) ? new byte?(currentSpell.CastLines) : null;
            }

            byte? castLinesCount = castLinesCountNullable;
            int? castLines = (castLinesCount != null) ? new int?((int)castLinesCount.GetValueOrDefault()) : null;

            if (castLines.GetValueOrDefault() <= 0 & castLines != null)
            {
                //Console.WriteLine("Sleeping for 10ms");
                Thread.Sleep(330);
            }
        }
        private bool CastDefensiveSpells()
        {
            FasSpiorad();
            Hide();
            BubbleBlock();
            Heal();
            DispellAllySuain();
            DispellAllyCurse();
            AoPoison();
            Dion();
            Aite();
            Fas();
            AiteAllies();
            FasAllies();

            DragonScale();
            Armachd();
            ArmachdAllies();
            BeagCradh();
            BeagCradhAllies();
            CastPlayerBuffs(); //Deireas Faileas, Monk Forms, Asgall, Perfect Defense,
                           //Aegis Spehre, ao beag suain, Muscle Stim, Nerve Stim, Mist, Mana Ward
                           //Vanish Elixir, Regens, Mantid Scent
            Comlhas();
            return true;
        }

        private bool Comlhas()
        {
            if (AllyPage == null)
            {
                return true;
            }

            if (AllyPage.allyMDCRbtn.Checked)
            {
                var playerToDion = _nearbyAllies.FirstOrDefault(Delegates.NotDioned);

                if (playerToDion != null)
                {
                    Console.WriteLine($"[Comlhas] PlayerToDion: {playerToDion?.Name}, Hash: {playerToDion.GetHashCode()}, has dion: {playerToDion.IsDioned}");
                    Client.UseSpell("mor dion comlha", null, _autoStaffSwitch, false);
                    return false;
                }
            }

            if (AllyPage.allyMDCSpamRbtn.Checked)
            {
                if (Client.UseSpell("mor dion comlha", null, _autoStaffSwitch, false))
                {
                    return false;
                }
            }
            else if (AllyPage.allyMICSpamRbtn.Checked)
            {
                if (!Client.UseSpell("ard ioc comlha", null, _autoStaffSwitch, false) &&
                    !Client.UseSpell("mor ioc comlha", null, _autoStaffSwitch, false))
                {
                    Client.UseSpell("ioc comlha", null, _autoStaffSwitch, false);
                }
                return false;
            }

            return true;
        }

        private bool CastPlayerBuffs()
        {
            if (Client.ClientTab.deireasFaileasCbox.Checked && !Client.HasEffect(EffectsBar.DeireasFaileas))
            {
                Client.UseSpell("deireas faileas", null, _autoStaffSwitch, true);
                return false;
            }

            if (Client.ClientTab.dragonsFireCbox.Checked && Client._isRegistered && !Client.HasEffect(EffectsBar.DragonsFire))
            {
                Client.UseItem("Dragon's Fire");
            }

            if (Client.ClientTab.druidFormCbox.Checked && !Client.HasEffect(EffectsBar.FeralForm) && !Client.HasEffect(EffectsBar.BirdForm) && !Client.HasEffect(EffectsBar.LizardForm) && !_swappingNecklace)
            {
                if (!Client.UseSpell("Feral Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Wild Feral form", null, _autoStaffSwitch, true) && !Client.UseSpell("Fierce Feral Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Master Feral Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Karura Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Wild Karura Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Fierce Karura Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Master Karura Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Komodas Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Wild Komodas Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Fierce Komodas Form", null, _autoStaffSwitch, true))
                {
                    Client.UseSpell("Master Komodas Form", null, _autoStaffSwitch, true);
                }
                Thread.Sleep(1000);
            }

            if (Client.ClientTab.asgallCbox.Checked && !Client.HasEffect(EffectsBar.AsgallFaileas))
            {
                Client.UseSpell("asgall faileas", null, true, true);
            }

            if (Client.ClientTab.perfectDefenseCbox.Checked && !Client.HasEffect(EffectsBar.PerfectDefense))
            {
                Client.UseSkill("Perfect Defense");
            }

            if (Client.ClientTab.aegisSphereCbox.Checked && !Client.HasEffect(EffectsBar.Armachd))
            {
                Client.UseSpell("Aegis Sphere", null, false, true);
            }

            if (Client.ClientTab.aoSuainCbox.Checked && Client.HasEffect(EffectsBar.BeagSuain))
            {
                Client.UseSkill("ao beag suain");
            }

            if (Client.ClientTab.muscleStimulantCbox.Checked && Client._isRegistered && !Client.HasEffect(EffectsBar.FasDeireas))
            {
                Client.UseItem("Muscle Stimulant");
            }

            if (Client.ClientTab.nerveStimulantCbox.Checked && Client._isRegistered && !Client.HasEffect(EffectsBar.Beannaich))
            {
                Client.UseItem("Nerve Stimulant");
            }

            if (Client.ClientTab.disenchanterCbox.Checked && DateTime.UtcNow.Subtract(_lastDisenchanterCast).TotalMinutes > 6.0)
            {
                Client.UseSpell("Disenchanter", null, _autoStaffSwitch, true);
                Thread.Sleep(1000);
                return false;
            }

            if (Client.ClientTab.monsterCallCbox.Checked && Client._isRegistered && DateTime.UtcNow.Subtract(_lastUsedMonsterCall).TotalSeconds > 2.0 && Client.UseItem("Monster Call"))
            {
                _lastUsedMonsterCall = DateTime.UtcNow;
            }

            if (Client.ClientTab.mistCbox.Checked && !Client.HasEffect(EffectsBar.Mist))
            {
                Client.UseSpell("Mist", null, _autoStaffSwitch, true);
            }

            if (Client.ClientTab.manaWardCbox.Checked)
            {
                Client.UseSpell("Mana Ward", null, false, false);
            }

            if (Client.ClientTab.vanishingElixirCbox.Checked && Client._isRegistered)
            {
                foreach (Player ally in _nearbyAllies)
                {
                    if (!ally._isHidden)
                    {
                        Client.UseItem("Vanishing Elixir");
                    }
                }
            }

            if (Client.ClientTab.autoDoubleCbox.Checked && !Client.HasEffect(EffectsBar.BonusExperience) && Client._isRegistered && Client.CurrentMP > 100)
            {
                // Mapping of combobox text to the item names
                var itemMappings = new Dictionary<string, string>
                {
                    { "Kruna 50%", "50 Percent EXP/AP Bonus" },
                    { "Kruna 100%", "Double EXP/AP Bonus" },
                    { "Xmas 50%", "XMas Bonus Exp-Ap" },
                    { "Star 100%", "Double Bonus Exp-Ap" },
                    { "Vday 100%", "VDay Bonus Exp-Ap" }
                };

                // Check for special case where additional logic is needed
                if (Client.ClientTab.doublesCombox.Text == "Xmas 100%")
                {
                    var itemText = _client.HasItem("Christmas Double Exp-Ap") ? "Christmas Double Exp-Ap" : "XMas Double Exp-Ap";
                    Client.ClientTab.UseItem(itemText);
                }
                else if (itemMappings.TryGetValue(Client.ClientTab.doublesCombox.Text, out var itemText))
                {
                    Client.ClientTab.UseItem(itemText);
                }

                Client.ClientTab.UpdateBonusTimer();
            }

            if (Client.ClientTab.regenerationCbox.Checked && (!Client.HasEffect(EffectsBar.Regeneration) || !Client.HasEffect(EffectsBar.IncreasedRegeneration)))
            {
                if (Client.HasSpell("Increased Regeneration") && !Client.HasEffect(EffectsBar.IncreasedRegeneration))
                {
                    Client.UseSpell("Increased Regeneration", Client.Player, _autoStaffSwitch, true);
                    return false;
                }
                if (Client.Spellbook.Any(new Func<Spell, bool>(Delegates.CastRegeneration)) && !Client.HasEffect(EffectsBar.Regeneration))
                {
                    TryCastAnyRank("Regeneration", Client.Player, _autoStaffSwitch, true);
                    return false;
                }
            }
            foreach (Ally currentAlly in ReturnAllyList())
            {
                if (_playersExistingOver250ms.Count > 0)
                {
                    foreach (Player player in _playersExistingOver250ms)
                    {
                        if (player.Name.Equals(currentAlly.Name, StringComparison.OrdinalIgnoreCase) && player != Client.Player && currentAlly.AllyPage.dbRegenCbox.Checked)
                        {
                            Client client = Client._server.FindClientByName(currentAlly.Name);
                            if (client != null)
                            {
                                if (!client.HasEffect(EffectsBar.IncreasedRegeneration) && Client.UseSpell("Increased Regeneration", client.Player, _autoStaffSwitch, true))
                                {
                                    return false;
                                }
                                if (Client.Spellbook.Any(s => Delegates.CastRegeneration(s)) && !client.HasEffect(EffectsBar.Regeneration) && TryCastAnyRank("Regeneration", client.Player, _autoStaffSwitch, true))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                // Fallback to using spells on the player directly if the client for the ally wasn't found
                                if ((!player.SpellAnimationHistory.ContainsKey((ushort)SpellAnimation.IncreasedRegeneration) || DateTime.UtcNow.Subtract(player.SpellAnimationHistory[(ushort)SpellAnimation.IncreasedRegeneration]).TotalSeconds > 1.5) && Client.UseSpell("Increased Regeneration", player, _autoStaffSwitch, true))
                                {
                                    return false;
                                }
                                if (Client.Spellbook.Any(s => Delegates.CastRegeneration(s)) && (!player.SpellAnimationHistory.ContainsKey((ushort)SpellAnimation.Regeneration) || DateTime.UtcNow.Subtract(player.SpellAnimationHistory[(ushort)SpellAnimation.Regeneration]).TotalSeconds > 1.5) && TryCastAnyRank("Regeneration", player, _autoStaffSwitch, true))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            if (!Client.ClientTab.mantidScentCbox.Checked || !Client._isRegistered || Client.HasEffect(EffectsBar.MantidScent))
            {
                return true;
            }
            if (!Client.UseItem("Mantid Scent") && !Client.UseItem("Potent Mantid Scent"))
            {
                Client.ClientTab.mantidScentCbox.Checked = false;
                Client.ServerMessage((byte)ServerMessageType.Whisper, "You do not own Mantid Scent");
                return false;
            }
            while (Client.Dialog == null)
            {
                Thread.Sleep(10);
            }
            Client.Dialog.DialogNext();
            Thread.Sleep(500);
            return false;
        }

        private bool DispellAllySuain()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                bool isDispelSuainChecked = ally.AllyPage.dispelSuainCbox.Checked;

                if (isDispelSuainChecked && TryGetSuainedAlly(ally, out Player player, out Client client))
                {

                    Client.UseSpell("ao suain" + player.Curse, player, _autoStaffSwitch, true);
                    Console.WriteLine($"[DispellAllySuain] Player {player.Name}, Hash: {player.GetHashCode()}. IsCursed: {player.IsSuained}");

                    return false;
                     
                }
            }
            return true;
        }

        private bool TryGetSuainedAlly(Ally ally, out Player player, out Client client)
        {
            if (IsAlly(ally, out player, out client))
            {
                Console.WriteLine($"[TryGetSuainedAlly] Player.ID: {player.ID}, Hash: {player.GetHashCode()}, Player {player.Name} is cursed: {player.IsSuained}");
                return player.IsSuained;
            }
            return false;
        }

        private void FasSpiorad()
        {
            if (_needFasSpiorad)
            {
                uint currentMP = Client.CurrentMP;
                DateTime startTime = DateTime.UtcNow;

                while (_needFasSpiorad)
                {
                    int fasSpioradThreshold;

                    bool isFasSpioradChecked = Client.ClientTab.fasSpioradCbox.Checked;
                    bool isThresholdParsed = int.TryParse(Client.ClientTab.fasSpioradText.Text.Trim(), out fasSpioradThreshold);
                    bool isBelowThreshold = Client.CurrentMP <= fasSpioradThreshold;

                    if (isFasSpioradChecked && isThresholdParsed && isBelowThreshold)
                    {
                        Client.UseSpell("fas spiorad", null, _autoStaffSwitch, false);
                    }

                    bool hasManaReachedHalf = Client.CurrentMP >= Client.Stats.MaximumMP / 2U;
                    bool hasManaIncreasedByFivePercent = Client.CurrentMP - currentMP >= Client.Stats.MaximumMP / 20U;
                    bool isDurationExceeded = DateTime.UtcNow.Subtract(startTime).TotalSeconds > 20.0;

                    if (hasManaReachedHalf || hasManaIncreasedByFivePercent || isDurationExceeded)
                    {
                        _needFasSpiorad = false;
                    }

                    Thread.Sleep(10);
                }
            }
        }

        private bool BeagCradh()
        {
            bool isBeagCradhChecked = Client.ClientTab.beagCradhCbox.Checked;
            bool isPlayerCursed = Client.Player.IsCursed;

            if (isBeagCradhChecked && !isPlayerCursed)
            {
                Client.UseSpell("beag cradh", Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool BeagCradhAllies()
        {

            foreach (Ally ally in ReturnAllyList())
            {
                bool isBeagCradhChecked = ally.AllyPage.dbBCCbox.Checked;

                if (isBeagCradhChecked && IsAlly(ally, out Player player, out Client client) && !player.IsCursed)
                {
                    Client.UseSpell("beag cradh", player, _autoStaffSwitch, false);
                    return false;
                }
            }

            return true;
        }

        private bool WakeScroll()
        {
            
            bool isWakeScrollChecked = Client.ClientTab.wakeScrollCbox.Checked;
            bool isRegistered = Client._isRegistered;

            if (isWakeScrollChecked && isRegistered && _nearbyAllies.Any(player => IsAllyAffectedByPramhOrAsleep(player)))
            {
                if (Client.UseItem("Wake Scroll"))
                {
                    foreach (Player player in _nearbyAllies)
                    {
                        Client client = _server.FindClientByName(player.Name);
                        if (client != null)
                        {
                            client.ClearEffect(EffectsBar.Pramh);
                        }
                    }
                }
                return false;
            }

            return true;
        }

        internal bool IsAllyAffectedByPramhOrAsleep(Player player)
        {
            if (!player.IsAsleep)
            {
                Client client = _server.FindClientByName(player.Name);
                return client != null && client.HasEffect(EffectsBar.Pramh);
            }
            return true;
        }

        private bool AoPoison()
        {
            bool isAoPoisonChecked = Client.ClientTab.aoPoisonCbox.Checked;
            bool isPlayerPoisoned = Client.Player.IsPoisoned;
            bool isFungusExtractChecked = Client.ClientTab.fungusExtractCbox.Checked;
            bool shouldUseFungusExtract = DateTime.UtcNow.Subtract(_lastUsedFungusBeetle).TotalSeconds > 1.0;

            //Ao allies
            foreach (Ally ally in ReturnAllyList())
            {
                bool isAoAllyChecked = ally.AllyPage.dispelPoisonCbox.Checked;

                if (isAoAllyChecked && IsAlly(ally, out Player player, out Client client) &&
                    client.HasEffect(EffectsBar.Poison) && player.IsPoisoned && shouldUseFungusExtract)
                {
                    if (Client._isRegistered && Client.HasItem("Fungus Beetle Extract"))
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            Client.UseItem("Fungus Beetle Extract");
                        }
                        _lastUsedFungusBeetle = DateTime.UtcNow;
                    }
                    else
                    {
                        Client.UseSpell("ao puinsein", player, _autoStaffSwitch, false);
                        return false;
                    }
                }
            }

            //Ao self
            if (isAoPoisonChecked && Client.HasEffect(EffectsBar.Poison) && isPlayerPoisoned)
            {
                if (isFungusExtractChecked && Client._isRegistered)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        Client.UseItem("Fungus Beetle Extract");
                    }
                    _lastUsedFungusBeetle = DateTime.UtcNow;
                }
                else
                {
                    Client.UseSpell("ao puinsein", Client.Player, _autoStaffSwitch, false);
                    return false;
                }
            }

            return true;
        }

        private bool Aite()
        {
            bool isAiteChecked = Client.ClientTab.aiteCbox.Checked;
            bool isPlayerAited = Client.Player.IsAited;
            double aiteDuration = Client.Player.AiteDuration;
            string aiteSpell = Client.ClientTab.aiteCombox.Text;

            if (isAiteChecked && !Client.HasEffect(EffectsBar.NaomhAite) && (!isPlayerAited || aiteDuration != 2.0))
            {
                Client.UseSpell(aiteSpell, Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool AiteAllies()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                bool isAiteChecked = ally.AllyPage.dbAiteCbox.Checked;
                string aiteSpell = ally.AllyPage.dbAiteCombox.Text;

                if (!isAiteChecked || !IsAlly(ally, out Player player, out Client client))
                {
                    continue;
                }

                if (client == null || client.HasEffect(EffectsBar.NaomhAite))
                {
                    continue;
                }

                if (player == null || player == client.Player || player.IsAited)
                {
                    continue;
                }

                Client.UseSpell(aiteSpell, player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool FasAllies()
        {

            foreach (Ally ally in ReturnAllyList())
            {
                bool isFasChecked = ally.AllyPage.dbFasCbox.Checked;
                string fasSpell = ally.AllyPage.dbFasCombox.Text;

                if (!isFasChecked || !IsAlly(ally, out Player player, out Client client))
                {
                    continue;
                }

                if (client == null || client.HasEffect(EffectsBar.FasNadur))
                {
                    continue;
                }

                if (player == null || player == client.Player || player.IsAited)
                {
                    continue;
                }

                Client.UseSpell(fasSpell, player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool Fas()
        {
            bool isFasChecked = Client.ClientTab.fasCbox.Checked;
            bool isPlayerFassed = Client.Player.IsFassed;
            double fasDuration = Client.Player.FasDuration;
            string fasSpell = Client.ClientTab.fasCombox.Text;

            if (isFasChecked && !Client.HasEffect(EffectsBar.FasNadur) && (!isPlayerFassed || fasDuration != 2.0))
            {
                Client.UseSpell(fasSpell, Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool DragonScale()
        {
            bool isDragonScaleChecked = Client.ClientTab.dragonScaleCbox.Checked;

            if (isDragonScaleChecked && Client._isRegistered && !Client.HasEffect(EffectsBar.Armachd))
            {
                if (!RecentlyUsedDragonScale)
                {
                    RecentlyUsedDragonScale = true;

                    Console.WriteLine("[DragonScale] Using Dragon's Scale");
                    
                    Client.UseItem("Dragon's Scale");

                    Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => RecentlyUsedDragonScale = false);

                    return false;
                }

            }

            return true;
        }

        private bool Dion()
        {
            bool isDionChecked = Client.ClientTab.dionCbox.Checked;

            if (!isDionChecked || Client.HasEffect(EffectsBar.Dion))
            {
                return false; // Exit early if Dion is not checked or effect already exists
            }

            string dionWhen = Client.ClientTab.dionWhenCombox.Text;
            bool shouldUseSpell = false;

            switch (dionWhen)
            {
                case "Always":
                    shouldUseSpell = true;
                    break;
                case "In Danger":
                    shouldUseSpell = _nearbyValidCreatures.Count > 0;
                    break;
                case "Taking Damage":
                    shouldUseSpell = Client.CurrentHP < Client.MaximumHP;
                    break;
                case "At Percent":
                    shouldUseSpell = Client.CurrentHP * 100U / Client.MaximumHP < Client.ClientTab.dionPctNum.Value;
                    break;
                case "Green Not Nearby":
                    shouldUseSpell = !_nearbyValidCreatures.Any(Delegates.isGreenMantis);
                    break;
            }

            if (shouldUseSpell || (Client.ClientTab.aoSithCbox.Checked && _recentlyAoSithed))
            {
                UseDionOrStone();
                return false; // Spell used, exit the method
            }

            // Reset Ao Sith flag if Dion effect is present
            if (Client.HasEffect(EffectsBar.Dion))
            {
                _recentlyAoSithed = false;
            }

            return true; // No spell used, continue execution
        }

        private bool Armachd()
        {
            bool isArmachdChecked = Client.ClientTab.armachdCbox.Checked;
            
            if (isArmachdChecked && !Client.HasEffect(EffectsBar.Armachd))
            {
                Client.UseSpell("armachd", Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool ArmachdAllies()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                bool isArmachdChecked = ally.AllyPage.dbArmachdCbox.Checked;

                if (!isArmachdChecked || !IsAlly(ally, out Player player, out Client client))
                {
                    continue;
                }

                if (client == null || client.HasEffect(EffectsBar.Armachd))
                {
                    continue;
                }

                if (player == null || player == client.Player || player.HasArmachd)
                {
                    continue;
                }

                Client.UseSpell("armachd", player, _autoStaffSwitch, false);

                return false;

            }

            return true;
        }

        private void UseDionOrStone()
        {
            string dionSpell = Client.ClientTab.dionCombox.Text;

            if (dionSpell == "Glowing Stone" && !RecentlyUsedGlowingStone)
            {
                RecentlyUsedGlowingStone = true;

                Client.UseItem("Glowing Stone");

                Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(_ => RecentlyUsedGlowingStone = false);
            }
            else if (dionSpell != "Glowing Stone")
            {
                Client.UseSpell(dionSpell, null, _autoStaffSwitch, false);
            }
        }

        private bool DispellAllyCurse()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                bool isDispelCurseChecked = ally.AllyPage.dispelCurseCbox.Checked;

                if (isDispelCurseChecked && TryGetCursedAlly(ally, out Player player, out Client client))
                {

                    var cursesToDispel = new HashSet<string> { "cradh", "mor cradh", "ard cradh" };

                    if (cursesToDispel.Contains(player.Curse))
                    {
                        Client.UseSpell("ao " + player.Curse, player, _autoStaffSwitch, true);

                        player.CurseDuration = 0.0;
                        player.Curse = "";
                        Console.WriteLine($"[DispellAllyCurse] Curse data reset on {player.Name}, Hash: {player.GetHashCode()}. Curse: {player.Curse}, CurseDuration: {player.CurseDuration}, IsCursed: {player.IsCursed}");
                        
                        return false;

                    }
                }
            }
            return true;
        }
        
        private bool TryGetCursedAlly(Ally ally, out Player player, out Client client)
        {

            if (IsAlly(ally, out player, out client))
            {
                Console.WriteLine($"[TryGetCursedAlly] Player.ID: {player.ID}, Hash: {player.GetHashCode()}, Player {player.Name} is cursed: {player.IsCursed}");
                return player.IsCursed;
            }
            return false;
        }

        internal bool IsAlly(Ally ally, out Player player, out Client client)
        {
            player = Client.GetNearbyPlayer(ally.Name);

            if (player == null)
            {
                client = null;
                return false;
            }

            client = Server.FindClientByName(ally.Name);

            if (client != null && client != Client)
            {
                return true;
            }

            return false;
        }

        private bool Heal()
        {
            if (Client.HasEffect(EffectsBar.FasSpiorad))
            {
                return false;
            }

            int loopPercentThreshold = 20;

            while (loopPercentThreshold <= 100 && !_needFasSpiorad)
            {
                foreach (Player player in Client.GetNearbyPlayerList())
                {
                    if (IsAllyAlreadyListed(player.Name) || player == Client.Player)
                    {
                        Ally ally = ReturnAllyList().FirstOrDefault(a => a.Name == player.Name);
                        AllyPage allyPage = ally?.AllyPage;
                        Client client = Server.FindClientByName(player.Name);

                        if (client == null) continue;

                        if ((allyPage == null && client != Client) || (client == Client && !Client.ClientTab.healCbox.Checked) || (client != Client && !allyPage.dbIocCbox.Checked))
                        {
                            continue;
                        }

                        if (ShouldExcludePlayer(player)) continue;

                        string healSpell = player == client.Player ? client.ClientTab.healCombox.Text : allyPage.dbIocCombox.Text;

                        //Console.WriteLine($"[CastDefensiveSpells] heal spell: {healSpell}");

                        if (loopPercentThreshold == 20 && player != client.Player && (client.HasSpell("ard ioc comlha") || client.HasSpell("mor ioc comlha")))
                        {

                            int alliesInNeed = Client.GetNearbyAllies().Count(p => p != Client.Player && IsAllyInNeed(p));

                            if (alliesInNeed > 2)
                            {
                                healSpell = Client.HasSpell("ard ioc comlha") ? "ard ioc comlha" : "mor ioc comlha";
                            }
                        }

                        if (!Client.GetNearbyPlayerList().Any(player => ShouldExcludePlayer(player)) || player == Client.Player || healSpell.Contains("comlha"))
                        {
                            
                            int healAtPercent = (int)((player == Client.Player) ? Client.ClientTab.healPctNum.Value : allyPage.dbIocNumPct.Value);
                            healAtPercent = ((healAtPercent > loopPercentThreshold) ? loopPercentThreshold : healAtPercent);
                            bool shouldHeal = player.NeedsHeal || (client.CurrentHP * 100 / client.MaximumHP) <= healAtPercent;

                            if (player.NeedsHeal || shouldHeal)
                            {

                                uint healAmount = (uint)Client.CalculateHealAmount(healSpell);

                                List<Player> playersHealed = new List<Player>();

                                if (!(healSpell == "ard ioc comlha") && !(healSpell == "mor ioc comlha"))
                                {
                                    if (Client.UseSpell(healSpell, player, _autoStaffSwitch, false))
                                    {
                                        playersHealed.Add(player);
                                    }
                                }
                                else if (Client.UseSpell(healSpell, null, _autoStaffSwitch, false))
                                {
                                    playersHealed.AddRange(Client.GetNearbyAllies());
                                }
                                foreach (Player p in playersHealed)
                                {

                                    if (((client != null) ? client.Player : null) == player)
                                    {

                                        // Calculate the new health, ensuring it does not exceed the maximum
                                        uint newHealth = Math.Min(client.CurrentHP + healAmount, client.MaximumHP);

                                        // Update the health and print the debug message
                                        client.CurrentHP = newHealth;
                                        Console.WriteLine($"[Heal] {client.Name} healed for {healAmount} HP. New HP: {client.CurrentHP} / {client.MaximumHP}");
                                        // Update health percentage and needs heal status
                                        p.HealthPercent = (byte)(client.CurrentHP * 100 / client.MaximumHP);
                                        p.NeedsHeal = p.HealthPercent <= healAtPercent;

                                    }
                                    else
                                    {
                                        // We don't have access to the actual health value, so we'll just update it based on a health percentage
                                        // Calculate the new health percentage based on loopPercentThreshold, ensuring it does not exceed 100%
                                        byte newHealthPercent = (byte)Math.Min(p.HealthPercent + 20, 100);

                                        // Update health percentage and needs heal status
                                        p.HealthPercent = newHealthPercent;
                                        p.NeedsHeal = p.HealthPercent <= healAtPercent;
                                    }                                    

                                }
                            }
                        }
                    }
                }
               
                loopPercentThreshold += 20;
            }
            return true;
        }

        private bool IsAllyInNeed(Player player)
        {
            return player.HealthPercent < 20 || player.NeedsHeal;
        }

        internal bool ShouldExcludePlayer(Player player)
        {
            // Checks if playerToCheck is not in friend list, is not the reference player,
            // and is either at the same location as reference player or is hidden
            return !Client.ClientTab.friendList.Items.OfType<string>().Contains(player.Name, StringComparer.CurrentCultureIgnoreCase) && player != Client.Player && (Equals(player.Location, Client.Player.Location) || player._isHidden);
        }

        private bool BubbleBlock()
        {
            bool isBubbleBlockChecked = Client.ClientTab.bubbleBlockCbox.Checked;
            bool isSpamBubbleChecked = Client.ClientTab.spamBubbleCbox.Checked;
            bool isFollowChecked = Client.ClientTab.followCbox.Checked;
            string walkMap = Client.ClientTab.walkMapCombox.Text;

            if (isBubbleBlockChecked && isSpamBubbleChecked)
            {
                if (Client.UseSpell("Bubble Block", null, true, true))
                {
                    return false;
                }
            }
            else if (isBubbleBlockChecked && Client._okToBubble)
            {
                if (walkMap == "WayPoints")
                {
                    if (CastBubbleBlock())
                    {
                        return false;
                    }
                }
                else if (isFollowChecked && Client._destnationReached && CastBubbleBlock())
                {
                    return false;
                }
            }

            return true;
        }
        private bool CastBubbleBlock()
        {
            // Check if the player has moved since the last bubble was cast.
            bool hasMoved = !Location.Equals(_lastBubbleLocation, Client._serverLocation);

            // Define the preferred order of bubble spells.
            var bubbleSpells = new[] { "Bubble Block", "Bubble Shield" };

            // Attempt to cast a bubble spell if the player has moved or the current bubble type is not set.
            if (hasMoved || string.IsNullOrEmpty(_bubbleType))
            {
                foreach (var spellName in bubbleSpells)
                {
                    if (Client.HasSpell(spellName) && Client.CanUseSpell(Client.Spellbook[spellName], null))
                    {
                        Client.UseSpell(spellName, null, _autoStaffSwitch, true);
                        _lastBubbleLocation = Client._serverLocation;
                        _bubbleType = spellName.ToUpperInvariant().Contains("BLOCK") ? "BLOCK" : "SHIELD";
                        return true;
                    }
                }
            }
            else
            {
                // If the player hasn't moved, attempt to refresh the current bubble type without the "true" force argument.
                string spellToCast = _bubbleType == "BLOCK" ? "Bubble Block" : "Bubble Shield";
                if (Client.HasSpell(spellToCast) && Client.CanUseSpell(Client.Spellbook[spellToCast], null))
                {
                    if (Client.UseSpell(spellToCast, null, _autoStaffSwitch, false))
                    _lastBubbleLocation = Client._serverLocation;
                    return true;

                }
            }

            return false;
        }

        private bool Hide()
        {
            if (CastHide())
            {
                bool_13 = true;
                return false;
            }

            return true;
        }
        private bool CastHide()
        {
            bool isHideChecked = Client.ClientTab.hideCbox.Checked;
            bool canUseSpells = Client._map.CanUseSpells;

            if (isHideChecked && canUseSpells)
            {
                string spellName = "";
                if (Client.HasSpell("Hide"))
                {
                    spellName = "Hide";
                }
                else if (Client.HasSpell("White Bat Stance"))
                {
                    spellName = "White Bat Stance";
                }
                if (!Client.HasEffect(EffectsBar.Hide) || DateTime.UtcNow.Subtract(Client._lastHidden).TotalSeconds > 50.0)
                {
                    Client.UseSkill("Assail");
                    Client.UseSpell(spellName, null, true, true);
                    Client._lastHidden = DateTime.UtcNow;
                }
                return true;
            }
            return false;
        }

        private bool CastOffensiveSpells()
        {
            _nearbyValidCreatures = Client.GetNearbyValidCreatures(11);

            if (_nearbyValidCreatures.Count > 0)
            {
                _nearbyValidCreatures = _nearbyValidCreatures.OrderBy(Delegates.NextRandom)
                                                           .ToList();
            }
            if (IsStrangerNearby() && _nearbyValidCreatures.Count > 0)
            {
                _nearbyValidCreatures.RemoveAll(Delegates.CreaturesExisitingLessThan2s);
            }
            if (IsStrangerNearby()) //we don't want to cast on overlapped creatures if a stranger is nearby
            {
                var duplicates = _nearbyValidCreatures
                    .SelectMany((creature, index) => _nearbyValidCreatures
                        .Where((otherCreature, otherIndex) => index != otherIndex && creature.Location == otherCreature.Location)
                        .Take(1))
                    .Distinct()
                    .ToList();

                foreach (var duplicate in duplicates)
                {
                    _nearbyValidCreatures.Remove(duplicate);
                }
            }
            if (EnemyPage != null)
            {
                if (EnemyPage.ignoreCbox.Checked)
                {
                    _nearbyValidCreatures.RemoveAll(CreaturesToIgnore);
                }
                if (EnemyPage.priorityCbox.Checked)
                {
                    List<Creature> priority = new List<Creature>();
                    List<Creature> nonPriority = new List<Creature>();
                    foreach (Creature creature in _nearbyValidCreatures)
                    {
                        if (EnemyPage.priorityLbox.Items.Contains(creature.SpriteID.ToString()))
                        {
                            priority.Add(creature);
                        }
                        else if (!EnemyPage.priorityOnlyCbox.Checked)
                        {
                            nonPriority.Add(creature);
                        }
                    }
                    if (EnemyPage.spellAllRbtn.Checked)
                    {
                        creature = null;
                        if (priority.Count > 0 && DecideAndExecuteEngagementStrategy(EnemyPage, priority))
                        {
                            bool_13 = true;
                            return true;
                        }
                        if (nonPriority.Count > 0 && DecideAndExecuteEngagementStrategy(EnemyPage, nonPriority))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                    else
                    {
                        if (priority.Count > 0 && SpellOneAtATime(EnemyPage, priority))
                        {
                            bool_13 = true;
                            return true;
                        }
                        if (!priority.Contains(creature) && nonPriority.Count > 0 && SpellOneAtATime(EnemyPage, nonPriority))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                }
                //Spell all at once
                else if (EnemyPage.spellAllRbtn.Checked)
                {
                    creature = null;
                    if (_nearbyValidCreatures.Count > 0 && DecideAndExecuteEngagementStrategy(EnemyPage, _nearbyValidCreatures))
                    {
                        bool_13 = true;
                        return true;
                    }
                }
                //Spell one at a time
                else if (_nearbyValidCreatures.Count > 0 && SpellOneAtATime(EnemyPage, _nearbyValidCreatures))
                {
                    bool_13 = true;
                    return true;
                }
            }
            else
            {
                List<Creature> creatureList = new List<Creature>();
                foreach (Enemy enemy in ReturnEnemyList())
                {
                    creatureList.Clear();
                    foreach (Creature creature in Client.GetCreaturesInRange(11, new ushort[] { enemy.SpriteID }))
                    {
                        if (creature.SpriteID.ToString() == enemy.SpriteID.ToString())
                        {
                            creatureList.Add(creature);
                        }
                    }
                    creatureList.OrderBy(new Func<Creature, Creature>(Delegates.IsCreature));
                    if (creatureList.Count > 0)
                    {
                        if (enemy.EnemyPage.spellAllRbtn.Checked)
                        {
                            creature = null;
                            if (DecideAndExecuteEngagementStrategy(enemy.EnemyPage, creatureList))
                            {
                                bool_13 = true;
                                return true;
                            }
                        }
                        else if (SpellOneAtATime(enemy.EnemyPage, creatureList))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                }
            }
            if (EnemyPage != null)
            {
                if (EnemyPage.priorityCbox.Checked)
                {
                    List<Creature> priority = new List<Creature>();
                    List<Creature> nonPriority = new List<Creature>();
                    foreach (Creature creature in _nearbyValidCreatures)
                    {
                        if (EnemyPage.priorityLbox.Items.Contains(creature.SpriteID.ToString()))
                        {
                            priority.Add(creature);
                        }
                        else if (!EnemyPage.priorityOnlyCbox.Checked)
                        {
                            nonPriority.Add(creature);
                        }
                    }
                    if (CastAttackSpell(EnemyPage, priority))
                    {
                        return true;
                    }
                    if (CastAttackSpell(EnemyPage, nonPriority))
                    {
                        return true;
                    }
                }
                else if (CastAttackSpell(EnemyPage, _nearbyValidCreatures))
                {
                    return true;
                }
            }
            else
            {
                List<Creature> toAttack = new List<Creature>();
                foreach (Enemy enemy in ReturnEnemyList())
                {
                    toAttack.Clear();
                    foreach (Creature creature in Client.GetCreaturesInRange(11, new ushort[] {enemy.SpriteID}))
                    {
                        if (creature.SpriteID.ToString() == enemy.SpriteID.ToString())
                        {
                            toAttack.Add(creature);
                        }
                    }
                    toAttack.OrderBy(new Func<Creature, Creature>(Delegates.IsCreature));
                    if (CastAttackSpell(enemy.EnemyPage, toAttack))
                    {
                        return true;
                    }
                }
            }
            bool_13 = false;
            return false;
        }

        private bool CastAttackSpell(EnemyPage enemyPage, List<Creature> creatureList)
        {
            bool result = false;

            if (!enemyPage.targetCbox.Checked || creatureList.Count == 0)
            {
                return false;
            }

            if (enemyPage.spellOneRbtn.Checked)
            {
                if (!creatureList.Contains(creature))
                {
                    creature = creatureList.OrderBy(new Func<Creature, int>(DistanceFromClientLocation)).FirstOrDefault<Creature>();
                }
                if (creature == null)
                {
                    return false;
                }
                creatureList = new List<Creature>
                {
                    creature
                };
            }
            if (EnemyPage != null && EnemyPage.mpndDioned.Checked)
            {
                if (!enemyPage.attackCboxOne.Checked && !enemyPage.attackCboxTwo.Checked)
                {
                    if (!_nearbyValidCreatures.Any(new Func<Creature, bool>(Delegates.IsDioned)))
                    {
                        goto IL_1A5;
                    }
                }
                else if (!_nearbyValidCreatures.All(new Func<Creature, bool>(Delegates.CanCastPND)))
                {
                    goto IL_1A5;
                }
                Creature creatureTarget = _nearbyValidCreatures.OrderBy(new Func<Creature, int>(DistanceFromClientLocation)).FirstOrDefault(new Func<Creature, bool>(Delegates.CanCastPND));
                if (creatureTarget != null)
                {
                    return Client.TryUseAnySpell(new[] { "ard pian na dion", "mor pian na dion", "pian na dion" }, creatureTarget, _autoStaffSwitch, false);
                }
            }
        IL_1A5:
            if (!_isSilenced)
            {
                if (enemyPage.attackCboxTwo.Checked)
                {
                    Creature target = SelectAttackTarget(enemyPage, creatureList, enemyPage.attackComboxTwo.Text);
                    if (target != null)
                    {
                        string spellName = enemyPage.attackComboxTwo.Text;
                        uint fnvHash = Utility.CalculateFNV(spellName);

                        switch (fnvHash)
                        {
                            case 1007116742U: // Supernova Shot
                                return Client.UseSpell("Supernova Shot", target, _autoStaffSwitch, false);

                            case 1285349432U: // Shock Arrow
                                return Client.UseSpell("Shock Arrow", target, _autoStaffSwitch, false) && 
                                       Client.UseSpell("Shock Arrow", target, _autoStaffSwitch, false);

                            case 1792178996U: // Volley
                                return Client.UseSpell("Volley", target, _autoStaffSwitch, true);

                            case 2591503996U: // MSPG
                                if (enemyPage.mspgPct.Checked && Client.ManaPct < 80)
                                {
                                    _manaLessThanEightyPct = true;
                                    Client.UseSpell("fas spiorad", null, _autoStaffSwitch, false);
                                    return true;
                                }
                                return Client.UseSpell("mor strioch pian gar", Client.Player, _autoStaffSwitch, true);


                            case 3122026787U: // Unholy Explosion
                                return Client.UseSpell("Unholy Explosion", target, _autoStaffSwitch, false);

                            case 3210331623U: // Cursed Tune
                                return TryCastAnyRank("Cursed Tune", target, _autoStaffSwitch, false);

                            case 3848328981U: // M/DSG
                                return Client.UseSpell("mor deo searg gar", Client.Player, _autoStaffSwitch, false) || 
                                       Client.UseSpell("deo searg gar", Client.Player, _autoStaffSwitch, false);        
                    }
                }
                }
                if (enemyPage.attackCboxOne.Checked)
                {
                    Creature target = SelectAttackTarget(enemyPage, creatureList, enemyPage.attackComboxOne.Text);
                    if (target != null)
                    {
                        string spellName = enemyPage.attackComboxOne.Text;
                        switch (spellName)
                        {
                            case "lamh":
                                return Client.TryUseAnySpell(new[] { "beag athar lamh", "beag srad lamh", "athar lamh", "srad lamh", "Howl" }, null, _autoStaffSwitch, false);

                            case "A/DS":
                                return Client.TryUseAnySpell(new[] { "ard deo searg", "deo searg" }, target, _autoStaffSwitch, false);

                            case "A/M/PND":
                                return Client.TryUseAnySpell(new[] { "ard pian na dion", "mor pian na dion", "pian na dion" }, target, _autoStaffSwitch, false);

                            case "Frost Arrow":
                                return TryCastAnyRank("Frost Arrow", target, _autoStaffSwitch, false);

                            default:
                                return Client.UseSpell(spellName, target, _autoStaffSwitch, false) || TryCastAnyRank(spellName, target, _autoStaffSwitch, false);
                        }
                    }
                }
            }
            if (_isSilenced && enemyPage.mpndSilenced.Checked)
            {
                Creature mpndTarget = SelectAttackTarget(enemyPage, creatureList, "A/M/PND");
                return Client.TryUseAnySpell(new[] { "ard pian na dion", "mor pian na dion", "pian na dion" }, mpndTarget, _autoStaffSwitch, false);
            }
            if (!_isSilenced || !enemyPage.mspgSilenced.Checked || SelectAttackTarget(enemyPage, creatureList, "MSPG") == null)
            {
                return result;
            }
            if (enemyPage.mspgPct.Checked && Client.ManaPct < 80)
            {
                _manaLessThanEightyPct = true;
                return Client.UseSpell("fas spiorad", null, _autoStaffSwitch, false); ;
            }
            Client.UseSpell("mor strioch pian gar", Client.Player, _autoStaffSwitch, true); //Adam check this
            return true;
        }

        private bool TryCastAnyRank(string spellNAme, Creature creatureTarget = null, bool autoStaffSwitch = true, bool keepSpellAfterUse = true)
        {
            for (int i = 20; i > 0; i--)
            {
                if (Client.UseSpell(spellNAme + " " + i.ToString(), creatureTarget, autoStaffSwitch, keepSpellAfterUse))
                {
                    return true;
                }
            }
            return false;
        }


        private bool SpellOneAtATime(EnemyPage enemyPage, List<Creature> creatureList)
        {
            foreach (var creature in creatureList.OrderBy(c => DistanceFromClientLocation(c)))
            {

                if (!creatureList.Contains(this.creature))
                {
                    this.creature = creatureList.OrderBy(new Func<Creature, int>(DistanceFromClientLocation)).FirstOrDefault<Creature>();
                }
                if (creature == null)
                {
                    continue; 
                }
                this.creature = creature;

                if (enemyPage.pramhFirstRbtn.Checked && enemyPage.spellsControlCbox.Checked && !creature.IsAsleep)
                {
                    Client.UseSpell(enemyPage.spellsControlCombox.Text, creature, _autoStaffSwitch, false);
                    return true;
                }
                if (enemyPage.spellFirstRbtn.Checked && ((enemyPage.spellsFasCbox.Checked && !creature.IsFassed) || (enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)))
                {
                    if (CastFasOrCurse(enemyPage, creature))
                    {
                        return true;
                    }
                }
                else if (enemyPage.pramhFirstRbtn.Checked && (!enemyPage.spellsControlCbox.Checked || creature.IsAsleep) && ((enemyPage.spellsFasCbox.Checked && !creature.IsFassed) || (enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)))
                {
                    if (CastFasOrCurse(enemyPage, creature))
                    {
                        return true;
                    }
                }
                else if (enemyPage.spellFirstRbtn.Checked && (!enemyPage.spellsFasCbox.Checked || creature.IsFassed) && (!enemyPage.spellsCurseCbox.Checked || creature.IsCursed) && enemyPage.spellsControlCbox.Checked && !creature.IsAsleep)
                {
                    Client.UseSpell(enemyPage.spellsControlCombox.Text, creature, _autoStaffSwitch, false);
                    return true;
                }
            }

            // If the method reaches this point, it means no spells were cast successfully on any of the creatures
            return false;
        }

        private bool CastFasOrCurse(EnemyPage enemyPage, Creature creature)
        {
            if (enemyPage.fasFirstRbtn.Checked && enemyPage.spellsFasCbox.Checked && !creature.IsFassed)
            {
                bool_13 = true;
                Client.UseSpell(enemyPage.spellsFasCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            if (enemyPage.curseFirstRbtn.Checked && enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)
            {
                bool_13 = true;
                Client.UseSpell(enemyPage.spellsCurseCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            if ((!enemyPage.fasFirstRbtn.Checked || !enemyPage.spellsFasCbox.Checked || creature.IsFassed) && enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)
            {
                bool_13 = true;
                Client.UseSpell(enemyPage.spellsCurseCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            if ((!enemyPage.curseFirstRbtn.Checked || !enemyPage.spellsCurseCbox.Checked || creature.IsCursed) && enemyPage.spellsFasCbox.Checked && !creature.IsFassed)
            {
                bool_13 = true;
                Client.UseSpell(enemyPage.spellsFasCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            return false;
        }


        private bool DecideAndExecuteEngagementStrategy(EnemyPage enemyPage, List<Creature> creatureList)
        {
            if (enemyPage.pramhFirstRbtn.Checked)
            {
                if (enemyPage.spellsControlCbox.Checked)
                    if (ExecutePramhStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
                if (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked)
                    if (ExecuteDebuffStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
            }
            else if (enemyPage.spellFirstRbtn.Checked)
            {
                if (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked)
                    if (ExecuteDebuffStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
                if (enemyPage.spellsControlCbox.Checked)
                    if (ExecutePramhStrategy(enemyPage, creatureList))
                    {
                        return true;
                    }
            }
            return false;
        }

        private bool ExecuteDebuffStrategy(EnemyPage enemyPage, List<Creature> creatureList)
        {
            List<Creature> eligibleCreatures = creatureList.Where(Delegates.IsNotFassedOrNotCursed).ToList();
            if (eligibleCreatures != null && eligibleCreatures.Any() && (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked))
            {
                if (enemyPage.fasFirstRbtn.Checked)
                {
                    if (enemyPage.spellsFasCbox.Checked)
                    {
                        if (CastFasIfApplicable(enemyPage, eligibleCreatures))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                    if (enemyPage.spellsCurseCbox.Checked)
                    {
                        if (CastCurseIfApplicable(enemyPage, eligibleCreatures))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                }
                else if (enemyPage.curseFirstRbtn.Checked)
                {
                    if (enemyPage.spellsCurseCbox.Checked)
                    {
                        if (CastCurseIfApplicable(enemyPage, eligibleCreatures))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }
                    if (enemyPage.spellsFasCbox.Checked)
                    {
                        if (CastFasIfApplicable(enemyPage, eligibleCreatures))
                        {
                            bool_13 = true;
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        private bool CastCurseIfApplicable(EnemyPage enemyPage, List<Creature> creatures)
        {
            lock (_lock)
            {
                //Console.WriteLine($"[CastCurseIfApplicable] Total creatures: {creatures.Count}");

                // Filter creatures that are not cursed
                var eligibleCreatures = creatures.Where(Delegates.IsNotCursed);

                //Console.WriteLine($"[CastCurseIfApplicable] Eligible creatures (not cursed): {eligibleCreatures.Count()}");

                // Select the nearest eligible creature if specified, otherwise select any eligible creature
                Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                    ? eligibleCreatures.OrderBy(new Func<Creature, int>(DistanceFromServerLocation)).FirstOrDefault()
                    : eligibleCreatures.FirstOrDefault();

                // If a target is found and casting curses is enabled, cast the curse spell
                if (targetCreature != null && enemyPage.spellsCurseCbox.Checked)
                {
                    //Console.WriteLine($"[CastCurseIfApplicable] Targeting creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, LastCursed: {targetCreature.LastCursed}, IsCursed: {targetCreature.IsCursed}");
                    Client.UseSpell(enemyPage.spellsCurseCombox.Text, targetCreature, _autoStaffSwitch, false);
                    bool_13 = true; // Indicate that an action was taken
                    return true;
                }

                return false; // No action was taken

            }
        }

        private bool CastFasIfApplicable(EnemyPage enemyPage, List<Creature> creatures)
        {
            lock (_lock)
            {
                //Console.WriteLine($"[CastFasIfApplicable] Total creatures: {creatures.Count}");

                // Filter creatures that are not fassed
                var eligibleCreatures = creatures.Where(Delegates.IsNotFassed);

                //Console.WriteLine($"[CastFasIfApplicable] Eligible creatures (not fassed): {eligibleCreatures.Count()}");

                // Select the nearest eligible creature if specified, otherwise select any eligible creature
                Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                    ? eligibleCreatures.OrderBy(new Func<Creature, int>(DistanceFromServerLocation)).FirstOrDefault()
                    : eligibleCreatures.FirstOrDefault();

                // If a target is found and casting the 'fas' spell is enabled, cast the spell
                if (targetCreature != null && enemyPage.spellsFasCbox.Checked)
                {
                    //Console.WriteLine($"[CastFasIfApplicable] Targeting creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, LastFassed: {targetCreature.LastFassed}, IsFassed: {targetCreature.IsFassed}");
                    Client.UseSpell(enemyPage.spellsFasCombox.Text, targetCreature, _autoStaffSwitch, false);
                    bool_13 = true; // Indicate that an action was taken
                    return true;
                }

                return false; // No action was taken
            }
          
        }

        private Creature SelectAttackTarget(EnemyPage enemyPage, List<Creature> creatureList, string spellName = "")
        {
            List<Creature> attackList = new List<Creature>();

            bool flag = enemyPage.attackCboxTwo.Checked && enemyPage.targetCbox.Checked && 
                        (enemyPage.attackComboxTwo.Text == "MSPG" || enemyPage.attackComboxTwo.Text == "M/DSG");


            if (enemyPage.targetCursedCbox.Checked && enemyPage.targetFassedCbox.Checked)
            {
                using (List<Creature>.Enumerator enumerator = creatureList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Creature creature = enumerator.Current;
                        if (creature.IsCursed && creature.IsFassed)
                        {
                            attackList.Add(creature);
                        }
                        else if (flag && creature.Location.DistanceFrom(Client._serverLocation) <= GetFurthestClient())
                        {
                            attackList.Clear();
                            break;
                        }
                    }
                }
            }
            else if (enemyPage.targetCursedCbox.Checked && !enemyPage.targetFassedCbox.Checked)
            {
                using (List<Creature>.Enumerator enumerator = creatureList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Creature creature = enumerator.Current;
                        if (creature.IsCursed)
                        {
                            attackList.Add(creature);
                        }
                        else if (flag && creature.Location.DistanceFrom(Client._serverLocation) <= GetFurthestClient())
                        {
                            attackList.Clear();
                            break;
                        }
                    }
                }
            }
            else if (enemyPage.targetFassedCbox.Checked && !enemyPage.targetCursedCbox.Checked)
            {
                using (List<Creature>.Enumerator enumerator = creatureList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Creature creature = enumerator.Current;
                        if (creature.IsFassed)
                        {
                            attackList.Add(creature);
                        }
                        else if (flag && creature.Location.DistanceFrom(Client._serverLocation) <= GetFurthestClient())
                        {
                            attackList.Clear();
                            break;
                        }
                    }
                }
            }
            else if (!enemyPage.targetCursedCbox.Checked && !enemyPage.targetFassedCbox.Checked)
            {
                attackList = creatureList;
            }

            if (attackList.Count == 0)
            {
                return null;
            }
            if (!(spellName == "Frost Arrow"))
            {
                if (!(spellName == "Cursed Tune"))
                {
                    if (!(spellName == "lamh") && !(spellName == "Shock Arrow") && !(spellName == "Volley"))
                    {
                        if (spellName != "A/M/PND")
                        {
                            attackList = attackList.Where(new Func<Creature, bool>(Delegates.IsNotDioned)).ToList<Creature>();
                        }
                        if (enemyPage.expectedHitsNum.Value > 0m)
                        {
                            attackList = attackList.Where(creature => CalculateHitCounter(creature, enemyPage)).ToList<Creature>();
                        }
                    }
                }
                else
                {
                    attackList = attackList.Where(new Func<Creature, bool>(Delegates.IsNotPoisoned)).ToList<Creature>();
                }
            }
            else
            {
                attackList = attackList.Where(new Func<Creature, bool>(Delegates.IsNotFrozen)).ToList<Creature>();
            }
            if (attackList.Count == 0)
            {
                return null;
            }
            if (creature != null && attackList.Contains(creature))
            {
                return creature;
            }
            if (!(enemyPage.targetCombox.Text == "Nearest") && (!_isSilenced || !enemyPage.mpndSilenced.Checked))
            {
                if (!enemyPage.targetCombox.Text.Contains("Cluster") || flag)
                {
                    return null;
                }
                int maxDistance = (enemyPage.targetCombox.Text == "Cluster 29") ? 3 : ((enemyPage.targetCombox.Text == "Cluster 13") ? 2 : 1);
                Dictionary<Creature, int> dictionary = new Dictionary<Creature, int>();
                List<Creature> list2 = Client.GetListOfValidCreatures(12).Concat(_playersExistingOver250ms).ToList<Creature>();
                List<Creature> list3 = Client.GetNearbyValidCreatures(12);
                List<Creature> list4 = list3.Where(creature => CONSTANTS.GREEN_BOROS.Contains(creature.SpriteID) || CONSTANTS.RED_BOROS.Contains(creature.SpriteID)).ToList();
                if (!list2.Contains(Client.Player))                {
                    list2.Add(Client.Player);
                }
                foreach (Creature c in list2)
                {
                    int num2 = 0;
                    foreach (Creature creature in attackList)
                    {
                        if (c.Location.DistanceFrom(creature.Location) <= maxDistance || (maxDistance > 1 && IsDiagonallyAdjacent(c.Location.Point, creature.Location.Point, maxDistance)))
                        {
                            num2++;
                        }
                    }
                    bool flag2 = false;
                    using (List<Creature>.Enumerator enumerator2 = list4.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            Creature creature = enumerator2.Current;
                            if (c == creature)
                            {
                                flag2 = true;
                                break;
                            }
                            foreach (Location loc in Client.GetAdjacentPoints(creature))
                            {
                                if (c.Location.DistanceFrom(loc) <= maxDistance || IsDiagonallyAdjacent(c.Location.Point, loc.Point, maxDistance))
                                {
                                    flag2 = true;
                                    break;
                                }
                            }
                        }
                        goto IL_5EA;
                    }
                IL_5DA:
                    dictionary.Add(c, num2);
                    continue;
                IL_5EA:
                    if (!flag2)
                    {
                        goto IL_5DA;
                    }
                }
                foreach (Creature creature in dictionary.Keys.ToList<Creature>())
                {
                    if ((creature.Type == CreatureType.Aisling || (creature.Type == CreatureType.WalkThrough && !list3.Contains(creature)) || creature.IsDioned) && dictionary[creature] <= 1)
                    {
                        dictionary.Remove(creature);
                    }
                }
                if (dictionary.Count <= 0)
                {
                    return null;
                }
                return dictionary.OrderByDescending(new Func<KeyValuePair<Creature, int>, int>(Delegates.KVPMatch)).ThenBy(new Func<KeyValuePair<Creature, int>, DateTime>(Delegates.KVPCreation)).First<KeyValuePair<Creature, int>>().Key;
            }
            else
            {
                if (attackList.Count <= 0)
                {
                    return null;
                }
                return attackList.OrderBy(creature => DistanceFromServerLocation(creature)).FirstOrDefault();

            }
        }

        private bool IsDiagonallyAdjacent(Point location, Point otherLocation, int maxDistance)
        {
            int adjustedDistance = maxDistance - 1;
            int deltaX = Math.Abs(location.X - otherLocation.X);
            int deltaY = Math.Abs(location.Y - otherLocation.Y);

            return deltaX == adjustedDistance && deltaY == adjustedDistance;
        }

        internal bool CalculateHitCounter(Creature creature, EnemyPage enemyPage)
        {
            if (creature.HealthPercent == 0 && creature.SpellAnimationHistory.Count != 0 && creature._animation != 33 && DateTime.UtcNow.Subtract(creature._lastUpdate).TotalSeconds <= 1.5)
            {
                return creature._hitCounter < enemyPage.expectedHitsNum.Value;
            }
            if (creature.HealthPercent != 0 && creature._hitCounter > enemyPage.expectedHitsNum.Value)
            {
                creature._hitCounter = (int)enemyPage.expectedHitsNum.Value - 1;
            }
            return true;
        }

        private int GetFurthestClient()
        {
            int result;
            try
            {
                List<Client> nearbyClients = Client.GetNearbyPlayerList()
                    .Select(player => Server.FindClientByName(player?.Name))
                    .Where(client => client != null)
                    .ToList();

                if (nearbyClients.Any())
                {
                    Client furthestClient = nearbyClients
                        .OrderByDescending(client => CalculateDistanceFromBaseClient(client))
                        .First();

                    result = 11 - furthestClient._serverLocation.DistanceFrom(Client._serverLocation);
                }
            }
            catch (Exception ex)
            {
                // Handle exception if needed
                Console.WriteLine($"Error in GetFurthestClient: {ex.Message}");
            }

            return 11; // Default value if no nearby clients or an exception occurs
        }

        private int CalculateDistanceFromBaseClient(Client client)
        {
            return client._serverLocation.DistanceFrom(Client._serverLocation);
        }

        private bool ExecutePramhStrategy(EnemyPage enemyPage, List<Creature> creatures)
        {
            List<Creature> creatureList = FilterCreaturesByControlStatus(enemyPage, creatures);
            if (CONSTANTS.GREEN_BOROS.Contains(enemyPage.Enemy.SpriteID))
            {
                List<Creature> greenBorosInRange = Client.GetCreaturesInRange(8, CONSTANTS.GREEN_BOROS.ToArray());
                foreach (Creature creature in greenBorosInRange.ToList<Creature>())
                {
                    foreach (Location location in Client.GetExitLocationsAsObstacles(new Location(Client._map.MapID, 0, 0)))
                    {
                        if (creature.Location.DistanceFrom(location) <= 3)
                        {
                            greenBorosInRange.Remove(creature);
                        }
                    }
                }
                creatureList.AddRange(greenBorosInRange);
            }
            Creature targetCreature = creatureList.FirstOrDefault<Creature>();
            if (targetCreature != null && creatureList.Any() && enemyPage.spellsControlCbox.Checked && !targetCreature.IsAsleep)
            {
                Console.WriteLine($"[ExecutePramhStrategy] Targeting creature ID: {targetCreature.ID}, Hash: {targetCreature.GetHashCode()}, Name: {targetCreature.Name}, LastPramhd: {DateTime.UtcNow}");
                Client.UseSpell(enemyPage.spellsControlCombox.Text, creatureList.FirstOrDefault<Creature>(), _autoStaffSwitch, false);
                return true;
            }
            return false;
        }

        private List<Creature> SelectCreaturesForPramh(EnemyPage enemyPage, List<Creature> creatures)
        {
            List<Creature> selectedCreatures = FilterCreaturesByControlStatus(enemyPage, creatures);

            if (CONSTANTS.GREEN_BOROS.Contains(enemyPage.Enemy.SpriteID))
            {
                var additionalCreatures = Client.GetCreaturesInRange(8, CONSTANTS.GREEN_BOROS.ToArray())
                    .Where(creature => !Client.GetExitLocationsAsObstacles(new Location(Client._map.MapID, 0, 0))
                    .Any(location => creature.Location.DistanceFrom(location) <= 3))
                    .ToList();

                selectedCreatures.AddRange(additionalCreatures);
            }

            return selectedCreatures;
        }

        private List<Creature> FilterCreaturesByControlStatus(EnemyPage enemyPage, List<Creature> creatureListIn)
        {
            bool isSuainSelected = enemyPage.spellsControlCombox.Text.Equals("suain", StringComparison.OrdinalIgnoreCase);

            return creatureListIn.Where(creature =>
                isSuainSelected ? !creature.IsSuained : !creature.IsAsleep || enemyPage.spellsControlCombox.Text != "suain"
            ).ToList();
        }

        private bool CreaturesToIgnore(Creature creature)
        {
            return EnemyPage.ignoreLbox.Items.Contains(creature.SpriteID.ToString());
        }

        private int DistanceFromServerLocation(Creature creature)
        {
            return creature.Location.DistanceFrom(Client._serverLocation);
        }

        private int DistanceFromClientLocation(Creature creature)
        {
            return creature.Location.DistanceFrom(Client._clientLocation);
        }

        private void AoSuain()
        {
            // Check if Ao Suain is enabled and the Suain effect is present before attempting to cast spells.
            if (!Client.ClientTab.aoSuainCbox.Checked || !Client.HasEffect(EffectsBar.Suain))
            {
                return;
            }
            Console.WriteLine("[AoSuain] Attempting to cast 'ao suain' to clear the Suain effect.");
            // Attempt to cast "Leafhopper Chirp" first. If it fails, attempt to cast "ao suain".
            // Only clear the Suain effect if one of the spells is successfully cast.
            if (Client.UseSpell("Leafhopper Chirp", null, false, false) || Client.UseSpell("ao suain", Client.Player, false, true))
            {
                Client.ClearEffect(EffectsBar.Suain);
            }
        }

        private void AutoGem()
        {
            bool shouldUseGem = Client.ClientTab.autoGemCbox.Checked &&
                                Client.Experience > 4250000000U &&
                                DateTime.UtcNow.Subtract(_lastUsedGem).TotalMinutes > 5.0;

            if (!shouldUseGem)
            {
                return;
            }

            // Determine the gem type based on the selected text, then use the gem.
            byte choice = Client.ClientTab.expGemsCombox.Text == "Ascend HP" ? (byte)1 : (byte)2;
            Client.UseExperienceGem(choice);
        }

        private void UpdatePlayersListBasedOnStrangers()
        {
            _playersExistingOver250ms = !IsStrangerNearby() ? _playersExistingOver250ms : _playersExistingOver250ms.Where(Delegates.HasPlayerExistedForOver2s).ToList();
        }

        private void CheckFasSpioradRequirement()
        {
            int requiredMp;

            if (Client.ClientTab.fasSpioradCbox.Checked && int.TryParse(Client.ClientTab.fasSpioradText.Text.Trim(), out requiredMp) && Client.ManaPct < requiredMp)
            {
                _needFasSpiorad = true;
            }
        }

        private void ManageSpellCasting()
        {
            DateTime utcNow = DateTime.UtcNow;
            while (Client._spellHistory.Count >= 3 || Client._spellCounter >= 3)
            {
                if (DateTime.UtcNow.Subtract(utcNow).TotalSeconds > 1.0)
                {
                    Client._spellHistory.Clear();
                }
                Thread.Sleep(10);
            }
            if (DateTime.UtcNow.Subtract(_lastCast).TotalSeconds > 1.0)
            {
                Client._spellHistory.Clear();
            }
        }

        private void ManageSpellCastingDelay()
        {
            var spellCastLines = Client?._currentSpell?.CastLines ?? 0;
            if (spellCastLines <= 0)
            {
                //Thread.Sleep(10); // Adjust the sleep time as needed
            }
        }

        internal bool IsAllyAlreadyListed(string name)
        {
            lock (_lock)
            {
                return _allyListName.Contains(name, StringComparer.CurrentCultureIgnoreCase);
            }
        }

        internal bool IsEnemyAlreadyListed(ushort sprite)
        {

            lock (_lock)
            {
                return _enemyListID.Contains(sprite);
            }
        }

        internal void UpdateAllyList(Ally ally)
        {
            lock (_lock)
            {
                _allyList.Add(ally);
                _allyListName.Add(ally.Name);
            }
        }

        internal void UpdateEnemyList(Enemy enemy)
        {
            lock (_lock)
            {
                _enemyList.Add(enemy);
                _enemyListID.Add(enemy.SpriteID);
            }
        }

        internal void RemoveAlly(string name)
        {
            if (Monitor.TryEnter(_lock, 1000))
            {
                try
                {
                    foreach (Ally ally in _allyList)
                    {
                        if (ally.Name == name)
                        {
                            _allyList.Remove(ally);
                            _allyListName.Remove(ally.Name);
                            break;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        internal List<Ally> ReturnAllyList()
        {
            lock (_lock)
            {
                return new List<Ally>(_allyList);
            }
        }

        internal List<Enemy> ReturnEnemyList()
        {
            lock (_lock)
            {
                return new List<Enemy>(_enemyList);
            }
        }

        internal void ClearEnemyLists(string name)
        {
            if (ushort.TryParse(name, out ushort spriteId))
            {
                List<Enemy> enemiesToRemove = new List<Enemy>();

                foreach (Enemy enemy in _enemyList)
                {
                    if (enemy.SpriteID == spriteId)
                    {
                        enemiesToRemove.Add(enemy);
                    }
                }

                if (enemiesToRemove.Any())
                {
                    if (Monitor.TryEnter(_lock, 1000))
                    {
                        try
                        {
                            foreach (Enemy enemy in enemiesToRemove)
                            {
                                _enemyList.Remove(enemy);
                                _enemyListID.Remove(enemy.SpriteID);
                            }
                        }
                        finally
                        {
                            Monitor.Exit(_lock);
                        }
                    }
                }
            }
        }

        internal bool IsStrangerNearby()
        {
            return _client.GetNearbyPlayerList().Any(player => IsNotInFriendList(player));
        }

        private bool IsNotInFriendList(Player player)
        {
            if (_client.ClientTab != null)
            {
                return !_client.ClientTab.friendList.Items.OfType<string>().Any(friend => string.Equals(friend, player.Name, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return true;//Adam
            }
        }

        private void Loot()
        {
            bool isPickupGoldChecked = Client.ClientTab.pickupGoldCbox.Checked;
            bool isPickupItemsChecked = Client.ClientTab.pickupItemsCbox.Checked;
            bool isDropTrashChecked = Client.ClientTab.dropTrashCbox.Checked;

            if (!isPickupGoldChecked && !isPickupItemsChecked && !isDropTrashChecked)
            {
                return;
            }

            try
            {
                if (!IsStrangerNearby())
                {
                    var lootArea = new Structs.Rectangle(new Point(Client._serverLocation.X - 2, Client._serverLocation.Y - 2), new Point(5, 5));
                    List<Objects.Object> nearbyObjects = Client.GetNearbyObjects(4);

                    if (nearbyObjects.Count > 0)
                    {
                        ProcessLoot(nearbyObjects, lootArea);
                    }

                    HandleTrashItems();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Loot: {ex.Message}");
            }
        }

        private void ProcessLoot(List<Objects.Object> nearbyObjects, Rectangle lootArea)
        {
            bool isPickupGoldChecked = Client.ClientTab.pickupGoldCbox.Checked;
            bool isPickupItemsChecked = Client.ClientTab.pickupItemsCbox.Checked;

            if (isPickupGoldChecked)
            {
                foreach (var obj in nearbyObjects.Where(obj => IsGold(obj, lootArea)))
                {
                    Client.Pickup(0, obj.Location);

                }
            }

            if (isPickupItemsChecked)
            {
                foreach (var obj in nearbyObjects.Where(obj => IsLootableItem(obj, lootArea)))
                {
                    Client.Pickup(0, obj.Location);
                }
            }
        }

        private bool IsGold(Objects.Object obj, Rectangle lootArea)
        {
            return lootArea.ContainsPoint(obj.Location.Point) && obj.SpriteID == 140 && DateTime.UtcNow.Subtract(obj.Creation).TotalSeconds > 2.0;
        }

        private bool IsLootableItem(Objects.Object obj, Rectangle lootArea)
        {
            return lootArea.ContainsPoint(obj.Location.Point) && obj.SpriteID != 140 && DateTime.UtcNow.Subtract(obj.Creation).TotalSeconds > 2.0;
        }

        private void HandleTrashItems()
        {
            if (Client.ClientTab.dropTrashCbox.Checked)
            {
                if (_dropCounter >= 15)
                {
                    foreach (Item item in Client.Inventory.ToList())
                    {
                        if (Client.ClientTab._trashToDrop.Contains(item.Name, StringComparer.CurrentCultureIgnoreCase))
                        {
                            Client.Drop(item.Slot, Client._serverLocation, item.Quantity);
                        }
                    }
                    _dropCounter = 0;
                }
                else
                {
                    _dropCounter++;
                }
            }
        }

        private sealed class Delegates
        {

            static internal bool HasPlayerExistedForOver250ms(Player player)
            {
                return DateTime.UtcNow.Subtract(player.Creation).TotalMilliseconds > 250.0;
            }
            static internal bool PlayerIsSkulled(Player player)
            {
                return player._isSkulled;
            }

            static internal bool HasPlayerExistedForOver2s(Player player)
            {
                return DateTime.UtcNow.Subtract(player.Creation).TotalSeconds > 2.0;
            }

            static internal bool IsPoisoned(Player player)
            {
                return player.IsPoisoned;
            }

            static internal bool isGreenMantis(Creature creature)
            {
                return creature.SpriteID == 87;
            }

            static internal bool NotDioned(Player player)
            {
                return !player.IsDioned;
            }

            static internal int NextRandom(Creature creature)
            {
                return Utility.Random();
            }

            static internal bool CreaturesExisitingLessThan2s(Creature creature)
            {
                return DateTime.UtcNow.Subtract(creature.Creation).TotalSeconds < 2.0;
            }

            static internal Creature IsCreature(Creature creature)
            {
                return creature;
            }

            static internal bool IsNotFassedOrNotCursed(Creature creature)
            {
                return !creature.IsFassed || !creature.IsCursed;
            }

            static internal bool IsNotFassed(Creature creature)
            {
                return !creature.IsFassed;
            }


            static internal bool IsFassed(Creature creature)
            {
                return creature.IsFassed;
            }


            static internal bool IsNotCursed(Creature creature)
            {
                return !creature.IsCursed;
            }


            static internal bool IsCursed(Creature creature)
            {
                return creature.IsCursed;
            }


            static internal bool IsDioned(Creature creature)
            {
                return creature.IsDioned;
            }


            static internal bool CanCastPND(Creature creature)
            {
                return creature._canPND;
            }


            static internal bool IsNotFrozen(Creature creature)
            {
                return !creature.IsFrozen;
            }


            static internal bool IsNotPoisoned(Creature creature)
            {
                return !creature.IsPoisoned;
            }


            static internal bool IsNotDioned(Creature creature)
            {
                return !creature.IsDioned;
            }


            static internal bool IsAsleep(Player player)
            {
                return player.IsAsleep;
            }

            static internal int KVPMatch(KeyValuePair<Creature, int> kvp)
            {
                return kvp.Value;
            }


            static internal DateTime KVPCreation(KeyValuePair<Creature, int> kvp)
            {
                return kvp.Key.Creation;
            }

            static internal bool CastRegeneration(Spell spell)
            {
                return spell.Name != "Increased Regeneration" && spell.Name.Contains("Regeneration");
            }


            static internal bool CreaturesExistingOver250ms(Creature creature)
            {
                return DateTime.UtcNow.Subtract(creature.Creation).TotalMilliseconds > 250.0;
            }


            static internal DateTime CreatureCreationDate(Creature creature)
            {
                return creature.Creation;
            }


            static internal bool CastArrowShot(string spellName)
            {
                return spellName.Contains("Arrow Shot");
            }


            static internal bool CreatureNotHiddenOrPoisoned(Creature creature)
            {
                Player player = creature as Player;
                if (player != null)
                {
                    if (player._isHidden)
                    {
                        return false;
                    }
                }
                return !creature.IsPoisoned;
            }

            static internal bool NeedsRegeneration(Creature creature)
            {
                return !creature.SpellAnimationHistory.ContainsKey(187) || DateTime.UtcNow.Subtract(creature.SpellAnimationHistory[187]).TotalSeconds > 4.0;
            }


            static internal bool NeedsCounterAttack(Creature creature)
            {
                return creature is Player && (!creature.SpellAnimationHistory.ContainsKey(184) || DateTime.UtcNow.Subtract(creature.SpellAnimationHistory[184]).TotalSeconds > 20.0);
            }



            static internal bool SpellNotNull(Spell spell)
            {
                return spell != null;
            }

            public static readonly Bot.Delegates class145 = new Bot.Delegates();


        }

    }


}
