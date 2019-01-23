using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot;
using pappab0t.Abstractions;

namespace pappab0t
{
    public class BotWrapper : IBot
    {
        private readonly Bot _bot;

        public BotWrapper(Bot bot)
        {
            _bot = bot;
        }

        public IReadOnlyDictionary<string, SlackChatHub> ConnectedHubs => _bot.ConnectedHubs;
        public string UserName => _bot.UserName;
        public string UserID => _bot.UserID;
        public string TeamName => _bot.TeamName;
        public IEnumerable<string> Aliases
        {
            get => _bot.Aliases;
            set => _bot.Aliases = value;
        }

        public List<IResponder> Responders => _bot.Responders;
        public IReadOnlyList<SlackChatHub> ConnectedChannels => _bot.ConnectedChannels;
        public IReadOnlyList<SlackChatHub> ConnectedDMs => _bot.ConnectedDMs;
        public IReadOnlyList<SlackChatHub> ConnectedGroups => _bot.ConnectedGroups;
        public string TeamID => _bot.TeamName;
        public bool IsConnected => _bot.IsConnected;
        public DateTime? ConnectedSince
        {
            get => _bot.ConnectedSince;
            set => _bot.ConnectedSince = value;
        }
        public Dictionary<string, object> ResponseContext => _bot.ResponseContext;
        public string SlackKey => _bot.SlackKey;
        public event MargieMessageReceivedEventHandler MessageReceived;
        public event MargieConnectionStatusChangedEventHandler ConnectionStatusChanged;
        public Task Connect(string slackKey)
        {
            return _bot.Connect(slackKey);
        }

        public void Disconnect()
        {
            _bot.Disconnect();
        }

        public Task Say(BotMessage message)
        {
            return _bot.Say(message);
        }

        public Task SendIsTyping(SlackChatHub chatHub)
        {
            return _bot.SendIsTyping(chatHub);
        }
    }
}
