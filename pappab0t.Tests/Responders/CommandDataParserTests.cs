using System.Collections.Generic;
using MargieBot;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    //todo drop public props on CDP. use CD from Parse()
    public abstract class CommandDataParserTests : ResponderTestsBase
    {
        private CommandDataParser _parser;

        protected CommandDataParserTests()
        {
            _parser = new CommandDataParser();
        }

        public class ToBot : CommandDataParserTests
        {
            [Fact]
            public void Should_return_true_when_bot_is_mentioned()
            {
                var context = CreateContext("pbot ge mig mat");
                _parser.Context = context;

                _parser.Parse();

                Assert.True(_parser.ToBot);
            }

            [Fact]
            public void Should_return_true_when_message_is_dm()
            {
                var context = CreateContext("ge mig mat", SlackChatHubType.DM);
                _parser.Context = context;

                _parser.Parse();

                Assert.True(_parser.ToBot);
            }

            [Fact]
            public void Should_return_true_when_bot_is_mentioned_and_message_is_dm()
            {
                var context = CreateContext("pbot ge mig mat", SlackChatHubType.DM);
                _parser.Context = context;

                _parser.Parse();

                Assert.True(_parser.ToBot);
            }

            [Fact]
            public void Should_return_false_when_message_is_not_dm_and_bot_is_not_mentioned()
            {
                var context = CreateContext("ge mig mat");
                _parser.Context = context;

                _parser.Parse();

                Assert.False(_parser.ToBot);
            }
        }

        public class Command : CommandDataParserTests
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
                _parser.Context = context;

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
                _parser.Context = context;

                _parser.Parse();

                Assert.Null(_parser.Command);
            }

            [Fact]
            public void Should_parse_commands_to_lower_case()
            {
                var cmd = "CMD";
                var context = CreateContext(cmd, SlackChatHubType.DM);
                _parser.Context = context;

                _parser.Parse();

                Assert.Equal(cmd.ToLower(), _parser.Command);
            }
        }

        public class Parameters : CommandDataParserTests
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
                _parser.Context = context;

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
                _parser.Context = context;

                _parser.Parse();

                Assert.Contains(expectedKey, _parser.Params.Keys);
                Assert.Equal(expectedValue, _parser.Params[expectedKey]);
            }

            [Fact]
            public void Should_exclude_user_reference_from_unnamed_params_when_sucesfully_parsed()
            {
                var context = CreateContext("pbot ge eriska 5kr");
                _parser.Context = context;

                _parser.Parse();

                Assert.Contains(Keys.CommandParser.UnnamedParam, _parser.Params.Keys);
                Assert.Equal("5kr", _parser.Params[Keys.CommandParser.UnnamedParam]);
            }

            [Fact]
            public void Should_parse_multiple_flags_into_separate_params()
            {
                var context = CreateContext("pbot cmd -xyz");
                _parser.Context = context;

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
                _parser.Context = context;

                _parser.Parse();

                Assert.Contains("t", _parser.Params.Keys);

                Assert.Equal("test test", _parser.Params["t"]);
            }

            [Fact]
            public void Should_group_multi_word_param_values_by_quotes()
            {
                var context = CreateContext("pbot cmd -t \"test test\" -x xyz");
                _parser.Context = context;

                _parser.Parse();

                Assert.Contains("t", _parser.Params.Keys);
                Assert.Contains("x", _parser.Params.Keys);

                Assert.Equal("test test", _parser.Params["t"]);
                Assert.Equal("xyz", _parser.Params["x"]);
            }

            [Theory]
            [InlineData("test para", "para")]
            [InlineData("test para1 para2", "para1 para2")]
            [InlineData("test -t test para", "para")]
            [InlineData("test -t \"test test\" para", "para")]
            [InlineData("test -t \"test test\" para1 para2", "para1 para2")]
            [InlineData("test --test \"test test\" para1 para2", "para1 para2")]
            [InlineData("test -test \"test test\" para1 para2", "test test para1 para2")]
            public void Should_group_unnamed_leftovers_into_unnamed_param(
                string msg, string expectedUnnamedValue)
            {
                var context = CreateContext(msg, SlackChatHubType.DM);
                _parser.Context = context;

                _parser.Parse();

                Assert.Contains(Keys.CommandParser.UnnamedParam, _parser.Params.Keys);

                Assert.Equal(expectedUnnamedValue, _parser.Params[Keys.CommandParser.UnnamedParam]);
            }

            [Theory]
            [InlineData("pbot cmd -u eriska")]
            [InlineData("pbot cmd -u <@U06BH8WTT>")]
            [InlineData("pbot cmd eriska")]
            [InlineData("pbot cmd <@U06BH8WTT>")]

            public void Should_include_UserKnown_param_when_a_user_is_known(
                string msg)
            {
                var context = CreateContext(msg);

                _parser.Context = context;

                _parser.Parse();

                Assert.Contains(Keys.CommandParser.UserKnownKey, _parser.Params.Keys);
            }

            [Theory]
            [InlineData("pbot cmd -u nisse")]
            [InlineData("pbot cmd nisse")]

            public void Should_not_include_UserKnown_param_when_a_user_is_not_known(
                string msg)
            {
                var context = CreateContext(msg);

                _parser.Context = context;

                _parser.Parse();

                Assert.DoesNotContain(Keys.CommandParser.UserKnownKey, _parser.Params.Keys);
            }
        }

        public class ParametersRaw : CommandDataParserTests
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
                _parser.Context = context;

                _parser.Parse();

                Assert.Equal(expectedParams, _parser.ParamsRaw);
            }
        }

        public class Context : CommandDataParserTests
        {
            [Fact]
            public void Setting_context_should_clear_previous_parsed_state()
            {
                var msg1 = "pbot cmd -t test";
                var context1 = CreateContext(msg1);
                var parser = new CommandDataParser
                {
                    Context = context1
                };

                parser.Parse();

                var msg2 = "test";
                var context2 = CreateContext(msg2);
                parser.Context = context2;

                Assert.Null(parser.Command);
                Assert.Null(parser.ParamsRaw);
                Assert.Empty(parser.Params);
            }
        }
    }
}
