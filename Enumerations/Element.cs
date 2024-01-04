using System;

namespace Talos.Enumerations
{
    [Flags]
    internal enum Element : byte
    {
        None = 0x0,
        Fire = 0x1,
        Water = 0x2,
        Wind = 0x3,
        Earth = 0x4,
        Holy = 0x5,
        Darkness = 0x6,
        Wood = 0x7,
        Metal = 0x8,
        Nature = 0x9,
        Any = 0xA
    }

}
