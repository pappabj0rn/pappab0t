using System;

namespace pappab0t.Modules.Inventory.Items
{
    public abstract class ItemType
    {
        public abstract string Name { get; }
        public abstract string IndefiniteAricle { get; }
        public abstract Guid Id { get; }
    }
}