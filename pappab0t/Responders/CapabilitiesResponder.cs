using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MargieBot;
using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Abstractions;
using pappab0t.Extensions;

namespace pappab0t.Responders
{
    public class CapabilitiesResponder : IResponder
    {
        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.IsDirectMessage()) &&
                   (Regex.IsMatch(context.Message.Text, @"vad kan du( göra)?", RegexOptions.IgnoreCase)
                   || Regex.IsMatch(context.Message.Text, @"\bhjälp\b", RegexOptions.IgnoreCase));
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var bot = context.Get<Bot>();

            var sb = new StringBuilder();
            sb.AppendLine("Här är en lista över mina nuvarande, exponerade, funktioner.");

            foreach (var exposedCapability in bot.Responders.OfType<IExposedCapability>())
            {
                sb.AppendLine("*{0}*".With(exposedCapability.Info.Usage));
                sb.AppendLine(">{0}".With(exposedCapability.Info.Explatation));
            }

            return new BotMessage{Text = sb.ToString()};
        }
    }
}
