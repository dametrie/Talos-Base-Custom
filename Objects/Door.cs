using System;
using Talos.Structs;

namespace Talos.Objects
{
    internal class Door : MapObject
    {
        internal Point Point => new Point(SourceX, SourceY);
        internal Location Location => new Location(SourceMapID, SourceX, SourceY);
        internal DateTime LastClicked { get; set; }
        internal bool RecentlyClicked => DateTime.UtcNow.Subtract(LastClicked).TotalSeconds < 1.5;
        internal bool Closed {  get; set; }
        internal Door(Location location, bool closed)
        {
            SourceX = location.X;
            SourceY = location.Y;
            SourceMapID = location.MapID;
            Closed = closed;
            LastClicked = DateTime.UtcNow;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Door otherDoor = (Door)obj;
            return SourceMapID == otherDoor.SourceMapID &&
                   SourceX == otherDoor.SourceX &&
                   SourceY == otherDoor.SourceY &&
                   Closed == otherDoor.Closed;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + SourceMapID.GetHashCode();
                hash = hash * 23 + SourceX.GetHashCode();
                hash = hash * 23 + SourceY.GetHashCode();
                hash = hash * 23 + Closed.GetHashCode();
                return hash;
            }
        }
    }
}
