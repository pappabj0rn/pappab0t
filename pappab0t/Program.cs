using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MargieBot;
using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.MessageHandler;
using pappab0t.Models;
using pappab0t.Responders;
using Raven.Client;
using Raven.Client.Document;

namespace pappab0t
{
    internal class Program
    {
        private static Bot _bot;
        private static string _slackKey;
        private static IDocumentStore _ravenStore;
       
        private static void Main()
        {
            try
            {
                var t = MainAsync();
                t.Wait();

                var run = true;
                Console.WriteLine("Press X to exit.");
                while (run)
                {
                    var consoleKeyInfo = Console.ReadKey();

                    if (consoleKeyInfo.Key == ConsoleKey.X)
                    {
                        run = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Console.ReadKey();
            }
        }

        static async Task MainAsync()
        {
            Init();

            _bot.Aliases = new List<string> {"pb0t"};

            foreach (var value in GetStaticResponseContextData())
            {
                _bot.ResponseContext.Add(value.Key, value.Value);
            }

            _bot.Responders.AddRange(GetResponders());

            _bot.ConnectionStatusChanged += isConnected =>
            {
                if (isConnected)
                {
                    Console.WriteLine("UserName: {0}\nUserId: {1}\nConnected at: {2}", _bot.UserName, _bot.UserID, _bot.ConnectedSince);
                }
            };

            await _bot.Connect(_slackKey);
        }

        private static void Init()
        {
            _ravenStore = CreateStore();
            _slackKey = ConfigurationManager.AppSettings[Keys.AppSettings.SlackKey];
            _bot = new Bot();
        }

        private static IEnumerable<IResponder> GetResponders()
        {
            var responders = new List<IResponder>
            {
                new ScoreResponder(),
                new ScoreboardRequestResponder(),
                new WikipediaResponder(),
                new WeekNumberResponder(),
                new CapabilitiesResponder(),
                new RavenDbLogResponder(),
                new DikaGameResponder(),
                new TimeResponder(),
                
                new RavenDbLoggerMessageHandler(),

                _bot.CreateResponder(
                    context => context.Message.MentionsBot &&
                               Regex.IsMatch(context.Message.Text, @"\b(tack|tanks)\b", RegexOptions.IgnoreCase),
                    context => context.Get<Phrasebook>().GetYoureWelcome()),

                _bot.CreateResponder(
                    context => (context.Message.MentionsBot || context.Message.ChatHub.Type == SlackChatHubType.DM) &&
                               !context.BotHasResponded &&
                               Regex.IsMatch(context.Message.Text, @"\b(hej|tja|tjena|läget|hi|hello|morrn|mrn|nirrb)\b",
                                   RegexOptions.IgnoreCase) &&
                               context.Message.User.ID != context.BotUserID &&
                               !context.Message.User.IsSlackbot,
                    context => context.Get<Phrasebook>().GetQuery(context.Message.Text))
            };
            
            return responders;
        }

        private static Dictionary<string, object> GetStaticResponseContextData()
        {
            return new Dictionary<string, object>
            {
                {"Phrasebook", new Phrasebook()},
                {"Bot",_bot},
                {"RavenStore",_ravenStore}
            };
        }

        private static IDocumentStore CreateStore()
        {
            var store = new DocumentStore
            {
                Url = ConfigurationManager.AppSettings[Keys.AppSettings.RavenUrl],
                DefaultDatabase = ConfigurationManager.AppSettings[Keys.AppSettings.RavenDbName]
            }.Initialize();

            return store;
        }
    }
}