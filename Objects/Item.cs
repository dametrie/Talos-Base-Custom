using System;


namespace Talos.Objects
{
    internal class Item
    {
        internal byte Slot { get; set; }
        internal ushort Sprite { get; set; }
        internal byte Color { get; set;}
        internal string Name { get; set; }
        internal int Quantity { get; set; }
        internal bool Stackable { get; set; }    
        internal int MaximumDurability { get; set; }
        internal int CurrentDurability { get; set; }
        internal DateTime LastUsed { get; set; }
        internal bool IsRenamed { get; set; }
        internal bool IsIdentified { get; set; }
        internal bool IsStaff { get; set; }
        internal bool IsMeleeWeapon { get; set; }
        internal bool IsBow { get; set; }
        internal Bow Bow { get; set; }
        internal Staff Staff { get; set; }
        internal MeleeWeapon Melee { get; set; }

        internal Item(byte slot, ushort sprite, byte color, string name, int quantity, bool stackable, int maximumDurability, int currentDurability)
        {
            Slot = slot;
            Sprite = sprite;
            Color = color;
            Name = name;
            Quantity = quantity;
            Stackable = stackable;
            MaximumDurability = maximumDurability;
            CurrentDurability = currentDurability;
            LastUsed = DateTime.MinValue;
            IsRenamed = false;
            IsIdentified = false;
        }

        internal Item(byte slot, ushort sprite, byte color, string name, int quantity, int maximumDurability, int currentDurability)
        {
            Slot = slot;
            Sprite = sprite;
            Color = color;
            Name = name;
            Quantity = quantity;
            MaximumDurability = maximumDurability;
            CurrentDurability = currentDurability;
            LastUsed = DateTime.MinValue;
            IsRenamed = false;
            IsIdentified = false;
        }
    }
}
