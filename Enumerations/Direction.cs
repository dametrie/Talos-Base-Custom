using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Enumerations
{
    [Flags]
    internal enum Direction : byte
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
        Invalid = 0xff
    }
}
