using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Models;
using pappab0t.Extensions;

namespace pappab0t.Responders
{
    public class HighScoreResponder : ResponderBase, IExposedCapability
    {
        private const string GameNameKey = "gameName";
        private const string HighScoreRegex = @"(?:\bhighscore\b|\bhs\b)(\s+(?<" + GameNameKey + @">\w+))?";

        private string _gameName;

        public override bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM)
                   && Regex.IsMatch(context.Message.Text, HighScoreRegex, RegexOptions.IgnoreCase);
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;

            var match = Regex.Match(Context.Message.Text, HighScoreRegex, RegexOptions.IgnoreCase);
            _gameName = match.Groups[GameNameKey].Value.IsNullOrEmpty()
                        ? String.Empty
                        : match.Groups[GameNameKey].Value;

            string messageText;
            using (var session = DocumentStore.OpenSession())
            {
                if (_gameName.IsNullOrEmpty())
                {
                    var lists = session.Query<HighScore>().ToList();

                    if (!lists.Any())
                        return new BotMessage { Text = "Jag hittar tyvärr inte en enda lista med poäng." };


                    messageText = BuildMultipleListsTextMessage(lists);
                }
                else
                {
                    var hs = session.Query<HighScore>()
                                    .FirstOrDefault(x => x.Name.Equals(_gameName, StringComparison.InvariantCultureIgnoreCase));

                    if (hs == null)
                        return new BotMessage { Text = "Jag har ingen lista som heter så." };

                    messageText = BuildSingleListTestMessage(hs);
                }
            }

            return new BotMessage { Text = messageText };
        }

        private string BuildSingleListTestMessage(HighScore hs)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Ställningen för {0}:\n", hs.Name);

            var i = 1;
            foreach (var score in hs.GetScores())
            {
                sb.AppendFormat("{0}: {1}p - {2}\n", i++, score.Value, Context.UserNameCache[score.UserId]);
            }

            return sb.ToString();
        }

        private string BuildMultipleListsTextMessage(IEnumerable<HighScore> highScores)
        {
            var sb = new StringBuilder("Ledarna i alla spel:\n");

            foreach (var highScore in highScores)
            {
                var leader = highScore.GetScores().First();
                sb.AppendFormat("{0}: {1}p - {2}\n", highScore.Name, leader.Value, Context.UserNameCache[leader.UserId]);
            }

            return sb.ToString();
        }

        public ExposedInformation Info
        {
            get
            {
                return new ExposedInformation
                {
                    Usage = "highscore|hs [spelnamn]",
                    Explatation = "Visar högsta poäng för alla spel eller hela listan för ett givet spel."
                };
            }
        }
    }
}