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
        /// Override Equals method to check equality
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Point point)
            {
                return Equals(point);
            }
            return false;
        }

        /// <summary>
        /// Override GetHashCode to ensure consistency with Equals
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Implements the == operator for Points
        /// </summary>
        public static bool operator ==(Point left, Point right) => left.Equals(right);

        /// <summary>
        /// Implements the != operator for Points
        /// </summary>
        public static bool operator !=(Point left, Point right) => !left.Equals(right);


        internal void Offset(Direction dir)
        {
            switch (dir)
            {
                case Direction.North:
                    --Y;
                    break;
                case Direction.East:
                    ++X;
                    break;
                case Direction.South:
                    ++Y;
                    break;
                case Direction.West:
                    --X;
                    break;
            }
        }


        internal Point Offsetter(Direction dir)
        {
            Point result = new Point(X, Y);
            result.Offset(dir);
            return result;
        }


        internal Direction GetDirection(Point point)
        {
            if (Y < point.Y)
                return Direction.North;
            if (X > point.X)
                return Direction.East;
            if (Y > point.Y)
                return Direction.South;
            if (X < point.X)
                return Direction.West;

            return Direction.Invalid;
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
            return (Math.Abs(X - x) + Math.Abs(Y - y));
        }
    }
}
