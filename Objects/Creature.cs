using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Talos.Enumerations;
using Talos.Maps;
using Talos.Structs;

namespace Talos.Objects
{
    internal class Creature : VisibleObject
    {
        private readonly ConcurrentDictionary<CreatureState, object> _states;

        private byte _health;
        private string _dion;
        internal int _clickCounter;
        internal int _hitCounter;
        internal DateTime _lastAnimationTime = DateTime.Now;
        internal ushort _lastAnimation;
        public bool IsActive { get; set; } = true; // Default to active when created
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        internal Direction Direction { get; set; }
        internal DateTime LastStep { get; set; }

        internal Dictionary<ushort, DateTime> LastAnimation { get; set; }
        internal Dictionary<ushort, DateTime> LastForeignAnimation { get; set; }
        internal CreatureType Type { get; set; }

        internal bool CanPND
        {
            get
            {
                string dionName = GetState<string>(CreatureState.DionName);
                return dionName != "Asgall Faileas";
            }
        }


        internal byte HealthPercent
        {
            get => (byte)((_health > 100) ? 100 : _health);
            set => _health = value;
        }


        // This implementation of IsAsleep is correct. However, it assumes that the sleep is not broken by any other action.
        // That is, if sleep is broken it will still wait until the original sleep duration has passed.
        // internal bool IsAsleep => DateTime.UtcNow.Subtract(LastPramhed).TotalSeconds < PramhDuration;

        // Similar problems with IsSuained. It assumes that the suain is not broken by any other action.
        // internal bool IsSuained => DateTime.UtcNow.Subtract(LastSuained).TotalSeconds < SuainDuration;


        // Animation based checks
        internal bool IsAsleep
        {
            get
            {
                // Check for 'Mesmerize' animation within the last 1.5 seconds
                bool isMesmerized = LastAnimation.TryGetValue((ushort)SpellAnimation.Mesmerize, out DateTime lastMesmerizeTime) &&
                                    DateTime.UtcNow - lastMesmerizeTime < TimeSpan.FromSeconds(2.5);

                if (isMesmerized)
                {
                    return true;
                }

                // Check for 'Pramh' animation within the last 3 seconds
                bool isPramhActive = LastAnimation.TryGetValue((ushort)SpellAnimation.Pramh, out DateTime lastPramhTime) &&
                                     DateTime.UtcNow - lastPramhTime < TimeSpan.FromSeconds(3.0);

                return isPramhActive;
            }
        }

        internal bool IsSuained
        {
            get
            {
                bool isSuained = LastAnimation.TryGetValue((ushort)SpellAnimation.Suain, out DateTime lastSuainTime) &&
                                DateTime.UtcNow - lastSuainTime < TimeSpan.FromSeconds(2.0);

                return isSuained;
            }
        }

        internal bool IsPoisoned
        {
            get
            {
                if (LastAnimation.TryGetValue((ushort)SpellAnimation.PinkPoison, out DateTime lastPinkPoisonTime) &&
                    DateTime.UtcNow - lastPinkPoisonTime < TimeSpan.FromSeconds(1.5))
                {
                    return true;
                }

                if (LastAnimation.TryGetValue((ushort)SpellAnimation.GreenBubblePoison, out DateTime lastGreenBubblePoisonTime) &&
                    DateTime.UtcNow - lastGreenBubblePoisonTime < TimeSpan.FromSeconds(3.0))
                {
                    return true;
                }

                return LastAnimation.TryGetValue((ushort)SpellAnimation.MedeniaPoison, out DateTime lastMedeniaPoisonTime) &&
                       DateTime.UtcNow - lastMedeniaPoisonTime < TimeSpan.FromSeconds(3.0);
            }
        }




        // CreatureState based checks
        internal bool IsCursed
        {
            get
            {
                bool isCursed = GetState<bool>(CreatureState.IsCursed);
                DateTime lastCursed = GetState<DateTime>(CreatureState.LastCursed);
                double duration = GetState<double>(CreatureState.CurseDuration);

                if (isCursed && (DateTime.UtcNow - lastCursed).TotalSeconds < duration)
                {
                    return true;
                }
                else
                {
                    // Automatically reset the state if expired
                    SetState(CreatureState.IsCursed, false);
                    SetState(CreatureState.CurseName, string.Empty);
                    SetState(CreatureState.LastCursed, DateTime.MinValue);
                    SetState(CreatureState.CurseDuration, 0.0);
                    return false;
                }
            }
        }


