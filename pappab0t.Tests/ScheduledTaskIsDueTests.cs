using System;
using pappab0t.Abstractions;
using Xunit;

namespace pappab0t.Tests
{
    public class ScheduledTaskIsDueTests
    {
        private DateTime _time;
        private readonly ScheduledTask _task;

        public ScheduledTaskIsDueTests()
        {
            _task = new IsDueTestTask();

            _time = new DateTime(2019,2,5,23,56,21);
            SystemTime.Now = () => _time; //todo replcace with injected. fails constantly
        }

        [Fact]
        public void Should_return_true_after_task_is_created()
        {
            Assert.True(_task.IsDue());
        }

        [Fact]
        public void Should_return_false_when_called_before_its_interval_after_Execution()
        {
            _task.Execute();

            _time = _time.AddMilliseconds(_task.Interval - 1);

            Assert.False(_task.IsDue());
        }

        [Fact]
        public void Should_return_true_when_called_after_its_interval_after_Execution()
        {
            _task.Execute();

            _time = _time.AddMilliseconds(_task.Interval + 1);

            Assert.True(_task.IsDue());
        }
    }

    internal class IsDueTestTask : ScheduledTask
    {
        public override int Interval => 10;
        public override bool Execute()
        {
            LastRunDate = SystemTime.Now();
            return true;
        }
    }
}