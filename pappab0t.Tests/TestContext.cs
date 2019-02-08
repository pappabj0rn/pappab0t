using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Abstractions;
using pappab0t.Models;
using pappab0t.Modules.Highscore;
using pappab0t.Modules.Inventory;

namespace pappab0t.Tests
{
    public abstract class TestContext
    {
        protected Dictionary<string, string> UserNameCache;
        protected string PappaBj0rnUserId => UserNameCache.First().Key;
        protected string EriskaUserId => UserNameCache.Last().Key;

        protected Mock<IPhrasebook> PhrasebookMock = new Mock<IPhrasebook>();
        protected Mock<IInventoryManager> InventoryManagerMock;
        protected Mock<IHighScoreManager> HightscoreManagerMock;
        protected Mock<IBot> BotMock;

        // ReSharper disable once InconsistentNaming
        protected Inventory Pappabj0rnInvetory;
        protected Inventory EriskaInvetory;
        protected bool InventoryManagerContextSet;
        protected readonly Mock<Random> RandomMock;

        protected TestContext()
        {
            UserNameCache = new Dictionary<string, string>
            {
                {"U06BHPNJG", "pappabj0rn"},
                {"U06BH8WTT", "eriska"}
            };

            SetupPhrasebookToReturnMethodNames();
            SetupInventoryManagerAndDefaultInventories();
            SetupHighscoreManager();
            SetupBot();

            RandomMock = new Mock<Random>();
        }

        protected void SetupPhrasebookToReturnMethodNames()
        {
            PhrasebookMock = new Mock<IPhrasebook>
            {
                DefaultValueProvider = new MethodNameDefaultValueProvider()
            };
        }

        private void SetupInventoryManagerAndDefaultInventories()
        {
            Pappabj0rnInvetory = new Inventory
            {
                Id = "Inventories/1",
                UserId = PappaBj0rnUserId,
                BEK = 100M
            };

            EriskaInvetory = new Inventory
            {
                Id = "Inventories/2",
                UserId = EriskaUserId,
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
                    if (!InventoryManagerContextSet)
                        throw new Exception("Context not set.");
                })
                .Returns(() => Pappabj0rnInvetory.Clone());

            InventoryManagerMock
                .Setup(x => x.GetUserInventory(Pappabj0rnInvetory.UserId))
                .Returns(() => Pappabj0rnInvetory.Clone());

            InventoryManagerMock
                .Setup(x => x.GetUserInventory(EriskaInvetory.UserId))
                .Returns(() => EriskaInvetory.Clone());

            InventoryManagerMock
                .Setup(x => x.GetAll())
                .Returns(() => new[] {Pappabj0rnInvetory.Clone(), EriskaInvetory.Clone() });

            InventoryManagerMock
                .Setup(x => x.Save(It.Is<Inventory>(i => i.Id == Pappabj0rnInvetory.Id)))
                .Callback<Inventory>(x => Pappabj0rnInvetory = x);

            InventoryManagerMock
                .Setup(x => x.Save(It.Is<Inventory>(i => i.Id == EriskaInvetory.Id)))
                .Callback<Inventory>(x => EriskaInvetory = x);

            InventoryManagerMock
                .Setup(x => x.Save(It.IsAny<IEnumerable<Inventory>>()))
                .Callback<IEnumerable<Inventory>>(x =>
                {
                    foreach (var i in x)
                    {
                        if (i.Id == Pappabj0rnInvetory.Id)
                            Pappabj0rnInvetory = i;

                        if (i.Id == EriskaInvetory.Id)
                            EriskaInvetory = i;
                    }
                });
        }

        private void SetupHighscoreManager()
        {
            HightscoreManagerMock = new Mock<IHighScoreManager>();
        }

        private void SetupBot()
        {
            BotMock = new Mock<IBot>();
        }

        protected ResponseContext CreateContext(
            string msg,
            SlackChatHubType hubType = SlackChatHubType.Channel,
            bool? mentionsBot = null,
            string userUuid = null)
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
                        ID = userUuid ?? UserNameCache.First().Key
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
            
            context.UserNameCache = UserNameCache ?? new Dictionary<string, string>();

            return context;
        }
    }
}