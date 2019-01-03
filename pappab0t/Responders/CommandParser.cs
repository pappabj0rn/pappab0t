using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;
using pappab0t.Extensions;

namespace pappab0t.Responders
{
    public class CommandParser : ICommandParser
    {
        public static string UnnamedParam = "unnamed";
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

        private string _currentKey;
        private string _currentValue;
        private void ParseParameters()
        {
            TryAddNonKeyedUser();

            var paramWords = ParamsRaw.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);

            _currentKey = "";
            _currentValue = "";
            var multiValueStarted = false;

            foreach (string word in paramWords)
            {
                if(word.StartsWith("--"))
                {
                    AddFoundKeyValue();
                    _currentKey = word.Substring(2);
                }
                else if (word.StartsWith("-"))
                {
                    AddFoundKeyValue();
                    string shortHand = word.Substring(1);

                    if (shortHand.Length > 1)
                        foreach (char c in shortHand)
                        {
                            if(!Params.ContainsKey(c.ToString()))
                                Params.Add(c.ToString(), "");
                        }
                    else
                        _currentKey = ShorthandMap.ContainsKey(shortHand)
                            ? ShorthandMap[shortHand]
                            : shortHand;
                }
                else if (!multiValueStarted && _currentKey == "")
                {
                    _currentKey = UnnamedParam;
                    multiValueStarted = true;
                    _currentValue += " " + word;
                }
                else
                {
                    if (word.StartsWith("\""))
                    {
                        multiValueStarted = true;
                        _currentValue += " " + word;
                    }
                    else if(word.EndsWith("\""))
                    {
                        multiValueStarted = false;
                        _currentValue += " " + word;
                        AddFoundKeyValue();
                    }
                    else
                    {
                        if (multiValueStarted)
                            _currentValue += " " + word;
                        else
                        {
                            _currentValue = word;
                            AddFoundKeyValue();
                        }
                    }
                }
            }

            //if (_currentKey == string.Empty)
            //    _currentKey = UnnamedParam;

            AddFoundKeyValue();
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

        private void AddFoundKeyValue()
        {
            if (_currentKey == "")
                return;

            if (Params.ContainsKey(_currentKey))
            {
                Params[_currentKey] += _currentValue;
                Params[_currentKey] = Params[_currentKey].Replace("\"", "").Trim();
            }
            else
                Params.Add(_currentKey, _currentValue.Replace("\"","").Trim());

            _currentKey = "";
            _currentValue = "";
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