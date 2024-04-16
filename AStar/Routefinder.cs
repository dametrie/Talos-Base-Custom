using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Talos.Base;
using Talos.Forms.UI;
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
                OpenNodes.Enqueue(QueuePriorityEnum.High, startNode);
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
                                        where currentMap.MapID != _client._map.MapID || !_client.IsLocationSurrounded(exit.SourceLocation)
                                        orderby currentNode.Location.DistanceFrom(exit.SourceLocation)
                                        select exit).ToList();
                    foreach (KeyValuePair<Point, WorldMap> worldMapEntry in (from worldMapEntry in currentMap.WorldMaps
                                                                             where currentMap.MapID != _client._map.MapID || !_client.IsLocationSurrounded(new Location(currentMap.MapID, worldMapEntry.Key.X, worldMapEntry.Key.Y))
                                                                             orderby _client._serverLocation.DistanceFrom(new Location(currentMap.MapID, worldMapEntry.Key.X, worldMapEntry.Key.Y))
                                                                             select worldMapEntry).ToList())
                    {
                        foreach (WorldMapNode worldMapNode in worldMapEntry.Value.Nodes)
                        {
                            if (_server._maps.ContainsKey(worldMapNode.MapID))
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
                                    float newDistance = currentNode.AccumulatedCost + 1f;
                                    if (adjacentNode.IsOpen)
                                    {
                                        if ((double)adjacentNode.AccumulatedCost > (double)newDistance)
                                        {
                                            adjacentNode.AccumulatedCost = newDistance;
                                            adjacentNode.Parent = currentNode;
                                        }
                                    }
                                    else
                                    {
                                        adjacentNode.AccumulatedCost = newDistance;
                                        adjacentNode.Parent = currentNode;
                                        adjacentNode.IsOpen = true;
                                        OpenNodes.Enqueue(QueuePriorityEnum.High, adjacentNode);
                                    }
                                }
                            }
                        }
                    }
                    foreach (Warp exit in exits)
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
                            float newDistance = currentNode.AccumulatedCost + 1f;
                            if (adjacentNode.IsOpen)
                            {
                                if ((double)adjacentNode.AccumulatedCost > (double)newDistance)
                                {
                                    adjacentNode.AccumulatedCost = newDistance;
                                    adjacentNode.Parent = currentNode;
                                }
                            }
                            else
                            {
                                adjacentNode.AccumulatedCost = newDistance;
                                adjacentNode.Parent = currentNode;
                                adjacentNode.IsOpen = true;
                                OpenNodes.Enqueue(QueuePriorityEnum.High, adjacentNode);
                            }
                        }
                    }
                    OpenNodes.TryRemove(QueuePriorityEnum.High, currentNode);
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


        internal sealed class RouteNode
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

        }
    }
}
