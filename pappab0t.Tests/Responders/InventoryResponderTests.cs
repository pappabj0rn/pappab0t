using MargieBot;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class InventoryResponderTests : ResponderTestsBase
    {
        public class CanRespond : InventoryResponderTests
        {
            [Fact]
            public void Should_return_true_when_bot_gets_a_dm_with_only_i()
            {
                var responder = new InventoryResponder();

                var context = CreateResponseContext("i", SlackChatHubType.DM);


                var canRespond = responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Fact]
            public void Should_return_false_when_bot_gets_dm_with_more_words_than_i()
            {
                var responder = new InventoryResponder();

                var context = CreateResponseContext("i en tunna", SlackChatHubType.DM);

                var canRespond = responder.CanRespond(context);

                Assert.False(canRespond);
            }

            [Fact]
            public void Should_return_true_when_bot_is_mentioned_with_only_i()
            {
                var responder = new InventoryResponder();

                var context = CreateResponseContext("pbot i", SlackChatHubType.Channel, mentionsBot:true);

                var canRespond = responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Fact]
            public void Should_return_false_when_bot_is_mentioned_with_more_words_than_i()
            {
                var responder = new InventoryResponder();

                var context = CreateResponseContext("i en tunna", SlackChatHubType.Channel);

                var canRespond = responder.CanRespond(context);

                Assert.False(canRespond);
            }

            [Fact]
            public void Should_return_true_when_bot_is_mentionedby_alias_with_only_i()
            {
                var responder = new InventoryResponder();

                var context = CreateResponseContext("pb0t i", SlackChatHubType.Channel, mentionsBot: true);

                var canRespond = responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Fact]
            public void Should_return_false_when_bot_is_greeted()
            {
                var responder = new InventoryResponder();

                var context = CreateResponseContext("hej pbot", SlackChatHubType.Channel, mentionsBot: true);

                var canRespond = responder.CanRespond(context);

                Assert.False(canRespond);
            }
        }
    }
}