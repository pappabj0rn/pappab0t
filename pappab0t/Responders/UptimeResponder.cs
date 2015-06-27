using System;
using System.Text.RegularExpressions;
using MargieBot;
using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class UptimeResponder : IResponder, IExposedCapability
    {
        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM) &&
                   Regex.IsMatch(context.Message.Text, @"\bupptid\b", RegexOptions.IgnoreCase);
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var connectedSince = context.Get<Bot>().ConnectedSince.Value;
            var timeSpan = DateTime.Now - connectedSince;

            return new BotMessage { Text = "Jag har varit uppe sedan {0} {1}. \n{2} dagar\n{3} timmar\n{4} minuter\n{5} sekunder"
                                            .With(
                                                connectedSince.ToShortDateString(), 
                                                connectedSince.ToLongTimeString(), 
                                                timeSpan.Days,
                                                timeSpan.Hours,
                                                timeSpan.Minutes,
                                                timeSpan.Seconds
                                            )};
        }

        public ExposedInformation Info
        {
            get
            {
                return new ExposedInformation{Usage = "upptid", Explatation = "Visar när pappab0t senast startades."};
            }
        }
    }
}
