using Mochi.Irc;

namespace Mochi.StreamKit.Twitch.Chat;

public class EmoteSetTagParser : IIrcTagParser<EmoteSetCollection>
{
    public EmoteSetCollection Parse(string value) => new(value);
}