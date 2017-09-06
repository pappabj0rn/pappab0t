using System;

namespace pappab0t.Models
{
    public class UserUrlPost
    {
        public UserUrlPost()
        {
            Created = DateTime.Now;
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public UrlMatchData UrlMatchData { get; set; }
        public DateTime Created { get; set; }
    }
}