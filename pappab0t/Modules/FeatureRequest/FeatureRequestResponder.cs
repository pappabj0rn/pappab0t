using System;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class FeatureRequestResponder : ResponderBase, IExposedCapability
    {
        public override bool CanRespond(ResponseContext context)
        {
            return context.Message.ChatHub.Type == SlackChatHubType.DM
                   && (
                    context.Message.Text.Equals("fr") 
                    || context.Message.Text.StartsWith("fr ")
                   )
                   || context.Message.MentionsBot
                   && (
                       context.Message.Text.EndsWith(" fr")
                       || context.Message.Text.Contains(" fr ")
                   );
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            throw new NotImplementedException();
        }

        public ExposedInformation Info { get; }
    }
}
