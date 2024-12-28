using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Base;
using Talos.Enumerations;
using Talos.Maps;
using Talos.Networking;
using Talos.Objects;
using Talos.Structs;

namespace Talos.Bashing
{
    internal class RogueBashing : BashingBase
    {
        // Track arrow cooldowns
        private DateTime LastDualCrystalArrow1 = DateTime.MinValue;
        private DateTime LastDualCrystalArrow = DateTime.MinValue;
        private DateTime LastCrystalArrow1 = DateTime.MinValue;
        private DateTime LastCrystalArrow = DateTime.MinValue;
        private DateTime LastSpecialArrowAttack = DateTime.MinValue;

#nullable disable
        private readonly HashSet<int> _backedOff = new HashSet<int>();

        private readonly string[] Arrows = new[]
        {
        "dual crystal arrows",
        "dual crystal arrows1",
        "crystal arrow1",
        "crystal arrow"
    };

        private int EngageRange => Convert.ToInt32(Client.ClientTab.engageRangeNum.Value);
        private int AttackRange => Convert.ToInt32(Client.ClientTab.atkRangeNum.Value);

        public RogueBashing(Bot bot) : base(bot)
        {
            // Show the rogue settings in UI
            Client.ClientTab.rogueGbox.Visible = true;
        }


        internal override bool DoBashing()
        {
            if (!CanPerformActions)
                return true;

            Update();

            // 1) Filter killable targets
            KillableTargets = FilterMonstersByCursedFased().ToList();

            // 2) Determine the best position & target to approach
            var positionAndTarget = GetOptimalPositionAndTarget(KillableTargets);
            if (!positionAndTarget.HasValue)
            {
                Target = null;
                return false;
            }

            (Creature target, Location attackPosition) = positionAndTarget.Value;
            Target = target;

            // 3) Send target info periodically
            if ((DateTime.UtcNow - LastSendTarget).TotalMilliseconds > SendTargetIntervalMs)
            {
                Client.DisplayTextOverTarget(2, target.ID, "[Target]");
                LastSendTarget = DateTime.UtcNow;
            }

            // 4) Check if user is stuck on a monster
            if (NearbyMonsters.Any(nm => nm.Location.DistanceFrom(Client._clientLocation) == 0))
                return TryUnstuck();

            // 5) Movement / positioning logic
            int distanceToTarget = Client._clientLocation.DistanceFrom(target.Location);

            // If distance>1 and not aligned, we pathfind or do micro-movement
            if (distanceToTarget > 1)
            {
                // If not on the same axis and outside of AttackRange => pathfind
                if (!SharesAxis(Client._clientLocation, target.Location))
                {
                    // Avoid if group members are pramhed or pathfinding fails
                    if (!Bot._nearbyAllies.Any(u => u.IsAsleep) && !TryPathfindToPoint(attackPosition, 0))
                        return false;
                }
                else if (distanceToTarget > AttackRange)
                {
                    // If the user is aligned but still out of AttackRange => step closer if possible
                    var dir = target.Location.GetDirection(Client._clientLocation);
                    Location nextStep = Client._clientLocation.Offset(dir);

                    if (Client._map.IsWalkable(Client, nextStep) &&
                        !Bot._nearbyAllies.Any(u => u.IsAsleep))
                    {
                        TryPathfindToPoint(nextStep, 0);
                    }
                }
            }

            // 6) Try facing or refreshing
            bool sameAxis = SharesAxis(Client._clientLocation, target.Location);
            if ((sameAxis && !TryFaceTarget(target)) || RefreshIfNeeded() || !sameAxis)
                return true;

            // 7) Try equipping necklace if needed
            if (EquipNecklaceIfNeeded(target) || Client.Bot._dontBash)
                return true;

            // 8) Finally, use skills on the target
            UseSkills(target);
            return true;
        }

