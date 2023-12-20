using Mochi.Irc;

namespace Mochi.StreamKit.Twitch.Chat;

public class BadgeTagParser : IIrcTagParser<BadgeCollection>
{
    public BadgeCollection Parse(string value) => new(value);
}