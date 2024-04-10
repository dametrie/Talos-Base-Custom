using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Talos.Base;
using Talos.Enumerations;
using Talos.Objects;
using Talos.Structs;

namespace Talos.Maps
{
    internal class Map
    {
        internal bool IsLoaded { get; private set; }
        internal short MapID { get; }
        internal byte Width { get; }
        internal byte Height { get; }
        internal byte Flags { get; set; }
        internal ushort Checksum { get; set; }  
        internal string Name { get; set; }
        internal sbyte Music { get; set; }
        internal bool CanUseSkills { get; set; }
        internal bool CanUseSpells { get; set; }

        internal ConcurrentDictionary<Point, Tile> Tiles { get; set; }
        internal ConcurrentDictionary<Point, Warp> Exits { get; set; }
        internal ConcurrentDictionary<Point, WorldMap> WorldMaps { get; set; }

        internal Map(short mapID, byte sizeX, byte sizeY, byte flags, string name, sbyte music)
        {
            MapID = mapID;
            Width = sizeX;
            Height = sizeY;
            Flags = flags;
            Name = name;
            Music = music;
            CanUseSkills = true;
            CanUseSpells = true;
            Tiles = new ConcurrentDictionary<Point, Tile>();
            Exits = new ConcurrentDictionary<Point, Warp>();
            WorldMaps = new ConcurrentDictionary<Point, WorldMap>();
        }

        internal Map(short mapID, byte sizeX, byte sizeY, byte flags, ushort checksum, string name)
        {
            MapID = mapID;
            Width = sizeX;
            Height = sizeY;
            Flags = flags;
            Checksum = checksum;
            Name = name;
            CanUseSkills = true;
            CanUseSpells = true;
            Tiles = new ConcurrentDictionary<Point, Tile>();
            Exits = new ConcurrentDictionary<Point, Warp>();
            WorldMaps = new ConcurrentDictionary<Point, WorldMap>();
        }

        internal void Initialize(byte[] data)
        {
            Tiles.Clear();
            int index = 0;
            for (short y = 0; y < Height; y = (short)(y + 1))
            {
                for (short x = 0; x < Width; x = (short)(x + 1))
                {
                    Point key = new Point(x, y);
                    short bg = (short)(data[index++] | (data[index++] << 8));
                    short lf = (short)(data[index++] | (data[index++] << 8));
                    short rf = (short)(data[index++] | (data[index++] << 8));
                    Tiles[key] = new Tile(bg, lf, rf);
                }
            }
            IsLoaded = true;
        }

        internal bool IsLocationWall(Location loc)
        {
            return IsTileWall(loc.X, loc.Y);
        }

        internal bool IsTileWall(short x, short y)
        {
            //Console.WriteLine($"Checking coordinates: ({x}, {y})");

            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                Point key = new Point(x, y);
                try
                {

                    bool isWall = Tiles[key].IsWall;
                    //Console.WriteLine($"Tile at coordinates ({x}, {y}) is{(isWall ? "" : " not")} a wall.");
                    return isWall;
                }
                catch (Exception ex) // Temporarily catch all exceptions to diagnose the issue
                {
                    Console.WriteLine($"Exception caught for key {key}. Exception type: {ex.GetType()}. Message: {ex.Message}");
                    throw; // Rethrow to preserve the stack trace
                }
            }
            else
            {
                Console.WriteLine($"Coordinates ({x}, {y}) are outside the map boundaries.");
                return false;
            }
        }


        internal bool IsLocationOpenForInteraction(Client client, Location location)
        {
            if (!IsLocationWall(location))
            {
                return !client.GetWorldObjects().Any(delegate (WorldObject worldEntity)
                {
                    Creature creature = worldEntity as Creature;
                    return creature != null && Location.Equals(creature.Location, location) && (creature.Type == CreatureType.Aisling || creature.Type == CreatureType.Merchant || creature.Type == CreatureType.Normal) && creature != client.Player;
                });
            }
            return false;
        }
    }
}
