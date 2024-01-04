using System;

namespace Talos.Enumerations
{
    [Flags]
    internal enum StatUpdateFlags : byte
    {
        None = 0x0,
        UnreadMail = 0x1,
        Unknown = 0x2,
        Secondary = 0x4,
        Experience = 0x8,
        Current = 0x10,
        Primary = 0x20,
        GameMasterA = 0x40,
        GameMasterB = 0x80,
        Swimming = 0xC0,
        Full = 0x3C
    }

}
