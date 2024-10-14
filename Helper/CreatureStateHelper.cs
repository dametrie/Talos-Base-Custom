using System;
using System.Collections.Generic;
using Talos.Base;
using Talos.Enumerations;
using Talos.Objects;

namespace Talos.Helper
{
    public static class CreatureStateHelper
    {
        // Cache to store state updates for creatures not yet visible to clients
        private static Dictionary<int, Dictionary<CreatureState, object>> _pendingUpdates = new Dictionary<int, Dictionary<CreatureState, object>>();

        // Method to update multiple creature states across all clients
        internal static void UpdateCreatureStates(Client castingClient, int creatureID, Dictionary<CreatureState, object> stateUpdates)
        {
            IEnumerable<Client> allClients = castingClient._server.Clients;

            foreach (var client in allClients)
            {
                if (client.WorldObjects.TryGetValue(creatureID, out var worldObject) && worldObject is Creature creature)
                {
                    // Creature is visible to this client, apply updates directly
                    foreach (var stateUpdate in stateUpdates)
                    {
                        creature.SetState(stateUpdate.Key, stateUpdate.Value);
                    }

                    Console.WriteLine($"[CreatureStateHelper] Updated Creature ID: {creatureID}, Creature Name: {creature.Name}, for Client: {client.Name}");
                }
                else
                {
                    // Creature not visible, cache the state updates
                    CachePendingUpdates(creatureID, stateUpdates);
                }
            }
        }

        // Method to update a single creature state across all clients
        internal static void UpdateCreatureState(Client castingClient, int creatureID, CreatureState state, object value)
        {
            IEnumerable<Client> allClients = castingClient._server.Clients;

            foreach (var client in allClients)
            {
                if (client.WorldObjects.TryGetValue(creatureID, out var worldObject) && worldObject is Creature creature)
                {
                    // Creature is visible to this client, apply the single state update
                    creature.SetState(state, value);
                    Console.WriteLine($"[CreatureStateHelper] Updated single state {state} for Creature ID: {creatureID}");
                }
                else
                {
                    // Creature not visible, cache the single state update
                    CachePendingUpdate(creatureID, state, value);
                }
            }
        }

        // Cache method for multiple state updates
        private static void CachePendingUpdates(int creatureID, Dictionary<CreatureState, object> stateUpdates)
        {
            if (!_pendingUpdates.ContainsKey(creatureID))
            {
                _pendingUpdates[creatureID] = new Dictionary<CreatureState, object>();
            }

            foreach (var update in stateUpdates)
            {
                _pendingUpdates[creatureID][update.Key] = update.Value;
            }

            Console.WriteLine($"[CreatureStateHelper] Cached state updates for Creature ID: {creatureID}");
        }

        // Cache method for a single state update
        private static void CachePendingUpdate(int creatureID, CreatureState state, object value)
        {
            if (!_pendingUpdates.ContainsKey(creatureID))
            {
                _pendingUpdates[creatureID] = new Dictionary<CreatureState, object>();
            }

            _pendingUpdates[creatureID][state] = value;
            Console.WriteLine($"[CreatureStateHelper] Cached single state {state} for Creature ID: {creatureID}");
        }

        // Method to apply cached updates when a creature becomes visible to a client
        internal static void ApplyCachedUpdates(Client client, int creatureID)
        {
            if (_pendingUpdates.TryGetValue(creatureID, out var cachedUpdates))
            {
                if (client.WorldObjects.TryGetValue(creatureID, out var worldObject) && worldObject is Creature creature)
                {
                    // Apply the cached updates to the creature
                    foreach (var update in cachedUpdates)
                    {
                        creature.SetState(update.Key, update.Value);
                    }

                    // Remove the entry from the cache after applying the updates
                    _pendingUpdates.Remove(creatureID);
                    Console.WriteLine($"[CreatureStateHelper] Applied cached updates for Creature ID: {creatureID}");
                }
            }
        }
    }

}