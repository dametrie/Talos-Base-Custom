using Talos.Structs;

namespace Talos.Objects
{
    internal class GroundItem : VisibleObject
    {
        internal bool IsItem { get; private set; }

        internal GroundItem(int id, ushort sprite, Location location, bool isItem)
            : base(id, string.Empty, sprite, location)
        {
            IsItem = isItem;
            SpriteID = sprite;
        }
    }
}
