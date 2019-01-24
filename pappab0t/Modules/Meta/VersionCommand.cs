using System;
using MargieBot;
using pappab0t.Abstractions;

namespace pappab0t.Modules.Meta
{
    public class VersionCommand : Command
    {
        public override BotMessage GetResponse()
        {
            var version = GetType().Assembly.GetName().Version;

            var buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build)
                .AddSeconds(version.Revision * 2);

            return new BotMessage{Text = $"pappab0t {version} ({buildDate:G})" };
        }

        public override bool RespondsTo(string cmd)
        {
            return cmd == "version";
        }
    }
}