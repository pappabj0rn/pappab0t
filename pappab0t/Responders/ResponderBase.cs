using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Models;
using Raven.Client;

namespace pappab0t.Responders
{
    public abstract class ResponderBase : IResponder
    {
        protected ResponseContext Context;

        protected IDocumentStore DocumentStore
        {
            get
            {
                return Context == null
                    ? null
                    : Context.Get<IDocumentStore>();
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

        public abstract bool CanRespond(ResponseContext context);
        public abstract BotMessage GetResponse(ResponseContext context);
    }
}