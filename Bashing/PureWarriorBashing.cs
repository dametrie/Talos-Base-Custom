using System;
using System.Collections.Generic;
using System.Linq;
using Talos.Base;
using Talos.Objects;

namespace Talos.Bashing
{
    internal sealed class PureWarriorBashing : BashingBase
    {
        public PureWarriorBashing(Bot bot)
            : base(bot)
        {
        }

        internal override void UseSkills(Creature target)
        {
            DateTime currentTime = DateTime.UtcNow;

            // Time since last skill usage
            bool canUseSkill = (currentTime - LastUsedSkill) > TimeSpan.FromMilliseconds(SkillIntervalMs);
            // Time since last assail usage
            bool canAssail = (currentTime - LastAssailed) > TimeSpan.FromMilliseconds(1000.0);

            // Attempt AOE or multi-target logic first if enough time has passed
            if (canUseSkill && DoActionForSurroundingCreatures())
            {
                LastUsedSkill = currentTime;
                return;
            }

            // Check distance to target
            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            if (distanceToTarget > 3)
                return; // Too far to do anything else

            // Melee range = 1 tile
            if (distanceToTarget == 1)
            {
                // If target is Asgalled but can use risky skills, attempt them
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
                {
                    LastUsedSkill = currentTime;
                }
            }
            else
            {
                // Range is <3 but not 1 (i.e., 2 or 3)
                // If target is Asgalled but player not invulnerable & not bashing asgall => bail
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                    return;

                // Check if we can use skill, do something for range <3
                if (canUseSkill && DoActionForRangeLessThan3(target))
                {
                    LastUsedSkill = currentTime;
                }
            }
        }

        private bool DoActionForSurroundingCreatures()
        {
            // Filter creatures we can use skills on
            List<Creature> surroundingTargets = GetSurroundingCreatures(KillableTargets)
                .Where(ShouldUseSkillsOnTarget)
                .ToList();

            int count = surroundingTargets.Count;
            if (count < 3)
            {
                if (count == 2)
                {
                    // If any have >= 80% HP, try AOE combos
                    bool hasHighHpMob = surroundingTargets.Any(mob => mob.HealthPercent >= 80);
                    if (hasHighHpMob)
                    {
                        if (TryUnsilentAoeCombo("Dark's Mega Blade") ||
                            TryUnsilentAoeCombo("Cyclone Kick") ||
                            TryUnsilentAoeCombo("Wheel Kick") ||
                            Client.NumberedSkill("Dune Swipe"))
                            return true;
                    }
                    else
                    {
                        // If not high HP, attempt a single "Dune Swipe"
                        if (Client.NumberedSkill("Dune Swipe"))
                            return true;
                    }
                }
            }
            else
            {
                // If >= 3 surrounding
                if (TryUnsilentAoeCombo("Dark's Mega Blade") ||
                    TryUnsilentAoeCombo("Cyclone Kick") ||
                    TryUnsilentAoeCombo("Wheel Kick") ||
                    Client.NumberedSkill("Dune Swipe"))
                    return true;
            }
            return false;
        }


        private bool DoActionForRangeLessThan3(Creature target)
        {
            // Must be able to use skills on target, and must be aligned to X or Y
            // Also must be facing the correct direction to use "Strikedown"
            bool alignedToTarget =
                (target.Location.X == Client.ClientLocation.X || target.Location.Y == Client.ClientLocation.Y) &&
                target.Location.GetDirection(Client.ClientLocation) == Client.ClientDirection;

            return ShouldUseSkillsOnTarget(target) &&
                   alignedToTarget &&
                   Client.UseSkill("Strikedown");
        }

        private bool DoActionForRange1(Creature target)
        {
            // Check if we want to kill this target and we can use skills
            if (!MeetsKillCriteria(target) || !ShouldUseSkillsOnTarget(target))
                return false;

            Skill lullabyPunch = Client.Skillbook["Lullaby Punch"];
            bool lullabyPunchAvailable = (lullabyPunch != null && lullabyPunch.CanUse);
            byte hp = target.HealthPercent;

            // 1) If HP >= 80 and Lullaby Punch is available => combo with "Charge", "Strikedown", or "Dune Swipe"
            if (hp >= 80 && lullabyPunchAvailable &&
                (TryComboWithSleepSkill("Charge") ||
                 TryComboWithSleepSkill("Strikedown") ||
                 TryComboWithSleepSkill("Dune Swipe")))
            {
                return true;
            }

            // 2) HP >= 20 => NumberedSkill("Strikedown")
            if (hp >= 20 && Client.NumberedSkill("Strikedown"))
                return true;

            // 3) HP >= 40 => NumberedSkill("Dune Swipe")
            if (hp >= 40 && Client.NumberedSkill("Dune Swipe"))
                return true;

            // 4) HP >= 60 => UseSkill("Sever")
            if (hp >= 60 && Client.UseSkill("Sever"))
                return true;

            // 5) HP >= 40 => risky skills
            if (hp >= 40 && CanUseRiskySkills() && DoRiskySkills())
                return true;

            // 6) HP >= 60 => either "Dark's Mega Blade" or "Cyclone Kick"
            if (hp >= 60 && (Client.UseSkill("Dark's Mega Blade") || Client.UseSkill("Cyclone Kick")))
                return true;

            // 7) HP <= 20 => "Wind Blade"
            if (hp <= 20 && Client.UseSkill("Wind Blade"))
                return true;

            return false;
        }

        private bool DoRiskySkills()
        {
            // If HP >=66 => use "Sacrifice" or "Mad Soul"
            if (Client.HealthPct >= 66 && (Client.UseSkill("Sacrifice") || Client.UseSkill("Mad Soul")))
            {
                Client.Player.NeedsHeal = true;
                return true;
            }

            // Check if we want to do the crasher sequence
            if (!UseCrasher)
                return false;

            Skill autoHemloch = Client.Skillbook["Auto Hemloch"];
            bool canAutoHemloch = (autoHemloch != null && autoHemloch.CanUse) || Client.Player.HealthPercent <= 5;

            Skill execute = Client.Skillbook["Execute"];
            bool canExecute = (execute != null && execute.CanUse);

            Skill crasher = Client.Skillbook["Crasher"];
            bool canCrasher = (crasher != null && crasher.CanUse);

            if (!canAutoHemloch || !(canExecute || canCrasher))
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
