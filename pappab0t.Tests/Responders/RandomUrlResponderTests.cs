using System;
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
                { "userUUID1","eriska" },
                { "userUUID2","falfa" },
            };

            Responder = new RandomUrlResponder(_phrasebookMock.Object, _documentStore);
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
            [InlineData("pbot ru video", true)]
            [InlineData("pbot ru music", true)]
            [InlineData("pbot ru image", true)]
            [InlineData("pbot ru document", true)]
            [InlineData("pbot ru other", true)]
            [InlineData("pbot ru t:video", true)]
            [InlineData("pbot ru t:music", true)]
            [InlineData("pbot ru t:image", true)]
            [InlineData("pbot ru t:document", true)]
            [InlineData("pbot ru t:other", true)]
            [InlineData("pbot ru t:video u:falfa", true)]
            [InlineData("pbot ru u:falfa", true)]
            [InlineData("pbot ru", true)]
            [InlineData("pbot rux", false)]
            [InlineData("pbot random urlx", false)]

            [InlineData("random url video", true, SlackChatHubType.DM)]
            [InlineData("random url music", true, SlackChatHubType.DM)]
            [InlineData("random url image", true, SlackChatHubType.DM)]
            [InlineData("random url document", true, SlackChatHubType.DM)]
            [InlineData("random url other", true, SlackChatHubType.DM)]
            [InlineData("random url", true, SlackChatHubType.DM)]
            [InlineData("random url bogusType", true, SlackChatHubType.DM)] // I want it to respond to it, but it should say it's an invalid type
            [InlineData("ru video", true, SlackChatHubType.DM)]
            [InlineData("ru music", true, SlackChatHubType.DM)]
            [InlineData("ru image", true, SlackChatHubType.DM)]
            [InlineData("ru document", true, SlackChatHubType.DM)]
            [InlineData("ru other", true, SlackChatHubType.DM)]
            [InlineData("ru t:video", true, SlackChatHubType.DM)]
            [InlineData("ru t:music", true, SlackChatHubType.DM)]
            [InlineData("ru t:image", true, SlackChatHubType.DM)]
            [InlineData("ru t:document", true, SlackChatHubType.DM)]
            [InlineData("ru t:other", true, SlackChatHubType.DM)]
            [InlineData("ru t:video u:falfa", true, SlackChatHubType.DM)]
            [InlineData("ru u:falfa", true, SlackChatHubType.DM)]
            [InlineData("ru", true, SlackChatHubType.DM)]
            [InlineData("rux", false, SlackChatHubType.DM)]
            [InlineData("random urlx", false, SlackChatHubType.DM)]

            [InlineData("random url video", false)]
            [InlineData("random url music", false)]
            [InlineData("random url image", false)]
            [InlineData("random url document", false)]
            [InlineData("random url other", false)]
            [InlineData("random url", false)]
            public void Should_respond_to_random_url_requests(string msg, bool expectedResult, SlackChatHubType hubType = SlackChatHubType.Channel)
            {
                var context = CreateContext(msg, hubType);

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
                    UserId = _userNameCache.First().Key,
                    UrlMatchData = new UrlMatchData
                    {
                        Domain = "www.nu",
                        TargetType = UrlTargetType.Other
                    }
                };

                _otherPost2 = new UserUrlPost
                {
                    UserId = _userNameCache.Skip(1).First().Key,
                    UrlMatchData = new UrlMatchData
                    {
                        Domain = "dn.se",
                        TargetType = UrlTargetType.Other
                    }
                };

                _videoPost1 = new UserUrlPost
                {
                    UserId = _userNameCache.First().Key,
                    UrlMatchData = new UrlMatchData
                    {
                        Domain = "youtube.com",
                        TargetType = UrlTargetType.Video
                    }
                };

                _videoPost2 = new UserUrlPost
                {
                    UserId = _userNameCache.Skip(1).First().Key,
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
                        .Customize(x => x.RandomOrdering())
                        .ToList();
                }
            }

            [Theory]
            [InlineData("pbot random url image", SlackChatHubType.Channel)]
            [InlineData("pbot ru image", SlackChatHubType.Channel)]
            [InlineData("pbot ru t:image", SlackChatHubType.Channel)]
            [InlineData("random url image", SlackChatHubType.DM)]
            [InlineData("ru image", SlackChatHubType.DM)]
            [InlineData("ru t:image", SlackChatHubType.DM)]
            public void Should_state_when_there_are_no_posts_of_given_type(string text, SlackChatHubType hubType)
            {
                var msg = "hitta inge";
                _phrasebookMock.Setup(x => x.NoDataFound())
                    .Returns(msg);

                var context = CreateContext(text,hubType);

                var response = Responder.GetResponse(context);

                Assert.Equal(msg, response.Text);
            }

            [Theory]
            [InlineData("pbot random url", SlackChatHubType.Channel)]
            [InlineData("pbot ru", SlackChatHubType.Channel)]
            [InlineData("random url", SlackChatHubType.DM)]
            [InlineData("ru", SlackChatHubType.DM)]
            public void Should_query_for_random_url_of_any_type_when_no_type_is_given(string text, SlackChatHubType hubType)
            {
                var context = CreateContext(text,hubType);

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

            [Theory]
            [InlineData("pbot random url video", SlackChatHubType.Channel)]
            [InlineData("pbot ru video", SlackChatHubType.Channel)]
            [InlineData("pbot ru t:video", SlackChatHubType.Channel)]
            [InlineData("random url video", SlackChatHubType.DM)]
            [InlineData("ru video", SlackChatHubType.DM)]
            [InlineData("ru t:video", SlackChatHubType.DM)]
            public void Should_query_for_random_url_with_given_type(string text, SlackChatHubType hubType)
            {
                var context = CreateContext(text, hubType);

                var used = AssertPostsUsed(context);

                Assert.False(used.Other1);
                Assert.False(used.Other2);
                Assert.True(used.Video1);
                Assert.True(used.Video2);
            }

            [Theory]
            [InlineData("pbot random url u:falfa", "falfa", SlackChatHubType.Channel)]
            [InlineData("pbot ru u:falfa", "falfa", SlackChatHubType.Channel)]
            [InlineData("random url u:falfa", "falfa", SlackChatHubType.DM)]
            [InlineData("ru u:falfa", "falfa", SlackChatHubType.DM)]
            public void Should_query_for_random_url_with_given_user(string text, string user, SlackChatHubType hubType)
            {
                var context = CreateContext(text, hubType);

                var used = AssertPostsUsed(context);

                Assert.Equal(
                    _userNameCache[_otherPost1.UserId]
                        .Equals(user,StringComparison.InvariantCultureIgnoreCase), 
                    used.Other1);

                Assert.Equal(
                    _userNameCache[_otherPost2.UserId]
                        .Equals(user, StringComparison.InvariantCultureIgnoreCase),
                    used.Other2);

                Assert.Equal(
                    _userNameCache[_videoPost1.UserId]
                        .Equals(user, StringComparison.InvariantCultureIgnoreCase),
                    used.Video1);

                Assert.Equal(
                    _userNameCache[_videoPost2.UserId]
                        .Equals(user, StringComparison.InvariantCultureIgnoreCase),
                    used.Video2);
            }

            [Theory]
            [InlineData("pbot random url t:video u:falfa", "falfa", SlackChatHubType.Channel)]
            [InlineData("pbot ru t:video u:falfa", "falfa", SlackChatHubType.Channel)]
            [InlineData("random url t:video u:falfa", "falfa", SlackChatHubType.DM)]
            [InlineData("ru t:video u:falfa", "falfa", SlackChatHubType.DM)]
            public void Should_query_for_random_url_with_given_type_and_user(string text, string user, SlackChatHubType hubType)
            {
                var context = CreateContext(text, hubType);

                var used = AssertPostsUsed(context);

                Assert.False(used.Other1);
                Assert.False(used.Other2);
                Assert.False(used.Video1);
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
