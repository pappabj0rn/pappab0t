using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Models;
using pappab0t.Responders;
using Raven.Client;
using Raven.Client.Embedded;
using Xunit;
using Xunit.Sdk;

namespace pappab0t.Tests.Responders
{
    public abstract class RandomUrlResponderTests : ResponderTestsBase
    {
        protected readonly RandomUrlResponder Responder;

        private readonly Mock<IPhrasebook> _phrasebookMock;
        private readonly IDocumentStore _documentStore;
        private readonly Dictionary<string, string> _userNameCache;

        protected RandomUrlResponderTests()
        {
            _phrasebookMock = new Mock<IPhrasebook>();
            _documentStore = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };
            _documentStore.Initialize();

            _userNameCache = new Dictionary<string, string>
            {
                { "userUUID1","Göran" },
                { "userUUID2","Nisse" },
                { "userUUID3","Bossse" },
                { "userUUID4","Putas" },
            };

            Responder = new RandomUrlResponder(_phrasebookMock.Object, _documentStore);
        }

        protected ResponseContext CreateRandomUrlContext(string msg)
        {
            var context = CreateResponseContext(
                msg,
                SlackChatHubType.Channel,
                mentionsBot: msg.StartsWith("pbot"));

            context.UserNameCache = _userNameCache;
            return context;
        }

        public class CanRespond : RandomUrlResponderTests
        {
            [Theory]
            [InlineData("pbot random url video", true)]
            [InlineData("pbot random url music", true)]
            [InlineData("pbot random url image", true)]
            [InlineData("pbot random url document", true)]
            [InlineData("pbot random url other", true)]
            [InlineData("pbot random url", true)]
            [InlineData("pbot random url bogusType", true)] // I want it to respond to it, but it should say it's an invalid type
            [InlineData("pbot nonrandom url", false)]
            [InlineData("pbot random potato", false)]

            [InlineData("random url video", false)]
            [InlineData("random url music", false)]
            [InlineData("random url image", false)]
            [InlineData("random url document", false)]
            [InlineData("random url other", false)]
            [InlineData("random url", false)]
            public void Should_respond_to_random_url_requests(string msg, bool expectedResult)
            {
                var context = CreateRandomUrlContext(msg);

                var result = Responder.CanRespond(context);

                Assert.Equal(expectedResult, result);
            }
        }

        public class GetResponse : RandomUrlResponderTests
        {
            private UserUrlPost _otherPost1;
            private UserUrlPost _otherPost2;
            private UserUrlPost _videoPost1;
            private UserUrlPost _videoPost2;

            public GetResponse()
            {
                InsertTestData();
            }

            private void InsertTestData()
            {
                _otherPost1 = new UserUrlPost
                {
                    UserId = "userUUID1",
                    UrlMatchData = new UrlMatchData
                    {
                        Domain = "www.nu",
                        TargetType = UrlTargetType.Other
                    }
                };

                _otherPost2 = new UserUrlPost
                {
                    UserId = "userUUID2",
                    UrlMatchData = new UrlMatchData
                    {
                        Domain = "dn.se",
                        TargetType = UrlTargetType.Other
                    }
                };

                _videoPost1 = new UserUrlPost
                {
                    UserId = "userUUID3",
                    UrlMatchData = new UrlMatchData
                    {
                        Domain = "youtube.com",
                        TargetType = UrlTargetType.Video
                    }
                };

                _videoPost2 = new UserUrlPost
                {
                    UserId = "userUUID4",
                    UrlMatchData = new UrlMatchData
                    {
                        Domain = "vimeo.com",
                        TargetType = UrlTargetType.Video
                    }
                };

                using (var session = _documentStore.OpenSession())
                {
                    session.Store(_otherPost1);
                    session.Store(_otherPost2);
                    session.Store(_videoPost1);
                    session.Store(_videoPost2);

                    session.SaveChanges();

                    // ReSharper disable once UnusedVariable
                    var q = session.Query<UserUrlPost>()
                        .Customize(x => x.WaitForNonStaleResults())
                        .Customize(x=>x.RandomOrdering())
                        .ToList();
                }
            }

            [Fact]
            public void Should_respond_with_invalid_type()
            {
                var msg = "fel typ";
                _phrasebookMock.Setup(x => x.InvalidType())
                    .Returns(msg);

                var context = CreateRandomUrlContext("pbot random url bogusType");

                var response = Responder.GetResponse(context);

                Assert.Equal(msg, response.Text);
            }

            [Fact]
            public void Should_query_for_random_url_of_any_type_when_no_type_is_given()
            {
                var context = CreateRandomUrlContext("pbot random url");

                var otherPost1Used = false;
                var otherPost2Used = false;
                var videoPost1Used = false;
                var videoPost2Used = false;

                for (var i = 0; i < 1000; i++)
                {
                    var response = Responder.GetResponse(context);

                    try
                    {
                        AssertPostUsed(_otherPost1, response);
                        otherPost1Used = true;
                    }
                    catch (EqualException)
                    {
                        try
                        {
                            AssertPostUsed(_otherPost2, response);
                            otherPost2Used = true;
                        }
                        catch (EqualException)
                        {
                            try
                            {
                                AssertPostUsed(_videoPost1, response);
                                videoPost1Used = true;
                            }
                            catch (EqualException)
                            {
                                AssertPostUsed(_videoPost2, response);
                                videoPost2Used = true;
                            }
                        }
                    }
                }

                Assert.True(otherPost1Used);
                Assert.True(otherPost2Used);
                Assert.True(videoPost1Used);
                Assert.True(videoPost2Used);
            }
            
            [SuppressMessage("ReSharper", "UnusedParameter.Local")]
            private void AssertPostUsed(UserUrlPost post, BotMessage response)
            {
                Assert.Equal(post.TimeStamp, 
                    response.Attachments[0].TimeStamp);

                Assert.Equal(_userNameCache[post.UserId], 
                    response.Attachments[0].AuthorName);

                Assert.Equal(post.UrlMatchData.ToString(),
                    response.Attachments[0].Title);

                Assert.Equal(post.UrlMatchData.ToString(),
                    response.Attachments[0].TitleLink);
            }

            [Fact]
            public void Should_query_for_random_url_with_given_type()
            {
                var context = CreateRandomUrlContext("pbot random url video");

                var otherPost1Used = false;
                var otherPost2Used = false;
                var videoPost1Used = false;
                var videoPost2Used = false;

                for (var i = 0; i < 1000; i++)
                {
                    var response = Responder.GetResponse(context);

                    try
                    {
                        AssertPostUsed(_otherPost1, response);
                        otherPost1Used = true;
                    }
                    catch (EqualException)
                    {
                        try
                        {
                            AssertPostUsed(_otherPost2, response);
                            otherPost2Used = true;
                        }
                        catch (EqualException)
                        {
                            try
                            {
                                AssertPostUsed(_videoPost1, response);
                                videoPost1Used = true;
                            }
                            catch (EqualException)
                            {
                                AssertPostUsed(_videoPost2, response);
                                videoPost2Used = true;
                            }
                        }
                    }
                }

                Assert.False(otherPost1Used);
                Assert.False(otherPost2Used);
                Assert.True(videoPost1Used);
                Assert.True(videoPost2Used);
            }

            [Fact]
            public void Should_state_when_there_are_no_posts_of_given_type()
            {
                var msg = "hitta inge";
                _phrasebookMock.Setup(x => x.NoDataFound())
                    .Returns(msg);

                var context = CreateRandomUrlContext("pbot random url image");

                var response = Responder.GetResponse(context);

                Assert.Equal(msg, response.Text);
            }
        }
    }
}
