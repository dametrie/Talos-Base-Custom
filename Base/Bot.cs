using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Talos.Definitions;
using Talos.Enumerations;
using Talos.Extensions;
using Talos.Forms;
using Talos.Forms.UI;
using Talos.Helper;
using Talos.Maps;
using Talos.Objects;
using Talos.Properties;
using Talos.Structs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        internal bool _rangerNear = false;
        internal bool _shouldAlertItemCap;
        internal bool _recentlyAoSithed;
        internal bool[] itemDurabilityAlerts = new bool[5];

        internal bool _dontWalk;
        internal bool _dontCast;
        internal bool _dontBash;
        private bool bool_32;
        private int? _leaderID;
        internal bool _hasRescue;

        internal byte _fowlCount;
        internal int currentWay;

        internal DateTime _lastEXP = DateTime.MinValue;
        internal DateTime _lastDisenchanterCast = DateTime.MinValue;
        internal DateTime _lastGrimeScentCast = DateTime.MinValue;
        internal DateTime _skullTime = DateTime.MinValue;
        internal DateTime _lastRefresh = DateTime.MinValue;
        internal DateTime _lastVineCast = DateTime.MinValue;
        internal DateTime _botChecks = DateTime.MinValue;
        internal DateTime _lastExpBonusAppliedTime = DateTime.MinValue;
        internal DateTime _spellTimer = DateTime.MinValue;
        internal DateTime _lastUsedGem = DateTime.MinValue;
        private DateTime _lastUsedFungusBeetle = DateTime.MinValue;
        private DateTime _lastUsedBeetleAid = DateTime.MinValue;
        internal TimeSpan _expBonusElapsedTime = TimeSpan.Zero;

        internal List<Ally> _allyList = new List<Ally>();
        internal List<Enemy> _enemyList = new List<Enemy>();
        internal List<Player> _playersExistingOver250ms = new List<Player>();
        internal List<Player> _playersNeedingRed = new List<Player>();
        internal List<Player> _nearbyAllies = new List<Player>();
        internal List<Creature> _nearbyValidCreatures = new List<Creature>();
        internal List<Location> ways = new List<Location>();

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
        internal DateTime _doorTime;
        internal Point _doorPoint;
        private bool _together;
        private DateTime _followerTimer;
        internal DateTime _lastMushroomBonusAppliedTime;
        internal object _mushroomBonusElapsedTime;
        internal bool _netRepair = false;
        internal DateTime _hammerTimer = DateTime.MinValue;

        public bool RecentlyUsedGlowingStone { get; set; } = false;
        public bool RecentlyUsedDragonScale { get; set; } = false;
        public bool RecentlyUsedFungusExtract { get; set; } = false;
        internal AllyPage AllyPage { get; set; }
        internal EnemyPage AllMonsters { get; set; }


        internal Bot(Client client, Server server) : base(client, server)
        {
            _client = client;
            _server = server;
            AddTask(new BotLoop(BotLoop));
            AddTask(new BotLoop(SoundLoop));
            AddTask(new BotLoop(WalkLoop));
            AddTask(new BotLoop(Misc));
        }

        private void Misc()
        {
            while (!_shouldThreadStop)
            {
                //Console.WriteLine("[MiscLoop] Pulse");
                //TavalyWallHacks(); //Adam Fix
                MonsterForm();

                Thread.Sleep(500); // Add a small sleep to avoid flooding the CPU default: 100
            }

        }

        private void TavalyWallHacks()
        {
            //Adam Fix
            if (Client.ClientTab.chkTavWallStranger.Checked && IsStrangerNearby() && Client.ClientTab.chkTavWallHacks.Checked && !Client._map.IsWall(Client._serverLocation))
            {
                Client.ClientTab.chkTavWallHacks.Checked = false;
                Client.RequestRefresh();
            }
            if (Client.ClientTab.chkTavWallStranger.Checked && !IsStrangerNearby() && !Client.ClientTab.chkTavWallHacks.Checked)
            {
                Client.ClientTab.chkTavWallHacks.Checked = true;
                Client.RequestRefresh();
            }
        }

        private void MonsterForm()
        {
            if (Client != null && Client.ClientTab != null)
            {
                bool strangerNear = IsStrangerNearby();
                bool deformChecked = Client.ClientTab.deformCbox.Checked;
                ushort desiredFormNum = (ushort)Client.ClientTab.formNum.Value;

                if (Client.ClientTab.formCbox.Checked)
                {
                    if (deformChecked && strangerNear)
                    {
                        // Only set if the state needs to change
                        if (Client.ClientTab.formCbox.Checked)
                        {
                            Client.ClientTab.SetMonsterForm(false, desiredFormNum);
                        }
                    }
                }
                else if (!Client.ClientTab.formCbox.Checked && deformChecked)
                {
                    if (!strangerNear)
                    {


                        Client.ClientTab.SetMonsterForm(true, desiredFormNum);
                    }
                }
            }

        }



        internal bool IsStrangerNearby()
        {
            return _client.GetNearbyPlayers().Any(player => IsNotInFriendList(player));
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

        private void WalkLoop()
        {

            while (!_shouldThreadStop)
            {
                //Console.WriteLine("[WalkLoop] Pulse");
                _rangerNear = IsRangerNearBy();
                if (!Client._exchangeOpen && Client.ClientTab != null)
                {
                    HandleDialog();
                    HandleDumbMTGWarp();
                    WalkActions();
                }

                Thread.Sleep(100); // Add a small sleep to avoid flooding the CPU default: 100
            }

        }

        private void WalkActions()
        {
            //var start = DateTime.UtcNow;
            //Console.WriteLine($"WalkActions started at {start:HH:mm:ss.fff}");

            _nearbyPlayers = Client.GetNearbyPlayers();
            _nearbyValidCreatures = Client.GetNearbyValidCreatures(12);
            var shouldWalk = !_dontWalk &&
                (!Client.ClientTab.rangerStopCbox.Checked || !_rangerNear);

            if (shouldWalk)
            {
                HandleWalkingCommand();
            }

            //var end = DateTime.UtcNow;
            //Console.WriteLine($"WalkActions ended at {end:HH:mm:ss.fff}, Duration: {(end - start).TotalMilliseconds} ms");
        }

        private void HandleWalkingCommand()
        {
            //var start = DateTime.UtcNow;
            //Console.WriteLine($"HandleWalkingCommand started at {start:HH:mm:ss.fff}");

            string comboBoxText = Client.ClientTab.walkMapCombox.Text;
            bool followChecked = Client.ClientTab.followCbox.Checked;
            string followName = Client.ClientTab.followText.Text;

            if (followChecked && !string.IsNullOrEmpty(followName))
            {
                FollowWalking(followName);
            }
            else if (Client.ClientTab.walkBtn.Text == "Stop")
            {
                RefreshLastStep();

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

            //var end = DateTime.UtcNow;
            //Console.WriteLine($"HandleWalkingCommand ended at {end:HH:mm:ss.fff}, Duration: {(end - start).TotalMilliseconds} ms");

        }


        private void FollowWalking(string playerName)
        {
            try
            {
                // Check if follow is enabled and player name is provided
                if (!Client.ClientTab.followCbox.Checked || string.IsNullOrEmpty(Client.ClientTab.followText.Text))
                    return;

                // Try to identify the leader (bot or player)
                Client botClient = _server.GetClient(playerName);
                Player leader = botClient?.Player ?? Client.WorldObjects.Values.OfType<Player>()
                    .FirstOrDefault(p => p.Name.Equals(playerName, StringComparison.CurrentCultureIgnoreCase));

                if (_client != null && (_client.ClientTab._isBashing || !_client._stopped))
                {
                    RefreshLastStep();
                }


                if (leader == null)
                {
                    // If no visible leader, check the last seen location
                    if (_leaderID.HasValue && Client.LastSeenLocations.TryGetValue(_leaderID.Value, out Location lastSeenLocation))
                    {
                        // If we have the last seen location, use it
                        Console.WriteLine($"Using last seen location for player {playerName}: {lastSeenLocation}");
                        Client._isWalking = Client.RouteFind(lastSeenLocation, 0, true, true)
                                            && !Client.ClientTab.oneLineWalkCbox.Checked
                                            && !Server._toggleWalk;
                    }
                    else
                    {
                        Console.WriteLine("No known location for player.");
                        Client._isWalking = false;
                        return;
                    }
                }
                else
                {
                    // We have a visible leader, proceed with following logic
                    _leaderID = leader.ID;
                    Location leaderLocation = leader.Location;
                    int distance = leaderLocation.DistanceFrom(Client._clientLocation);

                    // UnStucker logic if necessary
                    if (!UnStucker(leader))
                    {
                        // Determine follow distance
                        short followDistance = (leaderLocation.MapID == Client._map.MapID)
                            ? (short)Client.ClientTab.followDistanceNum.Value
                            : (short)0;

                        // Apply lockstep logic
                        if (Client.ClientTab.lockstepCbox.Checked && leaderLocation.MapID == Client._map.MapID)
                        {
                            if (distance > followDistance)
                            {
                                //Console.WriteLine($"[Lockstep Mode] Distance ({distance}) exceeds follow distance ({followDistance}). Initiating route find.");
                                Client._confirmBubble = false;
                                Client._isWalking = Client.RouteFind(leaderLocation, followDistance, true, true)
                                                    && !Client.ClientTab.oneLineWalkCbox.Checked
                                                    && !Server._toggleWalk;
                            }
                        }
                        else if (distance > followDistance)
                        {
                            //Console.WriteLine($"[Non-Lockstep Mode] Distance ({distance}) exceeds follow distance ({followDistance}). Recalculating path.");
                            Client._confirmBubble = false;
                            Client._isWalking = Client.RouteFind(leaderLocation, followDistance, true, true)
                                                && !Client.ClientTab.oneLineWalkCbox.Checked
                                                && !Server._toggleWalk;
                        }
                        else
                        {
                            Client._isWalking = false;
                        }

                        // Apply bubble logic for synchronization
                        if (Client._okToBubble
                            && DateTime.UtcNow.Subtract(Client.LastStep).TotalMilliseconds > 500.0
                            && DateTime.UtcNow.Subtract(Client.LastMoved).TotalMilliseconds > 500.0)
                        {
                            //Console.WriteLine("Bubble conditions met, checking for refresh.");
                            if (Client._serverLocation != Client._clientLocation)
                            {
                                Console.WriteLine("Client position differs from server, requesting refresh.");
                                Client._confirmBubble = false;
                                Client.RequestRefresh(true);
                            }
                            else if (Client._map.Name.Contains("Lost Ruins") || Client._map.Name.Contains("Assassin Dungeon")
                                     || _nearbyValidCreatures.Any(c => Client._serverLocation.DistanceFrom(c.Location) <= 6))
                            {
                                //Console.WriteLine("Bubble confirmed for specific maps or valid creatures nearby.");
                                Client._confirmBubble = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Client._isWalking = false;
                Console.WriteLine(ex.Message);
            }
        }


        private bool UnStucker(Player leader)
        {
            if (Server._toggleWalk)
            {
                return false;
            }

            if (DateTime.UtcNow.Subtract(leader.GetState<DateTime>(CreatureState.LastStep)).TotalSeconds > 5.0
                && leader.Location.MapID == Client._map.MapID
                && (DateTime.UtcNow.Subtract(_lastEXP).TotalSeconds > 5.0
                || DateTime.UtcNow.Subtract(_doorTime).TotalSeconds < 10.0))
            {

                // Get nearby object points (creatures with type Merchant or Aisling)
                HashSet<Location> objectPoints = (from c in Client.GetNearbyObjects().OfType<Creature>()
                                                  where c != null &&
                                                        c.Type == CreatureType.Merchant ||
                                                        c.Type == CreatureType.Aisling
                                                  select c.Location).ToHashSet<Location>();

                // Get warp points
                List<Location> warps = Client.GetAllWarpPoints(leader.Location);

                // Convert kvp.Key (Point) to Location using the leader's MapID
                List<Location> list = (from kvp in Server._maps[leader.Location.MapID].Tiles
                                       where !kvp.Value.IsWall
                                             && !warps.Contains(new Location(leader.Location.MapID, kvp.Key)) // Convert Point to Location
                                             && !objectPoints.Contains(new Location(leader.Location.MapID, kvp.Key)) // Convert Point to Location
                                             && kvp.Key != leader.Location.Point
                                       select new Location(leader.Location.MapID, kvp.Key)).ToList();

                // Determine the flood fill threshold
                int val = list.Count / 100;
                int num = Math.Max(5, Math.Min(val, 25));

                // Perform flood fill and check if the result meets the criteria
                bool flag = list.FloodFill(leader.Location).Take(26).Count() <= num;

                if (flag)
                {
                    // Select a random location from the list
                    Random random = new Random();
                    Location location = list.OrderBy(_ => random.Next()).FirstOrDefault();

                    // Perform the route find operation
                    if (Client.RouteFind(location, 0, true, true))
                    {
                        Thread.Sleep(1500);
                    }
                }

                return flag;
            }

            return false;
        }

        private void BasherWalking(Client client)
        {
            bool? isBashing = client?.ClientTab?._isBashing;

            if (isBashing.GetValueOrDefault()
                && !_rangerNear
                && !Client.HasEffect(EffectsBar.BeagSuain)
                && !Client.HasEffect(EffectsBar.Pramh)
                && !Client.HasEffect(EffectsBar.Suain)
                && Client.GetAllNearbyMonsters(0).Any<Creature>())
            {
                Direction direction = Utility.RandomEnumValue<Direction>();
                Client.Walk(direction);
                Thread.Sleep(300);
                Client.RequestRefresh(false);
            }
        }

        private void MoveToNearbyLocation()
        {
            // Move to nearby location logic
            Location serverLoc = Client._serverLocation;
            List<Location> potentialLocations = new List<Location>
            {
                new Location(serverLoc.MapID, serverLoc.X, (short)(serverLoc.Y - 1)),
                new Location(serverLoc.MapID, (short)(serverLoc.X + 1), serverLoc.Y),
                new Location(serverLoc.MapID, serverLoc.X, (short)(serverLoc.Y + 1)),
                new Location(serverLoc.MapID, (short)(serverLoc.X - 1), serverLoc.Y)
            };

            List<Creature> nearbyCreatures = Client.GetNearbyObjects().OfType<Creature>().ToList();

            foreach (var location in potentialLocations)
            {
                if (!Client._map.IsWall(location)
                    && nearbyCreatures.All(c => c.Location != location)
                    && Client.RouteFind(location, 0, true, true))
                {
                    break;
                }
            }

            Thread.Sleep(2500);
        }

        private void RefreshLastStep()
        {
            bool lastStepF5 = Client.ClientTab.chkLastStepF5.Checked;
            bool exceededStepTime = DateTime.UtcNow.Subtract(Client.LastStep).TotalSeconds > (double)Client.ClientTab.numLastStepTime.Value;

            if (lastStepF5 && exceededStepTime)
            {
                Client.RequestRefresh(true);
            }
        }


        //Added helper methods because was running into an issue where checkbox state wasn't
        //being assessed correctly. Assumed it was a UI thread problem.
        private bool GetCheckBoxChecked(CheckBox checkBox)
        {
            if (checkBox.InvokeRequired)
            {
                return (bool)checkBox.Invoke(new Func<bool>(() => checkBox.Checked));
            }
            else
            {
                return checkBox.Checked;
            }
        }

        private decimal GetNumericUpDownValue(NumericUpDown numericUpDown)
        {
            if (numericUpDown.InvokeRequired)
            {
                return (decimal)numericUpDown.Invoke(new Func<decimal>(() => numericUpDown.Value));
            }
            else
            {
                return numericUpDown.Value;
            }
        }

        private void WayPointWalking(bool skip = false)
        {
            try
            {
                // Check if there are any waypoints to walk to
                if (ways.Count == 0 ||
                    (_server.ClientStateList.ContainsKey(Client.Name)
                    && _server.ClientStateList[Client.Name] == CharacterState.WaitForSpells))
                {
                    Console.WriteLine("No waypoints available or client in WaitForSpells state.");
                    return;
                }

                if (currentWay < ways.Count)
                {

                    Client._walkSpeed = (double)Client.ClientTab.walkSpeedSldr.Value;


                    if (skip && Client.GetNearbyObjects().OfType<Creature>()
                        .Any(creature => creature.Type != CreatureType.WalkThrough && creature.Location.Point == ways[currentWay].Point))
                    {
                        Console.WriteLine($"Skipping waypoint {currentWay} due to nearby creature.");
                        currentWay++;
                    }
                    else
                    {
                        WayForm waysForm = Client.ClientTab._wayForm;

                        // Special door proximity condition
                        if (DateTime.UtcNow.Subtract(_doorTime).TotalSeconds < 2.5 &&
                            Client._clientLocation.Point.Distance(_doorPoint) < 6)
                        {
                            Console.WriteLine("Near door, adjusting walking speed.");
                            Client._walkSpeed = Client._walkSpeed > 350.0 ? 350.0 : Client._walkSpeed;
                        }
                        else
                        {

                            List<Creature> nearbyCreatures = Client.GetNearbyValidCreatures(12);

                            // Filter out creatures that are walled in and cannot be reached if selected
                            //if (Client.ClientTab._isBashing && Client.ClientTab.ignoreWalledInCbox.Checked)
                            //{
                            //    // Remove creatures that are walled in
                            //    nearbyCreatures = nearbyCreatures
                            //        .Where(creature => !Client.IsLocationSurrounded(creature.Location))
                            //        .ToList();
                            //    Console.WriteLine("Filtered out walled-in creatures while bashing.");
                            //}

                            // Filter out creatures that are dioned if selected
                            if (Client.ClientTab.chkIgnoreDionWaypoints.Checked)
                            {
                                // Remove creatures that are dioned
                                nearbyCreatures = nearbyCreatures
                                    .Where(creature => !creature.IsDioned)
                                    .ToList();
                                Console.WriteLine("Filtered out dioned creatures");
                            }


                            // Condition 1
                            if (GetCheckBoxChecked(waysForm.condition1)
                                && nearbyCreatures.Count(c => Client.WithinRange(c, (int)GetNumericUpDownValue(waysForm.proximityUpDwn1)))
                                    >= GetNumericUpDownValue(waysForm.mobSizeUpDwn1)
                                && !Client.ClientTab._isBashing)
                            {
                                Client._walkSpeed = (double)GetNumericUpDownValue(waysForm.walkSlowUpDwn1);
                                //Console.WriteLine("Condition 1 met, adjusting walking speed.");
                            }

                            // Condition 2
                            if (GetCheckBoxChecked(waysForm.condition2)
                                && nearbyCreatures.Count(c => Client.WithinRange(c, (int)GetNumericUpDownValue(waysForm.proximityUpDwn2)))
                                    >= GetNumericUpDownValue(waysForm.mobSizeUpDwn2)
                                && !Client.ClientTab._isBashing)
                            {
                                Client._walkSpeed = (double)GetNumericUpDownValue(waysForm.walkSlowUpDwn2);
                                //Console.WriteLine("Condition 2 met, adjusting walking speed.");
                            }

                            // Condition 3
                            if (GetCheckBoxChecked(waysForm.condition3)
                                && nearbyCreatures.Count(c => Client.WithinRange(c, (int)GetNumericUpDownValue(waysForm.proximityUpDwn3)))
                                    >= GetNumericUpDownValue(waysForm.mobSizeUpDwn3)
                                && !Client.ClientTab._isBashing)
                            {
                                Client._walkSpeed = (double)GetNumericUpDownValue(waysForm.walkSlowUpDwn3);
                                //Console.WriteLine("Condition 3 met, adjusting walking speed.");
                            }

                            // Condition 4
                            if (GetCheckBoxChecked(waysForm.condition4)
                                && nearbyCreatures.Count(c => Client.WithinRange(c, (int)GetNumericUpDownValue(waysForm.proximityUpDwn4)))
                                    >= GetNumericUpDownValue(waysForm.mobSizeUpDwn4))
                            {
                                //Console.WriteLine("Condition 4 met, stopping movement and checking bubble conditions.");
                                Client._stopped = true;

                                if (BackTracking())
                                {
                                    return;
                                }

                                if (Client._map.Name.Contains("Lost Ruins") || Client._map.Name.Contains("Assassin Dungeon")
                                    || _nearbyValidCreatures.Any(monster => monster.Location.DistanceFrom(Client._serverLocation) <= 6))
                                {
                                    Client._okToBubble = true;
                                }

                                // Also apply bubble to the bot's follow chain
                                foreach (Client client in Server.GetFollowChain(Client))
                                {
                                    if (!client._okToBubble)
                                    {
                                        client._okToBubble = true;
                                    }
                                }

                                Client._isWalking = false;
                                return;
                            }
                        }

                        // Reset stop status if none of the conditions applied
                        Client._stopped = false;
                        foreach (Client client in Server.GetFollowChain(Client))
                        {
                            client._okToBubble = false;
                        }
                        Client._okToBubble = false;

                        // Check if the bot needs to backtrack
                        if (BackTracking())
                        {
                            Console.WriteLine("Backtracking logic triggered, stopping waypoint navigation.");
                            return;
                        }

                        // Handle specific status effects and conditions on followers
                        foreach (Client client in Server.GetFollowChain(Client))
                        {
                            if (client.HasEffect(EffectsBar.Pramh) || client.HasEffect(EffectsBar.Suain)
                                || client.HasEffect(EffectsBar.BeagSuain) || client.HasEffect(EffectsBar.Skull)
                                || client.Player.IsSkulled)
                            {
                                Console.WriteLine("Cannot move due to effect, stopping waypoint navigation.");
                                return;
                            }
                        }


                        // Handle item pickup logic if configured
                        if (Client.ClientTab.toggleOverrideCbox.Checked && Client.ClientTab.overrideList.Items.Count > 0)
                        {
                            try
                            {
                                List<ushort> itemsToPickUp = new List<ushort>();

                                foreach (string item in Client.ClientTab.overrideList.Items.OfType<string>())
                                {
                                    if (ushort.TryParse(item, out ushort itemID))
                                    {
                                        itemsToPickUp.Add(itemID);
                                    }
                                }

                                if (itemsToPickUp.Count != 1 && itemsToPickUp[0] != (ushort)140 && !Client._inventoryFull)
                                    return;


                                List<GroundItem> nearbyGroundItems = Client.GetNearbyGroundItems(12, itemsToPickUp.ToArray());

                                foreach (GroundItem groundItem in nearbyGroundItems)
                                {
                                    int count = Client.Pathfinder.FindPath(Client._clientLocation, groundItem.Location).Count;

                                    if (count == 0)
                                    {
                                        count = Client.Pathfinder.FindPath(Client._clientLocation, groundItem.Location).Count;
                                    }

                                    if (count == 0 || count > Client.ClientTab.overrideDistanceNum.Value)
                                    {
                                        nearbyGroundItems.Remove(groundItem);
                                    }
                                }

                                if (nearbyGroundItems.Any())
                                {
                                    GroundItem closestItem = nearbyGroundItems.OrderBy(item => item.Location.DistanceFrom(Client._clientLocation)).FirstOrDefault();
                                    if (closestItem != null && Client._clientLocation.DistanceFrom(closestItem.Location) > 2)
                                    {
                                        // Move to the item
                                        Client._isWalking = Client.Pathfind(closestItem.Location, 2)
                                                                && !Client.ClientTab.oneLineWalkCbox.Checked
                                                                && !Server._toggleWalk;
                                        return;
                                    }

                                    if (Client._clientLocation.DistanceFrom(closestItem.Location) <= 2 && Client._serverLocation.DistanceFrom(closestItem.Location) > 2)
                                    {
                                        Client.RequestRefresh(true);
                                    }

                                    // Pick up the item if nearby
                                    if (Monitor.TryEnter(Client.CastLock, 200))
                                    {
                                        try
                                        {
                                            Client.Pickup((byte)0, closestItem.Location);
                                            Console.WriteLine("Picked up item.");
                                        }
                                        finally
                                        {
                                            Monitor.Exit(Client.CastLock);
                                        }
                                    }
                                    Client._isWalking = false;
                                    return;
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error during item pickup: {ex.Message}");
                                Client._isWalking = false;
                            }
                        }

                        Point currentPoint = this.Client._serverLocation.Point;

                        if (this.Client.ClientTab._isBashing)
                        {
                            Location currentWay = this.ways[this.currentWay];
                            Location nextWay = this.currentWay < this.ways.Count - 1 ? this.ways[this.currentWay + 1] : this.ways[0];

                            int distanceToCurrentWay = currentPoint.Distance(currentWay.Point);
                            int distanceToNextWay = currentWay.Point.Distance(nextWay.Point);
                            Direction currentDirection = currentPoint.Relation(currentWay.Point);
                            Direction nextDirection = nextWay.Point.Relation(currentWay.Point);

                            if (currentWay.MapID == this.Client._map.MapID && currentWay.MapID == nextWay.MapID && distanceToCurrentWay > 3 && distanceToCurrentWay < distanceToNextWay && currentDirection == nextDirection)
                            {
                                this.currentWay++;
                                return;
                            }
                        }

                        Location targetWay = this.ways[this.currentWay];
                        int distanceToTarget = this.Client._clientLocation.Point.Distance(targetWay.Point);

                        if (distanceToTarget > waysForm.distanceUpDwn.Value)
                        {
                            if (this.Client._map.MapID == targetWay.MapID)
                            {
                                this.Client._isWalking = this.Client.RouteFind(targetWay, (short)waysForm.distanceUpDwn.Value) && !this.Client.ClientTab.oneLineWalkCbox.Checked && !this.Server._toggleWalk;
                            }
                        }
                        else
                        {
                            this.Client._isWalking = false;

                            if (this.Client._clientLocation.Point.Distance(targetWay.Point) <= waysForm.distanceUpDwn.Value)
                            {
                                if (this.Client._map.MapID == targetWay.MapID)
                                {
                                    if (this.Client._serverLocation.Point.Distance(targetWay.Point) > waysForm.distanceUpDwn.Value)
                                    {
                                        this.Client.RequestRefresh();
                                    }
                                    else
                                    {
                                        this.currentWay++;
                                    }
                                }
                            }
                        }
                    }

                }
                else
                    currentWay = 0;
            }
            catch (ThreadAbortException ex)
            {

            }
            catch (Exception ex)
            {
                string str = AppDomain.CurrentDomain.BaseDirectory + "PathfinderCrashLogs\\";
                if (!Directory.Exists(str))
                    Directory.CreateDirectory(str);
                File.WriteAllText(str + DateTime.Now.ToString("MM-dd-HH-yyyy h mm tt") + ".log", ex.ToString());
            }



        }

        private bool BackTracking()
        {
            try
            {
                // Get list of clients that are following this client and have their 'startStrip' set to 'Stop'
                var clientList = Server.Clients
                    .Where(x => x?.ClientTab?.startStrip.Text == "Stop"
                                && x.ClientTab.followCbox.Checked
                                && x.ClientTab.followText.Text.Equals(Client.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                bool wasTogether = _together;
                _together = true;
                Client clientToFollow = null;

                // Check if any client is on a different map or too far away
                foreach (var client in clientList)
                {
                    if (client._map.MapID != Client._map.MapID || client._serverLocation.DistanceFrom(Client._serverLocation) > 9)
                    {
                        clientToFollow = client;
                        _together = false;
                        break;
                    }
                }

                // If the clients were together and are now apart, start the timer
                if (wasTogether && !_together)
                {
                    this._followerTimer = DateTime.UtcNow;
                }
                // If the clients have been apart for more than 5 seconds, backtrack to the client
                else if (!_together && DateTime.UtcNow.Subtract(this._followerTimer).TotalSeconds > 5.0)
                {
                    Client._walkSpeed = (double)Client.ClientTab.walkSpeedSldr.Value;
                    if (clientToFollow != null)
                    {
                        Client._isWalking = Client.RouteFind(clientToFollow._serverLocation, 3, shouldBlock: false);
                    }
                    return true;
                }

                return false;
            }
            catch
            {
                _together = true;
                return false;
            }
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
                    Client.Pathfind(new Location(3938, 7, 13), 0, true, false);
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
                    Client.Pathfind(new Location(3920, 13, 12), 0, true, false);
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
                    Client.Pathfind(new Location(3012, 15, 0), 0, true, false);
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
                    Client.Pathfind(new Location(10265, 93, 48), 0, true, false);
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
                    Client.Pathfind(new Location(424, 6, 6), 0, true, false);
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
                    Client.Pathfind(new Location(6530, 10, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537 && Client._clientLocation.X < 31 && Client._clientLocation.Y < 43)
                {
                    Client.Pathfind(new Location(6537, 0, 31), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6535 && Client._clientLocation.X > 68 && Client._clientLocation.Y < 56)
                {
                    Client.Pathfind(new Location(6535, 74, 49), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537 && Client._clientLocation.X < 43 && Client._clientLocation.Y > 43)
                {
                    Client.Pathfind(new Location(6537, 14, 74), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6536 && Client._clientLocation.X < 25 && Client._clientLocation.Y < 24)
                {
                    Client.Pathfind(new Location(6536, 0, 15), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6534 && Client._clientLocation.X > 45)
                {
                    Client.Pathfind(new Location(6534, 74, 28), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6536 && (Client._clientLocation.X > 25 || Client._clientLocation.Y > 26) && Client._clientLocation.X < 69)
                {
                    Client.Pathfind(new Location(6536, 72, 4), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6536 && Client._clientLocation.X > 68)
                {
                    Client.Pathfind(new Location(6536, 69, 0), 0, true, false);
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
                    Client.Pathfind(new Location(6530, 10, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537)
                {
                    Client.Pathfind(new Location(6537, 0, 4), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6539)
                {
                    Client.Pathfind(new Location(6539, 53, 74), 0, true, true);
                    return;
                }
                if (Client._map.MapID == 6538)
                {
                    Client.Pathfind(new Location(6538, 74, 16), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6540 && Client._clientLocation.X < 4)
                {
                    Client.Pathfind(new Location(6540, 3, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6541 && Client._clientLocation.X < 28 && Client._clientLocation.Y > 63)
                {
                    Client.Pathfind(new Location(6541, 23, 74), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6540)
                {
                    Client.Pathfind(new Location(6540, 35, 0), 0, true, false);
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
                    Client.Pathfind(new Location(6530, 10, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537)
                {
                    Client.Pathfind(new Location(6537, 0, 4), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6539 && Client._clientLocation.Y < 8)
                {
                    Client.Pathfind(new Location(6539, 1, 8), 1, true, false);
                    return;
                }
                if (Client._map.MapID == 6539)
                {
                    Client.Pathfind(new Location(6539, 4, 74), 0, true, true);
                    return;
                }
                if (Client._map.MapID == 6538 && Client._clientLocation.Y < 50)
                {
                    Client.Pathfind(new Location(6538, 74, 47), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6540)
                {
                    Client.Pathfind(new Location(6540, 0, 67), 0, true, false);
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
                    Client.Pathfind(new Location(6530, 10, 0), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6537)
                {
                    Client.Pathfind(new Location(6537, 0, 4), 0, true, false);
                    return;
                }
                if (Client._map.MapID == 6535)
                {
                    Client.Pathfind(new Location(6535, 20, 74), 0, true, false);
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
        private void SoundLoop()
        {
            if (Client.ClientTab == null || Client == null)
            {
                return;
            }
            while (!_shouldThreadStop)  // Assuming _shouldStop is a volatile bool that is set to true when you want to stop all threads
            {
                //Console.WriteLine("[SoundLoop] Pulse");
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
            try
            {
                while (!_shouldThreadStop)
                {

                    //Console.WriteLine("[BotLoop] Pulse - _shouldThreadStop: " + _shouldThreadStop);

                    try
                    {
                        if (Client.InArena)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                        if (currentAction == null)
                        {
                            currentAction = Client.ClientTab.currentAction;
                        }

                        _rangerNear = IsRangerNearBy();

                        if (CheckForStopConditions())
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        ProcessPlayers();

                        if (Client.CurrentHP <= 1U && Client.IsSkulled)
                        {
                            //Console.WriteLine("[BotLoop] Client HP <= 1 and is skulled, handling skull status");
                            HandleSkullStatus();
                            continue;
                        }

                        if (ShouldRequestRefresh())
                        {
                            Client.RequestRefresh(false);
                            _lastRefresh = DateTime.UtcNow;
                            continue;
                        }

                        if (AutoRedConditionsMet())
                        {
                            //Console.WriteLine("[BotLoop] AutoRed conditions met");
                            if (GetSkulledPlayers().Count > 0)
                            {
                                Console.WriteLine("[BotLoop] Skulledplayers > 0, calling RedSkulledPlayers");
                                RedSkulledPlayers();
                            }
                        }

                        if (IsStrangerNearby())
                        {
                            FilterStrangerPlayers();
                        }

/*                        double botCheckSeconds = DateTime.UtcNow.Subtract(_botChecks).TotalSeconds;

                        if (botCheckSeconds < 2.5)
                        {
                            continue;
                        }*/

                        PerformActions();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[BotLoop] Exception caught in inner try: {ex.Message}");
                    }
                }

                //Console.WriteLine("Exiting BotLoop, _shouldThreadStop: " + _shouldThreadStop);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[BotLoop] Exception caught in outer try: {ex.Message}");
            }
        }

        internal bool IsRangerNearBy()
        {
            if (!Settings.Default.paranoiaMode)
            {
                return Client.GetNearbyPlayers().Any(new Func<Player, bool>(RangerListContains));
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
                   Client._exchangeOpen || Client.ClientTab == null || Client.Dialog != null || _dontCast;
        }
        private void ProcessPlayers()
        {
            _nearbyAllies = Client.GetNearbyAllies();
            _nearbyValidCreatures = Client.GetNearbyValidCreatures(11);
            var nearbyPlayers = Client.GetNearbyPlayers();
            _playersExistingOver250ms = nearbyPlayers?
                .Where(p => (DateTime.UtcNow - p.Creation).TotalMilliseconds > 250)
                .ToList();
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
                   DateTime.UtcNow.Subtract(_lastEXP).TotalSeconds > 20.0 &&
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
            if (_playersExistingOver250ms.Any(p => p.IsSkulled))
            {
                _playersExistingOver250ms.RemoveAll(p => p.IsSkulled);
                foreach (Player player in Client.GetNearbyPlayers().Where(IsSkulledFriendOrGroupMember))
                {
                    _playersNeedingRed.Add(player);
                }
                return _playersNeedingRed = _playersNeedingRed.OrderBy(DistanceFromPlayer).ToList();
            }
            return new List<Player>();
        }

        private bool RedSkulledPlayers()
        {
            Console.WriteLine("[BotLoop] RedSkulledPlayers called");

            if (_playersNeedingRed.Count > 0 && Client.ClientTab.autoRedCbox.Checked)
            {
                Console.WriteLine("[BotLoop] Players needing red > 0 and autoRed checked");

                Player player = _playersNeedingRed[0];
                Console.WriteLine($"[BotLoop] Target player: {player?.Name}, HealthPercent: {player?.HealthPercent}");

                var inventory = Client.Inventory;
                bool canUseBeetleAid = inventory.HasItem("Beetle Aid") && Client._isRegistered &&
                                       DateTime.UtcNow.Subtract(_lastUsedBeetleAid).TotalMinutes > 2.0;
                bool canUseOtherItems = inventory.HasItem("Komadium") || inventory.HasItem("beothaich deum");

                Console.WriteLine($"[BotLoop] Can use Beetle Aid: {canUseBeetleAid}, Can use other items: {canUseOtherItems}");

                if (canUseBeetleAid || canUseOtherItems)
                {
                    _dontWalk = true;
                    Direction direction = player.Location.Point.Relation(Client._serverLocation.Point);
                    Console.WriteLine($"[BotLoop] Calculated direction to player: {direction}");

                    if (Client._serverLocation.DistanceFrom(player.Location) > 1)
                    {
                        Console.WriteLine("[BotLoop] Server distance from player > 1");

                        if (Client._clientLocation.DistanceFrom(player.Location) == 1)
                        {
                            Console.WriteLine("[BotLoop] Client distance from player = 1, requesting refresh");
                            Client.RequestRefresh(true);
                        }

                        Console.WriteLine("[BotLoop] Pathfinding to player location");
                        Client.Pathfind(player.Location, 1, true, true);
                    }
                    else if (direction != Client._serverDirection)
                    {
                        Console.WriteLine("[BotLoop] Turning to face player");
                        Client.Turn(direction);
                    }
                    else
                    {
                        if (canUseBeetleAid && Client.UseItem("Beetle Aid"))
                        {
                            _lastUsedBeetleAid = DateTime.UtcNow;
                            Console.WriteLine("[BotLoop] Used Beetle Aid, updated lastUsedBeetleAid");
                            player.SpellAnimationHistory[(ushort)SpellAnimation.Skull] = DateTime.UtcNow.AddSeconds(-2);
                        }
                        else if (canUseOtherItems && (Client.UseItem("Komadium") || Client.UseItem("beothaich deum")))
                        {
                            Console.WriteLine("[BotLoop] Used other item (Komadium or beothaich deum)");
                            player.SpellAnimationHistory[(ushort)SpellAnimation.Skull] = DateTime.UtcNow.AddSeconds(-2);
                            Thread.Sleep(1000); // Consider async/await pattern if possible
                        }

                        Console.WriteLine("[BotLoop] Using Transferblood skill");
                        Client.UseSkill("Transferblood");

                        return false;
                    }
                }
                else if (player == null || !Client.GetNearbyPlayers().Contains(player) || player.HealthPercent > 30 || DateTime.UtcNow.Subtract(player.SpellAnimationHistory[(ushort)SpellAnimation.Skull]).TotalSeconds > 5.0)
                {
                    Console.WriteLine("[BotLoop] Conditions for ending red-skull action met, resetting player and dontWalk flag");
                    player = null;
                    _dontWalk = false;

                    return false;
                }
            }

            Console.WriteLine("[BotLoop] RedSkulledPlayers method returning true");
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

        private void PerformActions()
        {

            _autoStaffSwitch = Client.ClientTab.autoStaffCbox.Checked;
            _fasSpiorad = Client.HasEffect(EffectsBar.FasSpiorad) || (Client.HasSpell("fas spiorad") && DateTime.UtcNow.Subtract(Client.Spellbook["fas spiorad"].LastUsed).TotalSeconds < 1.5);
            _isSilenced = Client.HasEffect(EffectsBar.Silenced);

            Loot();
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
                Spell castedSpell = currentClient.CastedSpell;
                castLinesCountNullable = (castedSpell != null) ? new byte?(castedSpell.CastLines) : null;
            }

            byte? castLinesCount = castLinesCountNullable;
            int? castLines = (castLinesCount != null) ? new int?((int)castLinesCount.GetValueOrDefault()) : null;

            if (castLines.GetValueOrDefault() <= 0 & castLines != null)
            {
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
            DispellPlayerCurse();
            BeagCradh();
            DispellAllyCurse();
            BeagCradhAllies();
            AoPoison();
            Dion();
            Aite();
            Fas();
            AiteAllies();
            FasAllies();

            DragonScale();
            Armachd();
            ArmachdAllies();


            Other(); //Deireas Faileas, Monk Forms, Asgall, Perfect Defense,
                     //Aegis Spehre, ao beag suain, Muscle Stim, Nerve Stim, Mist, Mana Ward
                     //Vanish Elixir, Regens, Mantid Scent, Repair hammer
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
                var playerToDion = _nearbyAllies.FirstOrDefault(p => !p.IsDioned);

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

        private bool Other()
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
                    Client.ClientTab.UseDouble(itemText);
                }
                else if (itemMappings.TryGetValue(Client.ClientTab.doublesCombox.Text, out var itemText))
                {
                    Client.ClientTab.UseDouble(itemText);
                }

                Client.ClientTab.UpdateExpBonusTimer();
            }

            if (Client.ClientTab.autoMushroomCbox.Checked && !Client.HasEffect(EffectsBar.BonusMushroom) && Client._isRegistered && Client.CurrentMP > 100)
            {
                var itemMappings = new Dictionary<string, string>
                {
                    { "Double", "Double Experience Mushroom" },
                    { "50 Percent", "50 Percent Experience Mushroom" },
                    { "Greatest", "Greatest Experience Mushroom" },
                    { "Greater", "Greater Experience Mushroom" },
                    { "Great", "Great Experience Mushroom" },
                    { "Experience Mushroom", "Experience Mushroom" }
                };

                if (Client.ClientTab.mushroomCombox.Text == "Best Available")
                {
                    var mushrooom = Client.ClientTab.FindBestMushroomInInventory(_client);
                    Client.ClientTab.UseMushroom(mushrooom);
                }
                else if (itemMappings.TryGetValue(Client.ClientTab.mushroomCombox.Text, out var mushroom))
                {
                    Client.ClientTab.UseMushroom(mushroom);
                }


                Client.ClientTab.UpdateMushroomBonusTimer();
            }



            if (Client.ClientTab.regenerationCbox.Checked && (!Client.HasEffect(EffectsBar.Regeneration) || !Client.HasEffect(EffectsBar.IncreasedRegeneration)))
            {
                if (Client.HasSpell("Increased Regeneration") && !Client.HasEffect(EffectsBar.IncreasedRegeneration))
                {
                    Client.UseSpell("Increased Regeneration", Client.Player, _autoStaffSwitch, true);
                    return false;
                }
                if (Client.Spellbook.Any(spell => spell.Name != "Increased Regeneration" && spell.Name.Contains("Regeneration"))
                   && !Client.HasEffect(EffectsBar.Regeneration))
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
                            Client client = Client._server.GetClient(currentAlly.Name);
                            if (client != null)
                            {
                                if (!client.HasEffect(EffectsBar.IncreasedRegeneration) && Client.UseSpell("Increased Regeneration", client.Player, _autoStaffSwitch, true))
                                {
                                    return false;
                                }
                                if (Client.Spellbook.Any(s => s.Name != "Increased Regeneration" && s.Name.Contains("Regeneration"))
                                    && !client.HasEffect(EffectsBar.Regeneration)
                                    && TryCastAnyRank("Regeneration", client.Player, _autoStaffSwitch, true))
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
                                if (Client.Spellbook.Any(s => s.Name != "Increased Regeneration" && s.Name.Contains("Regeneration"))
                                    && (!player.SpellAnimationHistory.ContainsKey((ushort)SpellAnimation.Regeneration)
                                        || DateTime.UtcNow.Subtract(player.SpellAnimationHistory[(ushort)SpellAnimation.Regeneration]).TotalSeconds > 1.5)
                                    && TryCastAnyRank("Regeneration", player, _autoStaffSwitch, true))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            if (Client.ClientTab.mantidScentCbox.Checked && Client._isRegistered && !Client.HasEffect(EffectsBar.MantidScent))
            {
                if (Client.UseItem("Mantid Scent") || Client.UseItem("Potent Mantid Scent"))
                {
                    return true;
                }

                Client.ClientTab.mantidScentCbox.Checked = false;
                Client.ServerMessage((byte)ServerMessageType.Whisper, "You do not own Mantid Scent");
                return false;
            }

            if (Client.ClientTab.equipmentrepairCbox.Checked && Client.needsToRepairHammer && DateTime.UtcNow.Subtract(_hammerTimer).TotalMinutes > 40.0)
            {
                Client.UseHammer();
            }

            //Adam this shit breaks casting
/*            Timer dialogWaitTime = Timer.FromSeconds(5);
            while (Client.Dialog == null)
            {
                if (dialogWaitTime.IsTimeExpired)
                    return false;
                Thread.Sleep(10);
            }

            Client.Dialog.DialogNext();*/

            return false;
        }

        private bool DispellAllySuain()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                bool isDispelSuainChecked = ally.AllyPage.dispelSuainCbox.Checked;

                if (isDispelSuainChecked && TryGetSuainedAlly(ally, out Player player, out Client client))
                {

                    Client.UseSpell("ao suain", player, _autoStaffSwitch, true);
                    //Console.WriteLine($"[DispellAllySuain] Player {player.Name}, Hash: {player.GetHashCode()}. IsSuained: {player.IsSuained}");

                    return false;

                }
            }
            return true;
        }

        private bool TryGetSuainedAlly(Ally ally, out Player player, out Client client)
        {
            if (IsAlly(ally, out player, out client))
            {
                //Console.WriteLine($"[TryGetSuainedAlly] Player.ID: {player.ID}, Hash: {player.GetHashCode()}, Player {player.Name} IsSuained: {player.IsSuained}");
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
                        Client client = _server.GetClient(player.Name);
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
                Client client = _server.GetClient(player.Name);
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

            // Process allies for Ao poison dispel
            if (!AoPoisonForAllies(shouldUseFungusExtract))
            {
                return false;
            }

            if (isAoPoisonChecked && Client.HasEffect(EffectsBar.Poison) && isPlayerPoisoned)
            {
                if (isFungusExtractChecked && Client._isRegistered)
                {
                    UseFungusBeetleExtract();
                }
                else
                {
                    Client.UseSpell("ao puinsein", Client.Player, _autoStaffSwitch, false);
                    return false;
                }
            }

            return true;
        }

        private bool AoPoisonForAllies(bool shouldUseFungusExtract)
        {
            foreach (Ally ally in ReturnAllyList())
            {
                if (ally.AllyPage.dispelPoisonCbox.Checked && IsAlly(ally, out Player player, out Client client))
                {
                    if (client.HasEffect(EffectsBar.Poison) && player.IsPoisoned && shouldUseFungusExtract)
                    {
                        if (Client._isRegistered && Client.HasItem("Fungus Beetle Extract"))
                        {
                            UseFungusBeetleExtract();
                        }
                        else
                        {
                            Client.UseSpell("ao puinsein", player, _autoStaffSwitch, false);
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private void UseFungusBeetleExtract()
        {
            for (int j = 0; j < 6; j++)
            {
                Client.UseItem("Fungus Beetle Extract");
            }
            _lastUsedFungusBeetle = DateTime.UtcNow;
        }

        private bool Aite()
        {
            bool isAiteChecked = Client.ClientTab.aiteCbox.Checked;
            bool isPlayerAited = Client.Player.IsAited;
            double aiteDuration = Client.Player.GetState<double>(CreatureState.AiteDuration);
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
                // Check if the Aite spell is enabled for the ally and get the spell name
                if (!ally.AllyPage.dbAiteCbox.Checked || string.IsNullOrEmpty(ally.AllyPage.dbAiteCombox.Text))
                {
                    continue;
                }

                // Ensure the ally is valid and retrieve the player and client
                if (!IsAlly(ally, out Player player, out Client client) || client == null || player == null)
                {
                    continue;
                }

                // Skip if the client already has aite or the player is the client itself
                if (client.HasEffect(EffectsBar.NaomhAite) || player == client.Player || player.IsAited)
                {
                    continue;
                }

                Client.UseSpell(ally.AllyPage.dbAiteCombox.Text, player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool FasAllies()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                // Check if the Fas spell is enabled for the ally and get the spell name
                if (!ally.AllyPage.dbFasCbox.Checked || string.IsNullOrEmpty(ally.AllyPage.dbFasCombox.Text))
                {
                    continue;
                }

                // Ensure the ally is valid and retrieve the player and client
                if (!IsAlly(ally, out Player player, out Client client) || client == null || player == null)
                {
                    continue;
                }

                // Skip if the client already has fas or the player is the client itself
                if (client.HasEffect(EffectsBar.FasNadur) || player == client.Player || player.IsFassed)
                {
                    continue;
                }

                Client.UseSpell(ally.AllyPage.dbFasCombox.Text, player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool Fas()
        {
            bool isFasChecked = Client.ClientTab.fasCbox.Checked;
            bool isPlayerFassed = Client.Player.IsFassed;
            double fasDuration = Client.Player.GetState<double>(CreatureState.FasDuration);
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

                    //Console.WriteLine("[DragonScale] Using Dragon's Scale");

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
                return false;
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
                    shouldUseSpell = !_nearbyValidCreatures.Any(c => c.SpriteID == 87);
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

        private bool DispellPlayerCurse()
        {
            Player player = Client.Player;

            var cursesToDispel = new HashSet<string> { "cradh", "mor cradh", "ard cradh" };

            var curseName = player.GetState<string>(CreatureState.CurseName);
            var curseDuration = player.GetState<double>(CreatureState.CurseDuration);

            if (cursesToDispel.Contains(curseName))
            {
                Client.UseSpell("ao " + curseName, player, _autoStaffSwitch, true);

                var stateUpdates = new Dictionary<CreatureState, object>
                {
                    { CreatureState.CurseDuration, 0.0 },
                    { CreatureState.CurseName, string.Empty }
                };
                CreatureStateHelper.UpdateCreatureStates(Client, player.ID, stateUpdates);

                // Console.WriteLine($"[DispellPlayerCurse] Curse '{curseName}' dispelled from {player.Name}. Resetting curse data.");

                return true;
            }

            return false;
        }
        private bool DispellAllyCurse()
        {
            foreach (Ally ally in ReturnAllyList())
            {
                bool isDispelCurseChecked = ally.AllyPage.dispelCurseCbox.Checked;

                if (isDispelCurseChecked && TryGetCursedAlly(ally, out Player player, out Client client))
                {

                    var cursesToDispel = new HashSet<string> { "cradh", "mor cradh", "ard cradh" };
                    var curseName = player.GetState<string>(CreatureState.CurseName);
                    var curseDuration = player.GetState<double>(CreatureState.CurseDuration);

                    if (cursesToDispel.Contains(curseName))
                    {
                        Client.UseSpell("ao " + curseName, player, _autoStaffSwitch, true);

                        var stateUpdates = new Dictionary<CreatureState, object>
                        {
                            { CreatureState.CurseDuration, 0.0 },
                            { CreatureState.CurseName, string.Empty }
                        };
                        CreatureStateHelper.UpdateCreatureStates(client, player.ID, stateUpdates);

                        //Console.WriteLine($"[DispellAllyCurse] Curse data reset on {player.Name}, Hash: {player.GetHashCode()}. Curse: {curseName}, CurseDuration: {curseDuration}, IsCursed: {player.IsCursed}");

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
                //Console.WriteLine($"[TryGetCursedAlly] Player.ID: {player.ID}, Hash: {player.GetHashCode()}, Player {player.Name} is cursed: {player.IsCursed}");
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

            client = Server.GetClient(ally.Name);

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
                foreach (Player player in Client.GetNearbyPlayers())
                {
                    if (IsAllyAlreadyListed(player.Name) || player == Client.Player)
                    {
                        Ally ally = ReturnAllyList().FirstOrDefault(a => a.Name == player.Name);
                        AllyPage allyPage = ally?.AllyPage;
                        Client client = Server.GetClient(player.Name);

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

                        if (!Client.GetNearbyPlayers().Any(player => ShouldExcludePlayer(player)) || player == Client.Player || healSpell.Contains("comlha"))
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
                else if (isFollowChecked && Client._confirmBubble && CastBubbleBlock())
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
                _dontBash = true;
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
                _nearbyValidCreatures = _nearbyValidCreatures.OrderBy(c => Utility.Random()).ToList();
            }

            if (IsStrangerNearby())
            {
                if (_nearbyValidCreatures.Count > 0)
                {
                    _nearbyValidCreatures.RemoveAll(c => DateTime.UtcNow.Subtract(c.Creation).TotalSeconds < 2.0);
                }
                RemoveDuplicateCreatures();
            }

            if (AllMonsters != null)
            {
                if (HandleAllMonstersSpells())
                {
                    return true;
                }
            }
            else
            {
                if (HandleEnemyListSpells())
                {
                    return true;
                }
            }

            if (AllMonsters != null)
            {
                if (CastAttackSpells(AllMonsters, _nearbyValidCreatures))
                {
                    return true;
                }
            }
            else
            {
                if (CastAttackSpellsForEnemies())
                {
                    return true;
                }
            }

            _dontBash = false;
            return false;
        }

        private void RemoveDuplicateCreatures()
        {
            var duplicates = _nearbyValidCreatures
                .GroupBy(c => c.Location)
                .SelectMany(g => g.Skip(1))
                .ToList();

            foreach (var duplicate in duplicates)
            {
                _nearbyValidCreatures.Remove(duplicate);
            }
        }

        private bool HandleAllMonstersSpells()
        {
            if (AllMonsters.ignoreCbox.Checked)
            {
                _nearbyValidCreatures.RemoveAll(creature => AllMonsters.ignoreLbox.Items.Contains(creature.SpriteID.ToString()));
            }

            List<Creature> priorityCreatures = new List<Creature>();
            List<Creature> nonPriorityCreatures = new List<Creature>();

            if (AllMonsters.priorityCbox.Checked)
            {
                foreach (var creature in _nearbyValidCreatures)
                {
                    if (AllMonsters.priorityLbox.Items.Contains(creature.SpriteID.ToString()))
                    {
                        priorityCreatures.Add(creature);
                    }
                    else if (!AllMonsters.priorityOnlyCbox.Checked)
                    {
                        nonPriorityCreatures.Add(creature);
                    }
                }
            }
            else
            {
                nonPriorityCreatures = new List<Creature>(_nearbyValidCreatures);
            }

            if (AllMonsters.spellAllRbtn.Checked)
            {
                creature = null;
                if (ExecuteEngagementStrategy(AllMonsters, priorityCreatures, nonPriorityCreatures))
                {
                    return true;
                }
            }
            else
            {
                if (SpellCreaturesOneAtATime(AllMonsters, priorityCreatures, nonPriorityCreatures))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HandleEnemyListSpells()
        {
            foreach (Enemy enemy in ReturnEnemyList())
            {
                var creatureList = Client.GetAllNearbyMonsters(11, new ushort[] { enemy.SpriteID })
                    .Where(c => c.SpriteID.ToString() == enemy.SpriteID.ToString())
                    .ToList();

                if (creatureList.Count > 0)
                {
                    if (enemy.EnemyPage.spellAllRbtn.Checked)
                    {
                        creature = null;
                        if (DecideAndExecuteEngagementStrategy(enemy.EnemyPage, creatureList))
                        {
                            _dontBash = true;
                            return true;
                        }
                    }
                    else if (SpellOneAtATime(enemy.EnemyPage, creatureList))
                    {
                        _dontBash = true;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool ExecuteEngagementStrategy(dynamic config, List<Creature> priority, List<Creature> nonPriority)
        {
            if (priority.Count > 0 && DecideAndExecuteEngagementStrategy(config, priority))
            {
                _dontBash = true;
                return true;
            }
            if (nonPriority.Count > 0 && DecideAndExecuteEngagementStrategy(config, nonPriority))
            {
                _dontBash = true;
                return true;
            }
            return false;
        }

        private bool SpellCreaturesOneAtATime(dynamic config, List<Creature> priority, List<Creature> nonPriority)
        {
            if (priority.Count > 0 && SpellOneAtATime(config, priority))
            {
                _dontBash = true;
                return true;
            }
            if (!priority.Contains(creature) && nonPriority.Count > 0 && SpellOneAtATime(config, nonPriority))
            {
                _dontBash = true;
                return true;
            }
            return false;
        }

        private bool CastAttackSpells(dynamic config, List<Creature> creatures)
        {
            if (config.priorityCbox.Checked)
            {
                List<Creature> priorityCreatures = new List<Creature>();
                List<Creature> nonPriorityCreatures = new List<Creature>();

                foreach (var creature in creatures)
                {
                    if (config.priorityLbox.Items.Contains(creature.SpriteID.ToString()))
                    {
                        priorityCreatures.Add(creature);
                    }
                    else if (!config.priorityOnlyCbox.Checked)
                    {
                        nonPriorityCreatures.Add(creature);
                    }
                }

                if (CastAttackSpell(config, priorityCreatures) || CastAttackSpell(config, nonPriorityCreatures))
                {
                    return true;
                }
            }
            else if (CastAttackSpell(config, creatures))
            {
                return true;
            }

            return false;
        }

        private bool CastAttackSpellsForEnemies()
        {
            foreach (Enemy enemy in ReturnEnemyList())
            {
                var creaturesToAttack = Client.GetAllNearbyMonsters(11, new ushort[] { enemy.SpriteID })
                    .Where(c => c.SpriteID.ToString() == enemy.SpriteID.ToString())
                    .ToList();

                if (CastAttackSpell(enemy.EnemyPage, creaturesToAttack))
                {
                    return true;
                }
            }

            return false;
        }


        private bool CastAttackSpell(EnemyPage enemyPage, List<Creature> creatureList)
        {
            if (!enemyPage.targetCbox.Checked || creatureList.Count == 0)
            {
                return false;
            }

            // Handle single-target spells
            if (enemyPage.spellOneRbtn.Checked)
            {
                if (!creatureList.Contains(creature))
                {
                    creature = creatureList.OrderBy(c => c.Location.DistanceFrom(Client._clientLocation)).FirstOrDefault();
                }
                if (creature == null)
                {
                    return false;
                }
                creatureList = new List<Creature> { creature };
            }

            // Handle PND spells if applicable
            if (AllMonsters != null && AllMonsters.mpndDioned.Checked)
            {
                bool proceedWithPND = true;

                if (!enemyPage.attackCboxOne.Checked && !enemyPage.attackCboxTwo.Checked)
                {
                    if (!_nearbyValidCreatures.Any(c => c.IsDioned))
                    {
                        proceedWithPND = false;
                    }
                }
                else if (!_nearbyValidCreatures.All(c => c.CanPND))
                {
                    proceedWithPND = false;
                }

                if (proceedWithPND)
                {
                    Creature creatureTarget = _nearbyValidCreatures
                        .Where(c => c.CanPND)
                        .OrderBy(c => c.Location.DistanceFrom(Client._clientLocation))
                        .FirstOrDefault();

                    if (creatureTarget != null)
                    {
                        return Client.TryUseAnySpell(
                            new[] { "ard pian na dion", "mor pian na dion", "pian na dion" },
                            creatureTarget,
                            _autoStaffSwitch,
                            false);
                    }
                }
            }

            // Handle attack spells when not silenced
            if (!_isSilenced)
            {
                // Attack Combo Box Two
                if (enemyPage.attackCboxTwo.Checked)
                {
                    Creature target = SelectAttackTarget(enemyPage, creatureList, enemyPage.attackComboxTwo.Text);
                    if (target != null)
                    {
                        string spellName = enemyPage.attackComboxTwo.Text.Trim();

                        switch (spellName)
                        {
                            case "Supernova Shot":
                                return Client.UseSpell("Supernova Shot", target, _autoStaffSwitch, false);

                            case "Shock Arrow":
                                return Client.UseSpell("Shock Arrow", target, _autoStaffSwitch, false) &&
                                       Client.UseSpell("Shock Arrow", target, _autoStaffSwitch, false);

                            case "Volley":
                                return Client.UseSpell("Volley", target, _autoStaffSwitch, true);

                            case "MSPG":
                                if (enemyPage.mspgPct.Checked && Client.ManaPct < 80)
                                {
                                    _manaLessThanEightyPct = true;
                                    Client.UseSpell("fas spiorad", null, _autoStaffSwitch, false);
                                    return true;
                                }
                                return Client.UseSpell("mor strioch pian gar", Client.Player, _autoStaffSwitch, true);

                            case "Unholy Explosion":
                                return Client.UseSpell("Unholy Explosion", target, _autoStaffSwitch, false);

                            case "Cursed Tune":
                                return TryCastAnyRank("Cursed Tune", target, _autoStaffSwitch, false);

                            case "M/DSG":
                                return Client.UseSpell("mor deo searg gar", Client.Player, _autoStaffSwitch, false) ||
                                       Client.UseSpell("deo searg gar", Client.Player, _autoStaffSwitch, false);
                        }
                    }
                }

                // Attack Combo Box One
                if (enemyPage.attackCboxOne.Checked)
                {
                    Creature target = SelectAttackTarget(enemyPage, creatureList, enemyPage.attackComboxOne.Text);
                    if (target != null)
                    {
                        string spellName = enemyPage.attackComboxOne.Text.Trim();

                        switch (spellName)
                        {
                            case "lamh":
                                return Client.TryUseAnySpell(
                                    new[] { "beag athar lamh", "beag srad lamh", "athar lamh", "srad lamh", "Howl" },
                                    null,
                                    _autoStaffSwitch,
                                    false);

                            case "A/DS":
                                return Client.TryUseAnySpell(
                                    new[] { "ard deo searg", "deo searg" },
                                    target,
                                    _autoStaffSwitch,
                                    false);

                            case "A/M/PND":
                                return Client.TryUseAnySpell(
                                    new[] { "ard pian na dion", "mor pian na dion", "pian na dion" },
                                    target,
                                    _autoStaffSwitch,
                                    false);

                            case "Frost Arrow":
                                return TryCastAnyRank("Frost Arrow", target, _autoStaffSwitch, false);

                            default:
                                return Client.UseSpell(spellName, target, _autoStaffSwitch, false) ||
                                       TryCastAnyRank(spellName, target, _autoStaffSwitch, false);
                        }
                    }
                }
            }

            // Handle spells when silenced
            if (_isSilenced)
            {
                if (enemyPage.mpndSilenced.Checked)
                {
                    Creature mpndTarget = SelectAttackTarget(enemyPage, creatureList, "A/M/PND");
                    return Client.TryUseAnySpell(
                        new[] { "ard pian na dion", "mor pian na dion", "pian na dion" },
                        mpndTarget,
                        _autoStaffSwitch,
                        false);
                }

                if (enemyPage.mspgSilenced.Checked && SelectAttackTarget(enemyPage, creatureList, "MSPG") != null)
                {
                    if (enemyPage.mspgPct.Checked && Client.ManaPct < 80)
                    {
                        _manaLessThanEightyPct = true;
                        return Client.UseSpell("fas spiorad", null, _autoStaffSwitch, false);
                    }
                    Client.UseSpell("mor strioch pian gar", Client.Player, _autoStaffSwitch, true);
                    return true;
                }
            }

            return false;
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
            foreach (var creature in creatureList.OrderBy(c => c.Location.DistanceFrom(Client._clientLocation)))
            {

                if (!creatureList.Contains(this.creature))
                {
                    this.creature = creatureList.OrderBy(c => c.Location.DistanceFrom(Client._clientLocation)).FirstOrDefault<Creature>();
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
                _dontBash = true;
                Client.UseSpell(enemyPage.spellsFasCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            if (enemyPage.curseFirstRbtn.Checked && enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)
            {
                _dontBash = true;
                Client.UseSpell(enemyPage.spellsCurseCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            if ((!enemyPage.fasFirstRbtn.Checked || !enemyPage.spellsFasCbox.Checked || creature.IsFassed) && enemyPage.spellsCurseCbox.Checked && !creature.IsCursed)
            {
                _dontBash = true;
                Client.UseSpell(enemyPage.spellsCurseCombox.Text, creature, _autoStaffSwitch, false);
                return true;
            }
            if ((!enemyPage.curseFirstRbtn.Checked || !enemyPage.spellsCurseCbox.Checked || creature.IsCursed) && enemyPage.spellsFasCbox.Checked && !creature.IsFassed)
            {
                _dontBash = true;
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
            List<Creature> eligibleCreatures = creatureList.Where(c => !c.IsFassed || !c.IsCursed).ToList();

            if (eligibleCreatures.Any() && (enemyPage.spellsFasCbox.Checked || enemyPage.spellsCurseCbox.Checked))
            {
                if (enemyPage.fasFirstRbtn.Checked)
                {
                    return ExecuteFasFirstStrategy(enemyPage, eligibleCreatures);
                }
                else if (enemyPage.curseFirstRbtn.Checked)
                {
                    return ExecuteCurseFirstStrategy(enemyPage, eligibleCreatures);
                }
            }

            return false;
        }

        private bool ExecuteFasFirstStrategy(EnemyPage enemyPage, List<Creature> eligibleCreatures)
        {
            // Try casting Fas first, then Curse if applicable
            if (enemyPage.spellsFasCbox.Checked && CastFasIfApplicable(enemyPage, eligibleCreatures))
            {
                _dontBash = true;
                return true;
            }

            if (enemyPage.spellsCurseCbox.Checked && CastCurseIfApplicable(enemyPage, eligibleCreatures))
            {
                _dontBash = true;
                return true;
            }

            return false;
        }

        private bool ExecuteCurseFirstStrategy(EnemyPage enemyPage, List<Creature> eligibleCreatures)
        {
            // Try casting Curse first, then Fas if applicable
            if (enemyPage.spellsCurseCbox.Checked && CastCurseIfApplicable(enemyPage, eligibleCreatures))
            {
                _dontBash = true;
                return true;
            }

            if (enemyPage.spellsFasCbox.Checked && CastFasIfApplicable(enemyPage, eligibleCreatures))
            {
                _dontBash = true;
                return true;
            }

            return false;
        }

        private bool CastCurseIfApplicable(EnemyPage enemyPage, List<Creature> creatures)
        {
            lock (_lock)
            {
                //Console.WriteLine($"[CastCurseIfApplicable] Total creatures: {creatures.Count}");

                // Filter creatures that are not cursed
                var eligibleCreatures = creatures.Where(c => !c.IsCursed);

                //Console.WriteLine($"[CastCurseIfApplicable] Eligible creatures (not cursed): {eligibleCreatures.Count()}");

                // Select the nearest eligible creature if specified, otherwise select any eligible creature
                Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                    ? eligibleCreatures.OrderBy(creature => creature.Location.DistanceFrom(Client._serverLocation)).FirstOrDefault()
                    : eligibleCreatures.FirstOrDefault();

                // If a target is found and casting curses is enabled, cast the curse spell
                if (targetCreature != null && enemyPage.spellsCurseCbox.Checked)
                {
                    //Console.WriteLine($"[CastCurseIfApplicable] Targeting creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, LastCursed: {targetCreature.LastCursed}, IsCursed: {targetCreature.IsCursed}");
                    Client.UseSpell(enemyPage.spellsCurseCombox.Text, targetCreature, _autoStaffSwitch, false);
                    _dontBash = true; // Indicate that an action was taken
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
                var eligibleCreatures = creatures.Where(c => !c.IsFassed);

                //Console.WriteLine($"[CastFasIfApplicable] Eligible creatures (not fassed): {eligibleCreatures.Count()}");

                // Select the nearest eligible creature if specified, otherwise select any eligible creature
                Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                    ? eligibleCreatures.OrderBy(creature => creature.Location.DistanceFrom(Client._serverLocation)).FirstOrDefault()
                    : eligibleCreatures.FirstOrDefault();

                // If a target is found and casting the 'fas' spell is enabled, cast the spell
                if (targetCreature != null && enemyPage.spellsFasCbox.Checked)
                {
                    //Console.WriteLine($"[CastFasIfApplicable] Targeting creature ID: {targetCreature.ID}, Name: {targetCreature.Name}, LastFassed: {targetCreature.LastFassed}, IsFassed: {targetCreature.IsFassed}");
                    Client.UseSpell(enemyPage.spellsFasCombox.Text, targetCreature, _autoStaffSwitch, false);
                    _dontBash = true; // Indicate that an action was taken
                    return true;
                }

                return false; // No action was taken
            }

        }

        private Creature SelectAttackTarget(EnemyPage enemyPage, List<Creature> creatureList, string spellName = "")
        {
            List<Creature> attackList;

            bool flag = enemyPage.attackCboxTwo.Checked && enemyPage.targetCbox.Checked &&
                        (enemyPage.attackComboxTwo.Text == "MSPG" || enemyPage.attackComboxTwo.Text == "M/DSG");

            bool checkCursed = enemyPage.targetCursedCbox.Checked;
            bool checkFassed = enemyPage.targetFassedCbox.Checked;

            // Build the attack list based on cursed and fassed filters
            attackList = BuildAttackList(creatureList, checkCursed, checkFassed, flag);

            if (attackList.Count == 0)
            {
                return null;
            }

            // Apply additional filters based on spellName
            attackList = FilterAttackListBySpell(attackList, spellName, enemyPage);

            if (attackList.Count == 0)
            {
                return null;
            }

            // If the current creature is still in the attack list, return it
            if (creature != null && attackList.Contains(creature))
            {
                return creature;
            }

            // Decide on the target selection method
            if (ShouldSelectClusterTarget(enemyPage, flag))
            {
                return SelectClusterTarget(enemyPage, attackList);
            }
            else
            {
                // Select the nearest creature from the attack list
                return attackList.OrderBy(c => c.Location.DistanceFrom(Client._serverLocation)).FirstOrDefault();
            }
        }

        private List<Creature> BuildAttackList(List<Creature> creatureList, bool checkCursed, bool checkFassed, bool flag)
        {
            List<Creature> attackList = new List<Creature>();

            if (!checkCursed && !checkFassed)
            {
                attackList = new List<Creature>(creatureList);
            }
            else
            {
                foreach (Creature creature in creatureList)
                {
                    bool addCreature = (!checkCursed || creature.IsCursed) && (!checkFassed || creature.IsFassed);

                    if (addCreature)
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

            return attackList;
        }

        private List<Creature> FilterAttackListBySpell(List<Creature> attackList, string spellName, EnemyPage enemyPage)
        {
            if (spellName == "Frost Arrow")
            {
                attackList = attackList.Where(c => !c.IsFrozen).ToList();
            }
            else if (spellName == "Cursed Tune")
            {
                attackList = attackList.Where(c => !c.IsPoisoned).ToList();
            }
            else if (spellName != "lamh" && spellName != "Shock Arrow" && spellName != "Volley")
            {
                if (spellName != "A/M/PND")
                {
                    attackList = attackList.Where(c => !c.IsDioned).ToList();
                }

                if (enemyPage.expectedHitsNum.Value > 0m)
                {
                    attackList = attackList.Where(creature => CalculateHitCounter(creature, enemyPage)).ToList();
                }
            }

            return attackList;
        }

        private bool ShouldSelectClusterTarget(EnemyPage enemyPage, bool flag)
        {
            return enemyPage.targetCombox.Text.Contains("Cluster") && !flag && (!_isSilenced || !enemyPage.mpndSilenced.Checked);
        }

        private Creature SelectClusterTarget(EnemyPage enemyPage, List<Creature> attackList)
        {
            int maxDistance = GetMaxClusterDistance(enemyPage.targetCombox.Text);

            Dictionary<Creature, int> clusterTargets = new Dictionary<Creature, int>();

            List<Creature> nearbyCreatures = Client.GetAllNearbyMonsters(12)
                                                   .Concat(_playersExistingOver250ms)
                                                   .ToList();

            List<Creature> validCreatures = Client.GetNearbyValidCreatures(12);

            List<Creature> borosCreatures = validCreatures
                .Where(c => CONSTANTS.GREEN_BOROS.Contains(c.SpriteID) || CONSTANTS.RED_BOROS.Contains(c.SpriteID))
                .ToList();

            if (!nearbyCreatures.Contains(Client.Player))
            {
                nearbyCreatures.Add(Client.Player);
            }

            foreach (Creature c in nearbyCreatures)
            {
                int count = attackList.Count(creature => IsWithinClusterRange(c.Location, creature.Location, maxDistance));

                if (!IsCreatureNearBoros(c, borosCreatures, maxDistance))
                {
                    clusterTargets[c] = count;
                }
            }

            // Remove unsuitable targets
            var unsuitableCreatures = clusterTargets.Keys
                .Where(c => (c.Type == CreatureType.Aisling ||
                            (c.Type == CreatureType.WalkThrough && !validCreatures.Contains(c)) ||
                            c.IsDioned) && clusterTargets[c] <= 1)
                .ToList();

            foreach (var c in unsuitableCreatures)
            {
                clusterTargets.Remove(c);
            }

            if (clusterTargets.Count == 0)
            {
                return null;
            }

            // Select the best target
            return clusterTargets
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key.Creation)
                .First()
                .Key;
        }

        private int GetMaxClusterDistance(string clusterText)
        {
            return clusterText switch
            {
                "Cluster 29" => 3,
                "Cluster 13" => 2,
                _ => 1,
            };
        }

        private bool IsWithinClusterRange(Location loc1, Location loc2, int maxDistance)
        {
            return loc1.DistanceFrom(loc2) <= maxDistance ||
                   (maxDistance > 1 && IsDiagonallyAdjacent(loc1.Point, loc2.Point, maxDistance));
        }

        private bool IsCreatureNearBoros(Creature c, List<Creature> borosCreatures, int maxDistance)
        {
            foreach (var borosCreature in borosCreatures)
            {
                if (c == borosCreature)
                {
                    return true;
                }

                foreach (Location adjLoc in Client.GetAdjacentPoints(borosCreature))
                {
                    if (IsWithinClusterRange(c.Location, adjLoc, maxDistance))
                    {
                        return true;
                    }
                }
            }

            return false;
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
                List<Client> nearbyClients = Client.GetNearbyPlayers()
                    .Select(player => Server.GetClient(player?.Name))
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
                List<Creature> greenBorosInRange = Client.GetAllNearbyMonsters(8, CONSTANTS.GREEN_BOROS.ToArray());
                foreach (Creature creature in greenBorosInRange.ToList<Creature>())
                {
                    foreach (Location location in Client.GetWarpPoints(new Location(Client._map.MapID, 0, 0)))
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
                var additionalCreatures = Client.GetAllNearbyMonsters(8, CONSTANTS.GREEN_BOROS.ToArray())
                    .Where(creature => !Client.GetWarpPoints(new Location(Client._map.MapID, 0, 0))
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
            _playersExistingOver250ms = !IsStrangerNearby() ? _playersExistingOver250ms : _playersExistingOver250ms.Where(p => DateTime.UtcNow.Subtract(p.Creation).TotalSeconds > 2.0).ToList();
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
            if (DateTime.UtcNow.Subtract(_spellTimer).TotalSeconds > 1.0)
            {
                Client._spellHistory.Clear();
            }
        }

        private void ManageSpellCastingDelay()
        {
            var spellCastLines = Client?.CastedSpell?.CastLines ?? 0;
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
                    List<Objects.GroundItem> nearbyObjects = Client.GetNearbyGroundItems(4);

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

        private void ProcessLoot(List<Objects.GroundItem> nearbyObjects, Rectangle lootArea)
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

        private bool IsGold(Objects.GroundItem obj, Rectangle lootArea)
        {
            return lootArea.ContainsPoint(obj.Location.Point) && obj.SpriteID == 140 && DateTime.UtcNow.Subtract(obj.Creation).TotalSeconds > 2.0;
        }

        private bool IsLootableItem(Objects.GroundItem obj, Rectangle lootArea)
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


    }


}
