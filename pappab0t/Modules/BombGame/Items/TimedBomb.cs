using System;
using pappab0t.Modules.Inventory.Items;

namespace pappab0t.Modules.BombGame.Items
{
    public class TimedBombType : ItemType
    {
        public override string Name => "Tidsinställd bomb";
        public override string IndefiniteAricle => "En";
        public override Guid Id => new Guid("de920049-a074-4ba3-acee-da1ea16e8151");
    }
    //public class TimedBomb
    //{
    //    public override string Name
    //    {
    //        get => GetFriendlyTypeName();
    //        set {}
    //    }

    //    public override string GetFriendlyTypeName() => "Tidsinställd bomb";
    //    public override string Type => nameof(TimedBomb);

    //    public override string GetDescription()
    //    {
    //        return "En tidsinställd bomb.";
    //    }
    //}
    //public class ReturnCookie : Item
    //{
    //}
    //public class MultidimentionalPipe : Item
    //{

    //}
    ////TODO: modfiers => souldbound tex, stats

    //public interface IActivatableItem
    //{
    //    /* ??? */ void Activate(/* ??? */);
    //}
    //public class ReturnCookieDetector : Item, IActivatableItem
    //{
    //    public Battery Battery { get; set; }
    //}

    //public class Battery : Item
    //{
    //    public int Power { get; set; }
    //}
}