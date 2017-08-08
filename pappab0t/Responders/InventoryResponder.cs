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
    public class InventoryResponder : IResponder, IExposedCapability
    {
        private ResponseContext _context;
        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.IsDirectMessage()) &&
                   Regex.IsMatch(context.Message.Text, @"\bi\b", RegexOptions.IgnoreCase);
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            _context = context;

            var inv = new InventoryManager(context).GetUserInventory();

            return new BotMessage
            {
                Text = BuildInventoryString(inv)
            };
        }

        private string BuildInventoryString(Inventory inv)
        {
            var sb = new StringBuilder();

            var displayName = _context.Message.IsDirectMessage()
                                ? "Du"
                                : _context.UserNameCache[_context.Message.User.ID];

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

        public ExposedInformation Info
        {
            get
            {
                return new ExposedInformation
                {
                    Usage = "i",
                    Explatation = "Visar din nuvarande inventarieförteckning."
                };
            }
        }
    }
}
