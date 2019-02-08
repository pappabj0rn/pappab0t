using System.Linq;
using System.Text;
using MargieBot;

using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using pappab0t.Modules.Inventory;

namespace pappab0t.Responders
{
    public class InventoryResponder : ResponderBase, IExposedCapability
    {
        private readonly IInventoryManager _invMan;

        public InventoryResponder(IInventoryManager invMan)
        {
            _invMan = invMan;
        }
        public override bool CanRespond(ResponseContext context)
        {
            Init(context);

            return CommandData.Command == "i"
                   && CommandData.ParamsRaw.IsNullOrEmpty();
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;
            _invMan.Context = context;

            var inv = _invMan.GetUserInventory();

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
                sb.AppendFormat("{0}. {1}\n", i, item.Name ?? item.GetFriendlyTypeName());//todo test
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
