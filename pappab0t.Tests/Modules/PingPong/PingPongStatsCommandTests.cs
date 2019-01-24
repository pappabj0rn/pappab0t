using System;
using pappab0t.Modules.PingPong;
using Xunit;

namespace pappab0t.Tests.Modules.PingPong
{
    public abstract class PingPongStatsCommandTests
    {
        private readonly PingPongStatsCommand _cmd;

        protected PingPongStatsCommandTests()
        {
            SystemTime.Now = () => new DateTime(2019, 1, 23, 21, 51, 9);

            _cmd = new PingPongStatsCommand();
        }

        public class GetResponse : PingPongStatsCommandTests
        {
            [Fact]
            public void Should_return_accumulated_pingpong_stats()
            {
                PingPongStats.LastPingTime = SystemTime.Now().AddSeconds(-5);
                PingPongStats.LastPongTime = SystemTime.Now().AddSeconds(-4);
                PingPongStats.LastFailedPing = SystemTime.Now().AddDays(-3);
                PingPongStats.LastResponse = "Greetings!";
                PingPongStats.PingsSent = 1337;

                var response = _cmd.GetResponse();

                Assert.Contains($"Pings sent: {PingPongStats.PingsSent}\r\n", response.Text);
                Assert.Contains($"Last ping: {PingPongStats.LastPingTime:G}\r\n", response.Text);
                Assert.Contains($"Last duration: {PingPongStats.LastPongTime - PingPongStats.LastPingTime:g}\r\n", response.Text);
                Assert.Contains($"Last failed ping: {PingPongStats.LastFailedPing:G}\r\n", response.Text);
                Assert.Contains($"Last response: {PingPongStats.LastResponse}", response.Text);
            }
        }

        public class RespondsTo : PingPongStatsCommandTests
        {
            [Theory]
            [InlineData("pingpongstats")]
            [InlineData("pps")]
            public void Should_respond_to_pingpongstats_and_aliases(string cmd)
            {
                var respondTo = _cmd.RespondsTo(cmd);

                Assert.True(respondTo);
            }
        }
    }
}
