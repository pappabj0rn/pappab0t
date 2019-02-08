using System;

namespace pappab0t
{
    public class MessageBusEventArgs<T> : EventArgs
    {
        public MessageBusEventArgs(T message)
        {
            Message = message;
        }

        public T Message { get; }
    }

    public class MessageBus<T>
    {
        private static MessageBus<T> _instance;
        private static readonly object Lock = new object();

        protected MessageBus()
        {
        }

        public static MessageBus<T> Instance
        {
            get
            {
                lock (Lock)
                {
                    return _instance ?? (_instance = new MessageBus<T>());
                }
            }
        }

        public event EventHandler<MessageBusEventArgs<T>> MessageRecieved;

        public void SendMessage(object sender, T message)
        {
            MessageRecieved?.Invoke(sender, new MessageBusEventArgs<T>(message));
        }
    }
}