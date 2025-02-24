using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                RouteNodes = new Dictionary<Location, RouteNode>();
                OpenNodes = new PriorityQueue<RouteNode>(); 
                RouteNode startNode = new RouteNode(start);
                RouteNode endNode = new RouteNode(end);
                RouteNodes.Add(start, startNode);
                RouteNodes.Add(end, endNode);
                startNode.IsOpen = true;
                startNode.AccumulatedCost = 0f;
                OpenNodes.Enqueue(startNode);
                RouteNode currentNode;
                while (true)
                {
                    if (OpenNodes.Count <= 0)
                    {
                        return new Stack<Location>();
                    }
                    currentNode = OpenNodes.Dequeue();
                    if (currentNode.Location.MapID == end.MapID)
                    {
                        break;
                    }
                    Map currentMap = _server._maps[currentNode.Location.MapID];

                    List<Warp> exits = (from exit in currentMap.Exits.Values
                                        where currentMap.MapID != _client.Map.MapID || !_client.IsLocationSurrounded(exit.SourceLocation)
                                        orderby currentNode.Location.DistanceFrom(exit.SourceLocation)
                                        select exit).ToList();

                    List<KeyValuePair<Point, WorldMap>> worldMapExits = (from worldMapEntry in currentMap.WorldMaps
                                                                             where currentMap.MapID != _client.Map.MapID || !_client.IsLocationSurrounded(new Location(currentMap.MapID, worldMapEntry.Key.X, worldMapEntry.Key.Y))
                                                                             orderby _client.ServerLocation.DistanceFrom(new Location(currentMap.MapID, worldMapEntry.Key.X, worldMapEntry.Key.Y))
                                                                             select worldMapEntry).ToList();

                    foreach (var worldMapEntry in worldMapExits)
                    {
                        foreach (WorldMapNode worldMapNode in worldMapEntry.Value.Nodes)
                        {
                            if (_server._maps.ContainsKey(worldMapNode.MapID))
                            {
                                ProcessAdjacentWorldMapLinks(currentNode, currentMap, worldMapEntry, worldMapNode);
                            }
                        }
                    }
                    foreach (Warp exit in exits)
                    {
                        ProcessAdjacentExits(end, currentNode, exit);
                    }
                    currentNode.IsOpen = false;
                    currentNode.IsClosed = true;
                }
                Stack<Location> route = new Stack<Location>();
                if (Location.NotEquals(currentNode.Location, end))
                {
                    route.Push(end);
                }
                while (true)
                {
                    if (currentNode == startNode)
                    {
                        return route;
                    }
                    if (currentNode == null || currentNode.Warp == null)
                    {
                        break;
                    }
                    _ = currentNode.Warp.SourceLocation;
                    route.Push(currentNode.Warp.SourceLocation);
                    currentNode = currentNode.Parent;
                }
                return route;
            }
            catch
            {
                return new Stack<Location>();
            }
        }

        private KeyValuePair<Point, WorldMap> ProcessAdjacentWorldMapLinks(RouteNode currentNode, Map currentMap, KeyValuePair<Point, WorldMap> worldMapEntry, WorldMapNode worldMapNode)
        {
            Location newLocation = worldMapNode.TargetLocation;
            Warp warp = new Warp((byte)worldMapEntry.Key.X, (byte)worldMapEntry.Key.Y, (byte)newLocation.X, (byte)newLocation.Y, currentMap.MapID, newLocation.MapID);
            RouteNode adjacentNode;

            if (RouteNodes.ContainsKey(newLocation))
            {
                adjacentNode = RouteNodes[newLocation];
            }
            else
            {
                adjacentNode = new RouteNode(newLocation)
                {
                    Warp = warp
                };
                RouteNodes.Add(newLocation, adjacentNode);
            }
            if (!adjacentNode.IsClosed)
            {
                float distanceCost = currentNode.Location.Point.Distance(warp.SourceLocation.Point);
                float newDistance = currentNode.AccumulatedCost + distanceCost + 5f;
                if (adjacentNode.IsOpen)
                {
                    if (adjacentNode.AccumulatedCost > newDistance)
                    {
                        adjacentNode.AccumulatedCost = newDistance;
                        adjacentNode.Parent = currentNode;
                        OpenNodes.UpdatePriority(adjacentNode);
                    }
                }
                else
                {
                    adjacentNode.AccumulatedCost = newDistance;
                    adjacentNode.Parent = currentNode;
                    adjacentNode.IsOpen = true;
                    OpenNodes.Enqueue(adjacentNode);
                }
            }
            return worldMapEntry;
        }


        private void ProcessAdjacentExits(Location end, RouteNode currentNode, Warp exit)
        {
            Location newLocation = exit.TargetLocation;
            RouteNode adjacentNode;

            if (RouteNodes.ContainsKey(newLocation) && Location.NotEquals(newLocation, end))
            {
                adjacentNode = RouteNodes[newLocation];
            }
            else if (Location.Equals(newLocation, end))
            {
                adjacentNode = RouteNodes[newLocation];
                RouteNodes[newLocation].Warp = exit;
            }
            else
            {
                adjacentNode = new RouteNode(newLocation)
                {
                    Warp = exit
                };
                RouteNodes.Add(newLocation, adjacentNode);
            }

            if (!adjacentNode.IsClosed)
            {
                // Calculate cost: current cost plus distance from current node to warp's source location plus a fixed penalty (5)
                float distanceCost = currentNode.Location.Point.Distance(exit.SourceLocation.Point);
                float newDistance = currentNode.AccumulatedCost + distanceCost + 5f;

                if (adjacentNode.IsOpen)
                {
                    if (adjacentNode.AccumulatedCost > newDistance)
                    {
                        adjacentNode.AccumulatedCost = newDistance;
                        adjacentNode.Parent = currentNode;
                        OpenNodes.UpdatePriority(adjacentNode);
                    }
                }
                else
                {
                    adjacentNode.AccumulatedCost = newDistance;
                    adjacentNode.Parent = currentNode;
                    adjacentNode.IsOpen = true;
                    OpenNodes.Enqueue(adjacentNode);
                }
            }
        }


        internal sealed class RouteNode : IComparable<RouteNode>
        {
            internal Warp Warp { get; set; }
            internal Location Location { get; }
            internal RouteNode Parent { get; set; }
            internal List<RouteNode> Neighbors { get; set; } = new List<RouteNode>();
            internal bool IsOpen { get; set; }
            internal bool IsClosed { get; set; }
            internal float AccumulatedCost { get; set; }
            internal string Source { get; set; }

            internal RouteNode(Location location)
            {
                Location = location;
                AccumulatedCost = float.MaxValue;
            }

            public int CompareTo(RouteNode other)
            {
                if (other == null)
                    return -1;
                return AccumulatedCost.CompareTo(other.AccumulatedCost);
            }

        }
    }
}
