using System;
using System.Collections.Generic;

namespace pappab0t.Modules.DikaGame
{
    public class DikaDeck
    {
        readonly List<DikaCard> _cards;

        public DikaDeck()
        {
            _cards = new List<DikaCard>();
            CreateCards();
            Shuffle();
        }

        public bool IsTopCardFaceUp
        {
            get { return _cards[0].IsFaceUp; }
        }

        private void CreateCards()
        {
            for (var i = 1; i <= 4; i++)
            {
                for (var j = 1; j <= 13; j++)
                {
                    _cards.Add(new DikaCard(j));
                }
            }
        }

        private void Shuffle()
        {
            var rnd = new Random();

            for (int i = 0; i < 52; i++)
            {
                int swapIndex;
                do
                {
                    swapIndex = rnd.Next(51);
                }
                //No point in swapping a card for itself
                while (swapIndex == i);

                DikaCard cardHolder = _cards[swapIndex];
                _cards[swapIndex] = _cards[i];
                _cards[i] = cardHolder;
            }
        }

        public DikaCard TakeOne()
        {
            var card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        public List<DikaCard> Take(int count)
        {
            var takeList = _cards.GetRange(0, count);
            _cards.RemoveRange(0, count);

            return takeList;
        }

        public void AddToEnd(DikaCard card)
        {
            _cards.Add(card);
        }

        public void AddToEnd(IEnumerable<DikaCard> cards)
        {
            _cards.AddRange(cards);
        }

        public int CalculateScore()
        {
            var score = 0;
            var currentSet = 0;

            foreach (var card in _cards)
            {
                if (card.IsFaceUp)
                {
                    currentSet++;
                    continue;
                }

                //Several face down cards in a row
                if (currentSet == 0)
                {
                    continue;
                }

                //Only one face up card
                if (currentSet == 1)
                {
                    score++;
                }
                    
                //Several face up cards
                else
                {
                    //...at the top of the deck.
                    if (score == 0)
                    {
                        score = currentSet;
                    }
                    else
                    {
                        score *= currentSet;
                    }
                }

                currentSet = 0;
            }
            
            //Add score for the final set
            if (currentSet == 1)
            {
                score++;
            }
            else
            {
                score *= currentSet;
            }

            return score;
        }
    }
}