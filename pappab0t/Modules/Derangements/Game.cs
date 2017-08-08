using pappab0t.Extensions;
using pappab0t.Modules.Common;

namespace pappab0t.Modules.Derangements
{
    public class Game
    {
        public string HitSymbol { get; set; }
        public string MissSymbol { get; set; }
        public string Outcome { get; private set; }

        public int Play()
        {
            if (HitSymbol.IsNullOrEmpty())
                HitSymbol = "-";

            if (MissSymbol.IsNullOrEmpty())
                MissSymbol = "_";

            var deck = new Deck();
            deck.CreateCards(1,1,10);
            deck.Shuffle();

            Outcome = "";
            var points = 0;

            for (var i = 0; i < deck.Cards.Count; i++)
            {
                if (deck.Cards[i].Value == i + 1)
                {
                    points++;
                    Outcome += HitSymbol;
                }
                Outcome += MissSymbol;
            }

            return points;
        }
    }
}
