using Mochi.Irc;

namespace Mochi.StreamKit.Twitch.Chat;

public class EmoteTagParser : IIrcTagParser<EmoteCollection>
{
    public EmoteCollection Parse(string value) => new(value);
}