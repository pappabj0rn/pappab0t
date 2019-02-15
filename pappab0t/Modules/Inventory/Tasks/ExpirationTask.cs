using System;
using System.Collections.Generic;
using System.Linq;
using pappab0t.Abstractions;
using pappab0t.Modules.Inventory.Items;
using pappab0t.Modules.Inventory.Items.Modifiers;

namespace pappab0t.Modules.Inventory.Tasks
{
    public class ExpirationTask : ScheduledTask
    {
        private readonly IInventoryManager _inventoryManager;

        public ExpirationTask(IInventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        public override int Interval => 7 * Milliseconds;

        public override bool Execute()
        {
            if (!IsDue())
                return false;

            LastRunDate = SystemTime.Now();

            var inventories = _inventoryManager.GetAll();
            var expiredData = new List<ExpiredItemData>();

            foreach (var inventory in inventories)
            {
                foreach (var item in inventory.Items)
                {
                    foreach (var modifier in item.Modifiers)
                    {
                        if (!HasExpiredModifiers(modifier))
                            continue;

                        expiredData.Add(new ExpiredModifierData
                        {
                            Date = modifier
                                .Modifiers
                                .OfType<Expires>()
                                .First()
                                .DateTime,
                            Modifier = modifier,
                            Item = item,
                            SourceInventory = inventory
                        });
                    }

                    if (!HasExpiredModifiers(item))
                        continue;

                    expiredData.Add(new ExpiredItemData
                    {
                        Date = item
                            .Modifiers
                            .OfType<Expires>()
                            .First()
                            .DateTime,
                        Item = item,
                        SourceInventory = inventory
                    });
                }
            }

            foreach (var data in expiredData.OrderBy(x=>x.Date))
            {
                if(data is ExpiredModifierData emd)
                {
                    MessageBus<ModifierExpiredMessage>.Instance.SendMessage(
                        this, 
                        new ModifierExpiredMessage(
                            emd.Modifier, 
                            emd.Item, 
                            emd.SourceInventory));
                }
                else
                {
                    MessageBus<ItemExpiredMessage>.Instance.SendMessage(
                        this, 
                        new ItemExpiredMessage(
                            data.Item, 
                            data.SourceInventory));
                }

            }

            return true;
        }

        private static bool HasExpiredModifiers(IModifiable modifiable)
        {
            return modifiable.Modifiers.Any(x => x is Expires e && e.DateTime < SystemTime.Now());
        }
    }

    internal class ExpiredItemData
    {
        public DateTime Date { get; set; }
        public Item Item { get; set; }
        public Inventory SourceInventory { get; set; }
    }

    internal class ExpiredModifierData : ExpiredItemData
    {
        public Modifier Modifier { get; set; }
    }
}
/*
     * public class TimedBombExpirationEventHandler : ScheduledTask
    {
        private readonly IBot _bot;
        private readonly IInventoryManager _inventoryManager;
        private readonly IHighScoreManager _highScoreManager;
        private readonly IPhrasebook _phrasebook;

        public override int Interval => 7 * Milliseconds;

        public TimedBombExpirationEventHandler(
            IBot bot,
            IInventoryManager inventoryManager, 
            IHighScoreManager highScoreManager,
            IPhrasebook phrasebook)
        {
            _bot = bot;
            _inventoryManager = inventoryManager;
            _highScoreManager = highScoreManager;
            _phrasebook = phrasebook;
        }

        public override bool Execute()
        {
            if (!IsDue())
                return false;

            LastRunDate = SystemTime.Now();

            var inventories = _inventoryManager.GetAll();
            var expiredTbs = new List<ExpiredTbData>();

            foreach (var inventory in inventories)
            {
                foreach (var tb in inventory.Items.Where(x=>x.Type is TimedBombType))
                {
                    if (!tb.Modifiers.Any(x => x is Expires e && e.DateTime < SystemTime.Now()))
                        continue;

                    expiredTbs.Add(new ExpiredTbData
                    {
                        Tb=tb,
                        Inv = inventory
                    });
                }
            }

            foreach (var data in expiredTbs)
            {
                var scoreDictionary = new Dictionary<string,int>();
                var handlerLog = data.Tb.Modifiers.OfType<IHandlerLog>().First();

                scoreDictionary.Add(data.Inv.UserId,handlerLog.LogEntries.Count());

                foreach (var entry in handlerLog.LogEntries.Where(x=>x.UserId != data.Inv.UserId))
                {
                    if(!scoreDictionary.ContainsKey(entry.UserId))
                        scoreDictionary.Add(entry.UserId,0);
                    scoreDictionary[entry.UserId]++;
                }

                foreach (var score in scoreDictionary)
                {
                    _highScoreManager.Handle("TimedBomb", score.Key, score.Value);
                }

                data.Inv.Items.Remove(data.Tb);
                _inventoryManager.Save(data.Inv);

                _bot.Say(new BotMessage
                {
                    Text = _phrasebook.TimedBombExpired()
                });
            }

            return true;
        }
    }

    internal class ExpiredTbData
    {
        public Item Tb { get; set; }
        public Inventory.Inventory Inv { get; set; }
    }
     */
