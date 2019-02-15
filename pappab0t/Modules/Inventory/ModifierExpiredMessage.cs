using pappab0t.Modules.Inventory.Items;
using pappab0t.Modules.Inventory.Items.Modifiers;

namespace pappab0t.Modules.Inventory
{
    public class ModifierExpiredMessage
    {
        public Modifier Modifier { get; set; }
        public Item Item { get; set; }
        public Inventory SourceInventory { get; }

        public ModifierExpiredMessage(Modifier modifier, Item item, Inventory inventory)
        {
            Modifier = modifier;
            Item = item;
            SourceInventory = inventory;
        }
    }
}