using Mochi.Texts;

namespace Mochi.StreamKit;

public interface IComment
{
    public List<IUserBadge> Badges { get; }
    public string Id { get; }
    public string Name { get; }
    public IComponent Content { get; }
    public string DisplayName { get; }
    public string ProfileImage { get; }
    public bool IsOwner { get; }
    public DateTimeOffset Timestamp { get; }
    public string UserId { get; }
    public bool IsFirstTime { get; }
    public bool HasGift { get; }
}