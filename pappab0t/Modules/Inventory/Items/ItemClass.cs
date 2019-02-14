using Raven.Database.Common;

namespace pappab0t.Modules.Inventory.Items
{
    public abstract class ItemClass
    {

    }

    public class Weapon : ItemClass
    {
    }

    public class Novelty : ItemClass
    {
    }

    public class Token : ItemClass
    {
    }

    public class Tool : ItemClass
    {
    }

    public class Resource : ItemClass
    {
        public ResourceType Type { get; set; }
        public int Amount { get; set; }
    }
}