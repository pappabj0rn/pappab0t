using System.Net;
using System.Text.RegularExpressions;
using Bazam.NoobWebClient;
using MargieBot.Models;
using MargieBot.Responders;
using Newtonsoft.Json.Linq;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class WikipediaResponder : IResponder, IExposedCapability
    {
        private const string WikiSinglewordRegex = @"\b(wiki|wikipedia)\b\s+(?<term>\w+)";
        private const string WikiMultiwordRegex = @"\b(wiki|wikipedia)\b\s+""(?<term>[\s\S]+)""";

        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.ChatHub.Type == SlackChatHubType.DM || context.Message.MentionsBot) &&
                   (Regex.IsMatch(context.Message.Text, WikiMultiwordRegex) ||
                    Regex.IsMatch(context.Message.Text, WikiSinglewordRegex));
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var match = Regex.Match(context.Message.Text, WikiMultiwordRegex);
            string searchTerm;
            if (match.Success)
            {
                searchTerm = match.Groups["term"].Value;
            }
            else
            {
                match = Regex.Match(context.Message.Text, WikiSinglewordRegex);
                searchTerm = match.Groups["term"].Value;
            }
            var requestUrl =
                string.Format(
                    "http://en.wikipedia.org/w/api.php?action=query&list=search&format=json&prop=extracts&exintro=&explaintext=&srsearch={0}&utf8=&continue=",
                    WebUtility.UrlEncode(searchTerm.Trim()));
            var response = new NoobWebClient().GetResponse(requestUrl, RequestMethod.Get).GetAwaiter().GetResult();
            var responseData = JObject.Parse(response);

            if (responseData["query"] != null && responseData["query"]["searchinfo"] != null)
            {
                var totalHits = responseData["query"]["searchinfo"]["totalhits"].Value<int>();

                if (totalHits > 0)
                {
                    var articleTitle = responseData["query"]["search"][0]["title"].Value<string>();
                    var articleRequestUrl =
                        "https://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro=&explaintext=&titles=" +
                        WebUtility.UrlEncode(articleTitle);
                    var articleResponse =
                        new NoobWebClient().GetResponse(articleRequestUrl, RequestMethod.Get).GetAwaiter().GetResult();
                    var articleData = JObject.Parse(articleResponse);

                    if (articleData["query"]["pages"]["-1"] == null)
                    {
                        var summary = articleData["query"]["pages"].First.First["extract"].Value<string>();
                        if (summary.IndexOf('\n') > 0)
                        {
                            summary = summary.Substring(0, summary.IndexOf('\n'));
                        }

                        return new BotMessage
                        {
                            Text = "http://en.wikipedia.org/wiki/{0}\n {1}".With(articleTitle.Replace(" ", "_"), summary)
                        };
                    }
                }
            }

            return new BotMessage
            {
                Text = "Har aldrig hört talas om {0}, vilket kanske inte är så konstigt. Men vad som _är_ konstigt är att Wikipedia inte heller har nån info om det."
                            .With(searchTerm)
            };
        }

        public ExposedInformation Info
        {
            get { return new ExposedInformation { Usage = "<wiki|wikipedia> <term|\"term1 term2\">", Explatation = "Kollar på wikipedia (eng) efter angivna termer." }; }
        }
    }
}