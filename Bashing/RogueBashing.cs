using System;
using System.Collections.Generic;
using System.Linq;
using Talos.Base;
using Talos.Definitions;
using Talos.Networking;
using Talos.Objects;
using Talos.Structs;

namespace Talos.Bashing
{
    internal class RogueBashing : BashingBase
    {

        // Standard skills
        private Skill ShadowStrike => Client.Skillbook["Shadow Strike"];
        private Skill KidneyShot => Client.Skillbook["Kidney Shot"];
        private Skill SpecialArrowAttack => Client.Skillbook["Special Arrow Attack"];
        private Skill Amnesia => Client.Skillbook["Amnesia"];
        private Skill ThrowSurigum => Client.Skillbook["Throw Surigum"];
        private Skill StabAndTwist => Client.Skillbook["Stab and Twist"];
        private Skill AssassinStrike => Client.Skillbook["Assassin Strike"];
        private Skill ShadowFigure => Client.Skillbook["Shadow Figure"];
        private Skill StabTwice => Client.Skillbook["Stab Twice"];

        // Monk->Rogue Skills
        private Skill Ambush => Client.Skillbook["Ambush"];
        private Skill MantisKick => Client.Skillbook["Mantis Kick"];
        private Skill HighKick => Client.Skillbook["High Kick"];
        private Skill Kick => Client.Skillbook["Kick"];


        // Numbered skills: try direct lookup first, then fall back to the numbered lookup.
        private Skill ArrowShot => Client.Skillbook["Arrow Shot"] ?? Client.Skillbook.GetNumberedSkill("Arrow Shot");
        private Skill RearStrike => Client.Skillbook["Rear Strike"] ?? Client.Skillbook.GetNumberedSkill("Rear Strike");


        // Warrior-Rogue skills
        private Skill Charge => Client.Skillbook["Charge"];
        private Skill WindBlade => Client.Skillbook["Wind Blade"];
        private Skill Crasher => Client.Skillbook["Crasher"];


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
            // Adam check this
            // check if invoke required, if so, show rogueGbox on the main form
            if (Client.ClientTab.InvokeRequired)
            {
                Client.ClientTab.Invoke(new Action(() => Client.ClientTab.rogueGbox.Visible = true));
            }
            else
                Client.ClientTab.rogueGbox.Visible = true;
        }

        /// <summary>
        /// Tries to use the best possible skill(s) on the given target.
        /// </summary>
        /// <param name="target">The creature we're fighting.</param>
        internal override void UseSkills(Creature target)
        {
            DateTime now = DateTime.UtcNow;
            Console.WriteLine($"[DEBUG] UseSkills called for target {target.Name} (ID: {target.ID}) at {target.Location}");

            bool canUseSkill = (now - LastUsedSkill) > TimeSpan.FromMilliseconds(SkillIntervalMs);
            bool canAssail = (now - LastAssailed) > TimeSpan.FromMilliseconds(1000.0);
            Console.WriteLine($"[DEBUG] canUseSkill: {canUseSkill}, canAssail: {canAssail}");

            // Attempt multi-target logic first
            if (DoActionForSurroundingCreature())
            {
                LastUsedSkill = now;
                Console.WriteLine("[DEBUG] Early exit in UseSkills due to secondary action.");
                return;
            }

            // Possibly back off if distance=2
            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            Console.WriteLine($"[DEBUG] Distance to primary target: {distanceToTarget}");
            if (distanceToTarget == 2)
            {
                if (TryBackOff(target, once: true) || TryApproach(target))
                    return;
            }

            // Try again if that opened AOE or anything
            if (DoActionForSurroundingCreature())
            {
                LastUsedSkill = now;
                Console.WriteLine("[DEBUG] Second Early exit in UseSkills due to secondary action.");
                return;
            }

            // Re-check distance in case we moved
            distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            if (distanceToTarget > 11)
            {
                Console.WriteLine("[DEBUG] Target is too far; exiting UseSkills.");
                return;

            }

            // Distances
            if (distanceToTarget == 1)
            {
                Console.WriteLine("[DEBUG] Processing melee-range actions for primary target.");
                // If asgalled but not bashing => skip
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                {
                    Console.WriteLine("[DEBUG] Primary target is asgalled and conditions not met; skipping melee action.");
                    return;
                }


                // Use skills if possible
                if (canUseSkill && DoActionForRange1(target))
                {
                    LastUsedSkill = now;
                    target._hitCounter++;
                    Console.WriteLine("[DEBUG] DoActionForRange1 executed on primary target.");
                }

                // Possibly do assails if we can
                if (canAssail && (BashAsgall || !target.IsAsgalled))
                {
                    UseAssails();
                    LastAssailed = now;
                    target._hitCounter++;
                    Console.WriteLine("[DEBUG] UseAssails executed on primary target.");
                }
            }
            else if (distanceToTarget == 2)
            {
                // If we can do ranged assails
                if (!canAssail) { Console.WriteLine("[DEBUG] Cannot perform assails at distance 2; exiting UseSkills."); return; }


                UseRangedAssails();
                LastAssailed = now;
                target._hitCounter++;
                Console.WriteLine("[DEBUG] Ranged assails executed on primary target.");
            }
            else
            {
                Console.WriteLine("[DEBUG] Processing actions for primary target at distance 3..11.");
                // 3..11
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall) { Console.WriteLine("[DEBUG] Primary target is asgalled and conditions not met; exiting."); return; }


                // If we can do skill
                if (canUseSkill && DoActionForRangeLt11(target))
                {
                    LastUsedSkill = now;
                    target._hitCounter++;
                    Console.WriteLine("[DEBUG] DoActionForRangeLt11 executed on primary target.");
                }

                // Then do assails if time allows
                if (!canAssail)
                    return;

                UseRangedAssails();
                LastAssailed = now;
                target._hitCounter++;
                Console.WriteLine("[DEBUG] Ranged assails executed on primary target.");
            }
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
            if (NearbyMonsters.Any(nm => nm.Location.DistanceFrom(Client.ClientLocation) == 0))
                return TryUnstuck();

