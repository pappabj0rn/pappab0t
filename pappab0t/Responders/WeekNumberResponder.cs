using System;
using System.Text.RegularExpressions;
using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Abstractions;
using pappab0t.Extensions;

namespace pappab0t.Responders
{
    public class WeekNumberResponder : IResponder, IExposedCapability
    {
        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM) &&
                   Regex.IsMatch(context.Message.Text, @"\bvecka\b", RegexOptions.IgnoreCase);
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            string[] formatStrings =
            {
                "Nu är det vecka {0}.",
                "{0}",
                "v{0}",
                "det är vecka {0}",
                "Det är vecka {0}."
            };

            return new BotMessage 
            {
                Text = formatStrings.Random()
                                    .With(DateTime.Now.Iso8601Week()) 
            };
        }

        public string Usage
        {
            get { return "vecka\n>Visar veckonummer."; }
        }
    }
}
