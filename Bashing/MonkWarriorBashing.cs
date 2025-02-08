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
        private Skill AutoHemloch => Client.Skillbook["Auto Hemloch"];
        private Skill Execute => Client.Skillbook["Execute"];
        private Skill Crasher => Client.Skillbook["Crasher"];
        private Skill CycloneBlade => Client.Skillbook["Cyclone Blade"];

        // Numbered skills: try direct lookup first, then fall back to the numbered lookup.
        private Skill DuneSwipe => Client.Skillbook["Dune Swipe"] ?? Client.Skillbook.GetNumberedSkill("Dune Swipe");
        private Skill Strikedown => Client.Skillbook["Strikedown"] ?? Client.Skillbook.GetNumberedSkill("Strikedown");
        private Skill WheelKick => Client.Skillbook["Wheel Kick"] ?? Client.Skillbook.GetNumberedSkill("Wheel Kick");

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
            var now = DateTime.UtcNow;
            bool canUseSkill = (now - LastUsedSkill) > TimeSpan.FromMilliseconds(SkillIntervalMs);
            bool canAssail = (now - LastAssailed) > TimeSpan.FromMilliseconds(1000.0);

            // AOE attack
            if (canUseSkill)
            {
                if (DoActionForSurroundingCreatures())
                {
                    LastUsedSkill = now;
                    return;
                }
            }

            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            if (distanceToTarget > 3)
            {
                return;
            }

            if (distanceToTarget == 1)
            {
                // When using a skill, check if the property is not null before calling it.
                if (target.IsAsgalled && canUseSkill && CanUseRiskySkills())
                {
                    if (DoRiskySkills())
                    {
                        LastUsedSkill = now;
                    }
                }

                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                {
                    return;
                }

                if (canAssail && (BashAsgall || !target.IsAsgalled))
                {
                    UseAssails();
                    LastAssailed = now;
                }

                if (canUseSkill)
                {
                    if (DoActionForRange1(target))
                    {
                        LastUsedSkill = now;
                    }
                }
            }
            else // distance is 2 or 3
            {
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                {
                    return;
                }

                if (canUseSkill)
                {
                    if (DoActionForRangeLessThan3(target))
                    {
                        LastUsedSkill = now;
                    }
                }
            }
        }

        /// <summary>
        /// DoActionForSurroundingCreatures: Checks surrounding creatures for an AOE opportunity.
        /// </summary>
        /// <returns>True if an AOE skill was used, otherwise false.</returns>
        private bool DoActionForSurroundingCreatures()
        {
            var now = DateTime.UtcNow;
            var nearby = GetSurroundingCreatures(KillableTargets)
                         .Where(ShouldUseSkillsOnTarget)
                         .ToList();

            if (nearby.Count >= 3 || (nearby.Count == 2 && nearby.Any(mob => mob.HealthPercent >= 80)))
            {
                // Check each skill property for null before using it.
                bool usedAoe = false;

                if (DarksMegaBlade != null && TryUnsilentAoeCombo(DarksMegaBlade))
                {
                    usedAoe = true;
                }
                else if (CycloneKick != null && TryUnsilentAoeCombo(CycloneKick))
                {
                    usedAoe = true;
                }
                else if (DuneSwipe != null && TryUnsilentAoeCombo(DuneSwipe))
                {
                    usedAoe = true;
                }
                else if (WheelKick != null && TryUnsilentAoeCombo(WheelKick))
                {
                    usedAoe = true;
                }

                if (usedAoe)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// DoActionForRangeLessThan3: Attempts a skill if target is 2-3 tiles away.
        /// </summary>
        /// <param name="target">The creature in range 2-3.</param>
        /// <returns>True if a skill was successfully used, otherwise false.</returns>
        private bool DoActionForRangeLessThan3(Creature target)
        {
            var now = DateTime.UtcNow;
            bool axisAligned = (target.Location.X == Client.ClientLocation.X ||
                                target.Location.Y == Client.ClientLocation.Y);
            bool directionMatch = (target.Location.GetDirection(Client.ClientLocation) == Client.ClientDirection);

            if (!ShouldUseSkillsOnTarget(target))
            {
                return false;
            }

            // Use Strikedown if aligned correctly.
            bool usedSkill = axisAligned && directionMatch &&
                             (Strikedown != null && Client.UseSkill(Strikedown.Name));
            return usedSkill;
        }

        /// <summary>
        /// DoActionForRange1: Attempts specific skills if the target is in melee range.
        /// </summary>
        /// <param name="target">The melee-range creature.</param>
        /// <returns>True if a skill was successfully used, otherwise false.</returns>
        private bool DoActionForRange1(Creature target)
        {
            var now = DateTime.UtcNow;
            if (!MeetsKillCriteria(target) || !ShouldUseSkillsOnTarget(target))
            {
                return false;
            }

            byte hp = target.HealthPercent;

            // If the target is high on health, try using the Charge combo.
            if (hp >= 80 && Charge != null && TryComboWithSleepSkill(Charge.Name))
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

            // Try risky skills if conditions are met.
            if (hp >= 40 && CanUseRiskySkills() && DoRiskySkills())
            {
                return true;
            }

            // For higher health targets, try Dark's Mega Blade or Cyclone Kick.
            if (hp >= 60 &&
                ((DarksMegaBlade != null && Client.UseSkill(DarksMegaBlade.Name)) ||
                 (CycloneKick != null && Client.UseSkill(CycloneKick.Name))))
            {
                return true;
            }

            // For low health targets, try Wind Blade.
            if (hp <= 20 && WindBlade != null && Client.UseSkill(WindBlade.Name))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// DoRiskySkills: Tries to execute a risky skill combination, e.g. Hemloch, Execute, Crasher.
        /// </summary>
        /// <returns>True if the risky combo was successfully used, otherwise false.</returns>
        private bool DoRiskySkills()
        {
            var now = DateTime.UtcNow;
            if (!UseCrasher)
            {
                return false;
            }

            bool canHemloch = (AutoHemloch?.CanUse ?? false) || (Client.Player.HealthPercent <= 5);
            bool canExecute = Execute?.CanUse ?? false;
            bool canCrasher = Crasher?.CanUse ?? false;

            if (!canHemloch || !(canExecute || canCrasher))
            {
                return false;
            }

            if (AutoHemloch != null)
            {
                Client.UseSkill(AutoHemloch.Name);
            }
            if (Execute != null)
            {
                Client.UseSkill(Execute.Name);
            }
            if (Crasher != null)
            {
                Client.UseSkill(Crasher.Name);
            }

            Client.Player.NeedsHeal = true;
            return true;
        }

        /// <summary>
        /// TryUnsilentAoeCombo: Attempts to use a given AOE skill, followed by "Cyclone Blade" if successful.
        /// </summary>
        /// <param name="skillName">Name of the initial AOE skill.</param>
        /// <returns>True if both skills are used successfully, otherwise false.</returns>
        private bool TryUnsilentAoeCombo(Skill skill)
        {
            var now = DateTime.UtcNow;
            if (skill == null || !Client.UseSkill(skill.Name))
            {
                Console.WriteLine($"[{now:HH:mm:ss.fff}] [MORRBASH] {skill?.Name ?? "Skill"} is null or usage returned false.");
                return false;
            }

            // Attempt the follow-up skill "Cyclone Blade".
            Client.UseSkill(CycloneBlade.Name);
            return true;
        }
    }
}
