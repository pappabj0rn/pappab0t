using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MargieBot;
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

        private const string gameMaxCountKey = "gmc";
        private const string DikaGameRegex = @"(?:\bdikagame\b|\bdg\b)(\s+(?<" + gameMaxCountKey + @">\w+))?";

        private int _maxGames = 1;

        public override bool CanRespond(ResponseContext context)
        {
            Init(context);

            return CommandParser.Command == "dg"
                || CommandParser.Command == "dikagame";

            return context.Message.IsDirectMessage()
                   && Regex.IsMatch(context.Message.Text, DikaGameRegex, RegexOptions.IgnoreCase);
        }

        public override BotMessage GetResponse(ResponseContext context)
        {
            Context = context;

            var match = Regex.Match(Context.Message.Text, DikaGameRegex, RegexOptions.IgnoreCase);
            int.TryParse(match.Groups[gameMaxCountKey].Value, out _maxGames);
            if (_maxGames < 1)
                _maxGames = 1;
            

            var invMan = new InventoryManager(Context);
            var userInv = invMan.GetUserInventory();

            var outcome = 0;
            var losses = 0;
            var broke = false;
            var maxScore = 0;
            decimal payout = 0M;
            var game = new Game();

            for (int i = 0; i < _maxGames; i++)
            {
                if (userInv.BEK < GameCost)
                {
                    if(i==0)
                        return new BotMessage { Text = PhraseBook.InsufficientFundsFormat().With(GameCost) };

                    broke = true;
                    break;
                }

                userInv.BEK -= GameCost;
                invMan.Save(userInv);
                
                var score = game.Play();

                if (maxScore < score)
                    maxScore = score;
                
                using (var session = DocumentStore.OpenSession())
                {
                    var pot = session.Query<MoneyPot>().FirstOrDefault(x => x.Name == GameKey) ?? new MoneyPot { Name = GameKey };
                    pot.BEK += Math.Round(GameCost * PotPercentage, 2);

                    var highScore = session.Query<HighScore>().FirstOrDefault(x => x.Name == GameKey) ?? new HighScore { Name = GameKey };
                    outcome = highScore.TryAddNewScore(new Score { UserId = userInv.UserId, Value = score });

                    if (outcome < 1) //no highscore
                    {
                        session.Store(pot);
                        session.SaveChanges();
                        losses++;
                    }
                    else
                    {
                        payout = Math.Round(PayoutDictionary[outcome] * pot.BEK, 2) + GameCost;
                        pot.BEK -= payout;
                        userInv.BEK += payout;

                        session.Store(pot);
                        session.Store(highScore);
                        session.SaveChanges();
                        invMan.Save(userInv);
                        break;
                    }
                }
            }
            
            if(outcome < 1)
            {
                return new BotMessage { Text = PhraseBook.DidntMakeHighScoreInCountFormat().With(maxScore, losses) };
            }

            var channelNames = context.Get<Dictionary<string, string>>(Keys.StaticContextKeys.ChannelsNameCache);
            SecondaryMessageResponder.Message = new BotMessage
            {
                ChatHub = new SlackChatHub{ID = channelNames.First(x=>x.Value=="general").Key},
                Text = PhraseBook.NewHighscoreFormat().With(GameKey, context.UserNameCache[context.Message.User.ID], outcome)
            };

            return new BotMessage
            {
                Text ="{0} {1}".With(
                        PhraseBook.PotPayoutFormat().With(maxScore, outcome, payout, losses + 1),
                        PhraseBook.Exclamation())
            };
        }

        public ExposedInformation Info => new ExposedInformation
        {
            Usage = "dikagame|dg", 
            Explatation = $"Endast DM. Blandar upp en kortlek och kör en omgång DikaGame(tm). Ett spel kostar {GameCost}kr. Nyckel (ex. för hs): {GameKey}."
        };
    }
}
