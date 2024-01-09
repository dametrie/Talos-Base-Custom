using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Objects
{
    internal class Spell
    {
        internal string Name { get; set; }  
        internal byte Slot { get; set; }    
        internal byte Type { get; set; }
        internal ushort Sprite { get; private set; }
        internal string Prompt { get; set; }
        internal byte CastLines { get; set; }
        internal byte CurrentLevel { get; set; }
        internal byte MaximumLevel { get; set; }
        internal double Ticks { get; set; }
        internal DateTime LastUsed { get; set; }
        internal DateTime Cooldown { get; set; }
        internal bool CanUse
        {
            get
            {
                if (!(Cooldown == DateTime.MinValue) && Ticks != 0.0)
                {
                    double num = Ticks - (double)(int)CastLines + 0.5;
                    TimeSpan nowFromLastUsed = DateTime.UtcNow.Subtract(LastUsed);
                    TimeSpan nowFromCooldown = DateTime.UtcNow.Subtract(Cooldown);
                    if (nowFromLastUsed.TotalSeconds > 1.5)
                    {
                        return nowFromCooldown.TotalSeconds > num;
                    }
                    return false;
                }
                return true;
            }
        }
        internal Dictionary<string, double> SpellDuration = new Dictionary<string, double>
        {
            { "dion", 10.0 },
            { "Draco Stance", 10.0 },
            { "Stone Skin", 10.0 },
            { "Wings of Protection", 13.0 },
            { "Asgall Faileas", 13.0 },
            { "Perfect Defense", 15.0 },
            { "mor dion", 20.0 },
            { "Iron Skin", 20.0 },
            { "mor dion comlha", 20.0 },
            { "beag naomh aite", 60.0 },
            { "naomh aite", 120.0 },
            { "beag cradh", 150.0 },
            { "Dark Seal", 155.0 },
            { "Darker Seal", 155.0 },
            { "Demise", 155.0 },
            { "cradh", 180.0 },
            { "mor cradh", 210.0 },
            { "ard fas nadur", 225.0 },
            { "ard cradh", 240.0 },
            { "mor naomh aite", 300.0 },
            { "beag fas nadur", 450.0 },
            { "fas nadur", 450.0 },
            { "mor fas nadur", 450.0 },
            { "ard naomh aite", 600.0 },
        };

        internal Spell()
        {
            Name = string.Empty;
            Prompt = string.Empty;
        }

        internal Spell(byte slot, string name, byte type, ushort sprite, string prompt, byte castLines, byte currentLevel, byte maximumLevel)
        {
            Slot = slot;
            Name = name;
            Type = type;
            Sprite = sprite;
            Prompt = prompt;
            CastLines = castLines;
            CurrentLevel = currentLevel;
            MaximumLevel = maximumLevel;
            LastUsed = DateTime.MinValue;
        }

        internal double GetSpellDuration(string spellName)
        {
            if (SpellDuration.ContainsKey(spellName))
                return SpellDuration[spellName];
            return 30.0;
        }
    }
}
