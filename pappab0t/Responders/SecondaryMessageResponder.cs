using MargieBot.Models;

namespace pappab0t.Responders
{
    public class SecondaryMessageResponder : ResponderBase
    {
        public static BotMessage Message { get; set; }

        public override bool CanRespond(ResponseContext context)
        {
            return Message != null;
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            var msg = Message;
            Message = null;
            return msg;
        }
    }
}
