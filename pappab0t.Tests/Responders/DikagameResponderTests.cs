using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
   public abstract class DikagameResponderTests : ResponderTestsBase
    {
        public class CanRespond : DikagameResponderTests
        {
            [Theory]
            [InlineData("dg")]
            [InlineData("dg 1")]
            [InlineData("dg 2")]
            [InlineData("dg 5")]
            [InlineData("DG")]
            [InlineData("dikagame")]
            [InlineData("Dikagame")]
            [InlineData("dikagame 1")]
            [InlineData("dikagame 2")]
            [InlineData("dikagame 5")]
            public void Should_return_true_for_the_given_scenarios(string msg)
            {
                var responder = new DikaGameResponder(null);

                var context = CreateContext(msg, SlackChatHubType.DM, false);

                var canRespond = responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Theory]
            [InlineData("dg", SlackChatHubType.Channel)]
            [InlineData("DG", SlackChatHubType.Channel)]
            [InlineData("dikagame", SlackChatHubType.Channel)]
            [InlineData("pbot dikagame", SlackChatHubType.Channel)]
            [InlineData("pbot Dikagame", SlackChatHubType.Channel)]
            [InlineData("pbot dg", SlackChatHubType.Channel)]
            [InlineData("pbot DG", SlackChatHubType.Channel)]

            [InlineData("hs dg", SlackChatHubType.Channel)]
            [InlineData("hs dg", SlackChatHubType.DM)]
            public void Should_return_false_for_the_given_scenarios(string msg, SlackChatHubType hubType)
            {
                var responder = new DikaGameResponder(null);

                var context = CreateContext(msg, hubType);

                var canRespond = responder.CanRespond(context);

                Assert.False(canRespond);
            }
        }
    }
}