            // 5) Movement / positioning logic
            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);

            // If distance>1 and not aligned, we pathfind or do micro-movement
            if (distanceToTarget > 1)
            {
                // If not on the same axis and outside of AttackRange => pathfind
                if (!SharesAxis(Client.ClientLocation, target.Location))
                {
                    // Avoid if group members are pramhed or pathfinding fails
                    if (!Bot.NearbyAllies.Any(u => u.IsAsleep) && !TryPathfindToPoint(attackPosition, 0))
                        return false;
                }
                else if (distanceToTarget > AttackRange)
                {
                    // If the user is aligned but still out of AttackRange => step closer if possible
                    var dir = target.Location.GetDirection(Client.ClientLocation);
                    Location nextStep = Client.ClientLocation.Offsetter(dir);

                    if (Client.Map.IsWalkable(Client, nextStep) &&
                        !Bot.NearbyAllies.Any(u => u.IsAsleep))
                    {
                        TryPathfindToPoint(nextStep, 0);
                    }
                }
            }

            // 6) Try facing or refreshing
            bool sameAxis = SharesAxis(Client.ClientLocation, target.Location);
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
                    ? Client.ClientLocation.DistanceFrom(info.AttackPosition)
                    : Client.ClientLocation.DistanceFrom(info.AttackPosition) * 3)
                .ThenBy(info =>
                {
                    // Additional sorting logic as in the original
                    int dist = Client.ClientLocation.DistanceFrom(info.Target.Location);
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
                Client.ClientLocation.DistanceFrom(info.Target.Location) == 1 ||
                Client.ClientLocation.DistanceFrom(info.AttackPosition) == 0);

            if (immediate.Target != null)
                return immediate;

            // Fallback: pathfinding approach
            var fallback = validPositions.FirstOrDefault(info =>
            {
                int dist = Client.ClientLocation.DistanceFrom(info.AttackPosition);
                if (dist <= 1) return true;

                var path = Client.Pathfinder.FindPath(Client.ServerLocation, info.AttackPosition);
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
            var top = new Location(Client.Map.MapID, target.Location.X, (short)(target.Location.Y - maxDistance));
            var east = new Location(Client.Map.MapID, (short)(target.Location.X + maxDistance), target.Location.Y);
            var south = new Location(Client.Map.MapID, target.Location.X, (short)(target.Location.Y + maxDistance));
            var west = new Location(Client.Map.MapID, (short)(target.Location.X - maxDistance), target.Location.Y);

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
                    if (!Warps.Contains(current) && Client.Map.IsWalkable(Client, current))
                        yield return current;
                    current = current.Offsetter(dir);
                }
            }
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

            if (SpecialArrowAttack != null)
                Client.UseSkill(SpecialArrowAttack.Name);

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
            var pkt = new ClientPacket(48);
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

            Direction dir = Client.ClientLocation.GetDirection(target.Location);
            return MicroAdjust(dir);
        }

        private bool MicroAdjust(Direction dir)
        {
            Location nextPos = Client.ClientLocation.Offsetter(dir);
            if (!Client.Map.IsWalkable(Client, nextPos) || Warps.Contains(nextPos))
                return false;

            Client.Walk(dir);
            return true;
        }

        private bool TryApproach(Creature target)
        {
            return Client.ClientLocation.DistanceFrom(target.Location) <= 1 ||
                   MicroAdjust(target.Location.GetDirection(Client.ClientLocation));
        }

        private bool TryAmbushTech(Creature target)
        {
            if (!CanUseSpecialArrowAttack())
                return false;

            // Check alignment
            Direction targetDirection = target.Direction;
            Direction userToTargetDir = Client.ClientLocation.GetDirection(target.Location);

            if (targetDirection != userToTargetDir)
                return false;

            // Position behind the target
            Location behindTarget = target.Location.Offsetter(userToTargetDir);
            if (!Client.Map.IsWalkable(Client, behindTarget) ||
                Warps.Contains(behindTarget) ||
                !(
                     (ShadowFigure != null && Client.UseSkill(ShadowFigure.Name)) ||
                     (ShadowStrike != null && Client.UseSkill(ShadowStrike.Name)) ||
                     (Ambush != null && Client.UseSkill(Ambush.Name))
                  ))
            {
                return false;
            }

            // Manually set these after a skill jump
            Client.ClientLocation = behindTarget;
            Client.ClientDirection = userToTargetDir;

            // Ranged attacks after approach
            UseRangedAssails();
            DoActionForSurroundingCreature();
            return true;
        }

        private void TurnForAction(Creature target, Action action)
        {
            Direction originalDir = Client.ClientDirection;
            Direction newDir = target.Location.GetDirection(Client.ClientLocation);

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
            if (ArrowShot != null)
                Client.UseSkill(ArrowShot.Name);
            else if (ThrowSurigum != null)
                Client.UseSkill(ThrowSurigum.Name);
        }

        /// <summary>
        /// Attempt "Amnesia" + skill combo.
        /// </summary>
        internal bool TryComboWithAmnesia(string skillName)
        {
            if (!Client.CanUseSkill(Amnesia))
                return false;

            Skill skill = Client.Skillbook[skillName] ?? Client.Skillbook.GetNumberedSkill(skillName);
            if (!Client.CanUseSkill(skill))
                return false;

            if (Amnesia != null)
                Client.UseSkill(Amnesia.Name);
            if (skill != null)
                Client.UseSkill(skill.Name);

            return true;
        }

        internal bool TryAmnesiaTech(Creature target, string skillName)
        {
            if (!Client.CanUseSkill(Amnesia))
                return false;

            Skill skill = Client.Skillbook[skillName] ?? Client.Skillbook.GetNumberedSkill(skillName);
            if (!Client.CanUseSkill(skill))
                return false;

            // Attempt to back off
            if (!TryBackOff(target))
                return false;

            // Then turn, use "Amnesia," walk forward, and skill
            Direction dir = target.Location.GetDirection(Client.ClientLocation);
            Client.Turn(dir);
            if (Amnesia != null)
                Client.UseSkill(Amnesia.Name);
            Client.Walk(dir);
            if (skill != null)
                Client.UseSkill(skill.Name);

            return true;
        }

        

        private bool DoActionForRange1(Creature target)
        {
            if (!MeetsKillCriteria(target) || !ShouldUseSkillsOnTarget(target))
                return false;

            byte hp = target.HealthPercent;

            // For high-health targets (>=80%), try a combo using Wolf Fang Fist or Frozen Strike with Charge.
            if (hp >= 80 && Charge != null && TryComboWithSleepSkill(Charge.Name))
            {
                return true;
            }

            if (hp >= 60)
            {
                if (target._hitCounter == 0 && (AssassinStrike != null && Client.UseSkill(AssassinStrike.Name)) ||
                    (RearStrike != null && Client.NumberedSkill(RearStrike.Name)) ||
                    (StabTwice != null && TryComboWithSleepSkill(StabTwice.Name, true)) ||
                    (KidneyShot != null && TryComboWithSleepSkill(KidneyShot.Name, true)) ||
                    (AssassinStrike != null && TryAmnesiaTech(target, AssassinStrike.Name)) ||
                    (RearStrike != null && TryAmnesiaTech(target, RearStrike.Name)))
                {
                    return true;
                }
            }
        

            if (hp >= 40 &&
                ((KidneyShot != null && Client.UseSkill(KidneyShot.Name)) ||
                    (StabTwice != null && Client.UseSkill(StabTwice.Name))))
            {
                    return true;
            }

            // Try to crasher
            if (hp >= 40 && CanUseCrashers())
            {
                // If OnlyCrasherAsgall is false, or the target is Asgalled
                if (!OnlyCrasherAsgall || target.IsAsgalled)
                {
                    if (DoCrashers())
                    {
                        return true;
                    }
                }
            }

            if (hp >= 20)
            {
                if (TryAmbushTech(target))
                    return true;
            }

            if (hp <= 20 &&
                ((MantisKick != null && Client.UseSkill(MantisKick.Name)) ||
                    (HighKick != null && Client.UseSkill(HighKick.Name)) ||
                    (Kick != null && Client.UseSkill(Kick.Name))) ||
                    (StabAndTwist != null && Client.UseSkill(StabAndTwist.Name)) ||
                    (WindBlade != null && Client.UseSkill(WindBlade.Name)))
            {
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
               ((target._hitCounter == 0 && RearStrike != null && Client.UseSkill(RearStrike.Name)) ||
                (Client.ClientLocation.DistanceFrom(target.Location) == 3 && RearStrike != null && TryComboWithAmnesia(RearStrike.Name))))
            {
                return true;
            }

            // 2) If hp>=20 => check if we can do special arrow attacks if we are behind or out of direct range
            bool behindOrFar = IsFacingAway(target) || (target.Location.DistanceFrom(Client.ClientLocation) >= 3);
            if (hp >= 20 && behindOrFar)
            {
                return TryUseSpecialArrowAttacks();
            }

            return false;
        }

        private bool DoActionForSurroundingCreature()
        {
            Console.WriteLine("[DEBUG] Entering DoActionForSurroundingCreature()");
            Skill rearStrike = Client.Skillbook.GetNumberedSkill(RearStrike.Name);
            bool canRearStrike = Client.CanUseSkill(rearStrike);
            Console.WriteLine($"[DEBUG] canRearStrike: {canRearStrike}");

            // 1) Possibly do "Rear Strike" on a different target
            if (canRearStrike)
            {
                foreach (Creature mob in KillableTargets)
                {
                    if (mob != Target &&
                        mob.Location.DistanceFrom(Client.ClientLocation) > 1 &&
                        MeetsKillCriteria(mob) &&
                        ShouldUseSkillsOnTarget(mob) &&
                        SharesAxis(Client.ClientLocation, mob.Location) &&
                        (mob._hitCounter <= 0 || Client.CanUseSkill(Amnesia)))
                    {
                        Console.WriteLine($"[DEBUG] Found secondary mob {mob.Name} (ID: {mob.ID}) for Rear Strike. HitCounter: {mob._hitCounter}");
                        TurnForAction(mob, () =>
                        {
                            if (mob._hitCounter > 0)
                            {
                                if (Amnesia != null)
                                {
                                    Console.WriteLine($"[DEBUG] Using Amnesia on {mob.ID}");
                                    Client.UseSkill(Amnesia.Name);
                                }
                            }
                            if (rearStrike != null)
                            {
                                Console.WriteLine($"[DEBUG] Using Rear Strike on {mob.ID}");
                                Client.UseSkill(rearStrike.Name);
                            }
                        });
                        Console.WriteLine("[DEBUG] Exiting DoActionForSurroundingCreature() after executing Rear Strike.");
                        return true;
                    }
                }
            }

            // 2) If "Assassin Strike" is available for a close target
            if (AssassinStrike != null && Client.CanUseSkill(AssassinStrike))
            {
                foreach (Creature mob in KillableTargets)
                {
                    if (mob != Target &&
                        mob.Location.DistanceFrom(Client.ClientLocation) == 1 &&
                        mob._hitCounter <= 0 &&
                        MeetsKillCriteria(mob) &&
                        ShouldUseSkillsOnTarget(mob))
                    {
                        Console.WriteLine($"[DEBUG] Found secondary mob {mob.Name} (ID: {mob.ID}) for Assassin Strike.");
                        TurnForAction(mob, () =>
                        {
                            Console.WriteLine($"[DEBUG] Using Assassin Strike on {mob.Name}");
                            Client.UseSkill(AssassinStrike.Name);
                        });
                        Console.WriteLine("[DEBUG] Exiting DoActionForSurroundingCreature() after executing Assassin Strike.");
                        return true;
                    }
                }
            }

            Console.WriteLine("[DEBUG] No secondary action executed in DoActionForSurroundingCreature().");
            return false;
        }

        private bool DoCrashers()
        {
            if (!UseCrashers)
                return false;

            bool hasHemloch = Client.Inventory.Contains("Hemloch");
            bool canHemloch = Client.Player.HealthPercent <= 5 || hasHemloch;


            bool canCrasher = (Crasher?.CanUse ?? false);

            if (!canHemloch || canCrasher)
                return false;

            if (hasHemloch)
            {
                Client.UseItem("Hemloch");
            }

            if (Crasher != null)
                Client.UseSkill(Crasher.Name);

            Client.Player.NeedsHeal = true;
            return true;
        }
    }

}
