using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Talos.Objects
{
    internal sealed class Spellbook : IEnumerable<Spell>
    {
        private Dictionary<string, Spell> SpellbookDictionary { get; } = new Dictionary<string, Spell>();
        private Spell[] SpellArray { get; } = new Spell[90];

        internal int MaxSpells => 90;

        internal Spell this[string spellName] => SpellbookDictionary.TryGetValue(spellName, out var spell) ? spell : null;

        internal Spell this[byte slot] => slot >= 0 && slot < MaxSpells ? SpellArray[slot] : null;

        internal void AddOrUpdateSpell(Spell spell)
        {
            if (spell == null)
                return;

            if (SpellbookDictionary.TryGetValue(spell.Name, out var existingSpell))
            {
                existingSpell.Slot = spell.Slot;
                existingSpell.CastLines = spell.CastLines;
                existingSpell.CurrentLevel = spell.CurrentLevel;
                existingSpell.MaximumLevel = spell.MaximumLevel;
                existingSpell.Ticks = spell.Ticks > existingSpell.Ticks ? spell.Ticks : existingSpell.Ticks;
            }
            else
            {
                SpellbookDictionary.Add(spell.Name, spell);
            }

            SpellArray[spell.Slot] = spell;
        }

        internal void UpdateSpellCooldown(string spellName, DateTime cooldown, double ticks)
        {
            if (SpellbookDictionary.TryGetValue(spellName, out var spell))
            {
                spell.Cooldown = cooldown;
                spell.Ticks = ticks;
                //Console.WriteLine($"[UpdateSpellCooldown] Spell: {spell.Name}, Cooldown: {spell.Cooldown}, Ticks: {spell.Ticks}");
            }
            else
            {
                //Console.WriteLine($"[UpdateSpellCooldown] Spell not found: {spellName}");
            }
        }

        internal void RemoveSpell(byte slot)
        {
            if (slot >= 0 && slot < MaxSpells && SpellArray[slot] != null)
            {
                SpellbookDictionary.Remove(SpellArray[slot]!.Name);
                SpellArray[slot] = null;
            }
        }

        public IEnumerator<Spell> GetEnumerator() => SpellbookDictionary.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

