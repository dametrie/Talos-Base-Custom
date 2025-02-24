using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Talos.Base;
using Talos.Definitions;
using Talos.Definitions;
using Talos.Objects;
using Talos.Structs;
using Talos.Utility;

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

        protected bool RequireDionForCrashers => Client.ClientTab.riskySkillsDionCbox.Checked;
        protected bool UseCrashers => Client.ClientTab.chkCrasher.Checked;
        protected bool OnlyCrasherAsgall => Client.ClientTab.chkCrasherOnlyAsgall.Checked;


        protected HashSet<Location> Warps { get; set; }
        public Creature Target { get; set; }
        protected List<ushort> PrioritySprites { get; set; }
        protected bool PriorityOnly => Client.ClientTab.priorityOnlyCbox.Checked;
        internal bool EnableProtection { get; set; }
        internal string ProtectName1 { get; set; }
        internal string ProtectName2 { get; set; }

        internal bool AssistBasherEnabled { get; set; }
        internal string AssistBasherName { get; set; }
        public int AssistBasherStray { get; set; }
        protected virtual bool CanMove
            => !Client.HasEffect(EffectsBar.BeagSuain)
               && !Client.HasEffect(EffectsBar.Pramh)
               && !Client.HasEffect(EffectsBar.Suain);

        protected virtual bool CanPerformActions
            => !Client.HasEffect(EffectsBar.Pramh)
               && !Client.HasEffect(EffectsBar.Suain);


        /// <summary>
        /// Initializes a new instance of the <see cref="BashingBase"/> class with a reference to the current <see cref="Bot"/>.
        /// </summary>
        /// <param name="bot">The bot instance used by this bashing class.</param>
        internal BashingBase(Bot bot)
        {
            Bot = bot;
        }

        /// <summary>
        /// Performs the main bashing routine. Returns true if the routine finishes successfully, false otherwise.
        /// </summary>
        /// <returns>Boolean indicating success or failure of the bashing routine.</returns>
        internal virtual bool DoBashing()
        {
            try
            {
                //Console.WriteLine($"[DEBUG] [{Client.Name}] DoBashing started at {DateTime.UtcNow}");

                if (!CanPerformActions)
                {
                    //Console.WriteLine($"[DEBUG] [{Client.Name}] Cannot perform actions. Exiting DoBashing.");
                    return true;
                }

                Update();
                //Console.WriteLine($"[DEBUG] [{Client.Name}] Update complete. NearbyPlayers: {NearbyPlayers.Count}, NearbyMonsters: {NearbyMonsters.Count}");

                // Get filtered monsters and ordered potential targets
                KillableTargets = FilterMonstersByCursedFased()?.ToList();
                //Console.WriteLine($"[DEBUG] [{Client.Name}] Filtered KillableTargets: {(KillableTargets == null ? 0 : KillableTargets.Count)}");

                if (!ValidateKillableTargets())
                {
                    //Console.WriteLine($"[DEBUG] [{Client.Name}] ValidateKillableTargets failed. Exiting DoBashing.");
                    return false;
                }
                //Console.WriteLine($"[Bashing] [{Client.Name}] Found {KillableTargets.Count} valid targets.");

                var potentialTargets = GetOrderedPotentialTargets()?.ToList();
                //Console.WriteLine($"[DEBUG] [{Client.Name}] Potential targets count: {(potentialTargets == null ? 0 : potentialTargets.Count)}");

                if (!ValidatePotentialTargets(potentialTargets))
                {
                    Console.WriteLine($"[DEBUG] [{Client.Name}] ValidatePotentialTargets failed. Exiting DoBashing.");
                    return false;
                }
                //Console.WriteLine($"[Bashing] [{Client.Name}] Found {potentialTargets.Count} ordered potential targets.");

                // Select the first valid target
                Target = potentialTargets.FirstOrDefault(MeetsKillCriteria) ?? potentialTargets.FirstOrDefault();
                if (Target == null)
                {
                    //Console.WriteLine($"[DEBUG] [{Client.Name}] No valid target found. Exiting DoBashing.");
                    return false;
                }

                //Console.WriteLine($"[DEBUG] [{Client.Name}] Target selected: {Target.Name} at {Target.Location}");

                // Manage target selection timing
                if (ShouldSendBashingTarget())
                {
                    SendBashingTarget(Target);
                    //Console.WriteLine($"[DEBUG] [{Client.Name}] Sent bashing target update.");
                }

                // Handle movement towards the target
                if (!HandleMovement(Target))
                {
                    //Console.WriteLine($"[DEBUG] [{Client.Name}] HandleMovement failed. Exiting DoBashing.");
                    return false;
                }

                //Console.WriteLine($"[DEBUG] [{Client.Name}] Movement handled successfully.");

                Client.Bot.EnsureWeaponEquipped();
                //Console.WriteLine($"[DEBUG] [{Client.Name}] Weapon check complete.");

                // Perform refresh or equip actions if needed
                if (RefreshIfNeeded() || EquipNecklaceIfNeeded(Target))
                {
                    //Console.WriteLine($"[DEBUG] [{Client.Name}] Refresh or equip action executed. Exiting DoBashing for this cycle.");
                    return true;
                }

                // Perform bashing if enabled
                if (Client.Bot?._dontBash == false)
                {
                    //Console.WriteLine($"[DEBUG] [{Client.Name}] Using skills on target {Target.Name}");
                    UseSkills(Target);
                }

                //Console.WriteLine("[DEBUG] Exiting DoBashing method normally.");
                return true;
            }
            catch (Exception ex)
            {
                Log($"[Bashing] [{Client.Name}] Exception in DoBashing: {ex.Message}\n{ex.StackTrace}");
                //Console.WriteLine($"[Bashing] [{Client.Name}] Exception in DoBashing: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Updates the nearby players, monsters, and warp points. Also handles priority sprite listing.
        /// </summary>
        internal void Update()
        {
            //Console.WriteLine("[DEBUG] Entering Update method...");
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
            //Console.WriteLine("[DEBUG] Exiting Update method.");
        }

        /// <summary>
        /// Validates the list of killable targets to ensure it is not null.
        /// </summary>
        /// <returns>True if valid, otherwise false.</returns>
        private bool ValidateKillableTargets()
        {
            if (KillableTargets == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validates the list of potential targets to ensure it is not null and not empty.
        /// </summary>
        /// <param name="potentialTargets">List of potential creature targets.</param>
        /// <returns>True if valid, otherwise false.</returns>
        private bool ValidatePotentialTargets(List<Creature> potentialTargets)
        {
            if (potentialTargets == null)
            {
                return false;
            }
            if (!potentialTargets.Any())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if it's time to send the bashing target, based on interval checks.
        /// </summary>
        /// <returns>True if target sending is needed, otherwise false.</returns>
        private bool ShouldSendBashingTarget()
        {
            bool result = (DateTime.UtcNow - LastSendTarget).TotalMilliseconds > SendTargetIntervalMs;
            return result;
        }

        /// <summary>
        /// Sends the bashing target to the client display, updating the LastSendTarget timestamp.
        /// </summary>
        /// <param name="target">The creature to be displayed as the target.</param>
        private void SendBashingTarget(Creature target)
        {
            Client.DisplayTextOverTarget(2, target.ID, "[Target]");
            LastSendTarget = DateTime.UtcNow;
        }

        private DateTime _lastPathfind = DateTime.MinValue;
        private TimeSpan _minPathInterval = TimeSpan.FromMilliseconds(25);

        /// <summary>
        /// Handles movement logic towards the specified target, including pathfinding and facing the target.
        /// </summary>
        /// <param name="target">The creature towards which to move.</param>
        /// <returns>True if movement logic succeeds, otherwise false.</returns>
        private bool HandleMovement(Creature target)
        {
            if (Client.Bot?._dontWalk == true) return false;

            //Console.WriteLine($"[Bashing] [{Client.Name}] Handling movement to {target.Name} at {target.Location}.");

            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            //Console.WriteLine($"[Bashing] [{Client.Name}] Distance to target: {distanceToTarget}");


            if (NearbyMonsters.Any(m => m.Location == Client.ClientLocation))
            {
                return TryUnstuck();
            }

            // Detect if the target is on top of the player
            if (distanceToTarget == 0)
            {
                bool unstuckResult = TryUnstuck();
                return unstuckResult;
            }

            if (distanceToTarget > 1)
            {
                if (!ShouldPathfindToTarget(target, distanceToTarget))
                {
                    //Console.WriteLine($"[Bashing] [{Client.Name}] Pathfinding skipped.");
                    return true;
                }

                if ((DateTime.UtcNow - _lastPathfind) < _minPathInterval)
                {
                    Console.WriteLine($"[Bashing] [{Client.Name}] Pathfinding skipped due to interval 25ms.");
                    return true;
                }

                _lastPathfind = DateTime.UtcNow;
                if (!TryPathfindToPoint(target.Location))
                {
                    //Console.WriteLine($"[Bashing] [{Client.Name}] Pathfinding failed.");
                    return false;
                }
            }
            else if (!TryFaceTarget(target))
            {
                //Console.WriteLine($"[Bashing] [{Client.Name}] Facing target failed.");
                return true;
            }

            return true;
        }

        /// <summary>
        /// Determines whether pathfinding to the target is necessary based on distance, monster walk timing, etc.
        /// </summary>
        /// <param name="target">The creature we're considering pathfinding to.</param>
        /// <param name="distance">Current distance from the target.</param>
        /// <returns>True if pathfinding should be performed, otherwise false.</returns>
        private bool ShouldPathfindToTarget(Creature target, int distance)
        {
            if (distance <= 3 && target.Direction == Client.ClientLocation.GetDirection(target.Location))
            {
                var lastStepTime = DateTime.UtcNow - target.LastStep;
                bool result = !(lastStepTime > TimeSpan.FromMilliseconds(MonsterWalkIntervalMs - PingCompensation * 2) &&
                                lastStepTime < TimeSpan.FromMilliseconds(MonsterWalkIntervalMs + PingCompensation * 2));
                //Console.WriteLine($"[DEBUG] Exiting ShouldPathfindToTarget with {result} (close range logic).");
                return result;
            }

            bool hasAsleepAllies = Bot.NearbyAllies?.Any(u => u.IsAsleep) ?? false;
            //Console.WriteLine($"[DEBUG] Exiting ShouldPathfindToTarget with {hasAsleepAllies} (based on nearby asleep allies).");
            return !hasAsleepAllies;
        }

        /// <summary>
        /// Retrieves all nearby players that are not on the friend list.
        /// </summary>
        /// <returns>An enumerable of <see cref="Player"/> objects representing strangers.</returns>
        protected IEnumerable<Player> GetStrangers()
        {
            // Create a HashSet with OrdinalIgnoreCase for fast lookups
            var friendSet = new HashSet<string>(Client.FriendBindingList, StringComparer.OrdinalIgnoreCase);

            var strangers = NearbyPlayers.Where(user => !friendSet.Contains(user.Name));
            return strangers;
        }

        /// <summary>
        /// Filters monsters based on cursed/fassed conditions and whether Asgall bashing is enabled.
        /// </summary>
        /// <param name="monsters">Optional list of monsters to filter. Defaults to <see cref="NearbyMonsters"/> if null.</param>
        /// <returns>An enumerable of <see cref="Creature"/> objects that pass the filter.</returns>
        protected virtual IEnumerable<Creature> FilterMonstersByCursedFased(IEnumerable<Creature> monsters = null)
        {
            // Use the provided list or default to NearbyMonsters
            var creatureList = monsters ?? NearbyMonsters;

            var filtered = creatureList.Where(monster =>
                (BashAsgall || !monster.IsAsgalled) &&
                (!Client.ClientTab.chkWaitForCradh.Checked || monster.IsCursed) && // Filter if 'WaitForCradh' is enabled
                (!Client.ClientTab.chkWaitForFas.Checked || monster.IsFassed));    // Filter if 'WaitForFas' is enabled

            return filtered;
        }

        /// <summary>
        /// Checks if the given monster meets all the criteria for being killed (Asgall, cursed, etc.).
        /// </summary>
        /// <param name="monster">The monster to check.</param>
        /// <returns>True if the monster meets the criteria, otherwise false.</returns>
        protected virtual bool MeetsKillCriteria(Creature monster)
        {
            bool isAsgallConditionMet = !monster.IsAsgalled || BashAsgall;
            bool waitForCradhConditionMet = !Client.ClientTab.chkWaitForCradh.Checked || monster.IsCursed;
            bool waitForFasConditionMet = !Client.ClientTab.chkWaitForFas.Checked || monster.IsFassed;

            bool result = isAsgallConditionMet && waitForCradhConditionMet && waitForFasConditionMet;
            return result;
        }

        /// <summary>
        /// Determines if we should use skills on the target, based on timing and monster facing direction.
        /// </summary>
        /// <param name="target">The creature to evaluate.</param>
        /// <returns>True if it's suitable to use skills, otherwise false.</returns>
        internal virtual bool ShouldUseSkillsOnTarget(Creature target)
        {
            TimeSpan timeSinceLastStep = DateTime.UtcNow - target.LastStep;

            if (IsFacingUs(target) || IsFacingAway(target))
            {
                TimeSpan lowerBound = TimeSpan.FromMilliseconds(Math.Max(50, MonsterWalkIntervalMs - PingCompensation * 2));
                TimeSpan upperBound = TimeSpan.FromMilliseconds(MonsterWalkIntervalMs + PingCompensation);

                bool withinFacingBounds = timeSinceLastStep < lowerBound || timeSinceLastStep > upperBound;
                return withinFacingBounds;
            }

            TimeSpan generalLowerBound = TimeSpan.FromMilliseconds(MonsterWalkIntervalMs - PingCompensation * 2);
            TimeSpan generalUpperBound = TimeSpan.FromMilliseconds(MonsterWalkIntervalMs + PingCompensation * 2);

            bool result = timeSinceLastStep < generalLowerBound || timeSinceLastStep > generalUpperBound;
            return result;
        }

        /// <summary>
        /// Determines if crashers can be used, based on user settings, Dion requirements, and current game status.
        /// </summary>
        /// <returns>True if crashers skills can be used, otherwise false.</returns>
        internal bool CanUseCrashers()
        {
            // Check if crashers are enabled
            if (!UseCrashers)
            {
                return false;
            }

            // Check for Dion requirement
            if (RequireDionForCrashers && !Client.Player.IsDioned)
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

            // Dion length check if required
            if (RequireDionForCrashers)
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

        /// <summary>
        /// Abstract method to use skills on a given target. Must be implemented by derived classes.
        /// </summary>
        /// <param name="target">The creature on which to use skills.</param>
        internal abstract void UseSkills(Creature target);

        /// <summary>
        /// Uses the Assail command if bashing Assails is enabled.
        /// </summary>
        internal virtual void UseAssails()
        {
            if (!Client.ClientTab.chkBashAssails.Checked)
            {
                return;
            }

            Client.Assail();
        }

        /// <summary>
        /// Orders potential targets for bashing based on distance, sprite priority, curses/fas, health, and creation time.
        /// Optionally filters to priority sprites only.
        /// </summary>
        /// <param name="monsters">Optional list of monsters to order. Defaults to <see cref="NearbyMonsters"/> if null.</param>
        /// <returns>An ordered enumerable of creatures.</returns>
        protected virtual IEnumerable<Creature> GetOrderedPotentialTargets(IEnumerable<Creature> monsters = null)
        {

            // If assist mode is on, see if we have a target from the lead basher
            if (AssistBasherEnabled)
            {
                var assistList = GetAssistBasherTargets();
                if (assistList != null && assistList.Any())
                {
                    return assistList;
                }
                // else fall through to normal logic
            }


            // Use provided monsters or fallback to NearbyMonsters
            IEnumerable<Creature> creatures = monsters ?? NearbyMonsters;
            IEnumerable<Player> strangers = GetStrangers();

            // Order the creatures based on multiple criteria
            var orderedCreatures = creatures
                .OrderBy(mob => GetPriorityDistance(mob))            // Prioritize by distance and sprite priority
                .ThenBy(mob => GetCursedAdjustedDistance(mob))       // Adjust for cursed & fassed state
                .ThenBy(mob => mob.HealthPercent)                    // Sort by health percentage
                .ThenBy(mob => mob.Creation)                        // Finally, sort by creation time
                .AsEnumerable();

            // Filter creatures if PriorityOnly is enabled
            if (PriorityOnly)
            {
                orderedCreatures = orderedCreatures.Where(mob => PrioritySprites.Contains(mob.SpriteID));
            }

            // Apply additional filtering logic
            var finalCreatures = orderedCreatures.Where(mob => IsValidTarget(mob, strangers));

            // If protection is enabled, reorder so monsters threatening protected player(s) come first.      
            if (EnableProtection)
            {
                finalCreatures = ApplyProtectionOrdering(finalCreatures);
            }

            return finalCreatures;
        }

        /// <summary>
        /// Returns creatures relevant to assisting a lead basher,
        /// or null if no valid assist target is found.
        /// </summary>
        protected virtual List<Creature> GetAssistBasherTargets()
        {
            string leadBasherName = AssistBasherName;
            if (string.IsNullOrEmpty(leadBasherName))
                return null;

            // get the lead basher client
            var leadClient = Client.Server.GetClient(leadBasherName);
            if (leadClient == null)
                return null; // lead not found

            // gather creatures near lead basher
            double maxDistance = AssistBasherStray;
            var leadLocation = leadClient.ClientLocation;

            var nearbyMonsters = Client.GetNearbyValidCreatures(8)
                .Where(mob => !Client.IsLocationSurrounded(mob.Location) &&
                              !Client.Map.IsWall(mob.Location))
                .Where(mob => mob.Location.DistanceFrom(leadLocation) <= maxDistance)
                .ToList();

            // if radioAssitantStray is checked, pick "stray" monster
            if (Client.ClientTab.radioAssitantStray.Checked)
            {
                var chosen = nearbyMonsters
                    .OrderBy(m => m.Location.DistanceFrom(Client.ClientLocation))
                    .FirstOrDefault(m => ShouldEngageTarget(m));

                if (chosen != null)
                    return new List<Creature> { chosen };

                // no stray found => null
                return null;
            }
            else
            {
                // otherwise, just use the lead basher's current target
                var leadTarget = leadClient.Bot?.target;
                if (leadTarget != null)
                    return new List<Creature> { leadTarget };

                return null;
            }
        }

        /// <summary>
        /// Determines if we want to engage a monster based on waiting for curse or fas
        /// </summary>
        protected virtual bool ShouldEngageTarget(Creature mob)
        {
            // Example snippet from your old code:
            if (Client.ClientTab.chkWaitForCradh.Checked && !mob.IsCursed)
                return false;

            if (Client.ClientTab.chkWaitForFas.Checked && !mob.IsFassed)
                return false;

            return true;
        }

        /// <summary>
        /// Reorders the given list of valid creatures so that monsters
        /// threatening the protected player(s) appear first.
        /// </summary>
        /// <param name="currentList">The already-sorted, valid monster list.</param>
        /// <returns>A re-ordered list giving priority to threats near your protected player.</returns>
        protected virtual IEnumerable<Creature> ApplyProtectionOrdering(IEnumerable<Creature> currentList)
        {
            Player whoToProtect = FindWhoToProtect();
            if (whoToProtect == null)
                return currentList; // No valid protect player found, so leave order as-is

            // Weighting: distance to me (the basher) + 2 * distance to the protected player
            // so that creatures near the protected player get top priority
            return currentList.OrderBy(mob =>
            {
                double distToMe = mob.Location.DistanceFrom(Client.ClientLocation);
                double distToProtected = mob.Location.DistanceFrom(whoToProtect.Location) * 2.0;
                return distToMe + distToProtected;
            });
        }

        /// <summary>
        /// Attempts to locate the player(s) to protect (if enabled),
        /// returning the first matching one found.
        /// </summary>
        private Player FindWhoToProtect()
        {
            if (!EnableProtection)
                return null;

            if (NearbyPlayers == null)
                NearbyPlayers = Client.GetNearbyPlayers();

            Player p1 = null;
            if (!string.IsNullOrEmpty(ProtectName1))
            {
                p1 = NearbyPlayers.FirstOrDefault(
                    p => p.Name.Equals(ProtectName1, StringComparison.OrdinalIgnoreCase));
            }
            if (p1 != null)
                return p1;

            Player p2 = null;
            if (!string.IsNullOrEmpty(ProtectName2))
            {
                p2 = NearbyPlayers.FirstOrDefault(
                    p => p.Name.Equals(ProtectName2, StringComparison.OrdinalIgnoreCase));
            }
            return p2;
        }

        /// <summary>
        /// Calculates a distance value factoring sprite priority (non-priority sprites have distance tripled).
        /// </summary>
        /// <param name="mob">The creature to evaluate.</param>
        /// <returns>An integer representing the adjusted distance.</returns>
        private int GetPriorityDistance(Creature mob)
        {
            return PrioritySprites.Contains(mob.SpriteID)
                ? mob.Location.DistanceFrom(Client.ClientLocation)
                : mob.Location.DistanceFrom(Client.ClientLocation) * 3;
        }

        /// <summary>
        /// Slightly adjusts distance if a creature is both cursed and fassed, effectively prioritizing it more.
        /// </summary>
        /// <param name="mob">The creature to evaluate.</param>
        /// <returns>An integer representing the cursed/fassed adjusted distance.</returns>
        private int GetCursedAdjustedDistance(Creature mob)
        {
            return mob.IsCursed && mob.IsFassed
                ? mob.Location.DistanceFrom(Client.ClientLocation) - 1
                : mob.Location.DistanceFrom(Client.ClientLocation);
        }

        /// <summary>
        /// Determines if a creature is a valid target by checking blocking, strangers, user range, and pathfinding distance.
        /// </summary>
        /// <param name="mob">The creature to evaluate.</param>
        /// <param name="strangers">An enumerable of nearby players who are not friends.</param>
        /// <returns>True if valid, otherwise false.</returns>
        private bool IsValidTarget(Creature mob, IEnumerable<Player> strangers)
        {
            // Check if the mob is blocked or invalid
            if (Client.IsLocationSurrounded(mob.Location))
                return false;

            if (Client.IsWalledIn(mob.Location))
                return false;

            // Ensure no strangers or nearby users within range
            bool hasNearbyStrangers = strangers.Any(stranger => stranger.WithinRange(mob, 4));
            bool hasNearbyPlayers = NearbyPlayers.Any(user =>
                user == Client.Player
                    ? Client.ClientLocation.DistanceFrom(mob.Location) == 0
                    : user.WithinRange(mob, 0));

            if (hasNearbyStrangers || hasNearbyPlayers)
                return false;

            // Check pathfinding distance (1 for adjacent, otherwise validate path)
            if (Client.ClientLocation.DistanceFrom(mob.Location) == 1)
                return true;

            int pathCount = Client.Pathfinder.FindPath(Client.ServerLocation, mob.Location).Count;
            return pathCount > 0 && pathCount < 15;
        }

        /// <summary>
        /// Checks if a target creature is facing away from the player.
        /// </summary>
        /// <param name="target">The creature to evaluate.</param>
        /// <returns>True if facing away, otherwise false.</returns>
        protected virtual bool IsFacingAway(Creature target)
        {
            return target.Location.GetDirection(Client.ClientLocation) == target.Direction;
        }

        /// <summary>
        /// Checks if a target creature is facing towards the player.
        /// </summary>
        /// <param name="target">The creature to evaluate.</param>
        /// <returns>True if facing the player, otherwise false.</returns>
        protected virtual bool IsFacingUs(Creature target)
        {
            return Client.ClientLocation.GetDirection(target.Location) == target.Direction;
        }

        /// <summary>
        /// Attempts to turn the client character so it faces the target.
        /// </summary>
        /// <param name="target">The creature we want to face.</param>
        /// <returns>True if the facing logic executes, otherwise false if actions can't be performed.</returns>
        protected virtual bool TryFaceTarget(Creature target)
        {
            if (!CanPerformActions)
            {
                return false;
            }

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

        /// <summary>
        /// Attempts to pathfind to the specified location if movement is allowed and walking is not disabled.
        /// </summary>
        /// <param name="location">The target location to pathfind toward.</param>
        /// <param name="distance">Acceptable distance to stop short of the location.</param>
        /// <returns>True if pathfinding succeeds, otherwise false.</returns>
        protected virtual bool TryPathfindToPoint(Location location, short distance = 1)
        {
            //Console.WriteLine($"[Bashing] [{Client.Name}] Pathfinding to {location}.");

            if (!CanMove || Client.Bot._dontWalk)
            {
                //Console.WriteLine($"[Bashing] [{Client.Name}] Cannot move or walking is disabled.");
                return false;
            }

            bool success = Client.Pathfind(location, distance);
            //Console.WriteLine($"[Bashing] [{Client.Name}] Pathfinding {(success ? "succeeded" : "failed")}.");
            return success;
        }

        /// <summary>
        /// Retrieves creatures within a given skill range of the player, defaulting to <see cref="KillableTargets"/> if available, otherwise <see cref="NearbyMonsters"/>.
        /// </summary>
        /// <param name="creatures">Optional list of creatures to check.</param>
        /// <param name="skillRange">The range within which to include creatures.</param>
        /// <returns>An enumerable of creatures within the specified range.</returns>
        protected IEnumerable<Creature> GetSurroundingCreatures(IEnumerable<Creature> creatures = null, int skillRange = 1)
        {
            // Debug statements here if you want more detail:
            // Console.WriteLine("[DEBUG] Entering GetSurroundingCreatures method...");
            creatures ??= KillableTargets ?? NearbyMonsters;
            return creatures.Where(mob => mob.Location.DistanceFrom(Client.ClientLocation) <= skillRange);
        }

        /// <summary>
        /// Checks if two locations share the same axis (i.e., X or Y coordinate).
        /// </summary>
        /// <param name="loc1">First location.</param>
        /// <param name="loc2">Second location.</param>
        /// <returns>True if they share an axis, otherwise false.</returns>
        internal bool SharesAxis(Location loc1, Location loc2)
        {
            // No debug statements added to keep it concise, but can be added if needed.
            return loc1.MapID == loc2.MapID &&
                   (loc1.X == loc2.X || loc1.Y == loc2.Y);
        }

        /// <summary>
        /// Equips a Light or Dark necklace in Shinewood Forest if the target's sprite indicates a mismatch and enough time has passed since the last swap.
        /// </summary>
        /// <param name="target">The creature we're fighting, used to determine which necklace to equip.</param>
        /// <returns>True if a necklace swap occurred, otherwise false.</returns>
        internal virtual bool EquipNecklaceIfNeeded(Creature target)
        {
            if (!Client.Map.Name.Contains("Shinewood Forest"))
            {
                return false;
            }

            bool canSwapNecklace = DateTime.UtcNow.Subtract(LastNeckSwap).TotalMilliseconds > (1000 + PingCompensation * 2);

            if (canSwapNecklace)
            {
                if (CONSTANTS.SHINEWOOD_HOLY.Contains(target.SpriteID) && !Client.OffenseElement.Equals("Light", StringComparison.OrdinalIgnoreCase))
                {
                    Client.Bot.SwapNecklace("Light");
                    LastNeckSwap = DateTime.UtcNow;
                    return true;
                }

                if (CONSTANTS.SHINEWOOD_DARK.Contains(target.SpriteID) && !Client.OffenseElement.Equals("Dark", StringComparison.OrdinalIgnoreCase))
                {
                    Client.Bot.SwapNecklace("Dark");
                    LastNeckSwap = DateTime.UtcNow;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to perform a sleep skill (e.g., Lullaby Punch) then uses another skill or item if successful.
        /// </summary>
        /// <param name="name">The name of the skill or item to use after the sleep skill.</param>
        /// <param name="isItem">True if it's an item instead of a skill.</param>
        /// <returns>True if both the sleep skill and the subsequent skill/item were used, otherwise false.</returns>
        internal bool TryComboWithSleepSkill(string name, bool isItem = false)
        {
            //Console.WriteLine("[DEBUG] Entering TryComboWithSleepSkill method...");
            if (!UseSleepSkill())
            {
                //Console.WriteLine("[DEBUG] Exiting TryComboWithSleepSkill with false (failed sleep skill).");
                return false;
            }

            bool result = isItem ? TryUseItem(name) : TryUseSkill(name);
            //Console.WriteLine($"[DEBUG] Exiting TryComboWithSleepSkill with {result}.");
            return result;
        }

        /// <summary>
        /// Attempts to use either "Lullaby Punch" or "Wolf Fang Fist" or "Frozen Strike".
        /// </summary>
        /// <returns>True if one of the sleep skills was used, otherwise false.</returns>
        private bool UseSleepSkill()
        {
            //Console.WriteLine("[DEBUG] Entering UseSleepSkill method...");
            bool skillUsed = Client.UseSkill("Lullaby Punch") || Client.UseSkill("Wolf Fang Fist") || Client.UseSkill("Frozen Strike");
            //Console.WriteLine($"[DEBUG] Exiting UseSleepSkill with {skillUsed}.");
            return skillUsed;
        }

        /// <summary>
        /// Attempts to use the named skill if it is in the skillbook and can be used.
        /// </summary>
        /// <param name="skillName">The name of the skill to use.</param>
        /// <returns>True if used successfully, otherwise false.</returns>
        private bool TryUseSkill(string skillName)
        {
            //Console.WriteLine("[DEBUG] Entering TryUseSkill method...");
            Skill skill = Client.Skillbook[skillName] ?? Client.Skillbook.GetNumberedSkill(skillName);
            if (skill == null || !skill.CanUse)
            {
                //Console.WriteLine("[DEBUG] Exiting TryUseSkill with false (skill null or not usable).");
                return false;
            }

            Client.UseSkill(skill.Name);
            //Console.WriteLine("[DEBUG] Exiting TryUseSkill with true.");
            return true;
        }

        /// <summary>
        /// Attempts to use an item from the inventory by name.
        /// </summary>
        /// <param name="itemName">The name of the item to use.</param>
        /// <returns>True if used successfully, otherwise false.</returns>
        private bool TryUseItem(string itemName)
        {
            Item item = Client.Inventory[itemName];
            if (item == null)
            {
                return false;
            }

            Client.UseItem(item.Name);
            return true;
        }

        /// <summary>
        /// Attempts to move the player from the current location if stuck, by checking nearby walkable tiles in random directions.
        /// </summary>
        /// <returns>True if the unstuck logic succeeded, otherwise false.</returns>
        protected virtual bool TryUnstuck()
        {
            //Console.WriteLine("[DEBUG] Entering TryUnstuck method...");
            if (!CanMove)
            {
                //Console.WriteLine("[DEBUG] Exiting TryUnstuck with false (cannot move).");
                return false;
            }

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
                    //Console.WriteLine("[DEBUG] Exiting TryUnstuck with true (walked).");
                    return true;
                }
            }

            //Console.WriteLine("[DEBUG] Exiting TryUnstuck with false (no walkable tile found).");
            return false;
        }

        /// <summary>
        /// Attempts to refresh the client state if certain conditions (inactivity, position desync) are met.
        /// </summary>
        /// <returns>True if a refresh request was made, otherwise false.</returns>
        internal virtual bool RefreshIfNeeded()
        {
            //Console.WriteLine("[DEBUG] Entering RefreshIfNeeded method...");
            DateTime currentTime = DateTime.UtcNow;
            TimeSpan pingDelay = TimeSpan.FromMilliseconds(PingCompensation * 2);

            // Refresh if no activity in the last 5 seconds
            if (IsRefreshRequired(currentTime))
            {
                Client.RefreshRequest();
                LastPointInSync = currentTime;
                //Console.WriteLine("[DEBUG] Exiting RefreshIfNeeded with true (activity-based refresh).");
                return true;
            }

            // Refresh if Client and Server points are out of sync
            if (Client.ClientLocation != Client.ServerLocation)
            {

                if ((currentTime - LastPointInSync).TotalMilliseconds > 1000)
                {
                    //Console.WriteLine($"[DEBUG] [{Client.Name}] RefreshIfNeeded: Desync detected. Waiting for sync...");

                    WaitForSync(pingDelay, currentTime);

                    if (Client.ClientLocation != Client.ServerLocation)
                    {
                        //Console.WriteLine($"[DEBUG] [{Client.Name}] Desync persists. Issuing RefreshRequest.");
                        Client.RefreshRequest();

                    }

                    LastPointInSync = DateTime.UtcNow;
                    //Console.WriteLine("[DEBUG] Exiting RefreshIfNeeded with true (desync refresh).");
                    return true;
                }
            }
            else
            {
                LastPointInSync = currentTime;
            }

            //Console.WriteLine("[DEBUG] Exiting RefreshIfNeeded with false (no refresh needed).");
            return false;
        }

        /// <summary>
        /// Determines whether a refresh request is required based on inactivity times.
        /// </summary>
        /// <param name="currentTime">The current UTC time.</param>
        /// <returns>True if a refresh is required, otherwise false.</returns>
        private bool IsRefreshRequired(DateTime currentTime)
        {
            bool result = (currentTime - Client.LastStep).TotalSeconds > 5.0 &&
                          (currentTime - Client.Bot._lastEXP).TotalSeconds > 5.0 &&
                          (currentTime - Client.Bot._lastRefresh).TotalSeconds > 5.0;
            return result;
        }

        /// <summary>
        /// Pauses briefly to allow server/client position to sync up; if still out of sync, triggers a refresh.
        /// </summary>
        /// <param name="pingDelay">The timespan to wait before concluding desync remains.</param>
        /// <param name="startTime">The time we started the sync wait.</param>
        private void WaitForSync(TimeSpan pingDelay, DateTime startTime)
        {
            Console.WriteLine($"[DEBUG] [{Client.Name}] Waiting for sync for up to {pingDelay.TotalMilliseconds} ms...");

            while ((DateTime.UtcNow - startTime) < pingDelay)
            {
                Thread.Sleep(25);
                if (Client.ClientLocation == Client.ServerLocation)
                {
                    Console.WriteLine($"[DEBUG] [{Client.Name}] Sync achieved.");
                    break;
                }

            }

            Console.WriteLine($"[DEBUG] [{Client.Name}] Exiting WaitForSync.");
        }

        /// <summary>
        /// Logs the specified message to a file and prints it to the console.
        /// </summary>
        /// <param name="message">The message to log.</param>
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
