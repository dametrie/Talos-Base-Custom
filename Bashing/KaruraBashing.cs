using System;
using System.Linq;
using Talos.Base;
using Talos.Objects;

namespace Talos.Bashing
{
    internal sealed class KaruraBashing : BashingBase
    {
        public KaruraBashing(Bot bot)
            : base(bot)
        {
        }

        internal override void UseSkills(Creature target)
        {
            DateTime currentTime = DateTime.UtcNow;

            bool canUseSkill = (currentTime - LastUsedSkill) > TimeSpan.FromMilliseconds(SkillIntervalMs);
            bool canAssail = (currentTime - LastAssailed) > TimeSpan.FromMilliseconds(1000.0);

            // Attempt AOE or multi-target logic first if enough time has passed
            if (canUseSkill && DoActionForSurroundingCreatures())
            {
                LastUsedSkill = currentTime;
                return;
            }

            // Check distance to target
            int distanceToTarget = Client.ClientLocation.DistanceFrom(target.Location);
            if (distanceToTarget != 1)
                return;

            // If the target is Asgalled, and we can do a "Crasher" combo, do it
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

        private bool DoActionForSurroundingCreatures()
        {
            var nearby = GetSurroundingCreatures(KillableTargets)
                .Where(ShouldUseSkillsOnTarget)
                .ToList();

            int count = nearby.Count;
            if (count < 3)
            {
                if (count == 2)
                {
                    // If at least one mob >=80% HP
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
                    else
                    {
                        // Attempt "Talon Kick" if no high HP mob
                        if (Client.NumberedSkill("Talon Kick"))
                            return true;
                    }
                }
            }
            else
            {
                // If >=3 mobs, try Raging Attack or Furious Bash
                if (Client.UseSkill("Raging Attack") || Client.UseSkill("Furious Bash"))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// If the target is Asgalled and we can do a crasher combo (Auto Hemloch + Animal Feast + Damage Scroll).
        /// </summary>
        private bool DoRiskySkills()
        {
            if (!UseCrasher)
                return false;

            Skill autoHemloch = Client.Skillbook["Auto Hemloch"];
            bool canHemloch = (autoHemloch?.CanUse ?? false) || Client.Player.HealthPercent <= 5;

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


        private bool DoActionForRange1(Creature target)
        {
            if (!MeetsKillCriteria(target) || !ShouldUseSkillsOnTarget(target))
                return false;

            Skill wolfFangFist = Client.Skillbook["Wolf Fang Fist"];
            bool wolfFangAvail = (wolfFangFist != null && wolfFangFist.CanUse);

            byte hp = target.HealthPercent;

            // 1) If hp >=80 & WolfFang is available => combos w/ "Sprint Potion", "Sneak Flight", or "Talon Kick"
            if (hp >= 80 && wolfFangAvail &&
                (TryComboWithSleepSkill("Sprint Potion", true) ||
                 TryComboWithSleepSkill("Sneak Flight") ||
                 TryComboWithSleepSkill("Talon Kick")))
            {
                return true;
            }

            // 2) If hp <= 20 => "Mantis Kick" OR "High Kick" OR "Kick"
            if (hp <= 20 &&
               (Client.UseSkill("Mantis Kick") | Client.UseSkill("High Kick") | Client.UseSkill("Kick")))
            {
                return true;
            }

            // 3) If hp >= 20 => "Sneak Flight" or "Talon Kick"
            if (hp >= 20 && (Client.NumberedSkill("Sneak Flight") || Client.NumberedSkill("Talon Kick")))
            {
                return true;
            }

            // 4) If hp >= 60 => Raging Attack or Whirlwind Attack (if risky)
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
