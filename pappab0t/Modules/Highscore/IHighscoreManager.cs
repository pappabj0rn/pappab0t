namespace pappab0t.Modules.Highscore
{
    public interface IHighScoreManager
    {
        int Handle(string highScoreName, string userId, int score);
    }
}