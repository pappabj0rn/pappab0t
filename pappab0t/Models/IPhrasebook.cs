namespace pappab0t.Models
{
    public interface IPhrasebook
    {
        string Affirmation();

        string ThankYou();

        string Exclamation();

        string MutedExclamation();

        string AttentionResponse(string input);

        string ScoreboardHype();

        string YoureWelcome();

        string OpenAppology();

        /// <summary>
        /// Foramt string
        /// 0: subject
        /// 1: name
        /// </summary>
        /// <returns></returns>
        string IDontKnowXxxNamedYyyFormat();

        string IDidntUnderstand();

        /// <summary>
        /// Format string
        /// 0: funds needed
        /// </summary>
        /// <returns></returns>
        string InsufficientFundsFormat();

        /// <summary>
        /// Format string 
        /// 0: amount of points
        /// </summary>
        /// <returns></returns>
        string DidntMakeHighScoreFormat();

        /// <summary>
        /// Format string
        /// 0: points
        /// 1: position
        /// 2: payout
        /// </summary>
        /// <returns></returns>
        string PotPayoutFormat();

        /// <summary>
        /// Format string
        /// 0: high score name
        /// 1: player name
        /// 2: position
        /// </summary>
        /// <returns></returns>
        string NewHighscoreFormat();

        string NoPoints();

        string Noted();

        string TauntOld();

        /// <summary>
        /// Format string
        /// 0: UserUUID
        /// 1: Because
        /// </summary>
        /// <returns></returns>
        string CreditUserBecauseFormat();

        string QuestionAction();

        string NoDataFound();
    }
}