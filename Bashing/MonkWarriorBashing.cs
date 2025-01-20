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

        internal override void UseSkills(Creature target)
        {
            DateTime currentTime = DateTime.UtcNow;

            bool canUseSkill = (currentTime - LastUsedSkill) > TimeSpan.FromMilliseconds(SkillIntervalMs);
            bool canAssail = ((currentTime - LastAssailed) > TimeSpan.FromMilliseconds(100.0));

            // Attempt AOE or multi-target logic first if enough time has passed
            if (canUseSkill && DoActionForSurroundingCreatures())
            {
                LastUsedSkill = currentTime;
                return;
            }

            // Check distance to target
            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            if (distanceToTarget > 3)
                return;

            // Melee range = 1 tile
            if (distanceToTarget == 1)
            {
                // If target is Asgalled, we can do risky skills (Crasher) if we haven't used a skill recently
                if (target.IsAsgalled && canUseSkill && CanUseRiskySkills() && DoRiskySkills())
                {
                    LastUsedSkill = currentTime;
                }

                // If target is Asgalled but player not invulnerable and not bashing Asgall, bail out
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                    return;

                // Attempt assails if we can and we’re allowed to bash asgall or target is not asgalled
                if (canAssail && (BashAsgall || !target.IsAsgalled))
                {
                    UseAssails();
                    LastAssailed = currentTime;
                }

                // If we can use skill and do something for range=1
                if (canUseSkill && DoActionForRange1(target))
                    LastUsedSkill = currentTime;
            }
            else
            {
                // Range is 2 or 3
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                    return;

                if (canUseSkill && DoActionForRangeLessThan3(target))
                    LastUsedSkill = currentTime;
            }
        }

        private bool DoActionForSurroundingCreatures()
        {
            List<Creature> nearby = GetSurroundingCreatures(KillableTargets)
                .Where(ShouldUseSkillsOnTarget)
                .ToList();

            int count = nearby.Count;
            if (count < 3)
            {
                if (count == 2)
                {
                    bool hasHighHpMob = nearby.Any(mob => mob.HealthPercent >= 80);
                    if (hasHighHpMob)
                    {
                        if (TryUnsilentAoeCombo("Dark's Mega Blade") ||
                            TryUnsilentAoeCombo("Cyclone Kick") ||
                            Client.NumberedSkill("Dune Swipe") ||
                            TryUnsilentAoeCombo("Wheel Kick"))
                        {
                            return true;
                        }
                    }
                    else if (Client.NumberedSkill("Dune Swipe"))
                    {
                        return true;
                    }
                }
            }
            else
            {
                // If 3 or more mobs
                if (TryUnsilentAoeCombo("Dark's Mega Blade") ||
                    TryUnsilentAoeCombo("Cyclone Kick") ||
                    TryUnsilentAoeCombo("Wheel Kick") ||
                    Client.NumberedSkill("Dune Swipe"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// For distance <3 (i.e., 2-3). Must align with X/Y and direction for "Strikedown."
        /// </summary>
        private bool DoActionForRangeLessThan3(Creature target)
        {
            // Must be able to use skills on target and must be axis-aligned + direction correct
            bool axisAligned = (target.Location.X == Client.ClientLocation.X || target.Location.Y == Client.ClientLocation.Y);
            bool directionMatch = (target.Location.GetDirection(Client.ClientLocation) == Client.ClientDirection);

            if (!ShouldUseSkillsOnTarget(target))
                return false;

            return axisAligned && directionMatch && Client.UseSkill("Strikedown");
        }

        private bool DoActionForRange1(Creature target)
        {
            if (!MeetsKillCriteria(target) || !ShouldUseSkillsOnTarget(target))
                return false;

            // Lullaby Punch OR Wolf Fang Fist
            Skill lullabyPunch = Client.Skillbook["Lullaby Punch"];
            Skill wolfFangFist = Client.Skillbook["Wolf Fang Fist"];

            bool hasSleepSkill = (lullabyPunch?.CanUse ?? false) || (wolfFangFist?.CanUse ?? false);

            byte hp = target.HealthPercent;

            // 1) if hp >=80 & hasSleepSkill => Try combos with "Charge", "Strikedown", "Dune Swipe"
            if (hp >= 80 && hasSleepSkill &&
                (TryComboWithSleepSkill("Charge") ||
                 TryComboWithSleepSkill("Strikedown") ||
                 TryComboWithSleepSkill("Dune Swipe")))
            {
                return true;
            }

            // 2) if hp >= 40 => NumberedSkill "Strikedown" or "Dune Swipe"
            if (hp >= 40 && (Client.NumberedSkill("Strikedown") || Client.NumberedSkill("Dune Swipe")))
                return true;

            // 3) if hp >= 40 => risky skills
            if (hp >= 40 && CanUseRiskySkills() && DoRiskySkills())
                return true;

            // 4) if hp >= 60 => use "Dark's Mega Blade" or "Cyclone Kick"
            if (hp >= 60 && (Client.UseSkill("Dark's Mega Blade") || Client.UseSkill("Cyclone Kick")))
                return true;

            // 5) if hp <= 20 => use "Wind Blade"
            if (hp <= 20 && Client.UseSkill("Wind Blade"))
                return true;

            return false;
        }

        private bool DoRiskySkills()
        {
            if (!UseCrasher)
                return false;

            Skill autoHemloch = Client.Skillbook["Auto Hemloch"];
            bool canHemloch = (autoHemloch?.CanUse ?? false) || Client.Player.HealthPercent <= 5;

            Skill execute = Client.Skillbook["Execute"];
            bool canExecute = (execute?.CanUse ?? false);

            Skill crasher = Client.Skillbook["Crasher"];
            bool canCrasher = (crasher?.CanUse ?? false);

            if (!canHemloch || !(canExecute || canCrasher))
                return false;

            Client.UseSkill("Auto Hemloch");
            Client.UseSkill("Execute");
            Client.UseSkill("Crasher");
            Client.Player.NeedsHeal = true;
            return true;
        }

        private bool TryUnsilentAoeCombo(string skillName)
        {
            Skill skill = Client.Skillbook[skillName] ?? Client.Skillbook.GetNumberedSkill(skillName);
            if (skill == null || !Client.UseSkill(skill.Name))
                return false;

            Client.UseSkill("Cyclone Blade");
            return true;
        }
    }

}
