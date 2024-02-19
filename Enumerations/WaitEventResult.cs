using System;

namespace Talos
{
    internal enum WaitEventResult
    {
        Signaled = 0x0,
        Abandoned = 0x80,
        Timeout = 0x102
    }

}
