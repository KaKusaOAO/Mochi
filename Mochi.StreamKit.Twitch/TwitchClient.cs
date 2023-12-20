using Mochi.Irc;
using Mochi.StreamKit.Twitch.API;
using Mochi.StreamKit.Twitch.Chat;
using Mochi.StreamKit.Twitch.OAuth2;
using Mochi.StreamKit.Twitch.Entities;
using Mochi.StreamKit.Twitch.Rest;
using User = Mochi.StreamKit.Twitch.API.User;

namespace Mochi.StreamKit.Twitch;

public class TwitchClient : IDisposable
{
    internal ClientState State { get; }

    public event Action<Entities.User>? Ready;
    
    public TwitchChatClient Chat { get; }
    public TwitchRestApiClient Rest { get; }
    private bool _disposed;

    public TwitchClient()
    {
        State = new ClientState(this);
        Chat = new TwitchChatClient(this);
        Rest = new TwitchRestApiClient(this);
    }

    public async Task LoginAsync(Credential credential)
    {
        await Rest.LoginAsync(credential);
        await Chat.LoginAsync(credential);
        Ready?.Invoke(Rest.CurrentUser);
    }

    public GlobalUser? GetCachedUser(string id) => State.GetUserFromCache(id);
    
    public GlobalUser? GetCachedUserByName(string name) => State.GetUserFromCacheByName(name);
    
    public GlobalUser GetOrCreateUser(User model) => 
        State.GetOrAddUser(model.Id, x => GlobalUser.Create(this, x, model));

    public EmoteSet? GetEmoteSetFromCache(string id) => State.GetEmoteSetFromCache(id);

    public void RemoveEmoteSetFromCache(string id) => State.RemoveEmoteSetFromCache(id);
    
    public EmoteSet GetOrCreateEmoteSet(string id) => State.GetOrAddEmoteSet(id);
    
    public GlobalUser GetOrCreateUser(IrcMessage model)
    {
        var id = model.Tags["user-id"]!.RawValue;
        return State.GetOrAddUser(id, x => GlobalUser.Create(this, x, model));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Chat.Dispose();
        Rest.Dispose();
    }
}