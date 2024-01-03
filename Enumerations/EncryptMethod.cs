using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Enumerations
{

    [Flags]
    internal enum EncryptMethod
    {
        None = 0x0,
        Normal = 0x1,
        MD5Key = 0x2
    }
}
