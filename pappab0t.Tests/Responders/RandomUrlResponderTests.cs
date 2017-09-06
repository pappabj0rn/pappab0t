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
            public void Should_state_when_there_are_no_posts_of_given_type()
            {
                var msg = "hitta inge";
                _phrasebookMock.Setup(x => x.NoDataFound())
                    .Returns(msg);

                var context = CreateRandomUrlContext("pbot random url image");

                var response = Responder.GetResponse(context);

                Assert.Equal(msg, response.Text);
            }

            [Fact]
            public void Should_query_for_random_url_of_any_type_when_no_type_is_given()
            {
                var context = CreateRandomUrlContext("pbot random url");

                var used = AssertPostsUsed(context);

                Assert.True(used.Other1);
                Assert.True(used.Other2);
                Assert.True(used.Video1);
                Assert.True(used.Video2);
            }

            private UsedPosts AssertPostsUsed(ResponseContext context)
            {
                var up = new UsedPosts();

                for (var i = 0; i < 100; i++)
                {
                    var response = Responder.GetResponse(context);

                    try
                    {
                        AssertPostUsed(_otherPost1, response);
                        up.Other1 = true;
                    }
                    catch (EqualException)
                    {
                        try
                        {
                            AssertPostUsed(_otherPost2, response);
                            up.Other2 = true; ;
                        }
                        catch (EqualException)
                        {
                            try
                            {
                                AssertPostUsed(_videoPost1, response);
                                up.Video1 = true;
                            }
                            catch (EqualException)
                            {
                                AssertPostUsed(_videoPost2, response);
                                up.Video2 = true;
                            }
                        }
                    }
                }

                return up;
            }
            
            [SuppressMessage("ReSharper", "UnusedParameter.Local")]
            private void AssertPostUsed(UserUrlPost post, BotMessage response)
            {
                Assert.Equal(
                    $"{post.Created:yy-MM-dd HH:mm} " +
                    $"{_userNameCache[post.UserId]}: " +
                    $"{post.UrlMatchData}",
                    response.Text);
            }

            [Fact]
            public void Should_query_for_random_url_with_given_type()
            {
                var context = CreateRandomUrlContext("pbot random url video");

                var used = AssertPostsUsed(context);

                Assert.False(used.Other1);
                Assert.False(used.Other2);
                Assert.True(used.Video1);
                Assert.True(used.Video2);
            }
        }
    }

    internal class UsedPosts
    {
        public bool Other1 { get; set; }
        public bool Other2 { get; set; }
        public bool Video1 { get; set; }
        public bool Video2 { get; set; }
    }
}
