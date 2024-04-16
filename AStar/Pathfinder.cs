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

internal class Pathfinder
{
    private PathNode[,] _pathNodes;
    private readonly byte _mapWidth;
    private readonly byte _mapHeight;
    private PriorityQueue<PathNode> _openNodes;
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
        _openNodes = new PriorityQueue<PathNode>((int)QueuePriorityEnum.High + 1);
        InitializePathNodes(true);
    }

    internal void InitializePathNodes(bool initializeBlockedNodes, Location location = default(Location), bool includeExits = true, bool includeAdditionalObstacles = true)
    {
        if (initializeBlockedNodes)
        {
            _pathNodes = new PathNode[_mapWidth, _mapHeight];

            // Initialize all path nodes with wall data
            for (short x = 0; x < _mapWidth; x++)
            {
                for (short y = 0; y < _mapHeight; y++)
                {
                    bool isWall = _map.IsTileWall(x, y);
                    _pathNodes[x, y] = new PathNode(_map.MapID, x, y)
                    {
                        IsOpen = !isWall
                    };
                    //Console.WriteLine($"Node at ({x},{y}) initialized as " + (isWall ? "Wall" : "Open"));
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

        // Handling dynamic obstacles
        List<Location> obstacles = (from creature in _client.GetWorldObjects().OfType<Creature>()
                                    where (creature.Type == CreatureType.Aisling || creature.Type == CreatureType.Merchant || creature.Type == CreatureType.Normal) && creature != _client.Player
                                    select creature.Location).ToList();


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
            obstacles.AddRange(_client.GetExitLocations(location));
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

        // Reevaluating each node after obstacle collection
        for (short x = 0; x < _mapWidth; x++)
        {
            for (short y = 0; y < _mapHeight; y++)
            {
                PathNode currentNode = _pathNodes[x, y];
                currentNode.IsOpen = !_map.IsTileWall(x, y) && !obstacles.Contains(new Location(_map.MapID, x, y));
                currentNode.IsVisited = false;
                currentNode.IsInOpenSet = false;
                currentNode.GCost = 0;
                currentNode.FCost = 0;
            }
        }
    }

    private float CalculateHeuristic(Location a, Location b)
    {
        // Manhattan distance
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    internal Stack<Location> FindPath(Location start, Location end, bool avoidObstacles = true)
    {
        InitializePathNodes(false, end, avoidObstacles);
        Console.WriteLine($"Starting pathfinding from {start} to {end}.");

        if (!_pathNodes[start.X, start.Y].IsOpen || !_pathNodes[end.X, end.Y].IsOpen)
        {
            //Console.WriteLine("Start or end location is blocked.");
            return new Stack<Location>();
        }

        PathNode startNode = _pathNodes[start.X, start.Y];
        PathNode endNode = _pathNodes[end.X, end.Y];
        startNode.GCost = 0;
        startNode.FCost = CalculateHeuristic(startNode.Location, endNode.Location);
        _openNodes.Enqueue(QueuePriorityEnum.High, startNode);

        while (_openNodes.Count > 0)
        {
            PathNode currentNode = _openNodes.Dequeue();
            //Console.WriteLine($"Processing node at {currentNode.Location}, FCost: {currentNode.FCost}");

            if (currentNode == endNode)
            {
                //Console.WriteLine("Reached destination, constructing path.");
                return ConstructPath(currentNode);
            }

            //Console.WriteLine($"Processing node at {currentNode.Location}, Neighbors: {currentNode.Neighbors.Length}");
            foreach (var neighbor in currentNode.Neighbors)
            {
                if (neighbor == null)
                {
                    //Console.WriteLine($"Skipped neighbor at {neighbor?.Location} - null.");
                    continue;
                }

                if (!neighbor.IsOpen)
                {
                    //Console.WriteLine($"Skipped neighbor at {neighbor?.Location} - not open.");
                    continue;
                }


                if (neighbor.IsVisited)
                {
                    //Console.WriteLine($"Skipped neighbor at {neighbor.Location} - Already visited.");
                    continue;
                }

                //Console.WriteLine($"Neighbor at {neighbor.Location} is open and will be evaluated.");

                float tentativeGCost = currentNode.GCost + 1;  // Adjust cost calculation if necessary
                if (!neighbor.IsInOpenSet || tentativeGCost < neighbor.GCost)
                {
                    neighbor.ParentNode = currentNode;
                    neighbor.GCost = tentativeGCost;
                    neighbor.FCost = tentativeGCost + CalculateHeuristic(neighbor.Location, end);

                    if (!neighbor.IsInOpenSet)
                    {
                        _openNodes.Enqueue(QueuePriorityEnum.High, neighbor);
                        neighbor.IsInOpenSet = true;
                        //Console.WriteLine($"Enqueued neighbor at {neighbor.Location} with GCost: {tentativeGCost}, FCost: {neighbor.FCost}");
                    }
                }
            }
            currentNode.IsVisited = true;
        }

        Console.WriteLine("Failed to find a path.");
        return new Stack<Location>(); // No path found
    }


    private Stack<Location> ConstructPath(PathNode node)
    {
        Stack<Location> path = new Stack<Location>();
        while (node != null)
        {
            Console.WriteLine($"Traceback - Current Node: {node.Location}, Parent Node: {node.ParentNode?.Location}");
            path.Push(node.Location);
            node = node.ParentNode;
        }
        return path;
    }
}

internal sealed class PathNode
{
    public Location Location { get; }
    public bool IsOpen { get; set; }
    public PathNode[] Neighbors { get; set; }
    public PathNode ParentNode { get; set; }
    public bool IsVisited { get; set; }
    public bool IsInOpenSet { get; set; }
    public float GCost { get; set; } = float.MaxValue; // Set high default cost
    public float FCost { get; set; } = float.MaxValue;

    public PathNode(short mapID, short x, short y)
    {
        Location = new Location(mapID, x, y);
        Neighbors = new PathNode[4];
    }
}

