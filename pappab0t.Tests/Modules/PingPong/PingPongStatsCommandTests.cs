using System;
using pappab0t.Abstractions;
using pappab0t.Modules.PingPong;
using Xunit;

namespace pappab0t.Tests.Modules.PingPong
{
    public abstract class PingPongStatsCommandTests
    {
        private readonly PingPongStatsCommand _ppsCmd;

        protected PingPongStatsCommandTests()
        {
            SystemTime.Now = () => new DateTime(2019, 1, 23, 21, 51, 9);

            _ppsCmd = new PingPongStatsCommand();
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

                var response = _ppsCmd.GetResponse();

                Assert.Contains($"Ping sent: {PingPongStats.PingsSent}\r\n", response.Text);
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
                var respondTo = _ppsCmd.RespondsTo(cmd);

                Assert.True(respondTo);
            }
        }
    }
}
