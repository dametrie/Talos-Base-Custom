using System;
using Talos.Objects;
using Talos.Structs;

namespace Talos.Maps
{
    internal class Warp : MapObject
    {
        internal override short SourceX => SourceX;
        internal override short SourceY => SourceY;
        internal override short SourceMapID => SourceMapID;
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
    }
}