        internal bool IsFassed
        {
            get
            {
                bool isFassed = GetState<bool>(CreatureState.IsFassed);
                DateTime lastFassed = GetState<DateTime>(CreatureState.LastFassed);
                double duration = GetState<double>(CreatureState.FasDuration);

                if (isFassed && (DateTime.UtcNow - lastFassed).TotalSeconds < duration)
                {
                    return true;
                }
                else
                {
                    SetState(CreatureState.IsFassed, false);
                    SetState(CreatureState.FasName, string.Empty);
                    SetState(CreatureState.LastFassed, DateTime.MinValue);
                    SetState(CreatureState.FasDuration, 0.0);
                    return false;
                }
            }
        }

        internal bool IsDioned
        {
            get
            {
                bool isDioned = GetState<bool>(CreatureState.IsDioned);
                DateTime lastDioned = GetState<DateTime>(CreatureState.LastDioned);
                double duration = GetState<double>(CreatureState.DionDuration);

                if (isDioned && (DateTime.UtcNow - lastDioned).TotalSeconds < duration)
                {
                    return true;
                }
                else
                {
                    SetState(CreatureState.IsDioned, false);
                    SetState(CreatureState.DionName, string.Empty);
                    SetState(CreatureState.LastDioned, DateTime.MinValue);
                    SetState(CreatureState.DionDuration, 0.0);
                    return false;
                }
            }
        }

        internal bool IsAsgalled
        {
            get
            {
                // We treat Asgall as a special case of Dion 
                return IsDioned && GetState<string>(CreatureState.DionName) == "Asgall Faileas";
            }
        }

        internal bool IsAited
        {
            get
            {
                bool isAited = GetState<bool>(CreatureState.IsAited);
                DateTime lastAited = GetState<DateTime>(CreatureState.LastAited);
                double duration = GetState<double>(CreatureState.AiteDuration);

                if (isAited && (DateTime.UtcNow - lastAited).TotalSeconds < duration)
                {
                    return true;
                }
                else
                {
                    SetState(CreatureState.IsAited, false);
                    SetState(CreatureState.AiteName, string.Empty);
                    SetState(CreatureState.LastAited, DateTime.MinValue);
                    SetState(CreatureState.AiteDuration, 0.0);
                    return false;
                }
            }
        }

        internal bool IsFrozen
        {
            get
            {
                bool isFrozen = GetState<bool>(CreatureState.IsFrozen);
                DateTime lastFrostArrow = GetState<DateTime>(CreatureState.LastFrostArrow);
                double duration = GetState<double>(CreatureState.FrostArrowDuration);

                if (isFrozen && (DateTime.UtcNow - lastFrostArrow).TotalSeconds < duration)
                {
                    return true;
                }
                else
                {
                    SetState(CreatureState.IsFrozen, false);
                    SetState(CreatureState.LastFrostArrow, DateTime.MinValue);
                    SetState(CreatureState.FrostArrowDuration, 0.0);
                    return false;
                }
            }
        }

        internal bool HasCursedTunes
        {
            get
            {
                bool hasCursedTunes = GetState<bool>(CreatureState.HasCursedTunes);
                DateTime lastCursedTune = GetState<DateTime>(CreatureState.LastCursedTune);
                double duration = GetState<double>(CreatureState.CursedTuneDuration);

                if (hasCursedTunes && (DateTime.UtcNow - lastCursedTune).TotalSeconds < duration)
                {
                    return true;
                }
                else
                {
                    SetState(CreatureState.HasCursedTunes, false);
                    SetState(CreatureState.LastCursedTune, DateTime.MinValue);
                    SetState(CreatureState.CursedTuneDuration, 0.0);
                    return false;
                }
            }
        }

        internal bool HasRegen
        {
            get
            {
                bool hasRegen = GetState<bool>(CreatureState.HasRegen);
                DateTime lastRegen = GetState<DateTime>(CreatureState.LastRegen);
                double duration = GetState<double>(CreatureState.RegenDuration);

                if (hasRegen && (DateTime.UtcNow - lastRegen).TotalSeconds < duration)
                {
                    return true;
                }
                else
                {
                    SetState(CreatureState.HasRegen, false);
                    SetState(CreatureState.LastRegen, DateTime.MinValue);
                    SetState(CreatureState.RegenDuration, 0.0);
                    SetState(CreatureState.RegenName, string.Empty);
                    return false;
                }
            }
        }

