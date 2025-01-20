using Talos.Enumerations;

namespace Talos.Objects
{
    internal record Statistics
    {
        internal byte Level { get; set; }

        internal byte Ability { get; set; }

        internal uint MaximumHP { get; set; }

        internal uint MaximumMP { get; set; }

        internal byte CurrentStr { get; set; }

        internal byte CurrentInt { get; set; }

        internal byte CurrentWis { get; set; }

        internal byte CurrentCon { get; set; }

        internal byte CurrentDex { get; set; }

        internal bool HasUnspentPoints { get; set; }

        internal byte UnspentPoints { get; set; }

        internal short MaximumWeight { get; set; }

        internal short CurrentWeight { get; set; }

        internal uint CurrentHP { get; set; }

        internal uint CurrentMP { get; set; }

        internal uint Experience { get; set; }

        internal uint ToNextLevel { get; set; }

        internal uint AbilityExperience { get; set; }

        internal uint ToNextAbility { get; set; }

        internal uint GamePoints { get; set; }

        internal uint Gold { get; set; }

        internal byte Blind { get; set; }

        internal Mail Mail { get; set; }

        internal Element OffenseElement { get; set; }

        internal Element DefenseElement { get; set; }

        internal byte MagicResistance { get; set; }

        internal sbyte ArmorClass { get; set; }

        internal byte Damage { get; set; }

        internal byte Hit { get; set; }

        internal uint HpToAscend { get; set; }

        internal uint MpToAscend { get; set; }
    }

}
