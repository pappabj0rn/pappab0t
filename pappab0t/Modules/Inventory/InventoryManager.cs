using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Raven.Client;

namespace pappab0t.Modules.Inventory
{
    public class InventoryManager : IInventoryManager
    {
        private readonly IDocumentStore _documentStore;
        public ResponseContext Context { get; set; }

        public InventoryManager(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public Inventory GetUserInventory()
        {
            return GetUserInventory(Context.Message.User.ID);
        }

        public Inventory GetUserInventory(string targetUserId)
        {
            using (var session = _documentStore.OpenSession())
            {
                var inv = session.Query<Inventory>().SingleOrDefault(x => x.UserId == targetUserId);

                if (inv != null)
                    return inv;

                inv = new Inventory { UserId = targetUserId, BEK = 100 };
                session.Store(inv);
                session.SaveChanges();

                return inv;
            }
        }

        public void Save(IEnumerable<Inventory> inventories)
        {
            using (var session = _documentStore.OpenSession())
            {
                foreach (var inventory in inventories)
                {
                    session.Store(inventory);
                }

                session.SaveChanges();
            }
        }

        public void Save(Inventory userInv)
        {
            Save(new []{userInv});
        }
    }
}
