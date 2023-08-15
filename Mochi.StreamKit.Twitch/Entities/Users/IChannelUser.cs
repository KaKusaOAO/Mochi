using Mochi.StreamKit.Twitch.Chat;

namespace Mochi.StreamKit.Twitch.Entities;

public interface IChannelUser : IUser
{
    public ChatRoom ChatRoom { get; }
    public List<Badge> ChannelBadges { get; }
    public bool IsModerator { get; }
    public bool IsSubscriber { get; }
}