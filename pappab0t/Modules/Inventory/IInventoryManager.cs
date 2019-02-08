using System.Collections.Generic;
using MargieBot;

namespace pappab0t.Modules.Inventory
{
    public interface IInventoryManager
    {
        ResponseContext Context { get; set; }
        
        IEnumerable<Inventory> GetAll();

        Inventory GetUserInventory();

        Inventory GetUserInventory(string targetUserId);

        void Save(IEnumerable<Inventory> inventories);

        void Save(Inventory userInv);
        void MoveItemByIndex(int itemIndex, string targetUserUuid);
    }
}