using System;
using MargieBot;
using pappab0t.Modules.PingPong;
using pappab0t.Tests.Responders;
using Xunit;

namespace pappab0t.Tests.Modules.PingPong
{
    public abstract class PongResponderTests : ResponderTestsBase
    {
        protected PongResponderTests()
        {
            Responder = new PongResponder();
        }

        public class CanRespond : PongResponderTests
        {
            [Fact]
            public void Should_respond_to_slackbot_dm_when_pingpongstatus_is_waiting_for_pong()
            {
                PingPongStatus.WaitingForPong = true;
                var context = CreateContext("Hi!", SlackChatHubType.DM, userUUID: "USLACKBOT");

                var canRespond = Responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Fact]
            public void Should_not_respond_to_slackbot_dm_when_pingpongstatus_is_not_waiting_for_pong()
            {
                PingPongStatus.WaitingForPong = false;
                var context = CreateContext("Hi!", SlackChatHubType.DM, userUUID: "USLACKBOT");

                var canRespond = Responder.CanRespond(context);

                Assert.False(canRespond);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_not_respond_to_general_slackbot_messages(bool pps)
            {
                PingPongStatus.WaitingForPong = pps;
                var context = CreateContext("Hi!", SlackChatHubType.Channel, userUUID: "USLACKBOT");

                var canRespond = Responder.CanRespond(context);

                Assert.False(canRespond);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_not_respond_to_general_messages(bool pps)
            {
                PingPongStatus.WaitingForPong = pps;
                var context = CreateContext("Hi!");

                var canRespond = Responder.CanRespond(context);

                Assert.False(canRespond);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void Should_not_respond_to_dm_from_other_users(bool pps)
            {
                PingPongStatus.WaitingForPong = pps;
                var context = CreateContext("Hi!", SlackChatHubType.DM);

                var canRespond = Responder.CanRespond(context);

                Assert.False(canRespond);
            }
        }

        public class GetResponse : PongResponderTests
        {
            private readonly ResponseContext _context;

            public GetResponse()
            {
                _context = CreateContext("Hi!", SlackChatHubType.DM, userUUID: "USLACKBOT");

                PingPongStats.LastFailedPing = DateTime.MinValue.AddMilliseconds(0021);
                PingPongStats.LastPingTime = SystemTime.Now().AddSeconds(-1);
                PingPongStats.LastPongTime = DateTime.MinValue.AddMilliseconds(0022);
                PingPongStats.PingsSent = 1;
                PingPongStats.LastResponse = null;

                PingPongStatus.WaitingForPong = true;

                SystemTime.Now = () => new DateTime(2019, 1, 23, 22, 43, 22);
            }

            [Fact]
            public void Should_update_status_and_stats()
            {
                Responder.GetResponse(_context);

                Assert.False(PingPongStatus.WaitingForPong);

                Assert.Equal(SystemTime.Now(), PingPongStats.LastPongTime);
                Assert.Equal(_context.Message.Text, PingPongStats.LastResponse);
            }
        }
    }
}
