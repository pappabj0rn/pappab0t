using pappab0t.Modules.Common;
using Xunit;


namespace pappab0t.Tests.Modules.Common
{
    public abstract class DeckTests
    {
        public class Shuffle
        {
            [Fact]
            public void TestMethod1()
            {
                var deck = new Deck();
                deck.CreateCards(1,1,10);

                deck.Shuffle();

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

                Assert.NotEqual("ABCDEFGHIJ",result);
            }
        }
    }
}
