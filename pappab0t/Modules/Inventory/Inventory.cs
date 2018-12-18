using System.Collections.Generic;

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
    }
}