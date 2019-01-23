using System.Timers;

namespace pappab0t.Abstractions
{
    public interface ITimer
    {
        void Start();
        void Stop();
        double Interval { get; set; }
        event ElapsedEventHandler Elapsed;
    }
}
