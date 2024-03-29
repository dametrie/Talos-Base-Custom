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
                Console.WriteLine($"[DEBUG] Checking CanUse for spell {Name}. Cooldown: {Cooldown}, Ticks: {Ticks}");

                if (!(Cooldown == DateTime.MinValue) && Ticks != 0.0)
                {
                    //double num = Ticks - (double)(int)CastLines + 0.5;
                    double num = Ticks;
                    TimeSpan timeSinceLastUsed = DateTime.UtcNow.Subtract(LastUsed);
                    TimeSpan timeSinceCooldownStarted = DateTime.UtcNow.Subtract(Cooldown);

                    Console.WriteLine($"[DEBUG] num: {num}");
                    Console.WriteLine($"[DEBUG] Time since Last Used: {timeSinceLastUsed.TotalSeconds} seconds");
                    Console.WriteLine($"[DEBUG] Time since Cooldown Started: {timeSinceCooldownStarted.TotalSeconds} seconds");

                    if (timeSinceLastUsed.TotalSeconds > 1.5)
                    {
                        if (timeSinceCooldownStarted.TotalSeconds > num)
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
                }
                Console.WriteLine("[DEBUG] Spell can be used. No cooldown or ticks.");
                return true;

            }
        }

        internal static Dictionary<string, double> SpellDuration = new Dictionary<string, double>
        {
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
            { "Cursed Tune 1", 15.0 },
            { "Cursed Tune 2", 20.0 },
            { "Cursed Tune 3", 25.0 },
            { "Cursed Tune 4", 30.0 },
            { "Cursed Tune 5", 35.0 },
            { "Cursed Tune 6", 40.0 },
            { "Cursed Tune 7", 45.0 },
            { "Cursed Tune 8", 50.0 },
            { "Cursed Tune 9", 55.0 },
            { "Cursed Tune 10", 60.0 },
            { "Cursed Tune 11", 65.0 },
            { "Cursed Tune 12", 70.0 },
            { "Regeneration 1", 30.0 },
            { "Regeneration 2", 30.0 },
            { "Regeneration 3", 30.0 },
            { "Regeneration 4", 30.0 },
            { "Regeneration 5", 30.0 },
            { "Regeneration 6", 30.0 },
            { "Regeneration 7", 30.0 },
            { "Regeneration 8", 30.0 },
            { "Regeneration 9", 30.0 },
            { "Regeneration 10", 30.0 },
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
            { "Reflection", 19.0 },
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