        /// <summary>
        /// Finds the best position & creature to engage based on EngageRange & AttackRange.
        /// </summary>
        internal (Creature Target, Location AttackPosition)? GetOptimalPositionAndTarget(
            IEnumerable<Creature> potentialTargets)
        {
            var allPositions = GetAllRangedAttackPositions(NearbyMonsters, EngageRange)
                // Sort logic: first by distance priority, then by health, then creation time
                .OrderBy(info => PrioritySprites.Contains(info.Target.SpriteID)
                    ? Client._clientLocation.DistanceFrom(info.AttackPosition)
                    : Client._clientLocation.DistanceFrom(info.AttackPosition) * 3)
                .ThenBy(info =>
                {
                    // Additional sorting logic as in the original
                    int dist = Client._clientLocation.DistanceFrom(info.Target.Location);
                    bool closeRange = (dist < 3 || dist <= 8);
                    return closeRange ? 0 : 1; // 0 means top priority if close
                })
                .ThenBy(info => info.Target.HealthPercent)
                .ThenBy(info => info.Target.Creation)
                .ToList();

            if (!allPositions.Any())
                return null;

            // Filter if priority only or meets kill criteria
            var validPositions = allPositions
                .Where(info => (!PriorityOnly || PrioritySprites.Contains(info.Target.SpriteID)) &&
                               MeetsKillCriteria(info.Target))
                .ToList();
            if (!validPositions.Any())
                validPositions = allPositions;

            // Attempt an immediate approach if user is already in range
            var immediate = validPositions.FirstOrDefault(info =>
                Client._clientLocation.DistanceFrom(info.Target.Location) == 1 ||
                Client._clientLocation.DistanceFrom(info.AttackPosition) == 0);

            if (immediate.Target != null)
                return immediate;

            // Fallback: pathfinding approach
            var fallback = validPositions.FirstOrDefault(info =>
            {
                int dist = Client._clientLocation.DistanceFrom(info.AttackPosition);
                if (dist <= 1) return true;

                var path = Client.Pathfinder.FindPath(Client._serverLocation, info.AttackPosition);
                return path.Count > 0 && path.Count < 15;
            });

            if (fallback.Target == null)
                return null;

            return fallback;
        }

        /// <summary>
        /// Gathers all possible ranged attack positions for each potential target.
        /// </summary>
        internal IEnumerable<(Creature Target, Location AttackPosition)> GetAllRangedAttackPositions(
            IEnumerable<Creature> potentialTargets, int maxDistance = 8)
        {
            return potentialTargets.SelectMany(target => GetAllRangedAttackPositions(target, maxDistance));
        }

        /// <summary>
        /// Returns all walkable, non-warp positions along North/East/South/West lines within maxDistance.
        /// </summary>
        internal IEnumerable<(Creature Target, Location AttackPosition)> GetAllRangedAttackPositions(
            Creature target, int maxDistance = 8)
        {
            var top = new Location(Client._map.MapID, target.Location.X, (short)(target.Location.Y - maxDistance));
            var east = new Location(Client._map.MapID, (short)(target.Location.X + maxDistance), target.Location.Y);
            var south = new Location(Client._map.MapID, target.Location.X, (short)(target.Location.Y + maxDistance));
            var west = new Location(Client._map.MapID, (short)(target.Location.X - maxDistance), target.Location.Y);

            // Each direction, iterating closer to the target
            foreach (var p in WalkLine(top, target.Location, Direction.South))
                yield return (target, p);

            foreach (var p in WalkLine(east, target.Location, Direction.West))
                yield return (target, p);

            foreach (var p in WalkLine(south, target.Location, Direction.North))
                yield return (target, p);

            foreach (var p in WalkLine(west, target.Location, Direction.East))
                yield return (target, p);

            IEnumerable<Location> WalkLine(Location start, Location end, Direction dir)
            {
                var current = start;
                while (current != end)
                {
                    if (!Warps.Contains(current) && Client._map.IsWalkable(Client, current))
                        yield return current;
                    current = current.Offset(dir);
                }
            }
        }

