using System;
using System.Collections.Generic;
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
        private readonly IPhrasebook _phrasebook;
        private readonly IDocumentStore _store;

        public RandomUrlResponder(IPhrasebook phrasebook, IDocumentStore store)
        {
            _phrasebook = phrasebook;
            _store = store;
        }

        public override bool CanRespond(ResponseContext context)
        {
            return context.Message.MentionsBot
                && context.Message.Text.Contains(" random url");
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            var regexMatch = BuildRegexMatch(context);

            if(!regexMatch.Success)
                return new BotMessage
                {
                    Text = _phrasebook.InvalidType()
                };
            
            var typeFilter = BuildTypeFilter(regexMatch);

            using (var session = _store.OpenSession())
            {
                IQueryable<UserUrlPost> filteredQuery = null;
                var query = session
                    .Query<UserUrlPost>()
                    .Customize(x => x.RandomOrdering());

                if (typeFilter.filterByType)
                    filteredQuery = query
                        .Where(x => x.UrlMatchData.TargetType == typeFilter.type);

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

        private static (bool filterByType, UrlTargetType type) BuildTypeFilter(Match regexMatch)
        {
            var type = UrlTargetType.Other;
            var filterByType = !regexMatch.Groups["type"].Value.IsNullOrEmpty();

            if (filterByType)
            {
                type = (UrlTargetType) Enum.Parse(
                    typeof(UrlTargetType),
                    regexMatch.Groups["type"].Value,
                    ignoreCase: true);
            }
            return (filterByType,type);
        }

        private static Match BuildRegexMatch(ResponseContext context)
        {
            var types = Enum
                .GetValues(typeof(UrlTargetType))
                .Cast<UrlTargetType>()
                .Aggregate(
                    "",
                    (prev, next) => $"{prev}|(?<type>{next})",
                    s => s.TrimStart('|'));

            var regexMatch = Regex.Match(
                context.Message.Text,
                $"^([\\w]* )?random url( (?:{types}))?$",
                RegexOptions.IgnoreCase);
            return regexMatch;
        }

        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "random url [video|image|document|music|other]",
            Explatation = "Hämtar upp en sparad url. Urltyp kan användas för att begränsa urvalet. " +
                          "Typerna kan inte kombineras."
        };
    }
}