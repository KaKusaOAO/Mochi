using System.Collections.Concurrent;
using Mochi.StreamKit.Twitch.Entities;

namespace Mochi.StreamKit.Twitch;

internal class ClientState
{
    private readonly TwitchClient _client;
    private readonly ConcurrentDictionary<string, GlobalUser> _users = new();
    private readonly ConcurrentDictionary<string, EmoteSet> _emoteSets = new();

    public ClientState(TwitchClient client)
    {
        _client = client;
    }
    
    
    public GlobalUser? GetUserFromCacheByName(string name) => _users.Values.FirstOrDefault(x => x.Name == name);

    public GlobalUser? GetUserFromCache(string id) => _users.GetValueOrDefault(id);
    
    public GlobalUser GetOrAddUser(string id, Func<string, GlobalUser> factory) => _users.GetOrAdd(id, factory);

    public EmoteSet GetOrAddEmoteSet(string id) => _emoteSets.GetOrAdd(id, x => new EmoteSet(_client, x));

    public EmoteSet? GetEmoteSetFromCache(string id) => _emoteSets.GetValueOrDefault(id);

    public void RemoveEmoteSetFromCache(string id) => _emoteSets.TryRemove(id, out _);
}