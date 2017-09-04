using pappab0t.Models;

namespace pappab0t
{
    public interface IUrlParser
    {
        UrlMatchData Parse(string url);
    }
}