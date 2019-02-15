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
    public class TimedBombExpirationEventHandler : IEventHandler
    {
        public const string HighScoreName = "TimedBomb";

        private readonly IBot _bot;
        private readonly IInventoryManager _inventoryManager;
        private readonly IHighScoreManager _highScoreManager;
        private readonly IPhrasebook _phrasebook;

        private bool _initialized;

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

        public void Initialize()
        {
            if (_initialized)
                return;

            MessageBus<ItemExpiredMessage>.Instance.MessageRecieved += InstanceOnMessageRecieved;

            _initialized = true;
        }

        private void InstanceOnMessageRecieved(object sender, MessageBusEventArgs<ItemExpiredMessage> e)
        {
            if (!(e.Message.Item.Type is TimedBombType))
                return;

            var scoreDictionary = new Dictionary<string, int>();
            var handlerLog = e.Message.Item.Modifiers.OfType<IHandlerLog>().First();

            scoreDictionary.Add(e.Message.SourceInventory.UserId, handlerLog.LogEntries.Count);

            foreach (var entry in handlerLog.LogEntries.Where(x => x.UserId != e.Message.SourceInventory.UserId))
            {
                if (!scoreDictionary.ContainsKey(entry.UserId))
                    scoreDictionary.Add(entry.UserId, 0);
                scoreDictionary[entry.UserId]++;
            }

            foreach (var score in scoreDictionary)
            {
                _highScoreManager.Handle(HighScoreName, score.Key, score.Value);
            }

            e.Message.SourceInventory.Items.Remove(e.Message.Item);
            _inventoryManager.Save(e.Message.SourceInventory);

            _bot.Say(new BotMessage
            {
                Text = _phrasebook.TimedBombExpired()
            });
        }

        public void Dispose()
        {
            MessageBus<ItemExpiredMessage>.Instance.MessageRecieved -= InstanceOnMessageRecieved;
        }
    }
}