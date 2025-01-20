using System;

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
