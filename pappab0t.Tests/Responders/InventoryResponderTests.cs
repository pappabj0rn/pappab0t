using MargieBot;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class InventoryResponderTests : ResponderTestsContext
    {
        public class CanRespond : InventoryResponderTests
        {
            [Theory]
            [InlineData("i", SlackChatHubType.DM)]
            [InlineData("pappab0t i", SlackChatHubType.Channel)]
            [InlineData("pb0t i", SlackChatHubType.Channel)]
            public void Should_return_true_for_the_given_scenarios(
                string msg, 
                SlackChatHubType hubType)
            {
                var responder = new InventoryResponder(null);

                var context = CreateContext(msg, hubType);

                var canRespond = responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Theory]
            [InlineData("i en tunna", SlackChatHubType.DM)]
            [InlineData("i en tunna", SlackChatHubType.Channel)]
            [InlineData("pbot i en tunna", SlackChatHubType.Channel)]
            [InlineData("test pbot ost i ett hus", SlackChatHubType.Channel)]
            [InlineData("hej pbot", SlackChatHubType.Channel)]
            public void Should_return_false_for_the_given_scenarios(string msg, SlackChatHubType hubType)
            {
                var responder = new InventoryResponder(null);

                var context = CreateContext(msg, hubType);

                var canRespond = responder.CanRespond(context);

                Assert.False(canRespond);
            }
        }
    }
}