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
        internal DateTime LastSuained { get; set; }
        internal DateTime LastArmachd { get; set; }
        internal DateTime LastPramhed { get; set; }
        internal DateTime LastFrostArrow { get; set; }
        internal DateTime LastCursedTune { get; set; }
        internal DateTime LastRegen { get; set; }
        internal DateTime LastIncreasedRegen { get; set; }
        internal double CurseDuration { get; set; }
        internal double FasDuration { get; set; }
        internal double AiteDuration { get; set; }
        internal double DionDuration { get; set; }
        internal double PramhDuration { get; set; } 
        internal double SuainDuration { get; set; }
        internal double FrostArrowDuration { get; set; }
        internal double CursedTuneDuration { get; set; }
        internal double RegenDuration { get; set; }
        internal double IncreasedRegenDuration { get; set; }
        internal double ArmachdDuration { get; set; }
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
        internal byte HealthPercent
        {
            get => (byte)((_health > 100) ? 100 : _health);
            set => _health = value;
        }
        internal bool IsDioned => DateTime.UtcNow.Subtract(LastDioned).TotalSeconds < DionDuration;
        internal bool IsCursed => DateTime.UtcNow.Subtract(LastCursed).TotalSeconds < CurseDuration;
        internal bool IsFassed => DateTime.UtcNow.Subtract(LastFassed).TotalSeconds < FasDuration;
        internal bool IsAited => DateTime.UtcNow.Subtract(LastAited).TotalSeconds < AiteDuration;
        internal bool IsAsleep => DateTime.UtcNow.Subtract(LastPramhed).TotalSeconds < PramhDuration;
        internal bool IsSuained => DateTime.UtcNow.Subtract(LastSuained).TotalSeconds < SuainDuration;
        internal bool IsFrozen => DateTime.UtcNow.Subtract(LastFrostArrow).TotalSeconds < FrostArrowDuration;
        internal bool HasCursedTunes => DateTime.UtcNow.Subtract(LastCursedTune).TotalSeconds < CursedTuneDuration;
        internal bool HasRegen => DateTime.UtcNow.Subtract(LastRegen).TotalSeconds < RegenDuration;
        internal bool HasIncreasedRegen => DateTime.UtcNow.Subtract(LastIncreasedRegen).TotalSeconds < IncreasedRegenDuration;
        internal bool HasArmachd => DateTime.UtcNow.Subtract(LastArmachd).TotalSeconds < ArmachdDuration;

        internal bool IsWFF
        {
            get
            {
                if (!SpellAnimationHistory.ContainsKey((ushort)SpellAnimation.Suain))
                    return false;
                return DateTime.UtcNow.Subtract(SpellAnimationHistory[(ushort)SpellAnimation.Suain]).TotalSeconds < 2.0;
            }
        }
       /* internal bool IsFrozen
        {
            get
            {
                if (SpellAnimationHistory.ContainsKey((ushort)SpellAnimation.FrostArrow) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[(ushort)SpellAnimation.FrostArrow]).TotalSeconds >= 2.0))
                    return true;
                if (SpellAnimationHistory.ContainsKey((ushort)SpellAnimation.FrostStrike) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[(ushort)SpellAnimation.FrostStrike]).TotalSeconds >= 1.0))
                    return true;
                return false;
            }
        }*/

        internal bool IsPoisoned
        {
            get
            {
                //pink poison
                if (SpellAnimationHistory.ContainsKey((ushort)SpellAnimation.PinkPoison) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[(ushort)SpellAnimation.PinkPoison]).TotalSeconds >= 1.5))
                    return true;
                //green bubble poison
                if (SpellAnimationHistory.ContainsKey((ushort)SpellAnimation.GreenBubblePoison) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[(ushort)SpellAnimation.GreenBubblePoison]).TotalSeconds >= 3.0))
                    return true;
                //med poison
                if (SpellAnimationHistory.ContainsKey((ushort)SpellAnimation.MedeniaPoison) && !(DateTime.UtcNow.Subtract(SpellAnimationHistory[(ushort)SpellAnimation.MedeniaPoison]).TotalSeconds >= 3.0))
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
            HealthPercent = 100;
            Type = (CreatureType)type;
            LastWalked = DateTime.UtcNow;
            LastCursed = DateTime.MinValue;
            LastFassed = DateTime.MinValue;
            LastAited = DateTime.MinValue;
            LastDioned = DateTime.MinValue;
            LastArmachd = DateTime.MinValue;
            Curse = "";
            Dion = "";
        }
    }
}
