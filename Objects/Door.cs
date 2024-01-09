using System;
using Talos.Structs;

namespace Talos.Objects
{
    internal class Door : MapObject
    {
        internal Point Point => new Point(SourceX, SourceY);
        internal Location Location => new Location(SourceMapID, SourceX, SourceY);
        internal DateTime LastClicked { get; set; }
        internal bool RecentlyClosed => DateTime.UtcNow.Subtract(LastClicked).TotalSeconds < 1.5;
        internal bool Closed {  get; set; }
        internal Door(Location location, bool closed)
        {
            SourceX = location.X;
            SourceY = location.Y;
            SourceMapID = location.MapID;
            Closed = closed;
            LastClicked = DateTime.UtcNow;
        }   
    }
}
