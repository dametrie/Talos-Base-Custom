using System;

namespace Talos.Enumerations
{
    [Flags]
    internal enum Mail : byte
    {
        HasParcel = 1,
        HasLetter = 16
    }

}
