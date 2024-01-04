using System;

namespace Talos
{
    [Flags]
    internal enum ProcessAccess
    {
        None = 0x0,
        Terminate = 0x1,
        CreateThread = 0x2,
        VmOperation = 0x8,
        VmRead = 0x10,
        VmWrite = 0x20,
        DuplicateHandle = 0x40,
        CreateProcess = 0x80,
        SetQuota = 0x100,
        SetInformation = 0x200,
        QueryInformation = 0x400,
        SuspendResume = 0x800,
        QueryLimitedInformation = 0x1000
    }

    [Flags]
    internal enum ProcessCreationFlags
    {
        DebugProcess = 0x1,
        DebugOnlyThisProcess = 0x2,
        Suspended = 0x4,
        DetachedProcess = 0x8,
        NewConsole = 0x10,
        NewProcessGroup = 0x200,
        UnicodeEnvironment = 0x400,
        SeparateWowVdm = 0x800,
        SharedWowVdm = 0x1000,
        InheritParentAffinity = 0x1000,
        ProtectedProcess = 0x40000,
        ExtendedStartupInfoPresent = 0x80000,
        BreakawayFromJob = 0x1000000,
        PreserveCodeAuthZLevel = 0x2000000,
        DefaultErrorMode = 0x4000000,
        NoWindow = 0x8000000
    }

}
