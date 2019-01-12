using Newtonsoft.Json;

namespace pappab0t.Modules.Inventory.Items
{
    public abstract class Item : IDescribable
    {
        [JsonRequired]
        public string Name { get; set; }
        public bool SoulBound { get; set; }

        public abstract string GetFriendlyTypeName();
        public abstract string GetDescription();
    }
}