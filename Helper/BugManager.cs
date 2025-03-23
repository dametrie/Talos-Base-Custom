using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Threading;
using Talos.Base;
using Talos.Definitions;
using Talos.Objects;
using Talos.Structs;
using Talos.Utility;

namespace Talos.Helper
{
    internal class BugMap
    {
        internal string ShopName { get; }
        internal short ShopMapID { get; }
        internal short BugsMapID { get; }
        internal List<string> BugList { get; }
        internal BugMap(string shopName, short shopMapId, short bugsMapId, List<string> bugList)
        {
            this.ShopName = shopName;
            this.ShopMapID = shopMapId;
            this.BugsMapID = bugsMapId;
            this.BugList = bugList;
        }
    }

    internal class BugManager
    {
        private readonly Client _client;
        private readonly List<BugMap> _bugMaps;
        private bool IsInsectNetEquipped =>
            _client.EquippedItems.Any(item => item != null && item.Name == "Insect Net");

        CommandManager commandManager = CommandManager.Instance;

        public BugManager(Client client)
        {
            _client = client;
            _bugMaps = new List<BugMap>
            {
                new BugMap("Mileth Armor Shop", 133, 8999, new List<string>
                {
                    "Assassin Bug", "Fungus Beetle", "Katydid", "Mealybug", "Weevil"
                }),
                new BugMap("Octavio's Armory", 6513, 8998, new List<string>
                {
                    "Brown Millipede", "Grey Millipede", "Leaf Beetle", "Leafhopper", "Robberfly"
                }),
                new BugMap("Asilon Weapon Shop", 10001, 8996, new List<string>
                {
                    "Bee Fly", "Gold Beetle", "Leaf Bug", "Lynx Spider", "Moss Mantid"
                }),
                new BugMap("Hwarone City", 10265, 8997, new List<string>
                {
                    "Blow Fly", "Carpenter Ant", "Cicada", "Tiger Mosquito", "Weaver Ant"
                })
            };
        }


        // State flags and variables.
        private bool _netRepair;
        private bool _generateRandom;
        private int _randomX;
        private int _randomY;
        private Creature _targetCreature;
        private DateTime _lastTurned;
        private ushort _bugSprite = 649;

        // Additional supporting members.
        private Dictionary<int, DateTime> blacklist = new Dictionary<int, DateTime>();


        public void BugLoop()
        {
            // Run only when bug hunting is enabled.
            if (_client.ClientTab?.toggleBugBtn.Text != "Disable")
                return;
            
            if (IsInsectNetEquipped)
            {
                _client.Bot.HasInsectNet = true;
            }

            // Ensure the Insect Net is available.
            if (!_client.HasItem("Insect Net") && !_client.Bot.HasInsectNet)
            {
                GetInsectNet();
                return;
            }
            else if (_client.HasItem("Insect Net") && !_client.Bot.HasInsectNet)
            {
                _client.UseItem("Insect Net");
                _client.Bot.HasInsectNet = true;
                return;
            }

            // Process the first incomplete bug map.
            var bugMapToProcess = _bugMaps.FirstOrDefault(map => !IsBugMapCompleted(map));
            if (bugMapToProcess != null)
            {
                ProcessBugMap(bugMapToProcess);
            }
            else
            {
                BugsCompleted();
            }
        }

        private bool IsBugMapCompleted(BugMap bugMap)
        {
            int completedCount = bugMap.BugList.Count(bug => _client.GetItemQuantity(bug) == 10);
            return completedCount == bugMap.BugList.Count;
        }

        private void ProcessBugMap(BugMap bugMap)
        {
            if (_client.Map.MapID != bugMap.ShopMapID && _client.Map.MapID != bugMap.BugsMapID)
            {
                // Not at the expected maps; route to the starting map.
                _client.Routefind(new Location(bugMap.ShopMapID, new Point(7, 7)), 0, false, true, true);
            }
            else if (_client.Map.MapID == bugMap.ShopMapID)
            {
                EnterBugMap();
            }
            else if (_client.Map.MapID == bugMap.BugsMapID)
            {
                if (_client.Bot.InsectNetRepair)
                {
                    _client.Routefind(new Location(bugMap.ShopMapID, new Point(7, 8)), 0, false, true, true);
                }
                else
                {
                    SmartBashBugsFowls(true);
                }
            }
        }

