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
using System.Drawing;
using Point = Talos.Structs.Point;
using Rectangle = Talos.Structs.Rectangle;
using Size = Talos.Structs.Size;

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
    public static Dictionary<short, List<Location>> SacredForest5 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest6 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest8 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest9 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest10 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest13 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest14 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest15 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest16 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest17 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest18 { get; set; }
    public static Dictionary<short, List<Location>> SacredForest19 { get; set; }
    public static Dictionary<short, List<Location>> MountGiragan19 { get; set; }
    public static Dictionary<short, List<Location>> MountGiragan20 { get; set; }
    public static Dictionary<short, List<Location>> MountGiragan22 { get; set; }
    public static Dictionary<short, List<Location>> MountGiragan23 { get; set; }

    public static Dictionary<short, Location[]> BlackList { get; set; } = new Dictionary<short, Location[]>();
    public static object PathLock { get; set; }

    internal Pathfinder(Client client)
    {
        _client = client;
        _map = client.Map;
        _mapWidth = _map.Width;
        _mapHeight = _map.Height;
        InitializePathNodes(true);
        InitializeBlockLists();
    }

    internal void InitializePathNodes(bool full, Location location = default(Location), bool avoidWarps = true)
    {
        _isLockstepWalking = false;
        _followPlayerName = _client.ClientTab.followText.Text;

        List<Location> obstacles = new List<Location>();

        // Collect blacklisted tiles immediately
        if (Pathfinder.BlackList.TryGetValue(_map.MapID, out Location[] blacklistedTiles))
        {
            obstacles.AddRange(blacklistedTiles);
            Console.WriteLine($"Loaded {blacklistedTiles.Length} blacklisted tiles for MapID {_map.MapID}");
        }
        else
        {
            Console.WriteLine($"No blacklist found for MapID {_map.MapID}");
        }

        // Collect obstacles from creatures
        obstacles.AddRange(
            _client.GetNearbyObjects()
            .OfType<Creature>()
            .Where(ShouldIncludeInObstacles)
            .Select(creature => creature.Location)
        );

        // Handle other clients' positions
        foreach (Client otherClient in _client.Server.Clients.Where(cli => cli != _client && cli.Map.MapID == _client.Map.MapID))
        {
            if (!(_isLockstepWalking && otherClient.Player.Name.Equals(_followPlayerName, StringComparison.OrdinalIgnoreCase)))
            {
                obstacles.AddRange(FindSimplePath(otherClient.ClientLocation.Point, otherClient.ServerLocation.Point).Take(3).Select(p => new Location(_map.MapID, p.X, p.Y)));
            }
        }

        // Collect warp points if avoiding warps
        if (avoidWarps)
        {
            obstacles.AddRange(_client.GetWarpPoints(location));
        }

        // Initialize path nodes if `full` is requested
        if (full)
        {
            _pathNodes = new PathNode[_mapWidth, _mapHeight];

            for (short x = 0; x < _mapWidth; x++)
            {
                for (short y = 0; y < _mapHeight; y++)
                {
                    _pathNodes[x, y] = new PathNode(_map.MapID, x, y)
                    {
                        Walkable = !_map.IsWall(x, y) || _map.Exits.ContainsKey(new Point(x, y))
                    };
                }
            }

            // Assign neighbors
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

        // Mark all collected obstacles as unwalkable
        foreach (Location loc in obstacles)
        {
            if (loc.X >= 0 && loc.X < _mapWidth && loc.Y >= 0 && loc.Y < _mapHeight)
            {
                _pathNodes[loc.X, loc.Y].Walkable = false;
                //Console.WriteLine($"Setting ({loc.X}, {loc.Y}) as UNWALKABLE (Obstacle)");
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
            Client client = _client.Server.GetClient(c.Name);
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
            Direction direction = currentPoint.GetDirection(endPoint);
            currentPoint = currentPoint.Offsetter(direction);

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

    public static void InitializeBlockLists()
    {
        SacredForest5 = BuildLocationList(
            new short[] { 11505, 11605, 11705 },
            (0, 4), (1, 4), (2, 4), (1, 3), (1, 2), (0, 2),
            (58, 54), (57, 55), (58, 55), (59, 55), (59, 56),
            (58, 56), (57, 56), (57, 57), (58, 57), (59, 57)
        );

        SacredForest6 = BuildLocationList(
            new short[] { 11506, 11606, 11706 },
            (49, 22), (49, 21), (50, 21), (27, 1), (26, 0),
            (2, 6), (18, 24), (18, 25), (18, 26), (17, 26),
            (19, 26), (20, 26), (20, 25), (20, 24), (19, 24),
            (19, 25), (21, 26), (21, 25), (21, 24), (17, 24),
            (17, 23)
        );

        SacredForest8 = BuildLocationList(
            new short[] { 11508, 11608, 11708 },
            (15, 27), (15, 26), (15, 25), (16, 27), (17, 25),
            (17, 23), (17, 22), (17, 21), (19, 15), (38, 46),
            (38, 47), (38, 48), (37, 49), (37, 50)
        );

        SacredForest9 = BuildLocationList(
            new short[] { 11509, 11609, 11709 },
            (34, 32), (34, 31), (34, 33), (35, 34), (35, 35),
            (33, 36), (33, 35), (32, 34), (31, 33), (24, 30),
            (24, 28), (23, 28), (22, 27), (20, 28), (20, 27),
            (20, 26), (20, 25), (19, 25), (16, 25), (16, 26),
            (15, 26), (14, 25), (15, 24), (14, 24), (13, 24),
            (13, 23), (13, 26), (13, 25), (8, 23), (5, 22),
            (6, 22), (7, 22), (4, 27), (4, 26), (4, 25),
            (12, 39), (13, 39), (27, 29), (26, 29), (25, 27),
            (24, 26), (23, 26), (22, 25), (21, 24), (20, 23),
            (19, 22)
        );
        
        SacredForest10 = BuildLocationList(
            new short[] { 11510, 11610, 11710 },
            (52, 58), (52, 57), (53, 57), (52, 56), (41, 47),
            (40, 47), (40, 46), (39, 46), (40, 45), (36, 43),
            (36, 44), (35, 44), (34, 44), (33, 44), (32, 44),
            (31, 44), (30, 44), (30, 45), (27, 42), (28, 43),
            (19, 45), (19, 44), (18, 44), (1, 32), (1, 33),
            (2, 33), (2, 34), (3, 34), (3, 33), (3, 4),
            (3, 5), (2, 5), (1, 5), (1, 4), (1, 3),
            (2, 3), (4, 4), (9, 2), (10, 2), (11, 2),
            (36, 5), (36, 4), (35, 4), (37, 4), (37, 5),
            (47, 5), (48, 5), (49, 6), (57, 20), (57, 19),
            (57, 18), (58, 18), (58, 19), (58, 20), (58, 21),
            (57, 22), (58, 26), (58, 25), (58, 24), (58, 23),
            (57, 25), (57, 26)
        );

        SacredForest13 = BuildLocationList(
            new short[] { 11516, 11616, 11716 },
            (19, 42), (18, 42), (17, 42), (19, 41), (14, 37),
            (15, 37), (16, 37), (14, 36), (14, 35), (14, 34),
            (15, 34), (12, 30), (13, 30), (4, 13), (5, 13),
            (6, 13), (7, 14), (8, 15), (9, 15), (10, 15),
            (11, 15), (12, 15), (13, 15), (15, 16), (14, 16),
            (16, 16), (17, 17), (18, 19), (19, 19), (19, 18),
            (20, 19), (23, 20), (23, 19), (24, 21), (25, 21),
            (25, 20), (26, 21), (27, 21), (27, 22), (28, 22),
            (28, 23), (30, 23), (41, 30), (41, 31), (41, 32),
            (41, 33), (40, 33), (29, 23), (29, 19), (28, 19),
            (28, 20), (27, 19), (27, 18), (26, 18), (25, 18),
            (21, 17), (22, 17), (22, 16), (21, 16), (20, 16),
            (16, 14), (17, 14), (17, 15), (15, 13), (15, 12),
            (14, 12), (13, 12), (10, 11), (11, 11), (12, 12),
            (12, 13), (12, 14), (11, 13), (10, 10), (9, 10),
            (8, 10), (8, 11), (7, 9), (6, 9), (6, 10),
            (6, 11), (5, 10), (4, 10), (4, 9)
        );

        SacredForest14 = BuildLocationList(
            new short[] { 11514, 11614, 11714 },
            (58, 30), (58, 29), (59, 29), (59, 28), (59, 27),
            (58, 28), (58, 35), (58, 36), (59, 36), (59, 37),
            (58, 37), (53, 7), (52, 7), (51, 7), (51, 6),
            (48, 7), (49, 7), (47, 6), (46, 6), (46, 5),
            (46, 4), (45, 4), (12, 8), (11, 8), (10, 8),
            (10, 7), (9, 8), (8, 6), (8, 5), (9, 5),
            (9, 6), (0, 4), (1, 4), (1, 3), (1, 2),
            (0, 2), (2, 4), (0, 5), (1, 5), (2, 5),
            (0, 13), (0, 14), (0, 15), (1, 13), (0, 6),
            (1, 7), (1, 6)
        );

        SacredForest15 = BuildLocationList(
            new short[] { 11515, 11615, 11715 },
            (56, 5), (56, 6), (56, 7), (55, 6), (58, 18),
            (58, 19), (58, 20), (58, 21), (57, 22), (56, 26),
            (57, 26), (57, 27), (57, 25), (58, 25), (58, 26),
            (58, 24), (58, 23), (52, 56), (52, 57), (52, 58),
            (53, 57), (54, 57), (55, 57), (56, 57), (41, 47),
            (40, 47), (40, 46), (39, 46), (40, 45), (40, 44),
            (30, 45), (30, 44), (27, 42), (28, 42), (28, 43),
            (22, 42), (20, 44), (20, 43), (20, 42), (21, 42),
            (21, 43), (21, 44), (18, 44), (18, 45), (19, 46),
            (18, 43), (18, 42), (1, 33), (1, 32), (2, 34),
            (2, 33), (3, 33), (3, 34), (1, 6), (1, 5),
            (1, 3), (1, 4), (2, 3), (2, 7), (2, 6),
            (2, 5), (3, 5), (3, 4), (5, 3), (5, 2),
            (6, 2), (7, 2), (8, 2), (9, 2), (10, 2),
            (11, 2), (10, 3), (10, 4), (13, 4), (35, 4),
            (36, 4), (36, 5), (37, 5), (37, 4), (38, 5)
        );

        SacredForest16 = BuildLocationList(
            new short[] { 11516, 11616, 11716 },
            (54, 59), (53, 59), (54, 58), (54, 57), (55, 57),
            (58, 6), (58, 5), (59, 6), (59, 7), (53, 4),
            (52, 4), (52, 5), (43, 4), (44, 4), (42, 4),
            (41, 4), (40, 6), (40, 5), (26, 9), (27, 9),
            (10, 6), (10, 7), (11, 7), (11, 8), (6, 8),
            (7, 8), (7, 7), (8, 10), (8, 9), (8, 8),
            (8, 7), (9, 7), (9, 8), (10, 8), (11, 5),
            (12, 5), (12, 6), (13, 6), (13, 5), (10, 15),
            (11, 15), (12, 15), (7, 24), (7, 25), (7, 26),
            (6, 25), (8, 24), (9, 24), (8, 23), (9, 31),
            (8, 31), (8, 32), (8, 33), (8, 34), (9, 34),
            (9, 33), (14, 41), (44, 56), (45, 56), (46, 56),
            (35, 59), (36, 59), (37, 59)
        );

        SacredForest17 = BuildLocationList(
            new short[] { 11517, 11617, 11717 },
            (39, 56), (39, 57), (40, 57), (40, 56), (41, 56),
            (41, 55), (41, 54), (42, 55), (42, 56), (41, 57),
            (41, 58), (42, 58), (43, 58), (43, 59), (42, 59),
            (39, 58), (38, 58), (38, 59), (39, 59), (40, 59),
            (41, 59), (40, 55), (40, 54), (40, 53), (32, 22),
            (32, 21), (32, 20), (22, 28), (22, 27), (23, 27),
            (23, 28), (22, 26), (21, 26), (20, 26), (20, 25),
            (19, 25), (20, 24), (19, 24), (21, 23), (21, 24),
            (6, 28), (5, 28), (4, 28), (4, 29), (4, 30),
            (3, 30), (3, 31), (3, 32), (24, 4), (24, 3)
        );

        SacredForest18 = BuildLocationList(
            new short[] { 11518, 11618, 11718 },
            (2, 23), (59, 12), (58, 12), (59, 11), (58, 13),
            (58, 18), (58, 19), (59, 19), (59, 20), (58, 23),
            (59, 23), (59, 6), (58, 6), (58, 5), (39, 4),
            (39, 5), (40, 3), (41, 3), (41, 4), (32, 8),
            (31, 8), (30, 8), (29, 8), (29, 9), (29, 10),
            (33, 9), (33, 8), (24, 8), (24, 9), (24, 10),
            (18, 8), (18, 7), (18, 6), (18, 5), (17, 5),
            (16, 5), (22, 5), (21, 5), (20, 5), (19, 5),
            (19, 6), (7, 7), (8, 7), (9, 7), (10, 7),
            (10, 6), (11, 5), (12, 5), (13, 5), (14, 5),
            (13, 6), (13, 7), (6, 10), (6, 11), (13, 37),
            (13, 38), (14, 38), (14, 39), (14, 37), (19, 46),
            (20, 46), (18, 46), (18, 47), (18, 48), (18, 49),
            (19, 49), (20, 49), (22, 55), (22, 56), (23, 56),
            (23, 57), (23, 58), (23, 59), (24, 56), (41, 44),
            (41, 43), (42, 43), (43, 43), (43, 42), (43, 41),
            (44, 41), (46, 43), (45, 43), (59, 25), (59, 26),
            (59, 27), (58, 27)
        );

        SacredForest19 = BuildLocationList(
            new short[] { 11519, 11619, 11719 },
            (55, 24), (55, 23), (55, 22), (54, 22), (53, 22),
            (52, 22), (52, 23), (52, 24), (4, 1), (4, 0),
            (5, 0), (5, 16), (5, 17), (5, 18), (5, 19),
            (5, 20), (5, 21), (5, 22), (5, 23), (5, 24),
            (5, 25), (5, 26), (5, 27), (6, 27), (6, 26),
            (6, 25), (6, 24), (6, 23), (6, 22), (6, 21),
            (6, 20), (6, 19), (6, 18), (6, 17), (6, 16),
            (6, 15), (17, 23), (18, 23), (18, 24), (18, 25),
            (18, 26), (17, 26), (16, 26), (19, 26), (19, 25),
            (19, 24), (20, 25), (20, 26), (21, 26), (21, 25),
            (21, 24), (20, 24)
        );

        MountGiragan19 = BuildLocationList(
            new short[] { 2105 },
            (13, 19), (15, 4), (16, 4), (17, 4), (15, 2),
            (16, 2), (15, 1), (2, 0), (1, 0)
        );

        MountGiragan20 = BuildLocationList(
            new short[] { 2106 },
            (18, 4), (19, 4), (17, 4), (17, 3), (18, 3),
            (18, 4), (14, 0), (13, 0), (16, 2), (18, 2),
            (12, 1), (11, 1), (10, 1), (4, 2), (4, 1),
            (5, 1), (6, 1), (3, 2), (2, 2), (2, 1)
        );

        MountGiragan22 = BuildLocationList(
            new short[] { 2108 },
            (2, 2), (2, 1), (1, 1), (1, 0), (2, 0),
            (3, 2), (4, 2), (4, 1), (9, 0), (11, 1),
            (10, 1), (10, 2), (16, 2), (18, 2), (18, 4),
            (19, 4), (13, 19), (13, 18)
        );

        MountGiragan23 = BuildLocationList(
            new short[] { 2109 },
            (2, 2), (2, 1), (1, 1), (2, 0), (4, 2),
            (4, 1), (5, 1), (8, 2), (9, 2), (10, 2),
            (16, 2), (18, 2), (18, 4), (19, 4), (13, 19),
            (13, 18), (16, 19)
        );

        var dictionary = new Dictionary<short, Location[]>();

        // Example of Rectangle -> Points logic:
        // Collect points from two rectangles and combine them.
        Rectangle rectangle1 = new Rectangle(new Point(3, 9), new Size(3, 3));
        List<Location> points1 = rectangle1.Points;

        Rectangle rectangle2 = new Rectangle(new Point(9, 1), new Size(3, 3));
        List<Location> points2 = rectangle2.Points;

        dictionary.Add(
            3058,
            points1.Concat(points2).ToArray()
        );



        var allBlockLists = new List<Dictionary<short, List<Location>>>()
        {
            SacredForest5, SacredForest6, SacredForest8, SacredForest9, SacredForest10,
            SacredForest13, SacredForest14, SacredForest15, SacredForest16, SacredForest17,
            SacredForest18, SacredForest19, MountGiragan19, MountGiragan20,
            MountGiragan22, MountGiragan23
        };

        foreach (var blockList in allBlockLists)
        {
            if (blockList != null)
            {
                foreach (var kvp in blockList)
                {
                    //Console.WriteLine($"Adding blacklist for MapID {kvp.Key}, {kvp.Value.Count} locations");
                    dictionary[kvp.Key] = kvp.Value.ToArray();
                }
            }
        }

        // Seasonal Event Blocklist for Mileth Fountain (Make a Wish)
        DateTime now = DateTime.UtcNow;
        if ((now.Month == 2 && now.Day >= 11) || (now.Month == 3 && now.Day <= 4))
        {
            Console.WriteLine("Seasonal Event Active: Adding 'Make a Wish' blocklist.");

            Rectangle fountainBlock1 = new Rectangle(new Point(83, 31), new Size(13, 12));
            dictionary[500] = fountainBlock1.Points.Select(p => new Location(500, p.X, p.Y)).ToArray();
        }

        // Pravat Cave Fountain Blocklist
        Rectangle fountainBlock2 = new Rectangle(new Point(40, 13), new Size(5, 5));
        dictionary[3052] = fountainBlock2.Points.Select(p => new Location(3052, p.X, p.Y)).ToArray();

        // Assign the dictionary to Pathfinder.BlackList
        BlackList = dictionary;

        // Finally, your lock object
        PathLock = new object();
    }

    private static Dictionary<short, List<Location>> BuildLocationList(short[] mapIDs, params (short x, short y)[] coords)
    {
        var locationsByMap = new Dictionary<short, List<Location>>();

        //Console.WriteLine($"Building blacklist for maps: {string.Join(", ", mapIDs)}");
        foreach (var mapID in mapIDs)
        {
            //Console.WriteLine($"Adding locations for MapID {mapID}");
            var list = new List<Location>(coords.Length);
            foreach (var (x, y) in coords)
                list.Add(new Location(mapID, x, y));

            locationsByMap[mapID] = list;
        }

        return locationsByMap;
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

