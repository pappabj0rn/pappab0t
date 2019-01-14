using System.Collections.Generic;

namespace pappab0t.Responders
{
    public class CommandData
    {
        public virtual bool ToBot { get; set; }
        public virtual string Command { get; set; }
        public virtual string ParamsRaw { get; set; }
        public virtual Dictionary<string, string> Params { get; set; }
    }
}