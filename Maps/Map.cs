using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Enumerations;
using Talos.Structs;

namespace Talos.Maps
{
    internal class Map
    {
        internal short MapID { get; }
        internal byte SizeX { get; }
        internal byte SizeY { get; }
        internal byte Flags { get; set; }
        internal string Name { get; set; }
        internal sbyte Music { get; set; }
        internal bool CanUseSkills { get; set; }
        internal bool CanUseSpells { get; set; }

        internal Dictionary<Point, Tile> Tiles { get; }
        internal Dictionary<Point, Warp> Exits { get; }
        internal Dictionary<Point, WorldMap> WorldMaps { get; set; }

        internal Map(short mapID, byte sizeX, byte sizeY, byte flags, string name, sbyte music)
        {
            MapID = mapID;
            SizeX = sizeX;
            SizeY = sizeY;
            Flags = flags;
            Name = name;
            Music = music;
            Tiles = new Dictionary<Point, Tile>();
            Exits = new Dictionary<Point, Warp>();
            WorldMaps = new Dictionary<Point, WorldMap>();
        }
    }
}
