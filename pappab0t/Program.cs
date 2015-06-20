using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MargieBot;
using MargieBot.Responders;
using pappab0t.Models;
using pappab0t.Responders;

namespace pappab0t
{
    internal class Program
    {
        private static Bot _bot;
        private static string _slackKey;

        private static DateTime? ConnectedSince { get; set; }
        private static bool ConnectionStatus { get; set; }


        private static void Main(string[] args)
        {
            try
            {
                var t = MainAsync(args);
                t.Wait();
                var run = true;
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
            }
        }

        static async Task MainAsync(string[] args)
        {
            _slackKey = ConfigurationManager.AppSettings["slackKey"];

            _bot = new Bot {Aliases = GetAliases()};

            foreach (var value in GetStaticResponseContextData())
            {
                _bot.ResponseContext.Add(value.Key, value.Value);
            }
            _bot.ResponseContext.Add("Bot",_bot);

            _bot.Responders.AddRange(GetResponders());

            _bot.ConnectionStatusChanged += isConnected =>
            {
                ConnectionStatus = isConnected;

                if (isConnected)
                {
                    ConnectedSince = _bot.ConnectedSince;

                    Console.WriteLine("UserName: {0}\nUserId: {1}",_bot.UserName, _bot.UserID);
                }
                else
                {
                    ConnectedSince = null;
                }
            };

            _bot.MessageReceived += message => Console.WriteLine("msg: "+message);

            await _bot.Connect(_slackKey);
        }

        /// <summary>
        ///     Replace the contents of the list returned from this method with any aliases you might want your bot to respond to.
        ///     If you
        ///     don't want your bot to respond to anything other than its actual name, just return an empty list here.
        /// </summary>
        /// <returns>A list of aliases that will cause the BotWasMentioned property of the ResponseContext to be true</returns>
        private static IReadOnlyList<string> GetAliases()
        {
            return new List<string> {"pb0t"};
        }

        /// <summary>
        ///     If you want to use this application to run your bot, here's where you start. Just scrap as many of the responders
        ///     described in this method as you want and start fresh. Define your own responders using the methods describe
        ///     at https://github.com/jammerware/margiebot/wiki/Configuring-responses and return them in an IList
        ///     <IResponder>
        ///         .
        ///         You create them in this project, in a separate one, or even in the ExampleResponders project if you want.
        ///         Boom! You have your own bot.
        /// </summary>
        /// <returns>A list of the responders this bot should respond with.</returns>
        private static IEnumerable<IResponder> GetResponders()
        {
            var responders = new List<IResponder>
            {
                new ScoreResponder(),
                new ScoreboardRequestResponder(),
                new WikipediaResponder(),
                new WeekNumberResponder(),
                new CapabilitiesResponder(),

                _bot.CreateResponder(
                    context => context.Message.MentionsBot &&
                               Regex.IsMatch(context.Message.Text, @"\b(tack|tanks)\b", RegexOptions.IgnoreCase),
                    context => context.Get<Phrasebook>().GetYoureWelcome()),

                _bot.CreateResponder(
                    context => context.Message.MentionsBot &&
                               !context.BotHasResponded &&
                               Regex.IsMatch(context.Message.Text, @"\b(hej|tja|tjena|läget|hi|hello|morrn|mrn|nirrb)\b",
                                   RegexOptions.IgnoreCase) &&
                               context.Message.User.ID != context.BotUserID &&
                               !context.Message.User.IsSlackbot,
                    context => context.Get<Phrasebook>().GetQuery(context.Message.Text))
            };
            
            return responders;
        }

        /// <summary>
        ///     If you want to share any data across all your responders, you can use the StaticResponseContextData property of the
        ///     bot to do it. I elected
        ///     to have most of my responders use a "Phrasebook" object to ensure a consistent tone across the bot's responses, so
        ///     I stuff the Phrasebook
        ///     into the context for use.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, object> GetStaticResponseContextData()
        {
            return new Dictionary<string, object>
            {
                {"Phrasebook", new Phrasebook()}
            };
        }
    }
}