using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Abstractions;
using pappab0t.Models;
using pappab0t.Modules.Inventory;
using pappab0t.Responders;
using Raven.Client;
using Raven.Client.Embedded;

namespace pappab0t.Tests.Responders
{
    public abstract class ResponderTestsContext//todo inherit TestContext?
    {
        protected Dictionary<string, string> UserNameCache;
        protected string PappaBj0rnUserId => UserNameCache.First().Key;
        protected string EriskaUserId => UserNameCache.Last().Key;
        protected Dictionary<string, object> StaticContextItems;
        protected IDocumentStore Store;

        protected Mock<IPhrasebook> PhraseBookMock;
        protected Mock<IInventoryManager> InventoryManagerMock;
        protected ICommandDataParser CommandDataParser;
        protected Mock<IBot> BotMock;

        // ReSharper disable once InconsistentNaming
        protected Inventory PappaBj0rnInvetory;
        protected Inventory EriskaInvetory;

        protected IResponder Responder;
        protected bool InventoryManagerContextSet;

        protected ResponderTestsContext()
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
            SetupBot();

            CommandDataParser = new CommandDataParser();
        }

        private void SetupInventoryManagerAndDefaultInventories()
        {
            PappaBj0rnInvetory = new Inventory
            {
                Id = "inventories/1",
                UserId = UserNameCache.First().Key,
                BEK = 100M
            };

            EriskaInvetory = new Inventory
            {
                Id = "inventories/2",
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
                .Returns(PappaBj0rnInvetory);

            InventoryManagerMock
                .Setup(x => x.GetUserInventory(PappaBj0rnInvetory.UserId))
                .Returns(PappaBj0rnInvetory);

            InventoryManagerMock
                .Setup(x => x.GetUserInventory(EriskaInvetory.UserId))
                .Returns(EriskaInvetory);

            InventoryManagerMock
                .Setup(x => x.MoveItemByIndex(It.IsAny<int>(), It.IsAny<string>()))
                .Callback((int i, string userId) =>
                {
                    var item = PappaBj0rnInvetory.Items[i];
                    PappaBj0rnInvetory.Items.RemoveAt(i);
                    EriskaInvetory.Items.Add(item);
                });
        }
        private void SetupBot()
        {
            BotMock = new Mock<IBot>();
        }

        protected ResponseContext CreateContext(
            string msg, 
            SlackChatHubType hubType = SlackChatHubType.Channel,
            bool? mentionsBot = null,
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
                        Type = hubType
                    },
                    Text = msg,
                    User = new SlackUser
                    {
                        ID = userUUID ?? UserNameCache.First().Key
                    },
                    MentionsBot = mentionsBot
                                  ?? msg.ToLower().Contains("pbot")
                                  || msg.ToLower().Contains("pb0t")
                                  || msg.ToLower().Contains("pappab0t")
                                  || msg.ToLower().Contains("<@botuuid>")
                }
            };

            context.Set(Keys.StaticContextKeys.Bot, new Bot
            {
                Aliases = new[] { "pbot", "pb0t" }
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

        protected void ConfigureRavenDB()
        {
            Store = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };

            Store.Initialize();

            StaticContextItems.Add("ravenStore", Store);
        }

        ~ResponderTestsContext()
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