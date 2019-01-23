namespace pappab0t.Abstractions
{
    public interface IScheduler
    {
        int Interval { get; set; }
        void Run();
        void Stop();
    }
}