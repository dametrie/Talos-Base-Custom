using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Talos.Base;
using Talos.Enumerations;
using Talos.Objects;

namespace Talos.Helper
{
    public static class CreatureStateHelper
    {
        // Cache to store state updates for creatures not yet visible to clients
        private static readonly ConcurrentDictionary<int, Dictionary<CreatureState, (object Value, DateTime Timestamp)>> _pendingUpdates
            = new ConcurrentDictionary<int, Dictionary<CreatureState, (object Value, DateTime Timestamp)>>();

        // A dictionary to store per-creature lock objects
        private static readonly ConcurrentDictionary<int, object> _creatureLocks = new ConcurrentDictionary<int, object>();

        // Cached updates are considered stale after 15 minutes
        private const int UpdateExpiryMinutes = 15;

        // Global update cache for fingerprinting (for debouncing redundant updates).
        // Key: creature ID; Value: (LastFingerprint, LastUpdateTime)
        private static readonly ConcurrentDictionary<int, (string LastFingerprint, DateTime LastUpdateTime)> _updateCache
            = new ConcurrentDictionary<int, (string, DateTime)>();

        // Debounce interval: if an update is identical (by fingerprint) and applied within this time span, skip it.
        private static readonly TimeSpan DebounceInterval = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Computes a normalized fingerprint (string) for the update dictionary.
        /// This version ignores keys that represent timestamps (e.g. those that start with "Last").
        /// </summary>
        private static string ComputeNormalizedUpdateHash(Dictionary<CreatureState, object> stateUpdates)
        {
            var sb = new StringBuilder();
            // Order the updates by key name to ensure consistency.
            foreach (var kvp in stateUpdates.OrderBy(kvp => kvp.Key.ToString()))
            {
                // Skip keys that represent timestamps (assuming they start with "Last")
                if (kvp.Key.ToString().StartsWith("Last"))
                    continue;

                sb.Append(kvp.Key.ToString());
                sb.Append("=");

                if (kvp.Value is double d)
                {
                    // Round double values to two decimals.
                    sb.Append(d.ToString("F2", CultureInfo.InvariantCulture));
                }
                else
                {
                    // For non-double values, simply use their string representation.
                    sb.Append(kvp.Value?.ToString() ?? "null");
                }
                sb.Append(";");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Updates multiple creature states across all clients in a thread-safe and debounced fashion.
        /// </summary>
        internal static void UpdateCreatureStates(Client castingClient, int creatureID, Dictionary<CreatureState, object> stateUpdates)
        {

            object creatureLock = _creatureLocks.GetOrAdd(creatureID, id => new object());

            lock (creatureLock)
            {
                IEnumerable<Client> allClients = castingClient.Server.Clients;
                DateTime now = DateTime.UtcNow;

                // Compute a normalized fingerprint for the update.
                string newFingerprint = ComputeNormalizedUpdateHash(stateUpdates);

                // Check if we already applied an identical update very recently.
                if (_updateCache.TryGetValue(creatureID, out var cacheEntry))
                {
                    if (cacheEntry.LastFingerprint == newFingerprint && (now - cacheEntry.LastUpdateTime) < DebounceInterval)
                    {
                        Console.WriteLine($"[CreatureStateHelper] Skipping redundant update for Creature ID: {creatureID}");
                        return;
                    }
                }

                // Process the update for every client.
                foreach (var client in allClients)
                {
                    if (client.WorldObjects.TryGetValue(creatureID, out var worldObject) && worldObject is Creature creature)
                    {
                        // Lock on the creature to ensure the update is atomic.
                        lock (creature)
                        {
                            foreach (var stateUpdate in stateUpdates)
                            {
                                creature.SetState(stateUpdate.Key, stateUpdate.Value);
                            }
                        }
                        Console.WriteLine($"[CreatureStateHelper] Updated Creature ID: {creatureID}, Creature Name: {creature.Name}, for Client: {client.Name}");
                    }
                    else
                    {
                        // If the creature isn't visible, cache the update.
                        CachePendingUpdates(creatureID, stateUpdates);
                    }
                }

                // Update the global update cache with the new fingerprint and timestamp.
                _updateCache.AddOrUpdate(creatureID,
                    (newFingerprint, now),
                    (id, old) => (newFingerprint, now));
            }
        }

        /// <summary>
        /// Updates a single creature state across all clients.
        /// </summary>
        internal static void UpdateCreatureState(Client castingClient, int creatureID, CreatureState state, object value)
        {
            object creatureLock = _creatureLocks.GetOrAdd(creatureID, id => new object());

            lock (creatureLock)
            {

                IEnumerable<Client> allClients = castingClient.Server.Clients;

                foreach (var client in allClients)
                {
                    if (client.WorldObjects.TryGetValue(creatureID, out var worldObject) && worldObject is Creature creature)
                    {
                        // Lock on the creature for thread safety.
                        lock (creature)
                        {
                            creature.SetState(state, value);
                        }
                        if (state != CreatureState.LastStep)
                        {
                            Console.WriteLine($"[CreatureStateHelper] Updated single state {state} for Creature ID: {creatureID}");
                        }
                    }
                    else
                    {
                        // If the creature isn't found, store the update for later.
                        CachePendingUpdate(creatureID, state, value);
                    }
                }
            }
        }

        /// <summary>
        /// Caches multiple state updates for creatures not currently visible.
        /// </summary>
        private static void CachePendingUpdates(int creatureID, Dictionary<CreatureState, object> stateUpdates)
        {
            var updatesWithTimestamp = new Dictionary<CreatureState, (object Value, DateTime Timestamp)>();

            foreach (var stateUpdate in stateUpdates)
            {
                updatesWithTimestamp[stateUpdate.Key] = (stateUpdate.Value, DateTime.UtcNow);
            }

            _pendingUpdates.AddOrUpdate(creatureID, updatesWithTimestamp, (key, oldUpdates) =>
            {
                foreach (var update in updatesWithTimestamp)
                {
                    oldUpdates[update.Key] = update.Value;
                }
                return oldUpdates;
            });

            Console.WriteLine($"[CreatureStateHelper] Cached updates for Creature ID: {creatureID}");
        }

        /// <summary>
        /// Caches a single state update for creatures not currently visible.
        /// </summary>
        private static void CachePendingUpdate(int creatureID, CreatureState state, object value)
        {
            var update = new Dictionary<CreatureState, (object Value, DateTime Timestamp)>
            {
                { state, (value, DateTime.UtcNow) }
            };

            _pendingUpdates.AddOrUpdate(creatureID, update, (key, oldUpdates) =>
            {
                oldUpdates[state] = (value, DateTime.UtcNow);
                return oldUpdates;
            });

            if (state != CreatureState.LastStep)
                Console.WriteLine($"[CreatureStateHelper] Cached single update for Creature ID: {creatureID}, State: {state}");
        }


        /// <summary>
        /// Applies any cached updates for a creature when it becomes visible.
        /// </summary>
        internal static void ApplyCachedUpdates(Client client, Creature creature)
        {
            if (_pendingUpdates.TryGetValue(creature.ID, out var cachedUpdates))
            {
                foreach (var stateUpdate in cachedUpdates)
                {
                    creature.SetState(stateUpdate.Key, stateUpdate.Value.Value);
                }

                // Remove cached updates after applying.
                _pendingUpdates.TryRemove(creature.ID, out _);

                //Console.WriteLine($"[CreatureStateHelper] Applied cached updates for Creature ID: {creature.ID}, Creature Name: {creature.Name}");
            }
        }

        /// <summary>
        /// Cleans up stale cached updates.
        /// </summary>
        internal static void CleanupOldCachedUpdates()
        {
            DateTime now = DateTime.UtcNow;

            foreach (var entry in _pendingUpdates.ToList())
            {
                var creatureID = entry.Key;
                var stateUpdates = entry.Value;

                bool isStale = stateUpdates.All(update => (now - update.Value.Timestamp).TotalMinutes > UpdateExpiryMinutes);

                if (isStale)
                {
                    _pendingUpdates.TryRemove(creatureID, out _);
                    //Console.WriteLine($"[CreatureStateHelper] Removed stale cached updates for Creature ID: {creatureID}");
                }
            }
        }
    }
}