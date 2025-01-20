using System;

namespace Talos.PInvoke
{
    internal struct ProcessInformation
    {
        internal IntPtr ProcessHandle { get; set; }

        internal IntPtr ThreadHandle { get; set; }

        internal int ProcessId { get; set; }

        internal int ThreadId { get; set; }
    }
}
