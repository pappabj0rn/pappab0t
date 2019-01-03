using System.Collections.Generic;
using MargieBot;
using Moq;
using pappab0t.Models;
using Raven.Client;
using Raven.Client.Embedded;

namespace pappab0t.Tests.Responders
{
    public abstract class ResponderTestsBase
    {
        protected Dictionary<string, string> UserNameCache;
        protected Dictionary<string, object> StaticContextItems;
        protected IDocumentStore Store;

        protected Mock<IPhrasebook> PhraseBookMock;

        protected ResponderTestsBase()
        {
            UserNameCache = new Dictionary<string, string> {{"U06BH8WTT", "eriska"}};
            StaticContextItems = new Dictionary<string, object>();

            PhraseBookMock = new Mock<IPhrasebook>();
            PhraseBookMock.SetupAllProperties();
            StaticContextItems.Add(
                Keys.StaticContextKeys.Phrasebook, 
                PhraseBookMock.Object);
        }

        protected ResponseContext CreateResponseContext(
            string text, 
            SlackChatHubType chatHubType, 
            bool mentionsBot = false,
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

            SetStaticContextItems(context);

            context.UserNameCache = UserNameCache ?? new Dictionary<string, string>();

            return context;
        }

        private void SetStaticContextItems(ResponseContext context)
        {
            if (StaticContextItems == null)
                return;

            foreach (var item in StaticContextItems)
            {
                context.Set(item.Key, item.Value);
            }
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

            SetStaticContextItems(context);
            context.UserNameCache = UserNameCache ?? new Dictionary<string, string>();
            return context;
        }

        protected void ConfigureRavenDB()
        {
            Store = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };

            Store.Initialize();

            StaticContextItems.Add("ravenStore", Store);
        }

        ~ResponderTestsBase()
        {
            Store?.Dispose();
        }
    }
}