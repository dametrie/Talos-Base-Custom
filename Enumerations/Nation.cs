using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Enumerations
{
    [Flags]
    internal enum Nation : byte
    {
        None = 1,
        Suomi = 2,
        Loures = 3,
        Mileth = 4,
        Tagor = 5,
        Rucesion = 6,
        Noes = 7
    }

}
