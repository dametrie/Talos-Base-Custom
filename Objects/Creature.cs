using System;
using System.Collections.Generic;
using Talos.Enumerations;
using Talos.Structs;

namespace Talos.Objects
{
    internal class Creature : VisibleObject
    {
        private byte _health;
        private string _dion;
        internal bool _canPND;
        internal int _clickCounter;
        internal int _hitCounter;
        internal DateTime _lastUpdate = DateTime.Now;
        internal ushort _animation;
        internal Direction Direction { get; set; }
        internal DateTime LastCursed { get; set; }
        internal DateTime LastFassed { get; set; } 
        internal DateTime LastAited { get; set; }
        internal DateTime LastWalked { get; set; }
        internal DateTime LastDioned { get; set; }
        internal double CurseDuration { get; set; }
        internal double FasDuration { get; set; }
        internal double AiteDuration { get; set; }
        internal double DionDuration { get; set; }
        internal Dictionary<ushort, DateTime> SpellAnimationHistory { get; set; }
        internal Dictionary<ushort, DateTime> SourceAnimationHistory { get; set; }
        internal CreatureType Type { get; set; }
        internal string Curse { get; set; }
        internal string Dion
        {
            get => _dion;
            set
            {
                if (value != "Asgall Faileas")
                    _canPND = true;
                _dion = value;                    
            }
        }
        internal byte Health
        {
            get => (byte)((_health > 100) ? 100: _health);
            set => _health = value;
        }
        internal bool IsDioned => DateTime.UtcNow.Subtract(LastDioned).TotalSeconds < DionDuration;
        internal bool IsCursed => DateTime.UtcNow.Subtract(LastCursed).TotalSeconds < CurseDuration;
        internal bool IsFassed => DateTime.UtcNow.Subtract(LastFassed).TotalSeconds < FasDuration;
        internal bool IsAited => DateTime.UtcNow.Subtract(LastAited).TotalSeconds < AiteDuration;
        internal bool IsWff
        {
            get 
            { 
                //wff
                if (!SpellAnimationHistory.ContainsKey(40)) { return false; }
                return DateTime.UtcNow.Subtract(SpellAnimationHistory[40]).TotalSeconds < 1.5;
            }
        }
        internal bool IsFrozen
        {
            get
            {   //frost arrow
                if (SpellAnimationHistory.ContainsKey(235) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[235]).TotalSeconds >= 2.0))
                    return true;
                //forst strike
                if (SpellAnimationHistory.ContainsKey(377) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[377]).TotalSeconds >= 1.0))
                    return true;
                return false;
            }
        }
        internal bool IsAsleep
        {
            get
            {
                //mes
                if (SpellAnimationHistory.ContainsKey(117) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[117]).TotalSeconds >= 1.5))
                    return true;
                //pramh
                if (SpellAnimationHistory.ContainsKey(32) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[32]).TotalSeconds >= 3.0))
                    return true;
                return false;
            }
        }
        internal bool IsPoisined
        {
            get
            {
                //pink poison
                if (SpellAnimationHistory.ContainsKey(25) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[25]).TotalSeconds >= 1.5))
                    return true;
                //green bubble poison
                if (SpellAnimationHistory.ContainsKey(247) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[247]).TotalSeconds >= 3.0))
                    return true;
                //med poison
                if (SpellAnimationHistory.ContainsKey(295) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[295]).TotalSeconds >= 3.0))
                    return true;
                return false;
            }
        }
        
        internal Creature(int id, string name, ushort sprite, byte type, Location location, Direction direction)
            : base(id, name, sprite, location)
        {
            Direction = direction;
            SpellAnimationHistory = new Dictionary<ushort, DateTime>();
            SourceAnimationHistory = new Dictionary<ushort, DateTime>();
            Health = 100;
            Type = (CreatureType)type;
            LastWalked = DateTime.UtcNow;
            LastCursed = DateTime.MinValue;
            LastFassed = DateTime.MinValue;
            LastAited = DateTime.MinValue;
            LastDioned = DateTime.MinValue;
            Curse = "";
            Dion = "";
        }
    }
}
