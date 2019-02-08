using System;
using System.Linq;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.SlackApis;

namespace pappab0t.Modules.PingPong
{
    public class PingTask : ScheduledTask
    {
        private readonly IBot _bot;
        private readonly ISlackDMApi _dmApi;
        private DateTime _lastRun;

        private const string SlackBotId = "USLACKBOT";
        private const string AtSlackbot = "@slackbot";

        public override int Interval => 30 * Milliseconds;

        public PingTask(IBot bot, ISlackDMApi dmApi)
        {
            _bot = bot;
            _dmApi = dmApi;
        }
        public override bool Execute()
        {
            if (!IsDue()
            || !_bot.ConnectedSince.HasValue)
                return false;
            
            _lastRun = SystemTime.Now();

            if (PingPongStatus.WaitingForPong)
            {
                _bot.ConnectedSince = null;
                PingPongStats.LastFailedPing = SystemTime.Now();
                PingPongStatus.WaitingForPong = false;
                return false;
            }

            var dm = _bot.ConnectedDMs.FirstOrDefault(x => x.Name.ToLower() == AtSlackbot);
            if (dm is null)
            {
                _dmApi.Open(_bot.SlackKey, SlackBotId);
            }

            var sbHub = _bot.ConnectedDMs.FirstOrDefault(x=>x.Name == AtSlackbot);
            _bot.Say(new BotMessage {Text = "hello", ChatHub = sbHub});

            PingPongStats.LastPingTime = SystemTime.Now();
            PingPongStats.PingsSent++;
            PingPongStatus.WaitingForPong = true;

            return true;
        }
    }
}