        private void BugsCompleted()
        {
            _client.ServerMessage(0, "Bugs Completed!");
            _client.ServerMessage(0, "Go get your reward!");
            SystemSounds.Beep.Play();
            Thread.Sleep(500);
        }

        private void GetInsectNet()
        {
            _client.RemoveShield();
            var targetLocation = new Location(165, new Point(6, 10));
            if (!Location.Equals(_client.ClientLocation, targetLocation))
            {
                _client.Routefind(targetLocation, 0, false, true, true);
                return;
            }
            PurchaseInsectNet();
            Thread.Sleep(2000);
        }

        private void PurchaseInsectNet()
        {
            var npc = _client.GetNearbyNPC("Gunnar");
            if (npc == null) 
                return;

            if (_client.RequestNamedPursuit(npc, "Enter Insect Tournament", true))
            {
                _client.ReplyDialog(1, npc.ID, 0, 2, 1);
                _client.ReplyDialog(1, npc.ID, 0, 2);
                _client.ReplyDialog(1, npc.ID, 0, 2);
                _client.ReplyDialog(1, npc.ID, 0, 2, 1);
                _client.ReplyDialog(1, npc.ID, 0, 2);
                _client.ReplyDialog(1, npc.ID, 0, 2);
                _client.ReplyDialog(1, npc.ID, 0, 2);
                _client.ReplyDialog(1, npc.ID, 0, 2);
                _client.ReplyDialog(1, npc.ID, 0, 2);
                _client.ReplyDialog(1, npc.ID, 0, 2);
                _client.ReplyDialog(1, npc.ID, 0, 2, 1);
                _client.ReplyDialog(1, npc.ID, 0, 1);
            }

        }

        private void EnterBugMap()
        {
            // Map-specific navigation.
            switch (_client.Map.MapID)
            {
                case 10265:
                    if (_client.ServerLocation.AbsoluteXY(90, 47) > 1)
                    {
                        _client.Pathfind(new Location(10265, 90, 47), 1);
                        return;
                    }
                    break;
                case 10001:
                    if (_client.ServerLocation.AbsoluteXY(8, 8) > 1)
                    {
                        _client.Pathfind(new Location(10001, 8, 8), 1);
                        return;
                    }
                    break;
                case 6513:
                    if (_client.ServerLocation.AbsoluteXY(10, 10) > 1)
                    {
                        _client.Pathfind(new Location(6513, 10, 10), 1);
                        return;
                    }
                    break;
                case 133:
                    if (_client.ServerLocation.AbsoluteXY(8, 6) > 1)
                    {
                        _client.Pathfind(new Location(133, 8, 6), 1);
                        return;
                    }
                    break;
            }

            var npcNames = new List<string> { "Torrance", "Octavio", "Mank", "Akum" };
            Creature npc = null;
            string dialogCommand = "Enter Hunting Ground";

            foreach (var name in npcNames)
            {
                npc = _client.GetNearbyNPC(name);
                if (npc == null)
                    continue;

                if (_client.Server.PursuitIDs.Values.Contains($"{dialogCommand}{npcNames.IndexOf(name) + 1}"))
                    break;

                _client.ClickObject(npc.ID);
                var startTime = DateTime.UtcNow;
                while (_client.Dialog == null)
                {
                    if ((DateTime.UtcNow - startTime).TotalSeconds > 2)
                        return;
                    Thread.Sleep(10);
                }
                _client.Dialog.Reply();
                break;
            }

            if (npc != null)
            {
                commandManager.ExecuteCommand(_client, "/repair all");
                Thread.Sleep(500);
                _client.Bot.InsectNetRepair = false;
                ProcessNpcDialog(npc, dialogCommand);
                Thread.Sleep(1000);
            }
        }

        private void ProcessNpcDialog(Creature npc, string dialogCommand)
        {
            var pursuitSuffix = npc.Name switch
            {
                "Torrance" => "1",
                "Octavio" => "2",
                "Mank" => "3",
                "Akum" => "4",
                _ => null
            };

            if (pursuitSuffix != null)
            {
                var pursuitId = _client.Server.PursuitIDs.FirstOrDefault(pair => pair.Value == $"{dialogCommand}{pursuitSuffix}").Key;
                if (pursuitId != 0)
                {
                    _client.PursuitRequest(1, npc.ID, pursuitId);
                    _client.ReplyDialog(1, npc.ID, 0, 2, 1);
                }
            }
        }

