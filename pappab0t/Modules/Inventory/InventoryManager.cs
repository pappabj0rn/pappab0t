using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Raven.Client;

namespace pappab0t.Modules.Inventory
{
    public class InventoryManager : RavenDbManager<Inventory>, IInventoryManager
    {
        private readonly IDocumentStore _documentStore;
        public ResponseContext Context { get; set; }

        private void OnMovingItem(ItemMoveMessage msg)
        {
            MessageBus<ItemMovingMessage>.Instance.SendMessage(this, (ItemMovingMessage) msg);
        }

        private void OnMovedItem(ItemMoveMessage msg)
        {
            MessageBus<ItemMovedMessage>.Instance.SendMessage(this, (ItemMovedMessage) msg);
        }

        public InventoryManager(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IEnumerable<Inventory> GetAll()
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<Inventory>().ToList();
            }
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

                WaitForNonStaleResultsIfEnabled(session);

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

                WaitForNonStaleResultsIfEnabled(session);
            }
        }

        public void Save(Inventory userInv)
        {
            Save(new []{userInv});
        }

        public void MoveItemByIndex(int itemIndex, string targetUserUuid)//todo overload that takes sourceuseruuid
        {
            var source = GetUserInventory();
            var item = source.Items[itemIndex];
            var target = GetUserInventory(targetUserUuid);
            var movingMsg = new ItemMovingMessage(item, source, target);
            var movedMsg = new ItemMovedMessage(item, source, target);

            OnMovingItem(movingMsg);

            if (movingMsg.Cancel)
                return;

            source.Items.Remove(item);
            target.Items.Add(item);
            Save(new[] {source, target});

            OnMovedItem(movedMsg);
        }
    }
}
