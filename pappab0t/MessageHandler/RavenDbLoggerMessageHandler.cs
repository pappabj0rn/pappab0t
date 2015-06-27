using System;
using MargieBot.Models;
using Newtonsoft.Json.Linq;
using Raven.Client;

namespace pappab0t.MessageHandler
{
    public class RavenDbLoggerMessageHandler : IMessageHandler
    {
        public void Execute(ResponseContext context)
        {
            var store = context.Get<IDocumentStore>();
            var jObject = JObject.Parse(context.Message.RawData);

            using (var session = store.OpenSession())
            {
                session.Store(context.Message);
                var metadata = session.Advanced.GetMetadataFor(context.Message);
                metadata.Add(Keys.RavenDB.Metadata.Created,DateTime.Now);
                metadata.Add(Keys.RavenDB.Metadata.TimeStamp, jObject["ts"].Value<string>());
                session.SaveChanges();
            }
        }
    }
}
