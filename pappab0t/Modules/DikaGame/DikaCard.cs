using pappab0t.Modules.Common;

namespace pappab0t.Modules.DikaGame
{
    public class DikaCard : Card
    {
        public bool IsFaceUp { get; private set; }

        public DikaCard(int value) : base(value)
        {
            IsFaceUp = false;
        }

        public void TurnFaceUp()
        {
            IsFaceUp = true;
        }
    }
}
