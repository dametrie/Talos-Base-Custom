using System;

namespace Talos.Objects
{
    internal abstract class WorldObject : IComparable<WorldObject>
    {
        protected internal int ID { get; }
        protected internal string Name { get; set; }
        protected internal DateTime Creation { get; private set; }
        internal void Created(DateTime creation)
        {
            Creation = creation;
        }
        internal WorldObject(int id, string name)
        {
            ID = id;
            Name = name;
            Creation = DateTime.Now;
        }
        public int CompareTo(WorldObject other) => ReferenceEquals(this, other) ? 0 : ID.CompareTo(other.ID);
    }
}
