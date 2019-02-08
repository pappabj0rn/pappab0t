using System;

namespace pappab0t.Abstractions
{
    public abstract class ScheduledTask
    {
        protected DateTime LastRunDate;

        protected const int Milliseconds = 1000;

        public abstract int Interval { get; }

        public virtual bool IsDue()
        {
            return SystemTime.Now() > LastRunDate.AddMilliseconds(Interval);
        }
        public abstract bool Execute();
    }
}
