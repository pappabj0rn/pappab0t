using System;
using MargieBot;

using pappab0t.Models;
using Raven.Client;

namespace pappab0t.Responders
{
    public abstract class ResponderBase : IResponder
    {
        protected ResponseContext Context;

        [Obsolete("use injection")]
        protected IDocumentStore DocumentStore => Context?.Get<IDocumentStore>();

        [Obsolete("use injection")]
        protected IPhrasebook PhraseBook => Context?.Get<IPhrasebook>();

        protected Bot Bot => Context?.Get<Bot>();

        public ICommandDataParser CommandDataParser { get; }
        public CommandData CommandData { get; private set; }

        public abstract bool CanRespond(ResponseContext context);
        public abstract BotMessage GetResponse(ResponseContext context);

        protected ResponderBase(ICommandDataParser commandDataParser = null)
        {
            CommandDataParser = commandDataParser ?? new CommandDataParser();
        }

        protected void Init(ResponseContext context)
        {
            Context = context;
            CommandDataParser.Context = Context;
            CommandData = CommandDataParser.Parse();
        }
    }
}