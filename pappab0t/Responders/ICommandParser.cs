using System.Collections.Generic;
using MargieBot;

namespace pappab0t.Responders
{
    public interface ICommandParser
    {
        bool ToBot { get; }
        string Command { get; }
        string ParamsRaw { get; }
        Dictionary<string, string> Params { get; }
        ResponseContext Context { get; set; }
        void Parse();
    }
}