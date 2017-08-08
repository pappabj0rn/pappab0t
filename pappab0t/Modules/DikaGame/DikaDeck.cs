using System.Collections.Generic;
using System.Linq;
using pappab0t.Modules.Common;

namespace pappab0t.Modules.DikaGame
{
    public class DikaDeck : Deck
    {
        public bool IsTopCardFaceUp => ((DikaCard)Cards[0]).IsFaceUp;

        public DikaDeck()
        {
            CreateCards(4,1,13);
            Shuffle();
        }

        protected override Card CreateCard(int value, Suit suit)
        {
            return new DikaCard(value);
        }

        public DikaCard TakeOne()
        {
            var card = Cards[0];
            Cards.RemoveAt(0);
            return card as DikaCard;
        }

        public List<DikaCard> Take(int count)
        {
            var takeList = Cards.GetRange(0, count);
            Cards.RemoveRange(0, count);

            return takeList.Cast<DikaCard>().ToList();
        }

        public void AddToEnd(DikaCard card)
        {
            Cards.Add(card);
        }

        public void AddToEnd(IEnumerable<DikaCard> cards)
        {
            Cards.AddRange(cards);
        }

        public int CalculateScore()
        {
            var score = 0;
            var currentSet = 0;

            foreach (var card in Cards)
            {
                if (((DikaCard)card).IsFaceUp)
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