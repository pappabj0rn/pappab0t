using System.Threading;
using pappab0t.Modules.Common;
using Xunit;


namespace pappab0t.Tests.Modules.Common
{
    public abstract class DeckTests
    {
        public class Shuffle
        {
            // Info on "Position maps"
            // ABCDEFGHIJ = all cards in order
            // jBCDEFGHIa = first and last cards swapped
            private const string DefaultPositionMap = "ABCDEFGHIJ";

            [Fact]
            public void An_unshuffled_deck_should_have_a_position_map_coresponding_to_all_cards_in_order()
            {
                var deck = CreateTestDeck();

                var result = PositionMap(deck);

                Assert.Equal(DefaultPositionMap, result);
            }

            private static Deck CreateTestDeck()
            {
                var deck = new Deck();
                deck.CreateCards(1, 1, 10);
                return deck;
            }

            private static string PositionMap(Deck deck)
            {
                var result = "";

                for (var i = 0; i < deck.Cards.Count; i++)
                {
                    if (deck.Cards[i].Value == i + 1)
                    {
                        result += (char)(deck.Cards[i].Value + 'A' - 1);
                    }
                    else
                    {
                        result += (char)(deck.Cards[i].Value + 'a' - 1);
                    }
                }
                return result;
            }

            [Fact]
            public void A_shuffled_deck_should_not_have_a_position_map_coresponding_to_all_cards_in_order()
            {
                var deck = CreateTestDeck();

                deck.Shuffle();

                var result = PositionMap(deck);

                Assert.NotEqual(DefaultPositionMap, result);
            }

            [Fact]
            public void Two_identical_decks_should_shuffle_to_differnet_positon_maps()
            {
                var deck1 = CreateTestDeck();
                var deck2 = CreateTestDeck();

                deck1.Shuffle();
                Thread.Sleep(100); //temp fix. should use global random
                deck2.Shuffle();

                Assert.NotEqual(PositionMap(deck1), PositionMap(deck2));
            }
        }
    }
}