        /// <summary>
        /// Tries to unsnare or unstick the user if they're stuck on a monster.
        /// </summary>
        private bool TryUnstuck()
        {
            // original logic you had to unstick / refresh
            return base.TryUnstuck();
        }

        /// <summary>
        /// Helpers to track arrow usage cooldown.
        /// </summary>
        private ref DateTime GetArrowLastUseRef(string name)
        {
            name = name.ToLower();

            if (name == "crystal arrow")
                return ref LastCrystalArrow;
            else if (name == "crystal arrow1")
                return ref LastCrystalArrow1;
            else if (name == "dual crystal arrows")
                return ref LastDualCrystalArrow;
            else if (name == "dual crystal arrows1")
                return ref LastDualCrystalArrow1;
            else
                throw new InvalidOperationException($"{name} doesn't exist");
        }


        private bool AnyArrowOffCooldown()
        {
            return Arrows.Any(ArrowOffCooldown);
        }

        private bool ArrowOffCooldown(string arrowName)
        {
            return (DateTime.UtcNow - GetArrowLastUseRef(arrowName)).TotalSeconds > 10.0;
        }

        /// <summary>
        /// Special Arrow Attack if any arrow is off cooldown.
        /// </summary>
        private bool CanUseSpecialArrowAttack()
        {
            return (DateTime.UtcNow - LastSpecialArrowAttack).TotalSeconds > 1.0 && AnyArrowOffCooldown();
        }

        private bool UseSpecialArrowAttack(string arrowName)
        {
            ref DateTime cooldownRef = ref GetArrowLastUseRef(arrowName);
            if ((DateTime.UtcNow - cooldownRef).TotalSeconds <= 10.0)
                return false;

            Item arrowItem = Client.Inventory[arrowName];
            if (arrowItem == null)
                return false;

            if (arrowItem.Slot != 0)
                SwapItemToSlot1(arrowItem);

            Client.UseSkill("Special Arrow Attack");

            // Swap arrow item back to original slot if needed
            if (arrowItem.Slot != 0)
                SwapItemToSlot1(arrowItem);

            cooldownRef = DateTime.UtcNow;
            return true;
        }

        internal bool TryUseSpecialArrowAttacks()
        {
            foreach (var arrow in Arrows)
            {
                if (UseSpecialArrowAttack(arrow))
                {
                    LastSpecialArrowAttack = DateTime.UtcNow;
                    return true;
                }
            }
            return false;
        }

        private void SwapItemToSlot1(Item item)
        {
            var pkt = new ClientPacket(48); // 48 = OpCode for slot manipulation
            pkt.WriteByte(0);
            pkt.WriteByte(item.Slot);
            pkt.WriteByte(1);
            Client.Enqueue(pkt);
        }

        private bool TryBackOff(Creature target, bool once = false)
        {
            // If 'once' is true, we attempt to back off only once per target
            if (once && !_backedOff.Add(target.ID))
                return false;

            Direction dir = Client._clientLocation.GetDirection(target.Location);
            return MicroAdjust(dir);
        }

        private bool MicroAdjust(Direction dir)
        {
            Location nextPos = Client._clientLocation.Offset(dir);
            if (!Client._map.IsWalkable(Client, nextPos) || Warps.Contains(nextPos))
                return false;

            Client.Walk(dir);
            return true;
        }

        private bool TryApproach(Creature target)
        {
            return Client._clientLocation.DistanceFrom(target.Location) <= 1 ||
                   MicroAdjust(target.Location.GetDirection(Client._clientLocation));
        }

        private bool TryAmbushTech(Creature target)
        {
            if (!CanUseSpecialArrowAttack())
                return false;

            // Check alignment
            Direction targetDirection = target.Direction;
            Direction userToTargetDir = Client._clientLocation.GetDirection(target.Location);

            if (targetDirection != userToTargetDir)
                return false;

            // Position behind the target
            Location behindTarget = target.Location.Offset(userToTargetDir);
            if (!Client._map.IsWalkable(Client, behindTarget) ||
                Warps.Contains(behindTarget) ||
                !(Client.UseSkill("Shadow Figure") ||
                  Client.UseSkill("Shadow Strike") ||
                  Client.UseSkill("Ambush")))
            {
                return false;
            }

            // Manually set these after a skill jump
            Client._clientLocation = behindTarget;
            Client._clientDirection = userToTargetDir;

            // Ranged attacks after approach
            UseRangedAssails();
            DoActionForSurroundingCreature();
            return true;
        }

