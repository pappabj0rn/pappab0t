using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Abstractions;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class CommandResponderTests : ResponderTestsBase
    {
        protected Mock<Command> CmdMock1;
        protected Mock<Command> CmdMock2;
        protected List<Mock<Command>> CommandMocks;

        protected CommandResponderTests()
        {
            CmdMock1 = new Mock<Command>();
            CmdMock1.Setup(x => x.RespondsTo(It.Is<string>(s => s == "cmd_t1")))
                .Returns(true);
            CmdMock2 = new Mock<Command>();
            CmdMock2.Setup(x => x.RespondsTo(It.Is<string>(s => s == "cmd_t2")))
                .Returns(true);

            CommandMocks = new List<Mock<Command>>
            {
                CmdMock1,
                CmdMock2
            };

            var commands = new List<Command>
            {
                CmdMock1.Object,
                CmdMock2.Object
            };

            Responder = new CommandResponder(CommandDataParser, commands);
        }

        public class CanRespond : CommandResponderTests
        {
            [Theory]
            [InlineData("cmd_t1")]
            [InlineData("cmd_t2")]
            public void Should_return_true_for_configured_commands(string msg)
            {
                var context = CreateContext(msg, SlackChatHubType.DM);
                var canRespond = Responder.CanRespond(context);
                Assert.True(canRespond);
            }

            [Theory]
            [InlineData("test cmd_t1")]
            [InlineData("describe")]
            [InlineData("TestCommand")]
            public void Should_return_false_for_anything_thats_not_a_configured_command(string msg)
            {
                var context = CreateContext(msg, SlackChatHubType.DM);
                var canRespond = Responder.CanRespond(context);
                Assert.False(canRespond);
            }
        }

        public class GetResponse : CommandResponderTests
        {
            [Theory]
            [InlineData("cmd_t1","-d data")]
            [InlineData("cmd_t2","-d data")]
            public void Should_return_response_from_responding_command(string msg, string parameters)
            {
                var context = CreateContext($"{msg} {parameters}", SlackChatHubType.DM);
                Responder.CanRespond(context);

                var response = Responder.GetResponse(context);

                var cmd = CommandMocks.First(x => x.Object.RespondsTo(msg));

                cmd.VerifySet(x=>x.CommandData = It.Is<CommandData>(
                    d=>d.Command == msg 
                       && d.Params.ContainsKey("d")));

                Assert.Equal(cmd.Object.GetResponse(), response);
            }
        }
    }
}
