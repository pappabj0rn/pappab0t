using System.Collections.Generic;
using System.Linq;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Models;
using pappab0t.Modules.BombGame.Items;
using pappab0t.Modules.Highscore;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items.Modifiers;

namespace pappab0t.Modules.BombGame
{
    public class TimedBombTask : ScheduledTask
    {
        private readonly IBot _bot;
        private readonly IInventoryManager _inventoryManager;
        private readonly IHighScoreManager _highScoreManager;
        private readonly IPhrasebook _phrasebook;

        public override int Interval => 7 * Milliseconds;

        public TimedBombTask(
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
                foreach (var tb in inventory.Items.OfType<TimedBomb>())
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
        public TimedBomb Tb { get; set; }
        public Inventory.Inventory Inv { get; set; }
    }
}