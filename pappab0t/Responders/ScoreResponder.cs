using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    /// <summary>
    ///     This responder makes MargieBot into a game! When a user says "@user+1" or similar in chat, Margie awards the
    ///     mentioned user a point. The
    ///     accompanying ScoreboardRequestResponder displays the scoreboard to chat.
    /// </summary>
    public class ScoreResponder : IResponder, IExposedCapability
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
            var match = Regex.Match(context.Message.Text, ScoreRegex);
            for (var i = 0; i < match.Groups["formattedUserID"].Captures.Count; i++)
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
                responseBuilder.Append( //TODO in i frasboken med knasiga utryck
                    "@{0}. Du kan inte ge dig själv poäng - vad skulle det vara för ett spel?. Hörrni, {0} är ju för söt, men jag undrar om alla indianer ror åt samma håll.\n\n"
                        .With(context.Message.User.FormattedUserID));
            }

            if (scoringUsers.Any())
            {
                if (responseBuilder.Length > 0)
                {
                    responseBuilder.Append("Hur som... ");
                }

                if (scoringUsers.Count() == 1)
                {
                    if (scoringUsers[0] == context.BotUserID)
                    {
                        int margieScore = Scorebook.GetUserScore(context.BotUserID);
                        responseBuilder.Append(
                            "Åh vad du är go! Om du insisterar. Då har jag {0} poäng.\n\n"
                                .With(margieScore));
                    }
                    else if (newScorers.Contains(scoringUsers[0]))
                    {
                        responseBuilder.Append(
                            "En ny deltagare! {0} är nu med på listan med en poäng. {1}"
                                .With(
                                        scoringResults.First(r => r.UserID == scoringUsers[0]).FormattedUserID,
                                        phrasebook.GetAffirmation())
                                    );
                    }
                    else
                    {
                        var scoredUser = scoringResults.First(r => r.UserID == scoringUsers[0]);

                        responseBuilder.Append(
                            "{0} {1} fick just en poäng. {2} {1}, du har nu {3}."
                            .With(
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
                    responseBuilder.Append("Poäng lite här och var.");
                    IList<ScoringResult> scoringUserResults = scoringResults.Where(r => r.IsValidScorer).ToList();

                    if (scoringUserResults.Count == 2)
                    {
                        responseBuilder.Append(
                            "{0} och {1} fick varsin poäng. {2}"
                            .With(
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
                                responseBuilder.Append(", och ");
                            }
                        }

                        responseBuilder.Append(" fick just en poäng var. " + phrasebook.GetExclamation());
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

        public string Usage
        {
            get { return "@(nickname) +1\n>Ger en poäng till specifierad användare."; }
        }
    }
}