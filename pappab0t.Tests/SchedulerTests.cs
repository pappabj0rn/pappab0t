using System;
using System.Timers;
using Moq;
using pappab0t.Abstractions;
using Xunit;

namespace pappab0t.Tests
{
    public class SchedulerTests
    {
        private readonly Scheduler _scheduler;
        private readonly TestTask _neverRunTask;
        private readonly TestTask _alwaysRunTask;

        private object lockObj = new object();
        private Mock<ITimer> _timerMock;

        public SchedulerTests()
        {
            _neverRunTask = new TestTask();
            _alwaysRunTask = new TestTask
            {
                DueFunc = () => true,
                ExecuteFunc = () => true
            };

            _timerMock = new Mock<ITimer>();

            _scheduler = new Scheduler(
                _timerMock.Object, 
                new[]
                {
                    _neverRunTask,
                    _alwaysRunTask
                })
            {
                Interval = 1
            };
        }

        [Fact]
        public void Run_should_set_timer_interval_and_start_timer()
        {
            int testInterval = 1337;
            _scheduler.Interval = testInterval;

            _scheduler.Run();

            _timerMock.VerifySet(x=>x.Interval=testInterval);
            _timerMock.Verify(x=>x.Start(),Times.Once);
        }

        [Fact]
        public void Stop_should_stop_the_timer()
        {
            _scheduler.Stop();

            _timerMock.Verify(x => x.Stop(), Times.Once);
        }

        [Fact]
        public void Should_check_if_task_is_due()
        {
            RaiseTimerEvent();

            Assert.True(_neverRunTask.DueChecked);
            Assert.True(_alwaysRunTask.DueChecked);
        }

        private void RaiseTimerEvent()
        {
            _timerMock.Raise(x => x.Elapsed += null, (ElapsedEventArgs) null);
        }

        [Fact]
        public void Should_execute_due_tasks()
        {
            RaiseTimerEvent();

            Assert.False(_neverRunTask.Executed);
            Assert.True(_alwaysRunTask.Executed);
        }
    }

    internal class TestTask : ScheduledTask
    {
        public Guid Id { get; set; }
        public bool DueChecked { get; private set; }
        public bool Executed { get; private set; }

        public Func<bool> DueFunc = () => false;
        public Func<bool> ExecuteFunc = () => false;

        public TestTask()
        {
            Id = Guid.NewGuid();
        }
        public override bool IsDue()
        {
            DueChecked = true;
            return DueFunc();
        }

        public override bool Execute()
        {
            Executed = true;
            return ExecuteFunc();
        }

        public override int Interval => 1;

        public void Reset()
        {
            DueChecked = false;
            Executed = false;
        }
    }
}
