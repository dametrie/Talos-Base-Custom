using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Base;

namespace Talos.Objects
{
    internal sealed class Caster : Spell
    {

        internal SpellCastDelegate SpellCastDelegate
        {
            get;
            private set;
        }

        internal Caster(string string_2)
        {
        }

        internal Caster(byte type, string name, string prompt, SpellCastDelegate spellCastDelegate)
        {
            base.Type = type;
            base.Name = name;
            base.Prompt = prompt;
            SpellCastDelegate = spellCastDelegate;
        }
    }

    internal delegate void SpellCastDelegate(Client client, uint target, string args);
}
