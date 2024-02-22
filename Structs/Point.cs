using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Enumerations;

namespace Talos.Structs
{
    internal struct Point : IEquatable<Point>
    {
        internal short X { get; set; }
        internal short Y { get; set; }

        /// <summary>
        /// Constructor for a point on the map
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal Point(short x, short y)
        {
            X = x;
            Y = y;
        }

        internal Point(int x, int y)
        {
            X = (short)x;
            Y = (short)y;
        }

        /// <summary>
        /// Return the distance between this point and another
        /// </summary>
        /// <param name="other">the other point to compare</param>
        /// <returns></returns>
        internal int Distance(Point other) => Distance(other.X, other.Y);

        /// <summary>
        /// Return the distance between this point and another
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal int Distance(short x, short y) => Math.Abs(X - x) + Math.Abs(Y - y);

        /// <summary>
        /// Returns true of both sets of points are equal
        /// </summary>
        /// <param name="other">the other point to compare</param>
        /// <returns></returns>
        public bool Equals(Point other) => X == other.X && Y == other.Y;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        internal Direction GetDirection(Direction dir)
        {
            return dir switch
            {
                Direction.North => (Direction)Y--,
                Direction.East => (Direction)X++,
                Direction.South => (Direction)Y++,
                Direction.West => (Direction)X--,
                _ => Direction.Invalid
            };
        }

        internal Direction GetDirection(Point loc)
        {
            if (Y == loc.Y)
            {
                if (X == loc.X)
                {
                    return Direction.Invalid;
                }
                return X < loc.X ? Direction.West : Direction.East;
            }
            return Y < loc.Y ? Direction.North : Direction.South;
        }

        /// <summary>
        /// Create a new point objbect the represents the current point translated by the direction
        /// </summary>
        /// <param name="dir">The direction to translate by</param>
        /// <returns></returns>
        internal Point TranslatePointByDirection(Direction dir)
        {
            Point result = new Point(X, Y);
            result.GetDirection(dir);
            return result;
        }

        /// <summary>
        /// Prints the value of the point to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"({X}, {Y})";


        /// <summary>
        /// Get the distance between this point and another
        /// </summary>
        /// <param name="point">Other point to compare</param>
        /// <returns></returns>
        internal int DistanceFrom(Point point)
        {
            return AbsoluteXY(point.X, point.Y);
        }
        
        /// <summary>
        /// Return the absolute value of the difference between this point and another
        /// </summary>
        /// <param name="x">Other X to compare</param>
        /// <param name="y">Other Y to compare</param>
        /// <returns></returns>
        internal int AbsoluteXY(short x, short y)
        {
            return Math.Abs(X - x) + Math.Abs(Y - y);
        }
    }
}
