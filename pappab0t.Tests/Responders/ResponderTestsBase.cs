using System.Collections.Generic;
using MargieBot;

namespace pappab0t.Tests.Responders
{
    public abstract class ResponderTestsBase
    {
        protected Dictionary<string, string> UserNameCache;

        protected ResponderTestsBase()
        {
            UserNameCache = new Dictionary<string, string> { { "U06BH8WTT", "eriska" } };
        }

        protected ResponseContext CreateResponseContext(
            string text, 
            SlackChatHubType chatHubType, 
            bool mentionsBot = false, 
            IDictionary<string,object> staticContextItems = null,
            string userUUID = null)
        {
            var context = new ResponseContext
            {
                BotUserID = "botUUID",
                BotUserName = "pappab0t",
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
            context.Set(Keys.StaticContextKeys.Bot, new Bot
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

            context.UserNameCache = UserNameCache ?? new Dictionary<string, string>();

            return context;
        }

        protected ResponseContext CreateContext(
            string msg, 
            SlackChatHubType hubType = SlackChatHubType.Channel,
            bool? mentionsBot = null)
        {
            var context = CreateResponseContext(
                msg,
                hubType,
                mentionsBot: mentionsBot 
                             ?? msg.Contains("pbot")
                             || msg.Contains("pb0t")
                             || msg.Contains("pappab0t")
                             || msg.Contains("<@botUUID>"));

            context.UserNameCache = UserNameCache ?? new Dictionary<string, string>();
            return context;
        }
    }
}