using System;
using System.Linq;
using System.Text;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class MoneyPotResponder : ResponderBase, IExposedCapability
    {
        private string _potName;

        public override bool CanRespond(ResponseContext context)
        {
            Init(context);

            return CommandData.Command == "pott"
                   || CommandData.Command == "p";
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Init(context);

            _potName = "";
            if (CommandData.Params.ContainsKey(Keys.CommandParser.UnnamedParam))
                _potName = CommandData.Params[Keys.CommandParser.UnnamedParam];

            if(_potName == "")
                using (var session = DocumentStore.OpenSession())
                {
                    var pots = session.Query<MoneyPot>();

                    var sb = new StringBuilder();
                    sb.AppendLine("Nuvarande potter:");

                    foreach (var pot in pots)
                    {
                        sb.AppendLine($"> {pot.Name} {pot.BEK:C}");
                    }

                    return new BotMessage{Text = sb.ToString()};
                }

            using (var session = DocumentStore.OpenSession())
            {
                var pot = session.Query<MoneyPot>()
                                 .FirstOrDefault(x=>x.Name.Equals(_potName, StringComparison.InvariantCultureIgnoreCase));

                return pot == null 
                    ? new BotMessage
                    {
                        Text = PhraseBook.IDontKnowXxxNamedYyy("nån pott", _potName)
                    } 
                    : new BotMessage
                    {
                        Text = $"{_potName}: {pot.BEK:C}"
                    };
            }
        }

        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "pott|p [pottnamn]",
            Explatation = "Visar potter, eller potten med givet namn."
        };
    }
}