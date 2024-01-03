using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Talos.Enumerations;

namespace Talos
{
    internal struct Statistics
    {
        internal byte Level;

        internal byte Ability;

        internal uint MaxHP;

        internal uint MaxMP;

        internal uint uint_2;

        internal uint uint_3;

        internal byte Str;

        internal byte Int;

        internal byte Wis;

        internal byte Con;

        internal byte Dex;

        internal bool HasUnspentPoints;

        internal byte AvailablePoints;

        internal short MaxWeight;

        internal short CurrentWeight;

        internal uint CurrentHP;

        internal uint CurrentMP;

        internal uint Experience;

        internal uint ToNextLevel;

        internal uint AbilityExp;

        internal uint ToNextAbility;

        internal uint GamePoints;

        internal uint Gold;

        internal byte byte_8;

        internal Mail Mail;

        internal Element AttackElement;

        internal Element DefenseElement;

        internal byte MagicResistance;

        internal sbyte ArmorClass;

        internal byte Damage;

        internal byte Hit;
    }

}
