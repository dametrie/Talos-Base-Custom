using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Talos.Base;
using Talos.Definitions;
using Talos.Enumerations;
using Talos.Maps;
using Talos.Objects;
using Talos.Structs;

namespace Talos.AStar
{
    internal class Pathfinder
    {
        private PathNode[,] _pathNodes;
        private readonly byte _mapWidth;
        private readonly byte _mapHeight;
        private List<PathNode> _openNodes;
        private Map _map;
        private Client _client;

        internal static Dictionary<short, Location[]> _specialLocations = new Dictionary<short, Location[]>
        {
            {
                3058, // Pravat Deep
                Rectangle.CreateRectangleLocations(3058, 3, 9, 3, 3)
                    .Concat(Rectangle.CreateRectangleLocations(3058, 9, 1, 3, 3))
                    .ToArray()
            }
        };

        internal Pathfinder(Client client)
        {
            _client = client;
            _map = client._map;
            _mapWidth = _map.Width;
            _mapHeight = _map.Height;
            _openNodes = new List<PathNode>();
            InitializePathNodes(true);
        }
        public bool IsLocationBlocked(Location loc)
        {
            return !_pathNodes[loc.Point.X, loc.Point.Y].IsOpen;
        }
        private void InitializePathNodes(bool initializeBlockedNodes, Location location = default(Location), bool includeExits = true)
        {
            if (initializeBlockedNodes)
            {
                //Console.WriteLine("Initializing path nodes...");
                _pathNodes = new PathNode[_mapWidth, _mapHeight];
                for (short x = 0; x < _mapWidth; x++)
                {
                    for (short y = 0; y < _mapHeight; y++)
                    {
                        _pathNodes[x, y] = new PathNode(_map.MapID, x, y)
                        {
                            IsOpen = !_map.IsTileWall(x, y)
                        };
                    }
                }
                //Console.WriteLine("Path nodes initialization complete.");

                //Console.WriteLine("Initializing neighbors.");
                for (short x = 0; x < _mapWidth; x++)
                {
                    for (short y = 0; y < _mapHeight; y++)
                    {
                        PathNode currentNode = _pathNodes[x, y];
                        Location currentLocation = currentNode.Location;
                        if (currentLocation.Y > 0)
                        {
                            currentNode.Neighbors[0] = _pathNodes[currentLocation.X, currentLocation.Y - 1];
                        }
                        if (currentLocation.X < _mapWidth - 1)
                        {
                            currentNode.Neighbors[1] = _pathNodes[currentLocation.X + 1, currentLocation.Y];
                        }
                        if (currentLocation.Y < _mapHeight - 1)
                        {
                            currentNode.Neighbors[2] = _pathNodes[currentLocation.X, currentLocation.Y + 1];
                        }
                        if (currentLocation.X > 0)
                        {
                            currentNode.Neighbors[3] = _pathNodes[currentLocation.X - 1, currentLocation.Y];
                        }
                    }
                }
                //Console.WriteLine("Neighbors initialization complete.");
            
        }
            //Console.WriteLine("Collecting obstacles...");
            List<Location> obstacles = new List<Location>(from creature in _client.GetWorldObjects().OfType<Creature>()
                                                          where (creature.Type == CreatureType.Aisling || creature.Type == CreatureType.Merchant || creature.Type == CreatureType.Normal) && creature != _client.Player
                                                          select creature.Location);
            //Console.WriteLine("Obstacles from world objects:");
            foreach (var obstacle in obstacles)
            {
                //Console.WriteLine($"- {obstacle}");
            }

            //Console.WriteLine("Obstacles from special locations:");
            if (_specialLocations.TryGetValue(_map.MapID, out Location[] specialObstacles))
            {
                foreach (var obstacle in specialObstacles)
                {
                    //Console.WriteLine($"- {obstacle}");
                    if (!obstacles.Contains(obstacle))
                    {
                        obstacles.Add(obstacle);
                    }
                }
            }

            if (includeExits)
            {
                //Console.WriteLine("Obstacles from exits:");

                foreach (Location location2 in _client.GetExitLocations(location))
                {
                    //Console.WriteLine($"- {location2}");
                    if (!obstacles.Contains(location2))
                    {
                        obstacles.Add(location2);
                    }
                }
            }

            //Console.WriteLine("Obstacles from creature coverage:");
            foreach (Creature creature in _client.GetCreaturesInRange(12, CONSTANTS.GREEN_BOROS.ToArray()))
            {
                foreach (Location loc in _client.GetCreatureCoverage(creature))
                {
                    //Console.WriteLine($"- {loc}");
                    if (!obstacles.Contains(loc))
                    {
                        obstacles.Add(loc);
                    }
                }
            }

            //Console.WriteLine("Obstacles collection complete.");
            //Console.WriteLine("Resetting path nodes properties...");
            for (short x = 0; x < _mapWidth; x++)
            {
                for (short y = 0; y < _mapHeight; y++)
                {
                    PathNode currentNode = _pathNodes[x, y];
                    currentNode.IsOpen = !_map.IsLocationWall(_pathNodes[x, y].Location);
                    //Console.WriteLine($"- {currentNode.Location} is open: {currentNode.IsOpen}");
                    currentNode.IsVisited = false;
                    currentNode.IsInOpenSet = false;
                    currentNode.GCost = 0f;
                    currentNode.FCost = 0f;
                }
            }
            //Console.WriteLine("Setting obstacles...");
            foreach (Location obstacle in obstacles)
            {
                _pathNodes[obstacle.X, obstacle.Y].IsOpen = true;
            }
            //Console.WriteLine("Clearing open nodes list...");
            _openNodes.Clear();
        }



