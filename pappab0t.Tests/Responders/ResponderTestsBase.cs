using System.Collections.Generic;
using MargieBot;

namespace pappab0t.Tests.Responders
{
    public abstract class ResponderTestsBase
    {
        protected static ResponseContext CreateResponseContext(
            string text, 
            SlackChatHubType chatHubType, 
            bool mentionsBot = false, 
            IDictionary<string,object> staticContextItems = null,
            string userUUID = null)
        {
            var context = new ResponseContext
            {
                BotUserID = "botUUID",
                BotUserName = "pbot",
                TeamID = "teamID",
                Message = new SlackMessage
                {
                    ChatHub = new SlackChatHub
                    {
                        ID = "hubID",
                        Name = "hubName",
                        Type = chatHubType
                    },
                    Text = text,
                    User = new SlackUser
                    {
                        ID = userUUID ?? "userUUID"
                    },
                    MentionsBot = mentionsBot
                }
            };
            context.Set("aliases",new Bot
            {
                Aliases = new []{"pbot","pb0t"}
            });

            if (staticContextItems != null)
            {
                foreach (var item in staticContextItems)
                {
                    context.Set(item.Key,item.Value);
                }
            }

            return context;
        }
    }
}