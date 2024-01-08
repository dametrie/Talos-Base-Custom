using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Enumerations;
using Talos.Maps;
using Talos.Structs;

namespace Talos.AStar
{
    internal static class Pathfinding
    {
        // ... other methods ...

        public static (List<Direction> pathDirections, short targetMapID) FindPath(Map map, Location start, Location goal)
        {
            // A* pathfinding algorithm implementation
            List<Point> pathPoints = AStar(map, start.Point, goal.Point);

            // Convert the points to a list of directions
            List<Direction> pathDirections = new List<Direction>();
            for (int i = 1; i < pathPoints.Count; i++)
            {
                Direction dir = GetDirectionFromPoints(pathPoints[i - 1], pathPoints[i]);
                pathDirections.Add(dir);
            }

            return (pathDirections, goal.MapID);
        }

        internal static List<Point> AStar(Map map, Point start, Point goal)
        {
            HashSet<Point> openSet = new HashSet<Point> { start };
            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

            Dictionary<Point, int> gScore = new Dictionary<Point, int> { { start, 0 } };
            Dictionary<Point, int> fScore = new Dictionary<Point, int> { { start, HeuristicCostEstimate(start, goal) } };

            while (openSet.Count > 0)
            {
                Point current = GetPointWithLowestFScore(openSet, fScore);

                if (current.Equals(goal))
                    return ReconstructPath(cameFrom, goal);

                openSet.Remove(current);

                foreach (Point neighbor in GetNeighbors(map, current))
                {
                    int tentativeGScore = gScore[current] + 1; // Assuming a cost of 1 to move to a neighbor

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + HeuristicCostEstimate(neighbor, goal);

                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                    }
                }
            }

            return null; // No path found
        }

        private static Point GetPointWithLowestFScore(HashSet<Point> openSet, Dictionary<Point, int> fScore)
        {
            int minFScore = int.MaxValue;
            Point minPoint = default(Point);

            foreach (Point point in openSet)
            {
                if (fScore.ContainsKey(point) && fScore[point] < minFScore)
                {
                    minFScore = fScore[point];
                    minPoint = point;
                }
            }

            return minPoint;
        }

        private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
        {
            List<Point> totalPath = new List<Point> { current };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }

            return totalPath;
        }

        private static int HeuristicCostEstimate(Point start, Point goal)
        {
            // A simple heuristic: Manhattan distance
            return Math.Abs(start.X - goal.X) + Math.Abs(start.Y - goal.Y);
        }

        private static List<Point> GetNeighbors(Map map, Point point)
        {
            List<Point> neighbors = new List<Point>();

            // Your logic for determining regular neighbors...

            // Consider exit points as valid neighbors
            if (map.Exits != null)
            {
                foreach (var exitPoint in map.Exits.Keys)
                {
                    // You might want to add additional conditions to check if the exit is reachable or valid
                    neighbors.Add(exitPoint);
                }
            }

            // Check neighboring tiles for walkability
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) // Skip the current point
                        continue;

                    int newX = point.X + dx;
                    int newY = point.Y + dy;

                    Point neighbor = new Point((short)newX, (short)newY);

                    if (IsValidNeighbor(map, point, neighbor))
                    {
                        neighbors.Add(neighbor);
                    }
                }
            }

            return neighbors;
        }

        private static bool IsValidNeighbor(Map map, Point current, Point neighbor)
        {
            // Check if the neighbor is within the map boundaries
            if (neighbor.X < 0 || neighbor.X >= map.SizeX || neighbor.Y < 0 || neighbor.Y >= map.SizeY)
            {
                return false;
            }

            // Check if the neighbor tile is walkable (not a wall)
            Tile neighborTile;
            if (map.Tiles.TryGetValue(neighbor, out neighborTile))
            {
                // Modify this condition based on your tile walkability criteria
                return !neighborTile.IsWall;
            }

            return false;
        }

        private static Direction GetDirectionFromPoints(Point start, Point end)
        {
            // Determine the direction based on the difference between points
            int deltaX = end.X - start.X;
            int deltaY = end.Y - start.Y;

            if (deltaX > 0)
                return Direction.East;
            else if (deltaX < 0)
                return Direction.West;
            else if (deltaY > 0)
                return Direction.South;
            else if (deltaY < 0)
                return Direction.North;

            return Direction.Invalid;
        }
    }

}
