using Talos.Objects;
using Talos.Structs;

namespace Talos.Maps
{
    internal class Warp : MapObject
    {
        internal byte TargetX { get; }
        internal byte TargetY { get; }
        internal short TargetMapID { get; }
        internal Location SourceLocation => new Location(SourceMapID, SourceX, SourceY);
        internal Location TargetLocation => new Location(TargetMapID, TargetX, TargetY);

        internal Warp(byte sourceX, byte sourceY, byte targetX, byte targetYbyte, short sourceMapID, short targetMapID)
        {
            SourceX = sourceX;
            SourceY = sourceY;
            TargetX = targetX;
            TargetY = targetYbyte;
            SourceMapID = sourceMapID;
            TargetMapID = targetMapID;
        }

        public override string ToString()
        {
            return $"Warp from {SourceMapID}:({SourceX}, {SourceY}) to {TargetMapID}:({TargetX}, {TargetY})";
        }
    }
}
