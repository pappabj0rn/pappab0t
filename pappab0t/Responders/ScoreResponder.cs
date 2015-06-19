using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Models;

namespace pappab0t.Responders
{
    /// <summary>
    ///     This responder makes MargieBot into a game! When a user says "@user+1" or similar in chat, Margie awards the
    ///     mentioned user a point. The
    ///     accompanying ScoreboardRequestResponder displays the scoreboard to chat.
    /// </summary>
    public class ScoreResponder : IResponder
    {
        private const string ScoreRegex = @"((?<formattedUserID><@(?<userID>U[a-zA-Z0-9]+)>)[\s,:]*)+?\+\s*1";

        // this responder holds a scorebook that keeps track of the score per teamID. We hold internal references
        // to the team we're scoring so we don't have to build the scorebook every time a response is requested, but
        // we still need to compare it to the ResponseContext's TeamID every time in case the bot is disconnected
        // and then connected to a different team.
        private Scorebook Scorebook { get; set; }
        private string TeamID { get; set; }

        public bool CanRespond(ResponseContext context)
        {
            if (Scorebook == null || TeamID != context.TeamID)
            {
                // start up scorebook for this team
                TeamID = context.TeamID;
                Scorebook = new Scorebook(TeamID);
            }
            // put the scorebook in context in case someone wants to see the scoreboard
            context.Set(Scorebook);

            return !context.Message.User.IsSlackbot && Regex.IsMatch(context.Message.Text, ScoreRegex);
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            // perform scoring
            var scoringResults = new List<ScoringResult>();

            // bet you anything there's a better way to do this
            Match match = Regex.Match(context.Message.Text, ScoreRegex);
            for (int i = 0; i < match.Groups["formattedUserID"].Captures.Count; i++)
            {
                scoringResults.Add(new ScoringResult
                {
                    FormattedUserID = match.Groups["formattedUserID"].Captures[i].Value,
                    IsNewScorer = !Scorebook.HasUserScored(match.Groups["userID"].Captures[i].Value),
                    IsValidScorer = (match.Groups["userID"].Captures[i].Value != context.Message.User.ID),
                    UserID = match.Groups["userID"].Captures[i].Value
                });
            }

            IList<string> newScorers = scoringResults.Where(r => r.IsNewScorer).Select(r => r.UserID).ToList();
            IList<string> scoringUsers = scoringResults.Where(r => r.IsValidScorer).Select(r => r.UserID).ToList();
            IList<string> allUsers = scoringResults.Select(r => r.UserID).ToList();

            // score the users and shove the scorebook into the context for use by the ScoreboardRequestResponder
            Scorebook.ScoreUsers(scoringUsers, 1);

            var phrasebook = context.Get<Phrasebook>();
            var responseBuilder = new StringBuilder();

            if (allUsers.Contains(context.Message.User.ID))
            {
                responseBuilder.Append(
                    string.Format(
                        "Bless your heart, {0}. You can't score yourself - what kinda game would that be?! Y'all, {0} is cute, but I think he/she might be dumb as a box o' rocks.\n\n",
                        context.Message.User.FormattedUserID));
            }

            if (scoringUsers.Any())
            {
                if (responseBuilder.Length > 0)
                {
                    responseBuilder.Append("Anyway... ");
                }

                if (scoringUsers.Count() == 1)
                {
                    if (scoringUsers[0] == context.BotUserID)
                    {
                        int margieScore = Scorebook.GetUserScore(context.BotUserID);
                        responseBuilder.Append(
                            string.Format(
                                "Awwww, aren't you a sweetie! *[blushes]* If you insist. Now I have {0} point{1}.\n\n",
                                margieScore, margieScore == 1 ? string.Empty : "s"));
                    }
                    else if (newScorers.Contains(scoringUsers[0]))
                    {
                        responseBuilder.Append(
                            string.Format("A new challenger appears, y'all! {0} is on the board with a point. {1}",
                                scoringResults.First(r => r.UserID == scoringUsers[0]).FormattedUserID,
                                phrasebook.GetAffirmation()));
                    }
                    else
                    {
                        var scoredUser = scoringResults.First(r => r.UserID == scoringUsers[0]);

                        responseBuilder.Append(
                            string.Format(
                                "{0} {1} just scored a point. {2} {1}, your score is now {3}.",
                                phrasebook.GetExclamation(),
                                scoredUser.FormattedUserID,
                                phrasebook.GetAffirmation(),
                                Scorebook.GetUserScore(scoredUser.UserID)
                                )
                            );
                    }
                }
                else
                {
                    responseBuilder.Append("There's points all over this joint, y'all. ");
                    IList<ScoringResult> scoringUserResults = scoringResults.Where(r => r.IsValidScorer).ToList();

                    if (scoringUserResults.Count == 2)
                    {
                        responseBuilder.Append(
                            string.Format(
                                "{0} and {1} each just scored a point. {2}",
                                scoringUserResults[0].FormattedUserID,
                                scoringUserResults[1].FormattedUserID,
                                phrasebook.GetAffirmation()
                                )
                            );
                    }
                    else
                    {
                        for (int i = 0; i < scoringUserResults.Count; i++)
                        {
                            responseBuilder.Append(scoringUserResults[i].FormattedUserID);

                            if (i < scoringResults.Count - 2)
                            {
                                responseBuilder.Append(", ");
                            }
                            else if (i == scoringResults.Count - 2)
                            {
                                responseBuilder.Append(", and ");
                            }
                        }

                        responseBuilder.Append(" each just scored a point. " + phrasebook.GetExclamation());
                    }
                }
            }

            return new BotMessage {Text = responseBuilder.ToString().Trim()};
        }

        private class ScoringResult
        {
            public string FormattedUserID { get; set; }
            public bool IsNewScorer { get; set; }
            public bool IsValidScorer { get; set; }
            public string UserID { get; set; }
        }
    }
}