using MargieBot;

namespace pappab0t.MessageHandler
{
    public interface IMessageHandler
    {
        void Execute(ResponseContext context);
    }
}