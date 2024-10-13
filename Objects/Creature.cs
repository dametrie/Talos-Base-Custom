using System;
using System.Collections.Generic;
using Talos.Enumerations;
using Talos.Maps;
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
        public bool IsActive { get; set; } = true; // Default to active when created
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        internal Direction Direction { get; set; }
        internal DateTime LastCursed { get; set; }
        internal DateTime LastFassed { get; set; } 
        internal DateTime LastAited { get; set; }
        internal DateTime LastStep { get; set; }
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
        
        internal bool IsFrozen => DateTime.UtcNow.Subtract(LastFrostArrow).TotalSeconds < FrostArrowDuration;
        internal bool HasCursedTunes => DateTime.UtcNow.Subtract(LastCursedTune).TotalSeconds < CursedTuneDuration;
        internal bool HasRegen => DateTime.UtcNow.Subtract(LastRegen).TotalSeconds < RegenDuration;
        internal bool HasIncreasedRegen => DateTime.UtcNow.Subtract(LastIncreasedRegen).TotalSeconds < IncreasedRegenDuration;
        internal bool HasArmachd => DateTime.UtcNow.Subtract(LastArmachd).TotalSeconds < ArmachdDuration;

        // This implementation of IsAsleep is correct. However, it assumes that the sleep is not broken by any other action.
        // That is, if sleep is broken it will still wait until the original sleep duration has passed.
        // internal bool IsAsleep => DateTime.UtcNow.Subtract(LastPramhed).TotalSeconds < PramhDuration;

        // Similar problems with IsSuained. It assumes that the suain is not broken by any other action.
        // internal bool IsSuained => DateTime.UtcNow.Subtract(LastSuained).TotalSeconds < SuainDuration;

        internal bool IsAsleep
        {
            get
            {
                // Check for 'Mesmerize' animation within the last 1.5 seconds
                bool isMesmerized = SpellAnimationHistory.TryGetValue((ushort)SpellAnimation.Mesmerize, out DateTime lastMesmerizeTime) &&
                                    DateTime.UtcNow - lastMesmerizeTime < TimeSpan.FromSeconds(2.5);

                if (isMesmerized)
                {
                    return true;
                }

                // Check for 'Pramh' animation within the last 3 seconds
                bool isPramhActive = SpellAnimationHistory.TryGetValue((ushort)SpellAnimation.Pramh, out DateTime lastPramhTime) &&
                                     DateTime.UtcNow - lastPramhTime < TimeSpan.FromSeconds(3.0);

                return isPramhActive;
            }
        }

        internal bool IsSuained
        {
            get
            {
                bool isSuained = SpellAnimationHistory.TryGetValue((ushort)SpellAnimation.Suain, out DateTime lastSuainTime) &&
                                DateTime.UtcNow - lastSuainTime < TimeSpan.FromSeconds(2.0);

                return isSuained;
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
                if (SpellAnimationHistory.TryGetValue((ushort)SpellAnimation.PinkPoison, out DateTime lastPinkPoisonTime) &&
                    DateTime.UtcNow - lastPinkPoisonTime < TimeSpan.FromSeconds(1.5))
                {
                    return true;
                }

                if (SpellAnimationHistory.TryGetValue((ushort)SpellAnimation.GreenBubblePoison, out DateTime lastGreenBubblePoisonTime) &&
                    DateTime.UtcNow - lastGreenBubblePoisonTime < TimeSpan.FromSeconds(3.0))
                {
                    return true;
                }

                return SpellAnimationHistory.TryGetValue((ushort)SpellAnimation.MedeniaPoison, out DateTime lastMedeniaPoisonTime) &&
                       DateTime.UtcNow - lastMedeniaPoisonTime < TimeSpan.FromSeconds(3.0);
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
            LastStep = DateTime.UtcNow;
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
