using System;
using System.Collections.Generic;
using System.Linq;
using Talos.Base;
using Talos.Objects;

namespace Talos.Bashing
{
    internal sealed class MonkWarriorBashing : BashingBase
    {
        public MonkWarriorBashing(Bot bot)
            : base(bot)
        {
        }

        /// <summary>
        /// UseSkills method: Tries to use the best possible skill(s) on the given target.
        /// </summary>
        /// <param name="target">The creature we're fighting.</param>
        internal override void UseSkills(Creature target)
        {
            Console.WriteLine("[DEBUG] Entering UseSkills method...");

            DateTime currentTime = DateTime.UtcNow;
            Console.WriteLine($"[DEBUG] UseSkills called at {currentTime}, Target: {target?.Name}");

            bool canUseSkill = (currentTime - LastUsedSkill) > TimeSpan.FromMilliseconds(SkillIntervalMs);
            bool canAssail = ((currentTime - LastAssailed) > TimeSpan.FromMilliseconds(1000.0));

            Console.WriteLine($"[DEBUG] canUseSkill: {canUseSkill}, canAssail: {canAssail}");

            // Attempt AOE if possible
            if (canUseSkill)
            {
                Console.WriteLine("[DEBUG] Checking DoActionForSurroundingCreatures for AOE possibility...");
                if (DoActionForSurroundingCreatures())
                {
                    Console.WriteLine("[DEBUG] AOE attack executed.");
                    LastUsedSkill = currentTime;
                    Console.WriteLine("[DEBUG] Exiting UseSkills method after AOE execution.");
                    return;
                }
            }

            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            Console.WriteLine($"[DEBUG] Distance to Target: {distanceToTarget}");

            if (distanceToTarget > 3)
            {
                Console.WriteLine("[DEBUG] Target too far away (distance > 3). Exiting UseSkills.");
                return;
            }

            if (distanceToTarget == 1)
            {
                Console.WriteLine("[DEBUG] Target is in melee range (distance == 1).");

                // Risky skills check
                if (target.IsAsgalled && canUseSkill && CanUseRiskySkills())
                {
                    Console.WriteLine("[DEBUG] Checking if DoRiskySkills can be used...");
                    if (DoRiskySkills())
                    {
                        Console.WriteLine("[DEBUG] Risky skill used successfully.");
                        LastUsedSkill = currentTime;
                    }
                }

                // Asgalled check
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                {
                    Console.WriteLine("[DEBUG] Aborting: Target is Asgalled but conditions are not met (player not Dioned and BashAsgall disabled).");
                    Console.WriteLine("[DEBUG] Exiting UseSkills method.");
                    return;
                }

                // Assail usage
                if (canAssail && (BashAsgall || !target.IsAsgalled))
                {
                    Console.WriteLine("[DEBUG] Using Assail skill...");
                    UseAssails();
                    LastAssailed = currentTime;
                }

                // Range-1 attack
                if (canUseSkill)
                {
                    Console.WriteLine("[DEBUG] Checking DoActionForRange1 for skill usage...");
                    if (DoActionForRange1(target))
                    {
                        Console.WriteLine("[DEBUG] Skill used for range 1.");
                        LastUsedSkill = currentTime;
                    }
                }
            }
            else
            {
                Console.WriteLine("[DEBUG] Target is in range 2-3.");

                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                {
                    Console.WriteLine("[DEBUG] Aborting: Target is Asgalled but conditions are not met (player not Dioned and BashAsgall disabled).");
                    Console.WriteLine("[DEBUG] Exiting UseSkills method.");
                    return;
                }

                if (canUseSkill)
                {
                    Console.WriteLine("[DEBUG] Checking DoActionForRangeLessThan3 for skill usage...");
                    if (DoActionForRangeLessThan3(target))
                    {
                        Console.WriteLine("[DEBUG] Skill used for range 2-3.");
                        LastUsedSkill = currentTime;
                    }
                }
            }

            Console.WriteLine("[DEBUG] Exiting UseSkills method normally.");
        }

