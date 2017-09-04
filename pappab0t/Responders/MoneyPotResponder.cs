using System;
using System.Linq;
using System.Text.RegularExpressions;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class MoneyPotResponder : ResponderBase, IExposedCapability
    {
        private const string PotNameKey = "potName";
        private const string HighScoreRegex = @"(?:\bpott\b|\bp\b)(\s+(?<" + PotNameKey + @">\w+))?";

        private string _potName;

        public override bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM)
                   && Regex.IsMatch(context.Message.Text, HighScoreRegex, RegexOptions.IgnoreCase);
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;

            var match = Regex.Match(Context.Message.Text, HighScoreRegex, RegexOptions.IgnoreCase);
            _potName = match.Groups[PotNameKey].Value;

            if(_potName.IsNullOrEmpty())
                return new BotMessage { Text = "Du måste ange vilken pott som skall visas." };

            using (var session = DocumentStore.OpenSession())
            {
                var pot = session.Query<MoneyPot>()
                                 .FirstOrDefault(x=>x.Name.Equals(_potName, StringComparison.InvariantCultureIgnoreCase));

                if(pot == null)
                    return new BotMessage{ Text = PhraseBook.IDontKnowXxxNamedYyyFormat().With("nån pott", _potName)};

                return new BotMessage{Text = "{0}-potten är uppe i {1}kr.".With(_potName, pot.BEK)};
            }
        }

        public ExposedInformation Info
        {
            get
            {
                return new ExposedInformation
                {
                    Usage = "pott|p <pottnamn>",
                    Explatation = "Visar potten med givet namn."
                };
            }
        }
    }
}