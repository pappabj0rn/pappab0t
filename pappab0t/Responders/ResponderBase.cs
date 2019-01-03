using MargieBot;

using pappab0t.Models;
using Raven.Client;

namespace pappab0t.Responders
{
    public abstract class ResponderBase : IResponder
    {
        protected ResponseContext Context;

        protected IDocumentStore DocumentStore => Context?.Get<IDocumentStore>();

        protected IPhrasebook PhraseBook => Context?.Get<IPhrasebook>();

        protected Bot Bot => Context?.Get<Bot>();

        public ICommandParser CommandParser { get; }

        public abstract bool CanRespond(ResponseContext context);
        public abstract BotMessage GetResponse(ResponseContext context);

        protected ResponderBase()
        {
            CommandParser = new CommandParser();
        }

        protected void Init(ResponseContext context)
        {
            Context = context;
            CommandParser.Context = Context;
            CommandParser.Parse();
        }
    }
}