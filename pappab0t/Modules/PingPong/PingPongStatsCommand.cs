using System.Collections.Generic;
using System.Linq;
using System.Text;
using MargieBot;
using pappab0t.Abstractions;

namespace pappab0t.Modules.PingPong
{
    public class PingPongStatsCommand : Command
    {
        public override BotMessage GetResponse()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Ping sent: {PingPongStats.PingsSent}");
            sb.AppendLine($"Last ping: {PingPongStats.LastPingTime:G}");
            sb.AppendLine($"Last duration: {PingPongStats.LastPongTime - PingPongStats.LastPingTime:g}");
            sb.AppendLine($"Last failed ping: {PingPongStats.LastFailedPing:G}");
            sb.AppendLine($"Last response: {PingPongStats.LastResponse}");

            return new BotMessage
            {
                Text = sb.ToString()
            };
        }

        public override bool RespondsTo(string cmd)
        {
            return CommandStrings.Contains(cmd);
        }

        public IEnumerable<string> CommandStrings => new[] {"pingpongstats", "pps"};
    }
}
