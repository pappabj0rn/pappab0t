using pappab0t.Modules.Inventory.Items;

namespace pappab0t.Modules.Inventory
{
    public class ItemExpiredMessage
    {
        public Item Item { get; set; }
        public Inventory SourceInventory { get; }

        public ItemExpiredMessage(Item item, Inventory inventory)
        {
            Item = item;
            SourceInventory = inventory;
        }
    }
}