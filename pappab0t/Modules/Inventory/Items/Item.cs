using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace pappab0t.Modules.Inventory.Items
{
    public abstract class Item
    {
        [JsonRequired]
        public string Name { get; set; }
        public bool SoulBound { get; set; }
    }
}