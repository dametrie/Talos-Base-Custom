using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Properties;

namespace Talos.Structs
{
    internal struct Tile
    {
        internal ushort Background { get; }
        internal ushort LeftForeground { get; }
        internal ushort RightForeground { get; }

        internal static byte[] SOTP => Resources.sotp;

        internal bool IsWall => (LeftForeground > 0 && (SOTP[LeftForeground - 1] & 15) == 15) || (RightForeground > 0 && (SOTP[RightForeground - 1] & 15) == 15);

        /// <summary>
        /// Constructor for a single tile on the map
        /// </summary>
        /// <param name="background">background visual data</param>
        /// <param name="leftForeground">leftForeground visual data</param>
        /// <param name="rightForeground">rightForeground visual data</param>
        internal Tile(ushort background, ushort leftForeground, ushort rightForeground)
        {
            Background = background;
            LeftForeground = leftForeground;
            RightForeground = rightForeground;
        }

    }
}
