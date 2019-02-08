using System.Linq;
using MargieBot;
using Moq;
using pappab0t.Models;
using pappab0t.Modules.Highscore;
using pappab0t.Tests.Responders;
using Xunit;

namespace pappab0t.Tests.Modules.HighScore
{
    public abstract class HighScoreManagerTests : ResponderTestsContext
    {
        protected HighScoreManager HighScoreManager;

        protected HighScoreManagerTests()
        {
            ConfigureRavenDB();

            HighScoreManager = new HighScoreManager(Store)
            {
                WaitForNonStaleResults = true
            };
        }

        public class Handle : HighScoreManagerTests
        {
            private const string EmptyHsName = "EmptyTestHighscore";
            private const string ExistingHsName = "ExistingTestHighscore";
            private const string FullgHsName = "FullTestHighscore";

            [Fact]
            public void Should_add_user_to_highscore_when_highscore_is_empty()
            {
                var result = HighScoreManager.Handle(EmptyHsName, PappaBj0rnUserId, 1);

                Assert.Equal(1, result);
            }

            [Fact]
            public void Should_add_user_to_highscore_when_highscore_exists()
            {
                HighScoreManager.Handle(ExistingHsName, PappaBj0rnUserId, 10);

                var result = HighScoreManager.Handle(ExistingHsName, PappaBj0rnUserId, 9);

                Assert.Equal(2, result);
            }

            [Fact]
            public void Should_not_add_user_to_highscore_when_highscore_is_full_and_users_has_lower_score_than_lowest_on_list()
            {
                for (var i = 0; i < 10; i++)
                {
                    HighScoreManager.Handle(FullgHsName, PappaBj0rnUserId, 100-i);
                }

                var score = 10;
                var result = HighScoreManager.Handle(FullgHsName, PappaBj0rnUserId, score);
                var hs = HighScoreManager.GetHighScore(FullgHsName);

                Assert.Equal(0, result);
                Assert.Equal(10, hs.GetScores().Count());
                Assert.True(score < hs.GetScores().Last().Value);
            }

            [Fact]
            public void Should_insert_user_into_highscore_when_highscore_exists_and_new_score_is_between_highscore_min_and_max()
            {
                HighScoreManager.Handle(ExistingHsName, PappaBj0rnUserId, 100);
                HighScoreManager.Handle(ExistingHsName, PappaBj0rnUserId, 10);

                var result = HighScoreManager.Handle(ExistingHsName, PappaBj0rnUserId, 25);

                Assert.Equal(2, result);
            }
        }
    }
}