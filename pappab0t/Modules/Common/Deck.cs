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

            for (var swapCount = (rnd.NextDouble()+100) * 100000; swapCount > 0; swapCount--)
            {
                var swapIndex1 = rnd.Next(Cards.Count - 1);
                var swapIndex2 = rnd.Next(Cards.Count - 1);

                var cardHolder = Cards[swapIndex1];
                Cards[swapIndex1] = Cards[swapIndex2];
                Cards[swapIndex2] = cardHolder;
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