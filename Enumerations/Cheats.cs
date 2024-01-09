using System;

namespace Talos.Enumerations
{
    [Flags]
    internal enum Cheats : byte
    {
        None = 0,
        NoBlind = 1,
        SeeHidden = 2,
        ZoomableMap = 4,
        NoForegrounds = 8,
        GmMode = 16,
        SeeGhosts = 32
    }

}
