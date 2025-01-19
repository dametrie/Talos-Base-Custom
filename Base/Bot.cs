using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Talos.Bashing;
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
using Talos.Utility;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Talos.Base
{
    internal class Bot : BotBase
    {
        private static object _lock { get; set; } = new object();
        private Dictionary<int, bool> routeFindPerformed = new Dictionary<int, bool>();
        private int _dropCounter;
        private int currentWaypointIndex = 0;
        private DateTime _sprintPotionLastUsed = DateTime.MinValue;
        internal Creature creature;
        internal Creature target;
        internal BashingBase BashingBase;
        CommandManager commandManager = CommandManager.Instance;

        private bool _autoStaffSwitch;
        private bool _fasSpiorad;
        private bool _isSilenced;
        private bool bashClassSet;
        private DateTime _lastHidden = DateTime.MinValue;
        internal bool _needFasSpiorad = true;
        internal bool _manaLessThanEightyPct = true;
        internal bool _rangerNear = false;
        internal bool _shouldAlertItemCap;
        internal bool _recentlyAoSithed;
        internal bool[] itemDurabilityAlerts = new bool[5];

        internal bool _dontWalk;
        internal bool _dontCast;
        internal bool _dontBash;
        private int? _leaderID;
        internal bool _hasRescue;

        internal byte _fowlCount;
        internal bool _receivedDblBonus = false;
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
        internal DateTime _lastUnstick = DateTime.MinValue;

        internal List<Ally> _allyList = new List<Ally>();
        internal List<Enemy> _enemyList = new List<Enemy>();
        internal List<Player> _playersExistingOver250ms = new List<Player>();
        internal List<Player> _skulledPlayers = new List<Player>();
    
        internal List<Creature> _nearbyValidCreatures = new List<Creature>();
        internal List<Location> ways = new List<Location>();

        internal List<Player> NearbyAllies { get; set; } = new List<Player>();
        internal HashSet<string> _allyListName = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        internal HashSet<ushort> _enemyListID = new HashSet<ushort>();



        internal System.Windows.Forms.Label currentAction;
        private Location _lastBubbleLocation;
        private Location _pcDeathSpot = new Location(3052, 27, 19);
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
        internal bool _spikeGameToggle;
        private DateTime _animationTimer = DateTime.MinValue;
        private bool generaterandom;
        private bool equipattempted;
        private DateTime neckSwap;
        private bool autoForm;
        private DateTime LastPointInSync = DateTime.MinValue;
        private DateTime LastDirectionInSync = DateTime.MinValue;
        private bool bashWffUsed;
        private DateTime assailUse = DateTime.MinValue;
        private DateTime skillUse = DateTime.MinValue;
        private bool hasrepaired;
        private bool _hasDeposited;

        public bool RecentlyUsedGlowingStone { get; set; } = false;
        public bool RecentlyUsedDragonScale { get; set; } = false;
        public bool RecentlyUsedFungusExtract { get; set; } = false;
        internal AllyPage AllyPage { get; set; }
        internal EnemyPage AllMonsters { get; set; }



        internal Bot(Client client, Server server) : base(client, server)
        {
            AddTask(new BotLoop(BotLoop));
            AddTask(new BotLoop(SoundLoop));
            AddTask(new BotLoop(WalkLoop));
            AddTask(new BotLoop(MultiLoop));
        }

        private void MultiLoop()
        {
            while (!_shouldThreadStop)
            {
                try
                {
                    // Block if Client or ClientTab is null
                    if (Client == null || Client.ClientTab == null)
                        continue;

                    Client.InventoryFull = Client.Inventory.IsFull;


                    //CheckScrollTimers();
                    HandleBashingCycle();
                    //DojoLoop();

                    // Block if conditions for ranger or exchange are not met
                    if ((_rangerNear && Client.ClientTab.rangerStopCbox.Checked) || Client.ExchangeOpen)
                    {
                        Thread.Sleep(100);
                        continue;
                    }


                    HolidayEvents();
                    // ItemFinding();
                    // TreasureChests();
                    // Raffles();
                    TaskLoop();
                    // TavalyWallHacks();
                    MonsterForm();
                    DuraLoop();

                    // Sleep before the next iteration
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MultiLoop] Exception occurred: {ex}");
                }
            }
        }

        private void DuraLoop()
        {
           
        }

        #region HandleBashingCycle
        private void HandleBashingCycle()
        {
            try
            {
                if (!bashClassSet)
                    SetBashClass();


                if (!Client?.ClientTab?.IsBashing ?? true)
                    return;

                if (_skulledPlayers.Count > 0 && Client.ClientTab.autoRedCbox.Checked)
                    return;

                EnsureOneLineWalkEnabled();

                if (!BashingBase.DoBashing())
                    WayPointWalking(true);

                if (Client?.ClientTab?.IsBashing ?? false && !(_skulledPlayers.Count > 0 && Client.ClientTab.autoRedCbox.Checked))
                {
                    EnsureOneLineWalkEnabled();
                    ProcessBashingTargets();
                }
            }
            catch (Exception ex)
            {
                LogException("HandleBashingCycle", ex);
            }
        }

        private void ProcessBashingTargets()
        {
            try
            {
                if (Client.ClientTab?.btnBashingNew.Text != "Stop Bashing")
                    return;

                if (_skulledPlayers.Count > 0 && Client.ClientTab.autoRedCbox.Checked || Client.IsSkulled)
                    return;

                EnsureOneLineWalkEnabled();

                if (!Client.ClientTab.assistBasherChk.Checked)
                {
                    HandleProtectionAndBashing();
                }
                else
                {
                    AssistBasherLogic();
                }
            }
            catch (Exception ex)
            {
                LogException("ProcessBashingTargets", ex);
            }
        }

        private void AssistBasherLogic()
        {
            string leadBasherName = Client.ClientTab.leadBasherTxt.Text;

            // Get valid monsters near the lead basher
            var nearbyMonsters = Client.GetNearbyValidCreatures(8)
                .Where(mob => !Client.IsLocationSurrounded(mob.Location) && !Client.Map.IsWall(mob.Location))
                .Where(mob => mob.Location.DistanceFrom(Server.GetClient(leadBasherName).ClientLocation) <= Client.ClientTab.numAssitantStray.Value)
                .ToList();

            // Assign the target
            target = Client.ClientTab.radioAssitantStray.Checked
                ? nearbyMonsters
                    .OrderBy(mob => mob.Location.DistanceFrom(Client.ClientLocation))
                    .FirstOrDefault(mob => ShouldEngageTarget(mob))
                : Server.GetClient(leadBasherName).Bot.target;

            if (target != null)
            {
                HandleAssistTargetMovement(target);
            }
            else
            {
                HandleAssistIdleState();
            }
        }

        private bool ShouldEngageTarget(Creature mob)
        {
            return (!Client.ClientTab.chkWaitForCradh.Checked || mob.IsCursed)
                   && (!Client.ClientTab.chkWaitForFas.Checked || mob.IsFassed);
        }

        private void HandleAssistTargetMovement(Creature target)
        {
            if (target.Location.DistanceFrom(Client.ClientLocation) > 1)
            {
                // Pathfind towards the target
                Client.Pathfind(target.Location, shouldBlock: false);

                // Check if we can use skills from range
                if (Client.ClientTab.chkUseSkillsFromRange.Checked &&
                    (target.Location.X == Client.ClientLocation.X || target.Location.Y == Client.ClientLocation.Y))
                {
                    // Face the target
                    Direction direction = target.Location.GetDirection(Client.ClientLocation);
                    if (Client.ClientDirection != direction)
                        Client.Turn(direction);

                    // Use skills
                    InitiateBashing();
                }
            }
            else
            {
                // Handle close-range interactions
                HandleCloseRangeMovement(target);
            }
        }

        private void HandleAssistIdleState()
        {
            // Enable follow mode if no valid targets exist
            if (!Client.ClientTab.followCbox.Checked)
            {
                Client.ClientTab.followCbox.Checked = true; // Start following the lead basher or group
                Client.ServerMessage((byte)ServerMessageType.TopRight, "No valid targets. Enabling follow mode...");
                return;
            }

            // Handle random waypoints if enabled
            if (Client.ClientTab.chkRandomWaypoints.Checked)
            {
                HandleRandomWaypoints();
                return;
            }

            // Check if the bot is stuck and needs to refresh or move randomly
            if ((DateTime.UtcNow - _lastUnstick).TotalMilliseconds > 3000)
            {
                Client.RefreshRequest(false);
                Client.Walk(RandomUtils.RandomEnumValue<Direction>());
                _lastUnstick = DateTime.UtcNow;
                Client.ServerMessage((byte)ServerMessageType.TopRight, "No valid actions. Unsticking...");
            }
        }


        private void HandleProtectionAndBashing()
        {
            var nearbyPlayers = Client.GetNearbyPlayers();
            var nearbyMonsters = Client.GetNearbyValidCreatures(10);

            Player whoToProtect = FindWhoToProtect(nearbyPlayers, nearbyMonsters);

            var validTargets = FilterValidTargets(nearbyMonsters, whoToProtect);

            if (validTargets.Any())
            {
                HandleTargetingAndMovement(validTargets, whoToProtect);
            }
            else
            {
                HandleRandomWaypoints();
            }
        }

        private void HandleTargetingAndMovement(List<Creature> validTargets, Player whoToProtect)
        {
            target = validTargets.FirstOrDefault(mob =>
                ShouldEngageTarget(mob) ||
                (whoToProtect != null && mob.Location.DistanceFrom(whoToProtect.Location) < 5)) ?? target;

            if (target == null)
                return;

            if (target.Location.DistanceFrom(Client.ClientLocation) > 1)
            {
                if (target.Location.DistanceFrom(Client.ClientLocation) > 4 && IsAnyGroupMemberPramhed())
                {
                    Client.ServerMessage((byte)ServerMessageType.TopRight, "Waiting for pramh...");
                    return;
                }

                MoveTowardsTarget(target);
            }
            else
            {
                FaceAndBashTarget(target);
            }
        }

        private bool IsAnyGroupMemberPramhed()
        {
            return NearbyAllies != null && NearbyAllies.Any(member => member.IsAsleep);
        }

        private void MoveTowardsTarget(Creature target)
        {
            Direction direction = target.Location.GetDirection(Client.ClientLocation);
            Client.Pathfind(target.Location);

            if (Client.ClientTab.chkUseSkillsFromRange.Checked &&
                (target.Location.X == Client.ClientLocation.X || target.Location.Y == Client.ClientLocation.Y))
            {
                if (Client.ClientDirection != direction)
                    Client.Turn(direction);

                InitiateBashing();
            }
        }

        private void FaceAndBashTarget(Creature target)
        {
            if (target.Location == Client.ClientLocation && !Client.HasEffect(EffectsBar.BeagSuain) &&
                !Client.HasEffect(EffectsBar.Pramh) && !Client.HasEffect(EffectsBar.Suain))
            {
                if ((DateTime.UtcNow - _lastUnstick).TotalMilliseconds > 3000)
                {
                    Client.RefreshRequest(false);
                    Client.Walk(RandomUtils.RandomEnumValue<Direction>());
                    _lastUnstick = DateTime.UtcNow;
                    return;
                }
            }

            Direction direction = target.Location.GetDirection(Client.ClientLocation);
            if (Client.ClientDirection != direction)
                Client.Turn(direction);

            InitiateBashing();
        }




        private void HandleRandomWaypoints()
        {
            if (!Client.ClientTab.chkRandomWaypoints.Checked)
                return;

            if (!generaterandom)
            {
                GenerateRandomWaypoints();
                generaterandom = true;
            }
            else
            {
                if (!NavigateToNextWaypoint())
                {
                    generaterandom = false; // Regenerate if navigation fails
                }
            }
        }


        private bool NavigateToNextWaypoint()
        {
            // Ensure there are waypoints to navigate
            if (!Client.Bot.ways.Any())
                return false;

            // Reset index if out of bounds
            if (currentWaypointIndex >= Client.Bot.ways.Count)
                currentWaypointIndex = 0; // Wrap around

            // Get the next waypoint
            var nextWaypoint = Client.Bot.ways[currentWaypointIndex];
            //Console.WriteLine($"[NavigateToNextWaypoint] Attempting to navigate to waypoint: {nextWaypoint}");

            // Check if pathfinding is valid
            var path = Client.Pathfinder.FindPath(Client.ServerLocation, nextWaypoint);
            if (path.Count > 0)
            {
                // Path exists; attempt to move
                Client.Pathfind(nextWaypoint);

                // Verify if we've reached the waypoint
                if (Client.ClientLocation == nextWaypoint)
                {
                    //Console.WriteLine($"[NavigateToNextWaypoint] Successfully reached waypoint: {nextWaypoint}");
                    currentWaypointIndex++; // Move to the next waypoint
                }
                else
                {
                    //Console.WriteLine($"[NavigateToNextWaypoint] Pathfinding incomplete; still moving towards: {nextWaypoint}");
                }
                return true;
            }
            else
            {
                // Pathfinding failed; log and skip this waypoint
                //Console.WriteLine($"[NavigateToNextWaypoint] Cannot find valid path to waypoint: {nextWaypoint}. Skipping...");
                currentWaypointIndex++;
            }

            return false;
        }


        private void GenerateRandomWaypoints()
        {
            Client.ClientTab.WayForm.waypointsLBox.Items.Clear();
            Client.Bot.ways.Clear();

            foreach (var map in Server._maps.Values.Where(m => m.MapID == Client.Map.MapID))
            {
                int waypointsToGenerate = MathUtils.Clamp((map.Height + map.Width) / 8, 5, 12);

                for (int i = 0; i < waypointsToGenerate; i++)
                {
                    Location location;
                    do
                    {
                        location = new Location(map.MapID,
                            (short)RandomUtils.Random(1, Client.Map.Width - 2),
                            (short)RandomUtils.Random(1, Client.Map.Height - 2));
                    }
                    while (Client.Map.IsWall(location) || Client.Pathfinder.FindPath(Client.ServerLocation, location).Count == 0);

                    if (!Client.Bot.ways.Contains(location))
                    {
                        Client.Bot.ways.Add(location);
                        Client.ClientTab.WayForm.waypointsLBox.Items.Add($"({location.X}, {location.Y}) {map.Name}: {map.MapID}");
                    }
                }
            }
        }

        private void HandleCloseRangeMovement(Creature target)
        {
            // Ensure the bot is not stuck or standing on the same point as the target
            if (target.Location == Client.ClientLocation && !Client.HasEffect(EffectsBar.BeagSuain) &&
                !Client.HasEffect(EffectsBar.Pramh) && !Client.HasEffect(EffectsBar.Suain))
            {
                // Unstick logic
                if ((DateTime.UtcNow - _lastUnstick).TotalMilliseconds > 3000)
                {
                    Client.RefreshRequest(false);
                    Client.Walk(RandomUtils.RandomEnumValue<Direction>());
                    _lastUnstick = DateTime.UtcNow;
                    return;
                }
            }

            // Face the target
            Direction direction = target.Location.GetDirection(Client.ClientLocation);
            if (Client.ClientDirection != direction)
                Client.Turn(direction);

            // Initiate bashing
            InitiateBashing();
        }



        private Player FindWhoToProtect(List<Player> users, List<Creature> monsters)
        {
            Player protect1 = Client.ClientTab.Protect1Cbx.Checked
                ? users.FirstOrDefault(u => u.Name.Equals(Client.ClientTab.Protected1Tbx.Text, StringComparison.OrdinalIgnoreCase))
                : null;

            Player protect2 = Client.ClientTab.Protect2Cbx.Checked
                ? users.FirstOrDefault(u => u.Name.Equals(Client.ClientTab.Protected2Tbx.Text, StringComparison.OrdinalIgnoreCase))
                : null;

            return protect1 != null && monsters.Any(mob => mob.IsNear(protect1))
                ? protect1
                : protect2;
        }

        private List<Creature> FilterValidTargets(List<Creature> monsters, Player whoToProtect)
        {
            var filteredPlayers = Client.GetNearbyPlayers()
                .Where(p => p != Client.Player &&
                               !Client.ClientTab.friendList.Items.Contains(p.Name))
                .ToList();

            var validMonsters = monsters
                .Where(mob =>
                    !Client.IsLocationSurrounded(mob.Location) &&                 // Not surrounded
                    !Client.IsWalledIn(mob.Location) &&                          // Not walled in
                    !filteredPlayers.Any(user => user.Location.DistanceFrom(mob.Location) <= 4) &&  // No blocking players nearby
                    !Client.GetNearbyPlayers().Any(user => user.Location == mob.Location) &&      // No users on top
                    PathIsValid(mob) &&                                          // Path exists
                    (whoToProtect == null || mob.IsNear(whoToProtect))           // Protect logic
                )
                .OrderBy(mob =>
                    whoToProtect != null
                        ? mob.Location.DistanceFrom(Client.ClientLocation) / 6.0
                          + mob.Location.DistanceFrom(whoToProtect.Location)
                        : mob.Location.DistanceFrom(Client.ClientLocation))     // Priority adjustment
                .ToList();

            return validMonsters;
        }



        private bool PathIsValid(Creature mob)
        {
            try
            {
                // Use the Pathfinder to find a path to the creature's location
                Stack<Location> path = Client.Pathfinder.FindPath(Client.ServerLocation, mob.Location);

                // Validate the path
                return path.Count > 0 && path.Count < 15;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PathIsValid] Exception: {ex.Message}");
                return false;
            }
        }


        private bool HasNearbyUsersBlocking(Location monsterLocation, int blockingRange = 4)
        {
            // Check for any nearby users blocking the monster's location
            return Client.GetNearbyPlayers().Any(p =>
                p.Location.DistanceFrom(monsterLocation) <= blockingRange || // User is within the blocking range
                p.Location == monsterLocation // User is exactly at the monster's location
            );
        }



        private void SetBashClass()
        {

            BashingBase = null; // Clear any previously set base

            if (Client.PreviousClassFlag == PreviousClass.Pure &&
                Client.TemuairClassFlag == TemuairClass.Warrior &&
                Client.MedeniaClassFlag == (MedeniaClass.Gladiator))
            {
                BashingBase = new PureWarriorBashing(this);
                Console.WriteLine("Activated: PureWarriorTavalyBashing (Warrior + Gladiator).");
            }
            else if (Client.MedeniaClassFlag == MedeniaClass.Druid)
            {
                if (Client.DruidFormFlag == DruidForm.Feral)
                {
                    BashingBase = new FeralBashing(this);
                    Console.WriteLine("Activated: PureFeralTavalyBashing (Druid Feral).");
                }
                else if (Client.DruidFormFlag == DruidForm.Karura)
                {
                    BashingBase = new KaruraBashing(this);
                    Console.WriteLine("Activated: PureKaruraTavalyBashing (Druid Karura).");
                }
            }
            else if (Client.PreviousClassFlag == PreviousClass.Monk &&
                     Client.MedeniaClassFlag == MedeniaClass.Gladiator)
            {
                BashingBase = new MonkWarriorBashing(this);
                Console.WriteLine("Activated: MonkWarriorTavalyBashing (PreviousClass Monk + Gladiator).");
            }
            else if (Client.MedeniaClassFlag == MedeniaClass.Archer)
            {
                BashingBase = new RogueBashing(this);
                Console.WriteLine("Activated: RogueTavalyBashing (Archer).");
            }

            bashClassSet = (BashingBase != null);

        }

        private void EnsureOneLineWalkEnabled()
        {
            if (!Client.ClientTab.oneLineWalkCbox.Checked)
                Client.ClientTab.oneLineWalkCbox.Checked = true;
        }


        private void LogException(string methodName, Exception ex)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bashCrashLogs");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            string filePath = Path.Combine(logPath, $"{DateTime.Now:MM-dd-HH-yyyy_hh-mm-ss}.log");
            File.WriteAllText(filePath, $"{methodName}: {ex}");
        }

        private void InitiateBashing()
        {
            if (_dontBash || target == null || Client.HasEffect(EffectsBar.Pramh))
            {
                Thread.Sleep(50);
                return;
            }

            if (!EnsureWeaponEquipped())
                return;

            HandleNecklaceSwapping();

            // Sync client and server positions if misaligned
            SyncWithServer();

            // If the target isn't at a distance = 1 or the correct direction
            // skip further bashing logic
            if (!IsTargetInRange())
                return;

            UseSkillsOnTarget();
        }

        private bool EnsureWeaponEquipped()
        {
            string equippedWeapName = Client.EquippedItems[1]?.Name;
            bool hasValidWeapon = Client.Weapons.Any(w => equippedWeapName != null && equippedWeapName.Equals(w.Name));

            if (hasValidWeapon)
                return true;

            if (!equipattempted)
            {
                if (!Client.SafeScreen)
                    Client.ServerMessage((byte)ServerMessageType.TopRight, "Equipping Weapon");

                string weaponToEquip = EquipWeaponForClass(Client.TemuairClassFlag);
                WaitForWeaponToEquip(weaponToEquip);
                equipattempted = true;
            }
            else
            {
                ShowWeaponErrorAndStop();
            }

            return false;
        }

        private string EquipWeaponForClass(TemuairClass classFlag)
        {
            return classFlag switch
            {
                TemuairClass.Warrior => Client.EquipGlad(),
                TemuairClass.Rogue => Client.EquipArcher(),
                TemuairClass.Monk => Client.EquipMonk(),
                _ => string.Empty
            };
        }


        private void WaitForWeaponToEquip(string weaponToEquip)
        {
            var timer = Utility.Timer.FromMilliseconds(1500);
            while (!timer.IsTimeExpired && Client.EquippedItems[1]?.Name != weaponToEquip)
            {
                Thread.Sleep(100);
            }
        }

        private void ShowWeaponErrorAndStop()
        {
            MessageBox.Show("A supported weapon is not equipped. Bashing stopped.");
            if (Client.ClientTab?.btnBashingNew.Text == "Stop Bashing")
            {
                Client.ClientTab.btnBashingNew.Text = "Start Bashing";
            }
        }

        private void HandleNecklaceSwapping()
        {
            if (!Client.Map.Name.Contains("Shinewood Forest"))
                return;

            TimeSpan sinceLastSwap = DateTime.UtcNow.Subtract(neckSwap);

            if (CONSTANTS.SHINEWOOD_HOLY.Contains(target.SpriteID) && Client.OffenseElement != "Light" && sinceLastSwap.TotalSeconds > 2.0 && !autoForm)
            {
                SwapNecklace("Light");
            }
            else if (CONSTANTS.SHINEWOOD_DARK.Contains((int)target.SpriteID) && Client.OffenseElement != "Dark" && sinceLastSwap.TotalSeconds > 2.0 && !autoForm)
            {
                SwapNecklace("Dark");
            }
        }

        private void SwapNecklace(string element)
        {
            autoForm = true;
            RemoveDruidForm();

            if (element == "Light")
                Client.EquipLightNeck();
            else if (element == "Dark")
                Client.EquipDarkNeck(); 

            neckSwap = DateTime.UtcNow;
            EnterDruidForm();
            autoForm = false;
        }
        private void EnterDruidForm()
        {
            if (!Client.ClientTab.druidFormCbox.Checked)
                return;

            if (Client.TemuairClassFlag == TemuairClass.Monk &&
                Client.MedeniaClassFlag == MedeniaClass.Druid &&
                !Client.HasEffect(EffectsBar.FeralForm) &&
                !Client.HasEffect(EffectsBar.KaruraForm) &&
                !Client.HasEffect(EffectsBar.KomodasForm))
            {
                foreach (var spell in CONSTANTS.DRUID_FORMS)
                {
                    if (Client.UseSpell(spell, null, _autoStaffSwitch))
                        break;
                }
            }

            Thread.Sleep(80);
        }

        private void RemoveDruidForm()
        {
            if (!Client.ClientTab.druidFormCbox.Checked)
                return;

            if (Client.TemuairClassFlag == TemuairClass.Monk &&
                Client.MedeniaClassFlag == MedeniaClass.Druid &&
                Client.HasEffect(EffectsBar.FeralForm) ||
                Client.HasEffect(EffectsBar.KaruraForm) ||
                Client.HasEffect(EffectsBar.KomodasForm))
            {
                foreach (var spell in CONSTANTS.DRUID_FORMS)
                {
                    if (Client.UseSpell(spell, null, _autoStaffSwitch))
                        break;
                }
            }

            Thread.Sleep(80);
        }
        private void SyncWithServer()
        {
            if (Client.ClientLocation != Client.ServerLocation)
            {
                if (!Client.SafeScreen)
                    Client.ServerMessage((byte)ServerMessageType.TopRight, "Syncing with server");

                TimeSpan sinceLastSync = DateTime.UtcNow.Subtract(LastPointInSync);

                if (sinceLastSync.TotalMilliseconds > 1000.0)
                {
                    Client.RefreshRequest(false);
                    Client.ClientDirection = Client.ServerDirection;
                    LastPointInSync = DateTime.UtcNow;
                    LastDirectionInSync = DateTime.UtcNow;

                    Direction targetDirection = target.Location.GetDirection(Client.ClientLocation);
                    if (targetDirection != Client.ClientDirection)
                        Client.Turn(targetDirection);
                }
            }
            else
            {
                LastPointInSync = DateTime.UtcNow;
            }
        }

        private bool IsTargetInRange()
        {
            int? distanceToTarget = target?.Location.DistanceFrom(Client.ClientLocation);
            Direction? targetDirection = target?.Location.GetDirection(Client.ClientLocation);

            return distanceToTarget == 1 && targetDirection == Client.ClientDirection;
        }

        private void UseSkillsOnTarget()
        {
            if (Client.ClientTab.chkFrostStrike.Checked && target.HealthPercent > 30)
            {
                Client.UseSkill("Frost Strike");
            }

            if (Client.ClientTab.chkBashAssails.Checked && !bashWffUsed)
            {
                Client.Assail();
            }

            if (Client.ClientTab.chkBashDion.Checked)
            {
                CrasherDionTargets();
            }

            if (!ShouldUseSkills())
                return;

            UseBashingSkills();

        }

        private bool ShouldUseCrasher()
        {
            // Use the configured health threshold if available, otherwise fallback to 60
            int healthThreshold = Client.ClientTab.chkCrasherAboveHP.Checked
                ? (int)Client.ClientTab.numCrasherHealth.Value
                : 60;

            // Ensure health is above the threshold and user is Dioned
            if (target.HealthPercent < healthThreshold || !Client.Player.IsDioned)
                return false;

            // Check if any of the skills are available and usable
            string[] skills = { "Crasher", "Execute", "Animal Feast" };
            if (skills.Any(skill => Client.HasSkill(skill) && Client.CanUseSkill(Client.Skillbook[skill])))
                return true;

            // Check if the Damage Scroll item is available and usable
            if (Client.HasItem("Damage Scroll"))
            {
                if (Client.CanUseItem(Client.Inventory["Damage Scroll"]) &&
                    DateTime.UtcNow.Subtract(Client.Inventory["Damage Scroll"].LastUsed).TotalSeconds > 28)
                {
                    {
                        return true;
                    }

                }
            }

            return false;
        }
        private void CrasherDionTargets()
        {
            if (!ShouldUseCrasher())
                return;

            if (Client.ClientTab.chkCrasherAboveHP.Checked &&
                Client.HealthPct > Client.ClientTab.numCrasherHealth.Value &&
                (!target.IsAsgalled || Client.ClientTab.chkCrasherOnlyAsgall.Checked))
            {
                ExecuteCrasher();
            }

        }
        private void ExecuteCrasher()
        {
            Client.UseSkill("Sacrifice");
            Client.UseSkill("Mad Soul");
            Client.UseSkill("Auto Hemloch");
            Client.UseSkill("Crasher");
            Client.UseSkill("Animal Feast");
            Client.UseSkill("Execute");
            Client.UseItem("Damage Scroll");
            skillUse = DateTime.UtcNow;
        }




        private bool ShouldUseSkills()
        {
            bool waitForCradh = Client.ClientTab.chkWaitForCradh.Checked;
            bool waitForFas = Client.ClientTab.chkWaitForFas.Checked;

            // Check conditions for Cradh and Fas
            bool cradhReady = !waitForCradh || target.IsCursed;
            bool fasReady = !waitForFas || target.IsFassed;

            // Ensure the target is in the correct direction and the skill delay has passed
            bool isDirectionAligned = Client.ClientDirection == target.Location.GetDirection(Client.ClientLocation);
            bool isSkillDelayElapsed = DateTime.UtcNow.Subtract(skillUse).TotalMilliseconds > (double)(Client.ClientTab?.numBashSkillDelay.Value ?? 0);

            return cradhReady && fasReady && isDirectionAligned && isSkillDelayElapsed;
        }

        private void UseBashingSkills()
        {
            foreach (string skillName in Client.ClientTab._bashingSkillList)
            {
                if (skillName.Equals("Sprint Potion", StringComparison.OrdinalIgnoreCase) && Client.IsRegistered)
                {
                    if ((DateTime.UtcNow - _sprintPotionLastUsed).TotalSeconds > 16.0)
                    {
                        if (!Client.SafeScreen)
                            Client.ServerMessage((byte)ServerMessageType.TopRight, $"Using {skillName}");
                        Client.UseItem(skillName);
                        _sprintPotionLastUsed = DateTime.UtcNow;
                        return;
                    }
                }

                if (skillName.Equals("Two Move Combo", StringComparison.OrdinalIgnoreCase))
                {
                    if ((DateTime.UtcNow - Client.ComboScrollLastUsed).TotalMinutes > 1.0 && Client.CurrentMP > 1500U)
                    {
                        if (!Client.SafeScreen)
                            Client.ServerMessage((byte)ServerMessageType.TopRight, $"Using {skillName}");
                        Client.UseItem(skillName);
                        Client.ComboScrollLastUsed = DateTime.UtcNow;
                        return;
                    }
                }

                if (skillName.Equals("Three Move Combo", StringComparison.OrdinalIgnoreCase))
                {
                    if ((DateTime.UtcNow - Client.ComboScrollLastUsed).TotalMinutes > 2.0 && Client.CurrentMP > 1500U)
                    {
                        if (!Client.SafeScreen)
                            Client.ServerMessage((byte)ServerMessageType.TopRight, $"Using {skillName}");
                        Client.UseItem(skillName);
                        Client.ComboScrollLastUsed = DateTime.UtcNow;
                        return;
                    }
                }

                if (!Client.Skillbook.SkillbookDictionary.TryGetValue(skillName, out var skill)
                    || !skill.CanUse)
                {
                    // If we don't have it or can't use it, continue
                    continue;
                }

                // For distance-based sets like fiveTileAttacksMed, threeTileAttacksMed, twoTileAttacksMed
                int distance = target.Location.DistanceFrom(Client.ClientLocation);

                // --- PF Skills (e.g., "Paralyze Force") or "Perfect Defense" or other special conditions ---
                bool isPFSkill = CONSTANTS.PF_SKILLS.Contains(skillName);

                List<Creature> nearbyMobs = Client.GetNearbyValidCreatures(8)
                    .Where(m => !Client.IsLocationSurrounded(m.Location))
                    .Where(m => !Client.Map.IsWall(m.Location))
                    .ToList();

                // Example: "PF" usage if aligned, and min mob count is met
                if (isPFSkill)
                {
                    // Make sure direction is aligned (like original checks)
                    if (Client.ClientLocation.GetDirection(target.Location) == target.Location.GetDirection(Client.ClientLocation))
                    {
                        // Check if we need a certain mob count to use PF
                        if (nearbyMobs.Count >= (int)Client.ClientTab.numPFCounter.Value)
                        {
                            // If skill is "Paralyze Force," check last use < 27 ms or so
                            if (skillName.Equals("Paralyze Force", StringComparison.OrdinalIgnoreCase))
                            {
                                TimeSpan sinceLastUse = DateTime.UtcNow - skill.LastUsed;
                                if (sinceLastUse.TotalMilliseconds < 27.0)
                                    continue;
                            }

                            if (!Client.SafeScreen)
                                Client.ServerMessage((byte)ServerMessageType.TopRight, $"Using {skillName}");
                            Client.UseSkill(skillName);
                            skillUse = DateTime.UtcNow;
                            return;
                        }
                    }
                    // If PF conditions fail, continue to next skill
                    continue;
                }

                if (skillName.Equals("Special Arrow Attack", StringComparison.OrdinalIgnoreCase))
                {
                    TimeSpan timeSinceLastUse = DateTime.UtcNow - skill.LastUsed;
                    if (timeSinceLastUse.TotalMilliseconds < 11.0)
                        continue;
                }

                if (skillName.Equals("Perfect Defense", StringComparison.OrdinalIgnoreCase))
                {
                    if (nearbyMobs.Count <= (int)Client.ClientTab.numPFCounter.Value)
                    {
                        if (!Client.SafeScreen)
                            Client.ServerMessage((byte)ServerMessageType.TopRight, $"Using {skillName}");
                        Client.UseSkill(skillName);
                        skillUse = DateTime.UtcNow;
                        return;
                    }
                    // else, continue
                    continue;
                }

                // Sneak Flight or Ambush type logic: Cancel if a wall is detected
                if (skillName.StartsWith("Sneak Flight", StringComparison.OrdinalIgnoreCase)
                    && Client.Map.IsWall(target.Location))
                {
                    if (!Client.SafeScreen)
                        Client.ServerMessage((byte)ServerMessageType.TopRight, "Wall Detected: Cancel Ambush");
                    continue;
                }

                // Wolf Fang Fist / Lullaby Punch logic 
                if (skillName.Equals("Wolf Fang Fist", StringComparison.OrdinalIgnoreCase) ||
                    skillName.Equals("Lullaby Punch", StringComparison.OrdinalIgnoreCase))
                {
                    if (!Client.SafeScreen)
                        Client.ServerMessage((byte)ServerMessageType.TopRight, $"Using {skillName}");
                    Client.UseSkill(skillName);
                    bashWffUsed = true;
                    skillUse = DateTime.UtcNow;
                    return;
                }

                // Distance-based checks for 5-, 3-, 2-tile attacks
                bool inRange = false;

                // Check if the skill is in any of the tile-based dictionaries
                bool isTileBasedSkill = CONSTANTS.FIVE_TILE_ATTACKS_MED.Contains(skillName) ||
                                        CONSTANTS.THREE_TILE_ATTACKS_MED.Contains(skillName) ||
                                        CONSTANTS.TWO_TILE_ATTACKS_MED.Contains(skillName);

                if (isTileBasedSkill)
                {
                    if (distance <= 5 && CONSTANTS.FIVE_TILE_ATTACKS_MED.Contains(skillName))
                        inRange = true;
                    else if (distance <= 3 && CONSTANTS.THREE_TILE_ATTACKS_MED.Contains(skillName))
                        inRange = true;
                    else if (distance <= 2 && CONSTANTS.TWO_TILE_ATTACKS_MED.Contains(skillName))
                        inRange = true;

                    if (!inRange)
                        continue; // Skip the skill if it's a tile-based skill but not in range
                }

                // If we reached here, we can use the skill normally

                if (!Client.SafeScreen)
                    Client.ServerMessage((byte)ServerMessageType.TopRight, $"Using {skillName}");
                Client.UseSkill(skillName);
                skillUse = DateTime.UtcNow;
                bashWffUsed = false;
   
                continue;
            }
        }

        #endregion

        #region TaskLoop
        private void TaskLoop()
        {
            Ascending();
        }

        private void Ascending()
        {
            if (Client.ClientTab.ascendBtn.Text != "Ascending")
                return;

            if (!DepositWarBagIfNeeded()) return;
            if (!DropSuccubusHair()) return;
            if (!HandleKillerOrDieOption()) return;
            if (!HandleGhostWalk()) return;
            if (!AscendHpIfNeeded()) return;
            if (!AscendMpIfNeeded()) return;
            if (!RetrieveWarBagIfNeeded()) return;

            Thread.Sleep(100);
        }


        private bool DepositWarBagIfNeeded()
        {
            // Check if the task is already done
            if (Client.WarBagDeposited)
                return true;

            if (!Client.Inventory.Contains("Warranty Bag") || Client.AscendTaskDone)
                return false;

            if (hasrepaired)
                hasrepaired = false;

            if (Client.Map.MapID == 135) // Mileth Storage
            {
                Location current = Client.ServerLocation;
                if (current.DistanceFrom(new Location(135, 6, 6)) <= 3)
                {
                    Creature npc = Client.GetNearbyNPCs()
                        .OrderBy(n => n.Location.DistanceFrom(Client.ServerLocation))
                        .FirstOrDefault();

                    if (npc != null)
                    {
                        if (npc.Location.DistanceFrom(Client.ServerLocation) <= 12)
                        {
                            if (!hasrepaired)
                            {
                                Client.PursuitRequest((byte)1, npc.ID, 92);
                                Thread.Sleep(1000);
                                Client.WithdrawItem(npc.ID, "Succubus's Hair", 1);
                                Thread.Sleep(1000);

                                int komCount = 52 - Client.Inventory.CountOf("Komadium");
                                if (komCount > 0 && komCount <= 51)
                                {
                                    Client.WithdrawItem(npc.ID, "Komadium", komCount);
                                    Thread.Sleep(1000);
                                }

                                int exkuraCount = 30 - Client.Inventory.CountOf("Exkuranum");
                                if (exkuraCount > 0 && exkuraCount <= 30)
                                {
                                    Client.WithdrawItem(npc.ID, "Exkuranum", exkuraCount);
                                    Thread.Sleep(1000);
                                }

                                int scrollCount = 30 - Client.Inventory.CountOf("Rucesion Song");
                                if (scrollCount > 0 && scrollCount <= 30)
                                {
                                    Client.WithdrawItem(npc.ID, "Rucesion Song", scrollCount);
                                    Thread.Sleep(1000);
                                }

                                int hemCount = 30 - Client.Inventory.CountOf("Hemloch");
                                if (hemCount > 0 && hemCount <= 30)
                                {
                                    Client.WithdrawItem(npc.ID, "Hemloch", hemCount);
                                    Thread.Sleep(1000);
                                }
                            }

                            // Deposit each war bag we find in inventory
                            foreach (Item obj in Client.Inventory)
                            {
                                if (obj.Name.Equals("Warranty Bag"))
                                {
                                    Client.DepositItem(npc.ID, "Warranty Bag");
                                    Thread.Sleep(1000);
                                }
                            }

                            // Wait up to 5 seconds for the bag to disappear
                            DateTime start = DateTime.UtcNow;
                            while (Client.Inventory.Contains("Warranty Bag"))
                            {
                                Thread.Sleep(10);
                                var timeSpan = DateTime.UtcNow.Subtract(start);
                                if (timeSpan.TotalSeconds > 5.0)
                                {
                                    Client.ClientTab.Invoke(new Action(() => Client.ClientTab.ascendBtn.Text = "Ascend"));
                                    MessageDialog.Show(Server.MainForm, "Could not deposit warranty bag. Error");
                                    break;
                                }
                            }

                            Client.WarBagDeposited = true;
                            Client.AscendTaskDone = false;
                            hasrepaired = true;

                            return true;
                        }
                    }
                    // Merchant not found or too far
                    Client.ServerMessage((byte)ServerMessageType.Whisper, "You need a merchant nearby to use this command.");
                    return false;
                }
            }

            // Route to Mileth storage
            Client.Routefind(new Location(135, 6, 6), 2);
            return false; 
        }
        private bool DropSuccubusHair()
        {
            Client.SuccHairDropped = true;

            if (Client.SuccHairDropped)
                return true;

            // We are ghost walking
            if (Client.Map.MapID == 435 ||
               Client.Map.MapID == 3085 ||
               Client.Map.MapID == 3086 ||
               Client.Map.MapID == 3087)
                return false;


            if (Client.EffectsBar.Contains((ushort)EffectsBar.Skull) || Client.AscendTaskDone)
                return false;

            // If we don’t have succubus hair, withdraw from Mileth
            if (!Client.Inventory.Contains("Succubus's Hair"))
            {
                if (Client.Map.MapID == 135) // Mileth Storage
                {
                    Location current = Client.ServerLocation;
                    if (current.DistanceFrom(new Location(135, 6, 6)) <= 3)
                    {
                        Creature creature = Client.GetNearbyNPCs()
                            .OrderBy(n => n.Location.DistanceFrom(Client.ServerLocation))
                            .FirstOrDefault();

                        if (creature != null &&
                            creature.Location.DistanceFrom(Client.ServerLocation) <= 12)
                        {
                            Client.PursuitRequest((byte)1, creature.ID, 92);
                            Thread.Sleep(1000);
                            Client.WithdrawItem(creature.ID, "Succubus's Hair", 1);
                            Client.Dialog?.Reply();

                            DateTime start = DateTime.UtcNow;
                            while (!Client.Inventory.Contains("Succubus's Hair"))
                            {
                                Thread.Sleep(10);
                                var timeSpan = DateTime.UtcNow.Subtract(start);
                                if (timeSpan.TotalSeconds > 5.0)
                                {
                                    Client.ClientTab.ascendBtn.Text = "Ascend";
                                    MessageDialog.Show(Server.MainForm,
                                        "Succubus Hair retreival failed.");
                                    break;
                                }
                            }

                            return false;
                        }

                        Client.ServerMessage((byte)ServerMessageType.Whisper, "You need a merchant nearby to use this command.");
                        return false;
                    }
                }
                Client.Routefind(new Location(135, 6, 6), 3);
                return false;
            }
            
            if (Client.Map.MapID == 500 && CONSTANTS.MILETH_ALTAR_SPOTS.Keys.Contains(Client.ServerLocation))
            {
                // Drop the succubus hair
                Client.Drop(
                    Client.Inventory["Succubus's Hair"].Slot,
                    CONSTANTS.MILETH_ALTAR_SPOTS[Client.ServerLocation]
                );
                Thread.Sleep(1500);
                Client.Dialog?.Reply();
                Thread.Sleep(100);

                // If we still don’t have skull, refresh and return
                if (!Client.EffectsBar.Contains((ushort)EffectsBar.Skull))
                {
                    Client.RefreshRequest();
                    return true;
                }

                // If we do have skull, possibly warp back (if Rucesion Song)
                var ascendNames = Server.MainForm.AutoAscendDataList
                    .Where(d => d.ContainsKey("Name"))
                    .Select(d => d["Name"].ToString());

                if (ascendNames.Contains(Client.Name) && Client.HasItem("Rucesion Song"))
                    Client.UseItem("Rucesion Song");

                Thread.Sleep(150);
                Client.SuccHairDropped = true;
                return true;
            }


            // Route to a random unoccupied altar spot
            var nextPoint = CONSTANTS.MILETH_ALTAR_SPOTS
                .OrderBy(_ => RandomUtils.Random())
                .FirstOrDefault(kvp =>
                    !Client.GetNearbyPlayers().Any(p => p.Location == kvp.Key)
                ).Key;

            Client.Routefind(new Location(500, nextPoint.X, nextPoint.Y));
            return false;

        }
        private bool HandleKillerOrDieOption()
        {
            // We are ghost walking
            if (Client.Map.MapID == 435 ||
                Client.Map.MapID == 3085 ||
                Client.Map.MapID == 3086 ||
                Client.Map.MapID == 3087)
                return true;

            // Check if "Killer" option is enabled
            if (Client.ClientTab.useKillerCbx.Checked)
            {
                string killerName = Client.ClientTab.killerNameTbx.Text;
                Client killer = Server.GetClient(killerName);

                if (killer == null)
                {
                    SystemSounds.Beep.Play();
                    Thread.Sleep(1000);
                    Client.ServerMessage((byte)ServerMessageType.Whisper, "Killer not found.");
                    return false;
                }

                Location killerLocation = killer.ServerLocation;

                if (Client.Map.MapID == killerLocation.MapID)
                {
                    Location current = Client.ServerLocation;
                    if (current.DistanceFrom(killerLocation) <= 6)
                    {
                        // Attack logic
                        if (Client.CurrentHP > 1U && !Client.UseSkill("Auto Hemloch"))
                        {
                            if (Client.Inventory.Contains("Hemloch"))
                                Client.UseItem("Hemloch");
                            Thread.Sleep(2000);
                        }

                        foreach (Client c in Server.ClientList)
                        {
                            if (c.Name == killerName)
                            {
                                if (c.HasSpell("mor strioch pian gar"))
                                {
                                    if (c.ManaPct < 50)
                                    {
                                        c.UseSpell("fas spiorad", null, true, false);
                                    }
                                    c.UseSpell("mor strioch pian gar", null, true, false);
                                }
                            }
                        }

                        return true; // Task completed
                    }
                }

                Client.Routefind(killerLocation); // Route to the killer
                return false;
            }

            // Check if "Die at PC" option is enabled
            if (Client.ClientTab.deathOptionCbx.Checked)
            {
                if (Client.ServerLocation.DistanceFrom(_pcDeathSpot) <= 2)
                    return true; // Task completed

                Client.Routefind(_pcDeathSpot, 2); // Route to the PC death spot
                return false;
            }

            // Neither option is enabled
            return true;
        }


        private bool HandleGhostWalk()
        {
            bool ascendHP = Client.ClientTab.ascendOptionCbx.Text == "HP";

            if (Client.Map.MapID == 435)
            {
                if (Client.ServerLocation.X < 6)
                    Client.Pathfind(new Location(435, 4, 20), 0);
                else
                    Client.Pathfind(new Location(435, 6, 23), 0);
                return true;
            }
            else if (Client.Map.MapID == 3085)
            {
                if (ascendHP)
                    Client.Pathfind(new Location(3085, 10, 14), 0);
                else
                    Client.Pathfind(new Location(3085, 10, 5), 0);
                return true;
            }


            return true;

        }
        private bool AscendHpIfNeeded()
        {
            if (Client.Map.MapID != 3086)
                return false;

            Location altarSpot = new Location(3086, 5, 2);
            Location current = Client.ServerLocation;

            if (current.DistanceFrom(altarSpot) > 2)
            {
                Client.Pathfind(altarSpot);
                return false;
            }

            // If within range
            Client.RefreshRequest();
            current = Client.ServerLocation;
            if (current.DistanceFrom(altarSpot) > 2)
                return false;

            Thread.Sleep(2500);
            commandManager.ExecuteCommand(Client, "/hp all");

            // If WarBag not deposited, set ascendBtn back and possibly complete ascending
            if (!Client.WarBagDeposited)
            {
                Client.ClientTab.Invoke(new Action(() => Client.ClientTab.ascendBtn.Text = "Ascend"));

                var ascendNames = Server.MainForm.AutoAscendDataList
                    .Where(d => d.ContainsKey("Name"))
                    .Select(d => d["Name"].ToString())
                    .ToList();

                if (ascendNames.Contains(Client.Name))
                    Server.ClientStateList[Client.Name] = CharacterState.AscendingComplete;
            }

            Client.AscendTaskDone = true;
            return true;
        }
        private bool AscendMpIfNeeded()
        {
            if (Client.Map.MapID != 3087)
                return false;

            Location altarSpot = new Location(3087, 5, 2);
            Location current = Client.ServerLocation;

            if (current.DistanceFrom(altarSpot) > 2)
            {
                Client.Pathfind(altarSpot);
                return false;
            }

            Client.RefreshRequest();
            current = Client.ServerLocation;
            if (current.DistanceFrom(altarSpot) > 2)
                return false;

            Thread.Sleep(2500);
            commandManager.ExecuteCommand(Client, "/mp all");

            if (!Client.WarBagDeposited)
            {
                Client.ClientTab.Invoke(new Action(() => Client.ClientTab.ascendBtn.Text = "Ascend"));

                var ascendNames = Server.MainForm.AutoAscendDataList
                    .Where(d => d.ContainsKey("Name"))
                    .Select(d => d["Name"].ToString());

                if (ascendNames.Contains(Client.Name))
                    Server.ClientStateList[Client.Name] = CharacterState.AscendingComplete;
            }

            Client.AscendTaskDone = true;
            return true;
        }
        private bool RetrieveWarBagIfNeeded()
        {
            // Original check: if WarBagDeposited && AscendTaskDone
            if (!Client.WarBagDeposited || !Client.AscendTaskDone)
                return false;

            // If we get here, we need to retrieve the bag
            if (Client.Map.MapID == 167) // Abel storage
            {
                Location current = Client.ServerLocation;
                if (current.DistanceFrom(new Location(167, 3, 8)) <= 2)
                {
                    Creature creature = Client.GetNearbyNPCs()
                        .OrderBy(n => n.Location.DistanceFrom(Client.ServerLocation))
                        .FirstOrDefault();

                    if (creature != null && creature.Location.DistanceFrom(Client.ServerLocation) <= 12)
                    {
                        Client.PublicMessage((byte)PublicMessageType.Chant, "Give my Warranty Bag back");
                        Client.Dialog?.Reply();

                        DateTime start = DateTime.UtcNow;
                        while (!Client.Inventory.Contains("Warranty Bag"))
                        {
                            Client.PublicMessage((byte)PublicMessageType.Chant, "Give my Warranty Bag back");
                            Thread.Sleep(500);

                            var timeSpan = DateTime.UtcNow.Subtract(start);
                            if (timeSpan.TotalSeconds > 5.0)
                            {
                                Client.ClientTab.Invoke(new Action(() => Client.ClientTab.ascendBtn.Text = "Ascend"));
                                MessageDialog.Show(Server.MainForm, "Couldn't retrieve warranty bag. Error");
                                break;
                            }
                        }

                        // Once done, set ascendBtn to Ascend
                        Client.ClientTab.Invoke(new Action(() => Client.ClientTab.ascendBtn.Text = "Ascend"));

                        var ascendNames = Server.MainForm.AutoAscendDataList
                            .Where(d => d.ContainsKey("Name"))
                            .Select(d => d["Name"].ToString());

                        if (ascendNames.Contains(Client.Name))
                            Server.ClientStateList[Client.Name] = CharacterState.AscendingComplete;

                        // We are done
                        return true;
                    }

                    Client.ServerMessage((byte)ServerMessageType.Whisper,
                        "You need a merchant nearby to use this command.");
                    return false;
                }
            }
            // Otherwise route to Abel storage
            Client.Routefind(new Location(167, 3, 8), 1);
            return false;
        }

        #endregion



        private void CheckScrollTimers()
        {
            if (Client.ComboScrollCounter > 0)
            {
                TimeSpan remainingTime = GetScrollRemainingTime();
                if (remainingTime < TimeSpan.FromSeconds(1))
                {
                    Client.ComboScrollCounter = 0;
                }
                else if (!Client.SafeScreen)
                {
                    Client.ServerMessage((byte)ServerMessageType.TopRight, $"Scroll in: {remainingTime:m\\:ss}");
                }
            }
            else if (Client.MedeniaClassFlag == MedeniaClass.Druid && !Client.SafeScreen)
            {
                Client.ServerMessage((byte)ServerMessageType.TopRight, "Scroll Ready.");
            }
        }

        private TimeSpan GetScrollRemainingTime()
        {
            int scrollMinutes = (Client.ComboScrollCounter == (Client.PreviousClassFlag == PreviousClass.Pure ? 3 : 2)) ? 2 : 1;
            TimeSpan totalScrollTime = new TimeSpan(0, scrollMinutes, 2);
            return totalScrollTime - (DateTime.UtcNow - Client.ComboScrollLastUsed);
        }


        private void HolidayEvents()
        {
            RetrieveDoubles();
        }

        private void RetrieveDoubles()
        {
            {
                int targetMapID = 3271;
                Location targetLocation = new Location(3271, 43, 58);
                string npcName = "Frosty3";



                // Logic to retrieve doubles
                if (Client.ClientTab != null && Client.ClientTab.toggleSeaonalDblBtn.Text == "Disable")
                {

                    // Adjust based on the calendar month
                    switch (DateTime.Now.Month)
                    {
                        case 12: // December
                            targetMapID = 3271;
                            targetLocation = new Location(3271, 43, 58);
                            npcName = "Frosty3";
                            break;

                        case 2: // February
                            targetMapID = 3043;
                            targetLocation = new Location(3043, 20, 24);
                            npcName = "Aidan";
                            break;

                        default:
                            Console.WriteLine("[HolidayEvents] Cannot retrieve doubles during this month.");
                            Client.ServerMessage((byte)ServerMessageType.Whisper, "Cannot retrieve doubles during this month.");
                            Client.ClientTab.toggleSeaonalDblBtn.Text = "Enable";
                            return;
                    }

                    if (!_receivedDblBonus)
                    {
                        // Check if we are on the target map and close to the target location
                        if (Client.Map.MapID == targetMapID && Client.ServerLocation.DistanceFrom(targetLocation) <= 2)
                        {
                            Creature creature = Client.GetNearbyNPC(npcName);
                            if (creature != null)
                            {
                                Console.WriteLine($"[HolidayEvents] Retrieved NPC Name: {creature.Name}, ID: {creature.ID}");
                                Console.WriteLine($"[HolidayEvents] Clicking creature");
                                Client.ClickObject(creature.ID);
                            }
                            else
                            {
                                Console.WriteLine($"[HolidayEvents] Creature was null");
                                return;
                            }
                            Thread.Sleep(1000);
                            Console.WriteLine($"[HolidayEvents] Hitting escape key");
                            Client.EscapeKey();
                            Console.WriteLine($"[HolidayEvents] Setting boolean to true");
                            _receivedDblBonus = true;
                            Console.WriteLine($"[HolidayEvents] Sleeping for 1 s");
                            Thread.Sleep(1000);
                        }
                        else // We need to walk to the target location
                        {
                            Console.WriteLine($"[HolidayEvents] Routing to target location: {targetLocation}");
                            Client.Routefind(targetLocation, 0, false, true, true);
                            return;
                        }
                    }
                    else
                    {
                        // Move to the next stage based on the map and location
                        if (Client.Map.MapID != 6925)
                        {
                            Console.WriteLine($"[HolidayEvents] Routing to Loures Harbor");
                            Client.Routefind(new Location(6925, 41, 4), 0, false, true, true);
                        }
                        else
                        {
                            if (Client.ServerLocation.DistanceFrom(new Location(6925, 41, 4)) <= 2)
                            {
                                Console.WriteLine($"[HolidayEvents] We are done!");
                                Client.ClientTab.toggleSeaonalDblBtn.Text = "Enable";
                                SystemSounds.Beep.Play();
                                Client.FlashWindowEx(Process.GetProcessById(Client.processId).MainWindowHandle);
                            }
                            else
                            {
                                Console.WriteLine($"[HolidayEvents] Moving closer to (41, 4)");
                                Client.Routefind(new Location(6925, 41, 4), 0, false, true, true);
                            }
                        }
                    }
                }
            }
        }
        private void TavalyWallHacks()
        {
            if (Client.ClientTab.chkTavWallStranger.Checked && IsStrangerNearby() && Client.ClientTab.chkTavWallHacks.Checked && !Client.Map.IsWall(Client.ServerLocation))
            {
                Client.ClientTab.chkTavWallHacks.Checked = false;
                Client.RefreshRequest();
            }
            if (Client.ClientTab.chkTavWallStranger.Checked && !IsStrangerNearby() && !Client.ClientTab.chkTavWallHacks.Checked)
            {
                Client.ClientTab.chkTavWallHacks.Checked = true;
                Client.RefreshRequest();
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
            return Client.GetNearbyPlayers().Any(player => IsNotInFriendList(player));
        }

        private bool IsNotInFriendList(Player player)
        {
            var clientTab = Client.ClientTab;

            if (clientTab != null || Client.ClientTab != null)
            {
                return !clientTab.friendList.Items.OfType<string>().Any(friend => string.Equals(friend, player.Name, StringComparison.OrdinalIgnoreCase));
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
                if (!Client.ExchangeOpen && Client.ClientTab != null)
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
            var clientTab = Client.ClientTab;
            if (clientTab == null || Client.ClientTab == null)
            {
                return;
            }

            _nearbyPlayers = Client.GetNearbyPlayers();
            _nearbyValidCreatures = Client.GetNearbyValidCreatures(12);
            var shouldWalk = !_dontWalk &&
                (!clientTab.rangerStopCbox.Checked || !_rangerNear);

            if (shouldWalk)
            {
                HandleWalkingCommand();
            }

            //var end = DateTime.UtcNow;
            //Console.WriteLine($"WalkActions ended at {end:HH:mm:ss.fff}, Duration: {(end - start).TotalMilliseconds} ms");
        }

        private void HandleWalkingCommand()
        {
            var clientTab = Client.ClientTab;
            if (clientTab == null || Client.ClientTab == null)
            {
                return;
            }

            string comboBoxText = clientTab.walkMapCombox.Text;
            bool followChecked = clientTab.followCbox.Checked;
            string followName = clientTab.followText.Text;

            if (followChecked && !string.IsNullOrEmpty(followName))
            {
                FollowWalking(followName);
            }
            else if (clientTab.walkBtn.Text == "Stop")
            {
                RefreshLastStep();

                if (comboBoxText == "SW Lure")
                {
                    SWLure();
                    return;
                }
                else if (comboBoxText == "WayPoints")
                {
                    WayPointWalking();
                    return;
                }

                // Determine the route target from comboBoxText
                if (TryGetRouteTarget(comboBoxText, out short mapID, out Location destination))
                {
                    // Handle extra map actions if needed
                    HandleExtraMapActions(destination);

                    // Update the walk button or directly route
                    if (mapID > 0)
                    {
                        Client.RouteFindByMapID(mapID);
                    }
                    else if (destination != default)
                    {
                        UpdateWalkButton(destination);
                    }
                }
                else
                {
                    UpdateWalkButton(destination);
                }

            }
        }
        private void HandleExtraMapActions(Location destination)
        {

            if (Client.Map == null)
                return;

            // Extracting for readability
            var currentMapID = Client.Map.MapID;
            var currentLocation = Client.ServerLocation;

            // First, handle specific cases triggered by the current map ID
            if (HandleMapSpecificActions(currentMapID, currentLocation))
            {
                return; // If handled, stop
            }

            // Next, handle cases based on the destination map ID
            if (HandleDestinationSpecificActions(destination))
            {
                return;
            }
        }
        private bool HandleMapSpecificActions(int currentMapID, Location currentLocation)
        {
            switch (currentMapID)
            {
                case 6525: // Oren Island Ruins0
                    return HandleOrenIslandRuins0(currentLocation);

                default:
                    return false; // Not handled here
            }
        }

        private bool HandleDestinationSpecificActions(Location destination)
        {
            switch (destination.MapID)
            {
                case 424: // Rucesion Black Market
                    return HandleBlackMarket();

                case 3012: // Loures Castle Way
                    return HandleLouresCastleWay();

                case 3938: // Loures Storage 12, to wait outside Canals
                    return HandleLouresStore12();

                case 3950: // Gladiator Arena Entrance
                    return HandleGladiatorArenaEntrance();

                case 6534: // Oren Ruins 2-1 to wait ouside 2-5
                    return HandleOrenRuinsWalkTo2dash5();

                case 6537: // Oren Ruins 2-4 to wait ouside 2-11
                    return HandleOrenRuinsWalkTo2dash11();

                case 6538: // Oren Ruins 3-1 to wait ouside 3-5
                    return HandleOrenRuinsWalkTo3dash5();

                case 6541: // Oren Ruins 3-4 to wait ouside 3-11
                    return HandleOrenRuinsWalkTo3dash11();

                case 10265: // Hwarone City, to wait outside Fire Canyon
                    return HandleFireCanyon();

                default:
                    return false; // Not handled here
            }
        }

        // Below are the helper methods for each map or destination.
        // Each returns true if it handled the action (meaning it performed
        // some routing or updated the UI and then returned early).
        private bool HandleOrenIslandRuins0(Location currentLocation)
        {
            if (currentLocation.AbsoluteXY(32, 23) > 2)
            {
                Client.Routefind(new Location(6525, 32, 23), 0, false, true, true);
                return true;
            }
            else
            {
                Client.PublicMessage(3, "Welcome Aisling");
                Thread.Sleep(500);
                return true;
            }
        }
        private bool HandleLouresStore12()
        {
            if (Client.Map.MapID == 3938)
            {
                if (Client.ClientLocation.AbsoluteXY(7, 13) > 2)
                {
                    Client.Pathfind(new Location(3938, 7, 13), 0, true, false);
                    return true;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                    return true;
                }
            }
            else
                return false;
        }
        private bool HandleGladiatorArenaEntrance()
        {
            if (Client.Map.MapID == 3950)
            {
                if (Client.ClientLocation.AbsoluteXY(13, 12) > 2)
                {
                    Client.Pathfind(new Location(3950, 13, 12), 0, true, false);
                    return true;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                    return true;
                }
            }
            else
                return false;
        }
        private bool HandleLouresCastleWay()
        {
            if (Client.Map.MapID == 3012)
            {
                if (Client.ClientLocation.AbsoluteXY(15, 0) > 2)
                {
                    Client.Pathfind(new Location(3012, 15, 0), 0, true, false);
                    return true;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                    return true;
                }
            }
            else
                return false;
        }
        private bool HandleFireCanyon()
        {
            if (Client.Map.MapID == 10265)
            {
                if (Client.ClientLocation.AbsoluteXY(93, 48) > 2)
                {
                    // Actually puts us in Hwarone City Entrance
                    Client.Pathfind(new Location(10265, 93, 48), 0, true, false);
                    return true;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                    return true;
                }
            }
            else
                return false;
        }
        private bool HandleBlackMarket()
        {
            if (Client.Map.MapID == 424)
            {
                if (Client.ClientLocation.AbsoluteXY(6, 6) > 2)
                {
                    Client.Pathfind(new Location(424, 6, 6), 0, true, false);
                    return true;
                }
                else
                {
                    Client.ClientTab.walkBtn.Text = "Walk";
                    return true;
                }
            }
            else
                return false;
        }

        #region Oren Ruins walking
        // The Oren Ruins related methods are more complex. Each of these methods is specific
        // to a destination and handles the logic to move through the annoying Nobis rooms
        private bool HandleOrenRuinsWalkTo3dash11()
        {
            // Handle sub-routes within specific maps
            if (HandleSubRouteLogicFor3dash11(Client.Map.MapID, Client.ClientLocation))
            {
                return true;
            }

            // Handle main routing logic
            var ruinsMapIDs = new HashSet<int> { 6924, 6701, 6700, 6530, 6531, 6533, 6537, 6535, 6538, 6539, 6540, 6541 };
            if (ruinsMapIDs.Contains(Client.Map.MapID))
            {
                return TryRouteOrPathfind(new Location(6541, 73, 4), 3);
            }

            // Handle Oren Island City
            if (Client.Map.MapID == 6228)
            {
                return TryRouteOrPathfind(new Location(6525, 62, 146), 4);
            }

            // Default to Oren Island City
            if (Client.Map.MapID != 6228)
            {
                return TryRouteOrPathfind(new Location(6228, 59, 169), 4);
            }

            return false;
        }
        private bool HandleSubRouteLogicFor3dash11(int mapID, Location clientLocation)
        {
            switch (mapID)
            {
                case 6530:
                    return Client.Pathfind(new Location(6530, 10, 0), 0, true, false);

                case 6537:
                    return Client.Pathfind(new Location(6537, 0, 4), 0, true, false);

                case 6539:
                    return Client.Pathfind(new Location(6539, 53, 74), 0, true, true);

                case 6538:
                    return Client.Pathfind(new Location(6538, 74, 16), 0, true, false);

                case 6540 when clientLocation.X < 4:
                    return Client.Pathfind(new Location(6540, 3, 0), 0, true, false);

                case 6540:
                    return Client.Pathfind(new Location(6540, 35, 0), 0, true, false);

                case 6541 when clientLocation.X < 28 && clientLocation.Y > 63:
                    return Client.Pathfind(new Location(6541, 23, 74), 0, true, false);

                case 6541 when IsCloseTo(new Location(6541, 73, 4), 3):
                    Client.ClientTab.walkBtn.Text = "Walk";
                    return true;

                default:
                    return false;
            }
        }
        private bool HandleOrenRuinsWalkTo3dash5()
        {
            // Handle sub-routes within specific maps
            if (HandleSubRouteLogicFor3dash5(Client.Map.MapID, Client.ClientLocation))
            {
                return true;
            }

            // Handle main routing logic
            var ruinsMapIDs = new HashSet<int> { 6924, 6701, 6700, 6530, 6531, 6533, 6537, 6535, 6538, 6539, 6540 };
            if (ruinsMapIDs.Contains(Client.Map.MapID))
            {
                return TryRouteOrPathfind(new Location(6538, 58, 73), 3);
            }

            // Handle Oren Island City
            if (Client.Map.MapID == 6228)
            {
                return TryRouteOrPathfind(new Location(6525, 62, 146), 4);
            }

            // Default to Oren Island City
            if (Client.Map.MapID != 6228)
            {
                return TryRouteOrPathfind(new Location(6228, 59, 169), 4);
            }

            return false;
        }
        private bool HandleSubRouteLogicFor3dash5(int mapID, Location clientLocation)
        {
            switch (mapID)
            {
                case 6530:
                    return Client.Pathfind(new Location(6530, 10, 0), 0, true, false);

                case 6537:
                    return Client.Pathfind(new Location(6537, 0, 4), 0, true, false);

                case 6539 when clientLocation.Y < 8:
                    return Client.Pathfind(new Location(6539, 1, 8), 1, true, false);

                case 6539:
                    return Client.Pathfind(new Location(6539, 4, 74), 0, true, true);

                case 6538 when clientLocation.Y < 50:
                    return Client.Pathfind(new Location(6538, 74, 47), 0, true, false);

                case 6540:
                    return Client.Pathfind(new Location(6540, 0, 67), 0, true, false);

                case 6538 when IsCloseTo(new Location(6538, 58, 73), 3):
                    Client.ClientTab.walkBtn.Text = "Walk";
                    return true;

                default:
                    return false;
            }
        }
        private bool HandleOrenRuinsWalkTo2dash5()
        {
            // Handle sub-routes within specific maps
            if (HandleSubRouteLogicFor2dash5(Client.Map.MapID, Client.ClientLocation))
            {
                return true;
            }

            // Handle main routing logic
            var ruinsMapIDs = new HashSet<int> { 6924, 6701, 6700, 6530, 6531, 6533, 6534, 6535 };
            if (ruinsMapIDs.Contains(Client.Map.MapID))
            {
                return TryRouteOrPathfind(new Location(6534, 1, 36), 3);
            }

            // Handle Oren Island City
            if (Client.Map.MapID == 6228)
            {
                return TryRouteOrPathfind(new Location(6525, 62, 146), 4);
            }

            // Default to Oren Island City
            if (Client.Map.MapID != 6228)
            {
                return TryRouteOrPathfind(new Location(6228, 59, 169), 4);
            }

            return false;
        }
        private bool HandleSubRouteLogicFor2dash5(int mapID, Location clientLocation)
        {
            switch (mapID)
            {
                case 6530:
                    return Client.Pathfind(new Location(6530, 10, 0), 0, true, false);

                case 6537:
                    return Client.Pathfind(new Location(6537, 0, 4), 0, true, false);

                case 6535:
                    return Client.Pathfind(new Location(6535, 20, 74), 0, true, false);

                case 6534 when IsCloseTo(new Location(6534, 1, 36), 3):
                    Client.ClientTab.walkBtn.Text = "Walk";
                    return true;

                default:
                    return false;
            }
        }
        private bool HandleOrenRuinsWalkTo2dash11()
        {
            // Handle sub-routes within specific maps
            if (HandleSubRouteLogicFor2dash11(Client.Map.MapID, Client.ClientLocation))
            {
                return true;
            }

            // Handle main routing logic
            var ruinsMapIDs = new HashSet<int> { 6924, 6701, 6700, 6530, 6531, 6533, 6534, 6535, 6536, 6537 };
            if (ruinsMapIDs.Contains(Client.Map.MapID))
            {
                return TryRouteOrPathfind(new Location(6537, 65, 1));
            }

            // Handle Oren Island City
            if (Client.Map.MapID == 6228)
            {
                return TryRouteOrPathfind(new Location(6525, 62, 146), 4);
            }

            // Default to Oren Island City
            if (Client.Map.MapID != 6228)
            {
                return TryRouteOrPathfind(new Location(6228, 59, 169), 4);
            }

            return false;
        }
        private bool HandleSubRouteLogicFor2dash11(int mapID, Location clientLocation)
        {
            switch (mapID)
            {
                case 6537 when clientLocation.X < 31 && clientLocation.Y < 43:
                    return Client.Pathfind(new Location(6537, 0, 31), 0, true, false);

                case 6535 when clientLocation.X > 68 && clientLocation.Y < 56:
                    return Client.Pathfind(new Location(6535, 74, 49), 0, true, false);

                case 6537 when clientLocation.X < 43 && clientLocation.Y > 43:
                    return Client.Pathfind(new Location(6537, 14, 74), 0, true, false);

                case 6536 when clientLocation.X < 25 && clientLocation.Y < 24:
                    return Client.Pathfind(new Location(6536, 0, 15), 0, true, false);

                case 6534 when clientLocation.X > 45:
                    return Client.Pathfind(new Location(6534, 74, 28), 0, true, false);

                case 6536 when (clientLocation.X > 25 || clientLocation.Y > 26) && clientLocation.X < 69:
                    return Client.Pathfind(new Location(6536, 72, 4), 0, true, false);

                case 6536 when clientLocation.X > 68:
                    return Client.Pathfind(new Location(6536, 69, 0), 0, true, false);

                case 6537 when IsCloseTo(new Location(6537, 65, 1), 3):
                    Client.ClientTab.walkBtn.Text = "Walk";
                    return true;

                default:
                    return false;
            }
        }
        #endregion

        private bool TryRouteOrPathfind(Location target, short distance = 0)
        {
            return TryRouteFind(target) || Client.Pathfind(target, distance, true, false);
        }
        // Utility methods to reduce repeated code
        private bool TryRouteFind(Location loc, short distance = 0, bool mapOnly = false, bool shouldBlock = true, bool avoidWarps = true)
        {
            return Client.Routefind(loc, distance, mapOnly, shouldBlock, avoidWarps);
        }

        private bool IsCloseTo(Location target, int threshold)
        {
            Location serverLocation = Client.ServerLocation;
            return (serverLocation.DistanceFrom(target) <= threshold);
        }



        /// <summary>
        /// Attempts to interpret the user input as either a map ID or a named destination.
        /// Returns true if a valid route target was found.
        /// </summary>
        private bool TryGetRouteTarget(string input, out short mapID, out Location destination)
        {
            mapID = 0;
            destination = default;

            // Try parsing as Map ID
            if (short.TryParse(input, out short parsedMapID))
            {
                mapID = parsedMapID;
                destination = new Location(mapID, 0, 0);
                return true;
            }

            // If not a map ID, try getting a named destination
            destination = GetDestinationBasedOnComboBoxText(input);
            return (destination != default);
        }


        private Location GetDestinationBasedOnComboBoxText(string text)
        {
            uint fnvHash = HashingUtils.CalculateFNV(text);
            return CONSTANTS.DESTINATION_MAP.TryGetValue(fnvHash, out var loc) ? loc : default;
        }

        private void FollowWalking(string followName)
        {
            try
            {
                if (Client == null || Client.ClientTab == null)
                    return;

                // Check if follow is enabled and player name is provided
                if (!Client.ClientTab.followCbox.Checked || string.IsNullOrEmpty(Client.ClientTab.followText.Text))
                    return;

                // Try to identify the leader (bot or player)
                Client botClientToFollow = Server.GetClient(followName);
                Player leader = botClientToFollow?.Player ?? Client.WorldObjects.Values.OfType<Player>()
                    .FirstOrDefault(p => p.Name.Equals(followName, StringComparison.CurrentCultureIgnoreCase));

                if (Client != null && (Client.ClientTab.IsBashing || !Client.Stopped))
                {
                    RefreshLastStep();
                }


                if (leader == null)
                {
                    // If no visible leader, check the last seen location
                    if (_leaderID.HasValue && Client.LastSeenLocations.TryGetValue(_leaderID.Value, out Location lastSeenLocation))
                    {
                        // If we have the last seen location, use it
                        //Console.WriteLine($"[FollowWalking] [{Client.Name}] Using last seen location for player {followName}: {lastSeenLocation}");
                        Client.IsWalking = Client.Routefind(lastSeenLocation)
                                            && !Client.ClientTab.oneLineWalkCbox.Checked
                                            && !Server._toggleWalk;
                    }
                    else
                    {
                        //Console.WriteLine($"[FollowWalking] [{Client.Name}] No known location for player.");
                        Client.IsWalking = false;
                        return;
                    }
                }
                else
                {
                    // We have a visible leader, proceed with following logic
                    _leaderID = leader.ID;
                    Location leaderLocation = leader.Location;
                    //Console.WriteLine($"[FollowWalking] [{Client.Name}] Current leader name: {leader.Name}, ID: {_leaderID}, location: {leaderLocation}");

                    int distance = leaderLocation.DistanceFrom(Client.ClientLocation);
                    //Console.WriteLine($"[FollowWalking] [{Client.Name}] is {distance} spaces from leader named: {leader.Name}");

                    // UnStucker logic if necessary
                    if (!UnStucker(leader))
                    {
                        // Determine follow distance
                        short followDistance = (leaderLocation.MapID == Client.Map.MapID)
                            ? (short)Client.ClientTab.followDistanceNum.Value
                            : (short)0;

                        // Apply lockstep logic
                        if (Client.ClientTab.lockstepCbox.Checked && leaderLocation.MapID == Client.Map.MapID)
                        {
                            if (distance > followDistance)
                            {
                                if (Client == null || Client.ClientTab == null)
                                    return;

                                //Console.WriteLine($"[FollowWalking] [{Client.Name}] Lockstep: Distance ({distance}) exceeds follow distance ({followDistance}). Recalculating path.");
                                Client.ConfirmBubble = false;
                                Client.IsWalking = Client.Routefind(leaderLocation, followDistance, true, true)
                                                    && !Client.ClientTab.oneLineWalkCbox.Checked
                                                    && !Server._toggleWalk;
                            }
                        }
                        else if (distance > followDistance)
                        {
                            if (Client == null || Client.ClientTab == null)
                                return;

                            //Console.WriteLine($"[FollowWalking] [{Client.Name}] Non-lockstep: Distance ({distance}) exceeds follow distance ({followDistance}). Recalculating path.");
                            Client.ConfirmBubble = false;
                            Client.IsWalking = Client.Routefind(leaderLocation, followDistance, true, true)
                                                && !Client.ClientTab.oneLineWalkCbox.Checked
                                                && !Server._toggleWalk;
                        }
                        else
                        {
                            //Console.WriteLine($"[FollowWalking] [{Client.Name}] else block triggered, setting _isWalking to false");
                            Client.IsWalking = false;
                        }

                        // Apply bubble logic for synchronization
                        if (Client.OkToBubble && 
                            DateTime.UtcNow.Subtract(Client.LastStep).TotalMilliseconds > 500.0)
                        {
                            //Console.WriteLine("Bubble conditions met, checking for refresh.");
                            if (Client.ServerLocation != Client.ClientLocation)
                            {
                                //Console.WriteLine($"[FollowWalking] [{Client.Name}] Client position differs from server, requesting refresh.");
                                Client.ConfirmBubble = false;
                                Client.RefreshRequest(true);
                            }
                            else if (Client.Map.Name.Contains("Lost Ruins") || Client.Map.Name.Contains("Assassin Dungeon")
                                     || _nearbyValidCreatures.Any(c => Client.ServerLocation.DistanceFrom(c.Location) <= 6))
                            {
                                //Console.WriteLine("Bubble confirmed for specific maps or valid creatures nearby.");
                                Client.ConfirmBubble = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Client.IsWalking = false;
                Console.WriteLine($"[FollowWalking] Exception occurred {ex.Message}");
            }
        }


        private bool UnStucker(Player leader)
        {
            if (Server._toggleWalk)
            {
                return false;
            }

            if (DateTime.UtcNow.Subtract(leader.GetState<DateTime>(CreatureState.LastStep)).TotalSeconds > 5.0
                && leader.Location.MapID == Client.Map.MapID
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
                List<Location> warps = Client.GetWarpPoints(leader.Location);

                // Build a list of all reachable (non-wall) tiles on the leader's current map, excluding warp tiles,
                // object-occupied tiles, and the leader's own current position.
                List<Location> list = (from kvp in Server._maps[leader.Location.MapID].Tiles
                                       where !kvp.Value.IsWall
                                             && !warps.Contains(new Location(leader.Location.MapID, kvp.Key))
                                             && !objectPoints.Contains(new Location(leader.Location.MapID, kvp.Key))
                                             && kvp.Key != leader.Location.Point
                                       select new Location(leader.Location.MapID, kvp.Key)).ToList();

                // Determine the flood fill threshold based on the number of available locations.
                // This threshold helps decide if the area around the leader is sufficiently navigable.
                // It is calculated as 1% of the total reachable tiles but constrained between 5 and 25
                // to maintain a reasonable minimum and maximum threshold.  
                int val = list.Count / 100;
                int num = Math.Max(5, Math.Min(val, 25));

                // Perform a flood fill algorithm starting from the leader's location to identify the
                // connected region of accessible tiles. The flood fill is limited to a maximum of 26 tiles
                // to prevent excessive computation and ensure performance remains optimal.
                // The result is compared against the threshold to determine if the navigable area meets
                // the required criteria for safe movement. If the count of connected tiles is less than
                // or equal to the threshold, it indicates limited navigable space
                bool flag = list.FloodFill(leader.Location).Take(26).Count() <= num;

                if (flag)
                {
                    Console.WriteLine($"[UnStucker] flag was triggered");

                    // Select a random location from the list
                    Random random = new Random();
                    Location location = list.OrderBy(_ => random.Next()).FirstOrDefault();

                    // Perform the route find operation
                    if (Client.Routefind(location, 0, true, true))
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
            bool? isBashing = client?.ClientTab?.IsBashing;

            if (isBashing.GetValueOrDefault()
                && !_rangerNear
                && !Client.HasEffect(EffectsBar.BeagSuain)
                && !Client.HasEffect(EffectsBar.Pramh)
                && !Client.HasEffect(EffectsBar.Suain)
                && Client.GetAllNearbyMonsters(0).Any<Creature>())
            {
                Direction direction = RandomUtils.RandomEnumValue<Direction>();
                Client.Walk(direction);
                Thread.Sleep(300);
                Client.RefreshRequest(false);
            }
        }

        private void MoveToNearbyLocation()
        {
            // Move to nearby location logic
            Location serverLoc = Client.ServerLocation;
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
                if (!Client.Map.IsWall(location)
                    && nearbyCreatures.All(c => c.Location != location)
                    && Client.Routefind(location, 0, true, true))
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
                Client.RefreshRequest(true);
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
                    (Server.ClientStateList.ContainsKey(Client.Name)
                    && Server.ClientStateList[Client.Name] == CharacterState.WaitForSpells))
                {
                    //Console.WriteLine($"[Waypoints] [{Client.Name}] No waypoints available or client in WaitForSpells state.");
                    return;
                }

                if (currentWay < ways.Count)
                {

                    Client.WalkSpeed = Client.ClientTab.walkSpeedSldr.Value;


                    if (skip && Client.GetNearbyObjects().OfType<Creature>()
                        .Any(creature => creature.Type != CreatureType.WalkThrough && creature.Location.Point == ways[currentWay].Point))
                    {
                        //Console.WriteLine($"[Waypoints] [{Client.Name}] Skipping waypoint {currentWay} due to nearby creature.");
                        currentWay++;
                    }
                    else
                    {
                        WayForm waysForm = Client.ClientTab.WayForm;

                        // Special door proximity condition
                        if (DateTime.UtcNow.Subtract(_doorTime).TotalSeconds < 2.5 &&
                            Client.ClientLocation.Point.Distance(_doorPoint) < 6)
                        {
                            //Console.WriteLine($"[Waypoints] [{Client.Name}] Near door, adjusting walking speed.");
                            Client.WalkSpeed = Client.WalkSpeed > 350.0 ? 350.0 : Client.WalkSpeed;
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
                                Console.WriteLine($"[Waypoints] [{Client.Name}] Filtered out dioned creatures");
                            }


                            // Condition 1
                            if (GetCheckBoxChecked(waysForm.condition1)
                                && nearbyCreatures.Count(c => Client.WithinRange(c, (int)GetNumericUpDownValue(waysForm.proximityUpDwn1)))
                                    >= GetNumericUpDownValue(waysForm.mobSizeUpDwn1)
                                && !Client.ClientTab.IsBashing)
                            {
                                Client.WalkSpeed = (double)GetNumericUpDownValue(waysForm.walkSlowUpDwn1);
                                //Console.WriteLine("Condition 1 met, adjusting walking speed.");
                            }

                            // Condition 2
                            if (GetCheckBoxChecked(waysForm.condition2)
                                && nearbyCreatures.Count(c => Client.WithinRange(c, (int)GetNumericUpDownValue(waysForm.proximityUpDwn2)))
                                    >= GetNumericUpDownValue(waysForm.mobSizeUpDwn2)
                                && !Client.ClientTab.IsBashing)
                            {
                                Client.WalkSpeed = (double)GetNumericUpDownValue(waysForm.walkSlowUpDwn2);
                                //Console.WriteLine("Condition 2 met, adjusting walking speed.");
                            }

                            // Condition 3
                            if (GetCheckBoxChecked(waysForm.condition3)
                                && nearbyCreatures.Count(c => Client.WithinRange(c, (int)GetNumericUpDownValue(waysForm.proximityUpDwn3)))
                                    >= GetNumericUpDownValue(waysForm.mobSizeUpDwn3)
                                && !Client.ClientTab.IsBashing)
                            {
                                Client.WalkSpeed = (double)GetNumericUpDownValue(waysForm.walkSlowUpDwn3);
                                //Console.WriteLine("Condition 3 met, adjusting walking speed.");
                            }

                            // Condition 4
                            if (GetCheckBoxChecked(waysForm.condition4)
                                && nearbyCreatures.Count(c => Client.WithinRange(c, (int)GetNumericUpDownValue(waysForm.proximityUpDwn4)))
                                    >= GetNumericUpDownValue(waysForm.mobSizeUpDwn4))
                            {
                                //Console.WriteLine("Condition 4 met, stopping movement and checking bubble conditions.");
                                Client.Stopped = true;

                                if (BackTracking())
                                {
                                    return;
                                }

                                if (Client.Map.Name.Contains("Lost Ruins") || Client.Map.Name.Contains("Assassin Dungeon")
                                    || _nearbyValidCreatures.Any(monster => monster.Location.DistanceFrom(Client.ServerLocation) <= 6))
                                {
                                    Client.OkToBubble = true;
                                }

                                // Also apply bubble to the bot's follow chain
                                foreach (Client client in Server.GetFollowChain(Client))
                                {
                                    if (!client.OkToBubble)
                                    {
                                        client.OkToBubble = true;
                                    }
                                }

                                Client.IsWalking = false;
                                return;
                            }
                        }

                        // Reset stop status if none of the conditions applied
                        Client.Stopped = false;
                        foreach (Client client in Server.GetFollowChain(Client))
                        {
                            client.OkToBubble = false;
                        }
                        Client.OkToBubble = false;

                        // Check if the bot needs to backtrack
                        if (BackTracking())
                        {
                            Console.WriteLine($"[Waypoints] [{Client.Name}] Backtracking logic triggered, stopping waypoint navigation.");
                            return;
                        }

                        // Handle specific status effects and conditions on followers
                        foreach (Client client in Server.GetFollowChain(Client))
                        {
                            if (client.HasEffect(EffectsBar.Pramh) || client.HasEffect(EffectsBar.Suain)
                                || client.HasEffect(EffectsBar.BeagSuain) || client.HasEffect(EffectsBar.Skull)
                                || client.Player.IsSkulled)
                            {
                                Console.WriteLine($"[Waypoints] [{Client.Name}] Cannot move due to effect, stopping waypoint navigation.");
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

                                if (itemsToPickUp.Count != 1 && itemsToPickUp[0] != 140 && !Client.InventoryFull)
                                    return;


                                List<GroundItem> nearbyGroundItems = Client.GetNearbyGroundItems(12, itemsToPickUp.ToArray());

                                foreach (GroundItem groundItem in nearbyGroundItems)
                                {
                                    int count = Client.Pathfinder.FindPath(Client.ClientLocation, groundItem.Location).Count;

                                    if (count == 0)
                                    {
                                        count = Client.Pathfinder.FindPath(Client.ClientLocation, groundItem.Location).Count;
                                    }

                                    if (count == 0 || count > Client.ClientTab.overrideDistanceNum.Value)
                                    {
                                        nearbyGroundItems.Remove(groundItem);
                                    }
                                }

                                if (nearbyGroundItems.Any())
                                {
                                    GroundItem closestItem = nearbyGroundItems.OrderBy(item => item.Location.DistanceFrom(Client.ClientLocation)).FirstOrDefault();
                                    if (closestItem != null && Client.ClientLocation.DistanceFrom(closestItem.Location) > 2)
                                    {
                                        // Move to the item
                                        Client.IsWalking = Client.Pathfind(closestItem.Location, 2)
                                                                && !Client.ClientTab.oneLineWalkCbox.Checked
                                                                && !Server._toggleWalk;
                                        return;
                                    }

                                    if (Client.ClientLocation.DistanceFrom(closestItem.Location) <= 2 && Client.ServerLocation.DistanceFrom(closestItem.Location) > 2)
                                    {
                                        Client.RefreshRequest(true);
                                    }

                                    // Pick up the item if nearby
                                    if (Monitor.TryEnter(Client.CastLock, 200))
                                    {
                                        try
                                        {
                                            Client.Pickup(0, closestItem.Location);
                                            Console.WriteLine("Picked up item.");
                                        }
                                        finally
                                        {
                                            Monitor.Exit(Client.CastLock);
                                        }
                                    }
                                    Client.IsWalking = false;
                                    return;
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error during item pickup: {ex.Message}");
                                Client.IsWalking = false;
                            }
                        }

                        Point currentPoint = this.Client.ServerLocation.Point;

                        if (this.Client.ClientTab.IsBashing)
                        {
                            Location currentWay = this.ways[this.currentWay];
                            Location nextWay = this.currentWay < this.ways.Count - 1 ? this.ways[this.currentWay + 1] : this.ways[0];

                            int distanceToCurrentWay = currentPoint.Distance(currentWay.Point);
                            int distanceToNextWay = currentWay.Point.Distance(nextWay.Point);
                            Direction currentDirection = currentPoint.GetDirection(currentWay.Point);
                            Direction nextDirection = nextWay.Point.GetDirection(currentWay.Point);

                            if (currentWay.MapID == this.Client.Map.MapID && currentWay.MapID == nextWay.MapID && distanceToCurrentWay > 3 && distanceToCurrentWay < distanceToNextWay && currentDirection == nextDirection)
                            {
                                this.currentWay++;
                                return;
                            }
                        }

                        Location targetWay = this.ways[this.currentWay];
                        int distanceToTarget = this.Client.ClientLocation.Point.Distance(targetWay.Point);

                        if (distanceToTarget > waysForm.distanceUpDwn.Value)
                        {
                            //Console.WriteLine($"[Waypoints] [{Client.Name}] Distance to target ({distanceToTarget}) is greater than allowed value ({waysForm.distanceUpDwn.Value}). Initiating RouteFind.");


                            //Console.WriteLine($"[Waypoints] [{Client.Name}] Client and target waypoint are on the same map (MapID: {this.Client._map.MapID}).");

                            bool routeFindResult = this.Client.Routefind(targetWay, (short)waysForm.distanceUpDwn.Value);
                            //Console.WriteLine($"[Waypoints] [{Client.Name}] RouteFind to {targetWay} returned: {routeFindResult}");

                            bool canWalk = !this.Client.ClientTab.oneLineWalkCbox.Checked && !this.Server._toggleWalk;
                            //Console.WriteLine($"[Waypoints] [{Client.Name}] Can walk conditions - oneLineWalkCbox.Checked: {this.Client.ClientTab.oneLineWalkCbox.Checked}, _toggleWalk: {this.Server._toggleWalk}, Result: {canWalk}");

                            this.Client.IsWalking = routeFindResult && canWalk;
                            //Console.WriteLine($"[Waypoints] [{Client.Name}] Client._isWalking set to: {this.Client._isWalking}");


                        }
                        else
                        {
                            //Console.WriteLine($"[Waypoints] [{Client.Name}] Distance to target waypoint({distanceToTarget}) is within allowed value ({waysForm.distanceUpDwn.Value}). Stopping movement.");

                            this.Client.IsWalking = false;
                            //Console.WriteLine($"[Waypoints] [{Client.Name}] Client._isWalking set to: {this.Client._isWalking}");

                            if (this.Client.ClientLocation.Point.Distance(targetWay.Point) <= waysForm.distanceUpDwn.Value)
                            {
                                //Console.WriteLine($"[Waypoints] [{Client.Name}] Client is within the allowed distance to the target.");

                                if (this.Client.Map.MapID == targetWay.MapID)
                                {
                                    //Console.WriteLine($"[Waypoints] [{Client.Name}] Client and target waypoint are on the same map (MapID: {this.Client._map.MapID}).");

                                    if (this.Client.ServerLocation.Point.Distance(targetWay.Point) > waysForm.distanceUpDwn.Value)
                                    {
                                        //Console.WriteLine($"[Waypoints] [{Client.Name}] Server's location is beyond the allowed distance. Requesting position refresh.");
                                        this.Client.RefreshRequest();
                                    }
                                    else
                                    {
                                        //Console.WriteLine($"[Waypoints] [{Client.Name}] Both client and server positions are within the allowed distance. Advancing to the next waypoint.");
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
                Console.WriteLine($"[Waypoints] [{Client.Name}] Thread aborted: {ex.Message}");
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
                    if (client.Map.MapID != Client.Map.MapID || client.ServerLocation.DistanceFrom(Client.ServerLocation) > 9)
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
                    Client.WalkSpeed = Client.ClientTab.walkSpeedSldr.Value;
                    if (clientToFollow != null)
                    {
                        Client.IsWalking = Client.Routefind(clientToFollow.ServerLocation, 3, shouldBlock: false);
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




        private void UpdateWalkButton(Location targetLocation)
        {
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return;
            }

            if (!clientTab.walkMapCombox.Text.Contains("Nobis") && ShouldProceedWithNavigation(targetLocation))
            {
                UpdateButtonStateBasedOnProximity(targetLocation);
            }
        }

        private bool ShouldProceedWithNavigation(Location targetLocation)
        {
            bool isRouteAvailable = Client.Routefind(targetLocation, 3, false, true, true);
            return !isRouteAvailable && Client.Map.MapID == targetLocation.MapID;
        }

        private void UpdateButtonStateBasedOnProximity(Location targetLocation)
        {
            Location currentLocation = Client.ServerLocation;
            int proximityThreshold = 4;
            if (currentLocation.DistanceFrom(targetLocation) <= proximityThreshold)
            {
                Client.ClientTab.walkBtn.Text = "Walk";
            }
        }

        private void HandleDumbMTGWarp()
        {
            if (Client.Map == null)
            {
                return;
            }

            //The warp into Mt. Giragan 1 is stupid and sometimes drops you in the warp to the world server
            if (Client.Map.MapID.Equals(2120)) //Giragan
            {
                Location location = Client.ServerLocation;
                if (location.X.Equals(39) && (location.Y.Equals(8) || location.Y.Equals(7)))
                {
                    Client.Walk(Direction.West);
                    Thread.Sleep(800);
                }
            }
        }

        private void HandleDialog()
        {
            if (Client.NpcDialog != null && Client.NpcDialog.Equals("You see strange dark fog upstream. Curiosity overcomes you and you take a small raft up the river through the black mist."))
            {
                Client.EnterKey();
                Client.NpcDialog = "";
            }
        }
        private void SoundLoop()
        {
            if (Client.ClientTab == null || Client == null)
            {
                return;
            }
            while (!_shouldThreadStop)
            {
                //Console.WriteLine("[SoundLoop] Pulse");
                if (Server._disableSound)
                {
                    Thread.Sleep(250);
                    return;
                }

                if (Client.ClientTab == null || Client == null)
                {
                    return;
                }

                if (Client.RecentlyDied)
                {
                    //soundPlayer.Stream = Resources.warning;
                    soundPlayer.PlaySync();
                    Client.RecentlyDied = false;
                }

                if (Client.ClientTab.alertSkulledCbox.Checked && Client.IsSkulled)
                {
                    soundPlayer.Stream = Resources.skull;
                    soundPlayer.PlaySync();
                }

                if (Client.ClientTab.alertRangerCbox.Checked && IsRangerNearBy())
                {
                    soundPlayer.Stream = Resources.ranger;
                    soundPlayer.PlaySync();
                }

                if (Client.ClientTab.alertStrangerCbox.Checked && IsStrangerNearby())
                {
                    soundPlayer.Stream = Resources.detection;
                    soundPlayer.PlaySync();
                }

                if (Client.ClientTab.alertItemCapCbox.Checked && _shouldAlertItemCap)
                {
                    soundPlayer.Stream = Resources.itemCap;
                    soundPlayer.PlaySync();
                    _shouldAlertItemCap = false;
                }

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
                        ProcessCreatureText();

                        if (Client.CurrentHP <= 1U && Client.IsSkulled)
                        {
                            //Console.WriteLine("[BotLoop] Client HP <= 1 and is skulled, handling skull status");
                            HandleSkullStatus();
                            continue;
                        }

                        if (ShouldRequestRefresh())
                        {
                            Client.RefreshRequest(false);
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

        private bool ProcessCreatureText()
        {
            if (Client == null || Client.ClientTab == null)
                return false;

            if (!Client.ClientTab.chkSpellStatus.Checked)  
                return false;
            
            foreach (Creature creature in _nearbyValidCreatures)
            {
                // Retrieve the creature's states
                bool isCursed = creature.GetState<bool>(CreatureState.IsCursed);
                bool isFassed = creature.GetState<bool>(CreatureState.IsFassed);

                // Determine the text to display based on the creature's states
                string displayText = string.Empty;

                if (isCursed && isFassed)
                {
                    // Creature is both cursed and fassed
                    displayText = "[C/F]";
                }
                else if (isCursed)
                {
                    // Creature is only cursed
                    displayText = "[C]";
                }
                else if (isFassed)
                {
                    // Creature is only fassed
                    displayText = "[F]";
                }

                // Display the text if applicable
                if (!string.IsNullOrEmpty(displayText))
                {
                    Client.DisplayTextOverTarget(2, creature.ID, displayText);
                }
            }

            return true;
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
            return (Client.InventoryFull && Client.ClientTab.toggleFarmBtn.Text == "Farming") ||
                   (Client.Bot._hasDeposited && Client.ClientTab.toggleFarmBtn.Text == "Farming") ||
                   Client.ExchangeOpen || Client.ClientTab == null || Client.Dialog != null || _dontCast;
        }
        private void ProcessPlayers()
        {
            NearbyAllies = Client.GetNearbyAllies();
            _nearbyValidCreatures = Client.GetNearbyValidCreatures(11);
            var nearbyPlayers = Client.GetNearbyPlayers();
            _playersExistingOver250ms = nearbyPlayers?
                .Where(p => (DateTime.UtcNow - p.Creation).TotalMilliseconds > 250)
                .ToList();
            _skulledPlayers.Clear();
        }
        private void HandleSkullStatus()
        {
            currentAction.Text = Client.Action + "Nothing, you're dead";
            _skullTime = (_skullTime == DateTime.MinValue) ? DateTime.UtcNow : _skullTime;
            LogIfSkulled();
            LogIfSkulledAndSurrounded();
            if (!Client.SafeScreen)
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
                    Client.Map.Name,
                    ": (",
                    Client.ServerLocation.ToString(),
                    ")"
                }));
                Process.Start(text);
                Client.DisconnectWait(false);
            }
        }
        private void LogIfSkulledAndSurrounded()
        {
            bool isOptionsSkullSurrboxChecked = Client.ClientTab.optionsSkullSurrbox.Checked;
            if (isOptionsSkullSurrboxChecked && DateTime.UtcNow.Subtract(_skullTime).TotalSeconds > 4.0 && Client.IsLocationSurrounded(Client.ServerLocation))
            {
                string text2 = AppDomain.CurrentDomain.BaseDirectory + "\\skull.txt";
                File.WriteAllText(text2, string.Concat(new string[]
                {
                    DateTime.UtcNow.ToLocalTime().ToString(),
                    "\n",
                    Client.Map.Name,
                    ": (",
                    Client.ServerLocation.ToString(),
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
                    _skulledPlayers.Add(player);
                }
                return _skulledPlayers = _skulledPlayers.OrderBy(DistanceFromPlayer).ToList();
            }
            return new List<Player>();
        }

        private bool RedSkulledPlayers()
        {
            if (Client == null || Client.ClientTab == null)
                return false;

            Console.WriteLine("[BotLoop] RedSkulledPlayers called");

            if (_skulledPlayers.Count > 0 && Client.ClientTab.autoRedCbox.Checked)
            {
                Console.WriteLine("[BotLoop] Players needing red > 0 and autoRed checked");

                Player player = _skulledPlayers[0];
                Console.WriteLine($"[BotLoop] Target player: {player?.Name}, HealthPercent: {player?.HealthPercent}");

                var inventory = Client.Inventory;
                bool canUseBeetleAid = inventory.Contains("Beetle Aid") && Client.IsRegistered &&
                                       DateTime.UtcNow.Subtract(_lastUsedBeetleAid).TotalMinutes > 2.0;
                bool canUseOtherItems = inventory.Contains("Komadium") || inventory.Contains("beothaich deum");

                Console.WriteLine($"[BotLoop] Can use Beetle Aid: {canUseBeetleAid}, Can use other items: {canUseOtherItems}");

                if (canUseBeetleAid || canUseOtherItems)
                {
                    _dontWalk = true;
                    Direction direction = player.Location.Point.GetDirection(Client.ServerLocation.Point);
                    Console.WriteLine($"[BotLoop] Calculated direction to player: {direction}");

                    if (Client.ServerLocation.DistanceFrom(player.Location) > 1)
                    {
                        Console.WriteLine("[BotLoop] Server distance from player > 1");

                        if (Client.ClientLocation.DistanceFrom(player.Location) == 1)
                        {
                            Console.WriteLine("[BotLoop] Client distance from player = 1, requesting refresh");
                            Client.RefreshRequest(true);
                        }

                        Console.WriteLine("[BotLoop] Pathfinding to player location");
                        Client.Pathfind(player.Location, 1, true, true);
                    }
                    else if (direction != Client.ServerDirection)
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
                            player.AnimationHistory[(ushort)SpellAnimation.Skull] = DateTime.UtcNow.AddSeconds(-2);
                        }
                        else if (canUseOtherItems && (Client.UseItem("Komadium") || Client.UseItem("beothaich deum")))
                        {
                            Console.WriteLine("[BotLoop] Used other item (Komadium or beothaich deum)");
                            player.AnimationHistory[(ushort)SpellAnimation.Skull] = DateTime.UtcNow.AddSeconds(-2);
                            Thread.Sleep(1000); // Consider async/await pattern if possible
                        }

                        Console.WriteLine("[BotLoop] Using Transferblood skill");
                        Client.UseSkill("Transferblood");

                        return false;
                    }
                }
                else if (player == null || !Client.GetNearbyPlayers().Contains(player) || player.HealthPercent > 30 || DateTime.UtcNow.Subtract(player.AnimationHistory[(ushort)SpellAnimation.Skull]).TotalSeconds > 5.0)
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
            return Client.GroupBindingList.Concat(Client.FriendBindingList).Contains(player.Name, StringComparer.CurrentCultureIgnoreCase) && player.IsSkulled;
        }

        private int DistanceFromPlayer(Player player)
        {
            return Client.ServerLocation.DistanceFrom(player.Location);
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

        private bool PerformActions()
        {
            var clientTab = Client.ClientTab;
            if (clientTab == null || Client.ClientTab == null)
            {
                return false;
            }

            _autoStaffSwitch = clientTab.autoStaffCbox.Checked;
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
            int? castLines = (castLinesCount != null) ? new int?(castLinesCount.GetValueOrDefault()) : null;

            if (castLines.GetValueOrDefault() <= 0 & castLines != null)
            {
                Thread.Sleep(330);
            }

            return true;
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
                var playerToDion = NearbyAllies.FirstOrDefault(p => !p.IsDioned);

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
                if (!Client.UseSpell("Nuadhiach Le Cheile", null, _autoStaffSwitch, false) &&
                    !Client.UseSpell("ard ioc comlha", null, _autoStaffSwitch, false) &&
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
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                //Exit early if clienttab is null e.g., switching servers
                return false;
            }

            if (clientTab.deireasFaileasCbox.Checked && !Client.HasEffect(EffectsBar.DeireasFaileas))
            {
                Client.UseSpell("deireas faileas", null, _autoStaffSwitch, true);
                return false;
            }

            if (clientTab.dragonsFireCbox.Checked && Client.IsRegistered && !Client.HasEffect(EffectsBar.DragonsFire))
            {
                Client.UseItem("Dragon's Fire");
            }

            if (clientTab.druidFormCbox.Checked && !Client.HasEffect(EffectsBar.FeralForm) && !Client.HasEffect(EffectsBar.KaruraForm) && !Client.HasEffect(EffectsBar.KomodasForm) && !_swappingNecklace)
            {
                if (!Client.UseSpell("Feral Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Wild Feral form", null, _autoStaffSwitch, true) && !Client.UseSpell("Fierce Feral Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Master Feral Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Karura Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Wild Karura Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Fierce Karura Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Master Karura Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Komodas Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Wild Komodas Form", null, _autoStaffSwitch, true) && !Client.UseSpell("Fierce Komodas Form", null, _autoStaffSwitch, true))
                {
                    Client.UseSpell("Master Komodas Form", null, _autoStaffSwitch, true);
                }
                Thread.Sleep(1000);
            }

            if (clientTab.asgallCbox.Checked && !Client.HasEffect(EffectsBar.AsgallFaileas))
            {
                Client.UseSpell("asgall faileas", null, true, true);
            }

            if (clientTab.perfectDefenseCbox.Checked && !Client.HasEffect(EffectsBar.PerfectDefense))
            {
                Client.UseSkill("Perfect Defense");
            }

            if (clientTab.aegisSphereCbox.Checked && !Client.HasEffect(EffectsBar.Armachd))
            {
                Client.UseSpell("Aegis Sphere", null, false, true);
            }

            if (clientTab.aoSuainCbox.Checked && Client.HasEffect(EffectsBar.BeagSuain))
            {
                Client.UseSkill("ao beag suain");
            }

            if (clientTab.muscleStimulantCbox.Checked && Client.IsRegistered && !Client.HasEffect(EffectsBar.FasDeireas))
            {
                Client.UseItem("Muscle Stimulant");
            }

            if (clientTab.nerveStimulantCbox.Checked && Client.IsRegistered && !Client.HasEffect(EffectsBar.Beannaich))
            {
                Client.UseItem("Nerve Stimulant");
            }

            if (clientTab.disenchanterCbox.Checked && DateTime.UtcNow.Subtract(_lastDisenchanterCast).TotalMinutes > 6.0)
            {
                Client.UseSpell("Disenchanter", null, _autoStaffSwitch, true);
                Thread.Sleep(1000);
                return false;
            }

            if (clientTab.monsterCallCbox.Checked && Client.IsRegistered && DateTime.UtcNow.Subtract(_lastUsedMonsterCall).TotalSeconds > 2.0 && Client.UseItem("Monster Call"))
            {
                _lastUsedMonsterCall = DateTime.UtcNow;
            }

            if (clientTab.mistCbox.Checked && !Client.HasEffect(EffectsBar.Mist))
            {
                Client.UseSpell("Mist", null, _autoStaffSwitch, true);
            }

            if (clientTab.manaWardCbox.Checked)
            {
                Client.UseSpell("Mana Ward", null, false, false);
            }

            if (clientTab.vanishingElixirCbox.Checked && Client.IsRegistered)
            {
                foreach (Player ally in NearbyAllies)
                {
                    if (!ally._isHidden)
                    {
                        Client.UseItem("Vanishing Elixir");
                    }
                }
            }

            if (clientTab.autoDoubleCbox.Checked && !Client.HasEffect(EffectsBar.BonusExperience) && Client.IsRegistered && Client.CurrentMP > 100)
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
                if (clientTab.doublesCombox.Text == "Xmas 100%")
                {
                    var itemText = Client.HasItem("Christmas Double Exp-Ap") ? "Christmas Double Exp-Ap" : "XMas Double Exp-Ap";
                    clientTab.UseDouble(itemText);
                }
                else if (itemMappings.TryGetValue(clientTab.doublesCombox.Text, out var itemText))
                {
                    clientTab.UseDouble(itemText);
                }

                clientTab.UpdateExpBonusTimer();
            }

            if (clientTab.autoMushroomCbox.Checked && !Client.HasEffect(EffectsBar.BonusMushroom) && Client.IsRegistered && Client.CurrentMP > 100)
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

                if (clientTab.mushroomCombox.Text == "Best Available")
                {
                    var mushrooom = clientTab.FindBestMushroomInInventory(Client);
                    clientTab.UseMushroom(mushrooom);
                }
                else if (itemMappings.TryGetValue(clientTab.mushroomCombox.Text, out var mushroom))
                {
                    clientTab.UseMushroom(mushroom);
                }


                clientTab.UpdateMushroomBonusTimer();
            }



            if (clientTab.regenerationCbox.Checked && (!Client.HasEffect(EffectsBar.Regeneration) || !Client.HasEffect(EffectsBar.IncreasedRegeneration)))
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
                        if (player != null && currentAlly != null && currentAlly.AllyPage != null && currentAlly.AllyPage.dbRegenCbox != null &&
                            player.Name.Equals(currentAlly.Name, StringComparison.OrdinalIgnoreCase) && player != Client.Player && currentAlly.AllyPage.dbRegenCbox.Checked)
                        {
                            Client client = Client.Server.GetClient(currentAlly.Name);
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
                                if ((!player.AnimationHistory.ContainsKey((ushort)SpellAnimation.IncreasedRegeneration) || DateTime.UtcNow.Subtract(player.AnimationHistory[(ushort)SpellAnimation.IncreasedRegeneration]).TotalSeconds > 1.5) && Client.UseSpell("Increased Regeneration", player, _autoStaffSwitch, true))
                                {
                                    return false;
                                }
                                if (Client.Spellbook.Any(s => s.Name != "Increased Regeneration" && s.Name.Contains("Regeneration"))
                                    && (!player.AnimationHistory.ContainsKey((ushort)SpellAnimation.Regeneration)
                                        || DateTime.UtcNow.Subtract(player.AnimationHistory[(ushort)SpellAnimation.Regeneration]).TotalSeconds > 1.5)
                                    && TryCastAnyRank("Regeneration", player, _autoStaffSwitch, true))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            if (clientTab.mantidScentCbox.Checked && Client.IsRegistered && !Client.HasEffect(EffectsBar.MantidScent))
            {
                if (Client.UseItem("Mantid Scent") || Client.UseItem("Potent Mantid Scent"))
                {
                    return true;
                }

                clientTab.mantidScentCbox.Checked = false;
                Client.ServerMessage((byte)ServerMessageType.Whisper, "You do not own Mantid Scent");
                return false;
            }

            if (clientTab.equipmentrepairCbox.Checked && Client.NeedsToRepair && DateTime.UtcNow.Subtract(_hammerTimer).TotalMinutes > 40.0)
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
            if (AllyPage == null)
            {
                return false;
            }

            foreach (Ally ally in ReturnAllyList())
            {
                if (ally.AllyPage == null) { break; } 

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

        private bool FasSpiorad()
        {
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            if (_needFasSpiorad)
            {
                uint currentMP = Client.CurrentMP;
                DateTime startTime = DateTime.UtcNow;

                while (_needFasSpiorad)
                {
                    int fasSpioradThreshold;

                    bool isFasSpioradChecked = clientTab.fasSpioradCbox.Checked;
                    bool isThresholdParsed = int.TryParse(clientTab.fasSpioradText.Text.Trim(), out fasSpioradThreshold);
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

            return true;
        }

        private bool BeagCradh()
        {
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isBeagCradhChecked = clientTab.beagCradhCbox.Checked;
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
            if (AllyPage == null)
            {
                return false;
            }

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
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isWakeScrollChecked = clientTab.wakeScrollCbox.Checked;
            bool isRegistered = Client.IsRegistered;

            if (isWakeScrollChecked && isRegistered && NearbyAllies.Any(player => IsAllyAffectedByPramhOrAsleep(player)))
            {
                if (Client.UseItem("Wake Scroll"))
                {
                    foreach (Player player in NearbyAllies)
                    {
                        Client client = Server.GetClient(player.Name);
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
                Client client = Server.GetClient(player.Name);
                return client != null && client.HasEffect(EffectsBar.Pramh);
            }
            return true;
        }

        private bool AoPoison()
        {
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isAoPoisonChecked = clientTab.aoPoisonCbox.Checked;
            bool isPlayerPoisoned = Client.Player.IsPoisoned;
            bool isFungusExtractChecked = clientTab.fungusExtractCbox.Checked;
            bool shouldUseFungusExtract = DateTime.UtcNow.Subtract(_lastUsedFungusBeetle).TotalSeconds > 1.0;

            // Process allies for Ao poison dispel
            if (!AoPoisonForAllies(shouldUseFungusExtract))
            {
                return false;
            }

            if (isAoPoisonChecked && Client.HasEffect(EffectsBar.Poison) && isPlayerPoisoned)
            {
                if (isFungusExtractChecked && Client.IsRegistered)
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
            if (AllyPage == null)
            {
                return false;
            }

            foreach (Ally ally in ReturnAllyList())
            {
                if (ally.AllyPage.dispelPoisonCbox.Checked && IsAlly(ally, out Player player, out Client client))
                {
                    if (client.HasEffect(EffectsBar.Poison) && player.IsPoisoned && shouldUseFungusExtract)
                    {
                        if (Client.IsRegistered && Client.HasItem("Fungus Beetle Extract"))
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
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isAiteChecked = clientTab.aiteCbox.Checked;
            bool isPlayerAited = Client.Player.IsAited;
            double aiteDuration = Client.Player.GetState<double>(CreatureState.AiteDuration);
            string aiteSpell = clientTab.aiteCombox.Text;

            if (isAiteChecked && !Client.HasEffect(EffectsBar.NaomhAite) && (!isPlayerAited || aiteDuration != 2.0))
            {
                Client.UseSpell(aiteSpell, Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool AiteAllies()
        {
            if (AllyPage == null)
            {
                return false;
            }

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
            if (AllyPage == null)
            {
                return false;
            }

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
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isFasChecked = clientTab.fasCbox.Checked;
            bool isPlayerFassed = Client.Player.IsFassed;
            double fasDuration = Client.Player.GetState<double>(CreatureState.FasDuration);
            string fasSpell = clientTab.fasCombox.Text;

            if (isFasChecked && !Client.HasEffect(EffectsBar.FasNadur) && (!isPlayerFassed || fasDuration != 2.0))
            {
                Client.UseSpell(fasSpell, Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool DragonScale()
        {
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isDragonScaleChecked = clientTab.dragonScaleCbox.Checked;

            if (isDragonScaleChecked && Client.IsRegistered && !Client.HasEffect(EffectsBar.Armachd))
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
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isDionChecked = clientTab.dionCbox.Checked;

            if (!isDionChecked || Client.HasEffect(EffectsBar.Dion))
            {
                return false;
            }

            string dionWhen = clientTab.dionWhenCombox.Text;
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

            if (shouldUseSpell || (clientTab.aoSithCbox.Checked && _recentlyAoSithed))
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
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isArmachdChecked = clientTab.armachdCbox.Checked;

            if (isArmachdChecked && !Client.HasEffect(EffectsBar.Armachd))
            {
                Client.UseSpell("armachd", Client.Player, _autoStaffSwitch, false);
                return false;
            }

            return true;
        }

        private bool ArmachdAllies()
        {
            if (AllyPage == null)
            {
                return false;
            }

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

        private bool UseDionOrStone()
        {
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            string dionSpell = clientTab.dionCombox.Text;

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

            return true;
        }

        private bool DispellPlayerCurse()
        {
            var clientTab = Client.ClientTab;
            Player player = Client.Player;

            if (clientTab == null || player == null)
            {
                return false;
            }

            var isDispelCurseChecked = clientTab.aoCurseCbox.Checked;

            var cursesToDispel = new HashSet<string> { "cradh", "mor cradh", "ard cradh" };

            var curseName = player.GetState<string>(CreatureState.CurseName);
            var curseDuration = player.GetState<double>(CreatureState.CurseDuration);

            if (isDispelCurseChecked && cursesToDispel.Contains(curseName))
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
            if (AllyPage == null)
            {
                return false;
            }

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
            if (Client.HasEffect(EffectsBar.FasSpiorad) || Client.ClientTab == null || AllyPage == null)
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

                        if (loopPercentThreshold == 20 && player != client.Player && (client.HasSpell("Nuadhiach Le Cheile") || client.HasSpell("ard ioc comlha") || client.HasSpell("mor ioc comlha")))
                        {

                            int alliesInNeed = Client.GetNearbyAllies().Count(p => p != Client.Player && IsAllyInNeed(p));

                            if (alliesInNeed > 2)
                            {
                                healSpell = Client.HasSpell("Nuadhiach Le Cheile")
                                            ? "Nuadhiach Le Cheile"
                                            : Client.HasSpell("ard ioc comlha")
                                                ? "ard ioc comlha"
                                                : "mor ioc comhla";
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

                                if (!(healSpell == "Nuadhiach Le Cheile") && !(healSpell == "ard ioc comlha") && !(healSpell == "mor ioc comlha"))
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
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isBubbleBlockChecked = clientTab.bubbleBlockCbox.Checked;
            bool isSpamBubbleChecked = clientTab.spamBubbleCbox.Checked;
            bool isFollowChecked = clientTab.followCbox.Checked;
            string walkMap = clientTab.walkMapCombox.Text;

            if (isBubbleBlockChecked && isSpamBubbleChecked)
            {
                if (Client.UseSpell("Bubble Block", null, true, true))
                {
                    return false;
                }
            }
            else if (isBubbleBlockChecked && Client.OkToBubble)
            {
                if (walkMap == "WayPoints")
                {
                    if (CastBubbleBlock())
                    {
                        return false;
                    }
                }
                else if (isFollowChecked && Client.ConfirmBubble && CastBubbleBlock())
                {
                    return false;
                }
            }

            return true;
        }
        private bool CastBubbleBlock()
        {
            // Check if the player has moved since the last bubble was cast.
            bool hasMoved = !Location.Equals(_lastBubbleLocation, Client.ServerLocation);

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
                        _lastBubbleLocation = Client.ServerLocation;
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
                        _lastBubbleLocation = Client.ServerLocation;
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
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }

            bool isHideChecked = clientTab.hideCbox.Checked;
            bool canUseSpells = Client.Map.CanUseSpells;

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
                if (!Client.HasEffect(EffectsBar.Hide) || DateTime.UtcNow.Subtract(_lastHidden).TotalSeconds > 50.0)
                {
                    Client.UseSkill("Assail");
                    Client.UseSpell(spellName, null, true, true);
                    _lastHidden = DateTime.UtcNow;
                }
                return true;
            }
            return false;
        }

        private bool CastOffensiveSpells()
        {
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return false;
            }
            _nearbyValidCreatures = Client.GetNearbyValidCreatures(11);

            if (_nearbyValidCreatures.Count > 0)
            {
                _nearbyValidCreatures = _nearbyValidCreatures.OrderBy(c => RandomUtils.Random()).ToList();
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
                    creature = creatureList.OrderBy(c => c.Location.DistanceFrom(Client.ClientLocation)).FirstOrDefault();
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
                        .OrderBy(c => c.Location.DistanceFrom(Client.ClientLocation))
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
            foreach (var creature in creatureList.OrderBy(c => c.Location.DistanceFrom(Client.ClientLocation)))
            {

                if (!creatureList.Contains(this.creature))
                {
                    this.creature = creatureList.OrderBy(c => c.Location.DistanceFrom(Client.ClientLocation)).FirstOrDefault<Creature>();
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
                //Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                //    ? eligibleCreatures.OrderBy(creature => creature.Location.DistanceFrom(Client.ServerLocation)).FirstOrDefault()
                //    : eligibleCreatures.FirstOrDefault();

                Creature targetCreature;

                if (enemyPage.NearestFirstCbx.Checked && !enemyPage.FarthestFirstCbx.Checked)
                {
                    // Select the nearest eligible creature
                    targetCreature = eligibleCreatures
                        .OrderBy(creature => creature.Location.DistanceFrom(Client.ServerLocation))
                        .FirstOrDefault();
                }
                else if (enemyPage.FarthestFirstCbx.Checked && !enemyPage.NearestFirstCbx.Checked)
                {
                    // Select the farthest eligible creature
                    targetCreature = eligibleCreatures
                        .OrderByDescending(creature => creature.Location.DistanceFrom(Client.ServerLocation))
                        .FirstOrDefault();
                }
                else
                {
                    // If neither checkbox is checked or both are checked, select any eligible creature
                    targetCreature = eligibleCreatures.FirstOrDefault();
                }

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
                //Creature targetCreature = enemyPage.NearestFirstCbx.Checked
                //    ? eligibleCreatures.OrderBy(creature => creature.Location.DistanceFrom(Client.ServerLocation)).FirstOrDefault()
                //    : eligibleCreatures.FirstOrDefault();
                
                Creature targetCreature;

                if (enemyPage.NearestFirstCbx.Checked && !enemyPage.FarthestFirstCbx.Checked)
                {
                    // Select the nearest eligible creature
                    targetCreature = eligibleCreatures
                        .OrderBy(creature => creature.Location.DistanceFrom(Client.ServerLocation))
                        .FirstOrDefault();
                }
                else if (enemyPage.FarthestFirstCbx.Checked && !enemyPage.NearestFirstCbx.Checked)
                {
                    // Select the farthest eligible creature
                    targetCreature = eligibleCreatures
                        .OrderByDescending(creature => creature.Location.DistanceFrom(Client.ServerLocation))
                        .FirstOrDefault();
                }
                else
                {
                    // If neither checkbox is checked or both are checked, select any eligible creature
                    targetCreature = eligibleCreatures.FirstOrDefault();
                }
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
                return attackList.OrderBy(c => c.Location.DistanceFrom(Client.ServerLocation)).FirstOrDefault();
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
                    else if (flag && creature.Location.DistanceFrom(Client.ServerLocation) <= GetFurthestClient())
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
            if (creature.HealthPercent == 0 
                && creature.AnimationHistory.Count != 0 
                && creature.LastAnimation != (byte)SpellAnimation.Miss 
                && DateTime.UtcNow.Subtract(creature.LastAnimationTime).TotalSeconds <= 1.5)
            {
                return creature._hitCounter < enemyPage.expectedHitsNum.Value;
            }
            if (creature.HealthPercent != 0 
                && creature._hitCounter > enemyPage.expectedHitsNum.Value)
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

                    result = 11 - furthestClient.ServerLocation.DistanceFrom(Client.ServerLocation);
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
            return client.ServerLocation.DistanceFrom(Client.ServerLocation);
        }

        private bool ExecutePramhStrategy(EnemyPage enemyPage, List<Creature> creatures)
        {
            List<Creature> creatureList = FilterCreaturesByControlStatus(enemyPage, creatures);
            if (CONSTANTS.GREEN_BOROS.Contains(enemyPage.Enemy.SpriteID))
            {
                List<Creature> greenBorosInRange = Client.GetAllNearbyMonsters(8, CONSTANTS.GREEN_BOROS.ToArray());
                foreach (Creature creature in greenBorosInRange.ToList<Creature>())
                {
                    foreach (Location location in Client.GetWarpPoints(new Location(Client.Map.MapID, 0, 0)))
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
                    .Where(creature => !Client.GetWarpPoints(new Location(Client.Map.MapID, 0, 0))
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
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return;
            }

            bool shouldUseGem = clientTab.autoGemCbox.Checked &&
                                Client.Experience > 4250000000U &&
                                DateTime.UtcNow.Subtract(_lastUsedGem).TotalMinutes > 5.0;

            if (!shouldUseGem)
            {
                return;
            }

            // Determine the gem type based on the selected text, then use the gem.
            byte choice = clientTab.expGemsCombox.Text == "Ascend HP" ? (byte)1 : (byte)2;
            Client.UseExperienceGem(choice);
        }

        private void UpdatePlayersListBasedOnStrangers()
        {
            _playersExistingOver250ms = !IsStrangerNearby() ? _playersExistingOver250ms : _playersExistingOver250ms.Where(p => DateTime.UtcNow.Subtract(p.Creation).TotalSeconds > 2.0).ToList();
        }

        private void CheckFasSpioradRequirement()
        {
            int requiredMp;
            var clientTab = Client.ClientTab;
            if (clientTab == null)
            {
                return;
            }

            if (clientTab.fasSpioradCbox.Checked && int.TryParse(clientTab.fasSpioradText.Text.Trim(), out requiredMp) && Client.ManaPct < requiredMp)
            {
                _needFasSpiorad = true;
            }
        }

        private void ManageSpellCasting()
        {
            DateTime utcNow = DateTime.UtcNow;
            while (Client.SpellHistory.Count >= 3 || Client.SpellCounter >= 3)
            {
                if (DateTime.UtcNow.Subtract(utcNow).TotalSeconds > 1.0)
                {
                    Client.SpellHistory.Clear();
                }
                Thread.Sleep(10);
            }
            if (DateTime.UtcNow.Subtract(_spellTimer).TotalSeconds > 1.0)
            {
                Client.SpellHistory.Clear();
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

        internal void AddAlly(Ally ally)
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
            var clientTab = Client.ClientTab;

            if (clientTab == null || Client.ClientTab == null)
            {
                return;
            }

            bool isPickupGoldChecked = clientTab.pickupGoldCbox.Checked;
            bool isPickupItemsChecked = clientTab.pickupItemsCbox.Checked;
            bool isDropTrashChecked = clientTab.dropTrashCbox.Checked;

            if (!isPickupGoldChecked && !isPickupItemsChecked && !isDropTrashChecked)
            {
                return;
            }

            try
            {
                if (!IsStrangerNearby())
                {
                    var lootArea = new Structs.Rectangle(new Point(Client.ServerLocation.X - 2, Client.ServerLocation.Y - 2), new Point(5, 5));
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
                            Client.Drop(item.Slot, Client.ServerLocation, item.Quantity);
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
