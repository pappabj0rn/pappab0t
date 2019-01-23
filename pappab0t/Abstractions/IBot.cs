using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MargieBot;

namespace pappab0t.Abstractions
{
    public interface IBot
    {
        IReadOnlyDictionary<string, SlackChatHub> ConnectedHubs { get; }
        string UserName { get; }
        string UserID { get; }
        string TeamName { get; }
        IEnumerable<string> Aliases { get; set; }
        List<IResponder> Responders { get; }
        IReadOnlyList<SlackChatHub> ConnectedChannels { get; }
        IReadOnlyList<SlackChatHub> ConnectedDMs { get; }
        IReadOnlyList<SlackChatHub> ConnectedGroups { get; }
        string TeamID { get; }
        bool IsConnected { get; }
        DateTime? ConnectedSince { get; set; }
        Dictionary<string, object> ResponseContext { get; }
        string SlackKey { get; }

        event MargieMessageReceivedEventHandler MessageReceived;
        event MargieConnectionStatusChangedEventHandler ConnectionStatusChanged;

        Task Connect(string slackKey);
        void Disconnect();
        Task Say(BotMessage message);
        Task SendIsTyping(SlackChatHub chatHub);
    }
}
