﻿using MargieBot;
using pappab0t.Responders;

namespace pappab0t
{
    public abstract class Command
    {
        public virtual CommandData CommandData { get; set; }

        public abstract BotMessage GetResponse();
        public abstract bool RespondsTo(string cmd);
    }
}
