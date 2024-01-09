using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Objects
{
    internal class Skill
    {
        internal string Name { get; set; }  
        internal byte Slot { get; set; }    
        internal ushort Sprite { get; private set; }
        internal byte CurrentLevel { get; set; }    
        internal byte MaxLevel { get; set; }    
        internal double Ticks { get; set; }
        internal DateTime Cooldown { get; set; }
        internal DateTime LastUsed { get; set; }
        internal bool CanUse
        {
            get
            {
                if (!(Cooldown == DateTime.MinValue) && Ticks != 0.0)
                {
                    double num = Math.Max(1.0, Ticks);
                    TimeSpan nowFromLastUsed = DateTime.UtcNow.Subtract(LastUsed);
                    TimeSpan nowFromCooldown = DateTime.UtcNow.Subtract(Cooldown);
                    if (!(nowFromLastUsed.TotalSeconds > 0.5))
                    {
                        return false;
                    }
                    return nowFromCooldown.TotalSeconds > num;
                }
                return true;
            }
        }
        internal Skill(byte slot, string name, ushort sprite, byte currentLevel, byte maxLevel)
        {
            Slot = slot;
            Name = name;
            Sprite = sprite;
            CurrentLevel = currentLevel;
            MaxLevel = maxLevel;
            LastUsed = DateTime.MinValue;
        }
    }
}
