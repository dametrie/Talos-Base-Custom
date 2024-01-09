using System;

namespace Talos.Enumerations
{
    [Flags]
    internal enum Element : byte
    {
        None = 0,
        Fire = 1,
        Water = 2,
        Wind = 3,
        Earth = 4,
        Holy = 5,
        Darkness = 6,
        Wood = 7,
        Metal = 8,
        Nature = 9,
        Any = 10
    }

}
