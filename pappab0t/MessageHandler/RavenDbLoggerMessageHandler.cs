using MargieBot.Models;
using Raven.Client;

namespace pappab0t.MessageHandler
{
    public class RavenDbLoggerMessageHandler : MessageHandlerBase
    {
        protected override void Act(ResponseContext context)
        {
            var store = context.Get<IDocumentStore>();

            using (var session = store.OpenSession())
            {
                session.Store(context.Message);
                session.SaveChanges();
            }
        }
    }
}
