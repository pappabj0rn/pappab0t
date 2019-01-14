using System.Text;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Models;
using pappab0t.Modules.Inventory;

namespace pappab0t.Responders
{
    public class DescribeResponder : ResponderBase, IExposedCapability
    {
        //todo usage of responder with another user should require admin privileges
        private readonly IInventoryManager _invMan;
        private readonly IPhrasebook _phrasebook;
        private const string ItemParamKey = "s";

        private BotMessage _returnMsg;

        public DescribeResponder(IInventoryManager invMan, IPhrasebook phrasebook, ICommandDataParser commandDataParser)
            : base(commandDataParser)
        {
            _invMan = invMan;
            _phrasebook = phrasebook;
        }

        public override bool CanRespond(ResponseContext context)
        {
            Init(context);

            return CommandData.Command == "beskriv";
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Init(context);
            _invMan.Context = context;

            //todo refactor help into base. some interface for the responder which enables -? and --help in base
            if (CommandData.Params.ContainsKey("?"))
            {
                var sb = new StringBuilder();
                sb.AppendLine("Beskrivning av kommando: beskriv");
                sb.AppendLine("Visar beskrivning av specificerad sak.");
                sb.AppendLine("ex: beskriv sak 1");
                sb.AppendLine("ex: beskriv -s 1");
                sb.AppendLine("Parametrar:");
                sb.AppendLine("s: [sak nr], för att se vad du har för saker, använd kommando i.");

                if (CommandData.Params.ContainsKey("a"))
                    sb.AppendLine("u: <user>");

                sb.AppendLine("?: Hjälp (denna text)");

                return new BotMessage
                {
                    Text = sb.ToString()
                };
            }

            if (UnknownUserSpecified())
            {
                return new BotMessage { Text = _phrasebook.IDontKnowXxxNamedYyy("nån", UserParam()) };
            }

            if (KnownUserSpecifiedButNoItem())
            {
                return new BotMessage{Text = _phrasebook.DescribeUser()};
            }

            var userInventory = KnownUserSpecified() 
                ? _invMan.GetUserInventory(UserParam()) 
                : _invMan.GetUserInventory();

            int itemIndex = GetItemIndex();
            if (_returnMsg != null)
                return _returnMsg;

            if (itemIndex == -1)
            {
                return new BotMessage { Text = _phrasebook.Impossible() };
            }

            if (itemIndex > userInventory.Items.Count - 1)
            {
                return new BotMessage { Text = _phrasebook.DesribeItemToFewItems() };
            }

            var item = userInventory.Items[itemIndex];

            return new BotMessage
            {
                Text = _phrasebook.ItemDescription(
                    item.GetFriendlyTypeName(), 
                    item.GetDescription())
            };
        }

        private string UserParam()
        {
            return CommandData.Params[Keys.CommandParser.UserIdKey];
        }

        private bool KnownUserSpecified()
        {
            return CommandData.Params.ContainsKey(Keys.CommandParser.UserIdKey)
                   && CommandData.Params.ContainsKey(Keys.CommandParser.UserKnownKey);
        }

        private bool KnownUserSpecifiedButNoItem()
        {
            return KnownUserSpecified()
                   && !(CommandData.Params.ContainsKey(Keys.CommandParser.UnnamedParam)
                        || CommandData.Params.ContainsKey(ItemParamKey));
        }

        private bool UnknownUserSpecified()
        {
            return CommandData.Params.ContainsKey(Keys.CommandParser.UserIdKey)
                   && !CommandData.Params.ContainsKey(Keys.CommandParser.UserKnownKey);
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
            else if (UnnamedParamSpecified())
            {
                if (!int.TryParse(UnnamedParam().Replace("sak ", ""),
                    out index))
                {
                    CreateIdidntUnderstandMessage();
                }

                index--;
            }

            return index;
        }

        private string UnnamedParam()
        {
            return CommandData.Params[Keys.CommandParser.UnnamedParam];
        }

        private bool UnnamedParamSpecified()
        {
            return CommandData.Params.ContainsKey(Keys.CommandParser.UnnamedParam);
        }

        private void CreateIdidntUnderstandMessage()
        {
            _returnMsg = new BotMessage { Text = _phrasebook.IDidntUnderstand() };
        }

        public ExposedInformation Info => new ExposedInformation
        {
            Explatation = "Visar beskrivning av specificerad sak.",
            Usage = "beskriv <sakNr>"
        };
    }
}
