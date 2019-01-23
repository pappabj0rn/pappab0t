using System;
using MargieBot;
using pappab0t.Responders;

namespace pappab0t.Modules.PingPong
{
    public class PongResponder : ResponderBase
    {
        public override bool CanRespond(ResponseContext context)
        {
            return context.Message.User.ID == "USLACKBOT"
                   && context.Message.ChatHub.Type == SlackChatHubType.DM
                   && PingPongStatus.WaitingForPong;
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            PingPongStatus.WaitingForPong = false;

            PingPongStats.LastPongTime = SystemTime.Now();
            PingPongStats.LastResponse = context.Message.Text;

            return null;
        }
    }
}
