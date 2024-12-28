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
                //Console.WriteLine($"[SKILL] Checking CanUse for skill {Name}. Cooldown until: {Cooldown.AddSeconds(Ticks)}, Current Time: {DateTime.UtcNow}");

                // Check if the cooldown has expired
                if (DateTime.UtcNow >= Cooldown.AddSeconds(Ticks))
                {
                    //Console.WriteLine("[SKILL] skill can be used. Cooldown period has passed.");
                    return true;
                }
                else
                {
                    //Console.WriteLine($"[SKILL] skill {Name} cannot be used yet. Still within cooldown period.");
                    return false;
                }
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
