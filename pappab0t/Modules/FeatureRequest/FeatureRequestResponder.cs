using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class FeatureRequestResponder : ResponderBase, IExposedCapability
    {
        public override bool CanRespond(ResponseContext context)
        {
            Init(context);

            return CommandData.Command == "fr";
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage{Text = "not implemented" };
        }

        public ExposedInformation Info { get; }
    }
}
