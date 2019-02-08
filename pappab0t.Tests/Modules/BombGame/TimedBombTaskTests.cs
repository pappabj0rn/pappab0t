using System;
using MargieBot;
using Moq;
using pappab0t.Models;
using pappab0t.Modules.BombGame;
using pappab0t.Modules.BombGame.Items;
using pappab0t.Modules.Inventory.Items.Modifiers;
using Xunit;

namespace pappab0t.Tests.Modules.BombGame
{
    public abstract class TimedBombTaskTests : TestContext
    {
        private readonly TimedBombTask _task;

        protected TimedBombTaskTests()
        {
            _task = new TimedBombTask(
                BotMock.Object, 
                InventoryManagerMock.Object, 
                HightscoreManagerMock.Object, 
                PhrasebookMock.Object);

            var tb1 = new TimedBomb();
            var tbHandlerLog = new HandlerLog();

            tb1.Modifiers.Add(tbHandlerLog);
            tb1.Modifiers.Add(new Expires
            {
                DateTime = SystemTime
                    .Now()
                    .AddDays(-7)
            });

            tbHandlerLog.Add(PappaBj0rnUserId);
            tbHandlerLog.Add(EriskaUserId);
            tbHandlerLog.Add(PappaBj0rnUserId);
            tbHandlerLog.Add(EriskaUserId);
            tbHandlerLog.Add(PappaBj0rnUserId);

            Pappabj0rnInvetory.Items.Add(tb1);


            var tb2 = new TimedBomb();
            var tb2HandlerLog = new HandlerLog();

            tb2.Modifiers.Add(tbHandlerLog);
            tb2.Modifiers.Add(new Expires
            {
                DateTime = SystemTime
                    .Now()
                    .AddDays(2)
            });

            tb2HandlerLog.Add(EriskaUserId);

            EriskaInvetory.Items.Add(tb2);
        }

        public class Execute : TimedBombTaskTests
        {
            [Fact]
            public void Should_search_all_inventories_for_expired_timed_bombs()
            {
                _task.Execute();

                InventoryManagerMock.Verify(x => x.GetAll(), Times.Once);
            }

            [Fact]//todo observed to fail: 1 time
            public void Should_score_total_log_count_to_current_owner_and_total_individual_log_count_to_involved_users()
            {
                _task.Execute();

                HightscoreManagerMock.Verify(x => x.Handle("TimedBomb", PappaBj0rnUserId, 5), Times.Once);
                HightscoreManagerMock.Verify(x => x.Handle("TimedBomb", PappaBj0rnUserId, 3), Times.Never);
                HightscoreManagerMock.Verify(x => x.Handle("TimedBomb", EriskaUserId, 2), Times.Once);
            }

            [Fact]
            public void Should_remove_expired_bomb()
            {
                _task.Execute();

                Assert.Empty(Pappabj0rnInvetory.Items);
            }

            [Fact]
            public void Should_return_false_when_task_is_not_due()
            {
                var time = new DateTime(2019,2,7,2,49,11);
                SystemTime.Now = () => time;

                _task.Execute();

                SystemTime.Now = () => time.AddMilliseconds(_task.Interval-1);

                Assert.False(_task.Execute());
            }

            [Fact]
            public void Should_return_true_when_task_is_due()
            {
                Assert.True(_task.Execute());
            }

            [Fact]
            public void Should_announce_when_a_bomb_expires()
            {
                BotMessage botMsg=null;
                BotMock.Setup(x => x.Say(It.IsAny<BotMessage>()))
                    .Callback((BotMessage msg) => botMsg = msg);

                _task.Execute();
                
                Assert.Equal(nameof(IPhrasebook.TimedBombExpired), botMsg.Text);
            }
        }
    }
}