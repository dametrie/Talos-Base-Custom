using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Maps;
using Talos.Structs;

namespace Talos.AStar
{
    internal class Pathfinder2
    {
        private readonly int Height;
        private readonly int Width;
        private readonly int[] NeighborIndexes;
        public readonly PathNode2[,] PathNodes;
        private readonly Dictionary<PathNode2, int> PriortyQueue;


        internal Pathfinder2(Map map, HashSet<Location>? blacklistedPoints = null)
        {
            Height = map.Height;
            Width = map.Width;
            PathNodes = new PathNode2[Height, Width];
            PriortyQueue = new Dictionary<PathNode2, int>();
            NeighborIndexes = Enumerable.Range(0, 4).ToArray();

            //create nodes
            //make sure indices are in range
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    PathNodes[y, x] = new PathNode2(map.MapID, (short)x, (short)y);
                }
            }

            //assign walls
            foreach (var tile in map.Tiles)
                if (tile.Value.IsWall)
                    PathNodes[tile.Key.Y, tile.Key.X].IsWall = true;

            //assign blacklisted points
            if(blacklistedPoints is not null)
            {
                foreach (var point in blacklistedPoints)
                    PathNodes[point.Y, point.X].IsBlackListed = true;
            }


           //assign neighbors
           //make sure indices are in range
           for (int y = 0; y < Height; y++)
           {
                for (int x = 0; x < Width; x++)
                {
                     if (x > 0)
                        PathNodes[y, x].Neighbords[3] = PathNodes[y, x - 1];
                     if (x < Width - 1)
                        PathNodes[y, x].Neighbords[1] = PathNodes[y, x + 1];
                     if (y > 0)
                        PathNodes[y, x].Neighbords[0] = PathNodes[y - 1, x];
                     if (y < Height - 1)
                        PathNodes[y, x].Neighbords[2] = PathNodes[y + 1, x];
                }
           }

        }

        internal Stack<Location> FindPath(Location start, Location end, Pathfinder2Options options)
        {
            //if the end is blacklisted, we can't reach it
            //return empty path
            if (PathNodes[end.Y, end.X].IsBlackListed)
                return new Stack<Location>();

            //if we're at the end already
            //return empty path
            if (start.Equals(end))
                return new Stack<Location>();

            //if we're are one tile away
            //return a path with the end location
            if (Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y) == 1)
                return new Stack<Location>(new[] { end });

            //reset queue
            PriortyQueue.Clear();

            //prepare the nodes
            foreach (var node in PathNodes)
            {
                node.Reset();

                if (options.MaxRadius.HasValue && (start.DistanceFrom(node) > options.MaxRadius.Value))
                    node.Closed = true;

                //check if options.BlockedList contains the node
                if (options.BlockedPoints != null && options.BlockedPoints.Contains(new Location(node.MapID, new Structs.Point(node.X, node.Y))))
                    node.IsBlocked = true;
            }

            var path = InnerFindPath(start, end, options.IgnoreWalls);

            return path;
        }

        private Stack<Location> InnerFindPath(Location start, Location end, bool ignoreWalls)
        {

            var startNode = PathNodes[start.Y, start.X];
            var endNode = PathNodes[end.Y, end.X];

            startNode.Open = true;
            PriortyQueue.Add(startNode, 0);

            while (PriortyQueue.Count > 0)
            {
                var currentNode = PriortyQueue.OrderBy(x => x.Value).First().Key;
                PriortyQueue.Remove(currentNode);

                //Console.WriteLine($"Exploring node: ({currentNode.X}, {currentNode.Y})");

                // Don't consider closed nodes
                if (currentNode.Closed)
                {
                    //Console.WriteLine("Node is closed. Skipping.");
                    continue;
                }

                // For each undiscovered walkable neighbor, set its parent and add it to the queue
                for (var i = 0; i < 4; i++)
                {
                    // Get the neighbor
                    var neighbor = currentNode.Neighbords[NeighborIndexes[i]];

                    // Don't consider closed nodes or nodes that are already open
                    if (neighbor == null || neighbor.Closed || neighbor.Open)
                    {
                        //Console.WriteLine($"Skipping neighbor: ({neighbor?.X}, {neighbor?.Y})");
                        continue;
                    }

                    //Console.WriteLine($"Considering neighbor: ({neighbor.X}, {neighbor.Y})");

                    // If we locate the end, set parent and return the path
                    if (neighbor.Equals(endNode))
                    {
                        neighbor.Parent = currentNode;
                        //Console.WriteLine("Found end node. Constructing path.");
                        return new Stack<Location>(GetParentChain(endNode));
                    }

                    // Don't add walls unless we're ignoring them
                    // Don't add blocked nodes
                    if (!neighbor.IsWalkable(ignoreWalls))
                    {
                        //Console.WriteLine($"Neighbor is not walkable: ({neighbor.X}, {neighbor.Y})");
                        continue;
                    }

                    // Add neighbor to the priority queue
                    neighbor.Parent = currentNode;
                    PriortyQueue.Add(neighbor, neighbor.DistanceFrom(startNode) + neighbor.DistanceFrom(endNode));
                    neighbor.Open = true;

                    //Console.WriteLine($"Added neighbor to the queue: ({neighbor.X}, {neighbor.Y})");
                }

                // Mark current node as closed
                currentNode.Closed = true;
                currentNode.Open = false;

                //Console.WriteLine($"Closed node: ({currentNode.X}, {currentNode.Y})");
            }

            // If no path is found, return an empty stack
            //Console.WriteLine("No path found.");
            return new Stack<Location>(GetParentChain(endNode));
        }


        private IEnumerable<Location> GetParentChain(PathNode2 pathNode)
        {
            var chain = new List<Location>();
            while (pathNode != null)
            {
                chain.Add(new Location(pathNode.MapID, new Structs.Point(pathNode.X, pathNode.Y)));
                pathNode = pathNode.Parent;
            }
            //chain.Reverse(); // Reverse the list to get the path from start to end
            return chain;
        }
    }

    internal sealed class PathNode2
    {
        internal bool Closed { get; set; } = true;
        internal bool IsBlackListed { get; set; }
        internal bool IsBlocked { get; set; }
        internal bool IsWall { get; set; }
        internal bool Open { get; set; }
        internal PathNode2? Parent { get; set; }
        internal PathNode2?[] Neighbords { get; } = new PathNode2?[4];
        internal short X { get; set; }
        internal short Y { get; set; }

        internal short MapID { get; }

        internal PathNode2(short mapID, short x, short y)
        {   
            MapID = mapID;
            X = x;
            Y = y;
        }

        internal bool IsWalkable(bool ignoreWalls) => !IsBlackListed && !IsBlocked && (ignoreWalls || !IsWall);

        internal void Reset()
        {
            Closed = IsBlackListed;
            Open = false;
            IsBlocked = false;
            Parent = null;
        }

        internal int DistanceFrom(PathNode2 startNode)
        {
            //get the distance from this node to another node
            return Math.Abs(X - startNode.X) + Math.Abs(Y - startNode.Y);
        }
    }

    internal sealed class Pathfinder2Options
    {
        internal bool IgnoreWalls { get; set; }
        internal HashSet<Location>? BlockedPoints { get; set; }

        public int? MaxRadius { get; set; }
    }
}
