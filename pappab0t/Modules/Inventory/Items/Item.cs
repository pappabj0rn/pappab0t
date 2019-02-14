using System.Collections.Generic;
using Newtonsoft.Json;
using pappab0t.Modules.Inventory.Items.Modifiers;

namespace pappab0t.Modules.Inventory.Items
{
    public sealed class Item
    {
        public Item(ItemClass @class, ItemType type)
        {
            Class = @class;
            Type = type;
            Modifiers = new List<Modifier>();
        }

        public string Id { get; set; }

        [JsonRequired]
        public string Name { get; set; }

        public ItemClass Class { get; set; }

        public ItemType Type { get; set; }

        public List<Modifier> Modifiers { get; set; }
    }
}