        private void SmartBashBugsFowls(bool bugs = true)
        {
            if (bugs && GetBugOnGroundTarget())
                return;

            EnsureSynchronizedLocation();

            if (bugs)
                GetBugTarget();
            else
                GetFowlTarget();

            if (_targetCreature != null)
            {
                if (IsPlayerNearby(_targetCreature))
                {
                    blacklist[_targetCreature.ID] = DateTime.UtcNow;
                    return;
                }

                _generateRandom = false;
                var direction = _targetCreature.Location.GetDirection(_client.ClientLocation);
                HandleCreatureMovement(direction);
                HandleCreatureSkills(direction);
            }
            else
            {
                HandleRandomMovement();
            }
        }
        private bool GetBugOnGroundTarget()
        {
            try
            {
                // Retrieve nearby ground items, filter by inventory count, order by distance, and get the closest one.
                var target = _client.GetNearbyGroundItems(12, CONSTANTS.BUG_NUMBERS.ToArray())
                                   .Where(g => _client.Inventory.CountOf(g.SpriteID) < 10)
                                   .OrderBy(g => g.Location.DistanceFrom(_client.ClientLocation))
                                   .FirstOrDefault();

                if (target == null)
                    return false;

                // If the target is far (distance >= 3), pathfind to it; otherwise, pick it up.
                if (target.Location.DistanceFrom(_client.ClientLocation) >= 3)
                    _client.Pathfind(target.Location);
                else
                    _client.Pickup(0, target.Location);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private bool IsPlayerNearby(Creature creature)
        {
            return _client.GetNearbyPlayers()
                          .Where(p => p != _client.Player)
                          .Any(p => p.Location.DistanceFrom(creature.Location) <= 2);
        }

        private void EnsureSynchronizedLocation()
        {
            if (!Location.Equals(_client.ClientLocation, _client.ServerLocation) && _client.StepCount % 8 == 0)
            {
                var startTime = DateTime.UtcNow;
                while (!Location.Equals(_client.ClientLocation, _client.ServerLocation))
                {
                    if ((DateTime.UtcNow - startTime).TotalMilliseconds > 800)
                        _client.RefreshRequest(true);
                    Thread.Sleep(50);
                }
            }

            _client.Bot.RefreshLastStep();
        }

        private void HandleCreatureMovement(Direction direction)
        {
            double distance = _client.ClientLocation.DistanceFrom(_targetCreature.Location);
            double elapsedTarget = (DateTime.UtcNow - _targetCreature.GetState<DateTime>(CreatureState.LastStep)).TotalSeconds;
            double elapsedTurn = (DateTime.UtcNow - _lastTurned).TotalSeconds;
            Direction desiredDir = _targetCreature.Location.GetDirection(_client.ClientLocation);
            Direction clientDir = _client.ClientDirection; // Or use _client.ServerDirection if that is the intended value

            //Console.WriteLine($"[Debug] Distance: {distance}, ElapsedTarget: {elapsedTarget:F2}s, ElapsedTurn: {elapsedTurn:F2}s");
            //Console.WriteLine($"[Debug] DesiredDir: {desiredDir}, ClientDir: {clientDir}");
            //Console.WriteLine($"[Debug] Target actual direction: {_targetCreature.Direction}, " +
            //                  $"Client Location: {_client.ClientLocation}, Target Location: {_targetCreature.Location}");


            bool prePathfindCondition = (distance <= 2) && (elapsedTarget <= 2.0) &&
                (_targetCreature.Direction != clientDir ||
                 _client.ClientLocation.X != _targetCreature.Location.X ||
                 _client.ClientLocation.Y != _targetCreature.Location.Y);

            if (prePathfindCondition)
            {
                if (distance == 1 && desiredDir != clientDir && elapsedTurn > 2.0)
                {
                    Console.WriteLine("Pre-pathfind: Turning towards bug (first branch)");
                    _client.Turn(desiredDir);
                    _lastTurned = DateTime.UtcNow;
                }

                Console.WriteLine("Pre-pathfind: Calling Pathfind to target");
                _client.Pathfind(_targetCreature.Location, 1, true, true);
            }
            else
            {
                Console.WriteLine("No pre-pathfind conditions met: Calling Pathfind to target");
                _client.Pathfind(_targetCreature.Location, 1, true, true);
            }

            if (distance == 1)
            {
                if (elapsedTarget > 2.0 && desiredDir != clientDir)
                {
                    Console.WriteLine("Post-pathfind: Turning towards bug (second branch)");
                    _client.Turn(desiredDir);
                    _lastTurned = DateTime.UtcNow;
                }
            }
        }

        private void HandleCreatureSkills(Direction direction)
        {
            var now = DateTime.UtcNow;
            var clientLoc = _client.ClientLocation;
            var targetLoc = _targetCreature.Location;
            var distance = targetLoc.DistanceFrom(clientLoc);
            var timeDiff = (now - _targetCreature.GetState<DateTime>(CreatureState.LastStep)).TotalMilliseconds;
            var serverDir = _client.ServerDirection;
            var desiredDir = direction;
            bool alignedOnAxis = (targetLoc.X == clientLoc.X) || (targetLoc.Y == clientLoc.Y);

            //Console.WriteLine($"[HandleCreatureSkills] Distance: {distance}, TimeDiff: {timeDiff:F2}ms, " +
            //                  $"ServerDir: {serverDir}, DesiredDir: {desiredDir}, Aligned: {alignedOnAxis}");

            // Check if the client and target are aligned on one axis and facing the same direction.
            if (alignedOnAxis && desiredDir == serverDir)
            {

                if (distance == 1 && (timeDiff < 400 || timeDiff > 1000))
                {
                    foreach (var skill in CONSTANTS.ASSAILS)
                        _client.UseSkill(skill);
                }
                else
                {
                    if (distance <= 2)
                    {
                        foreach (var skill in CONSTANTS.TWO_TILE_ATTACKS_MED)
                            _client.NumberedSkill(skill);
                    }
                    if (distance <= 3)
                    {
                        foreach (var skill in CONSTANTS.THREE_TILE_ATTACKS_TEM)
                            _client.UseSkill(skill);
                        foreach (var skill in CONSTANTS.THREE_TILE_ATTACKS_MED)
                            _client.NumberedSkill(skill);
                    }
                    if (distance <= 4)
                    {
                        if (_client.TemuairClassFlag == TemuairClass.Monk &&
                            _client.HasItem("Sprint Potion") &&
                            (now - _client.Inventory.Find("Sprint Potion").LastUsed).TotalMilliseconds > 15000)
                        {
                            _client.UseItem("Sprint Potion");
                        }
                        else if (_client.TemuairClassFlag == TemuairClass.Warrior &&
                                 (now - _client.Skillbook["Charge"].LastUsed).TotalMilliseconds > 30000)
                        {
                            _client.UseSkill("Charge");
                        }
                    }
                    if (distance <= 5)
                    {
                        foreach (var skill in CONSTANTS.FIVE_TILE_ATTACKS_MED)
                            _client.NumberedSkill(skill);
                    }
                }
            }

            if ((now - _client.LastStep).TotalMilliseconds > 500 && distance <= 2)
            {
                foreach (var skill in CONSTANTS.ASSAILS)
                    _client.UseSkill(skill);
            }
        }


        private void HandleRandomMovement()
        {
            if (!_generateRandom)
            {
                _randomX = RandomUtils.Random(0, 28);
                _randomY = RandomUtils.Random(0, 28);
                _generateRandom = true;
            }

            var randomLocation = new Location(_client.Map.MapID, (short)_randomX, (short)_randomY);

            if (_client.Pathfinder.FindPath(_client.Player.Location, randomLocation, true, 0).Count == 0)
            {
                _generateRandom = false;
                return;
            }
            if (_client.ServerLocation.DistanceFrom(randomLocation) >= 1 && !_client.Map.IsWall(randomLocation))
            {
                _client.Pathfind(randomLocation, 0, true, true);
                return;
            }
            _generateRandom = false;
        }

        private void GetBugTarget()
        {
            try
            {
                var creatures = _client.GetAllNearbyMonsters(12, new ushort[] { _bugSprite })
                    .Where(creature => !blacklist.ContainsKey(creature.ID) ||
                                       DateTime.UtcNow.Subtract(blacklist[creature.ID]).TotalSeconds > 5.0)
                    .ToList();

                if (creatures.Any())
                {
                    if (_targetCreature == null || !creatures.Contains(_targetCreature))
                    {
                        _targetCreature = creatures.OrderBy(creature => creature.Location.DistanceFrom(_client.ServerLocation)).First();
                    }
                }
                else
                {
                    _targetCreature = null;
                }
            }
            catch
            {
                _targetCreature = null;
            }
        }

        private void GetFowlTarget()
        {
            // Implement fowl target retrieval logic, similar to GetBugTarget if needed.
        }

    }
}
