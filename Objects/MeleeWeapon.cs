using System;
using Talos.Enumerations;

namespace Talos.Objects
{
    internal class MeleeWeapon
    {
        private TemuairClass _temuairClass;
        internal string Name { get; set; }
        internal int AbilityRequired { get; set; }
        internal int InsightRequired { get; set; }
        internal bool MasterRequired { get; set; }
        internal bool OneHanded { get; set; }
        internal MeleeWeapon()
        {
            Name = "";
            AbilityRequired = 0;
            InsightRequired = 0;
            MasterRequired = false;
            OneHanded = false;
            _temuairClass = TemuairClass.Peasant;
        }
        internal MeleeWeapon(string name, int abilityRequired, int insightRequired, bool masterRequired, bool oneHanded, TemuairClass temuairClass = TemuairClass.Peasant)
        {
            Name = name;
            AbilityRequired = abilityRequired;
            InsightRequired = insightRequired;
            MasterRequired = masterRequired;
            OneHanded = oneHanded;
            _temuairClass = temuairClass;
        }

        internal bool CanUse(byte currentAbility, int currentInsight, TemuairClass currentTemuairClass)
        {
            return currentAbility >= AbilityRequired && currentInsight >= InsightRequired && currentTemuairClass >= _temuairClass;
        }
    }
}
