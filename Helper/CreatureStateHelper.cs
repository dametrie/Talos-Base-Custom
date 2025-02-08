using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        // Cached updates are considered stale after 5 minutes
        private const int UpdateExpiryMinutes = 5;

        // Method to update multiple creature states across all clients
        internal static void UpdateCreatureStates(Client castingClient, int creatureID, Dictionary<CreatureState, object> stateUpdates)
        {
            IEnumerable<Client> allClients = castingClient.Server.Clients;

            foreach (var client in allClients)
            {
                if (client.WorldObjects.TryGetValue(creatureID, out var worldObject) && worldObject is Creature creature)
                {
                    // Apply the updates to the creature's state
                    foreach (var stateUpdate in stateUpdates)
                    {
                        creature.SetState(stateUpdate.Key, stateUpdate.Value);
                    }

                    Console.WriteLine($"[CreatureStateHelper] Updated Creature ID: {creatureID}, Creature Name: {creature.Name}, for Client: {client.Name}");
                }
                else
                {
                    // If the creature isn't found, store the update for later
                    CachePendingUpdates(creatureID, stateUpdates);
                }
            }
        }

        internal static void UpdateCreatureState(Client castingClient, int creatureID, CreatureState state, object value)
        {
            IEnumerable<Client> allClients = castingClient.Server.Clients;

            foreach (var client in allClients)
            {
                if (client.WorldObjects.TryGetValue(creatureID, out var worldObject) && worldObject is Creature creature)
                {
                    creature.SetState(state, value);
                    if (state != CreatureState.LastStep)
                    {
                        Console.WriteLine($"[CreatureStateHelper] Updated single state {state} for Creature ID: {creatureID}");

                    }
                }
                else
                {
                    // If the creature isn't found, store the single state update for later
                    CachePendingUpdate(creatureID, state, value);
                }
            }
        }

        // Helper method to cache multiple state updates for creatures not currently in view
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

        // Helper method to cache a single state update
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

        // Method to apply cached updates when the creature becomes visible again
        internal static void ApplyCachedUpdates(Client client, Creature creature)
        {
            if (_pendingUpdates.TryGetValue(creature.ID, out var cachedUpdates))
            {
                foreach (var stateUpdate in cachedUpdates)
                {
                    creature.SetState(stateUpdate.Key, stateUpdate.Value.Value);
                }

                // Remove cached updates after applying them
                _pendingUpdates.TryRemove(creature.ID, out _);

                Console.WriteLine($"[CreatureStateHelper] Applied cached updates for Creature ID: {creature.ID}, Creature Name: {creature.Name}");
            }
        }

        // Method to clean up old cached updates
        internal static void CleanupOldCachedUpdates()
        {
            DateTime now = DateTime.UtcNow;

            foreach (var entry in _pendingUpdates.ToList())  // Convert to list to avoid enumeration issues
            {
                var creatureID = entry.Key;
                var stateUpdates = entry.Value;

                // Check if any state updates are older than the expiry time
                bool isStale = stateUpdates.All(update => (now - update.Value.Timestamp).TotalMinutes > UpdateExpiryMinutes);

                if (isStale)
                {
                    _pendingUpdates.TryRemove(creatureID, out _);
                    Console.WriteLine($"[CreatureStateHelper] Removed stale cached updates for Creature ID: {creatureID}");
                }
            }
        }
    }


}