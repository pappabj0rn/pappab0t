using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MargieBot;

using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using pappab0t.Modules.Inventory;

namespace pappab0t.Responders
{
    public class InventoryResponder : ResponderBase, IExposedCapability
    {
        public override bool CanRespond(ResponseContext context)
        {
            Context = context;

            var aliases = Bot.Aliases.Aggregate(Bot.UserName+"|", (prev, next) => prev + "|" + next, s => $"({s.Substring(1)})");

            return context.Message.MentionsBot 
                && Regex.IsMatch(context.Message.Text, $"{aliases} i", RegexOptions.IgnoreCase)
                || context.Message.IsDirectMessage() 
                && context.Message.Text == "i";
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;

            var inv = new InventoryManager(context).GetUserInventory();

            return new BotMessage
            {
                Text = BuildInventoryString(inv)
            };
        }

        private string BuildInventoryString(Inventory inv)
        {
            var sb = new StringBuilder();

            var displayName = Context.Message.IsDirectMessage()
                                ? "Du"
                                : Context.UserNameCache[Context.Message.User.ID];

            sb.AppendFormat("{0} har:\n", displayName);
            sb.AppendFormat("{0} kr", inv.BEK);

            if (inv.Items.Any())
                sb.AppendLine(", samt följande objekt:");

            var i = 0;
            foreach (var item in inv.Items)
            {
                i++;
                sb.AppendFormat("{0}. {1}\n", i, item.Name);
            }

            return sb.ToString();
        }

        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "i",
            Explatation = "Visar din nuvarande inventarieförteckning."
        };
    }
}
