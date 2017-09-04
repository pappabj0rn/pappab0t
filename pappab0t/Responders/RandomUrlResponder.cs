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
            var types = Enum
                .GetValues(typeof(UrlTargetType))
                .Cast<UrlTargetType>()
                .Aggregate(
                    "", 
                    (prev, next) => $"{prev}|(?<type>{next})",
                    s=>s.TrimStart('|'));

            var regexMatch = Regex.Match(
                context.Message.Text,
                $"^([\\w]* )?random url( (?:{types}))?$", 
                RegexOptions.IgnoreCase);

            if(!regexMatch.Success)
                return new BotMessage
                {
                    Text = _phrasebook.InvalidType()
                };

            var filterByType = !regexMatch.Groups["type"].Value.IsNullOrEmpty();
            var type = UrlTargetType.Other;
            if (filterByType)
            {
                type = (UrlTargetType)Enum.Parse(
                    typeof(UrlTargetType), 
                    regexMatch.Groups["type"].Value, 
                    ignoreCase: true);
            }

            using (var session = _store.OpenSession())
            {
                IQueryable<UserUrlPost> q = null;
                var rq = session
                    .Query<UserUrlPost>()
                    .Customize(x => x.RandomOrdering());

                if (filterByType)
                    q = rq.Where(x => x.UrlMatchData.TargetType == type);

                var post = (q ?? rq)
                    .FirstOrDefault();

                if (post == null)
                    return new BotMessage {Text = _phrasebook.NoDataFound()};

                return new BotMessage
                {
                    Attachments = new  List<SlackAttachment>
                    {
                        new SlackAttachment
                        {
                            AuthorName = context.UserNameCache[post.UserId],
                            Title = post.UrlMatchData.ToString(),
                            TitleLink = post.UrlMatchData.ToString(),
                            TimeStamp = post.TimeStamp
                        }
                    }
                };
            }
        }

        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "random url [video|image|document|music|other]",
            Explatation = "Hämtar upp en sparad url. Urltyp kan användas för att begränsa urvalet. Typerna kan inte kombineras."
        };
    }
}