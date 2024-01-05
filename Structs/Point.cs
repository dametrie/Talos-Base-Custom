using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Structs
{
    internal struct Point : IEquatable<Point>
    {
        internal short X { get; }
        internal short Y { get; }

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
        /// Prints the value of the point to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"({X}, {Y})";
    }
}
