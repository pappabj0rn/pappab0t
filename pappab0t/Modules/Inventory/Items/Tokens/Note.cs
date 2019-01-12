namespace pappab0t.Modules.Inventory.Items.Tokens
{
    public class Note : Item
    {
        public string Text { get; set; }
        public override string GetFriendlyTypeName() => "Lapp";

        public override string GetDescription() => Text;
    }
}
