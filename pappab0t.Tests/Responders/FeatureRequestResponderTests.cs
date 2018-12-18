using MargieBot;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class FeatureRequestResponderTests : ResponderTestsBase
    {
        public class CanRespond : FeatureRequestResponderTests
        {
            [Theory]
            [InlineData("fr", SlackChatHubType.DM)]
            [InlineData("fr list", SlackChatHubType.DM)]
            [InlineData("fr view 123", SlackChatHubType.DM)]
            [InlineData("fr add sample text", SlackChatHubType.DM)]
            [InlineData("fr del 123", SlackChatHubType.DM)]
            [InlineData("fr delc 123", SlackChatHubType.DM)]
            [InlineData("fr edit 123 new text", SlackChatHubType.DM)]
            [InlineData("fr coment 123 comment text", SlackChatHubType.DM)]

            [InlineData("pbot fr")]
            [InlineData("pbot fr list")]
            [InlineData("pbot fr view 123")]
            [InlineData("pbot fr add sample text")]
            [InlineData("pbot fr del 123")]
            [InlineData("pbot fr delc 123")]
            [InlineData("pbot fr edit 123 new text")]
            [InlineData("pbot fr coment 123 comment text")]
            public void Should_respond_to_feature_requests(string text, SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(text, hubType);

                var responder = new FeatureRequestResponder();
                var canRespond = responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Theory]
            [InlineData("frasses?", SlackChatHubType.DM)]
            [InlineData("far", SlackChatHubType.DM)]

            [InlineData("pbot frasses?")]
            [InlineData("pbot far")]
            public void Should_not_respond_to_non_feature_requests(string text, SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(text, hubType);

                var responder = new FeatureRequestResponder();
                var canRespond = responder.CanRespond(context);

                Assert.False(canRespond);
            }
        }

        public class GetResponse : FeatureRequestResponderTests
        {

        }
    }
}
