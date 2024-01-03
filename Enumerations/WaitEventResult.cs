using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos
{
    [Flags]
    internal enum WaitEventResult
    {
        Signaled = 0x0,
        Abandoned = 0x80,
        Timeout = 0x102
    }

}
