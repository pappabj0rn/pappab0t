using MargieBot;

namespace pappab0t.Responders
{
    public interface ICommandDataParser
    {
        ResponseContext Context { get; set; }

        CommandData Parse();
    }
}