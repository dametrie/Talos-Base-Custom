using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talos.Base;

namespace Talos.Objects
{
    internal delegate void SpellCastDelegate(Client client, uint target, string args);
    internal sealed class ProxySpell : Spell
    {

        internal SpellCastDelegate OnUse
        {
            get;
            private set;
        }

        internal ProxySpell(string spellName)
        {
        }

        internal ProxySpell(byte type, string name, string prompt, SpellCastDelegate onUse)
        {
            Type = type;
            Name = name;
            Prompt = prompt;
            OnUse = onUse;
        }
    }
}
