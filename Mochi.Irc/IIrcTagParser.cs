namespace Mochi.Irc;

public interface IIrcTagParser
{
    public IIrcTagValue Parse(string value);
}

public interface IIrcTagParser<out T> : IIrcTagParser where T : IIrcTagValue
{
    public new T Parse(string value);
    IIrcTagValue IIrcTagParser.Parse(string value) => Parse(value);
}

public class GenericTagParser : IIrcTagParser<LiteralIrcTagValue>
{
    public LiteralIrcTagValue Parse(string value) => new(value);
}