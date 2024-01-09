
namespace Talos.Objects
{
    internal abstract class MapObject
    {
        internal virtual byte SourceX { get; set; }
        internal virtual byte SourceY { get; set; }
        internal virtual short SourceMapID { get; set; }
    }
}
