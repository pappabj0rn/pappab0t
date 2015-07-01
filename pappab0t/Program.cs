using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bazam.NoobWebClient;
using MargieBot;
using MargieBot.Models;
using MargieBot.Responders;
using Newtonsoft.Json.Linq;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.MessageHandler;
using pappab0t.Models;
using Raven.Client;
using Raven.Client.Document;
using StructureMap;

namespace pappab0t
{
    internal class Program
    {
        private static Bot _bot;
        private static string _slackKey;
        private static string[] _botAliases;
        private static IDocumentStore _ravenStore;
        private static Dictionary<string, string> _userNameCache;
        private static List<IMessageHandler> _messageHandlers;
       
        private static void Main()
        {
            try
            {
                InitStructureMap();

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

        private static void InitStructureMap()
        {
            ObjectFactory.Initialize(x =>
            {
                //x.For<TYPE>().Use<IMPL>()
                x.Scan(y =>
                {
                    y.AddAllTypesOf<IResponder>();
                    y.AddAllTypesOf<IMessageHandler>();
                    y.AssemblyContainingType(typeof(IExposedCapability));
                });
            });
        }

        static async Task MainAsync()
        {
            Init();

            _bot.Aliases = _botAliases;

            foreach (var value in GetStaticResponseContextData())
            {
                _bot.ResponseContext.Add(value.Key, value.Value);
            }

            _bot.Responders.AddRange(GetResponders());

            _bot.ConnectionStatusChanged += async isConnected =>
            {
                if (!isConnected) return;

                Console.WriteLine("UserName: {0}\nUserId: {1}\nConnected at: {2}", _bot.UserName, _bot.UserID, _bot.ConnectedSince);
                await PopulateUsernameCache();
            };

            _bot.MessageReceived += _bot_MessageReceived;

            await _bot.Connect(_slackKey);
        }

        private static void Init()
        {
            _ravenStore = CreateStore();
            _slackKey = ConfigurationManager.AppSettings[Keys.AppSettings.SlackKey];
            _bot = new Bot();
            _userNameCache = new Dictionary<string, string>();
            _messageHandlers = ObjectFactory.GetAllInstances<IMessageHandler>().ToList();

            _botAliases = ConfigurationManager.AppSettings[Keys.AppSettings.BotAliases].Split(',');
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

        private static IEnumerable<IResponder> GetResponders()
        {
            var responders = ObjectFactory.GetAllInstances<IResponder>().ToList();

            responders.AddRange(new[]
            {
                _bot.CreateResponder(
                    context => context.Message.MentionsBot &&
                               Regex.IsMatch(context.Message.Text, @"\b(tack|tanks)\b", RegexOptions.IgnoreCase),
                    context => context.Get<Phrasebook>().GetYoureWelcome()),

                _bot.CreateResponder(
                    context => (context.Message.MentionsBot || context.Message.IsDirectMessage()) &&
                               !context.BotHasResponded &&
                               Regex.IsMatch(context.Message.Text, @"\b(hej|tja|tjena|yo|läget|hi|hello|morrn|mrn|nirrb)\b",
                                   RegexOptions.IgnoreCase) &&
                               context.Message.User.ID != context.BotUserID &&
                               !context.Message.User.IsSlackbot,
                    context => context.Get<Phrasebook>().GetQuery(context.Message.Text)),
                
                _bot.CreateResponder(
                    context => (context.Message.MentionsBot || context.Message.IsDirectMessage())
                               && !context.BotHasResponded
                               && context.Message.User.ID != context.BotUserID 
                               && !context.Message.User.IsSlackbot,
                    context => context.Get<Phrasebook>().GetIDidntUnderstand())
            });

            return responders;
        }

        private static async Task PopulateUsernameCache()
        {
            var client = new NoobWebClient();

            var values = new List<string>
                {
                    "token", _slackKey
                };

            var json = await client.GetResponse("https://slack.com/api/users.list", RequestMethod.Post, values.ToArray());
            var jData = JObject.Parse(json);

            foreach (var user in jData[Keys.Slack.UserListJson.Members].Values<JObject>())
            {
                _userNameCache.Add(user["id"].Value<string>(), user["name"].Value<string>());
            }
        }

        static void _bot_MessageReceived(string json)
        {
            //Copied from MargieBot source and modified to allow all messages

            var jObject = JObject.Parse(json);
            if (jObject[Keys.Slack.MessageJson.Type].Value<string>() != "message") return;

            var channelID = jObject[Keys.Slack.MessageJson.Channel].Value<string>();
            SlackChatHub hub;

            if (_bot.ConnectedHubs.ContainsKey(channelID))
            {
                hub = _bot.ConnectedHubs[channelID];
            }
            else
            {
                hub = SlackChatHub.FromID(channelID);
                var hubs = new List<SlackChatHub>();
                hubs.AddRange(_bot.ConnectedHubs.Values);
                hubs.Add(hub);
            }

            var messageText = (jObject[Keys.Slack.MessageJson.Text] != null ? jObject[Keys.Slack.MessageJson.Text].Value<string>() : null);

            var message = new SlackMessage
            {
                ChatHub = hub,
                RawData = json,
                Text = messageText,
                User = (jObject[Keys.Slack.MessageJson.User] != null ? new SlackUser { ID = jObject[Keys.Slack.MessageJson.User].Value<string>() } : null)
            };

            var context = new ResponseContext
            {
                BotHasResponded = false,
                BotUserID = _bot.UserID,
                BotUserName = _bot.UserName,
                Message = message,
                TeamID = _bot.TeamID,
                UserNameCache = new ReadOnlyDictionary<string, string>(_userNameCache)
            };

            if (_bot.ResponseContext != null)
            {
                foreach (var key in _bot.ResponseContext.Keys)
                {
                    context.Set(key, _bot.ResponseContext[key]);
                }
            }

            foreach (var handler in _messageHandlers)
            {
                handler.Execute(context);
            }
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