using System;
using System.Linq;
using System.Text.RegularExpressions;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using pappab0t.Modules.Inventory;

namespace pappab0t.Responders
{
    //todo: ge ören
    //todo: ge med @

    public class GiveResponder : ResponderBase, IExposedCapability
    {
        private const string UserKey = "user";
        private const string AmountKey = "amount";
        private const string IndexKey = "index";
        private const string MoneyRegex = @"\bge\b\s(?<" + UserKey + @">\w+)\s+(?<" + AmountKey + @">[0-9]+)kr";
        private const string ItemRegex = @"\bge\b\s(?<" + UserKey + @">\w+)\s+item_(?<" + IndexKey + @">[0-9]+)";

        private string _targetUsername;
        private string _targetUserId;
        private int _amount, _itemIndex;
        private bool _moneyTransfer;

        public override bool CanRespond(ResponseContext context)
        {
            //return CommandInfo.ToBot && CommandInfo.Command == "ge"
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
            
            _targetUserId = context.UserNameCache
                                  .FirstOrDefault(x => x.Value.Equals(_targetUsername, StringComparison.InvariantCultureIgnoreCase))
                                  .Key;

            if(_targetUserId.IsNullOrEmpty())
                return new BotMessage { Text = "{0} {1}"
                                                .With(PhraseBook.OpenAppology(), PhraseBook.IDontKnowXxxNamedYyyFormat()
                                                                                                .With("nån användare",_targetUsername)) };

            var invMan = new InventoryManager(Context);
            var userInventory = invMan.GetUserInventory();
            var targetInventory = invMan.GetUserInventory(_targetUserId);

            string msgText;
            if (_moneyTransfer)
            {
                if(userInventory.BEK < _amount)
                    return new BotMessage { Text = "Du kan inte ge nån mer pengar än du har." };

                userInventory.BEK -= _amount;
                targetInventory.BEK += _amount;

                msgText = "{0}kr överlämnade.".With(_amount);
            }
            else
            {
                if(userInventory.Items.Count<_itemIndex)
                    return new BotMessage { Text = "Du har inte så många saker." };

                var item = userInventory.Items[_itemIndex - 1];
                userInventory.Items.Remove(item);
                targetInventory.Items.Add(item);

                msgText = "{0} överlämnad.".With(item.Name);
            }

            invMan.Save(new[] {userInventory, targetInventory});
            
            return new BotMessage{Text = msgText};
        }

        private void CollectParams()
        {
            var match = Regex.Match(Context.Message.Text, MoneyRegex);
            if (match.Success)
            {
                _amount = int.Parse(match.Groups[AmountKey].Value);
                _moneyTransfer = true;
            }
            else
            {
                match = Regex.Match(Context.Message.Text, ItemRegex);
                _itemIndex = int.Parse(match.Groups[IndexKey].Value);
                _moneyTransfer = false;
            }

            _targetUsername = match.Groups[UserKey].Value;
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