        private PathNode GetNextNode()
        {
            PathNode result = null;
            float minCost = float.MaxValue;
            foreach (PathNode node in _openNodes)
            {
                if (node != null && node.FCost < minCost)
                {
                    result = node;
                    minCost = node.FCost;
                }
            }
            return result;
        }

        internal Stack<Location> FindPath(Location start, Location end, bool ignoreObstacles = true, short threshold = 1)
        {
            //Console.WriteLine("Initializing path nodes...");
            InitializePathNodes(false, end, ignoreObstacles);
            //Console.WriteLine("Path nodes initialization complete.");

            if (Location.Equals(start, end))
            {
                //Console.WriteLine("Start and end locations are the same. Returning empty path.");
                return new Stack<Location>();
            }

            PathNode startNode = _pathNodes[start.X, start.Y];
            PathNode endNode = _pathNodes[end.X, end.Y];

            if (startNode.IsOpen && endNode.IsOpen && startNode != null && endNode != null)
            {
                //Console.WriteLine("Start and end nodes are open and not null. Proceeding with pathfinding.");

                startNode.IsInOpenSet = true;
                startNode.GCost = start.DistanceFrom(end);
                startNode.FCost = 0f;
                _openNodes.Add(startNode);

                while (true)
                {
                    if (_openNodes.Count > 0)
                    {
                        //Console.WriteLine("Nodes in open set. Proceeding with pathfinding loop.");

                        PathNode currentNode = GetNextNode();

                        if (currentNode == null)
                        {
                            //Console.WriteLine("Current node is null. Exiting pathfinding loop.");
                            break;
                        }

                        if (currentNode != endNode)
                        {
                            //Console.WriteLine("Current node is not the end node. Exploring neighbors.");

                            PathNode[] adjacentNodes = currentNode.Neighbors;

                            foreach (PathNode nextNode in adjacentNodes)
                            {
                                if (nextNode != null && !nextNode.IsVisited && (nextNode == endNode || nextNode.IsOpen))
                                {
                                    float newGCost = currentNode.GCost + 1f;
                                    float newFCost = newGCost + nextNode.Location.DistanceFrom(end);

                                    if (!nextNode.IsInOpenSet)
                                    {
                                        nextNode.GCost = newGCost;
                                        nextNode.FCost = newFCost;
                                        nextNode.ParentNode = currentNode;
                                        nextNode.IsInOpenSet = true;
                                        _openNodes.Add(nextNode);
                                    }
                                    else if (nextNode.GCost > newGCost)
                                    {
                                        nextNode.GCost = newGCost;
                                        nextNode.FCost = newFCost;
                                        nextNode.ParentNode = currentNode;
                                    }
                                }
                            }
                            _openNodes.Remove(currentNode);
                            currentNode.IsVisited = true;
                            currentNode.IsInOpenSet = false;
                            continue;
                        }

                        //Console.WriteLine("End node reached. Constructing path.");

                        Stack<Location> path = new Stack<Location>();

                        while (currentNode != startNode)
                        {
                            if (currentNode.Location.DistanceFrom(end) >= threshold)
                            {
                                path.Push(currentNode.Location);
                            }
                            currentNode = currentNode.ParentNode;
                        }
                        //Console.WriteLine($"Returning path: {path}");
                        return path;
                    }

                    //Console.WriteLine("No nodes in open set. Exiting pathfinding loop.");
                    return new Stack<Location>();
                }

                //Console.WriteLine("Requesting refresh from client.");
                _client.RequestRefresh();
                return new Stack<Location>();
            }

            //Console.WriteLine("Start or end node is closed or null. Returning empty path.");
            return new Stack<Location>();
        }

        private bool FilterCreature(Creature creature)
        {
            if (creature.Type != CreatureType.Aisling && creature.Type != CreatureType.Merchant && creature.Type != CreatureType.Normal)
            {
                return false;
            }
            return creature != _client.Player;
        }
    }

    internal sealed class PathNode
    {
        internal Location Location { get; private set; }
        internal bool IsOpen { get; set; }
        internal PathNode[] Neighbors { get; private set; }
        internal PathNode ParentNode { get; set; }
        internal bool IsVisited { get; set; }
        internal bool IsInOpenSet { get; set; }
        internal float GCost { get; set; }
        internal float FCost { get; set; }

        internal PathNode(short mapID, short x, short y)
        {
            Location = new Location(mapID, x, y);
            Neighbors = new PathNode[4];
        }
    }
}
