using MargieBot.Models;

namespace pappab0t.Extensions
{
    public static class SlackMessageExtensions
    {
        public static bool IsDirectMessage(this SlackMessage msg)
        {
            return msg.ChatHub.Type == SlackChatHubType.DM;
        }
    }
}