        /// <summary>
        /// DoActionForSurroundingCreatures: Checks surrounding creatures for an AOE opportunity.
        /// </summary>
        /// <returns>True if an AOE skill was used, otherwise false.</returns>
        private bool DoActionForSurroundingCreatures()
        {
            Console.WriteLine("[DEBUG] Entering DoActionForSurroundingCreatures method...");

            var nearby = GetSurroundingCreatures(KillableTargets)
                .Where(ShouldUseSkillsOnTarget)
                .ToList();

            Console.WriteLine($"[DEBUG] Found {nearby.Count} surrounding creature(s).");

            if (nearby.Count >= 3 || (nearby.Count == 2 && nearby.Any(mob => mob.HealthPercent >= 80)))
            {
                Console.WriteLine("[DEBUG] Attempting AOE combos/skills...");
                bool usedAoe =
                    TryUnsilentAoeCombo("Dark's Mega Blade") ||
                    TryUnsilentAoeCombo("Cyclone Kick") ||
                    Client.NumberedSkill("Dune Swipe") ||
                    TryUnsilentAoeCombo("Wheel Kick");

                Console.WriteLine($"[DEBUG] AOE skill usage result: {usedAoe}");
                if (usedAoe)
                {
                    Console.WriteLine("[DEBUG] Exiting DoActionForSurroundingCreatures with true.");
                    return true;
                }
            }

            Console.WriteLine("[DEBUG] No AOE condition met. Exiting DoActionForSurroundingCreatures with false.");
            return false;
        }

        /// <summary>
        /// DoActionForRangeLessThan3: Attempts a skill if target is 2-3 tiles away.
        /// </summary>
        /// <param name="target">The creature in range 2-3.</param>
        /// <returns>True if a skill was successfully used, otherwise false.</returns>
        private bool DoActionForRangeLessThan3(Creature target)
        {
            Console.WriteLine("[DEBUG] Entering DoActionForRangeLessThan3 method...");

            bool axisAligned = (target.Location.X == Client.ClientLocation.X ||
                                target.Location.Y == Client.ClientLocation.Y);
            bool directionMatch = (target.Location.GetDirection(Client.ClientLocation) == Client.ClientDirection);

            Console.WriteLine($"[DEBUG] AxisAligned: {axisAligned}, DirectionMatch: {directionMatch}");

            if (!ShouldUseSkillsOnTarget(target))
            {
                Console.WriteLine("[DEBUG] ShouldUseSkillsOnTarget returned false, exiting with false.");
                return false;
            }

            bool usedSkill = axisAligned && directionMatch && Client.NumberedSkill("Strikedown");
            Console.WriteLine($"[DEBUG] Attempted 'Strikedown': {usedSkill}");

            Console.WriteLine("[DEBUG] Exiting DoActionForRangeLessThan3 method.");
            return usedSkill;
        }

