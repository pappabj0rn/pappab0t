using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Models;
using pappab0t.Modules.Inventory;
using pappab0t.Responders;
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
        protected Mock<IInventoryManager> InventoryManagerMock;
        protected ICommandDataParser CommandDataParser;

        // ReSharper disable once InconsistentNaming
        protected Inventory Pappabj0rnInvetory;
        protected Inventory EriskaInvetory;

        protected IResponder Responder;
        protected bool InventoryManagerContextSet;

        protected ResponderTestsBase()
        {
            UserNameCache = new Dictionary<string, string>
            {
                {"U06BHPNJG", "pappabj0rn"},
                {"U06BH8WTT", "eriska"}
            };

            StaticContextItems = new Dictionary<string, object>();

            SetupPhrasebookToReturnMethodNames();

            SetupInventoryManagerAndDefaultInventories();

            StaticContextItems.Add(
                Keys.StaticContextKeys.Phrasebook, 
                PhraseBookMock.Object);

            CommandDataParser = new CommandDataParser();
        }

        private void SetupInventoryManagerAndDefaultInventories()
        {
            Pappabj0rnInvetory = new Inventory
            {
                Id = "Inventories/1",
                UserId = UserNameCache.First().Key,
                BEK = 100M
            };

            EriskaInvetory = new Inventory
            {
                Id = "Inventories/2",
                UserId = UserNameCache.Last().Key,
                BEK = 100M
            };

            InventoryManagerMock = new Mock<IInventoryManager>();

            InventoryManagerContextSet = false;

            InventoryManagerMock
                .SetupSet(x => x.Context = It.IsAny<ResponseContext>())
                .Callback(() => InventoryManagerContextSet = true);

            InventoryManagerMock
                .Setup(x => x.GetUserInventory())
                .Callback(() =>
                {
                    if(!InventoryManagerContextSet)
                        throw new Exception("Context not set.");
                })
                .Returns(Pappabj0rnInvetory);

            InventoryManagerMock
                .Setup(x => x.GetUserInventory(Pappabj0rnInvetory.UserId))
                .Returns(Pappabj0rnInvetory);

            InventoryManagerMock
                .Setup(x => x.GetUserInventory(EriskaInvetory.UserId))
                .Returns(EriskaInvetory);
        }

        //todo merge
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
                        ID = userUUID ?? UserNameCache.First().Key
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

        protected void SetupPhrasebookToReturnMethodNames()
        {
            PhraseBookMock = new Mock<IPhrasebook>
            {
                DefaultValueProvider = new MethodNameDefaultValueProvider()
            };
        }
    }
}

internal class MethodNameDefaultValueProvider : LookupOrFallbackDefaultValueProvider
{
    public MethodNameDefaultValueProvider()
    {
        Register(typeof(string), (type, mock) => mock.Invocations.Last().Method.Name);
    }
}