using Mochi.Irc;
using Mochi.Texts;

namespace Mochi.StreamKit.Twitch.Entities;

public interface IChatMessage : IEntity<string>
{
    public List<Badge> Badges { get; }
    public IComponent Content { get; }
    public string OriginalContent { get; }
    public IUser Author { get; }
    public bool IsOwner { get; }
    public DateTimeOffset Timestamp { get; }
    public bool IsFirstTime { get; }
    public bool HasGift { get; }
    public bool IsSentFromLocal { get; }
    public CommentReference? CommentReference { get; }
    public bool IsMeAction { get; }
}