using pappab0t.Modules.Inventory.Items;

namespace pappab0t.Modules.Inventory
{
    public abstract class ItemMoveMessage
    {
        public Item Item { get; }
        public Inventory SourceInventory { get; }
        public Inventory TargetInventory { get; }

        protected ItemMoveMessage(Item item, Inventory sourceInventory, Inventory targetInventory)
        {
            Item = item;
            SourceInventory = sourceInventory;
            TargetInventory = targetInventory;
        }
    }

    public class ItemMovingMessage : ItemMoveMessage
    {
        public bool Cancel { get; set; }

        public ItemMovingMessage(Item item, Inventory sourceInventory, Inventory targetInventory) 
            : base(item, sourceInventory, targetInventory)
        {
        }
    }

    public class ItemMovedMessage : ItemMoveMessage
    {
        public ItemMovedMessage(Item item, Inventory sourceInventory, Inventory targetInventory) 
            : base(item, sourceInventory, targetInventory)
        {
        }
    }
}