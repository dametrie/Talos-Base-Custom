using System;
using System.Collections.Generic;
using System.Linq;
using Talos.Base;
using Talos.Objects;

namespace Talos.Bashing
{
    /// <summary>
    /// MonkWarriorBashing: Implements bashing logic for a Monk/Gladiator combination.
    /// </summary>
    internal sealed class MonkWarriorBashing : BashingBase
    {

        // Standard skills
        private Skill DarksMegaBlade => Client.Skillbook["Dark's Mega Blade"];
        private Skill CycloneKick => Client.Skillbook["Cyclone Kick"];
        private Skill Charge => Client.Skillbook["Charge"];
        private Skill WindBlade => Client.Skillbook["Wind Blade"];
        private Skill CycloneBlade => Client.Skillbook["Cyclone Blade"];

        // Numbered skills: try direct lookup first, then fall back to the numbered lookup.
        private Skill DuneSwipe => Client.Skillbook["Dune Swipe"] ?? Client.Skillbook.GetNumberedSkill("Dune Swipe");
        private Skill Strikedown => Client.Skillbook["Strikedown"] ?? Client.Skillbook.GetNumberedSkill("Strikedown");
        private Skill WheelKick => Client.Skillbook["Wheel Kick"] ?? Client.Skillbook.GetNumberedSkill("Wheel Kick");

        // Crasher skills
        private Skill AutoHemloch => Client.Skillbook["Auto Hemloch"];
        private Skill Execute => Client.Skillbook["Execute"];
        private Skill Crasher => Client.Skillbook["Crasher"];


        public MonkWarriorBashing(Bot bot)
            : base(bot)
        {
        }

        /// <summary>
        /// Tries to use the best possible skill(s) on the given target.
        /// </summary>
        /// <param name="target">The creature we're fighting.</param>
        internal override void UseSkills(Creature target)
        {
            var now = DateTime.UtcNow;
            bool canUseSkill = (now - LastUsedSkill) > TimeSpan.FromMilliseconds(SkillIntervalMs);
            bool canAssail = (now - LastAssailed) > TimeSpan.FromMilliseconds(1000.0);

            if (canUseSkill && DoActionForSurroundingCreatures())
            {
                LastUsedSkill = now;
                return;
            }

            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            if (distanceToTarget > 3)
                return;

            if (distanceToTarget == 1)
            {
                if (target.IsAsgalled && canUseSkill && CanUseCrashers())
                {
                    if (DoCrashers())
                    {
                        LastUsedSkill = now;
                    }
                }

                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                    return;

                if (canAssail && (BashAsgall || !target.IsAsgalled))
                {
                    UseAssails();
                    LastAssailed = now;
                }

                if (canUseSkill && DoActionForRange1(target))
                    LastUsedSkill = now;
            }
            else // Distance is 2 or 3 tiles.
            {
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                    return;

                if (canUseSkill && DoActionForRangeLessThan3(target))
                    LastUsedSkill = now;
            }
        }

