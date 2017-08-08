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
        private const decimal GameCost = 1;
        private const decimal PotPercentage = .75m;
        private const string GameKey = "DerangementsGame";

        private static readonly Dictionary<int, decimal> PayoutDictionary = new Dictionary<int, decimal>
        {
            {1,1.0m},
            {2,2.0m},
            {3,4.0m},
            {4,8.0m},
            {5,16.0m},
            {6,32.0m},
            {7,64.0m},
            {8,128.0m},
            {9,256.0m},
            {10,512.0m}
        };

        public override bool CanRespond(ResponseContext context)
        {
            return context.Message.IsDirectMessage() && Regex.IsMatch(context.Message.Text, @"^(?:\bderangements\b|\bdr\b)", RegexOptions.IgnoreCase);
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;

            var invMan = new InventoryManager(Context);
            var userInv = invMan.GetUserInventory();

            if (userInv.BEK < GameCost)
                return new BotMessage { Text = PhraseBook.GetInsufficientFundsFormat().With(GameCost) };

            userInv.BEK -= GameCost;
            invMan.Save(userInv);

            var game = new Game
            {
                MissSymbol = ":flower_playing_cards:",
                HitSymbol = ":black_joker:"
            };

            var score = game.Play();
            
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
                    return new BotMessage { Text = PhraseBook.NoPoints() };
                }

                var highScore = session.Query<HighScore>().FirstOrDefault(x => x.Name == GameKey) ?? new HighScore { Name = GameKey };
                outcome = highScore.TryAddNewScore(new Score { UserId = userInv.UserId, Value = score });

                if (outcome < 1)
                {
                    session.Store(pot);
                    session.SaveChanges();
                    return new BotMessage { Text = $"{game.Outcome}\n{PhraseBook.GetDidntMakeHighScoreFormat().With(score)}" };
                }


                payout = Math.Min(Math.Round(PayoutDictionary[score] * GameCost, 2), pot.BEK);
                pot.BEK -= payout;
                userInv.BEK += payout;

                session.Store(pot);
                session.Store(highScore);
                session.SaveChanges();
            }

            invMan.Save(userInv);

            var channelNames = context.Get<Dictionary<string, string>>(Keys.StaticContextKeys.ChannelsNameCache);
            SecondaryMessageResponder.Message = new BotMessage
            {
                ChatHub = new SlackChatHub { ID = channelNames.First(x => x.Value == "general").Key },
                Text = PhraseBook.GetNewHighscoreFormat().With(GameKey, context.UserNameCache[context.Message.User.ID], outcome)
            };

            return new BotMessage
            {
                Text = $"{game.Outcome}\n{PhraseBook.GetPotPayoutFormat().With(score, outcome, payout)} {PhraseBook.GetExclamation()}"
            };
        }

        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "derangements|dr",
            Explatation = $"Endast DM. Blandar upp tio kort och kör en omgång Derangements. Ett spel kostar {GameCost}kr."
        };
    }
}