using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Talos.Enumerations;
using Talos.Structs;

namespace Talos.Maps
{
    internal class Map
    {
        internal bool IsLoaded { get; private set; }
        internal short MapID { get; }
        internal byte SizeX { get; }
        internal byte SizeY { get; }
        internal byte Flags { get; set; }
        internal ushort Checksum { get; set; }  
        internal string Name { get; set; }
        internal sbyte Music { get; set; }
        internal bool CanUseSkills { get; set; }
        internal bool CanUseSpells { get; set; }

        internal Dictionary<Point, Tile> Tiles { get; set; }
        internal Dictionary<Point, Warp> Exits { get; set; }
        internal Dictionary<Point, WorldMap> WorldMaps { get; set; }

        internal Map(short mapID, byte sizeX, byte sizeY, byte flags, string name, sbyte music)
        {
            MapID = mapID;
            SizeX = sizeX;
            SizeY = sizeY;
            Flags = flags;
            Name = name;
            Music = music;
            CanUseSkills = true;
            CanUseSpells = true;
            Tiles = new Dictionary<Point, Tile>();
            Exits = new Dictionary<Point, Warp>();
            WorldMaps = new Dictionary<Point, WorldMap>();
        }

        internal Map(short mapID, byte sizeX, byte sizeY, byte flags, ushort checksum, string name)
        {
            MapID = mapID;
            SizeX = sizeX;
            SizeY = sizeY;
            Flags = flags;
            Checksum = checksum;
            Name = name;
            CanUseSkills = true;
            CanUseSpells = true;
            Tiles = new Dictionary<Point, Tile>();
            Exits = new Dictionary<Point, Warp>();
            WorldMaps = new Dictionary<Point, WorldMap>();
        }

        internal void Initialize(byte[] byte_3)
        {
            Tiles.Clear();
            int index = 0;
            for (short y = 0; y < SizeY; y = (short)(y + 1))
            {
                for (short x = 0; x < SizeX; x = (short)(x + 1))
                {
                    Point key = new Point(x, y);
                    short short_ = (short)(byte_3[index++] | (byte_3[index++] << 8));
                    short short_2 = (short)(byte_3[index++] | (byte_3[index++] << 8));
                    short short_3 = (short)(byte_3[index++] | (byte_3[index++] << 8));
                    Tiles[key] = new Tile(short_, short_2, short_3);
                }
            }
            IsLoaded = true;
        }
    }
}
