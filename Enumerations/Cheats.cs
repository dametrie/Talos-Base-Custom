using System;

namespace Talos.Enumerations
{
    [Flags]
    internal enum Cheats : byte
    {
        None = 0x0,
        NoBlind = 0x1,
        SeeHidden = 0x2,
        ZoomableMap = 0x4,
        NoForegrounds = 0x8,
        GmMode = 0x10,
        SeeGhosts = 0x20
    }

}
