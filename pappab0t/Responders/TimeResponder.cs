﻿using System;
using System.Text.RegularExpressions;
using MargieBot;

using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;

namespace pappab0t.Responders
{
    public class TimeResponder : IResponder, IExposedCapability
    {
        public bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot || context.Message.IsDirectMessage()) &&
                   Regex.IsMatch(context.Message.Text, @"\btid\b", RegexOptions.IgnoreCase);
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage{Text = DateTime.Now.ToLongTimeString()};
        }

        public ExposedInformation Info
        {
            get { return new ExposedInformation
            {
                Usage = "tid", 
                Explatation = "Visar serverns tid."
            }; }
        }
    }
}
