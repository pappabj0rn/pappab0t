using MargieBot;

using pappab0t.Models;
using Raven.Client;

namespace pappab0t.Responders
{
    public abstract class ResponderBase : IResponder
    {
        protected ResponseContext Context;

        protected IDocumentStore DocumentStore => Context?.Get<IDocumentStore>();

        protected Phrasebook PhraseBook => Context?.Get<Phrasebook>();

        protected Bot Bot => Context?.Get<Bot>();

        public abstract bool CanRespond(ResponseContext context);
        public abstract BotMessage GetResponse(ResponseContext context);
    }
}