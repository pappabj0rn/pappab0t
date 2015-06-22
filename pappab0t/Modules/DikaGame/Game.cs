namespace pappab0t.Modules.DikaGame
{
    public class Game
    {
        public int Play()
        {
            var deck = new DikaDeck();

            while (!deck.IsTopCardFaceUp)
            {
                var card = deck.TakeOne();
                card.TurnFaceUp();

                if (card.Value == 1)
                {
                    deck.AddToEnd(card);
                    continue;
                }

                var workList = deck.Take(card.Value - 1);
                workList.Reverse();
                workList.Add(card);

                deck.AddToEnd(workList);
            }

            return deck.CalculateScore();
        }
    }
}