using System;
using System.Collections.Generic;
using Talos.Base;
using Talos.Enumerations;
using Talos.Objects;

namespace Talos.Helper
{
    public static class CreatureStateHelper
    {
        // Method to update creature states across all clients
        internal static void UpdateCreatureStates(Client castingClient, int creatureID, Dictionary<CreatureState, object> stateUpdates)
        {
            IEnumerable<Client> allClients = castingClient._server.Clients;

            foreach (var client in allClients)
            {
                if (client.WorldObjects.TryGetValue(creatureID, out var worldObject) && worldObject is Creature creature)
                {
                    foreach (var stateUpdate in stateUpdates)
                    {
                        creature.SetState(stateUpdate.Key, stateUpdate.Value);
                    }

                    Console.WriteLine($"[CreatureStateHelper] Updated Creature ID: {creatureID}, Creature Name: {creature.Name}, for Client: {client.Name}");
                }
            }
        }
    }
}