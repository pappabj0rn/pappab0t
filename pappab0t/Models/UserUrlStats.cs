using System.Collections.Generic;

namespace pappab0t.Models
{
    public class UserUrlStats
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public Dictionary<string, int> DomainCount { get; set; }
        public Dictionary<UrlTargetType, int> TypeCount { get; set; }
    }
}