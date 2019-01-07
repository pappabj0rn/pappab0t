using System.Linq;
using MargieBot;
using pappab0t.Models;
using pappab0t.Responders;
using Raven.Client.Document;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class MoneyPotResponderTests : ResponderTestsBase
    {
        protected readonly MoneyPotResponder Responder;

        protected MoneyPotResponderTests()
        {
            Responder = new MoneyPotResponder();
        }

        public class CanRespond : MoneyPotResponderTests
        {
            [Theory]
            [InlineData("pott", SlackChatHubType.DM)]
            [InlineData("pott xyz", SlackChatHubType.DM)]
            [InlineData("p", SlackChatHubType.DM)]
            [InlineData("p -?", SlackChatHubType.DM)]
            [InlineData("p xyz", SlackChatHubType.DM)]
            [InlineData("pbot p", SlackChatHubType.DM)]
            [InlineData("pbot p xyz", SlackChatHubType.DM)]

            [InlineData("pbot pott", SlackChatHubType.Channel)]
            [InlineData("pbot pott xyz", SlackChatHubType.Channel)]
            [InlineData("pbot p", SlackChatHubType.Channel)]
            [InlineData("pbot p -?", SlackChatHubType.Channel)]
            [InlineData("pbot p xyz", SlackChatHubType.Channel)]
            public void Should_return_true_for_the_given_scenarios(
                string msg,
                SlackChatHubType hubType)
            {
                var context = CreateContext(msg, hubType);

                var canRespond = Responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Theory]
            [InlineData("i pott", SlackChatHubType.DM)]
            [InlineData("pbot i p xyz", SlackChatHubType.Channel)]
            public void Should_return_false_for_the_given_scenarios(
                string msg,
                SlackChatHubType hubType)
            {
                var context = CreateContext(msg, hubType);

                var canRespond = Responder.CanRespond(context);

                Assert.False(canRespond);
            }
        }

        public class Response : MoneyPotResponderTests
        {
            public Response()
            {
                ConfigureRavenDB();
                SetupPots();
            }

            private void SetupPots()
            {
                using (var session = Store.OpenSession())
                {
                    session.Store(new MoneyPot{BEK = 123M, Name = "p1"});
                    session.Store(new MoneyPot{BEK = 456M, Name = "p2"});
                    session.SaveChanges();
                }
            }

            [Fact]
            public void Should_list_all_stored_pots_when_no_pot_name_is_given()
            {
                var context = CreateContext("pott", SlackChatHubType.DM);

                var response = Responder.GetResponse(context);

                Assert.Contains("> p1 123,00 kr", response.Text);
                Assert.Contains("> p2 456,00 kr", response.Text);
                Assert.DoesNotContain("kr kr", response.Text);
            }

            [Theory]
            [InlineData("p1","p1: 123,00 kr")]
            [InlineData("p2","p2: 456,00 kr")]
            public void Should_show_named_pot(
                string potName, 
                string expectedResponse)
            {
                var context = CreateContext($"pott {potName}", SlackChatHubType.DM);

                var response = Responder.GetResponse(context);

                Assert.Equal(expectedResponse, response.Text);
            }

            [Fact]
            public void Should_respond_with_unknown_pot_when_given_a_non_existing_pot_name()
            {
                var potName = "nonextpot";
                var context = CreateContext($"pott {potName}", SlackChatHubType.DM);

                PhraseBookMock
                    .Setup(x => x.IDontKnowXxxNamedYyyFormat())
                    .Returns("{0} {1}");

                var response = Responder.GetResponse(context);

                Assert.Equal($"nån pott {potName}", response.Text);
            }
        }
    }
}
