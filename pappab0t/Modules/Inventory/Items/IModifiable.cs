using System.Collections.Generic;
using pappab0t.Modules.Inventory.Items.Modifiers;

namespace pappab0t.Modules.Inventory.Items
{
    public interface IModifiable
    {
        List<Modifier> Modifiers { get; set; }
    }
}