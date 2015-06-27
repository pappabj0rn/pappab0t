using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using Raven.Client;

namespace pappab0t.Responders
{
    public class RavenDbLogResponder : IResponder, IExposedCapability
    {
        private const string FromDateKey = "fromDate";
        private const string ToDateKey = "toDate";
        private const string PageKey = "page";
        private const string SingleDayRegex = @"\blogg\b\s+(?<" + FromDateKey + @">[0-9]{6})(\ss(?<" + PageKey + ">[0-9]{0,3}))?";
        private const string MultipleDayRegex = @"\blogg\b\s+(?<" + FromDateKey + ">[0-9]{6})-(?<" + ToDateKey + @">[0-9]{6})(\ss(?<" + PageKey + ">[0-9]{0,3}))?";
        private const int PageSize = 1000;
        
        private ResponseContext _context;
        private DateTime _fromDate;
        private DateTime _toDate;
        private int _page;
        private string _dateRange;
        private RavenQueryStatistics _queryStats;

        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot 
                    || context.Message.ChatHub.Type == SlackChatHubType.DM) &&
                   (Regex.IsMatch(context.Message.Text, SingleDayRegex, RegexOptions.IgnoreCase)
                    || Regex.IsMatch(context.Message.Text, SingleDayRegex, RegexOptions.IgnoreCase));
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            _context = context;
            
            CollectParams();

            var logPage = CreateLog();

            var pageInfo = CreatePageInfo();

            return new BotMessage
            {
                Attachments = new[]
                {
                    new SlackAttachment
                    {
                        Title = "Loggutdrag",
                        PreText = "Logg för {0} {1}".With(_dateRange,pageInfo),
                        Text = logPage
                    }
                }
            };
        }

        private string CreatePageInfo()
        {
            var pages = _queryStats.TotalResults / PageSize;
            var remainder = _queryStats.TotalResults % PageSize;
            
            if (remainder > 0)
                pages++;

            return _queryStats.TotalResults > 0 
                        ? "(s. {0}/{1})".With(_page, pages)
                        : "(s. 0/0)";
        }

        private void CollectParams()
        {
            var match = Regex.Match(_context.Message.Text, MultipleDayRegex);
            if (match.Success)
            {
                CollectInputParams(match);
                _dateRange = "{0} - {1}".With(match.Groups[FromDateKey].Value, match.Groups[ToDateKey].Value);
            }
            else
            {
                match = Regex.Match(_context.Message.Text, SingleDayRegex);
                CollectInputParams(match);
                _dateRange = match.Groups[FromDateKey].Value;
            }
        }

        private void CollectInputParams(Match match)
        {
            _fromDate = ParseDate(match.Groups[FromDateKey].Value);
            _toDate = String.IsNullOrEmpty(match.Groups[ToDateKey].Value) 
                        ? DateTime.Now.AddDays(1) 
                        : ParseDate(match.Groups[ToDateKey].Value);
            _page = int.Parse(match.Groups[PageKey].Value.Fallback("1"));
        }

        private static DateTime ParseDate(string yyMMdd)
        {
            return new DateTime(2000+int.Parse(yyMMdd.Substring(0,2)),
                                int.Parse(yyMMdd.Substring(2,2)),
                                int.Parse(yyMMdd.Substring(4)));
        }

        private string CreateLog()
        {
            var store = _context.Get<IDocumentStore>();

            var sb = new StringBuilder();
            using (var session = store.OpenSession())
            {
                var messages = session.Advanced
                                    .DocumentQuery<SlackMessage>()
                                    .Statistics(out _queryStats)
                                    .WhereBetween(Keys.RavenDB.Metadata.Created, _fromDate.ToShortDateString(), _toDate.ToShortDateString())
                                    .OrderBy(new[] { Keys.RavenDB.Metadata.Created })
                                    .Skip((_page-1)*PageSize)
                                    .Take(PageSize)
                                    .ToList();

                foreach (var slackMessage in messages)
                {
                    var meta = session.Advanced.GetMetadataFor(slackMessage);

                    sb.AppendFormat("{0} {1}: {2}\n", meta.Value<DateTime>(Keys.RavenDB.Metadata.Created), _context.UserNameCache[slackMessage.User.ID], slackMessage.Text);
                }
            }

            return sb.ToString();
        }

        public ExposedInformation Info 
        {
            get { return new ExposedInformation
            {
                Usage = "logg [fromDate](-[toDate])( p[2])",
                Explatation = "Sammanställer meddelanden som loggats för det givna datumet (eller inom givet intervall) och postar dem i chatten. 1000 meddelanden visas per sida. Ex 1: logg 150621. Ex 2: logg 150621-150623 s2"
            }; } 
        }
    }
}
