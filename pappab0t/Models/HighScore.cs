using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace pappab0t.Models
{
    public class HighScore
    {
        public string Id { get; set; }
        public string Name { get; set; }
        private List<Score> Scores { get; set; }

        public HighScore()
        {
            Scores = new List<Score>();
        }

        public IEnumerable<Score> GetScores()
        {
            return Scores;
        }

        public int TryAddNewScore(Score s)
        {
            if (Scores.Count >= 10 && s.Value <= Scores.MinBy(x => x.Value).Value) 
                return -1;

            Scores.Add(s);

            Scores.Sort((s1, s2) => s2.Value.CompareTo(s1.Value));

            Scores = Scores.Take(10).ToList();

            return Scores.IndexOf(s)+1;
        }
        
    }

    public class Score
    {
        public string UserId { get; set; }
        public int Value { get; set; }
    }
}
