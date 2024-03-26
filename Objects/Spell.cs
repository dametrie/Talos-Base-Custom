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
        internal static Dictionary<string, double> SpellDuration = new Dictionary<string, double>
        {
            { "Cursed Tune", 3.5 },
            { "Frost Arrow 1", 4.0 },
            { "Frost Arrow 2", 4.0 },
            { "Frost Arrow 3", 4.0 },
            { "Frost Arrow 4", 4.0 },
            { "Frost Arrow 5", 4.0 },
            { "Frost Arrow 6", 4.0 },
            { "Frost Arrow 7", 4.0 },
            { "Frost Arrow 8", 4.0 },
            { "Frost Arrow 9", 4.0 },
            { "Frost Arrow 10", 4.0 },
            { "Frost Arrow 11", 4.0 },
            { "dionLR", 8.0 },
            { "dion", 10.0 },
            { "Draco Stance", 10.0 },
            { "Stone Skin", 10.0 },
            { "Wings of Protection", 13.0 },
            { "Asgall Faileas", 13.0 },
            { "Perfect Defense", 15.0 },
            { "beah pramh", 8.0 },
            { "pramh", 16.0 },
            { "dall", 18.0 },
            { "reflection", 19.0 },//Adam check spelling
            { "mor dion", 20.0 },
            { "Mesmerize", 20.0 },
            { "Iron Skin", 20.0 },
            { "mor dion comlha", 20.0 },
            { "Increased Regeneration", 35.0 },
            { "beag naomh aite", 60.0 },
            { "naomh aite", 120.0 },
            { "beag cradh", 150.0 },
            { "armachd", 150.0 },
            { "beannaich", 150.0 },
            { "Dark Seal", 155.0 },
            { "Darker Seal", 155.0 },
            { "Demise", 155.0 },
            { "creag neart", 163.0 },
            { "cradh", 180.0 },
            { "mor cradh", 210.0 },
            { "ard fas nadur", 225.0 },
            { "ard cradh", 240.0 },
            { "mor beannaich", 240.0 },
            { "Cat's Hearing", 240.0 },
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

        internal static double GetSpellDuration(string spellName)
        {
            if (SpellDuration.ContainsKey(spellName))
                return SpellDuration[spellName];
            return 30.0;
        }
    }
}
