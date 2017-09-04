using pappab0t.Models;

namespace pappab0t
{
    public sealed class EmptyUrlMatchData : UrlMatchData
    {
        public override string Protocol => string.Empty;
        public override string Domain => string.Empty;
        public override string Path => string.Empty;
        public override string FileName => string.Empty;
        public override string Query => string.Empty;
        public override string Anchor => string.Empty;
    }
}