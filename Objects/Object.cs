using Talos.Structs;

namespace Talos.Objects
{
    internal class Object : VisibleObject
    {
        internal bool Exists { get; private set; }

        internal Object(int id, ushort sprite, Location location, bool exists)
            : base(id, string.Empty, sprite, location)
        {
            Exists = exists;
            SpriteID = sprite;
        }
    }
}
