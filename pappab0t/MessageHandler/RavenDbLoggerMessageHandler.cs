using System;
using MargieBot.Models;
using Raven.Client;

namespace pappab0t.MessageHandler
{
    public class RavenDbLoggerMessageHandler : IMessageHandler
    {
        public void Execute(ResponseContext context)
        {
            var store = context.Get<IDocumentStore>();

            using (var session = store.OpenSession())
            {
                session.Store(context.Message);
                var metadata = session.Advanced.GetMetadataFor(context.Message);
                metadata.Add(Keys.RavenDB.Metadata.Created,DateTime.Now);
                session.SaveChanges();
            }
        }
    }
}
