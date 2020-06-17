using System.Collections.Generic;
using System.Text;
using MargieBot;

namespace pappab0t.Responders
{
    public class DebugResponder : ResponderBase
    {
        public override bool CanRespond(ResponseContext context)
        {
            Init(context);

            return CommandData.Command == "debug";
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Init(context);

            var sb = new StringBuilder();

            if (CommandData.Params.ContainsKey("channels"))
            {
                var channelNames = context.Get<Dictionary<string, string>>(Keys.StaticContextKeys.ChannelsNameCache);
                sb.AppendLine("Channels");
                foreach (var channelName in channelNames)
                {
                    sb.AppendLine($"{channelName.Key}, {channelName.Value}");
                }
            }


            return new BotMessage
            {
                Text = sb.ToString()
            };
        }
    }
}