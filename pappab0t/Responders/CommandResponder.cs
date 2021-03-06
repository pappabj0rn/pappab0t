﻿using System.Collections.Generic;
using System.Linq;
using MargieBot;
using pappab0t.Abstractions;

namespace pappab0t.Responders
{
    public class CommandResponder : IResponder
    {
        private readonly ICommandDataParser _cmdDataParser;
        private readonly List<Command> _commands;
        private CommandData _commandData;

        public CommandResponder(ICommandDataParser cmdDataParser, List<Command> commands)
        {
            _cmdDataParser = cmdDataParser;
            _commands = commands;
        }

        public bool CanRespond(ResponseContext context)
        {
            _cmdDataParser.Context = context;
            _commandData = _cmdDataParser.Parse();

            return _commands.Any(x => x.RespondsTo(_commandData.Command));
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var cmd = _commands
                .First(x => x.RespondsTo(_commandData.Command));

            cmd.CommandData = _commandData;
            cmd.Context = context;

            return cmd.GetResponse();
        }
    }
}