using System.Collections.Generic;

namespace pappab0t.Modules.Inventory.Items.Modifiers
{
    public abstract class Modifier
    {
        protected Modifier()
        {
            Modifiers = new List<Modifier>();
        }

        public List<Modifier> Modifiers { get; set; }
    }
}