using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class ScoreboardRequestResponder : IResponder
    {
        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM) &&
                   Regex.IsMatch(context.Message.Text, @"\bscore\b", RegexOptions.IgnoreCase);
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            IReadOnlyDictionary<string, int> scores = context.Get<Scorebook>().GetScores();

            if (scores.Count <= 0)
                return new BotMessage {Text = "Not a one-of-ya has scored yet. Come on, sleepyheads!"};

            var builder = new StringBuilder(context.Get<Phrasebook>().GetScoreboardHype());
            builder.Append("```");

            // add the scores to a list for sorting. while we do, figure out who has the longest name for the pseudo table formatting
            var sortedScores = new List<KeyValuePair<string, int>>();
            var longestName = string.Empty;

            foreach (var key in scores.Keys)
            {
                var newScore = new KeyValuePair<string, int>(context.UserNameCache[key], scores[key]);

                if (newScore.Key.Length > longestName.Length)
                {
                    longestName = newScore.Key;
                }

                sortedScores.Add(newScore);
            }
            sortedScores.Sort((x, y) => y.Value.CompareTo(x.Value));

            foreach (var userScore in sortedScores)
            {
                var nameString = new StringBuilder(userScore.Key);
                while (nameString.Length < longestName.Length)
                {
                    nameString.Append(" ");
                }

                builder.Append(nameString + " | " + userScore.Value + "\n");
            }

            builder.Append("```");

            return new BotMessage
            {
                Text = builder.ToString()
            };
        }
    }
}