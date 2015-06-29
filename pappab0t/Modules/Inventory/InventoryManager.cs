using System.Linq;
using MargieBot.Models;
using Raven.Client.Document;

namespace pappab0t.Modules.Inventory
{
    public class InventoryManager
    {
        private readonly ResponseContext _context;

        public InventoryManager(ResponseContext context)
        {
            _context = context;
        }

        public Inventory GetUserInventory()
        {
            return GetUserInventory(_context.Message.User.ID);
        }

        public Inventory GetUserInventory(string targetUserId)
        {
            var ravenStore = _context.Get<DocumentStore>();
            using (var session = ravenStore.OpenSession())
            {
                var inv = session.Query<Inventory>().SingleOrDefault(x => x.UserId == targetUserId);

                if (inv == null)
                {
                    inv = new Inventory { UserId = targetUserId, BEK = 100 };
                    session.Store(inv);
                    session.SaveChanges();
                }

                return inv;
            }
        }

        public void Save(Inventory[] inventories)
        {
            var ravenStore = _context.Get<DocumentStore>();
            using (var session = ravenStore.OpenSession())
            {
                foreach (var inventory in inventories)
                {
                    session.Store(inventory);
                }

                session.SaveChanges();
            }
        }
    }
}
