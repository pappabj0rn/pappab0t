using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Models;
using Raven.Client.Document;

namespace pappab0t.Responders
{
    public abstract class ResponderBase : IResponder
    {
        public abstract bool CanRespond(ResponseContext context);
        public abstract BotMessage GetResponse(ResponseContext context);

        protected ResponseContext Context;

        protected DocumentStore DocumentStore
        {
            get
            {
                return Context == null
                    ? null
                    : Context.Get<DocumentStore>();
            }
        }

        protected Phrasebook PhraseBook
        {
            get
            {
                return Context == null
                    ? null
                    : Context.Get<Phrasebook>();
            }
        }
    }
}