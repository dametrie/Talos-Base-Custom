using System;

namespace Talos.Objects
{
    internal class Bow
    {
        internal string Name { get; set; }
        internal int AbilityRequired { get; set; }
        internal int ArcherySkillRequired { get; set; }

        internal Bow()
        {
            Name = "";
            AbilityRequired = 0;
            ArcherySkillRequired = 0;
        }

        internal Bow(string name, int abilityRequired, int archerySkillRequired)
        {
            Name = name;
            AbilityRequired = abilityRequired;
            ArcherySkillRequired = archerySkillRequired;
        }

        internal bool CanUse(byte currentAbility, int currentArcherySkill)
        {
            return currentAbility >= AbilityRequired && currentArcherySkill >= ArcherySkillRequired;
        }

    }
}
