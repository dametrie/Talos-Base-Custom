using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Definitions
{
    [Flags]
    internal enum ProcessAccessFlags : uint
    {
        None = 0,
        Terminate = 0x0001,
        CreateThread = 0x0002,
        VmOperation = 0x0008,
        VmRead = 0x0010,
        VmWrite = 0x0020,
        DuplicateHandle = 0x0040,
        CreateProcess = 0x0080,
        SetQuota = 0x0100,
        SetInformation = 0x0200,
        QueryInformation = 0x0400,
        SuspendResume = 0x0800,
        QueryLimitedInformation = 0x1000,
        Synchronize = 0x00100000,
        FullAccess = 0x1F0FFF
    }

    [Flags]
    internal enum ProcessCreationFlags : uint
    {
        DebugProcess = 0x0001,
        DebugOnlyThisProcess = 0x0002,
        Suspended = 0x0004,
        DetachedProcess = 0x0008,
        NewConsole = 0x0010,
        NewProcessGroup = 0x0200,
        UnicodeEnvironment = 0x0400,
        SeparateWowVdm = 0x0800,
        SharedWowVdm = 0x1000,
        InheritParentAffinity = 0x00010000,
        ProtectedProcess = 0x00040000,
        ExtendedStartupInfoPresent = 0x00080000,
        BreakawayFromJob = 0x01000000,
        PreserveCodeAuthZLevel = 0x02000000,
        DefaultErrorMode = 0x04000000,
        NoWindow = 0x08000000
    }

    [Flags]
    internal enum WaitEventResult : uint
    {
        Signaled = 0,
        Abandoned = 128,
        Timeout = 258
    }

    [Flags]
    internal enum Cheats : byte
    {
        None = 0,
        NoBlind = 1,
        SeeHidden = 2,
        ZoomableMap = 4,
        NoForegrounds = 8,
        GmMode = 16,
        SeeGhosts = 32
    }
}
