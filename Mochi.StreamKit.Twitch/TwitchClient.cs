using Mochi.StreamKit.Twitch.API;
using Mochi.StreamKit.Twitch.Chat;
using Mochi.StreamKit.Twitch.OAuth2;

namespace Mochi.StreamKit.Twitch;

public class TwitchClient
{
    public TwitchChatClient Chat { get; }
    public TwitchRestApiClient Rest { get; }
    public event Action<User>? Ready;

    public TwitchClient()
    {
        Chat = new TwitchChatClient(this);
        Rest = new TwitchRestApiClient(this);
    }

    public async Task LoginAsync(Credential credential)
    {
        await Rest.LoginAsync(credential);
        await Chat.LoginAsync(credential);
        Ready?.Invoke(Rest.CurrentUser!);
    }
}