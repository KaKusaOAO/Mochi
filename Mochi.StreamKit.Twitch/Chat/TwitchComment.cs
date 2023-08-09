using Mochi.Irc;
using Mochi.StreamKit.Twitch.Chat.Irc;
using Mochi.Texts;

namespace Mochi.StreamKit.Twitch.Chat;

public class TwitchComment : IComment
{
    public IrcMessage Message { get; }
    private readonly Lazy<IComponent> _contentResolver;

    public TwitchComment(ChatRoom chatRoom, IrcMessage message)
    {
        _contentResolver = new Lazy<IComponent>(ResolveContent);
        Message = message;

        if (RoomId != chatRoom.RoomState.RoomId)
            throw new ArgumentException("Room ID mismatch!");

        ChatRoom = chatRoom;
    }
    
    public List<Badge> Badges => Message.GetBadges().Badges.Select(x => ChatRoom.Badges.GetBadgeAsync(x).Result).ToList()!;
    List<IUserBadge> IComment.Badges => Badges.OfType<IUserBadge>().ToList();

    public string Id => "tw-" + Message.Tags["id"]?.RawValue;
    public string Name => Message.Source!.Nick!;
    public IComponent Content => _contentResolver.Value;

    private IComponent ResolveContent()
    {
        var content = Message.Parameters ?? "";
        if (content.StartsWith("\u0001ACTION "))
        {
            content = content.Replace("\u0001ACTION ", "").Replace("\u0001", "");
        }

        return Message.GetEmotes().ParseContent(content);
    }
    
    public string DisplayName => Message.Tags["display-name"]?.RawValue ?? Name;
    public string ProfileImage { get; }
    public bool IsOwner => RoomId == Message.Tags["user-id"]?.RawValue;
    public DateTimeOffset Timestamp { get; }
    public string UserId { get; }
    public bool IsFirstTime { get; }
    public bool HasGift { get; }
    public ChatRoom ChatRoom { get; }
    public string RoomId => Message.Tags["room-id"]?.RawValue;
}