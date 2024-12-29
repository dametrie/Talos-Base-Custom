using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Talos.Base;
using Talos.Definitions;
using Talos.Enumerations;
using Talos.Objects;
using Talos.Structs;
using Talos.Utility;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Talos.Bashing
{
    internal abstract class BashingBase
    {
        protected DateTime LastNeckSwap = DateTime.UtcNow.Subtract(TimeSpan.FromHours(5.0));
        protected DateTime LastPointInSync = DateTime.UtcNow;
        protected DateTime LastDirectionInSync = DateTime.UtcNow;
        protected DateTime LastAssailed = DateTime.UtcNow;
        protected DateTime LastSendTarget = DateTime.UtcNow;
        protected DateTime LastUsedSkill { get; set; } = DateTime.Now;

        protected List<Player> NearbyPlayers { get; set; }
        protected List<Creature> NearbyMonsters { get; set; }
        protected List<Creature> KillableTargets { get; set; }

        protected Bot Bot { get; }
        protected Client Client => Bot.Client;

        protected int MonsterWalkIntervalMs => Convert.ToInt32(Client.ClientTab.monsterWalkIntervalNum1.Value);
        protected int PingCompensation => Convert.ToInt32(Client.ClientTab.pingCompensationNum1.Value);
        protected bool BashAsgall => Client.ClientTab.chkBashAsgall.Checked;
        protected int SkillIntervalMs => Convert.ToInt32(Client.ClientTab.numSkillInt.Value);
        protected int SendTargetIntervalMs { get; set; } = 250;

        protected bool RequireDionForRiskySkills => Client.ClientTab.riskySkillsDionCbox.Checked;
        protected bool UseRiskySkills => Client.ClientTab.riskySkillsCbox.Checked;
        protected bool UseCrasher => Client.ClientTab.crasherCbox.Checked;
        protected HashSet<Location> Warps { get; set; }
        public Creature Target { get; set; }
        protected List<ushort> PrioritySprites { get; set; }
        protected bool PriorityOnly => Client.ClientTab.priorityOnlyCbox.Checked;

        protected virtual bool CanMove
            => !Client.HasEffect(EffectsBar.BeagSuain)
               && !Client.HasEffect(EffectsBar.Pramh)
               && !Client.HasEffect(EffectsBar.Suain);

        protected virtual bool CanPerformActions
            => !Client.HasEffect(EffectsBar.Pramh)
               && !Client.HasEffect(EffectsBar.Suain);


        internal BashingBase(Bot bot)
        {
            Bot = bot;
        }


        internal virtual bool DoBashing()
        {
            try
            {
                if (!CanPerformActions)
                    return true;

                Update();

                // Get filtered monsters and ordered potential targets
                KillableTargets = FilterMonstersByCursedFased()?.ToList();
                if (!ValidateKillableTargets())
                    return false;

                var potentialTargets = GetOrderedPotentialTargets()?.ToList();
                if (!ValidatePotentialTargets(potentialTargets))
                    return false;

                // Select the first valid target
                Target = potentialTargets.FirstOrDefault(MeetsKillCriteria) ?? potentialTargets.FirstOrDefault();
                if (Target == null)
                    return false;

                // Manage target selection timing
                if (ShouldSendBashingTarget())
                    SendBashingTarget(Target);

                // Handle movement towards the target
                if (!HandleMovement(Target))
                    return false;

                // Perform refresh or equip actions if needed
                if (RefreshIfNeeded() || EquipNecklaceIfNeeded(Target))
                    return true;

                // Perform bashing if enabled
                if (Client.Bot?._dontBash == false)
                    UseSkills(Target);

                return true;
            }
            catch (Exception ex)
            {
                Log($"Exception in DoBashing: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }



        internal void Update()
        {
            NearbyPlayers = Client.GetNearbyPlayers();
            NearbyMonsters = Client.GetNearbyValidCreatures(10);
            Warps = Client.GetAllWarpPoints();

            if (!Client.ClientTab.priorityCbox.Checked)
            {
                PrioritySprites = new List<ushort>();
            }
            else
            {
                PrioritySprites = Client.ClientTab.priorityLBox.Items
                                    .OfType<string>()
                                    .Select(ushort.Parse)
                                    .ToList();
            }

        }
        private bool ValidateKillableTargets()
        {
            if (KillableTargets == null)
            {
                Log("KillableTargets is null.");
                return false;
            }
            return true;
        }

        private bool ValidatePotentialTargets(List<Creature> potentialTargets)
        {
            if (potentialTargets == null)
            {
                Log("potentialTargets is null.");
                return false;
            }
            if (!potentialTargets.Any())
                return false;

            return true;
        }

        private bool ShouldSendBashingTarget()
        {
            return (DateTime.UtcNow - LastSendTarget).TotalMilliseconds > SendTargetIntervalMs;
        }

        private void SendBashingTarget(Creature target)
        {
            Client.DisplayTextOverTarget(2, target.ID, "[Target]");
            LastSendTarget = DateTime.UtcNow;
        }

        private bool HandleMovement(Creature target)
        {
            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);

            if (distanceToTarget == 0)
                return TryUnstuck();

            if (distanceToTarget > 1)
            {
                if (!ShouldPathfindToTarget(target, distanceToTarget))
                    return true;

                if (!TryPathfindToPoint(target.Location))
                    return false;
            }
            else if (!TryFaceTarget(target))
            {
                return true;
            }

            return true;
        }

        private bool ShouldPathfindToTarget(Creature target, int distance)
        {
            if (distance <= 3 && target.Direction == Client.ClientLocation.GetDirection(target.Location))
            {
                var lastStepTime = DateTime.UtcNow - target.LastStep;
                return !(lastStepTime > TimeSpan.FromMilliseconds(MonsterWalkIntervalMs - PingCompensation * 2) &&
                         lastStepTime < TimeSpan.FromMilliseconds(MonsterWalkIntervalMs + PingCompensation * 2));
            }

            return Bot._nearbyAllies?.Any(u => u.IsAsleep) ?? false;
        }


        protected IEnumerable<Player> GetStrangers()
        {
            // Create a HashSet with OrdinalIgnoreCase for fast lookups
            var friendSet = new HashSet<string>(Client.FriendBindingList, StringComparer.OrdinalIgnoreCase);

            return NearbyPlayers.Where(user => !friendSet.Contains(user.Name));
        }

        protected virtual IEnumerable<Creature> FilterMonstersByCursedFased(
             IEnumerable<Creature> monsters = null)
        {
            // Use the provided list or default to NearbyMonsters
            var creatureList = monsters ?? NearbyMonsters;

            return creatureList.Where(monster =>
                (BashAsgall || !monster.IsAsgalled) && 
                (!Client.ClientTab.chkWaitForCradh.Checked || monster.IsCursed) && // Filter if 'WaitForCradh' is enabled
                (!Client.ClientTab.chkWaitForFas.Checked || monster.IsFassed)); // Filter if 'WaitForFas' is enabled
        }

        protected virtual bool MeetsKillCriteria(Creature monster)
        {
            bool isAsgallConditionMet = !monster.IsAsgalled || BashAsgall;
            bool waitForCradhConditionMet = !Client.ClientTab.chkWaitForCradh.Checked || monster.IsCursed;
            bool waitForFasConditionMet = !Client.ClientTab.chkWaitForFas.Checked || monster.IsFassed;

            return isAsgallConditionMet && waitForCradhConditionMet && waitForFasConditionMet;
        }

        internal virtual bool ShouldUseSkillsOnTarget(Creature target)
        {
            TimeSpan timeSinceLastStep = DateTime.UtcNow - target.LastStep;

            if (IsFacingUs(target) || IsFacingAway(target))
            {
                TimeSpan lowerBound = TimeSpan.FromMilliseconds(Math.Max(50, MonsterWalkIntervalMs - PingCompensation * 2));
                TimeSpan upperBound = TimeSpan.FromMilliseconds(MonsterWalkIntervalMs + PingCompensation);

                return timeSinceLastStep < lowerBound || timeSinceLastStep > upperBound;
            }

            TimeSpan generalLowerBound = TimeSpan.FromMilliseconds(MonsterWalkIntervalMs - PingCompensation * 2);
            TimeSpan generalUpperBound = TimeSpan.FromMilliseconds(MonsterWalkIntervalMs + PingCompensation * 2);

            return timeSinceLastStep < generalLowerBound || timeSinceLastStep > generalUpperBound;
        }
        internal bool CanUseRiskySkills()
        {
            // Check if risky skills are enabled
            if (!UseRiskySkills)
            {
                return false;
            }

            // Check for Dion requirement
            if (RequireDionForRiskySkills && !Client.Player.IsDioned)
            {
                return false;
            }

            // Check for pramhed/suained status or low MP in follow chain
            bool hasDisabledFollowers = Client.Server
                .GetFollowChain(Client)
                .Any(cli => cli.Player.IsAsleep || cli.Player.IsSuained || cli.CurrentMP < 10000U);

            if (hasDisabledFollowers)
            {
                return false;
            }

            // Check if too many surrounding creatures exist
            int surroundingCreaturesCount = GetSurroundingCreatures(NearbyMonsters)
                .Count(mob => mob.Location != Client.ClientLocation);

            if (surroundingCreaturesCount > 3)
            {
                return false;
            }

            // Final Dion length check if required
            if (RequireDionForRiskySkills)
            {
                DateTime lastDioned = Client.Player.GetState<DateTime>(CreatureState.LastDioned);
                double dionDuration = Client.Player.GetState<double>(CreatureState.DionDuration);
                double dionTimeRemaining = dionDuration - (DateTime.UtcNow - lastDioned).TotalSeconds;


                if (dionTimeRemaining <= 2.0)
                {
                    return false;
                }
            }

            return true;
        }

        internal abstract void UseSkills(Creature target);
        internal virtual void UseAssails()
        {
            if (!Client.ClientTab.chkBashAssails.Checked)
                return;

            Client.Assail();   
        }
        protected virtual IEnumerable<Creature> GetOrderedPotentialTargets(IEnumerable<Creature> monsters = null)
        {
            // Use provided monsters or fallback to NearbyMonsters
            IEnumerable<Creature> creatures = monsters ?? NearbyMonsters;
            IEnumerable<Player> strangers = GetStrangers();

            // Step 1: Order the creatures based on multiple criteria
            var orderedCreatures = creatures
                .OrderBy(mob => GetPriorityDistance(mob))            // Prioritize by distance and sprite priority
                .ThenBy(mob => GetCursedAdjustedDistance(mob))       // Adjust for cursed & fas-nadured state
                .ThenBy(mob => mob.HealthPercent)                    // Sort by health percentage
                .ThenBy(mob => mob.Creation)                        // Finally, sort by creation time
                .AsEnumerable();

            // Step 2: Filter creatures if PriorityOnly is enabled
            if (PriorityOnly)
            {
                orderedCreatures = orderedCreatures.Where(mob => PrioritySprites.Contains(mob.SpriteID));
            }

            // Step 3: Apply additional filtering logic
            return orderedCreatures.Where(mob => IsValidTarget(mob, strangers));
        }

        private int GetPriorityDistance(Creature mob)
        {
            // Apply sprite priority logic: prioritized sprites have normal distance, others are tripled
            return PrioritySprites.Contains(mob.SpriteID)
                ? mob.Location.DistanceFrom(Client.ClientLocation)
                : mob.Location.DistanceFrom(Client.ClientLocation) * 3;
        }

        private int GetCursedAdjustedDistance(Creature mob)
        {
            // Reduce distance slightly if the creature is both cursed and fas-nadured
            return mob.IsCursed && mob.IsFassed
                ? mob.Location.DistanceFrom(Client.ClientLocation) - 1
                : mob.Location.DistanceFrom(Client.ClientLocation);
        }

        private bool IsValidTarget(Creature mob, IEnumerable<Player> strangers)
        {
            // Check if the mob is blocked or invalid
            if (Client.IsLocationSurrounded(mob.Location))
                return false;

            // Ensure no strangers or nearby users within range
            bool hasNearbyStrangers = strangers.Any(stranger => stranger.WithinRange(mob, 4));
            bool hasNearbyUsers = NearbyPlayers.Any(user =>
                user == Client.Player
                    ? Client.ClientLocation.DistanceFrom(mob.Location) == 0
                    : user.WithinRange(mob, 0));

            if (hasNearbyStrangers || hasNearbyUsers)
                return false;

            // Check pathfinding distance (1 for adjacent, otherwise validate path)
            if (Client.ClientLocation.DistanceFrom(mob.Location) == 1)
                return true;

            int pathCount = Client.Pathfinder.FindPath(Client.ServerLocation, mob.Location).Count;
            return pathCount > 0 && pathCount < 15;
        }
        protected virtual bool IsFacingAway(Creature target)
        {
            return target.Location.GetDirection(Client.ClientLocation) == target.Direction;
        }

        protected virtual bool IsFacingUs(Creature target)
        {
            return Client.ClientLocation.GetDirection(target.Location) == target.Direction;
        }

        protected virtual bool TryFaceTarget(Creature target)
        {
            if (!CanPerformActions)
                return false;

            DateTime currentTime = DateTime.UtcNow;
            Direction targetDirection = target.Location.GetDirection(Client.ClientLocation);

            bool isOutOfSync = Client.ClientDirection != Client.ServerDirection;
            bool shouldResync = isOutOfSync &&
                                (currentTime - LastDirectionInSync).TotalMilliseconds > (1000 + PingCompensation * 2);

            // Resynchronize if needed
            if (shouldResync)
            {
                Client.ClientDirection = Client.ServerDirection;
                LastDirectionInSync = currentTime;
            }
            else if (!isOutOfSync)
            {
                LastDirectionInSync = currentTime;
            }

            // Adjust direction to face the target
            if (shouldResync || Client.ClientDirection != targetDirection)
            {
                Client.Turn(targetDirection);
            }

            return true;
        }

        protected virtual bool TryPathfindToPoint(Location location, short distance = 1)
        {
            return CanMove && !Client.Bot._dontWalk && Client.Pathfind(location, distance);
        }

        protected IEnumerable<Creature> GetSurroundingCreatures(IEnumerable<Creature> creatures = null, int skillRange = 1)
        {
            creatures ??= KillableTargets ?? NearbyMonsters;
            return creatures.Where(mob => mob.Location.DistanceFrom(Client.ClientLocation) <= skillRange);
        }

        internal bool SharesAxis(Location loc1, Location loc2)
        {
            return loc1.MapID == loc2.MapID &&
                   (loc1.X == loc2.X || loc1.Y == loc2.Y);
        }

        internal virtual bool EquipNecklaceIfNeeded(Creature target)
        {
            if (!Client.Map.Name.Contains("Shinewood Forest"))
                return false;

            bool canSwapNecklace = DateTime.UtcNow.Subtract(LastNeckSwap).TotalMilliseconds > (1000 + PingCompensation * 2);

            if (canSwapNecklace)
            {
                if (CONSTANTS.SHINEWOOD_HOLY.Contains(target.SpriteID) && !Client.OffenseElement.Equals("Light", StringComparison.OrdinalIgnoreCase))
                {
                    Client.EquipLightNeck();
                    LastNeckSwap = DateTime.UtcNow;
                    return true;
                }

                if (CONSTANTS.SHINEWOOD_DARK.Contains(target.SpriteID) && !Client.OffenseElement.Equals("Dark", StringComparison.OrdinalIgnoreCase))
                {
                    Client.EquipDarkNeck();
                    LastNeckSwap = DateTime.UtcNow;
                    return true;
                }
            }

            return false;
        }

        internal bool TryComboWithSleepSkill(string name, bool isItem = false)
        {
            if (!UseSleepSkill())
                return false;

            return isItem ? TryUseItem(name) : TryUseSkill(name);
        }

        private bool UseSleepSkill()
        {
            return Client.UseSkill("Lullaby Punch") || Client.UseSkill("Wolf Fang Fist");
        }

        private bool TryUseSkill(string skillName)
        {
            Skill skill = Client.Skillbook[skillName] ?? Client.Skillbook.GetNumberedSkill(skillName);
            if (skill == null || !skill.CanUse)
                return false;

            Client.UseSkill(skill.Name);
            return true;
        }

        private bool TryUseItem(string itemName)
        {
            Item item = Client.Inventory[itemName];
            if (item == null)
                return false;

            Client.UseItem(item.Name);
            return true;
        }
        protected virtual bool TryUnstuck()
        {
            if (!CanMove)
                return false;

            Location currentLoc = Client.ClientLocation;

            // Randomize the directions and check for walkability
            var randomDirections = Enum.GetValues(typeof(Direction))
                                       .Cast<Direction>()
                                       .OrderBy(_ => RandomUtils.Random());

            foreach (var dir in randomDirections)
            {
                var targetPoint = currentLoc.Offsetter(dir);
                if (Client.Map.IsWalkable(Client, targetPoint))
                {
                    Client.Walk(dir);
                    return true;
                }
            }

            return false;
        }
        internal virtual bool RefreshIfNeeded()
        {
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan pingDelay = TimeSpan.FromMilliseconds(PingCompensation * 2);

            // Refresh if no activity in the last 5 seconds
            if (IsRefreshRequired(currentTime))
            {
                Client.RefreshRequest();
                LastPointInSync = currentTime;
                return true;
            }

            // Refresh if Client and Server points are out of sync
            if (Client.ClientLocation != Client.ServerLocation)
            {
                if ((currentTime - LastPointInSync).TotalMilliseconds > 1000)
                {
                    WaitForSync(pingDelay, currentTime);

                    if (Client.ClientLocation != Client.ServerLocation)
                        Client.RefreshRequest();

                    LastPointInSync = DateTime.UtcNow;
                    return true;
                }
            }
            else
            {
                LastPointInSync = currentTime;
            }

            return false;
        }

        private bool IsRefreshRequired(DateTime currentTime)
        {
            return (currentTime - Client.LastStep).TotalSeconds > 5.0 &&
                   (currentTime - Client.Bot._lastEXP).TotalSeconds > 5.0 &&
                   (currentTime - Client.Bot._lastRefresh).TotalSeconds > 5.0;
        }

        private void WaitForSync(TimeSpan pingDelay, DateTime startTime)
        {
            while ((DateTime.UtcNow - startTime) < pingDelay)
            {
                Thread.Sleep(25);
                if (Client.ClientLocation == Client.ServerLocation)
                    break;
            }
        }

        private void Log(string message)
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bashCrashLogs");
            string fileName = DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss-tt") + ".log";
            string logFilePath = Path.Combine(logDirectory, fileName);

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            File.WriteAllText(logFilePath, message);
            Console.WriteLine(message);
        }


    }
}

