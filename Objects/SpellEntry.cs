using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Objects
{
    internal class SpellEntry
    {
        internal Spell Spell { get; set; }
        internal Creature Creature { get; set; }
        internal DateTime CooldownEndTime { get; set; }
        internal SpellEntry(Spell spell, Creature creature)
        {
            Spell = spell;
            Creature = creature;
            CooldownEndTime = DateTime.UtcNow;
        }
    }
}
