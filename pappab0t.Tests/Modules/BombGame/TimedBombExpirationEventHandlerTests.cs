using MargieBot;
using Moq;
using pappab0t.Models;
using pappab0t.Modules.BombGame;
using pappab0t.Modules.BombGame.Items;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items;
using pappab0t.Modules.Inventory.Items.Modifiers;
using pappab0t.Modules.Inventory.Items.Tokens;
using Xunit;

namespace pappab0t.Tests.Modules.BombGame
{
    public class TimedBombExpirationEventHandlerTests : TestContext
    {
        private Item _expiredTimedBomb;

        public TimedBombExpirationEventHandlerTests()
        {
            CreateExpiredTimedBomb();
            Pappabj0rnInvetory.Items.Add(_expiredTimedBomb);
        }

        private void CreateExpiredTimedBomb()
        {
            _expiredTimedBomb = new Item(new Novelty(), new TimedBombType());

            var log = new HandlerLog();
            log.Add(PappaBj0rnUserId);
            log.Add(EriskaUserId);
            log.Add(PappaBj0rnUserId);
            log.Add(EriskaUserId);
            log.Add(PappaBj0rnUserId);

            _expiredTimedBomb.Modifiers.Add(log);
            _expiredTimedBomb.Modifiers.Add(new Expires { DateTime = SystemTime.Now().AddDays(-1) });
        }

        private TimedBombExpirationEventHandler CreateEvtHandler()
        {
            return new TimedBombExpirationEventHandler(
                BotMock.Object,
                InventoryManagerMock.Object,
                HightscoreManagerMock.Object,
                PhrasebookMock.Object);
        }

        [Fact]
        public void Should_score_total_log_count_to_current_owner_and_total_individual_log_count_to_involved_users()
        {
            using (var evtHandler = CreateEvtHandler())
            {
                evtHandler.Initialize();
                
                MessageBus<ItemExpiredMessage>.Instance.SendMessage(
                    this,
                    new ItemExpiredMessage(
                        _expiredTimedBomb,
                        Pappabj0rnInvetory));

                HightscoreManagerMock.Verify(x => x.Handle(
                    TimedBombExpirationEventHandler.HighScoreName, 
                    PappaBj0rnUserId, 
                    5), 
                    Times.Once);

                HightscoreManagerMock.Verify(x => x.Handle(
                    TimedBombExpirationEventHandler.HighScoreName, 
                    PappaBj0rnUserId, 
                    3), 
                    Times.Never);

                HightscoreManagerMock.Verify(x => x.Handle(
                    TimedBombExpirationEventHandler.HighScoreName, 
                    EriskaUserId, 
                    2), 
                    Times.Once);
            }
        }

        [Fact]
        public void Should_remove_expired_bomb()
        {
            using (var evtHandler = CreateEvtHandler())
            {
                evtHandler.Initialize();

                MessageBus<ItemExpiredMessage>.Instance.SendMessage(
                    this,
                    new ItemExpiredMessage(
                        _expiredTimedBomb,
                        Pappabj0rnInvetory));

                Assert.Empty(Pappabj0rnInvetory.Items);
            }
        }

        [Fact]
        public void Initialize_should_not_resubscribe_to_events()
        {
            using (var evtHandler = CreateEvtHandler())
            {
                evtHandler.Initialize();
                evtHandler.Initialize();

                MessageBus<ItemExpiredMessage>.Instance.SendMessage(
                    this,
                    new ItemExpiredMessage(
                        _expiredTimedBomb,
                        Pappabj0rnInvetory));

                HightscoreManagerMock.Verify(x => x.Handle(
                    TimedBombExpirationEventHandler.HighScoreName, 
                    PappaBj0rnUserId, 
                    5), 
                    Times.Once);
            }
        }

        [Fact]
        public void Should_announce_when_a_bomb_expires()
        {
            using (var evtHandler = CreateEvtHandler())
            {
                evtHandler.Initialize();

                BotMessage botMsg = null;
                BotMock.Setup(x => x.Say(It.IsAny<BotMessage>()))
                    .Callback((BotMessage msg) => botMsg = msg);

                MessageBus<ItemExpiredMessage>.Instance.SendMessage(
                    this,
                    new ItemExpiredMessage(
                        _expiredTimedBomb,
                        Pappabj0rnInvetory));

                Assert.Equal(nameof(IPhrasebook.TimedBombExpired), botMsg.Text);
            }
        }

        [Fact]
        public void Should_only_handle_timedbombs()
        {
            using (var evtHandler = CreateEvtHandler())
            {
                evtHandler.Initialize();

                _expiredTimedBomb.Type = new NoteType();

                MessageBus<ItemExpiredMessage>.Instance.SendMessage(
                    this,
                    new ItemExpiredMessage(
                        _expiredTimedBomb,
                        Pappabj0rnInvetory));

                HightscoreManagerMock.Verify(x => x.Handle(
                    It.IsAny<string>(), 
                    It.IsAny<string>(), 
                    It.IsAny<int>()), 
                    Times.Never);
            }
        }

        [Fact]
        public void Should_unsubscribe_from_events_on_dispose()
        {
            using (var evtHandler = CreateEvtHandler())
            {
                evtHandler.Initialize();
            } // dispose

            MessageBus<ItemExpiredMessage>.Instance.SendMessage(
                this,
                new ItemExpiredMessage(
                    _expiredTimedBomb,
                    Pappabj0rnInvetory));

            HightscoreManagerMock.Verify(x => x.Handle(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>()),
                Times.Never);
        }
    }
}