using System;

namespace pappab0t.Modules.Inventory.Items.Tokens
{
    public class NoteType : ItemType
    {
        public override string Name => "Anteckning";
        public override string IndefiniteAricle => "En";
        public override Guid Id => new Guid("562ae10d-2823-4fea-9fa6-9b7b35da6659");
    }
}
