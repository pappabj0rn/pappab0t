using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;
using pappab0t.Extensions;

namespace pappab0t.Responders
{
    public class CommandParser : ICommandParser
    {
        public static class Keys
        {
            public static string UserIdKey = "user";
        }

        private Dictionary<string, string> ShorthandMap => new Dictionary<string, string>
        {
            {"u", "user"}
        };

        private List<string> _words = new List<string>();
        private int _cmdWordIndex;
        private ResponseContext _context;

        public void Parse()
        {
            if (!ToBot)
                return;

            _words = Context.Message.Text.Split(' ').ToList();

            _cmdWordIndex = _words.FindIndex(x => x == Command);

            ParamsRaw = 
                _words
                    .Skip(_cmdWordIndex + 1)
                    .Aggregate("", (a, b) => a + " " + b)
                    .Trim();

            Params = new Dictionary<string, string>();
            ParseParameters();
        }

        public ResponseContext Context
        {
            get => _context;
            set
            {
                ResetParser();
                _context = value;
            }
        }

        private void ResetParser()
        {
            _words = new List<string>();
            ParamsRaw = null;
            Params = new Dictionary<string, string>();
        }

        private void ParseParameters()
        {
            TryAddNonKeyedUser();

            var paramWords = ParamsRaw.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
            var currentKey = "";
            var currentValue = "";

            foreach (string word in paramWords)
            {
                if(word.StartsWith("--"))
                {
                    AddFoundKeyValue(currentKey, currentValue);
                    currentKey = word.Substring(2);
                }
                else if (word.StartsWith("-"))
                {
                    AddFoundKeyValue(currentKey, currentValue);
                    string shortHand = word.Substring(1);

                    if (shortHand.Length > 1)
                        foreach (char c in shortHand)
                        {
                            Params.Add(c.ToString(), "");
                        }
                    else
                        currentKey = ShorthandMap.ContainsKey(shortHand)
                            ? ShorthandMap[shortHand]
                            : shortHand;
                }
                else
                {
                    currentValue += " " + word;
                }
            }

            AddFoundKeyValue(currentKey, currentValue);
            MapUser();
        }

        private void MapUser()
        {
            if (!Params.ContainsKey(Keys.UserIdKey))
                return;

            Params[Keys.UserIdKey] = Params[Keys.UserIdKey].Trim();

            if (Params[Keys.UserIdKey].StartsWith("<@"))
                Params[Keys.UserIdKey] = Params[Keys.UserIdKey].Substring(2, 9);

            else if (Context.UserNameCache.Any(x => x.Value == Params[Keys.UserIdKey]))
                Params[Keys.UserIdKey] = Context.UserNameCache.First(x => x.Value == Params[Keys.UserIdKey]).Key;
        }

        private void AddFoundKeyValue(string currentKey, string currentValue)
        {
            if (currentKey != "")
                Params.Add(currentKey, currentValue.Replace("\"","").Trim());
        }

        private void TryAddNonKeyedUser()
        {
            if (ParamsRaw.StartsWith("<@"))
                Params.Add(Keys.UserIdKey, ParamsRaw.Substring(2, 9));

            else if (_words.Count > _cmdWordIndex + 1
                     && Context.UserNameCache.Any(x => x.Value == _words[_cmdWordIndex + 1]))
                Params.Add(Keys.UserIdKey, Context.UserNameCache.First(x => x.Value == _words[_cmdWordIndex + 1]).Key);
        }

        public bool ToBot => Context.Message.MentionsBot
                             || Context.Message.IsDirectMessage();

        public string Command => 
            _words
                .FirstOrDefault(x=>!IsBotReference(x))
                ?.ToLower();

        public string ParamsRaw { get; private set; }
        public Dictionary<string,string> Params { get; private set; }

        private bool IsBotReference(string str)
        {
            if (str == $"<@{Context.BotUserID}>"
                || str == Context.BotUserName)
                return true;

            var bot = Context.Get<Bot>(pappab0t.Keys.StaticContextKeys.Bot);
            return bot.Aliases.Contains(str);
        }
    }
}