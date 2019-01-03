using MargieBot;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class InventoryResponderTests : ResponderTestsBase
    {
        public class CanRespond : InventoryResponderTests
        {
            [Theory]
            [InlineData("i", SlackChatHubType.DM)]
            [InlineData("pappab0t i", SlackChatHubType.Channel, true)]
            [InlineData("pb0t i", SlackChatHubType.Channel, true)]
            public void Should_return_true_for_the_given_scenarios(
                string msg, 
                SlackChatHubType hubType, 
                bool mentionsBot = false)
            {
                var responder = new InventoryResponder();

                var context = CreateResponseContext(msg, hubType, mentionsBot);

                var canRespond = responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Theory]
            [InlineData("i en tunna", SlackChatHubType.DM)]
            [InlineData("i en tunna", SlackChatHubType.Channel)]//
            [InlineData("pbot i en tunna", SlackChatHubType.Channel)]
            [InlineData("test pbot ost i ett hus", SlackChatHubType.Channel)]
            [InlineData("hej pbot", SlackChatHubType.Channel)]
            public void Should_return_false_for_the_given_scenarios(string msg, SlackChatHubType hubType)
            {
                var responder = new InventoryResponder();

                var context = CreateContext(msg, hubType);

                var canRespond = responder.CanRespond(context);

                Assert.False(canRespond);
            }
        }
    }
}