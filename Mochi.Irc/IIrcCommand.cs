namespace Mochi.Irc;

public interface IIrcCommand
{
    public string Command { get; }
    public List<string> Arguments { get; }
}