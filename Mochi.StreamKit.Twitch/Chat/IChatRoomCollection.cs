namespace Mochi.StreamKit.Twitch.Chat;

public interface IChatRoomCollection : IReadOnlyDictionary<string, ChatRoom>, IReadOnlyCollection<ChatRoom>
{
    
}