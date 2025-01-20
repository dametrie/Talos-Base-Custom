using System;
using System.Collections.Generic;
using System.Linq;
using Talos.Base;
using Talos.Objects;

namespace Talos.Bashing
{
    internal sealed class FeralBashing : BashingBase
    {
        public FeralBashing(Bot bot)
            : base(bot)
        {
        }

        internal override void UseSkills(Creature target)
        {
            DateTime currentTime = DateTime.UtcNow;

            // Time since last used skill and last assail
            bool canUseSkill = (currentTime - LastUsedSkill) > TimeSpan.FromMilliseconds(SkillIntervalMs);
            bool canAssail = (currentTime - LastAssailed) > TimeSpan.FromMilliseconds(100.0);

            // Attempt AOE or multi-target logic first if enough time has passed
            if (canUseSkill && DoActionForSurroundingCreatures())
            {
                LastUsedSkill = currentTime;
                return;
            }

            // Check distance to target
            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            if (distanceToTarget > 5)
                return;

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
                    Client.UseSkill("Claw Fist");
                    UseAssails();
                    LastAssailed = currentTime;
                }

                // If we can use skill and do something for range=1
                if (canUseSkill && DoActionForRange1(target))
                    LastUsedSkill = currentTime;
            }
            else
            {
                // Range is <= 5 but not 1
                // If target is Asgalled but player not invulnerable & not bashing asgall => bail
                if (target.IsAsgalled && !Client.Player.IsDioned && !BashAsgall)
                    return;

                // If can use skill, do action for range<5
                if (canUseSkill && DoActionForRangeLessThan5(target))
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
                        if (Client.UseSkill("Raging Attack"))
                            return true;

                        if (CanUseRiskySkills() && Client.UseSkill("Whirlwind Attack"))
                        {
                            Client.Player.NeedsHeal = true;
                            return true;
                        }
                    }
                }
            }
            else
            {
                // If >= 3 mobs, try Raging Attack or Furious Bash
                if (Client.UseSkill("Raging Attack") || Client.UseSkill("Furious Bash"))
                    return true;
            }

            return false;
        }


        private bool DoRiskySkills()
        {
            if (!UseCrasher)
                return false;

            Skill autoHemloch = Client.Skillbook["Auto Hemloch"];
            bool canHemloch = (autoHemloch != null && autoHemloch.CanUse) || Client.Player.HealthPercent <= 5;

            Skill animalFeast = Client.Skillbook["Animal Feast"];
            bool canAnimalFeast = (animalFeast != null && animalFeast.CanUse);

            Skill crasher = Client.Skillbook["Crasher"];
            bool canCrasher = (crasher?.CanUse ?? false);

            if (!canHemloch || !(canAnimalFeast || canCrasher))
                return false;
            Client.UseSkill("Auto Hemloch");
            Client.UseSkill("Animal Feast");
            Client.UseSkill("Crasher");
            if (Client.HasItem("Damage Scroll"))
                Client.UseItem("Damage Scroll");
            Client.Player.NeedsHeal = true;
            return true;

        }

        /// <summary>
        /// Logic for range < 5 but not 1 tile (i.e., 2-5).
        /// Must be aligned to X or Y & facing direction for "Pounce."
        /// </summary>
        private bool DoActionForRangeLessThan5(Creature target)
        {
            if (!ShouldUseSkillsOnTarget(target))
                return false;

            bool alignedAxis = (target.Location.X == Client.ClientLocation.X || target.Location.Y == Client.ClientLocation.Y);
            bool directionMatch = (target.Location.GetDirection(Client.ClientLocation) == Client.ClientDirection);

            return alignedAxis && directionMatch && Client.NumberedSkill("Pounce");
        }


        private bool DoActionForRange1(Creature target)
        {
            if (!MeetsKillCriteria(target) || !ShouldUseSkillsOnTarget(target))
                return false;

            Skill wolfFangFist = Client.Skillbook["Wolf Fang Fist"];
            bool wolfFangAvail = (wolfFangFist != null && wolfFangFist.CanUse);

            byte hp = target.HealthPercent;

            // 1) hp <= 40 => "Pounce"
            if (hp <= 40 && Client.NumberedSkill("Pounce"))
                return true;

            // 2) hp >= 80 => "Double Rake"
            if (hp >= 80 && Client.NumberedSkill("Double Rake"))
                return true;

            // 3) hp >= 60 & wolfFangAvail => TryComboWithSleepSkill("Sprint Potion", true) or "Pounce"
            //    original logic uses "Wolf Fang Fist" for combos
            if (hp >= 60 && wolfFangAvail &&
               (TryComboWithSleepSkill("Sprint Potion", true) || TryComboWithSleepSkill("Pounce")))
            {
                return true;
            }

            // 4) hp >= 40 => "Mass Strike"
            if (hp >= 40 && Client.NumberedSkill("Mass Strike"))
                return true;

            // 5) hp <= 20 => "Mantis Kick" OR "High Kick" OR "Kick"
            if (hp <= 20 &&
               (Client.UseSkill("Mantis Kick") | Client.UseSkill("High Kick") | Client.UseSkill("Kick")))
            {
                return true;
            }

            // 6) If hp >= 60 => "Raging Attack", else if risky => "Whirlwind Attack"
            if (hp >= 60)
            {
                if (Client.UseSkill("Raging Attack"))
                    return true;

                if (CanUseRiskySkills() && DoRiskySkills() && Client.UseSkill("Whirlwind Attack"))
                {
                    Client.Player.NeedsHeal = true;
                    return true;
                }
            }

            return false;
        }
    }

}
