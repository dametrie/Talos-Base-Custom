using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Enumerations
{
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
