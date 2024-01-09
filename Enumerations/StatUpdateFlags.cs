using System;

namespace Talos.Enumerations
{
    [Flags]
    internal enum StatUpdateFlags : byte
    {
        None = 0,
        UnreadMail = 1,
        Unknown = 2,
        Secondary = 4,
        Experience = 8,
        Current = 16,
        Primary = 32,
        GameMasterA = 64,
        GameMasterB = 128,
        Swimming = 192,
        Full = 60
    }

}
