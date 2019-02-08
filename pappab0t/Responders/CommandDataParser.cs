using System;
using System.Collections.Generic;
using System.Linq;
using MargieBot;
using pappab0t.Extensions;

namespace pappab0t.Responders
{
    public class CommandDataParser : ICommandDataParser
    {
        private Dictionary<string, string> ShorthandMap => new Dictionary<string, string>
        {
            {"u", "user"}
        };

        private List<string> _words = new List<string>();
        private int _cmdWordIndex;
        private ResponseContext _context;

        public CommandData Parse()
        {
            if (!ToBot)
                return new CommandData();

            _words = Context.Message.Text.Split(' ').ToList();

            _cmdWordIndex = _words.FindIndex(x => x.Equals(Command,StringComparison.InvariantCultureIgnoreCase));

            ParamsRaw = 
                _words
                    .Skip(_cmdWordIndex + 1)
                    .Aggregate("", (a, b) => a + " " + b)
                    .Trim();

            Params = new Dictionary<string, string>();
            ParseParameters();
            FlagKnownUser();

            return new CommandData
            {
                ToBot = ToBot,
                Command = Command,
                Params =  Params,
                ParamsRaw = ParamsRaw
            };
        }

        private void FlagKnownUser()
        {
            if (Params.ContainsKey(Keys.CommandParser.UserIdKey)
                && Context.UserNameCache.ContainsKey(Params[Keys.CommandParser.UserIdKey]))
            {
                Params.Add(Keys.CommandParser.UserKnownKey,"");
            }
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

            var paramWords = ParamsRaw
                .Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            if(Params.ContainsKey(Keys.CommandParser.UserIdKey))
                paramWords.RemoveAt(0);

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
                    _currentKey = Keys.CommandParser.UnnamedParam;
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

            AddFoundKeyValue();
            MapUser();
        }

        private void MapUser()
        {
            if (!Params.ContainsKey(Keys.CommandParser.UserIdKey))
                return;

            Params[Keys.CommandParser.UserIdKey] = Params[Keys.CommandParser.UserIdKey].Trim();

            if (Params[Keys.CommandParser.UserIdKey].StartsWith("<@"))
                Params[Keys.CommandParser.UserIdKey] = Params[Keys.CommandParser.UserIdKey].Substring(2, 9);

            else if (Context.UserNameCache.Any(x => x.Value == Params[Keys.CommandParser.UserIdKey]))
                Params[Keys.CommandParser.UserIdKey] = Context.UserNameCache.First(x => x.Value == Params[Keys.CommandParser.UserIdKey]).Key;
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
            {
                Params.Add(Keys.CommandParser.UserIdKey, ParamsRaw.Substring(2, 9));
            }

            else if (_words.Count > _cmdWordIndex + 1
                     && Context.UserNameCache.Any(x => x.Value == _words[_cmdWordIndex + 1]))
            {
                Params.Add(Keys.CommandParser.UserIdKey, Context.UserNameCache.First(x => x.Value == _words[_cmdWordIndex + 1]).Key);
            }
        }

        private bool ToBot => Context.Message.MentionsBot
                             || Context.Message.IsDirectMessage();

        private string Command => 
            _words
                .FirstOrDefault(x=>!IsBotReference(x))
                ?.ToLower();

        private string ParamsRaw { get; set; }
        private Dictionary<string,string> Params { get; set; }

        private bool IsBotReference(string str)
        {
            if (str == $"<@{Context.BotUserID}>"
                || str == Context.BotUserName)
                return true;

            var bot = Context.Get<Bot>(Keys.StaticContextKeys.Bot);
            return bot.Aliases.Contains(str.ToLower());
        }
    }
}