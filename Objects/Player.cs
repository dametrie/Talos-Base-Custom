using System;
using Talos.Enumerations;
using Talos.Structs;

namespace Talos.Objects
{
    internal sealed class Player : Creature
    {
        internal bool _isHidden;
        internal ushort HeadSprite { get; set; }
        internal ushort ArmorSprite1 { get; set; }
        internal ushort ArmorSprite2 { get; set; }
        internal ushort WeaponSprite { get; set; }
        internal ushort AccessorySprite1 { get; set; }
        internal ushort AccessorySprite2 { get; set; }
        internal ushort AccessorySprite3 { get; set; }
        internal ushort OvercoatSprite { get; set; }
        internal byte BodySprite { get; set; }
        internal byte BootsSprite { get; set; }
        internal byte ShieldSprite { get; set; }
        internal byte HeadColor { get; set; }
        internal byte BootColor { get; set; }
        internal byte AccessoryColor1 { get; set; }
        internal byte AccessoryColor2 { get; set; }
        internal byte AccessoryColor3 { get; set; }
        internal byte LanternSize { get; set; }
        internal byte RestPosition { get; set; }
        internal byte OvercoatColor { get; set; }
        internal byte BodyColor { get; set; }
        internal byte FaceSprite { get; set; }
        internal byte NameTagStyle { get; set; }
        internal string GroupName { get; set; }
        internal bool NeedsHeal { get; set; }
        internal new string Name { get { return base.Name; } set { base.Name = value; } }
       
        internal bool IsSkulled
        {
            get
            {
               return AnimationHistory.ContainsKey((ushort)SpellAnimation.Skull) && DateTime.UtcNow.Subtract(AnimationHistory[(ushort)SpellAnimation.Skull]).TotalSeconds < 2.0;
            }
        }

        internal Player(int id, string name, Location location, Direction direction)
            : base(id, name, 0, (byte)CreatureType.Aisling, location, direction)
        {
            Type = CreatureType.Aisling;

        }

        internal void NakedPlayer()
        {
            ArmorSprite1 = 0;
            ArmorSprite2 = 0;
            WeaponSprite = 0;
            AccessorySprite1 = 0;
            AccessorySprite2 = 0;
            AccessorySprite3 = 0;
            OvercoatSprite = 0;
            BodySprite = 0;
            BootsSprite = 0;
            ShieldSprite = 0;
            HeadColor = 0;
            BootColor = 0;
            AccessoryColor1 = 0;
            AccessoryColor2 = 0;
            AccessoryColor3 = 0;
            LanternSize = 0;
            RestPosition = 0;
            OvercoatColor = 0;
        }
    }
}
