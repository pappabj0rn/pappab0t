using System;
using pappab0t.Modules.Inventory.Items;

namespace pappab0t.Modules.BombGame.Items
{
    public class TimedBombType : ItemType
    {
        public override string Name => "Tidsinställd bomb";
        public override string IndefiniteAricle => "En";
        public override Guid Id => new Guid("de920049-a074-4ba3-acee-da1ea16e8151");
    }
}