using Mochi.Irc;
using Mochi.StreamKit.Twitch.Chat.Irc;

namespace Mochi.StreamKit.Twitch.Chat;

public class TwitchIrcMessageSerializer : IrcMessageSerializer
{
    public TwitchIrcMessageSerializer()
    {
        RegisterTagParser("badges", new BadgeTagParser());
        RegisterTagParser("emotes", new EmoteTagParser());
        RegisterTagParser("emote-sets", new EmoteSetTagParser());
        
        RegisterCommandParser("CAP", new CapIrcCommandParser());
    }
}