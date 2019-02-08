using System;
using System.Linq;
using pappab0t.Models;
using Raven.Client;

namespace pappab0t.Modules.Highscore
{
    public class HighScoreManager : RavenDbManager<HighScore>, IHighScoreManager
    {
        private readonly IDocumentStore _store;

        public HighScoreManager(IDocumentStore store)
        {
            _store = store;
        }

        public int Handle(string highScoreName, string userId, int score)
        {
            using (var session = _store.OpenSession())
            {
                var hs = session
                             .Query<HighScore>()
                             .SingleOrDefault(x => x.Name == highScoreName)
                         ?? new HighScore{Name = highScoreName};

                var pos = hs.TryAddNewScore(new Score {UserId = userId, Value = score});

                if (pos > 0)
                {
                    session.Store(hs);
                    session.SaveChanges();
                }

                WaitForNonStaleResultsIfEnabled(session);

                return Math.Max(0, pos);
            }
        }

        public HighScore GetHighScore(string highScoreName)
        {
            using (var session = _store.OpenSession())
            {
                return session
                    .Query<HighScore>()
                    .FirstOrDefault(x => x.Name == highScoreName);
            }
        }
    }
}