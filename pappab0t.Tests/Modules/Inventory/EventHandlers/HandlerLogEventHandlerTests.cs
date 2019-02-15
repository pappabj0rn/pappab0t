using System.Linq;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.EventHandlers;
using pappab0t.Modules.Inventory.Items;
using pappab0t.Modules.Inventory.Items.Modifiers;
using pappab0t.Modules.Inventory.Items.Tokens;
using Xunit;

namespace pappab0t.Tests.Modules.Inventory.EventHandlers
{
    public class HandlerLogEventHandlerTests : TestContext
    {
        private readonly Item _note;
        private readonly HandlerLog _log;

        public HandlerLogEventHandlerTests()
        {
            _note = new Item(new Novelty(), new NoteType());
            _log = new HandlerLog();
            _log.Add(PappaBj0rnUserId);

            _note.Modifiers.Add(new Expires { DateTime = SystemTime.Now().AddDays(1) });
            _note.Modifiers.Add(_log);

            Pappabj0rnInvetory.Items.Add(_note);
        }

        [Fact]
        public void Should_add_target_user_to_handler_log_when_a_timed_bomb_is_moved()
        {
            using (var evtHandler = new HandlerLogEventHandler())
            {
                evtHandler.Initialize();
                
                MessageBus<ItemMovingMessage>.Instance.SendMessage(
                    this,
                    new ItemMovingMessage(
                        _note, 
                        Pappabj0rnInvetory, 
                        EriskaInvetory));

                Assert.Equal(2,_log.LogEntries.Count);
                Assert.Equal(1, _log.LogEntries.Count(x=>x.UserId == PappaBj0rnUserId));
                Assert.Equal(1, _log.LogEntries.Count(x=>x.UserId == EriskaUserId));
            }
        }

        [Fact]
        public void Initialize_should_not_resubscribe_to_events()
        {
            using (var evtHandler = new HandlerLogEventHandler())
            {
                evtHandler.Initialize();
                evtHandler.Initialize();

                MessageBus<ItemMovingMessage>.Instance.SendMessage(
                    this,
                    new ItemMovingMessage(
                        _note,
                        Pappabj0rnInvetory,
                        EriskaInvetory));

                Assert.Equal(2, _log.LogEntries.Count);
                Assert.Equal(1, _log.LogEntries.Count(x => x.UserId == PappaBj0rnUserId));
                Assert.Equal(1, _log.LogEntries.Count(x => x.UserId == EriskaUserId));
            }
        }

        [Fact]
        public void Should_unsubscribe_from_events_on_dispose()
        {
            using (var evtHandler = new HandlerLogEventHandler())
            {
                evtHandler.Initialize();
            } // dispose
            
            MessageBus<ItemMovingMessage>.Instance.SendMessage(
                this,
                new ItemMovingMessage(
                    _note,
                    Pappabj0rnInvetory,
                    EriskaInvetory));

            Assert.Equal(1, _log.LogEntries.Count);
        }
    }
}