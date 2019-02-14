using System;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Models;
using pappab0t.Modules.BombGame;
using pappab0t.Modules.BombGame.Items;
using pappab0t.Modules.Inventory.Items.Modifiers;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Modules.BombGame
{
    public abstract class TimedBombCommandTests : TestContext
    {
        protected readonly TimedBombCommand _cmd;

        protected TimedBombCommandTests()
        {
            _cmd = new TimedBombCommand(
                InventoryManagerMock.Object, 
                PhrasebookMock.Object,
                RandomMock.Object);
        }

        public class CanRespond : TimedBombCommandTests
        {
            [Theory]
            [InlineData("timedbomb")]
            [InlineData("tb")]
            public void Should_respond_to_given_commands(string cmdString)
            {
                var responds = _cmd.RespondsTo(cmdString);

                Assert.True(responds);
            }
        }

        public class GetResponse : TimedBombCommandTests
        {
            public GetResponse()
            {
                _cmd.Context = CreateContext("tb", SlackChatHubType.DM);
                _cmd.CommandData = new CommandData
                {
                    Command = "tb"
                };

                Pappabj0rnInvetory.BEK = TimedBombCommand.Cost;
            }

            [Fact]
            public void Should_inform_of_pricing_when_user_hasnt_got_enough_money()
            {
                Pappabj0rnInvetory.BEK = TimedBombCommand.Cost - 1;

                var response = _cmd.GetResponse();

                Assert.Equal(nameof(IPhrasebook.PlayInsufficientFunds), response.Text);

                PhrasebookMock
                    .Verify(x => x.PlayInsufficientFunds(TimedBombCommand.Cost), 
                        Times.Once);
            }

            [Fact]
            public void Should_put_timed_bomb_in_calling_users_inventory_and_deduct_cost_when_user_has_enough_money()
            {
                var response = _cmd.GetResponse();

                var tb = Pappabj0rnInvetory.Items.First();

                //todo item creation moves to some factory that creates items from config
                //also enables random items och given class
                Assert.Equal(0M, Pappabj0rnInvetory.BEK);
                Assert.True(tb.Type is TimedBombType);
                Assert.Equal(nameof(IPhrasebook.ItemCreated), response.Text);

                PhrasebookMock
                    .Verify(x => x.ItemCreated(tb.Type.Name), 
                        Times.Once);
            }

            [Fact]
            public void Created_bomb_should_have_its_expiration_date_set_to_today_plus_7_days_plus_random_minutes()
            {
                var randomMinutes = 30;
                RandomMock
                    .Setup(x => x.Next(TimedBombCommand.LowQualityExpirationSpan))
                    .Returns(randomMinutes);

                var time = new DateTime(2019, 1, 29, 23, 24, 22);
                SystemTime.Now = () => time;

                _cmd.GetResponse();

                var bomb = Pappabj0rnInvetory.Items.First(x=>x.Type is TimedBombType);
                Assert.Equal(
                    time.AddDays(TimedBombCommand.BaseExpiration).AddMinutes(randomMinutes), 
                    bomb.Modifiers.OfType<Expires>().First().DateTime);
            }

            [Fact]
            public void Created_bomb_should_have_a_hanlerlog_with_current_user_logged()
            {
                _cmd.GetResponse();

                var bomb = Pappabj0rnInvetory.Items.First(x => x.Type is TimedBombType);
                Assert.NotEmpty(bomb.Modifiers.OfType<IHandlerLog>());
            }
        }
    }
}