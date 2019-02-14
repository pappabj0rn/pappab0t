using System.Collections.Generic;
using System.Linq;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items;
using pappab0t.Modules.Inventory.Items.Tokens;
using pappab0t.Tests.Responders;
using Xunit;

namespace pappab0t.Tests.Modules.Inventory
{
    public abstract class InventoryManagerTests : ResponderTestsContext
    {
        private const string TestlappName = "testlapp";
        protected IInventoryManager InventoryManager;

        protected InventoryManagerTests()
        {
            ConfigureRavenDB();
            InventoryManager = new InventoryManager(Store)
            {
                WaitForNonStaleResults = true
            };

            var ei = InventoryManager.GetUserInventory(EriskaUserId);
            var pbi = InventoryManager.GetUserInventory(PappaBj0rnUserId);
            pbi.Items.Add(new Item(new Token(), new NoteType()) {Name = TestlappName});
            InventoryManager.Save(new []{ei,pbi});
        }

        public class MoveItemByIndex : InventoryManagerTests
        {
            [Fact]
            public void Should_raise_MovingItem_event()
            {
                ItemMoveMessage msg = null;
                MessageBus<ItemMovingMessage>.Instance.MessageRecieved += (sender, args) => msg = args.Message;

                InventoryManager.Context = CreateContext("ge eriska sak 1", userUUID: UserNameCache.First().Key);

                InventoryManager.MoveItemByIndex(0, EriskaUserId);

                Assert.NotNull(msg);
                Assert.Equal(TestlappName, msg.Item.Name);
                Assert.Equal(PappaBj0rnUserId, msg.SourceInventory.UserId);
                Assert.Equal(EriskaUserId, msg.TargetInventory.UserId);
            }

            [Fact]
            public void Should_move_item_to_target_invenotry()
            {
                InventoryManager.Context = CreateContext("ge eriska sak 1", userUUID: UserNameCache.First().Key);

                InventoryManager.MoveItemByIndex(0, EriskaUserId);
                var source = InventoryManager.GetUserInventory(PappaBj0rnUserId);
                var target = InventoryManager.GetUserInventory(EriskaUserId);

                Assert.Empty(source.Items);
                Assert.True(target.Items.Any(x=>x.Name == TestlappName));
            }

            [Fact]
            public void Should_raise_MovedItem_event()
            {
                ItemMoveMessage msg = null;
                MessageBus<ItemMovedMessage>.Instance.MessageRecieved += (sender, args) => msg = args.Message;

                InventoryManager.Context = CreateContext("ge eriska sak 1", userUUID: UserNameCache.First().Key);

                InventoryManager.MoveItemByIndex(0, EriskaUserId);

                Assert.NotNull(msg);
                Assert.Equal(TestlappName, msg.Item.Name);
                Assert.Equal(PappaBj0rnUserId, msg.SourceInventory.UserId);
                Assert.Equal(EriskaUserId, msg.TargetInventory.UserId);
            }

            [Fact]
            public void Should_not_move_item_when_eventargs_cancel_is_set_to_true()
            {
                ItemMovingMessage msg;
                MessageBus<ItemMovingMessage>.Instance.MessageRecieved += (sender, args) =>
                {
                    msg = args.Message;
                    msg.Cancel = true;
                };

                var movedRaised = false;
                MessageBus<ItemMovedMessage>.Instance.MessageRecieved += (sender, args) => movedRaised = true;
                
                InventoryManager.Context = CreateContext("ge eriska sak 1", userUUID: UserNameCache.First().Key);

                InventoryManager.MoveItemByIndex(0, EriskaUserId);
                var source = InventoryManager.GetUserInventory(PappaBj0rnUserId);
                var target = InventoryManager.GetUserInventory(EriskaUserId);

                Assert.False(movedRaised);
                Assert.True(source.Items.Any(x => x.Name == TestlappName));
                Assert.Empty(target.Items);
            }
        }

        public class GetAll : InventoryManagerTests
        {
            [Fact]
            public void Should_return_all_stored_inventories()
            {
                IEnumerable<pappab0t.Modules.Inventory.Inventory> storedInventories;
                using (var session = Store.OpenSession())
                {
                    storedInventories = session.Query<pappab0t.Modules.Inventory.Inventory>().ToList();
                }

                var allInventories = InventoryManager.GetAll().ToList();

                Assert.Equal(storedInventories.Count(), allInventories.Count());
                foreach (var si in storedInventories)
                {
                    Assert.True(allInventories.Any(x=>x.Id == si.Id));
                }
            }
        }
    }
}