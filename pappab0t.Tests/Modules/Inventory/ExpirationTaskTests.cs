using System;
using Moq;
using pappab0t.Modules.BombGame.Items;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items;
using pappab0t.Modules.Inventory.Items.Modifiers;
using pappab0t.Modules.Inventory.Tasks;
using Xunit;

namespace pappab0t.Tests.Modules.Inventory
{
    public class ExpirationTaskTests : TestContext
    {
        private readonly ExpirationTask _task;

        private Item _expiredItem;
        private Item _itemWithExpiredMod;
        private Modifier _expiredMod;

        public ExpirationTaskTests()
        {
            _task = new ExpirationTask(InventoryManagerMock.Object);

            CreateExpiredTimedBomb();
            CreateItemWithExpiredModifier();
            Pappabj0rnInvetory.Items.Add(_expiredItem);
            Pappabj0rnInvetory.Items.Add(_itemWithExpiredMod);
        }

        private void CreateExpiredTimedBomb()
        {
            _expiredItem = new Item(new Novelty(), new TimedBombType());

            _expiredItem.Modifiers.Add(new HandlerLog());
            _expiredItem.Modifiers.Add(new Expires { DateTime = SystemTime.Now().AddDays(-1) });
        }

        private void CreateItemWithExpiredModifier()
        {
            _itemWithExpiredMod = new Item(new Novelty(), new TimedBombType());

            _expiredMod = new Soulbound();
            _expiredMod.Modifiers.Add(new Expires { DateTime = SystemTime.Now().AddDays(-2) });

            _itemWithExpiredMod.Modifiers.Add(_expiredMod);
        }

        [Fact]
        public void Should_search_all_inventories_for_expired_timed_bombs()
        {
            _task.Execute();

            InventoryManagerMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public void Should_return_false_when_task_is_not_due()
        {
            var time = new DateTime(2019, 2, 7, 2, 49, 11);
            SystemTime.Now = () => time;

            _task.Execute();

            SystemTime.Now = () => time.AddMilliseconds(_task.Interval - 1);

            Assert.False(_task.Execute());
        }

        [Fact]
        public void Should_return_true_when_task_is_due()
        {
            Assert.True(_task.Execute());
        }

        [Fact]
        public void Should_raise_itemExpired_message_when_a_inventory_contains_an_expired_item()
        {
            ItemExpiredMessage msg = null;
            MessageBus<ItemExpiredMessage>.Instance.MessageRecieved += (sender, args) => msg = args.Message;

            _task.Execute();

            Assert.Equal(_expiredItem, msg.Item);
            Assert.Equal(Pappabj0rnInvetory.Id, msg.SourceInventory.Id);
        }

        [Fact]
        public void Should_raise_modifierExpired_message_when_a_inventory_contains_an_item_with_an_expired_modifier()
        {
            ModifierExpiredMessage msg = null;
            MessageBus<ModifierExpiredMessage>.Instance.MessageRecieved += (sender, args) => msg = args.Message;

            _task.Execute();

            Assert.Equal(_expiredMod, msg.Modifier);
            Assert.Equal(_itemWithExpiredMod, msg.Item);
            Assert.Equal(Pappabj0rnInvetory.Id, msg.SourceInventory.Id);
        }

        [Fact]
        public void Should_raise_messages_ordered_by_expiration_date()
        {
            var itemFirst = false;
            var modFirst = false;

            MessageBus<ItemExpiredMessage>.Instance.MessageRecieved += (sender, args) => itemFirst = !modFirst;
            MessageBus<ModifierExpiredMessage>.Instance.MessageRecieved += (sender, args) => modFirst = !itemFirst;

            _task.Execute();

            Assert.True(modFirst);
        }
    }
}