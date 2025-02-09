using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Talos.Enumerations;
using Talos.Structs;

namespace Talos.Objects
{
    internal class Creature : VisibleObject
    {
        private readonly ConcurrentDictionary<CreatureState, object> _states;

        private readonly object _stateLock = new object();
        private byte _health;
        private string _dion;
        internal int _clickCounter;
        internal int _hitCounter;
        internal DateTime LastAnimationTime { get; set; } = DateTime.Now;
        internal ushort LastAnimation { get; set; }
        public bool IsActive { get; set; } = true; // Default to active when created
        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
        internal Direction Direction { get; set; }
        internal DateTime LastStep { get; set; }

        internal Dictionary<ushort, DateTime> AnimationHistory { get; set; }
        internal Dictionary<ushort, DateTime> ForeignAnimationHistory { get; set; }
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


        public bool IsNear(Player player, int range = 1)
        {
            return Location.DistanceFrom(player.Location) <= range;
        }

        public bool IsNear(Location loctation, int range = 1)
        {
            return Location.DistanceFrom(loctation) <= range;
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
                bool isMesmerized = AnimationHistory.TryGetValue((ushort)SpellAnimation.Mesmerize, out DateTime lastMesmerizeTime) &&
                                    DateTime.UtcNow - lastMesmerizeTime < TimeSpan.FromSeconds(2.5);

                if (isMesmerized)
                {
                    return true;
                }

                // Check for 'Pramh' animation within the last 3 seconds
                bool isPramhActive = AnimationHistory.TryGetValue((ushort)SpellAnimation.Pramh, out DateTime lastPramhTime) &&
                                     DateTime.UtcNow - lastPramhTime < TimeSpan.FromSeconds(3.0);

                return isPramhActive;
            }
        }

        internal bool IsSuained
        {
            get
            {
                bool isSuained = AnimationHistory.TryGetValue((ushort)SpellAnimation.Suain, out DateTime lastSuainTime) &&
                                DateTime.UtcNow - lastSuainTime < TimeSpan.FromSeconds(2.0);

                return isSuained;
            }
        }

        internal bool IsPoisoned
        {
            get
            {
                if (AnimationHistory.TryGetValue((ushort)SpellAnimation.PinkPoison, out DateTime lastPinkPoisonTime) &&
                    DateTime.UtcNow - lastPinkPoisonTime < TimeSpan.FromSeconds(1.5))
                {
                    return true;
                }

                if (AnimationHistory.TryGetValue((ushort)SpellAnimation.GreenBubblePoison, out DateTime lastGreenBubblePoisonTime) &&
                    DateTime.UtcNow - lastGreenBubblePoisonTime < TimeSpan.FromSeconds(3.0))
                {
                    return true;
                }

                return AnimationHistory.TryGetValue((ushort)SpellAnimation.MedeniaPoison, out DateTime lastMedeniaPoisonTime) &&
                       DateTime.UtcNow - lastMedeniaPoisonTime < TimeSpan.FromSeconds(3.0);
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



        /// <summary>
        /// Checks whether the state is active (i.e. the flag is true and the elapsed time
        /// since the timestamp is less than the given duration). If not, it resets the state.
        /// </summary>
        private bool CheckAndResetState(
            CreatureState flagKey, 
            CreatureState timestampKey, 
            CreatureState durationKey, 
            CreatureState resetNameKey)
        {
            lock (_stateLock)
            {
                bool flag = GetState<bool>(flagKey);
                DateTime timestamp = GetState<DateTime>(timestampKey);
                double duration = GetState<double>(durationKey);
                double elapsed = (DateTime.UtcNow - timestamp).TotalSeconds;

                if (flag && elapsed < duration)
                {
                    return true;
                }
                else
                {
                    // Reset the associated state.
                    SetState(flagKey, false);
                    SetState(resetNameKey, string.Empty);
                    SetState(timestampKey, DateTime.MinValue);
                    SetState(durationKey, 0.0);
                    return false;
                }
            }
        }

        // CreatureState based checks
        internal bool IsCursed => CheckAndResetState(
             CreatureState.IsCursed,
             CreatureState.LastCursed,
             CreatureState.CurseDuration,
             CreatureState.CurseName
         );

        internal bool IsFassed => CheckAndResetState(
             CreatureState.IsFassed,
             CreatureState.LastFassed,
             CreatureState.FasDuration,
             CreatureState.FasName
         );

        internal bool IsDioned => CheckAndResetState(
             CreatureState.IsDioned,
             CreatureState.LastDioned,
             CreatureState.DionDuration,
             CreatureState.DionName
        );

        internal bool IsAited => CheckAndResetState(
             CreatureState.IsAited,
             CreatureState.LastAited,
             CreatureState.AiteDuration,
             CreatureState.AiteName
        );

        internal bool IsFrozen => CheckAndResetState(
             CreatureState.IsFrozen,
             CreatureState.LastFrostArrow,
             CreatureState.FrostArrowDuration,
             CreatureState.FrostArrowName
        );

        internal bool HasCursedTunes => CheckAndResetState(
             CreatureState.HasCursedTunes,
             CreatureState.LastCursedTune,
             CreatureState.CursedTuneDuration,
             CreatureState.CursedTuneName
        );

        internal bool HasRegen => CheckAndResetState(
             CreatureState.HasRegen,
             CreatureState.LastRegen,
             CreatureState.RegenDuration,
             CreatureState.RegenName
        );

        internal bool HasIncreasedRegen => CheckAndResetState(
             CreatureState.HasIncreasedRegen,
             CreatureState.LastIncreasedRegen,
             CreatureState.IncreasedRegenDuration,
             CreatureState.RegenName
        );

        internal bool HasArmachd => CheckAndResetState(
             CreatureState.HasArmachd,
             CreatureState.LastArmachd,
             CreatureState.ArmachdDuration,
             CreatureState.ArmachdName
        );

        internal double GetRemainingDionTime()
        {
            DateTime lastDioned = GetState<DateTime>(CreatureState.LastDioned);
            double dionDuration = GetState<double>(CreatureState.DionDuration);

            if (dionDuration <= 0 || lastDioned == DateTime.MinValue)
                return 0;

            // Calculate the remaining time
            double elapsedTime = (DateTime.UtcNow - lastDioned).TotalSeconds;
            return dionDuration - elapsedTime;
        }



       
        internal Creature(int id, string name, ushort sprite, byte type, Location location, Direction direction)
            : base(id, name, sprite, location)
        {
            Direction = direction;
            AnimationHistory = new Dictionary<ushort, DateTime>();
            ForeignAnimationHistory = new Dictionary<ushort, DateTime>();
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