        private void TurnForAction(Creature target, Action action)
        {
            Direction originalDir = Client._clientDirection;
            Direction newDir = target.Location.GetDirection(Client._clientLocation);

            if (originalDir != newDir)
                Client.Turn(newDir);

            action();

            if (originalDir != newDir)
                Client.Turn(originalDir);
        }

        /// <summary>
        /// Default ranged assails: "Arrow Shot", "Throw Surigum", etc.
        /// </summary>
        internal virtual void UseRangedAssails()
        {
            Client.NumberedSkill("Arrow Shot");
            Client.UseSkill("Throw Surigum");
        }

        /// <summary>
        /// Attempt "Amnesia" + skill combo.
        /// </summary>
        internal bool TryComboWithAmnesia(string skillName)
        {
            if (!Client.CanUseSkill("Amnesia"))
                return false;

            Skill skill = Client.Skillbook[skillName] ?? Client.Skillbook.GetNumberedSkill(skillName);
            if (!Client.CanUseSkill(skill))
                return false;

            Client.UseSkill("Amnesia");
            Client.UseSkill(skill.Name);
            return true;
        }

        internal bool TryAmnesiaTech(Creature target, string skillName)
        {
            if (!Client.CanUseSkill("Amnesia"))
                return false;

            Skill skill = Client.Skillbook[skillName] ?? Client.Skillbook.GetNumberedSkill(skillName);
            if (!Client.CanUseSkill(skill))
                return false;

            // Attempt to back off
            if (!TryBackOff(target))
                return false;

            // Then turn, use "Amnesia," walk forward, and skill
            Direction dir = target.Location.GetDirection(Client._clientLocation);
            Client.Turn(dir);
            Client.UseSkill("Amnesia");
            Client.Walk(dir);
            Client.UseSkill(skill.Name);
            return true;
        }

        /// <summary>
        /// Overridden logic to handle a variety of range-based attacks for Rogues.
        /// </summary>
        internal override void UseSkills(Creature target)
        {
            DateTime now = DateTime.UtcNow;
            bool canUseSkill = (now - LastUsedSkill) > TimeSpan.FromMilliseconds(SkillIntervalMs);
            bool canAssail = (now - LastAssailed) > TimeSpan.FromMilliseconds(500);

            // Attempt multi-target logic first
            if (DoActionForSurroundingCreature())
            {
                LastUsedSkill = now;
                return;
            }

            // Possibly back off if distance=2
            int distanceToTarget = Client._clientLocation.DistanceFrom(target.Location);
            if (distanceToTarget == 2)
            {
                if (TryBackOff(target, once: true) || TryApproach(target))
                    return;
            }

            // Try again if that opened AOE or anything
            if (DoActionForSurroundingCreature())
            {
                LastUsedSkill = now;
                return;
            }

            // Re-check distance in case we moved
            distanceToTarget = Client._clientLocation.DistanceFrom(target.Location);
            if (distanceToTarget > 11)
                return;

            // Distances
            if (distanceToTarget == 1)
            {
                // If asgalled but not bashing => skip
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                    return;

                // Use skills if possible
                if (canUseSkill && DoActionForRange1(target))
                {
                    LastUsedSkill = now;
                    target._hitCounter++;
                }

                // Possibly do assails if we can
                if (canAssail && (BashAsgall || !target.IsAsgalled))
                {
                    UseAssails();
                    LastAssailed = now;
                    target._hitCounter++;
                }
            }
            else if (distanceToTarget == 2)
            {
                // If we can do ranged assails
                if (!canAssail)
                    return;

                UseRangedAssails();
                LastAssailed = now;
                target._hitCounter++;
            }
            else
            {
                // 3..11
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                    return;

                // If we can do skill
                if (canUseSkill && DoActionForRangeLt11(target))
                {
                    LastUsedSkill = now;
                    target._hitCounter++;
                }

                // Then do assails if time allows
                if (!canAssail)
                    return;

                UseRangedAssails();
                LastAssailed = now;
                target._hitCounter++;
            }
        }

