using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MargieBot;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using pappab0t.Modules.Derangements;
using pappab0t.Modules.Inventory;

namespace pappab0t.Responders
{
    public class DerangementsGameResponder : ResponderBase, IExposedCapability
    {
        private readonly IInventoryManager _invMan;
        private const decimal GameCost = 1;
        private const decimal PotPercentage = .75m;
        private const string GameKey = "DerangementsGame";

        public DerangementsGameResponder(IInventoryManager invMan)
        {
            _invMan = invMan;
        }

        public override bool CanRespond(ResponseContext context)
        {
            return context.Message.IsDirectMessage() && Regex.IsMatch(context.Message.Text, @"^(?:\bderangements\b|\bdr\b|\bdrd\b)", RegexOptions.IgnoreCase);
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            return new BotMessage{Text = "Stängd tills vidare då en crach har noterats vid testkörning."};
            Context = context;
            _invMan.Context = context;

            var enableDebugOutput = Regex.IsMatch(context.Message.Text, @"^(?:\bdrd\b)", RegexOptions.IgnoreCase);
            var userInv = _invMan.GetUserInventory();

            if (userInv.BEK < GameCost)
                return new BotMessage { Text = PhraseBook.InsufficientFundsFormat().With(GameCost) };

            userInv.BEK -= GameCost;
            _invMan.Save(userInv);

            var game = new Game
            {
                MissSymbol = ":flower_playing_cards:",
                HitSymbol = ":black_joker:"
            };

            var score = game.Play();
            var debugOutput = enableDebugOutput 
                ? game.DebugOutput 
                : "";

            decimal payout;
            int outcome;

            using (var session = DocumentStore.OpenSession())
            {
                var pot = session.Query<MoneyPot>().FirstOrDefault(x => x.Name == GameKey) ?? new MoneyPot { Name = GameKey };
                pot.BEK += Math.Round(GameCost * PotPercentage, 2);

                if (score == 0)
                {
                    session.Store(pot);
                    session.SaveChanges();
                    return new BotMessage { Text = PhraseBook.NoPoints() + $"\n{debugOutput}" };
                }

                var highScore = session.Query<HighScore>().FirstOrDefault(x => x.Name == GameKey) ?? new HighScore { Name = GameKey };
                outcome = highScore.TryAddNewScore(new Score { UserId = userInv.UserId, Value = score });

                if (outcome < 1)
                {
                    session.Store(pot);
                    session.SaveChanges();
                    return new BotMessage { Text = $"{game.Outcome}\n{PhraseBook.DidntMakeHighScoreFormat().With(score)}\n{debugOutput}" };
                }


                payout = Math.Min(Math.Round((decimal)Math.Pow(2.0,score) * GameCost, 2), pot.BEK);
                pot.BEK -= payout;
                userInv.BEK += payout;

                session.Store(pot);
                session.Store(highScore);
                session.SaveChanges();
            }

            _invMan.Save(userInv);

            var channelNames = context.Get<Dictionary<string, string>>(Keys.StaticContextKeys.ChannelsNameCache);
            SecondaryMessageResponder.Message = new BotMessage
            {
                ChatHub = new SlackChatHub { ID = channelNames.First(x => x.Value == "general").Key },
                Text = PhraseBook.NewHighscoreFormat().With(GameKey, context.UserNameCache[context.Message.User.ID], outcome)
            };

            return new BotMessage
            {
                Text = $"{game.Outcome}\n{PhraseBook.PotPayoutFormat().With(score, outcome, payout)} {PhraseBook.Exclamation()}\n{debugOutput}"
            };
        }

        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "derangements|dr|drd",
            Explatation = $"Endast DM. Blandar upp tio kort och kör en omgång Derangements. Ett spel kostar {GameCost}kr. Nyckel (ex. för hs): {GameKey}. Starta med drd för att få debugutskrift av restulat."
        };
    }
}