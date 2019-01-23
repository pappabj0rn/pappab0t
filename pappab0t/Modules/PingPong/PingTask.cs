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

        public override bool IsDue()
        {
            return _lastRun.AddMilliseconds(Interval) < SystemTime.Now();
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
            }

            var dm = _bot.ConnectedDMs.FirstOrDefault(x => x.Name.ToLower() == AtSlackbot);
            if (dm is null)
            {
                _dmApi.Open(_bot.SlackKey, SlackBotId);
            }

            var sbHub = _bot.ConnectedDMs.FirstOrDefault(x=>x.Name == AtSlackbot);
            Console.WriteLine("Ping " + SystemTime.Now());
            _bot.Say(new BotMessage {Text = "hello", ChatHub = sbHub});

            PingPongStatus.PingSent = SystemTime.Now();
            PingPongStatus.WaitingForPong = true;

            return true;
        }
    }
}
