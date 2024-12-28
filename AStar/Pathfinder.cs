using System.Collections.Generic;
using System;
using Talos.AStar;
using Talos.Base;
using Talos.Maps;
using Talos.Structs;
using System.Linq;
using Talos.Enumerations;
using Talos.Objects;
using Talos.Definitions;
using System.Threading;

internal class Pathfinder
{
    private PathNode[,] _pathNodes;
    private readonly byte _mapWidth;
    private readonly byte _mapHeight;
    private readonly object Sync = new object();
    private PriorityQueue<PathNode> _openNodes;
    private Map _map;
    private Client _client;
    private bool _isLockstepWalking;
    private string _followPlayerName;

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
        InitializePathNodes(true);
    }

    internal void InitializePathNodes(bool full, Location location = default(Location), bool avoidWarps = true)
    {
        // Initialize lockstep walking variables
        _isLockstepWalking = false;
        _followPlayerName = _client.ClientTab.followText.Text;

        // Collect obstacles
        List<Location> obstacles = (
            from creature in _client.GetNearbyObjects().OfType<Creature>()
            where ShouldIncludeInObstacles(creature)
            select creature.Location).ToList();

        // Handle other clients on the same map
        foreach (Client otherClient in _client._server.Clients.Where(cli => cli != _client && cli._map.MapID == _client._map.MapID))
        {
            if (!(_isLockstepWalking && otherClient.Player.Name.Equals(_followPlayerName, StringComparison.OrdinalIgnoreCase)))
            {
                // Add up to 3 points from their path to the obstacles
                var pathPoints = FindSimplePath(otherClient._clientLocation.Point, otherClient._serverLocation.Point).Take(3);
                foreach (Point point in pathPoints)
                {
                    Location loc = new Location(_map.MapID, point.X, point.Y);
                    if (!obstacles.Contains(loc))
                    {
                        obstacles.Add(loc);
                    }
                }
            }
        }

        if (full)
        {
            _pathNodes = new PathNode[_mapWidth, _mapHeight];

            // Initialize all path nodes with wall data
            for (short x = 0; x < _mapWidth; x++)
            {
                for (short y = 0; y < _mapHeight; y++)
                {
                    bool isWall = _map.IsWall(x, y);
                    Point currentPoint = new Point(x, y);
                    bool isExit = _map.Exits.ContainsKey(currentPoint);

                    _pathNodes[x, y] = new PathNode(_map.MapID, x, y)
                    {
                        Walkable = !isWall || isExit  // Nodes are open if not a wall or if it's an exit
                    };
                    //Console.WriteLine($"Node at ({x}, {y}) initialized as " + (_pathNodes[x, y].IsOpen ? "Open" : "Closed"));
                }
            }

            // Setting neighbors for each node
            for (short x = 0; x < _mapWidth; x++)
            {
                for (short y = 0; y < _mapHeight; y++)
                {
                    PathNode currentNode = _pathNodes[x, y];
                    if (y > 0) currentNode.Neighbors[0] = _pathNodes[x, y - 1]; // Up
                    if (x < _mapWidth - 1) currentNode.Neighbors[1] = _pathNodes[x + 1, y]; // Right
                    if (y < _mapHeight - 1) currentNode.Neighbors[2] = _pathNodes[x, y + 1]; // Down
                    if (x > 0) currentNode.Neighbors[3] = _pathNodes[x - 1, y]; // Left
                }
            }
        }



        if (avoidWarps)
        {
            foreach (Location location2 in _client.GetWarpPoints(location))
            {
                if (!obstacles.Contains(location2))
                {
                    obstacles.Add(location2);
                }
            }
        }
        else
        {
            //remove exits from obstacles
            obstacles.RemoveAll(loc => _map.Exits.ContainsKey(new Point(loc.X, loc.Y)));
        }

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



        //Console.WriteLine("Obstacles from creature coverage:");
        foreach (Creature creature in _client.GetAllNearbyMonsters(12, CONSTANTS.GREEN_BOROS.ToArray()))
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

        // Reevaluating each node after obstacle collection
        for (short x = 0; x < _mapWidth; x++)
        {
            for (short y = 0; y < _mapHeight; y++)
            {
                PathNode currentNode = _pathNodes[x, y];
                currentNode.Walkable = !_map.IsWall(x, y) || _map.Exits.ContainsKey(new Point(x, y));

                if (obstacles.Contains(new Location(_map.MapID, x, y)))
                {
                    currentNode.Walkable = false;
                }

                currentNode.IsClosed = false;
                currentNode.ParentNode = null;
                currentNode.IsOpen = false;
                currentNode.GCost = 0;
                currentNode.FCost = 0;
            }
        }
    }


    // Helper method to determine if a creature should be included in obstacles
    private bool ShouldIncludeInObstacles(Creature c)
    {
        // Exclude own player
        if (c == _client.Player)
            return false;

        // Include creatures of specified types
        bool isRelevantType = c.Type == CreatureType.Aisling || c.Type == CreatureType.Merchant || c.Type == CreatureType.Normal;
        if (!isRelevantType)
            return false;

        // Check if lockstep walking is enabled and the creature is the one we're following
        if (_client.ClientTab.lockstepCbox.Checked && c.Type == CreatureType.Aisling && c.Name.Equals(_followPlayerName, StringComparison.OrdinalIgnoreCase))
        {
            // Get the client instance of the creature
            Client client = _client._server.GetClient(c.Name);
            if (client != null && DateTime.UtcNow.Subtract(client.LastStep).TotalSeconds < 2.0)
            {
                _isLockstepWalking = true;
                return false; // Exclude from obstacles
            }
        }

        // Include in obstacles
        return true;
    }

    private float CalculateHeuristic(Location a, Location b)
    {
        // Manhattan distance
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private IEnumerable<Point> FindSimplePath(Point startPoint, Point endPoint)
    {
        Point currentPoint = startPoint;

        while (currentPoint != endPoint)
        {
            Direction direction = currentPoint.Relation(endPoint);
            currentPoint = currentPoint.TranslatePointByDirection(direction);

            yield return currentPoint;
        }
    }

    internal Stack<Location> FindPath(Location start, Location end, bool avoidWarps = true, short distance = 1)
    {
        if (Monitor.TryEnter(Sync, 150))
        {
            try
            {
                InitializePathNodes(false, end, avoidWarps);
                //Console.WriteLine($"Starting pathfinding from {start} to {end}.");

                //if (!_pathNodes[start.X, start.Y].Walkable)
                //{
                //    Console.WriteLine("Start location is blocked.");
                //    return new Stack<Location>();
                //}

                if (end.X < 0 || end.X >= _mapWidth || end.Y < 0 || end.Y >= _mapHeight)
                {
                    Console.WriteLine("End location is outside map boundaries.");
                    return new Stack<Location>();
                }

                PathNode startNode = _pathNodes[start.X, start.Y];
                PathNode endNode = _pathNodes[end.X, end.Y];
                startNode.GCost = 0;
                startNode.FCost = CalculateHeuristic(startNode.Location, endNode.Location);
                _openNodes = new PriorityQueue<PathNode>();
                _openNodes.Enqueue(startNode);
                startNode.IsOpen = true;

                while (_openNodes.Count > 0)
                {
                    PathNode currentNode = _openNodes.Dequeue();
                    //Console.WriteLine($"Processing node at {currentNode.Location}, FCost: {currentNode.FCost}");

                    if (currentNode == endNode)
                    {
                        //Console.WriteLine("Constructing path.");
                        return ConstructPath(currentNode);
                    }

                    currentNode.IsClosed = true;

                    //Console.WriteLine($"Processing node at {currentNode.Location}, Neighbors: {currentNode.Neighbors.Length}");
                    foreach (var neighbor in currentNode.Neighbors)
                    {
                        if (neighbor == null || neighbor.IsClosed)
                            continue;

                        if (!neighbor.Walkable && neighbor != endNode)
                            continue;

                        //Console.WriteLine($"Neighbor at {neighbor.Location} is walkable and will be evaluated.");

                        float tentativeGCost = currentNode.GCost + 1;

                        if (!neighbor.IsOpen || tentativeGCost < neighbor.GCost)
                        {
                            neighbor.ParentNode = currentNode;
                            neighbor.GCost = tentativeGCost;
                            neighbor.FCost = tentativeGCost + CalculateHeuristic(neighbor.Location, end);

                            if (!neighbor.IsOpen)
                            {
                                _openNodes.Enqueue(neighbor);
                                neighbor.IsOpen = true;
                                //Console.WriteLine($"Enqueued neighbor at {neighbor.Location} with GCost: {tentativeGCost}, FCost: {neighbor.FCost}");
                            }
                            else
                            {
                                _openNodes.UpdatePriority(neighbor);
                            }
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(Sync);
            }       
        }
        Console.WriteLine("Failed to find a path.");
        return new Stack<Location>(); // No path found
    }


    private Stack<Location> ConstructPath(PathNode node)
    {
        HashSet<Location> visitedLocations = new HashSet<Location>();
        Stack<Location> path = new Stack<Location>();

        while (node != null)
        {
            //// Check if we have already visited this node to detect cycles.
            //if (visitedLocations.Contains(node.Location))
            //{
            //    Console.WriteLine($"Cycle detected at location {node.Location}. Breaking out of loop.");
            //    break;
            //}

            //visitedLocations.Add(node.Location);
            path.Push(node.Location);
            node = node.ParentNode;
        }

        return path;
    }
}

internal sealed class PathNode : IComparable<PathNode>
{
    public Location Location { get; }
    public bool Walkable { get; set; }
    public PathNode[] Neighbors { get; set; }
    public PathNode ParentNode { get; set; }
    public bool IsClosed { get; set; }
    public bool IsOpen { get; set; }
    public float GCost { get; set; } = float.MaxValue; // Set high default cost
    public float FCost { get; set; } = float.MaxValue;

    public PathNode(short mapID, short x, short y)
    {
        Location = new Location(mapID, x, y);
        Neighbors = new PathNode[4];
    }

    public int CompareTo(PathNode other)
    {
        if (other == null)
            return -1;
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0)
        {
            // Break ties using GCost
            compare = GCost.CompareTo(other.GCost);
        }
        return compare;
    }
}

