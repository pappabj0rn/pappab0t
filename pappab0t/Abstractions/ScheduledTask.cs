namespace pappab0t.Abstractions
{
    public abstract class ScheduledTask
    {
        public const int Milliseconds = 1000;
        public abstract bool IsDue();
        public abstract bool Execute();
        public abstract int Interval { get; }
    }
}
