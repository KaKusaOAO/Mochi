using System.Collections;

namespace Mochi.StreamKit.Twitch.Chat;

public class MutableChatRoomCollection : IChatRoomCollection, ICollection<ChatRoom>
{
    private readonly List<ChatRoom> _chatRooms = new();

    public IEnumerator<ChatRoom> GetEnumerator() => _chatRooms.GetEnumerator();

    IEnumerator<KeyValuePair<string, ChatRoom>> IEnumerable<KeyValuePair<string, ChatRoom>>.GetEnumerator() => 
        _chatRooms.Select(r => new KeyValuePair<string, ChatRoom>(r.ChannelName, r)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(ChatRoom item)
    {
        if (_chatRooms.Any(r => r.ChannelName == item.ChannelName)) return;
        _chatRooms.Add(item);
    }

    public void Clear() => _chatRooms.Clear();

    public bool Contains(ChatRoom item) => _chatRooms.Contains(item);

    public void CopyTo(ChatRoom[] array, int arrayIndex)
    {
        foreach (var t in _chatRooms)
        {
            array[arrayIndex++] = t;
        }
    }

    public bool Remove(ChatRoom item) => _chatRooms.Remove(item);

    int ICollection<ChatRoom>.Count => _chatRooms.Count;

    int IReadOnlyCollection<KeyValuePair<string, ChatRoom>>.Count => _chatRooms.Count;

    public bool IsReadOnly => false;

    public bool ContainsKey(string key) => _chatRooms.Any(r => r.ChannelName == key);

    public bool TryGetValue(string key, out ChatRoom value)
    {
        value = _chatRooms.FirstOrDefault(r => r.ChannelName == key)!;
        return value != null!;
    }

    public ChatRoom this[string key]
    {
        get
        {
            TryGetValue(key, out var result);
            return result;
        }
    }
    
    public IEnumerable<string> Keys => _chatRooms.Select(r => r.ChannelName);
    public IEnumerable<ChatRoom> Values => _chatRooms;
    public int Count => _chatRooms.Count;
}