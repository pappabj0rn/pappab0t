using System.Collections.Generic;
using Newtonsoft.Json;
using pappab0t.Modules.Inventory.Items.Modifiers;

namespace pappab0t.Modules.Inventory.Items
{
    public abstract class Item : IDescribable
    {
        protected Item()
        {
            Modifiers = new List<Modifier>();
        }

        public string Id { get; set; }

        [JsonRequired]
        public virtual string Name { get; set; }

        public virtual string Type { get; set; }

        public List<Modifier> Modifiers { get; set; }

        public abstract string GetFriendlyTypeName();

        public abstract string GetDescription();
    }
}