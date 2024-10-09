using Talos.Structs;

namespace Talos.Objects
{
    internal class GroundItem : VisibleObject
    {
        internal bool Exists { get; private set; }

        internal GroundItem(int id, ushort sprite, Location location, bool exists)
            : base(id, string.Empty, sprite, location)
        {
            Exists = exists;
            SpriteID = sprite;
        }
    }
}
