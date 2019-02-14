using System;
using System.Linq;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Models;
using pappab0t.Modules.BombGame.Items;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items;
using pappab0t.Modules.Inventory.Items.Modifiers;
using pappab0t.Modules.Inventory.Items.Tokens;

namespace pappab0t.Modules.BombGame
{
    public class TimedBombCommand : Command
    {
        private readonly IInventoryManager _inventoryManager;
        private readonly IPhrasebook _phrasebook;
        private readonly Random _random;

        public static decimal Cost => 50M;
        public static int LowQualityExpirationSpan => 120;
        public static int BaseExpiration => 7;

        public TimedBombCommand(
            IInventoryManager inventoryManager, 
            IPhrasebook phrasebook,
            Random random)
        {
            _inventoryManager = inventoryManager;
            _phrasebook = phrasebook;
            _random = random;
        }

        public override BotMessage GetResponse()
        {
            _inventoryManager.Context = Context;
            var inv = _inventoryManager.GetUserInventory();
            if(inv.BEK < Cost)
                return new BotMessage{Text = _phrasebook.PlayInsufficientFunds(Cost)};

            inv.BEK -= Cost;
            var tb = new Item(new Novelty(), new TimedBombType());//todo item factory
            tb.Modifiers.Add(new Expires
            {
                DateTime = SystemTime
                    .Now()
                    .AddDays(BaseExpiration)
                    .AddMinutes(_random.Next(LowQualityExpirationSpan))
            });
            var log = new HandlerLog();
            log.Add(Context.Message.User.ID);
            tb.Modifiers.Add(log);
            inv.Items.Add(tb);

            _inventoryManager.Save(inv);

            return new BotMessage{Text = _phrasebook.ItemCreated(tb.Type.Name)};
        }

        public override bool RespondsTo(string cmd)
        {
            return new[] {"timedbomb", "tb"}.Contains(cmd);
        }
    }
}