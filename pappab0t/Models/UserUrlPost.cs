namespace pappab0t.Models
{
    public class UserUrlPost
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public UrlMatchData UrlMatchData { get; set; }
        public int TimeStamp { get; set; }
    }
}