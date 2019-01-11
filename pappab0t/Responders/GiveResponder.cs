using System;
using System.Linq;
using System.Reflection;
using System.Text;
using MargieBot;
using Newtonsoft.Json;
using pappab0t.Abstractions;
using pappab0t.Models;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items;

namespace pappab0t.Responders
{
    public class GiveResponder : ResponderBase, IExposedCapability
    {
        private readonly IInventoryManager _invMan;
        private readonly IPhrasebook _phrasebook;
        private const string MoneyParamKey = "p";
        private const string ItemParamKey = "s";
        private const string CreateItemParamKey = "c";

        private GiveMode _mode = GiveMode.NotSet;
        private BotMessage _returnMsg;

        public GiveResponder(IInventoryManager invMan, IPhrasebook phrasebook, ICommandParser commandParser)
            : base(commandParser)
        {
            _invMan = invMan;
            _phrasebook = phrasebook;
        }

        public override bool CanRespond(ResponseContext context)
        {
            Init(context);

            return CommandParser.ToBot
                   && CommandParser.Command == "ge";
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Init(context);
            _invMan.Context = context;
            _returnMsg = null;

            if (CommandParser.Params.ContainsKey("?"))
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

                if (CommandParser.Params.ContainsKey("a"))
                    sb.AppendLine("c: [typ] {json-data}");

                sb.AppendLine("?: Hjälp (denna text)");

                return new BotMessage
                {
                    Text = sb.ToString()
                };
            }

            if (!CommandParser.Params.ContainsKey(Keys.CommandParser.UserIdKey)
                || !CommandParser.Params.ContainsKey(Keys.CommandParser.UserKnownKey))
            {
                var unknown = CommandParser.Params.ContainsKey(Keys.CommandParser.UserIdKey) 
                              && !CommandParser.Params.ContainsKey(Keys.CommandParser.UserKnownKey)
                    ? CommandParser.Params[Keys.CommandParser.UserIdKey]
                    : CommandParser.ParamsRaw.Split(' ')[0];

                return new BotMessage { Text = _phrasebook.IDontKnowXxxNamedYyy("nån",unknown) };
            }

            var saveInventories = SaveMode.None;
            var userInventory = _invMan.GetUserInventory();
            var targetInventory = _invMan.GetUserInventory(CommandParser.Params[Keys.CommandParser.UserIdKey]);

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

                    if (userInventory.Items[itemIndex].SoulBound)
                    {
                        _returnMsg = new BotMessage { Text = _phrasebook.CantMoveSoulboundItems() };
                        break;
                    }

                    targetInventory.Items.Add(userInventory.Items[itemIndex]);
                    userInventory.Items.RemoveAt(itemIndex);
                    saveInventories = SaveMode.Both;
                    _returnMsg = new BotMessage { Text = _phrasebook.ItemTransfered(targetInventory.Items.Last().Name) };
                    break;
                case GiveMode.CreateItem:
                    var item = CreateItem();

                    if (item is null)
                    {
                        CreateIdidntUnderstandMessage();
                        break;
                    }

                    targetInventory.Items.Add(item);
                    saveInventories = SaveMode.Target;
                    _returnMsg = new BotMessage { Text = _phrasebook.ItemCreated(item.Name) };
                    break;
                case GiveMode.NotSet:
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

        private Item CreateItem()
        {
            var itemData = CommandParser
                .Params[CreateItemParamKey]
                .Split(new[]{' '}, 2);

            if (itemData.Length != 2)
                return null;

            var type = typeof(GiveResponder)
                .GetTypeInfo()
                .Assembly
                .GetTypes()
                .FirstOrDefault(x => x.Name == itemData[0]);

            if (type is null)
                return null;

            try
            {
                return JsonConvert.DeserializeObject(itemData[1], type) as Item;
            }
            catch (Exception)
            {
                //todo craete exception viewer/log
                return null;
            }
        }

        private int GetItemIndex()
        {
            var index = -1;

            if (CommandParser.Params.ContainsKey(ItemParamKey))
            {
                if (!int.TryParse(CommandParser.Params[ItemParamKey], out index))
                {
                    CreateIdidntUnderstandMessage();
                }

                index--;
            }
            else if (CommandParser.Params.ContainsKey(Keys.CommandParser.UnnamedParam))
            {
                if (!int.TryParse(CommandParser.Params[Keys.CommandParser.UnnamedParam].Replace("sak ", ""),
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

            if (CommandParser.Params.ContainsKey(MoneyParamKey))
            {
                if (!decimal.TryParse(CommandParser.Params[MoneyParamKey], out amount))
                {
                    CreateIdidntUnderstandMessage();
                }
            }
            else if(CommandParser.Params.ContainsKey(Keys.CommandParser.UnnamedParam))
            {
                if (!decimal.TryParse(CommandParser.Params[Keys.CommandParser.UnnamedParam].Replace("kr",""), 
                    out amount))
                {
                    CreateIdidntUnderstandMessage();
                }
            }

            return amount;
        }

        private void SetMode()
        {
            if (CommandParser.Params.ContainsKey(MoneyParamKey)
                || CommandParser.Params.ContainsKey(Keys.CommandParser.UnnamedParam)
                && CommandParser.Params[Keys.CommandParser.UnnamedParam].Contains("kr"))
                _mode = GiveMode.GiveMoney;

            else if(CommandParser.Params.ContainsKey(ItemParamKey)
                    || CommandParser.Params.ContainsKey(Keys.CommandParser.UnnamedParam)
                    && CommandParser.Params[Keys.CommandParser.UnnamedParam].Contains("sak "))
                _mode = GiveMode.GiveItem;

            else if (CommandParser.Params.ContainsKey(CreateItemParamKey))
                _mode = GiveMode.CreateItem;
        }
        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "ge <användare> <Xkr|item_X>",
            Explatation = "Ger X pengar eller sak X till given användare."
        };

        enum GiveMode
        {
            NotSet,
            GiveMoney,
            GiveItem,
            CreateItem
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