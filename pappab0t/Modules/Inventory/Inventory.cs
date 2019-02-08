using System.Collections.Generic;
using pappab0t.Extensions;
using pappab0t.Modules.Inventory.Items;

namespace pappab0t.Modules.Inventory
{
    public class Inventory
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public decimal BEK { get; set; }
        public List<Item> Items { get; set; }

        public Inventory()
        {
            Items = new List<Item>();
        }

        public Inventory Clone()
        {
            var copy = (Inventory) MemberwiseClone();
            copy.Id = string.Copy(Id);
            copy.UserId = string.Copy(UserId);
            copy.Items = new List<Item>(Items);
            return copy;
        }
    }
}