        /// <summary>
        /// DoActionForRange1: Attempts specific skills if the target is in melee range.
        /// </summary>
        /// <param name="target">The melee-range creature.</param>
        /// <returns>True if a skill was successfully used, otherwise false.</returns>
        private bool DoActionForRange1(Creature target)
        {
            Console.WriteLine("[DEBUG] Entering DoActionForRange1 method...");

            if (!MeetsKillCriteria(target) || !ShouldUseSkillsOnTarget(target))
            {
                Console.WriteLine("[DEBUG] Target does not meet kill criteria or ShouldUseSkillsOnTarget returned false.");
                Console.WriteLine("[DEBUG] Exiting DoActionForRange1 method with false.");
                return false;
            }

            byte hp = target.HealthPercent;
            Console.WriteLine($"[DEBUG] Target's HealthPercent: {hp}");

            if (hp >= 80 && TryComboWithSleepSkill("Charge"))
            {
                Console.WriteLine("[DEBUG] Used 'Charge' combo successfully, returning true.");
                return true;
            }

            if (hp >= 40 && (Client.NumberedSkill("Strikedown") || Client.NumberedSkill("Dune Swipe")))
            {
                Console.WriteLine("[DEBUG] Used 'Strikedown' or 'Dune Swipe', returning true.");
                return true;
            }

            if (hp >= 40 && CanUseRiskySkills())
            {
                Console.WriteLine("[DEBUG] Checking DoRiskySkills call...");
                if (DoRiskySkills())
                {
                    Console.WriteLine("[DEBUG] Risky skills used successfully, returning true.");
                    return true;
                }
            }

            if (hp >= 60 && (Client.UseSkill("Dark's Mega Blade") || Client.UseSkill("Cyclone Kick")))
            {
                Console.WriteLine("[DEBUG] Used 'Dark's Mega Blade' or 'Cyclone Kick', returning true.");
                return true;
            }

            if (hp <= 20 && Client.UseSkill("Wind Blade"))
            {
                Console.WriteLine("[DEBUG] Used 'Wind Blade' (target HP <= 20), returning true.");
                return true;
            }

            Console.WriteLine("[DEBUG] No melee skill conditions met, returning false.");
            Console.WriteLine("[DEBUG] Exiting DoActionForRange1 method.");
            return false;
        }

        /// <summary>
        /// DoRiskySkills: Tries to execute a risky skill combination, e.g. Hemloch, Execute, Crasher.
        /// </summary>
        /// <returns>True if the risky combo was successfully used, otherwise false.</returns>
        private bool DoRiskySkills()
        {
            Console.WriteLine("[DEBUG] Entering DoRiskySkills method...");

            if (!UseCrasher)
            {
                Console.WriteLine("[DEBUG] UseCrasher is false, exiting with false.");
                return false;
            }

            bool canHemloch = (Client.Skillbook["Auto Hemloch"]?.CanUse ?? false) || Client.Player.HealthPercent <= 5;
            bool canExecute = (Client.Skillbook["Execute"]?.CanUse ?? false);
            bool canCrasher = (Client.Skillbook["Crasher"]?.CanUse ?? false);

            Console.WriteLine($"[DEBUG] canHemloch: {canHemloch}, canExecute: {canExecute}, canCrasher: {canCrasher}");

            if (!canHemloch || !(canExecute || canCrasher))
            {
                Console.WriteLine("[DEBUG] Conditions for Hemloch/Execute/Crasher not met, returning false.");
                return false;
            }

            Client.UseSkill("Auto Hemloch");
            Client.UseSkill("Execute");
            Client.UseSkill("Crasher");
            Client.Player.NeedsHeal = true;

            Console.WriteLine("[DEBUG] Risky skill combo used successfully, returning true.");
            Console.WriteLine("[DEBUG] Exiting DoRiskySkills method.");
            return true;
        }

        /// <summary>
        /// TryUnsilentAoeCombo: Attempts to use a given AOE skill, followed by "Cyclone Blade" if successful.
        /// </summary>
        /// <param name="skillName">Name of the initial AOE skill.</param>
        /// <returns>True if both skills are used successfully, otherwise false.</returns>
        private bool TryUnsilentAoeCombo(string skillName)
        {
            Console.WriteLine($"[DEBUG] Entering TryUnsilentAoeCombo method with skill '{skillName}'...");

            Skill skill = Client.Skillbook[skillName] ?? Client.Skillbook.GetNumberedSkill(skillName);
            if (skill == null || !Client.UseSkill(skill.Name))
            {
                Console.WriteLine("[DEBUG] Skill is null or usage of skill returned false, exiting TryUnsilentAoeCombo with false.");
                return false;
            }

            Console.WriteLine("[DEBUG] Skill successfully used, now attempting 'Cyclone Blade'...");
            Client.UseSkill("Cyclone Blade");
            Console.WriteLine("[DEBUG] 'Cyclone Blade' used, exiting TryUnsilentAoeCombo with true.");
            return true;
        }
    }
}
