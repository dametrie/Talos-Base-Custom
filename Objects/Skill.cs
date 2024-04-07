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
        /*        internal bool CanUse
                {
                    get
                    {
                        Console.WriteLine($"[DEBUG] Checking CanUse for spell. Cooldown: {Cooldown}, Ticks: {Ticks}");

                        if (!(Cooldown == DateTime.MinValue) && Ticks != 0.0)
                        {
                            double num = Math.Max(1.0, Ticks);
                            TimeSpan nowFromLastUsed = DateTime.UtcNow.Subtract(LastUsed);
                            TimeSpan nowFromCooldown = DateTime.UtcNow.Subtract(Cooldown);

                            Console.WriteLine($"[DEBUG] num: {num}");
                            Console.WriteLine($"[DEBUG] Now from Last Used: {nowFromLastUsed.TotalSeconds} seconds");
                            Console.WriteLine($"[DEBUG] Now from Cooldown: {nowFromCooldown.TotalSeconds} seconds");

                            if (!(nowFromLastUsed.TotalSeconds > 0.5))
                            {
                                Console.WriteLine("[DEBUG] Spell cannot be used yet. Last used less than 0.5 seconds ago.");
                                return false;
                            }
                            if (nowFromCooldown.TotalSeconds > num)
                            {
                                Console.WriteLine("[DEBUG] Spell can be used. Cooldown period has passed.");
                                return true;
                            }
                            else
                            {
                                Console.WriteLine("[DEBUG] Spell cannot be used yet. Still within cooldown period.");
                                return false;
                            }
                        }
                        else
                        {
                            Console.WriteLine("[DEBUG] Spell can be used. No cooldown or ticks.");
                            return true;
                        }
                    }
                }*/

        internal bool CanUse
        {
            get
            {
                Console.WriteLine($"[DEBUG] Checking CanUse for skill {Name}. Cooldown until: {Cooldown.AddSeconds(Ticks)}, Current Time: {DateTime.UtcNow}");

                // Check if the cooldown has expired
                if (DateTime.UtcNow >= Cooldown.AddSeconds(Ticks))
                {
                    Console.WriteLine("[DEBUG] skill can be used. Cooldown period has passed.");
                    return true;
                }
                else
                {
                    Console.WriteLine("[DEBUG] skill cannot be used yet. Still within cooldown period.");
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