        /// <summary>
        /// Checks surrounding creatures for an AOE opportunity.
        /// </summary>
        /// <returns>True if an AOE attack was performed; otherwise false.</returns>
        private bool DoActionForSurroundingCreatures()
        {
            var nearby = GetSurroundingCreatures(KillableTargets)
                         .Where(ShouldUseSkillsOnTarget)
                         .ToList();

            if (nearby.Count >= 3 || (nearby.Count == 2 && nearby.Any(mob => mob.HealthPercent >= 80)))
            {
                if (DarksMegaBlade != null && TryUnsilentAoeCombo(DarksMegaBlade))
                    return true;

                if (CycloneKick != null && TryUnsilentAoeCombo(CycloneKick))
                    return true;

                if (DuneSwipe != null && TryUnsilentAoeCombo(DuneSwipe))
                    return true;

                if (WheelKick != null && TryUnsilentAoeCombo(WheelKick))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to use a skill if the target is 2-3 tiles away and aligned.
        /// </summary>
        /// <param name="target">The target creature.</param>
        /// <returns>True if a skill was used successfully, otherwise false.</returns>
        private bool DoActionForRangeLessThan3(Creature target)
        {
            bool axisAligned = (target.Location.X == Client.ClientLocation.X ||
                                target.Location.Y == Client.ClientLocation.Y);
            bool directionMatch = (target.Location.GetDirection(Client.ClientLocation) == Client.ClientDirection);

            if (!ShouldUseSkillsOnTarget(target))
                return false;

            return axisAligned && directionMatch &&
                   (Strikedown != null && Client.UseSkill(Strikedown.Name));
        }

        /// <summary>
        /// Attempts to use a melee-range skill based on the target's health.
        /// </summary>
        /// <param name="target">The target in melee range.</param>
        /// <returns>True if a skill was successfully used; otherwise false.</returns>
        private bool DoActionForRange1(Creature target)
        {
            if (!MeetsKillCriteria(target) || !ShouldUseSkillsOnTarget(target))
                return false;

            byte hp = target.HealthPercent;

            // For high-health targets (>=80%), try a combo using Lullaby Punch with Charge/Strikedown/Dune Swipe.
            if (hp >= 80 &&
                (TryComboWithSleepSkill(Charge.Name) ||
                 TryComboWithSleepSkill(Strikedown.Name) ||
                 TryComboWithSleepSkill(DuneSwipe.Name)))
            {
                return true;
            }

            // For targets with higher health (>=60%), try either Dark's Mega Blade or Cyclone Kick.
            if (hp >= 60 &&
                ((DarksMegaBlade != null && Client.UseSkill(DarksMegaBlade.Name)) ||
                 (CycloneKick != null && Client.UseSkill(CycloneKick.Name))))
            {
                return true;
            }

            // For moderate health, try Strikedown or Dune Swipe.
            if (hp >= 40 &&
                ((Strikedown != null && Client.UseSkill(Strikedown.Name)) ||
                 (DuneSwipe != null && Client.UseSkill(DuneSwipe.Name))))
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

            // For very low-health targets (<=20%), try Wind Blade.
            if (hp <= 20 && WindBlade != null && Client.UseSkill(WindBlade.Name))
                return true;

            return false;
        }

        /// <summary>
        /// Tries to execute a risky skill combination, e.g. Hemloch, Execute, Crasher.
        /// </summary>
        /// <returns>True if the risky combo was successfully used, otherwise false.</returns>
        private bool DoCrashers()
        {
            if (!UseCrashers)
                return false;

            bool hasHemloch = Client.Inventory.Contains("Hemloch");
            bool autoHemlochAvailable = (AutoHemloch?.CanUse ?? false);
            bool canHemloch = autoHemlochAvailable || Client.Player.HealthPercent <= 5 || hasHemloch;

            bool canExecute = Execute?.CanUse ?? false;
            bool canCrasher = Crasher?.CanUse ?? false;

            if (!canHemloch || !(canExecute || canCrasher))
                return false;

            if (AutoHemloch != null && autoHemlochAvailable)
            {
                Client.UseSkill(AutoHemloch.Name);
            }
            else if (hasHemloch)
            {
                Client.UseItem("Hemloch");
            }

            if (Execute != null)
                Client.UseSkill(Execute.Name);
            if (Crasher != null)
                Client.UseSkill(Crasher.Name);


            Client.Player.NeedsHeal = true;
            return true;
        }

        /// <summary>
        /// Attempts an unsilent AOE combo by using the given skill and then Cyclone Blade.
        /// </summary>
        /// <param name="skill">The initial AOE skill to try.</param>
        /// <returns>True if both skills were used successfully; otherwise false.</returns>
        private bool TryUnsilentAoeCombo(Skill skill)
        {
            var now = DateTime.UtcNow;
            if (skill == null || !Client.UseSkill(skill.Name))
            {
                Console.WriteLine($"[{now:HH:mm:ss.fff}] [MORRBASH] {skill?.Name ?? "Skill"} is null or usage returned false.");
                return false;
            }

            if (CycloneBlade != null)
                Client.UseSkill(CycloneBlade.Name);

            return true;
        }
    }
}
