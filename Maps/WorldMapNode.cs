using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Structs;

namespace Talos.Maps
{
    internal class WorldMapNode
    {
        internal Point Position { get; set; }
        internal string Name { get; set; }  
        internal short MapID { get; set; }
        
        internal Point Location { get; set; }

        internal Location TargetLocation => new Location(MapID, Location);

        internal WorldMapNode(Point position, string name, short mapId, Point location)
        {
            Position = position;
            Name = name;
            MapID = mapId;
            Location = location;
        }
    }
}
