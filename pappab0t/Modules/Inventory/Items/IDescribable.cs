namespace pappab0t.Modules.Inventory.Items
{
    public interface IDescribable
    {
        string GetFriendlyTypeName();
        string GetDescription();
    }
}