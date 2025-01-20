using System;
using System.Collections.Generic;
using System.Linq;
using Talos.Enumerations;
using Talos.Structs;

namespace Talos.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool ContainsI(this IEnumerable<string> enumerable, string str)
        {
            return enumerable.Contains(str, StringComparer.OrdinalIgnoreCase);
        }

        internal static IEnumerable<Location> FloodFill(this IEnumerable<Location> locations, Location start)
        {
            // Convert the collection of locations to a HashSet for faster lookup
            HashSet<Location> allLocations = locations.ToHashSet<Location>();

            // Create a set to store the filled shape, starting with the initial location
            HashSet<Location> shape = new HashSet<Location> { start };

            // Stack for locations to be discovered and processed
            Stack<Location> discoveryQueue = new Stack<Location>();
            discoveryQueue.Push(start);

            // Yield the starting location as part of the flood fill
            yield return start;

            while (discoveryQueue.Count > 0)
            {
                // Pop an item from the stack
                Location location = discoveryQueue.Pop();

                // Get neighboring locations
                foreach (Location neighbor in GetNeighbors(location, allLocations))
                {
                    // If the neighbor has not already been visited, add it to the shape and discovery queue
                    if (shape.Add(neighbor))
                    {
                        yield return neighbor;
                        discoveryQueue.Push(neighbor);
                    }
                }
            }
        }
    

        internal static IEnumerable<Location> GetNeighbors(Location location, HashSet<Location> localAllLocations)
        {
            // Offset the location by direction to get neighbors
            Location north = location.Offsetter(Direction.North);
            Location east = location.Offsetter(Direction.East);
            Location south = location.Offsetter(Direction.South);
            Location west = location.Offsetter(Direction.West);

            // Check each direction and yield the valid neighbors if they exist in the set
            Location resultLocation;
            if (localAllLocations.TryGetValue(north, out resultLocation))
            {
                yield return resultLocation;
            }
            if (localAllLocations.TryGetValue(east, out resultLocation))
            {
                yield return resultLocation;
            }
            if (localAllLocations.TryGetValue(south, out resultLocation))
            {
                yield return resultLocation;
            }
            if (localAllLocations.TryGetValue(west, out resultLocation))
            {
                yield return resultLocation;
            }
        }
    }
}
