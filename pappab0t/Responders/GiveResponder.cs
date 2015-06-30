using System;
using System.Linq;
using System.Text.RegularExpressions;
using MargieBot.Models;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using pappab0t.Modules.Inventory;
using WebSocketSharp;

namespace pappab0t.Responders
{
    public class GiveResponder : ResponderBase, IExposedCapability
    {
        private const string UserKey = "user";
        private const string AmountKey = "amount";
        private const string IndexKey = "index";
        private const string MoneyRegex = @"\bge\b\s(?<" + UserKey + @">\w+)\s(?<" + AmountKey + @">[0-9]+)kr";
        private const string ItemRegex = @"\bge\b\s(?<" + UserKey + @">\w+)\sitem_(?<" + IndexKey + @">[0-9]+)";

        private string targetUsername;
        private string targetUserId;
        private int amount, itemIndex;
        private bool moneyTransfer;

        public override bool CanRespond(ResponseContext context)
        {
            return (
                    context.Message.MentionsBot 
                    || context.Message.IsDirectMessage()
                   ) 
                   &&
                   (
                       Regex.IsMatch(context.Message.Text, MoneyRegex, RegexOptions.IgnoreCase)
                       || Regex.IsMatch(context.Message.Text, ItemRegex, RegexOptions.IgnoreCase)
                   );
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;
            CollectParams();
            
            targetUserId = context.UserNameCache
                                  .FirstOrDefault(x => x.Value.Equals(targetUsername, StringComparison.InvariantCultureIgnoreCase))
                                  .Key;

            if(targetUserId.IsNullOrEmpty())
                return new BotMessage { Text = "{0} {1}"
                                                .With(PhraseBook.GetOpenAppology(), PhraseBook.GetIDontKnowXxxNamedYyy()
                                                                                                .With("nån användare",targetUsername)) };

            var invMan = new InventoryManager(Context);
            var userInventory = invMan.GetUserInventory();
            var targetInventory = invMan.GetUserInventory(targetUserId);

            string msgText;
            if (moneyTransfer)
            {
                if(userInventory.BEK < amount)
                    return new BotMessage { Text = "Du kan inte ge nån mer pengar än du har." };

                userInventory.BEK -= amount;
                targetInventory.BEK += amount;

                msgText = "{0}kr överlämnade.".With(amount);
            }
            else
            {
                if(userInventory.Items.Count<itemIndex)
                    return new BotMessage { Text = "Du har inte så många saker." };

                var item = userInventory.Items[itemIndex - 1];
                userInventory.Items.Remove(item);
                targetInventory.Items.Add(item);

                msgText = "{0} är överlämnad.".With(item.Name);
            }

            invMan.Save(new[] {userInventory, targetInventory});
            
            return new BotMessage{Text = msgText};
        }

        private void CollectParams()
        {
            var match = Regex.Match(Context.Message.Text, MoneyRegex);
            if (match.Success)
            {
                amount = int.Parse(match.Groups[AmountKey].Value);
                moneyTransfer = true;
            }
            else
            {
                match = Regex.Match(Context.Message.Text, ItemRegex);
                itemIndex = int.Parse(match.Groups[IndexKey].Value);
                moneyTransfer = false;
            }

            targetUsername = match.Groups[UserKey].Value;
        }

        public ExposedInformation Info
        {
            get
            {
                return new ExposedInformation
                {
                    Usage = "ge <användare> {Xkr|item_X}",
                    Explatation = "Ger X pengar eller sak X till given användare."
                };
            }
        }
    }
}