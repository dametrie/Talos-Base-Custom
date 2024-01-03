using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos
{
    internal struct ProcessInformation
    {
        internal IntPtr ProcessHandle { get; set; }

        internal IntPtr ThreadHandle { get; set; }

        internal int ProcessId { get; set; }

        internal int ThreadId { get; set; }
    }
}
