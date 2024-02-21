using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.AStar;
using Talos.Enumerations;

namespace Talos.Structs
{
    internal struct Location : IEquatable<Location>
    {
        internal short MapID { get; set; }
        internal short X { get; set; }
        internal short Y { get; set; }

        internal Point Point => new Point(X, Y);

        /// <summary>
        /// Constructor for a location that has a mapID and a point
        /// </summary>
        /// <param name="mapID"></param>
        /// <param name="point"></param>
        internal Location(short mapID, Point point)
        {
            MapID = mapID;
            X = point.X;
            Y = point.Y;
        }

        internal Location(short mapID, short x, short y)
        {
            MapID = mapID;
            X = x;
            Y = y;
        }

        internal Location TranslateLocationByDirection(Direction dir)
        {
            Location result = new Location(MapID, new Point(X, Y));

            switch (dir)
            {
                case Direction.North:
                    result.Y--;
                    break;
                case Direction.East:
                    result.X++;
                    break;
                case Direction.South:
                    result.Y++;
                    break;
                case Direction.West:
                    result.X--;
                    break;
                    
            }

            return result;
        }

        internal int DistanceFrom(Location other)
        {
            if (MapID != other.MapID)
                return int.MaxValue;
            else
                return Point.DistanceFrom(other.Point);
        }

        /// <summary>
        /// Returns true if both locations are equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Location other) => MapID == other.MapID && Point.Equals(other.Point);

        internal static bool Equals(Location location1, Location location2)
        {
            return location1.MapID == location2.MapID && location1.Point.Equals(location2.Point);
        }

        internal static bool NotEquals(Location location1, Location location2)
        {
            return !Equals(location1, location2);
        }

        /// <summary>
        /// Prints the value of the point to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{MapID}:{Point}";

        /// <summary>
        /// Attempts to parse a string into a location
        /// </summary>
        public static bool TryParse(string input, out Location location)
        {
            location = default(Location);
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            string[] array = input.Split(new char[]
            {
                ','
            });
            if (array.Length != 3)
            {
                return false;
            }
            short mapID;
            if (!short.TryParse(array[0], out mapID))
            {
                return false;
            }
            short x;
            if (!short.TryParse(array[1], out x))
            {
                return false;
            }
            short y;
            if (!short.TryParse(array[2], out y))
            {
                return false;
            }
            location = new Location(mapID, new Point(x, y));
            return true;
        }

        internal int DistanceFrom(PathNode2 node)
        {
            //check the distance from location to node
            return Math.Abs(X - node.X) + Math.Abs(Y - node.Y);
        }
    }
}
