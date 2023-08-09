using System.Net.WebSockets;
using Mochi.IO;
using Mochi.Irc;
using Mochi.StreamKit.Twitch.Chat.Irc;
using Mochi.StreamKit.Twitch.OAuth2;
using Mochi.Utils;

namespace Mochi.StreamKit.Twitch.Chat;

public class TwitchChatClient : IDisposable
{
    public TwitchClient Client { get; }
    private readonly ClientWebSocket _webSocket;
    private readonly WebSocketInputStream _inputStream;
    private readonly WebSocketOutputStream _outputStream;
    internal readonly IrcHandler _ircHandler;
    private readonly MutableChatRoomCollection _chatRooms = new();
    public IChatRoomCollection ChatRooms => _chatRooms;

    public event Func<TwitchComment, Task>? MessageReceived;

    public TwitchChatClient(TwitchClient client)
    {
        Client = client;
        _webSocket = new ClientWebSocket();
        _inputStream = new WebSocketInputStream(_webSocket);
        _outputStream = new WebSocketOutputStream(_webSocket, WebSocketMessageType.Text);
        _ircHandler = new IrcHandler(_inputStream, _outputStream, new TwitchIrcMessageSerializer());
    }

    public async Task LoginAsync(Credential credential)
    {
        var accessToken = credential.AccessToken;
        if (credential.Type == CredentialType.App)
        {
            accessToken = null;
        }
        
        await _webSocket.ConnectAsync(new Uri(TwitchConfig.IrcWebSocketUrl), CancellationToken.None);
        
        await _ircHandler.WriteMessageAsync(new IrcMessage(new CapIrcCommand(CapabilityAction.Request)) 
        {
            Parameters = string.Join(' ', new List<string>
            {
                "twitch.tv/tags", "twitch.tv/commands"
            })
        });

        if (accessToken == null)
        {
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("PASS", "SCHMOOPIIE")));
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("NICK", "justinfan27073")));
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("USER", "justinfan27073", "8", "*"))
            {
                Parameters = "justinfan27073"
            });
        }
        else
        {
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("PASS", $"oauth:{accessToken}")));
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("NICK", Client.Rest.CurrentUser!.Login.ToLower())));
        }

        _ = RunEventLoopAsync();
    }

    private async Task RunEventLoopAsync()
    {
        while (true)
        {
            var message = await _ircHandler.ReadMessageAsync();
            if (message == null) return;
            
            var channelName = message.Command.Arguments.FirstOrDefault()?.TrimStart('#');
            
            if (message.Command.Command == "PING")
            {
                await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("PONG"))
                {
                    Parameters = message.Parameters
                });
            }
            
            if (message.Command.Command == "JOIN")
            {
                if (!_chatRooms.ContainsKey(channelName!))
                {
                    var room = new ChatRoom(this, channelName!);
                    _chatRooms.Add(room);
                }
            }

            if (message.Command.Command is "PRIVMSG" or "ROOMSTATE")
            {
                await _chatRooms[channelName!].OnReceivedIrcMessageAsync(message);
            }
        }
    }

    public Task<ChatRoom> SubscribeChannelAsync(string name) => SubscribeChannelAsync(name, CancellationToken.None);
    public async Task<ChatRoom> SubscribeChannelAsync(string name, CancellationToken token)
    {
        if (_chatRooms.TryGetValue(name, out var room)) return room;
        await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("JOIN", $"#{name}")));
        SpinWait.SpinUntil(() => token.IsCancellationRequested || _chatRooms.TryGetValue(name, out room));
        token.ThrowIfCancellationRequested();
        
        room.MessageReceived += RoomOnMessageReceived;
        return room;
    }

    public async Task UnsubscribeChannelAsync(string name)
    {
        if (!_chatRooms.TryGetValue(name, out var room)) return;
        
        room.MessageReceived -= RoomOnMessageReceived;
        await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("PART", $"#{name}")));
    }

    private async Task RoomOnMessageReceived(TwitchComment comment)
    {
        if (MessageReceived != null) 
            await MessageReceived.Invoke(comment);
    }

    public void Dispose()
    {
        _inputStream.Dispose();
        _outputStream.Dispose();
        _webSocket.Dispose();
        GC.SuppressFinalize(this);
    }
}