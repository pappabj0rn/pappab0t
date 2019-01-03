using System.Collections.Generic;
using MargieBot;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class CommandParserTests : ResponderTestsBase
    {
        private CommandParser _parser;

        protected void CreateCommandParser(ResponseContext context)
        {
            _parser = new CommandParser {Context = context};
        }

        public class ToBot : CommandParserTests
        {
            [Fact]
            public void Should_return_true_when_bot_is_mentioned()
            {
                var context = CreateContext("pbot ge mig mat");
                CreateCommandParser(context);

                _parser.Parse();

                Assert.True(_parser.ToBot);
            }

            [Fact]
            public void Should_return_true_when_message_is_dm()
            {
                var context = CreateContext("ge mig mat", SlackChatHubType.DM);
                CreateCommandParser(context);

                _parser.Parse();

                Assert.True(_parser.ToBot);
            }

            [Fact]
            public void Should_return_true_when_bot_is_mentioned_and_message_is_dm()
            {
                var context = CreateContext("pbot ge mig mat", SlackChatHubType.DM);
                CreateCommandParser(context);

                _parser.Parse();

                Assert.True(_parser.ToBot);
            }

            [Fact]
            public void Should_return_false_when_message_is_not_dm_and_bot_is_not_mentioned()
            {
                var context = CreateContext("ge mig mat");
                CreateCommandParser(context);

                _parser.Parse();

                Assert.False(_parser.ToBot);
            }
        }

        public class Command : CommandParserTests
        {
            [Theory]
            [InlineData("<@botUUID> ge nisse 5kr", "ge")]
            [InlineData("<@botUUID> i", "i")]

            [InlineData("pappab0t ge nisse 5kr", "ge")]
            [InlineData("pappab0t i", "i")]

            [InlineData("pbot ge nisse 5kr", "ge")]
            [InlineData("pbot i", "i")]

            [InlineData("pbot ge nisse 5kr", "ge", SlackChatHubType.DM)]
            [InlineData("pbot i", "i", SlackChatHubType.DM)]

            [InlineData("ge nisse 5kr", "ge", SlackChatHubType.DM)]
            [InlineData("i", "i", SlackChatHubType.DM)]

            [InlineData("hej pbot", "hej")]
            public void Should_return_first_word_of_message_that_isnt_a_nick_for_the_bot(
                string msg, 
                string expectedCommand,
                SlackChatHubType type = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, type);
                CreateCommandParser(context);

                _parser.Parse();

                Assert.Equal(expectedCommand, _parser.Command);
            }

            [Theory]
            [InlineData("pbot", SlackChatHubType.DM, true)]
            [InlineData("pbot", SlackChatHubType.Channel, true)]
            [InlineData("ost i en tunna", SlackChatHubType.Channel, false)]
            public void Should_return_null_when_message_has_no_command(
                string msg,
                SlackChatHubType type,
                bool mentionsBot)
            {
                var context = CreateContext(msg, type, mentionsBot);
                CreateCommandParser(context);

                _parser.Parse();

                Assert.Null(_parser.Command);
            }

            [Fact]
            public void Should_parse_commands_to_lower_case()
            {
                var cmd = "CMD";
                var context = CreateContext(cmd, SlackChatHubType.DM);
                CreateCommandParser(context);

                _parser.Parse();

                Assert.Equal(cmd.ToLower(), _parser.Command);
            }
        }

        public class Parameters : CommandParserTests
        {
            [Theory]
            [InlineData("pbot ge eriska 5kr", "user", "U06BH8WTT")]
            [InlineData("pbot ge <@U06BH8WTT> 5kr", "user", "U06BH8WTT")]
            public void Should_parse_user_reference_without_key(
                string msg,
                string expectedKey,
                string expectedValue,
                SlackChatHubType type = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, type);
                CreateCommandParser(context);

                _parser.Parse();

                Assert.Contains(expectedKey, _parser.Params.Keys);
                Assert.Equal(expectedValue, _parser.Params[expectedKey]);
            }

            [Theory]
            [InlineData("pbot cmd -u eriska", "user", "U06BH8WTT")]
            [InlineData("pbot cmd -u <@U06BH8WTT>", "user", "U06BH8WTT")]
            [InlineData("pbot cmd --user eriska", "user", "U06BH8WTT")]
            [InlineData("pbot cmd --user <@U06BH8WTT>", "user", "U06BH8WTT")]
            public void Should_parse_user_reference_with_key(
                string msg,
                string expectedKey,
                string expectedValue,
                SlackChatHubType type = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, type);
                CreateCommandParser(context);

                _parser.Parse();

                Assert.Contains(expectedKey, _parser.Params.Keys);
                Assert.Equal(expectedValue, _parser.Params[expectedKey]);
            }

            [Fact]
            public void Should_parse_multiple_flags_into_separate_params()
            {
                var context = CreateContext("pbot cmd -xyz");
                CreateCommandParser(context);

                _parser.Parse();

                var expectedParams = new[] { "x", "y", "z" };
                foreach (string expectedParam in expectedParams)
                {
                    Assert.Contains(expectedParam, _parser.Params.Keys);

                    Assert.Equal("", _parser.Params[expectedParam]);
                }
            }

            [Fact]
            public void Should_not_include_quotes_on_multi_word_param_values()
            {
                var context = CreateContext("pbot cmd -t \"test test\"");
                CreateCommandParser(context);

                _parser.Parse();

                Assert.Contains("t", _parser.Params.Keys);

                Assert.Equal("test test", _parser.Params["t"]);
            }
        }

        public class ParametersRaw : CommandParserTests
        {
            [Theory]
            [InlineData("pbot ge nisse 5kr", "nisse 5kr")]
            [InlineData("pbot i", "")]
            [InlineData("pbot cmd --verbose -v --test t", "--verbose -v --test t")]

            [InlineData("pbot ge nisse 5kr", "nisse 5kr", SlackChatHubType.DM)]
            [InlineData("pbot i", "", SlackChatHubType.DM)]
            [InlineData("pbot cmd --verbose -v --test t", "--verbose -v --test t", SlackChatHubType.DM)]

            [InlineData("ge nisse 5kr", "nisse 5kr", SlackChatHubType.DM)]
            [InlineData("i", "", SlackChatHubType.DM)]
            [InlineData("cmd --verbose -v --test t", "--verbose -v --test t", SlackChatHubType.DM)]
            public void Should_return_full_message_after_command(
                string msg,
                string expectedParams,
                SlackChatHubType type = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, type);
                CreateCommandParser(context);

                _parser.Parse();

                Assert.Equal(expectedParams, _parser.ParamsRaw);
            }
        }
    }
}
