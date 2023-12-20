namespace Mochi.Irc;

public interface IIrcCommandParser
{
    public IIrcCommand Parse(string args);
}

public interface IIrcCommandParser<out T> : IIrcCommandParser where T : IIrcCommand
{
    public new T Parse(string args);
    IIrcCommand IIrcCommandParser.Parse(string args) => Parse(args);
}