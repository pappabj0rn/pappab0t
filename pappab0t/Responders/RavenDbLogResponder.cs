using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MargieBot.Models;
using MargieBot.Responders;
using Newtonsoft.Json.Linq;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using Raven.Client;

namespace pappab0t.Responders
{
    public class RavenDbLogResponder : ResponderBase, IExposedCapability
    {
        private const string FromDateKey = "fromDate";
        private const string ToDateKey = "toDate";
        private const string PageKey = "page";
        private const string SingleDayRegex = @"\blogg\b\s+(?<" + FromDateKey + @">[0-9]{6})(\ss(?<" + PageKey + ">[0-9]{0,3}))?";
        private const string MultipleDayRegex = @"\blogg\b\s+(?<" + FromDateKey + ">[0-9]{6})-(?<" + ToDateKey + @">[0-9]{6})(\ss(?<" + PageKey + ">[0-9]{0,3}))?";
        private const int PageSize = 1000;
        
        private DateTime _fromDate;
        private DateTime _toDate;
        private int _page;
        private string _dateRange;
        private RavenQueryStatistics _queryStats;
        private int _logMessageCount;

        public override bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot 
                    || context.Message.IsDirectMessage()) &&
                   (Regex.IsMatch(context.Message.Text, SingleDayRegex, RegexOptions.IgnoreCase)
                    || Regex.IsMatch(context.Message.Text, SingleDayRegex, RegexOptions.IgnoreCase));
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;
            
            CollectParams();

            var logPage = CreateLog();
            var pageInfo = CreatePageInfo();

            return new BotMessage
            {
                Text = "Logg för {0} {1}".With(_dateRange, pageInfo),
                Attachments = new[]
                {
                    new SlackAttachment
                    {
                        Title = "Loggutdrag",
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
                        ? "(s. {0}/{1}, {2} rader)".With(_page, pages, _logMessageCount)
                        : "(s. 0/0)";
        }

        private void CollectParams()
        {
            var match = Regex.Match(Context.Message.Text, MultipleDayRegex);
            if (match.Success)
            {
                CollectInputParams(match);
                _dateRange = "{0} - {1}".With(match.Groups[FromDateKey].Value, match.Groups[ToDateKey].Value);
            }
            else
            {
                match = Regex.Match(Context.Message.Text, SingleDayRegex);
                CollectInputParams(match);
                _dateRange = match.Groups[FromDateKey].Value;
            }
        }

        private void CollectInputParams(Match match)
        {
            _fromDate = ParseDate(match.Groups[FromDateKey].Value);
            _toDate = String.IsNullOrEmpty(match.Groups[ToDateKey].Value)
                        ? _fromDate.AddDays(1) 
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
            var sb = new StringBuilder();
            using (var session = DocumentStore.OpenSession())
            {
                var messages = session.Advanced
                                    .DocumentQuery<SlackMessage>()
                                    .Statistics(out _queryStats)
                                    .WhereEquals(x=>x.ChatHub.ID, Context.Message.ChatHub.ID)
                                    .AndAlso()
                                    .WhereBetween(Keys.RavenDB.Metadata.Created, _fromDate.ToShortDateString(), _toDate.ToShortDateString())
                                    .OrderBy(new[] { Keys.RavenDB.Metadata.TimeStamp })
                                    .Skip((_page-1)*PageSize)
                                    .Take(PageSize)
                                    .ToList();

                _logMessageCount = messages.Count;

                foreach (var message in messages)
                {
                    string username;
                    if (message.User == null && message.Text != null)
                    {
                        var jObject = JObject.Parse(message.RawData);
                        username = "`BOT`{0}".With(jObject[Keys.Slack.MessageJson.Username].Value<string>());
                    }
                    else if(message.User != null)
                    {
                        username = Context.UserNameCache[message.User.ID];
                    }
                    else
                    {
                        continue;
                    }

                    var meta = session.Advanced.GetMetadataFor(message);

                    sb.AppendFormat("{0} {1}: {2}\n", meta.Value<DateTime>(Keys.RavenDB.Metadata.Created), username, message.Text);
                }
            }

            return sb.ToString();
        }

        public ExposedInformation Info 
        {
            get { return new ExposedInformation
            {
                Usage = "logg <fromDate>[-toDate] [sX]",
                Explatation = "Sammanställer meddelanden som loggats för det givna datumet (eller inom givet intervall) och postar dem i chatten. 1000 meddelanden visas per sida. Ex 1: logg 150621. Ex 2: logg 150621-150623 s2"
            }; } 
        }
    }
}
