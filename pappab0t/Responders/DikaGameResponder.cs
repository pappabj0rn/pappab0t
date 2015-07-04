using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MargieBot.Models;
using pappab0t.Abstractions;
using pappab0t.Extensions;
using pappab0t.Models;
using pappab0t.Modules.DikaGame;
using pappab0t.Modules.Inventory;

namespace pappab0t.Responders
{
    public class DikaGameResponder : ResponderBase, IExposedCapability
    {
        private const decimal GameCost = 1;
        private const decimal PotPercentage = .75m;
        private const string GameKey = "DikaGame";

        private static readonly Dictionary<int, decimal> PayoutDictionary = new Dictionary<int, decimal>
        {
            {1,0.75m},
            {2,0.50m},
            {3,0.25m},
            {4,0.20m},
            {5,0.15m},
            {6,0.12m},
            {7,0.09m},
            {8,0.07m},
            {9,0.05m},
            {10,0.02m}
        };

        public override bool CanRespond(ResponseContext context)
        {
            return (context.Message.MentionsBot && Regex.IsMatch(context.Message.Text, @"^\w+\s(?:dikagame|dg)", RegexOptions.IgnoreCase))
                || (context.Message.IsDirectMessage() && Regex.IsMatch(context.Message.Text, @"^(?:dikagame|dg)", RegexOptions.IgnoreCase));
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;

            var invMan = new InventoryManager(Context);
            var userInv = invMan.GetUserInventory();

            if(userInv.BEK < GameCost)
                return new BotMessage{Text = PhraseBook.GetInsufficientFundsFormat().With(GameCost)};

            userInv.BEK -= GameCost;
            invMan.Save(userInv);

            var game = new Game();
            var score = game.Play();

            decimal payout;
            int outcome;
            using (var session = DocumentStore.OpenSession())
            {
                var pot = session.Query<MoneyPot>().FirstOrDefault(x => x.Name == GameKey) ?? new MoneyPot { Name = GameKey };
                pot.BEK += Math.Round(GameCost * PotPercentage,2);
            
                var highScore = session.Query<HighScore>().FirstOrDefault(x => x.Name == GameKey) ?? new HighScore { Name = GameKey };
                outcome = highScore.TryAddNewScore(new Score { UserId = userInv.UserId, Value = score });

                if (outcome < 1)
                {
                    session.Store(pot);
                    session.SaveChanges();
                    return new BotMessage { Text = PhraseBook.GetDidntMakeHighScoreFormat().With(score) };
                }
                    

                payout = Math.Round(PayoutDictionary[outcome] * pot.BEK,2)+GameCost;
                pot.BEK -= payout;
                userInv.BEK += payout;

                session.Store(pot);
                session.Store(highScore);
                session.SaveChanges();
            }
            
            invMan.Save(userInv);
            
            return new BotMessage
            {
                Text ="{0} {1}".With(
                        PhraseBook.GetPotPayoutFormat().With(score, outcome, payout),
                        PhraseBook.GetExclamation())
            };
        }

        public ExposedInformation Info
        {
            get 
            { 
                return new ExposedInformation
                            {
                                Usage = "dikagame|dg", 
                                Explatation = "Blandar upp en kortlek och kör en omgång DikaGame(tm). Ett spel kostar {0}kr.".With(GameCost)
                            }; 
            }
        }
    }
}
