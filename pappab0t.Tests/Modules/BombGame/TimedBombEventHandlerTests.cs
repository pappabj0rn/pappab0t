using System.Linq;
using pappab0t.Modules.BombGame;
using pappab0t.Modules.BombGame.Items;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items.Modifiers;
using Xunit;

namespace pappab0t.Tests.Modules.BombGame
{
    public class TimedBombEventHandlerTests : TestContext
    {
        [Fact]
        public void Should_add_target_user_to_handler_log_when_a_timed_bomb_is_moved()
        {
            using (var evtHandler = new TimedBombEventHandler())
            {
                evtHandler.Initialize();

                var tb = new TimedBomb();
                var log = new HandlerLog();
                log.Add(PappaBj0rnUserId);

                tb.Modifiers.Add(new Expires{DateTime = SystemTime.Now().AddDays(1)});
                tb.Modifiers.Add(log);

                Pappabj0rnInvetory.Items.Add(tb);
                
                MessageBus<ItemMovingMessage>.Instance.SendMessage(
                    this,
                    new ItemMovingMessage(
                        tb, 
                        Pappabj0rnInvetory, 
                        EriskaInvetory));

                Assert.Equal(2,log.LogEntries.Count());
                Assert.Equal(1, log.LogEntries.Count(x=>x.UserId == PappaBj0rnUserId));
                Assert.Equal(1, log.LogEntries.Count(x=>x.UserId == EriskaUserId));
            }
        }

        [Fact]
        public void Initialize_should_not_resubscribe_to_events()
        {
            using (var evtHandler = new TimedBombEventHandler())
            {
                evtHandler.Initialize();
                evtHandler.Initialize();

                var tb = new TimedBomb();
                var log = new HandlerLog();
                log.Add(PappaBj0rnUserId);

                tb.Modifiers.Add(new Expires { DateTime = SystemTime.Now().AddDays(1) });
                tb.Modifiers.Add(log);

                Pappabj0rnInvetory.Items.Add(tb);

                MessageBus<ItemMovingMessage>.Instance.SendMessage(
                    this,
                    new ItemMovingMessage(
                        tb,
                        Pappabj0rnInvetory,
                        EriskaInvetory));

                Assert.Equal(2, log.LogEntries.Count());
                Assert.Equal(1, log.LogEntries.Count(x => x.UserId == PappaBj0rnUserId));
                Assert.Equal(1, log.LogEntries.Count(x => x.UserId == EriskaUserId));
            }
        }
    }
}