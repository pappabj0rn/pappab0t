using System;
using System.Text.RegularExpressions;
using MargieBot;

using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class WeekNumberResponder : IResponder, IExposedCapability
    {
        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.IsDirectMessage()) &&
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

        public ExposedInformation Info
        {
            get { return new ExposedInformation { Usage = "vecka", Explatation = "Visar veckonummer." }; }
        }
    }
}
