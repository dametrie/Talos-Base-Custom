using Talos.Structs;

namespace Talos.Objects
{
    internal abstract class VisibleObject : WorldObject
    {
        internal ushort Sprite { get; set; }
        internal Location Location { get; set; }
        internal VisibleObject(int id, string name, ushort sprite, Location location)
            : base(id, name)
        {
            Sprite = sprite;
            Location = location;
        }

        internal bool WithinRange(VisibleObject other, int distance = 12)
        {
            if (Location.MapID != other.Location.MapID)
                return false;
            else
                return Location.Point.DistanceFrom(other.Location.Point) <= distance;
        }
    }
}
