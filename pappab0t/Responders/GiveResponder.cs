using System.Linq;
using System.Text;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Models;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items.Modifiers;

namespace pappab0t.Responders
{
    public class GiveResponder : ResponderBase, IExposedCapability
    {
        private readonly IInventoryManager _invMan;
        private readonly IPhrasebook _phrasebook;
        private const string MoneyParamKey = "p";
        private const string ItemParamKey = "s";

        private GiveMode _mode = GiveMode.NotSet;
        private BotMessage _returnMsg;

        public GiveResponder(IInventoryManager invMan, IPhrasebook phrasebook, ICommandDataParser commandDataParser)
            : base(commandDataParser)
        {
            _invMan = invMan;
            _phrasebook = phrasebook;
        }

        public override bool CanRespond(ResponseContext context)
        {
            Init(context);

            return CommandData.ToBot
                   && CommandData.Command == "ge";
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Init(context);
            _invMan.Context = context;
            _returnMsg = null;

            if (CommandData.Params.ContainsKey("?"))
            {
                var sb = new StringBuilder();
                sb.AppendLine("Beskrivning av kommando: ge");
                sb.AppendLine("Ger en användare pengar eller saker.");
                sb.AppendLine("ex pengar: ge nisse 10,5kr");
                sb.AppendLine("ex pengar: ge nisse -p 10,5");
                sb.AppendLine("ex sak: ge nisse sak 1");
                sb.AppendLine("ex sak: ge nisse -s 1");
                sb.AppendLine("Parametrar:");
                sb.AppendLine("p: [decimal], ");
                sb.AppendLine("s: [sak nr], för att se vad du har för saker att ge, använd kommando i.");
                sb.AppendLine("?: Hjälp (denna text)");

                return new BotMessage
                {
                    Text = sb.ToString()
                };
            }

            if (!CommandData.Params.ContainsKey(Keys.CommandParser.UserIdKey)
                || !CommandData.Params.ContainsKey(Keys.CommandParser.UserKnownKey))
            {
                var unknown = CommandData.Params.ContainsKey(Keys.CommandParser.UserIdKey) 
                              && !CommandData.Params.ContainsKey(Keys.CommandParser.UserKnownKey)
                    ? CommandData.Params[Keys.CommandParser.UserIdKey]
                    : CommandData.ParamsRaw.Split(' ')[0];

                return new BotMessage { Text = _phrasebook.IDontKnowXxxNamedYyy("nån",unknown) };
            }

            var saveInventories = SaveMode.None;
            var userInventory = _invMan.GetUserInventory();
            var targetInventory = _invMan.GetUserInventory(CommandData.Params[Keys.CommandParser.UserIdKey]);

            SetMode();

            switch (_mode)
            {
                case GiveMode.GiveMoney:
                    var amount = GetAmount();
                    if (_returnMsg != null)
                        break;

                    if (amount < 0)
                    {
                        _returnMsg = new BotMessage { Text = _phrasebook.Impossible() };
                        break;
                    }
                    if (userInventory.BEK < amount)
                    {
                        _returnMsg = new BotMessage { Text = _phrasebook.MoneyTransferInsufficientFunds() };
                        break;
                    }

                    userInventory.BEK -= amount;
                    targetInventory.BEK += amount;
                    saveInventories = SaveMode.Both;
                    _returnMsg = new BotMessage { Text = _phrasebook.MoneyTransfered(amount) };
                    break;
                case GiveMode.GiveItem:
                    var itemIndex = GetItemIndex();
                    if (_returnMsg != null)
                        break;

                    if (itemIndex == -1)
                    {
                        _returnMsg = new BotMessage { Text = _phrasebook.Impossible() };
                        break;
                    }

                    if (itemIndex > userInventory.Items.Count-1)
                    {
                        _returnMsg = new BotMessage { Text= _phrasebook.ItemTransferToFewItems() };
                        break;
                    }

                    if (userInventory.Items[itemIndex].Modifiers.Any(x=>x is Soulbound))
                    {
                        _returnMsg = new BotMessage { Text = _phrasebook.CantMoveSoulboundItems() };
                        break;
                    }

                    var itemName = userInventory.Items[itemIndex].Name;
                    _invMan.MoveItemByIndex(itemIndex,targetInventory.UserId);
                    saveInventories = SaveMode.None;
                    _returnMsg = new BotMessage { Text = _phrasebook.ItemTransfered(itemName) };//todo should look at events instad as move could be canceled in the future
                    break;
                default:
                    CreateIdidntUnderstandMessage();
                    break;
            }

            switch (saveInventories)
            {
                case SaveMode.Current:
                    _invMan.Save(userInventory);
                    break;
                case SaveMode.Target:
                    _invMan.Save(targetInventory);
                    break;
                case SaveMode.Both:
                    _invMan.Save(new[] { userInventory, targetInventory });
                    break;
            }

            return _returnMsg;
        }

        private int GetItemIndex()
        {
            var index = -1;

            if (CommandData.Params.ContainsKey(ItemParamKey))
            {
                if (!int.TryParse(CommandData.Params[ItemParamKey], out index))
                {
                    CreateIdidntUnderstandMessage();
                }

                index--;
            }
            else if (CommandData.Params.ContainsKey(Keys.CommandParser.UnnamedParam))
            {
                if (!int.TryParse(CommandData.Params[Keys.CommandParser.UnnamedParam].Replace("sak ", ""),
                    out index))
                {
                    CreateIdidntUnderstandMessage();
                }

                index--;
            }

            return index;
        }

        private void CreateIdidntUnderstandMessage()
        {
            _returnMsg = new BotMessage {Text = _phrasebook.IDidntUnderstand()};
        }

        private decimal GetAmount()
        {
            decimal amount = -1;

            if (CommandData.Params.ContainsKey(MoneyParamKey))
            {
                if (!decimal.TryParse(CommandData.Params[MoneyParamKey], out amount))
                {
                    CreateIdidntUnderstandMessage();
                }
            }
            else if(CommandData.Params.ContainsKey(Keys.CommandParser.UnnamedParam))
            {
                if (!decimal.TryParse(CommandData.Params[Keys.CommandParser.UnnamedParam].Replace("kr",""), 
                    out amount))
                {
                    CreateIdidntUnderstandMessage();
                }
            }

            return amount;
        }

        private void SetMode()
        {
            if (CommandData.Params.ContainsKey(MoneyParamKey)
                || CommandData.Params.ContainsKey(Keys.CommandParser.UnnamedParam)
                && CommandData.Params[Keys.CommandParser.UnnamedParam].Contains("kr"))
                _mode = GiveMode.GiveMoney;

            else if(CommandData.Params.ContainsKey(ItemParamKey)
                    || CommandData.Params.ContainsKey(Keys.CommandParser.UnnamedParam)
                    && CommandData.Params[Keys.CommandParser.UnnamedParam].Contains("sak "))
                _mode = GiveMode.GiveItem;
        }
        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "ge <användare> <Xkr|sak X>",
            Explatation = "Ger X pengar eller sak X till given användare."
        };

        enum GiveMode
        {
            NotSet,
            GiveMoney,
            GiveItem
        }

        enum SaveMode
        {
            None,
            Current,
            Target,
            Both
        }
    }
}