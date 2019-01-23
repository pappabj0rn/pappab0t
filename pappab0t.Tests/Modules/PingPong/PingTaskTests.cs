using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Abstractions;
using pappab0t.Modules.PingPong;
using pappab0t.SlackApis;
using Xunit;

namespace pappab0t.Tests.Modules.PingPong
{
    public abstract class PingTaskTests
    {
        private const string SlackToken = "29186412-4626-42CD-81C5-B9CC34B0D578";
        private const string SlackBotId = "USLACKBOT";

        private readonly List<SlackChatHub> _connectedDms = new List<SlackChatHub>();

        private readonly ScheduledTask _task;
        private readonly Mock<ISlackDMApi> _slackDmApiMock;
        private readonly Mock<IBot> _botMock;

        protected PingTaskTests()
        {
            SystemTime.Now = () => new DateTime(2019, 1, 15, 23, 0, 0);

            _botMock = new Mock<IBot>();
            _botMock
                .Setup(x=>x.SlackKey)
                .Returns(SlackToken);

            _botMock
                .Setup(x => x.ConnectedDMs)
                .Returns(() => _connectedDms);

            _botMock
                .Setup(x => x.ConnectedSince)
                .Returns(SystemTime.Now);

            _slackDmApiMock = new Mock<ISlackDMApi>();

            _task = new PingTask(_botMock.Object, _slackDmApiMock.Object);

            _connectedDms.Add(new SlackChatHub
            {
                Name = "@slackbot",
                ID = "D5QB040V8",
                Type = SlackChatHubType.DM
            });
        }

        public class IsDue : PingTaskTests
        {
            [Fact]
            public void Should_return_true_when_it_hasnt_been_executed()
            {
                Assert.True(_task.IsDue());
            }

            [Fact]
            public void Should_return_false_when_it_has_been_executed_for_less_than_its_interval_ago()
            {
                var time = new DateTime(2019, 1, 15, 23, 0, 0);
                SystemTime.Now = () => time;

                _task.Execute();

                time = time.AddMilliseconds(_task.Interval - 10);
                SystemTime.Now = () => time;

                Assert.False(_task.IsDue());
            }
        }

        public class Execute : PingTaskTests
        {

            [Fact]
            public void Should_return_false_when_execute_is_called_before_interval_has_past()
            {
                _task.Execute();

                var time = SystemTime.Now();
                SystemTime.Now = () => time.AddMilliseconds(_task.Interval - 10);

                var executed = _task.Execute();

                Assert.False(executed);
            }

            [Fact]
            public void Should_return_true_when_it_hasnt_been_executed_before()
            {
                var executed = _task.Execute();

                Assert.True(executed);
            }

            [Fact]
            public void Should_create_dm_connection_to_slackbot_when_one_does_not_exist()
            {
                _connectedDms.RemoveAt(0);

                _task.Execute();

                _slackDmApiMock.Verify(x =>
                        x.Open(
                            SlackToken,
                            SlackBotId,
                            false,
                            true),
                    Times.Once);
            }

            [Fact]
            public void Should_not_create_dm_connection_to_slackbot_when_one_exist()
            {
                _task.Execute();

                _slackDmApiMock.Verify(x =>
                        x.Open(
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<bool>(),
                            It.IsAny<bool>()),
                    Times.Never);
            }

            [Fact]
            public void Should_send_hello_to_slackbot()
            {
                _task.Execute();

                _botMock.Verify(x => x.Say(It.Is<BotMessage>(m =>
                    m.Text == "hello"
                    && m.ChatHub == _connectedDms.First())));
            }

            [Fact]
            public void Should_update_pingpongstatus()
            {
                PingPongStatus.PingSent = DateTime.MinValue;
                PingPongStatus.WaitingForPong = false;

                _task.Execute();

                Assert.Equal(SystemTime.Now(), PingPongStatus.PingSent);
                Assert.True(PingPongStatus.WaitingForPong);
            }

            [Fact]
            public void Should_reconnect_and_reset_pingpongstatus_when_pingpongstatus_is_set_to_waiting_for_pong()
            {
                PingPongStatus.PingSent = DateTime.MinValue;
                PingPongStatus.WaitingForPong = true;

                _task.Execute();

                _botMock.VerifySet(x => x.ConnectedSince = null);
            }

            [Fact]
            public void Should_not_try_to_ping_or_reconnect_when_bot_is_offline()
            {
                _botMock
                    .Setup(x => x.ConnectedSince)
                    .Returns((DateTime?)null);

                PingPongStatus.WaitingForPong = false;

                _task.Execute();

                _botMock.Verify(x => x.Say(It.Is<BotMessage>(m =>
                    m.Text == "hello"
                    && m.ChatHub == _connectedDms.First()))
                ,Times.Never);

                _botMock.VerifySet(x => x.ConnectedSince = null, Times.Never);

                Assert.False(PingPongStatus.WaitingForPong);
            }
        }
    }
}
