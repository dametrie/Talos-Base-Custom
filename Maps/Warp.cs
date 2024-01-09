using System;
using Talos.Objects;
using Talos.Structs;

namespace Talos.Maps
{
    internal class Warp : MapObject
    {
        internal override byte SourceX => this.SourceX;
        internal override byte SourceY => this.SourceY;
        internal override short SourceMapID => this.SourceMapID;
        internal byte TargetX { get; }
        internal byte TargetY { get; }
        internal short TargetMapID { get; }
        internal Location SourceLocation => new Location(this.SourceMapID, this.SourceX, this.SourceY);
        internal Location TargetLocation => new Location(this.TargetMapID, this.TargetX, this.TargetY);

        internal Warp(byte sourceX, byte sourceY, byte targetX, byte targetYbyte, short sourceMapID, short targetMapID)
        {
            this.SourceX = sourceX;
            this.SourceY = sourceY;
            this.TargetX = targetX;
            this.TargetY = targetYbyte;
            this.SourceMapID = sourceMapID;
            this.TargetMapID = targetMapID;
        }
    }
}
