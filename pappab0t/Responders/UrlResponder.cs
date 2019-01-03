using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;
using pappab0t.Extensions;
using pappab0t.Models;
using Raven.Client;

namespace pappab0t.Responders
{
    public class UrlResponder : ResponderBase
    {
        private readonly IUrlParser _urlParser;
        private readonly IPhrasebook _phrasebook;
        private UrlMatchData _urlMatchData;

        public UrlResponder(IUrlParser urlParser, IPhrasebook phrasebook)
        {
            _urlParser = urlParser;
            _phrasebook = phrasebook;
        }

        public override bool CanRespond(ResponseContext context)
        {
            return MatchUrl(context);
        }

        private bool MatchUrl(ResponseContext context)
        {
            var startOfUrl = Math.Max(
                context.Message.Text.IndexOf("<http"), 
                context.Message.Text.IndexOf("<ftp"));

            if (startOfUrl == -1)
                return false;

            var endOfUrl = context.Message.Text.IndexOf(">", startOfUrl);
            if (endOfUrl == -1)
                return false;

            _urlMatchData = _urlParser.Parse(
                context.Message.Text.Substring(
                    startOfUrl,
                    endOfUrl-startOfUrl));

            return !(_urlMatchData == UrlMatchData.Empty);
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            if(context.Message.ChatHub.Type == SlackChatHubType.DM)
                return new BotMessage { Text = _phrasebook.QuestionAction() };

            var store = context.Get<IDocumentStore>();

            using (var session = store.OpenSession())
            {
                var existingPost = session
                    .Query<UserUrlPost>()
                    .FirstOrDefault(x=>x.UrlMatchData == _urlMatchData);

                if (existingPost != null)
                    return new BotMessage
                    {
                        Text = 
                            _phrasebook.TauntOld() 
                            + " "
                            + _phrasebook.CreditUserBecauseFormat().With(
                                existingPost.UserId,
                                "OP")
                    };

                var udSansQuery = new UrlMatchData(_urlMatchData)
                {
                    Query = null
                };
                existingPost = session
                    .Query<UserUrlPost>()
                    .FirstOrDefault(x => x.UrlMatchData == udSansQuery);

                StorePostedData(context, session);
                UpdateUserStats(context, session);
                session.SaveChanges();

                if (existingPost != null)
                    return new BotMessage
                    {
                        Text = _phrasebook.QuestionSimilarUrl()
                    };
            }

            return null;
        }

        private void StorePostedData(ResponseContext context, IDocumentSession session)
        {
            var postData = new UserUrlPost
            {
                UrlMatchData = _urlMatchData,
                UserId = context.Message.User.ID
            };
            session.Store(postData);
        }

        private void UpdateUserStats(ResponseContext context, IDocumentSession session)
        {
            var stats = session
                            .Query<UserUrlStats>()
                            .FirstOrDefault(x => x.UserId == context.Message.User.ID)
                        ?? new UserUrlStats
                        {
                            UserId = context.Message.User.ID,
                            DomainCount = new Dictionary<string, int>(),
                            TypeCount = new Dictionary<UrlTargetType, int>()
                        };

            IncreaseDomainCount(stats);

            IncreaseTypeCount(stats);

            session.Store(stats);
        }

        private void IncreaseDomainCount(UserUrlStats stats)
        {
            if (!stats.DomainCount.ContainsKey(_urlMatchData.Domain))
                stats.DomainCount.Add(_urlMatchData.Domain, 0);

            stats.DomainCount[_urlMatchData.Domain]++;
        }

        private void IncreaseTypeCount(UserUrlStats stats)
        {
            if (!stats.TypeCount.ContainsKey(_urlMatchData.TargetType))
                stats.TypeCount.Add(_urlMatchData.TargetType, 0);

            stats.TypeCount[_urlMatchData.TargetType]++;
        }
    }
}