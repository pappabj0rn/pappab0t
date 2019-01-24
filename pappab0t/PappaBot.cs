using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bazam.Http;
using MargieBot;
using Newtonsoft.Json.Linq;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.MessageHandler;
using pappab0t.Models;
using pappab0t.Modules.Inventory;
using pappab0t.Responders;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace pappab0t
{
    public class PappaBot
    {
        private static Bot _bot;
        private static string _slackKey;
        private static string[] _botAliases;
        private static IDocumentStore _ravenStore;
        private static IInventoryManager _inventoryManager;
        private static Dictionary<string, string> _userNameCache;
        private static Dictionary<string, string> _channelsNameCache;
        private static List<IMessageHandler> _messageHandlers;
        private static IScheduler _scheduler;

        public PappaBot()
        {
            //TODO: refacor and test bot core.
        }

        public void Configure()
        {
            Init();

            _bot.Aliases = _botAliases;

            foreach (var value in GetStaticResponseContextData())
            {
                _bot.ResponseContext.Add(value.Key, value.Value);
            }

            _bot.Responders.AddRange(GetResponders());

            _bot.MessageReceived += _bot_MessageReceived;


            _scheduler.Interval = 10 * 1000;
            
        }

        public void Start()
        {
            ConnectToSlackIfDisconnected(false);
            _scheduler.Run();
        }

        private static void Init()
        {
            _bot = new Bot();
            _bot.ConnectionStatusChanged += ConnectToSlackIfDisconnected;

            _ravenStore = CreateStore();

            ObjectFactory.Container.Configure(
                x=>
                {
                    x.For<IDocumentStore>()
                        .Use(_ravenStore);

                    x.For<ICommandDataParser>()
                        .Use<CommandDataParser>();

                    x.For<IBot>().Use(() => new BotWrapper(_bot));

                    
                });

            _slackKey = ConfigurationManager.AppSettings[Keys.AppSettings.SlackKey];
            
            _userNameCache = new Dictionary<string, string>();
            _channelsNameCache = new Dictionary<string, string>();
            _messageHandlers = ObjectFactory.Container.GetAllInstances<IMessageHandler>().ToList();

            _botAliases = ConfigurationManager.AppSettings[Keys.AppSettings.BotAliases].Split(',');

            

            _scheduler = new Scheduler(new TimerAdapter(), ObjectFactory.Container.GetAllInstances<ScheduledTask>());
        }

        private static void ConnectToSlackIfDisconnected(bool isConnected)
        {
            if (!isConnected)
            {
                Task.Factory.StartNew(async () =>
                {
                    while (!_bot.IsConnected)
                    {
                        try
                        {
                            Console.WriteLine("Trying to connect to Slack...");
                            await _bot.Connect(_slackKey);
                        }
                        catch (Exception ex)
                        {
                            //Error(ex.InnerException?.Message ?? ex.Message);
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                        }
                    }

                    await PopulateUsernameCache();
                    await PopulateChannelsCache();
                });
            }
            else
            {
                Console.WriteLine("Connected!");
            }
        }

        private static Dictionary<string, object> GetStaticResponseContextData()
        {
            return new Dictionary<string, object>
            {
                {Keys.StaticContextKeys.Phrasebook, new Phrasebook()},
                {Keys.StaticContextKeys.Bot,_bot},
                {Keys.StaticContextKeys.RavenStore,_ravenStore},
                {Keys.StaticContextKeys.ChannelsNameCache,_channelsNameCache}
            };
        }

        private static IEnumerable<IResponder> GetResponders()
        {
            var responders = ObjectFactory.Container.GetAllInstances<IResponder>().ToList();
            MoveMultiMessageresponderLast(responders);

            responders.AddRange(new[]
            {
                SimpleResponder.Create(
                    context => context.Message.MentionsBot &&
                               Regex.IsMatch(context.Message.Text, @"\b(tack|tanks)\b", RegexOptions.IgnoreCase),
                    context => context.Get<Phrasebook>().YoureWelcome()),

                SimpleResponder.Create(
                    context => (context.Message.MentionsBot || context.Message.IsDirectMessage()) &&
                               !context.BotHasResponded &&
                               Regex.IsMatch(context.Message.Text, @"\b(hej|tja|tjena|yo|läget|hi|hello|morrn|mrn|nirrb/)\b",
                                   RegexOptions.IgnoreCase) &&
                               context.Message.User.ID != context.BotUserID &&
                               !context.Message.User.IsSlackbot,
                    context => context.Get<Phrasebook>().AttentionResponse(context.Message.Text)),

                SimpleResponder.Create(
                    context => (context.Message.MentionsBot || context.Message.IsDirectMessage())
                               && !context.BotHasResponded
                               && context.Message.User.ID != context.BotUserID 
                               && !context.Message.User.IsSlackbot,
                    context => context.Get<Phrasebook>().IDidntUnderstand())
            });

            return responders;
        }

        private static void MoveMultiMessageresponderLast(List<IResponder> responders)
        {
            var temp = responders.Find(x => x is SecondaryMessageResponder);
            responders.Remove(temp);
            responders.Add(temp);
        }

        private static async Task PopulateUsernameCache()
        {
            _userNameCache = new Dictionary<string, string>();

            var client = new NoobWebClient();

            var values = new List<string>
                {
                    "token", _slackKey
                };

            var json = await client.DownloadString("https://slack.com/api/users.list", RequestMethod.Post, values.ToArray());
            var jData = JObject.Parse(json);

            foreach (var user in jData[Keys.Slack.UserListJson.Members].Values<JObject>())
            {
                _userNameCache.Add(user["id"].Value<string>(), user["name"].Value<string>());
            }
        }

        private static async Task PopulateChannelsCache()
        {
            _channelsNameCache = new Dictionary<string, string>();

            var client = new NoobWebClient();

            var values = new List<string>
                {
                    "token", _slackKey,
                    "exclude_archived","1"
                };

            var json = await client.DownloadString("https://slack.com/api/channels.list", RequestMethod.Post, values.ToArray());
            var jData = JObject.Parse(json);

            foreach (var channel in jData[Keys.Slack.ChannelsListJson.Channels].Values<JObject>())
            {
                _channelsNameCache.Add(channel["id"].Value<string>(), channel["name"].Value<string>());
            }
        }

        static void _bot_MessageReceived(string json)
        {
            //Copied from MargieBot source and modified to allow all messages
            //ConsoleDebugLog(json);

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
                hubs.Add(hub); // ?
            }

            var messageText = jObject[Keys.Slack.MessageJson.Text]?.Value<string>();

            var message = new SlackMessage
            {
                ChatHub = hub,
                RawData = json,
                Text = messageText,
                User = jObject[Keys.Slack.MessageJson.User] != null ? new SlackUser { ID = jObject[Keys.Slack.MessageJson.User].Value<string>() } : null
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
#if DEBUG
            var eds = new EmbeddableDocumentStore
            {
                DataDirectory = "Data",
                //UseEmbeddedHttpServer = true,
                DefaultDatabase = "pbot",
                //Configuration = {Port = 8090}
            };
            eds.Initialize();

            return eds;
#endif
#pragma warning disable 162
            var store = new DocumentStore
            {
                Url = ConfigurationManager.AppSettings[Keys.AppSettings.RavenUrl],
                DefaultDatabase = ConfigurationManager.AppSettings[Keys.AppSettings.RavenDbName]
            }.Initialize();

            return store;
#pragma warning restore 162
        }
    }
}