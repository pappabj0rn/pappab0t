using System;

namespace pappab0t.Models
{
    public interface IPhrasebook
    {
        string Affirmation();

        string AttentionResponse(string input);

        string Exclamation();

        string MutedExclamation();

        string ScoreboardHype();

        string YoureWelcome();

        string OpenAppology();

        /// <summary>
        /// Foramt string
        /// 0: subject
        /// 1: name
        /// </summary>
        /// <returns></returns>
        [Obsolete("use IDontKnowXxxNamedYyy(string,string)")]
        string IDontKnowXxxNamedYyyFormat();
        string IDontKnowXxxNamedYyy(string xxx, string yyy);

        string IDidntUnderstand();

        /// <summary>
        /// Format string
        /// 0: funds needed
        /// </summary>
        /// <returns></returns>
        [Obsolete("use PlayInsufficientFunds(decimal)")]
        string InsufficientFundsFormat();

        string PlayInsufficientFunds(decimal required);

        /// <summary>
        /// Format string 
        /// 0: amount of points
        /// </summary>
        /// <returns></returns>
        [Obsolete("use PlayDidntMakeHighScore(int)")]
        string DidntMakeHighScoreFormat();

        string PlayDidntMakeHighScore(int points);

        /// <summary>
        /// Format string
        /// 0: points
        /// 1: position
        /// 2: payout
        /// </summary>
        /// <returns></returns>
        [Obsolete("use PotPayout(int, int, decimal)")]
        string PotPayoutFormat();

        string PotPayout(decimal money);

        /// <summary>
        /// Format string
        /// 0: high score name
        /// 1: player name
        /// 2: position
        /// </summary>
        /// <returns></returns>
        [Obsolete("use NewHighscore(string, string, int)")]
        string NewHighscoreFormat();

        string NewHighscore(string hsName, string player, int position);

        string NoPoints();

        string Noted();

        string QuestionAction();
        string QuestionSimilarUrl();

        string CantMoveSoulboundItems();

        /// <summary>
        /// Format string
        /// 0: UserUUID
        /// 1: Because
        /// </summary>
        /// <returns></returns>
        [Obsolete("use CreditUserBecause(string, string)")]
        string CreditUserBecauseFormat();

        string CreditUserBecause(string userUuid, string reason);

        string DesribeItemToFewItems();

        string DescribeUser();

        [Obsolete("use DidntMakeHighScoreInCount(int, int)")]
        string DidntMakeHighScoreInCountFormat();

        string DidntMakeHighScoreInCount(int points, int turns);
        /// <summary>
        /// Format string
        /// 0: money, decimal
        /// </summary>
        /// <returns></returns>
        string MoneyTransfered(decimal amount);

        string MoneyTransferInsufficientFunds();

        string NoDataFound();

        string ItemCreated(string item);
        string ItemDescription(string typeName, string description);
        string ItemTransfered(string item);
        string ItemTransferToFewItems();

        string Impossible();

        string TauntOld();

        string ThankYou();

        string TimedBombExpired();
    }
}