using System;

namespace Talos.Enumerations
{
    [Flags]
    internal enum EncryptMethod
    {
        None = 0,
        Normal = 1,
        MD5Key = 2
    }
}
