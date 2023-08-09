using Mochi.Irc;
using Mochi.StreamKit.Twitch.Chat.Irc;

namespace Mochi.StreamKit.Twitch.Chat;

public class TwitchIrcMessageSerializer : IrcMessageSerializer
{
    public TwitchIrcMessageSerializer()
    {
        var badgeTagParser = new BadgeTagParser();
        RegisterTagParser("badges", badgeTagParser);
        RegisterTagParser("badge-info", badgeTagParser);
        RegisterTagParser("emotes", new EmoteTagParser());
        
        RegisterCommandParser("CAP", new CapIrcCommandParser());
    }
}