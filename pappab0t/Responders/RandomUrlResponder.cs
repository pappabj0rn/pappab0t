using System;
using System.Linq;
using System.Text.RegularExpressions;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using Raven.Client;

namespace pappab0t.Responders
{
    public class RandomUrlResponder : ResponderBase, IExposedCapability
    {
        private const string TypeGroupName = "type";
        private const string UserGroupName = "user";
        private readonly IPhrasebook _phrasebook;
        private readonly IDocumentStore _store;

        public RandomUrlResponder(IPhrasebook phrasebook, IDocumentStore store)
        {
            _phrasebook = phrasebook;
            _store = store;
        }

        public override bool CanRespond(ResponseContext context)
        {
            var triggerMatch = Regex.Match(
                context.Message.Text,
                "^(?:[\\w]* )?(random url|ru)($| [.]*)",
                RegexOptions.IgnoreCase);

            return (context.Message.MentionsBot
                    || context.Message.ChatHub.Type == SlackChatHubType.DM)
                && triggerMatch.Success;
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;

            var matches = BuildRegexMatch(context);
            
            var typeFilter = BuildFilter(matches.typeMatch, matches.userMatch);

            using (var session = _store.OpenSession())
            {
                IQueryable<UserUrlPost> filteredQuery = null;
                var query = session
                    .Query<UserUrlPost>()
                    .Customize(x => x.RandomOrdering());

                if (typeFilter.filterByType)
                    filteredQuery = query
                        .Where(x => x.UrlMatchData.TargetType == typeFilter.type);

                if (typeFilter.filterByUser)
                    filteredQuery = (filteredQuery ?? query)
                        .Where(x => x.UserId == typeFilter.userId);

                var post = (filteredQuery ?? query)
                    .FirstOrDefault();

                if (post == null)
                    return new BotMessage {Text = _phrasebook.NoDataFound()};

                return new BotMessage
                {
                    Text = $"{post.Created:yy-MM-dd HH:mm} {context.UserNameCache[post.UserId]}: {post.UrlMatchData}"
                };
            }
        }

        private (bool filterByType, UrlTargetType type, bool filterByUser, string userId) 
            BuildFilter(Match typeMatch, Match userMatch)
        {
            var type = UrlTargetType.Other;
            var filterByType = !typeMatch.Groups[TypeGroupName].Value.IsNullOrEmpty();

            if (filterByType)
            {
                type = (UrlTargetType)Enum.Parse(
                    typeof(UrlTargetType),
                    typeMatch.Groups[TypeGroupName].Value,
                    ignoreCase: true);
            }

            var userId = "";
            var filterByUser = !userMatch.Groups[UserGroupName].Value.IsNullOrEmpty();

            if (filterByUser)
            {
                userId = Context
                    .UserNameCache
                    .Where(x=>x.Value == userMatch.Groups[UserGroupName].Value)
                    .Select(x=>x.Key)
                    .FirstOrDefault();
            }

            return (filterByType,type, filterByUser, userId);
        }

        private static (Match typeMatch, Match userMatch) BuildRegexMatch(ResponseContext context)
        {
            var types = Enum
                .GetValues(typeof(UrlTargetType))
                .Cast<UrlTargetType>()
                .Aggregate(
                    "",
                    (prev, next) => $"{prev}|(?<{TypeGroupName}>{next})",
                    s => s.TrimStart('|'));

            var typeMatch = Regex.Match(
                context.Message.Text,
                $"( (t:)?(?:{types}))",
                RegexOptions.IgnoreCase);

            var userMatch = Regex.Match(
                context.Message.Text,
                $"( u:(?<{UserGroupName}>[\\w]*))",
                RegexOptions.IgnoreCase);

            return (typeMatch, userMatch);
        }

        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "<random url|ru> [t:][video|image|document|music|other] [u:[username]]",
            Explatation = "Hämtar upp en sparad url. Urltyp och användarnamn kan användas för att begränsa urvalet."
        };
    }
}