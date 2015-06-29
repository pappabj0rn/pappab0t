﻿using System.Text.RegularExpressions;
using MargieBot.Models;
using MargieBot.Responders;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using pappab0t.Modules.DikaGame;

namespace pappab0t.Responders
{
    public class DikaGameResponder : IResponder, IExposedCapability
    {
        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.IsDirectMessage()) &&
                   Regex.IsMatch(context.Message.Text, @"\b(dikagame|dg)\b", RegexOptions.IgnoreCase);
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var game = new Game();
            var score = game.Play();

            return new BotMessage
            {
                Text = "Du fick {0}p. {1}".With(score, context.Get<Phrasebook>().GetExclamation())
            };
        }

        public ExposedInformation Info
        {
            get 
            { 
                return new ExposedInformation
                            {
                                Usage = "dikagame|dg", 
                                Explatation = "Blandar upp en kortlek och kör en omgång DikaGame(tm)."
                            }; 
            }
        }
    }
}
