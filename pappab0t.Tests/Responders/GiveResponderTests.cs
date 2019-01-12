using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Models;
using pappab0t.Modules.Inventory;
using pappab0t.Modules.Inventory.Items.Tokens;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class GiveResponderTests : ResponderTestsBase
    {
        protected GiveResponderTests()
        {
            Responder = new GiveResponder(InventoryManagerMock.Object, PhraseBookMock.Object, CommandParser);
        }

        public class CanRespond : GiveResponderTests
        {
            [Theory]
            [InlineData("ge", SlackChatHubType.DM)]
            [InlineData("pbot ge")]
            public void Should_return_true_for_valid_scenarios(
                string msg,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);

                var canRespond = Responder.CanRespond(context);

                Assert.True(canRespond);
            }

            [Theory]
            [InlineData("hs ge", SlackChatHubType.DM)]
            [InlineData("gel", SlackChatHubType.DM)]

            [InlineData("ge")]
            [InlineData("ge mig en TUNNA!")]
            [InlineData("pbot gel")]
            [InlineData("pbot hs ge")]
            public void Should_return_false_for_invalid_scenarios(
                string msg,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);

                var canRespond = Responder.CanRespond(context);

                Assert.False(canRespond);
            }
        }

        public class GetResponse : GiveResponderTests
        {
            [Theory]
            [InlineData("pbot ge -?")]
            [InlineData("ge -?", SlackChatHubType.DM)]
            public void Should_return_msg_explaining_usage_for_the_given_scenarios(
                string msg,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);

                var response = Responder.GetResponse(context);

                Assert.StartsWith("Beskrivning av kommando: ge\r\n", response.Text);
                Assert.Contains("p: ", response.Text);
                Assert.Contains("s: ", response.Text);
            }

            [Theory]
            [InlineData("pbot ge -?a", true)]
            [InlineData("pbot ge -?", false)]
            [InlineData("ge -?a", true, SlackChatHubType.DM)]
            [InlineData("ge -?", false, SlackChatHubType.DM)]
            public void Should_toggle_explenation_of_admin_params_when_param_a_is_present(
                string msg,
                bool includesAdminParams,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);

                var response = Responder.GetResponse(context);

                var createItemExplenation = "\r\nc: [typ] {json-data}\r\n";

                if (includesAdminParams)
                    Assert.Contains(createItemExplenation, response.Text);
                else
                    Assert.DoesNotContain(createItemExplenation, response.Text);
            }

            [Theory]
            [InlineData("pbot ge eriska 10kr", 10.0)]
            [InlineData("pbot ge <@U06BH8WTT> 10kr", 10.0)]
            [InlineData("pbot ge <@U06BH8WTT> 8,5kr", 8.5)]
            [InlineData("pbot ge <@U06BH8WTT> -p 7,6", 7.6)]
            [InlineData("pbot ge eriska -p 7,6", 7.6)]

            [InlineData("ge eriska -p 7,6", 7.6, SlackChatHubType.DM)]
            [InlineData("ge <@U06BH8WTT> -p 7,6", 7.6, SlackChatHubType.DM)]
            [InlineData("ge <@U06BH8WTT> 8,5kr", 8.5, SlackChatHubType.DM)]
            [InlineData("ge <@U06BH8WTT> 10kr", 10.0, SlackChatHubType.DM)]
            [InlineData("ge eriska 10kr", 10.0, SlackChatHubType.DM)]
            public void Should_take_specified_amount_from_current_user_and_give_to_specified_user(
                string msg,
                decimal expectedAmount,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);

                Responder.GetResponse(context);

                InventoryManagerMock.Verify(x=>x.GetUserInventory(),Times.Once);
                InventoryManagerMock.Verify(x=>x.GetUserInventory(EriskaInvetory.UserId),Times.Once);
                InventoryManagerMock.Verify(x => x.Save(It.Is<Inventory>(i => i.UserId == Pappabj0rnInvetory.UserId)), Times.Never);
                InventoryManagerMock.Verify(x => x.Save(It.Is<Inventory>(i => i.UserId == EriskaInvetory.UserId)), Times.Never);
                InventoryManagerMock.Verify(x => x.Save(It.IsAny<IEnumerable<Inventory>>()), Times.Once);

                Assert.Equal(100 - expectedAmount, Pappabj0rnInvetory.BEK);
                Assert.Equal(100 + expectedAmount, EriskaInvetory.BEK);
            }

            [Theory]
            [InlineData("pbot ge eriska sak 1")]
            [InlineData("pbot ge eriska -s 1")]
            [InlineData("pbot ge <@U06BH8WTT> sak 1")]
            [InlineData("pbot ge <@U06BH8WTT> -s 1")]

            [InlineData("ge <@U06BH8WTT> -s 1", SlackChatHubType.DM)]
            [InlineData("ge <@U06BH8WTT> sak 1", SlackChatHubType.DM)]
            [InlineData("ge eriska -s 1", SlackChatHubType.DM)]
            [InlineData("ge eriska sak 1", SlackChatHubType.DM)]
            public void Should_move_specified_item_from_current_user_to_specified_user(
                string msg,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var testItem = new Note {Name = "test"};
                Pappabj0rnInvetory.Items.Add(testItem);

                var context = CreateContext(msg, hubType);

                Responder.GetResponse(context);

                InventoryManagerMock.Verify(x => x.GetUserInventory(), Times.Once);
                InventoryManagerMock.Verify(x => x.GetUserInventory(EriskaInvetory.UserId), Times.Once);
                InventoryManagerMock.Verify(x => x.Save(It.Is<Inventory>(i => i.UserId == Pappabj0rnInvetory.UserId)), Times.Never);
                InventoryManagerMock.Verify(x => x.Save(It.Is<Inventory>(i => i.UserId == EriskaInvetory.UserId)), Times.Never);
                InventoryManagerMock.Verify(x => x.Save(It.IsAny<IEnumerable<Inventory>>()), Times.Once);

                Assert.Empty(Pappabj0rnInvetory.Items);
                Assert.Contains(testItem, EriskaInvetory.Items);
            }

            [Theory]
            [InlineData("pbot ge eriska -c \"Note {'Name':'test'}\"")]
            [InlineData("pbot ge <@U06BH8WTT> -c \"Note {'Name':'test'}\"")]

            [InlineData("ge <@U06BH8WTT> -c \"Note {'Name':'test'}\"", SlackChatHubType.DM)]
            [InlineData("ge eriska -c \"Note {'Name':'test'}\"", SlackChatHubType.DM)]
            public void Should_create_specified_item_for_specified_user(
                string msg,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);

                Responder.GetResponse(context);

                InventoryManagerMock.Verify(x => x.GetUserInventory(), Times.Once);
                InventoryManagerMock.Verify(x => x.GetUserInventory(EriskaInvetory.UserId), Times.Once);
                InventoryManagerMock.Verify(x => x.Save(It.Is<Inventory>(i=>i.UserId == Pappabj0rnInvetory.UserId)), Times.Never);
                InventoryManagerMock.Verify(x => x.Save(It.Is<Inventory>(i=>i.UserId == EriskaInvetory.UserId)), Times.Once);
                InventoryManagerMock.Verify(x => x.Save(It.IsAny<IEnumerable<Inventory>>()), Times.Never);

                Assert.Empty(Pappabj0rnInvetory.Items);
                Assert.NotEmpty(EriskaInvetory.Items);

                Assert.Equal(typeof(Note), EriskaInvetory.Items[0].GetType());
                Assert.Equal("test", EriskaInvetory.Items[0].Name);
            }

            [Theory]
            [InlineData("ge pappabj0rn -c \"Note {Name:'test'}\"")]
            [InlineData("ge <@U06BHPNJG> -c \"Note {Name:'test'}\"")]
            public void Should_allow_creating_items_to_self(
                string msg,
                SlackChatHubType hubType = SlackChatHubType.DM)
            {
                var context = CreateContext(msg, hubType);

                Responder.GetResponse(context);

                InventoryManagerMock.Verify(x => x.Save(It.Is<Inventory>(i=>i.UserId == Pappabj0rnInvetory.UserId)), Times.Once);
                InventoryManagerMock.Verify(x => x.Save(It.IsAny<IEnumerable<Inventory>>()), Times.Never);

                Assert.NotEmpty(Pappabj0rnInvetory.Items);
                Assert.Equal(typeof(Note), Pappabj0rnInvetory.Items[0].GetType());
                Assert.Equal("test", Pappabj0rnInvetory.Items[0].Name);
            }

            [Fact]
            public void Should_set_context_on_inventory_manager_before_loding_user_inventory()
            {
                var context = CreateContext("pbot ge eriska 0kr");

                Responder.GetResponse(context);

                Assert.True(InventoryManagerContextSet);
            }

            [Theory]
            [InlineData("pbot ge eriska 10kr", nameof(IPhrasebook.MoneyTransfered), "10")]
            [InlineData("pbot ge eriska -p \"-10\"", nameof(IPhrasebook.Impossible))]
            [InlineData("pbot ge eriska 110kr", nameof(IPhrasebook.MoneyTransferInsufficientFunds))]
            [InlineData("pbot ge eriska sak 1", nameof(IPhrasebook.ItemTransfered), "Test")]
            [InlineData("pbot ge eriska sak 2", nameof(IPhrasebook.ItemTransferToFewItems))]
            [InlineData("pbot ge eriska sak 0", nameof(IPhrasebook.Impossible))]
            [InlineData("pbot ge nisse 10kr", nameof(IPhrasebook.IDontKnowXxxNamedYyy), "nån", "nisse")]
            [InlineData("pbot ge nisse sak 1", nameof(IPhrasebook.IDontKnowXxxNamedYyy), "nån", "nisse")]
            [InlineData("pbot ge -u nisse 10kr", nameof(IPhrasebook.IDontKnowXxxNamedYyy), "nån", "nisse")]
            [InlineData("pbot ge -u nisse -p 10", nameof(IPhrasebook.IDontKnowXxxNamedYyy), "nån", "nisse")]
            [InlineData("pbot ge -u nisse -s 1", nameof(IPhrasebook.IDontKnowXxxNamedYyy), "nån", "nisse")]
            [InlineData("pbot ge eriska sak x", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska -s x", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska 10", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska xkr", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska -p x", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska -c \"Note {'Name':'Test'}\"", nameof(IPhrasebook.ItemCreated), "Test")]
            [InlineData("pbot ge eriska -c \"Note {Name:'Test'}\"", nameof(IPhrasebook.ItemCreated), "Test")]
            [InlineData("pbot ge eriska -c \"Note {'Name':true}\"", nameof(IPhrasebook.ItemCreated), "true")]
            [InlineData("pbot ge eriska -c \"Note {'FooBar':'Test'}\"", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska -c \"FooBar {'Name':'Test'}\"", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska -c \"Note 'Name':'Test'}\"", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska -c \"Note {'Name''Test'}\"", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska -c \"Note {'Name:'Test'}\"", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska -c \"Note {'Name':Test'}\"", nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot ge eriska -c Note {'Name':'Test'}", nameof(IPhrasebook.IDidntUnderstand))]
            public void Should_respond_with_correct_message(
                string msg,
                string expectedMetodCall,
                string expectedPhParam1 = null,
                string expectedPhParam2 = null)
            {
                var testItem = new Note {Name = "Test"};
                Pappabj0rnInvetory.Items.Add(testItem);

                var context = CreateContext(msg);

                var response = Responder.GetResponse(context);

                Assert.Equal(expectedMetodCall, response?.Text);

                if (expectedPhParam1 != null)
                    Assert.Equal(expectedPhParam1, PhraseBookMock.Invocations.Last().Arguments[0].ToString());

                if (expectedPhParam2 != null)
                    Assert.Equal(expectedPhParam2, PhraseBookMock.Invocations.Last().Arguments[1].ToString());
            }

            [Fact]
            public void Should_not_transfer_souldbound_items()
            {
                var testItem = new Note { Name = "test", SoulBound = true };
                Pappabj0rnInvetory.Items.Add(testItem);
                
                var expectedPhrasebookCall = nameof(IPhrasebook.CantMoveSoulboundItems);

                var context = CreateContext("pbot ge eriska sak 1");

                var response = Responder.GetResponse(context);

                InventoryManagerMock.Verify(x=>x.Save(It.IsAny<Inventory>()), Times.Never);
                InventoryManagerMock.Verify(x=>x.Save(It.IsAny<IEnumerable<Inventory>>()), Times.Never);

                Assert.Equal(expectedPhrasebookCall, response?.Text);

                Assert.Empty(EriskaInvetory.Items);
                Assert.Contains(testItem, Pappabj0rnInvetory.Items);
            }

            [Theory]
            [InlineData("pbot ge eriska sak 1")]
            public void Should_not_store_responses_between_calls(string msg)
            {
                var context = CreateContext("pbot ge eriska sak 1");

                var pbCall1 = "call 1";
                var pbCall2 = "call 2";

                var testItem = new Note { Name = "test", SoulBound = true };
                Pappabj0rnInvetory.Items.Add(testItem);
                
                PhraseBookMock
                    .Setup(x => x.CantMoveSoulboundItems())
                    .Returns(pbCall1);
                
                Responder.GetResponse(context);

                PhraseBookMock
                    .Setup(x => x.CantMoveSoulboundItems())
                    .Returns(pbCall2);

                var response = Responder.GetResponse(context);

                Assert.Equal(pbCall2, response.Text);
            }
        }
    }
}