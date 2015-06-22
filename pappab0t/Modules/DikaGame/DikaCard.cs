namespace pappab0t.Modules.DikaGame
{
    public class DikaCard
    {
        public int Value { get; private set; }
        public bool IsFaceUp { get; private set; }

        public DikaCard(int value)
        {
            Value = value;
            IsFaceUp = false;
        }

        public void TurnFaceUp()
        {
            IsFaceUp = true;
        }
    }
}