        internal bool HasIncreasedRegen
        {
            get
            {
                bool hasIncreasedRegen = GetState<bool>(CreatureState.HasIncreasedRegen);
                DateTime lastIncreasedRegen = GetState<DateTime>(CreatureState.LastIncreasedRegen);
                double duration = GetState<double>(CreatureState.IncreasedRegenDuration);

                if (hasIncreasedRegen && (DateTime.UtcNow - lastIncreasedRegen).TotalSeconds < duration)
                {
                    return true;
                }
                else
                {
                    SetState(CreatureState.HasIncreasedRegen, false);
                    SetState(CreatureState.LastIncreasedRegen, DateTime.MinValue);
                    SetState(CreatureState.IncreasedRegenDuration, 0.0);
                    SetState(CreatureState.RegenName, string.Empty);
                    return false;
                }
            }
        }

        internal bool HasArmachd
        {
            get
            {
                bool hasArmachd = GetState<bool>(CreatureState.HasArmachd);
                DateTime lastArmachd = GetState<DateTime>(CreatureState.LastArmachd);
                double duration = GetState<double>(CreatureState.ArmachdDuration);

                if (hasArmachd && (DateTime.UtcNow - lastArmachd).TotalSeconds < duration)
                {
                    return true;
                }
                else
                {
                    // Reset state if expired
                    SetState(CreatureState.HasArmachd, false);
                    SetState(CreatureState.LastArmachd, DateTime.MinValue);
                    SetState(CreatureState.ArmachdDuration, 0.0);
                    return false;
                }
            }
        }


        internal Creature(int id, string name, ushort sprite, byte type, Location location, Direction direction)
            : base(id, name, sprite, location)
        {
            Direction = direction;
            LastAnimation = new Dictionary<ushort, DateTime>();
            LastForeignAnimation = new Dictionary<ushort, DateTime>();
            HealthPercent = 100;
            Type = (CreatureType)type;

            _states = new ConcurrentDictionary<CreatureState, object>();

            InitializeDefaultStates();
        }

        private void InitializeDefaultStates()
        {
            // Boolean states
            _states[CreatureState.IsCursed] = false;
            _states[CreatureState.IsFassed] = false;
            _states[CreatureState.IsAited] = false;
            _states[CreatureState.IsDioned] = false;
            _states[CreatureState.IsFrozen] = false;
            _states[CreatureState.HasCursedTunes] = false;
            _states[CreatureState.HasRegen] = false;
            _states[CreatureState.HasIncreasedRegen] = false;
            _states[CreatureState.HasArmachd] = false;
            _states[CreatureState.IsAsleep] = false;
            _states[CreatureState.IsSuained] = false;
            _states[CreatureState.IsPoisoned] = false;

            // Timestamps
            DateTime minDateTime = DateTime.MinValue;
            _states[CreatureState.LastCursed] = minDateTime;
            _states[CreatureState.LastFassed] = minDateTime;
            _states[CreatureState.LastAited] = minDateTime;
            _states[CreatureState.LastDioned] = minDateTime;
            _states[CreatureState.LastFrostArrow] = minDateTime;
            _states[CreatureState.LastCursedTune] = minDateTime;
            _states[CreatureState.LastRegen] = minDateTime;
            _states[CreatureState.LastIncreasedRegen] = minDateTime;
            _states[CreatureState.LastArmachd] = minDateTime;
            _states[CreatureState.LastPramhed] = minDateTime;
            _states[CreatureState.LastSuained] = minDateTime;
            _states[CreatureState.LastStep] = minDateTime;

            // Durations
            double zeroDuration = 0.0;
            _states[CreatureState.CurseDuration] = zeroDuration;
            _states[CreatureState.FasDuration] = zeroDuration;
            _states[CreatureState.AiteDuration] = zeroDuration;
            _states[CreatureState.DionDuration] = zeroDuration;
            _states[CreatureState.PramhDuration] = zeroDuration;
            _states[CreatureState.SuainDuration] = zeroDuration;
            _states[CreatureState.FrostArrowDuration] = zeroDuration;
            _states[CreatureState.CursedTuneDuration] = zeroDuration;
            _states[CreatureState.RegenDuration] = zeroDuration;
            _states[CreatureState.IncreasedRegenDuration] = zeroDuration;
            _states[CreatureState.ArmachdDuration] = zeroDuration;


            _states[CreatureState.CurseName] = string.Empty;
            _states[CreatureState.FasName] = string.Empty;
            _states[CreatureState.AiteName] = string.Empty;
            _states[CreatureState.DionName] = string.Empty;
            _states[CreatureState.RegenName] = string.Empty;
            _states[CreatureState.PramhName] = string.Empty;


        }

        // Methods to set and get state values
        public void SetState(CreatureState state, object value)
        {
            _states[state] = value;
        }

        public T GetState<T>(CreatureState state)
        {
            if (_states.TryGetValue(state, out object value) && value is T typedValue)
            {
                return typedValue;
            }
            return default(T);
        }

    }
}
