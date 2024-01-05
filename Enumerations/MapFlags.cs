using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Enumerations
{
    internal enum MapFlags : ulong
    {
        Hostile = 1,
        NonHostile = 2,
        NoSpells = 4,
        NoSkills = 8,
        NoChat = 16,
        Snowing = 32,
    }
}
