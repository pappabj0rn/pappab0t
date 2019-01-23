using System.Collections.Generic;
using System.Linq;
using pappab0t.Abstractions;

namespace pappab0t
{
    public class Scheduler : IScheduler
    {
        private readonly IEnumerable<ScheduledTask> _scheduledTasks;
        private readonly ITimer _timer;

        public Scheduler(ITimer timer, IEnumerable<ScheduledTask> scheduledTasks)
        {
            _timer = timer;
            _scheduledTasks = scheduledTasks;

            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var task in _scheduledTasks.Where(t=>t.IsDue()))
            {
                task.Execute();
            }
        }

        public int Interval { get; set; }

        public void Run()
        {
            _timer.Interval = Interval;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}