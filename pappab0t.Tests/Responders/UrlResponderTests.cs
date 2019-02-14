using System.Collections.Generic;
using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Models;
using pappab0t.Responders;
using Xunit;

namespace pappab0t.Tests.Responders
{
    public abstract class UrlResponderTests : ResponderTestsContext
    {
        private readonly Mock<IUrlParser> _urlParserMock;
        private readonly Mock<IPhrasebook> _phrasebookMock;
        private UrlResponder _responder;

        protected UrlResponderTests()
        {
            _urlParserMock = new Mock<IUrlParser>();
            _phrasebookMock = new Mock<IPhrasebook>();
            _phrasebookMock.SetupAllProperties();

            _responder = new UrlResponder(
                new UrlParser(), 
                _phrasebookMock.Object);
        }


        public class CanRespond : UrlResponderTests
        {
            [Theory]
            [InlineData("<http://www.nu/folder1/folder2/ost.jpg>", true)]
            [InlineData("här är en häst <http://www.nu/folder1/folder2/ost.jpg>, typ", true)]
            [InlineData("lite vanlig text", false)]
            [InlineData("dålig url <http", false)]
            [InlineData("dålig url <http>", false)]
            [InlineData("dålig url <ftp", false)]
            [InlineData("dålig url <ftp>", false)]
            [InlineData("<http://www.nu|www.nu>", true)]
            public void Should_handle_theory_data(string msg, bool expectedResult)
            {
                var context = CreateContext(msg);

                var canRespond = _responder.CanRespond(context);

                Assert.Equal(expectedResult,canRespond);
            }
        }

        public abstract class GetResponseBase : UrlResponderTests
        {
            protected UrlMatchData ParsedUrlData;
            protected ResponseContext ResponseContext;
            protected BotMessage Response;

            protected GetResponseBase()
            {
                ParsedUrlData = new UrlMatchData
                {
                    Protocol = "http",
                    Domain = "www.nu",
                    Path = "/folder1/folder2/",
                    FileName = "ost.jpg",
                    TargetType = UrlTargetType.Image
                };

                _urlParserMock.Setup(x => x.Parse(It.IsAny<string>()))
                    .Returns(()=>ParsedUrlData);

                ConfigureRavenDB();

                _responder = new UrlResponder(
                    _urlParserMock.Object,
                    _phrasebookMock.Object);
            }

            protected void UseDefaultContext()
            {
                ResponseContext = CreateContext("<http://www.nu/folder1/folder2/ost.jpg>");
            }

            protected void TriggerResponder()
            {
                _responder.CanRespond(ResponseContext);
                Response = _responder.GetResponse(ResponseContext);
            }

            protected void WaitForNonStaleUserUrlStats()
            {
                using (var session = Store.OpenSession())
                {
                    // ReSharper disable once UnusedVariable
                    var q = session
                        .Query<UserUrlStats>()
                        .Customize(x => x.WaitForNonStaleResults())
                        .FirstOrDefault(x => 
                            x.UserId == ResponseContext.Message.User.ID);
                }
            }
        }

        public class GetResponse : GetResponseBase
        {
            [Fact]
            public void Should_store_posted_url_data()
            {
                UseDefaultContext();

                TriggerResponder();

                List<UserUrlPost> storedData;

                using (var session = Store.OpenSession())
                {
                    storedData = session
                        .Query<UserUrlPost>()
                        .Customize(x=>x.WaitForNonStaleResults())
                        .ToList();
                }

                Assert.Equal(1, storedData.Count);
                var postedData = storedData.First();

                Assert.Equal(ParsedUrlData,postedData.UrlMatchData);
                Assert.Equal(ResponseContext.Message.User.ID,postedData.UserId);
            }
            
            [Fact]
            public void Should_store_user_url_stats_when_no_previous_record_is_stored()
            {
                UseDefaultContext();

                TriggerResponder();

                List<UserUrlStats> storedData;

                using (var session = Store.OpenSession())
                {
                    storedData = session
                        .Query<UserUrlStats>()
                        .Customize(x => x.WaitForNonStaleResults())
                        .ToList();
                }

                Assert.Equal(1, storedData.Count);
                var userUrlStats = storedData.First();

                Assert.Equal(ResponseContext.Message.User.ID, userUrlStats.UserId);

                Assert.Equal(1, userUrlStats.DomainCount.Count);
                Assert.Equal(ParsedUrlData.Domain, userUrlStats.DomainCount.First().Key);
                Assert.Equal(1, userUrlStats.DomainCount.First().Value);

                Assert.Equal(1, userUrlStats.TypeCount.Count);
                Assert.Equal(UrlTargetType.Image,userUrlStats.TypeCount.First().Key);
                Assert.Equal(1, userUrlStats.TypeCount.First().Value);
            }

