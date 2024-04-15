using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Talos.Base;
using Talos.Maps;
using Talos.Structs;

namespace Talos.AStar
{
    internal class RouteFinder
    {
        private Dictionary<Location, RouteNode> RouteNodes;
        private PriorityQueue<RouteNode> OpenNodes;
        private HashSet<Location> ClosedNodes;
        private Server _server;
        private Client _client;

        internal RouteFinder(Server server, Client client)
        {
            _server = server;
            _client = client;
            ClosedNodes = new HashSet<Location>();
        }

        internal Stack<Location> FindRoute(Location start, Location end)
        {
            Console.WriteLine($"Starting route finding from {start} to {end}");
            RouteNodes = new Dictionary<Location, RouteNode>();
            OpenNodes = new PriorityQueue<RouteNode>();

            RouteNode startNode = new RouteNode(start) { AccumulatedCost = 0f, IsOpen = true };
            RouteNode endNode = new RouteNode(end);
            RouteNodes[start] = startNode;
            RouteNodes[end] = endNode;

            OpenNodes.Enqueue(QueuePriorityEnum.High, startNode);

            while (OpenNodes.Count > 0)
            {
                if (OpenNodes.TryDequeue(out RouteNode currentNode))
                {
                    Console.WriteLine($"Dequeued node at {currentNode.Location}, Cost: {currentNode.AccumulatedCost}");

                    if (currentNode.Location.MapID == end.MapID)
                    {
                        Console.WriteLine("Reached end node.");
                        return ConstructPath(startNode, endNode);
                    }

                    if (ClosedNodes.Contains(currentNode.Location))
                    {
                        Console.WriteLine($"Skipping processing for {currentNode.Location} as it is already closed.");
                        continue;
                    }

                    ProcessCurrentNode(currentNode, end);
                    ClosedNodes.Add(currentNode.Location);
                    Console.WriteLine($"Added {currentNode.Location} to closed nodes.");

                    currentNode.IsOpen = false;
                    currentNode.IsClosed = true;
                }
            }

            Console.WriteLine("Failed to find a route.");
            return new Stack<Location>();
        }

        private void ProcessCurrentNode(RouteNode currentNode, Location end)
        {
            if (!_server._maps.ContainsKey(currentNode.Location.MapID))
            {
                Console.WriteLine($"Map ID {currentNode.Location.MapID} not found. Skipping node.");
                return; // Skip further processing of this node
            }

            Map currentMap = _server._maps[currentNode.Location.MapID];

            // Using LINQ to filter and sort exits
            var exits = currentMap.Exits.Values
                .Where(exit => currentMap.MapID != _client._map.MapID || !_client.IsLocationSurrounded(exit.SourceLocation))
                .OrderBy(exit => currentNode.Location.DistanceFrom(exit.TargetLocation))
                .ToList();

            foreach (var exit in exits)
            {
                UpdateNode(currentNode, exit.TargetLocation, exit, end);
            }

            // Using LINQ to filter and sort world map links
            var worldMapEntries = currentMap.WorldMaps
                .Where(entry => currentMap.MapID != _client._map.MapID || !_client.IsLocationSurrounded(new Location(currentMap.MapID, entry.Key.X, entry.Key.Y)))
                .OrderBy(entry => _client._serverLocation.DistanceFrom(new Location(currentMap.MapID, entry.Key.X, entry.Key.Y))) 
                .ToList();

            foreach (var entry in worldMapEntries)
            {
                foreach (var node in entry.Value.Nodes)
                {
                    if (_server._maps.ContainsKey(node.MapID))
                    {
                        var newLocation = new Location(node.MapID, node.Location.X, node.Location.Y); 
                        UpdateNode(currentNode, newLocation, new Warp((byte)entry.Key.X, (byte)entry.Key.Y, (byte)node.Location.X, (byte)node.Location.Y, currentNode.Location.MapID, node.MapID), end);
                    }
                }
            }
        }


        private void UpdateNode(RouteNode currentNode, Location targetLocation, Warp warp, Location end)
        {
            if (!RouteNodes.TryGetValue(targetLocation, out RouteNode adjacentNode))
            {
                adjacentNode = new RouteNode(targetLocation) { Warp = warp };
                RouteNodes[targetLocation] = adjacentNode;
                Console.WriteLine($"Added new node for location {targetLocation}");
            }

            float newCost = currentNode.AccumulatedCost + 1; // Assuming a cost of 1 for simplicity
            if (!adjacentNode.IsClosed && (!adjacentNode.IsOpen || adjacentNode.AccumulatedCost > newCost))
            {
                Console.WriteLine($"Updating node at {targetLocation} from cost {adjacentNode.AccumulatedCost} to {newCost}");
                adjacentNode.AccumulatedCost = newCost;
                adjacentNode.Parent = currentNode; // Ensure parent is set here correctly
                if (!adjacentNode.IsOpen)
                {
                    adjacentNode.IsOpen = true;
                    OpenNodes.Enqueue(QueuePriorityEnum.Low, adjacentNode);
                    Console.WriteLine($"Enqueued {targetLocation} with cost {newCost}");
                }
                else
                {
                    Console.WriteLine($"Re-enqueued {targetLocation} with updated cost {newCost}");
                    // Consider logic here to remove the older node from the queue if it exists
                }
            }
            else if (!adjacentNode.IsClosed && adjacentNode.Parent == null)
            {
                // If the node is not closed and has no parent, ensure it is linked back
                adjacentNode.Parent = currentNode;
                Console.WriteLine($"Linking parent to node at {targetLocation} as it was previously unlinked.");
            }
            else
            {
                Console.WriteLine($"No update required for {targetLocation} with existing cost {adjacentNode.AccumulatedCost}");
            }
        }


        private float Heuristic(Location a, Location b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y); // Manhattan distance
        }

        private Stack<Location> ConstructPath(RouteNode startNode, RouteNode endNode)
        {
            Stack<Location> path = new Stack<Location>();
            RouteNode currentNode = endNode;

            if (currentNode == null)
            {
                Console.WriteLine("End node is null, cannot construct path.");
                return path;
            }

            if (currentNode.Parent == null && currentNode != startNode)
            {
                Console.WriteLine("End node's parent is null and it is not the start node.");
            }

            while (currentNode != null && currentNode != startNode)
            {
                if (currentNode.Location == null)
                {
                    Console.WriteLine("Current node's location is null, breaking loop.");
                    break;
                }

                path.Push(currentNode.Location);
                Console.WriteLine($"Added {currentNode.Location} to path; moving to parent node.");
                currentNode = currentNode.Parent;
            }

            if (currentNode == null)
            {
                Console.WriteLine("Reached a null parent before reaching the start node.");
            }
            else if (currentNode == startNode)
            {
                Console.WriteLine($"Reached the start node; final path length: {path.Count}");
            }

            Console.WriteLine("Path construction complete. Path has " + path.Count + " steps.");
            return path;
        }

    }

    internal sealed class RouteNode
    {
        internal Warp Warp { get; set; }

        internal Location Location { get; }

        internal RouteNode Parent { get; set; }

        internal bool IsOpen { get; set; }

        internal bool IsClosed { get; set; }

        internal float AccumulatedCost { get; set; }

        internal RouteNode(Location location)
        {
            Location = location;
        }
    }
}
