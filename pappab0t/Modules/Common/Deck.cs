using System;
using System.Collections.Generic;

namespace pappab0t.Modules.Common
{
    public class Deck
    {
        public List<Card> Cards;

        public Deck()
        {
            Cards = new List<Card>();
        }

        public void Shuffle()
        {
            var rnd = new Random();

            for (var i = 0; i < Cards.Count; i++)
            {
                int swapIndex;
                do
                {
                    swapIndex = rnd.Next(Cards.Count-1);
                }
                //No point in swapping a card for itself
                while (swapIndex == i);

                var cardHolder = Cards[swapIndex];
                Cards[swapIndex] = Cards[i];
                Cards[i] = cardHolder;
            }
        }

        public void CreateCards(int suits, int min, int max)
        {
            for (var i = 1; i <= suits; i++)
            {
                for (var j = min; j <= max; j++)
                {
                    Cards.Add(CreateCard(j,(Suit)i));
                }
            }
        }

        protected virtual Card CreateCard(int value, Suit suit)
        {
            return new Card(value, suit);
        }
    }
}