            [Fact]
            public void Should_update_stored_user_url_stats()
            {
                UseDefaultContext();

                for (var i = 0; i < 2; i++)
                {
                    ParsedUrlData.Query = $"i={i}"; //make PUD unique
                    TriggerResponder();

                    WaitForNonStaleUserUrlStats();
                }

                List<UserUrlStats> storedData;

                using (var session = Store.OpenSession())
                {
                    storedData = session
                        .Query<UserUrlStats>()
                        .Customize(x => x.WaitForNonStaleResults())
                        .Where(x => x.UserId == ResponseContext.Message.User.ID)
                        .ToList();
                }

                Assert.Equal(1, storedData.Count);
                var userUrlStats = storedData.First();

                Assert.Equal(1, userUrlStats.DomainCount.Count);
                Assert.Equal(ParsedUrlData.Domain, userUrlStats.DomainCount.First().Key);
                Assert.Equal(2, userUrlStats.DomainCount.First().Value);

                Assert.Equal(1, userUrlStats.TypeCount.Count);
                Assert.Equal(UrlTargetType.Image, userUrlStats.TypeCount.First().Key);
                Assert.Equal(2, userUrlStats.TypeCount.First().Value);
            }

            [Fact]
            public void Should_keep_quiet_after_storing_url()
            {
                UseDefaultContext();
                TriggerResponder();

                Assert.Null(Response);
            }

            [Fact]
            public void Should_question_post_when_there_are_similar_posts_in_db()
            {
                _phrasebookMock
                    .Setup(x => x.QuestionSimilarUrl())
                    .Returns("poff");

                UseDefaultContext();
                TriggerResponder();

                WaitForNonStaleUserUrlStats();

                ParsedUrlData = new UrlMatchData
                {
                    Protocol = "http",
                    Domain = "www.nu",
                    Path = "/folder1/folder2/",
                    Query = "test=true",
                    FileName = "ost.jpg",
                    TargetType = UrlTargetType.Image
                };

                ResponseContext = CreateContext("<http://www.nu/folder1/folder2/ost.jpg?test=true>");

                TriggerResponder();

                Assert.StartsWith(_phrasebookMock.Object.QuestionSimilarUrl(), Response.Text);
            }

            public class CaseUrlPostedPreviously : GetResponseBase
            {
                public CaseUrlPostedPreviously()
                {
                    _phrasebookMock
                        .Setup(x => x.TauntOld())
                        .Returns("oäz!");

                    _phrasebookMock
                        .Setup(x => x.CreditUserBecause(It.IsAny<string>(),It.IsAny<string>()))
                        .Returns("op");

                    UseDefaultContext();
                    TriggerResponder();

                    WaitForNonStaleUserUrlStats();
                }

                [Fact]
                public void Should_not_store_posted_url_data()
                {
                    TriggerResponder();

                    List<UserUrlStats> storedStats;
                    List<UserUrlPost> storedPosts;

                    using (var session = Store.OpenSession())
                    {
                        storedStats = session
                            .Query<UserUrlStats>()
                            .Customize(x=>x.WaitForNonStaleResults())
                            .ToList();

                        storedPosts = session
                            .Query<UserUrlPost>()
                            .Customize(x => x.WaitForNonStaleResults())
                            .ToList();
                    }

                    Assert.Equal(1, storedStats.Count);
                    var userUrlStats = storedStats.First();

                    Assert.Equal(1, userUrlStats.DomainCount.Count);
                    Assert.Equal(ParsedUrlData.Domain, userUrlStats.DomainCount.First().Key);
                    Assert.Equal(1, userUrlStats.DomainCount.First().Value);

                    Assert.Equal(1, userUrlStats.TypeCount.Count);
                    Assert.Equal(UrlTargetType.Image, userUrlStats.TypeCount.First().Key);
                    Assert.Equal(1, userUrlStats.TypeCount.First().Value);

                    Assert.Equal(1, storedPosts.Count);
                }

                [Fact]
                public void Should_taunt_poster()
                {
                    TriggerResponder();

                    Assert.StartsWith(_phrasebookMock.Object.TauntOld(), Response.Text);
                }

                [Fact]
                public void Should_credit_original_poster()
                {
                    var opUuid = ResponseContext.Message.User.ID;
                    
                    ResponseContext.Message.User.ID = "anotherUserUUID";

                    TriggerResponder();

                    Assert.EndsWith(_phrasebookMock.Object.CreditUserBecause(opUuid,"OP"), Response.Text);
                }
            }

            public class CasePostedInPrivate : GetResponseBase
            {
                public CasePostedInPrivate()
                {
                    UseDefaultContext();
                    ResponseContext.Message.ChatHub.Type = SlackChatHubType.DM;
                }

                [Fact]
                public void Should_not_store_posted_url_data()
                {
                    TriggerResponder();

                    List<UserUrlStats> storedStats;
                    List<UserUrlPost> storedPosts;

                    using (var session = Store.OpenSession())
                    {
                        storedStats = session
                            .Query<UserUrlStats>()
                            .Customize(x => x.WaitForNonStaleResults())
                            .ToList();

                        storedPosts = session
                            .Query<UserUrlPost>()
                            .Customize(x => x.WaitForNonStaleResults())
                            .ToList();
                    }

                    Assert.Equal(0, storedStats.Count);
                    Assert.Equal(0, storedPosts.Count);
                }

                [Fact]
                public void Should_question_poster()
                {
                    _phrasebookMock
                        .Setup(x => x.QuestionAction())
                        .Returns("?");

                    TriggerResponder();

                    Assert.Equal(
                        _phrasebookMock
                            .Object
                            .QuestionAction(),
                        Response.Text);
                }
            }
        }
    }
}
