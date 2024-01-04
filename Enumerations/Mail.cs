using System;

namespace Talos.Enumerations
{
    [Flags]
    internal enum Mail : byte
    {
        HasParcel = 0x1,
        HasLetter = 0x10
    }

}
