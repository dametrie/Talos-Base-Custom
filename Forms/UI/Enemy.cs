namespace Talos.Forms.UI
{
    internal class Enemy
    {
        private string _name;
        private ushort? _spriteID;
        internal EnemyPage EnemyPage { get; set; }
        internal bool IsAllMonsters => _name == "all monsters";

        internal ushort SpriteID
        {
            get => _spriteID ?? 0;
            set
            {
                _name = value.ToString();
                _spriteID = value;
            }
        }

        internal Enemy(ushort sprite)
        {
            SpriteID = sprite;
        }

        internal Enemy(string name)
        {
            _name = name;
            _spriteID = ushort.TryParse(name, out ushort result) ? result : null;
        }

        public virtual string ToString()
        {
            return _name;
        }
    }
}