        private bool DoActionForRange1(Creature target)
        {
            if (!MeetsKillCriteria(target) || !ShouldUseSkillsOnTarget(target))
                return false;

            byte hp = target.HealthPercent;

            if (hp >= 60)
            {
                if (target._hitCounter == 0 && (Client.UseSkill("Assassin Strike") || Client.NumberedSkill("Rear Strike")) ||
                    TryComboWithSleepSkill("Stab Twice", true) ||
                    TryComboWithSleepSkill("Kidney Shot", true) ||
                    TryAmnesiaTech(target, "Assassin Strike") ||
                    TryAmnesiaTech(target, "Rear Strike"))
                {
                    return true;
                }
            }

            if (hp >= 40)
            {
                if (Client.UseSkill("Kidney Shot") || Client.UseSkill("Stab Twice"))
                    return true;
            }

            if (hp >= 20)
            {
                if (TryAmbushTech(target))
                    return true;
            }

            if (hp <= 20)
            {
                Client.UseSkill("Stab and Twist");
                return true;
            }

            return false;
        }

        private bool DoActionForRangeLt11(Creature target)
        {
            if (!MeetsKillCriteria(target))
                return false;

            byte hp = target.HealthPercent;

            // 1) If hp>=60 & hits=0 => "Rear Strike" or "Amnesia + Rear Strike"
            if (hp >= 60 &&
               ((target._hitCounter == 0 && Client.NumberedSkill("Rear Strike")) ||
                (Client._clientLocation.DistanceFrom(target.Location) == 3 && TryComboWithAmnesia("Rear Strike"))))
            {
                return true;
            }

            // 2) If hp>=20 => check if we can do special arrow attacks if we are behind or out of direct range
            bool behindOrFar = IsFacingAway(target) || (target.Location.DistanceFrom(Client._clientLocation) >= 3);
            if (hp >= 20 && behindOrFar)
            {
                return TryUseSpecialArrowAttacks();
            }

            return false;
        }

        private bool DoActionForSurroundingCreature()
        {
            Skill rearStrike = Client.Skillbook.GetNumberedSkill("Rear Strike");
            bool canRearStrike = Client.CanUseSkill(rearStrike);

            // 1) Possibly do "Rear Strike" on a different target
            if (canRearStrike)
            {
                foreach (Creature mob in KillableTargets)
                {
                    if (mob != Target &&
                        mob.Location.DistanceFrom(Client._clientLocation) > 1 &&
                        MeetsKillCriteria(mob) &&
                        ShouldUseSkillsOnTarget(mob) &&
                        SharesAxis(Client._clientLocation, mob.Location) &&
                        (mob._hitCounter <= 0 || Client.CanUseSkill("Amnesia")))
                    {
                        TurnForAction(mob, () =>
                        {
                            if (mob._hitCounter > 0)
                                Client.UseSkill("Amnesia");
                            Client.UseSkill(rearStrike.Name);
                        });
                        return true;
                    }
                }
            }

            // 2) If "Assassin Strike" is available for a close target
            if (Client.CanUseSkill("Assassin Strike"))
            {
                foreach (Creature mob in KillableTargets)
                {
                    if (mob != Target &&
                        mob.Location.DistanceFrom(Client._clientLocation) == 1 &&
                        mob._hitCounter <= 0 &&
                        MeetsKillCriteria(mob) &&
                        ShouldUseSkillsOnTarget(mob))
                    {
                        TurnForAction(mob, () => Client.UseSkill("Assassin Strike"));
                        return true;
                    }
                }
            }

            return false;
        }
    }

}
