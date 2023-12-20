namespace Mochi.Irc;

public class IrcMessage
{
    public TagCollection Tags { get; init; } = new();
    public MessageSource? Source { get; init; }
    public IIrcCommand Command { get; init; }
    public string? Parameters { get; init; }

    public IrcMessage(IIrcCommand command)
    {
        Command = command;
    }
}