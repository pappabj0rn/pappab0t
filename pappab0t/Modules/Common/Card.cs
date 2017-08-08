namespace pappab0t.Modules.Common
{
    public class Card
    {
        public int Value { get; }
        public Suit Suit { get; }

        public Card(int value, Suit suit = Suit.Hearts)
        {
            Value = value;
            Suit = suit;
        }
    }
}
