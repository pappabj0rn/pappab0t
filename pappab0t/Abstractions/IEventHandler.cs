using System;

namespace pappab0t.Abstractions
{
    public interface IEventHandler : IDisposable
    {
        void Initialize();
    }
}