using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Talos.Enumerations;

namespace Talos.Player
{
    internal struct Statistics
    {
        internal byte _level;

        internal byte _ability;

        internal uint _maximumHP;

        internal uint _maximumMP;

        internal byte _currentStr;

        internal byte _currentInt;

        internal byte _currentWis;

        internal byte _currentCon;

        internal byte _currentDex;

        internal bool _hasUnspentPoints;

        internal byte _unspentPoints;

        internal short _maximumWeight;

        internal short _currentWeight;

        internal uint _currentHP;

        internal uint _currentMP;

        internal uint _experience;

        internal uint _toNextLevel;

        internal uint _abilityExp;

        internal uint _toNextAbility;

        internal uint _gamePoints;

        internal uint _gold;

        internal byte _blind;

        internal Mail _mail;

        internal Element _offenseElement;

        internal Element _defenseElement;

        internal byte _magicResistance;

        internal sbyte _armorClass;

        internal byte _damage;

        internal byte _hit;

        internal uint _hpToAscend;

        internal uint _mpToAscend;
    }

}
