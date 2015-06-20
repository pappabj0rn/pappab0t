using System;
using MargieBot.Models;
using MargieBot.Responders;

namespace pappab0t.MessageHandler
{
    public abstract class MessageHandlerBase : IResponder
    {
        public bool CanRespond(ResponseContext context)
        {
            Act(context);
            return false;
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            throw new NotImplementedException("Message handlers doesn't implement GetResponse.");
        }

        protected abstract void Act(ResponseContext context);
    }
}
