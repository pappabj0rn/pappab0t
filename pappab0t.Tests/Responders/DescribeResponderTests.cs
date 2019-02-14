using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Models;
using pappab0t.Modules.Inventory.Items;
using pappab0t.Modules.Inventory.Items.Tokens;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class DescribeResponderTests : ResponderTestsContext
    {
        protected DescribeResponderTests()
        {
            Responder = new DescribeResponder(
                InventoryManagerMock.Object, 
                PhraseBookMock.Object, 
                CommandDataParser);
        }

        public class CanRespond : DescribeResponderTests
        {
            [Theory]
            [InlineData("pbot beskriv sak 1")]
            [InlineData("pbot beskriv -s 1")]
            [InlineData("pbot beskriv -?")]
            [InlineData("pbot beskriv")]

            [InlineData("beskriv", SlackChatHubType.DM)]
            [InlineData("beskriv -?", SlackChatHubType.DM)]
            [InlineData("beskriv -s 1", SlackChatHubType.DM)]
            [InlineData("beskriv sak 1", SlackChatHubType.DM)]
            public void Should_return_true_for_the_given_scanarios(
                string msg,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);
                var canRespond = Responder.CanRespond(context);
                Assert.True(canRespond);
            }

            [Theory]
            [InlineData("pbot x beskriv sak 1")]
            [InlineData("pbot x")]

            [InlineData("x", SlackChatHubType.DM)]
            [InlineData("x beskriv sak 1", SlackChatHubType.DM)]
            public void Should_return_false_for_the_given_scanarios(
                string msg,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);
                var canRespond = Responder.CanRespond(context);
                Assert.False(canRespond);
            }
        }

        public class GetDescriptionResponse : DescribeResponderTests
        {
            [Theory]
            [InlineData("pbot beskriv -?")]
            [InlineData("beskriv -?", SlackChatHubType.DM)]
            public void Should_return_msg_explaining_usage_for_the_given_scenarios(
                string msg,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);

                var response = Responder.GetResponse(context);

                Assert.StartsWith("Beskrivning av kommando: beskriv\r\n", response.Text);
                Assert.Contains("s: ", response.Text);
            }

            [Theory]
            [InlineData("pbot beskriv -?a", true)]
            [InlineData("pbot beskriv -?", false)]
            [InlineData("beskriv -?a", true, SlackChatHubType.DM)]
            [InlineData("beskriv -?", false, SlackChatHubType.DM)]
            public void Should_toggle_explenation_of_admin_params_when_param_a_is_present(
                string msg,
                bool includesAdminParams,
                SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);

                var response = Responder.GetResponse(context);

                var adminParamExplenation = "\r\nu: <user>\r\n";

                if (includesAdminParams)
                    Assert.Contains(adminParamExplenation, response.Text);
                else
                    Assert.DoesNotContain(adminParamExplenation, response.Text);
            }

            [Theory]
            [InlineData("pbot beskriv sak 1", "pappabj0rn")]
            [InlineData("pbot beskriv -s 1", "pappabj0rn")]
            [InlineData("pbot beskriv eriska sak 1", "eriska")]
            [InlineData("pbot beskriv eriska -s 1", "eriska")]
            [InlineData("pbot beskriv -u eriska -s 1", "eriska")]
            public void Should_return_specified_item_descption(
                string msg,
                string targetInventory)
            {
                var itemTypeP = new Mock<ItemType>();
                var itemP = new Item(new Token(), itemTypeP.Object);

                var itemTypeE = new Mock<ItemType>();
                var itemE = new Item(new Token(), itemTypeE.Object);


                var expectedDict = new Dictionary<string, ItemType>
                    {
                        {"pappabj0rn", itemTypeP.Object},
                        { "eriska", itemTypeE.Object}
                    };

                PappaBj0rnInvetory.Items.Add(itemP);
                EriskaInvetory.Items.Add(itemE);

                var context = CreateContext(msg);

                var response = Responder.GetResponse(context);

                Assert.Equal(
                    nameof(IPhrasebook.ItemDescription), 
                    response.Text);

                Assert.Equal(
                    expectedDict[targetInventory], 
                    PhraseBookMock.Invocations.Last().Arguments[0]);
            }

            [Theory]
            [InlineData("pbot beskriv sak 2",nameof(IPhrasebook.DescribeItemToFewItems))]
            [InlineData("pbot beskriv eriska",nameof(IPhrasebook.DescribeUser))]
            [InlineData("pbot beskriv -u nisse",nameof(IPhrasebook.IDontKnowXxxNamedYyy),"nån","nisse")]
            [InlineData("pbot beskriv eriska sak 1",nameof(IPhrasebook.ItemDescription))]
            [InlineData("pbot beskriv -u eriska -s 1",nameof(IPhrasebook.ItemDescription))]
            [InlineData("pbot beskriv sak 0",nameof(IPhrasebook.Impossible))]
            [InlineData("pbot beskriv sak X",nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot beskriv -s X",nameof(IPhrasebook.IDidntUnderstand))]
            [InlineData("pbot beskriv X",nameof(IPhrasebook.IDidntUnderstand))]
            public void Should_respond_with_correct_message(
                string msg,
                string expectedMetodCall,
                string expectedPhParam1 = null,
                string expectedPhParam2 = null)
            {
                var typeArticle = "en";
                var typeName = "test2";

                var typeMock = new Mock<ItemType>();
                typeMock.Setup(x => x.IndefiniteAricle).Returns(typeArticle);
                typeMock.Setup(x => x.Name).Returns(typeName);
                var item = new Item(new Token(), typeMock.Object);

                PappaBj0rnInvetory.Items.Add(item);
                EriskaInvetory.Items.Add(item);

                var context = CreateContext(msg);

                var response = Responder.GetResponse(context);

                Assert.Equal(expectedMetodCall, response?.Text);

                if (expectedPhParam1 != null)
                    Assert.Equal(expectedPhParam1, PhraseBookMock.Invocations.Last().Arguments[0].ToString());

                if (expectedPhParam2 != null)
                    Assert.Equal(expectedPhParam2, PhraseBookMock.Invocations.Last().Arguments[1].ToString());
            }
        }
    }
}
