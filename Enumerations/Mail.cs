using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Enumerations
{
    [Flags]
    internal enum Mail : byte
    {
        HasParcel = 0x1,
        HasLetter = 0x10
    }

}
