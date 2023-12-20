namespace Mochi.Irc;

public class MessageSource
{
    public string? Nick { get; }
    public string Host { get; }

    public MessageSource(string host, string? nick = null)
    {
        Nick = nick;
        Host = host;
    }
}