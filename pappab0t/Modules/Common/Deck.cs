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

        /// <summary>
        /// Fisher–Yates shuffle
        /// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
        /// </summary>
        public void Shuffle()
        {
            var rnd = new Random();

            for (var i = Cards.Count - 1; i > 0; i--)
            {
                var j = rnd.Next(0, i + 1);
                var tempCard = Cards[i];
                Cards[i] = Cards[j];
                Cards[j] = tempCard;
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