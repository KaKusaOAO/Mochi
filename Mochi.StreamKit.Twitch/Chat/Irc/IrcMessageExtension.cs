using Mochi.Irc;

namespace Mochi.StreamKit.Twitch.Chat.Irc;

public static class IrcMessageExtension
{
    public static BadgeCollection GetBadges(this IrcMessage message) => 
        message.Tags.Values.OfType<BadgeCollection>().FirstOrDefault() ?? new BadgeCollection("");
    
    public static EmoteCollection GetEmotes(this IrcMessage message) => 
        message.Tags.Values.OfType<EmoteCollection>().FirstOrDefault() ?? new EmoteCollection("");
}