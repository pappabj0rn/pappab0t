using System.Collections.Generic;
using MargieBot;

namespace pappab0t.Modules.Inventory
{
    public interface IInventoryManager
    {
        Inventory GetUserInventory();
        Inventory GetUserInventory(string targetUserId);
        void Save(IEnumerable<Inventory> inventories);
        void Save(Inventory userInv);
        ResponseContext Context { get; set; }
    }
}