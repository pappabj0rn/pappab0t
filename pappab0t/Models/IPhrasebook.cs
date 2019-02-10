namespace pappab0t.Models
{
    public interface IPhrasebook
    {
        string Affirmation();

        string AttentionResponse(string input);

        string CantMoveSoulboundItems();

        string CreditUserBecause(string userUuid, string reason);

        string DescribeItemToFewItems();

        string DescribeUser();

        string DidntMakeHighScoreInCount(int points, int turns);

        string Exclamation();

        string IDidntUnderstand();

        string IDontKnowXxxNamedYyy(string xxx, string yyy);

        string Impossible();

        string ItemCreated(string item);

        string ItemDescription(string typeName, string description);

        string ItemTransfered(string item);

        string ItemTransferToFewItems();

        string MoneyTransfered(decimal amount);

        string MoneyTransferInsufficientFunds();

        string MutedExclamation();

        string NewHighscore(string hsName, string player, int position);

        string NoDataFound();

        string NoPoints();

        string Noted();

        string OpenAppology();

        string PlayDidntMakeHighScore(int points);

        string PlayInsufficientFunds(decimal required);

        string PotPayout(int points, int position, decimal money, int turns);

        string QuestionAction();

        string QuestionSimilarUrl();

        string ScoreboardHype();

        string TauntOld();

        string ThankYou();

        string TimedBombExpired();

        string YoureWelcome();
    }
}