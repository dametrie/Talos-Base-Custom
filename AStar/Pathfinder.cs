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
        private List<PathNode> _blockedNodes;
        private Map _map;
        private Client _client;

        internal static Dictionary<short, Location[]> _specialLocations = new Dictionary<short, Location[]>
        {
            //{
            //    3058, // Pravat Deep?
            //   /new Rectangle(new Location(3, new Point(3, 9)), new Location(3, new Point(3, 3))).LocationList.Concat(new RectangleZeus(new Location(9, new Point(9, 1)), new Location(9, new Point(3, 3))).LocationList).ToArray()
            //}
        };

        internal Pathfinder(Client client)
        {
            _client = client;
            _map = client._map;
            _mapWidth = _map.Width;
            _mapHeight = _map.Height;
            _blockedNodes = new List<PathNode>();
            InitializePathNodes(true);
        }

        private void InitializePathNodes(bool initializeBlockedNodes, Location location = default(Location), bool includeAdditionalObstacles = true)
        {
            if (initializeBlockedNodes)
            {
                _pathNodes = new PathNode[_mapWidth, _mapHeight];
                for (short x = 0; x < _mapWidth; x++)
                {
                    for (short y = 0; y < _mapHeight; y++)
                    {
                        _pathNodes[x, y] = new PathNode(_map.MapID, x, y)
                        {
                            IsBlocked = !_map.WithinGrid(x, y)
                        };
                    }
                }
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
            }
            List<Location> obstacles = new List<Location>(from creature in _client.GetWorldObjects().OfType<Creature>()
                                                          where (creature.Type == CreatureType.Aisling || creature.Type == CreatureType.Merchant || creature.Type == CreatureType.Normal) && creature != _client.Player
                                                          select creature.Location);
            if (_specialLocations.TryGetValue(_map.MapID, out Location[] specialObstacles))
            {
                foreach (Location location1 in specialObstacles)
                {
                    if (!obstacles.Contains(location1))
                    {
                        obstacles.Add(location1);
                    }
                }
            }
            if (includeAdditionalObstacles)
            {
                foreach (Location location2 in _client.GetObstacleLocations(location))
                {
                    if (!obstacles.Contains(location2))
                    {
                        obstacles.Add(location2);
                    }
                }
            }
            foreach (Creature creature in _client.GetCreaturesInRange(12, CONSTANTS.GREEN_BOROS.ToArray()))
            {
                foreach (Location loc in _client.GetCreatureCoverage(creature))
                {
                    if (!obstacles.Contains(loc))
                    {
                        obstacles.Add(loc);
                    }
                }
            }
            for (short x = 0; x < _mapWidth; x++)
            {
                for (short y = 0; y < _mapHeight; y++)
                {
                    PathNode currentNode = _pathNodes[x, y];
                    currentNode.IsBlocked = !_map.IsLocationWithinBounds(_pathNodes[x, y].Location);
                    currentNode.IsVisited = false;
                    currentNode.IsInOpenSet = false;
                    currentNode.GCost = 0f;
                    currentNode.FCost = 0f;
                }
            }
            foreach (Location obstacle in obstacles)
            {
                _pathNodes[obstacle.Point.X, obstacle.Point.Y].IsBlocked = true;
            }
            _blockedNodes.Clear();
        }

        private PathNode GetNextNode()
        {
            PathNode result = null;
            float minCost = float.MaxValue;
            foreach (PathNode node in _blockedNodes)
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
            InitializePathNodes(false, end, ignoreObstacles);
            if (Location.Equals(start, end))
            {
                return new Stack<Location>();
            }
            PathNode startNode = _pathNodes[start.Point.X, start.Point.Y];
            PathNode endNode = _pathNodes[end.Point.X, end.Point.Y];
            if (startNode.IsBlocked && endNode.IsBlocked && startNode != null && endNode != null)
            {
                startNode.IsInOpenSet = true;
                startNode.GCost = start.DistanceFrom(end);
                startNode.FCost = 0f;
                _blockedNodes.Add(startNode);
                while (true)
                {
                    if (_blockedNodes.Count > 0)
                    {
                        PathNode currentNode = GetNextNode();
                        if (currentNode == null)
                        {
                            break;
                        }
                        if (currentNode != endNode)
                        {
                            PathNode[] adjacentNodes = currentNode.Neighbors;
                            foreach (PathNode nextNode in adjacentNodes)
                            {
                                if (nextNode != null && !nextNode.IsVisited && (nextNode == endNode || nextNode.IsBlocked))
                                {
                                    float newGCost = currentNode.GCost + 1f;
                                    float newFCost = newGCost + nextNode.Location.DistanceFrom(end);
                                    if (!nextNode.IsInOpenSet)
                                    {
                                        nextNode.GCost = newGCost;
                                        nextNode.FCost = newFCost;
                                        nextNode.ParentNode = currentNode;
                                        nextNode.IsInOpenSet = true;
                                        _blockedNodes.Add(nextNode);
                                    }
                                    else if (nextNode.GCost > newGCost)
                                    {
                                        nextNode.GCost = newGCost;
                                        nextNode.FCost = newFCost;
                                        nextNode.ParentNode = currentNode;
                                    }
                                }
                            }
                            _blockedNodes.Remove(currentNode);
                            currentNode.IsVisited = true;
                            currentNode.IsInOpenSet = false;
                            continue;
                        }
                        Stack<Location> path = new Stack<Location>();
                        while (currentNode != startNode)
                        {
                            if (currentNode.Location.DistanceFrom(end) >= threshold)
                            {
                                path.Push(currentNode.Location);
                            }
                            currentNode = currentNode.ParentNode;
                        }
                        return path;
                    }
                    return new Stack<Location>();
                }
                _client.RequestRefresh();
                return new Stack<Location>();
            }
            return new Stack<Location>();
        }

        [CompilerGenerated]
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
        internal Location Location
        {
            get;
            private set;
        }

        internal bool IsBlocked
        {
            get;
            set;
        }

        internal PathNode[] Neighbors
        {
            get;
            private set;
        }

        internal PathNode ParentNode
        {
            get;
            set;
        }

        internal bool IsVisited
        {
            get;
            set;
        }

        internal bool IsInOpenSet
        {
            get;
            set;
        }

        internal float GCost
        {
            get;
            set;
        }

        internal float FCost
        {
            get;
            set;
        }

        internal PathNode(short mapID, short x, short y)
        {
            Location = new Location(mapID, x, y);
            Neighbors = new PathNode[4];
        }
    }
}
