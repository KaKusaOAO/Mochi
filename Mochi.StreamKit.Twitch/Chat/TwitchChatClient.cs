using System.Net.WebSockets;
using Mochi.IO;
using Mochi.Irc;
using Mochi.StreamKit.Twitch.Chat.Irc;
using Mochi.StreamKit.Twitch.Entities;
using Mochi.StreamKit.Twitch.OAuth2;
using Mochi.Texts;
using Mochi.Utils;

namespace Mochi.StreamKit.Twitch.Chat;

public class TwitchChatClient : IDisposable
{
    public TwitchClient Client { get; }
    private ClientWebSocket _webSocket = null!;
    private WebSocketInputStream _inputStream = null!;
    private  WebSocketOutputStream _outputStream = null!;
    private IrcHandler _ircHandler = null!;
    private bool _disposed;
    private readonly MutableChatRoomCollection _chatRooms = new();
    private Credential? _lastCredential;
    
    public IChatRoomCollection ChatRooms => _chatRooms;

    public event Func<TwitchComment, Task>? MessageReceived;

    public TwitchChatClient(TwitchClient client)
    {
        Client = client;
        InitializeConnection();
    }

    public async Task ReadTestMessageAsync(string origin)
    {
        Logger.Info(TranslateText.Of("Test Read: %s")
            .AddWith(Component.Literal(origin).SetColor(TextColor.Gold))
        );
        var message = _ircHandler.ParseMessage(origin);
        await ProcessMessageAsync(message);
    }

    private void InitializeConnection()
    {
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
                "tags", "commands", "membership"
            }.Select(x => $"twitch.tv/{x}"))
        });

        if (accessToken == null)
        {
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("PASS", "SCHMOOPIIE")));
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("NICK", "justinfan27073")));
            
            // Not sure what this means, but it presents in the not-logged-in Twitch chat login sequence.
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("USER", "justinfan27073", "8", "*"))
            {
                Parameters = "justinfan27073"
            });
        }
        else
        {
            SpinWait.SpinUntil(() => Client.Rest.CurrentUser != null!);
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("PASS", $"oauth:{accessToken}")));
            await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("NICK", Client.Rest.CurrentUser.Name.ToLower())));
        }

        _lastCredential = credential;
        _ = RunEventLoopAsync();
    }

    private async Task RunEventLoopAsync()
    {
        await RunMessageLoopAsync();
        if (_disposed) return;
        
        try
        {
            await _inputStream.DisposeAsync();
            await _outputStream.DisposeAsync();
            _webSocket.Dispose();
        }
        catch (Exception ex)
        {
            Logger.Warn(ex);
        }

        if (_lastCredential == null)
        {
            throw new Exception("Cannot re-login as we did not have a successful login before.");
        }

        // Wait for a period of time to prevent reconnecting too much time in a short period of time.
        // This can happen if network issues are present (or debugging, which is bad due to the rate limit thing)
        Logger.Info("Waiting for reconnecting...");
        await Task.Delay(1000);
        
        InitializeConnection();
        await LoginAsync(_lastCredential);

        var rooms = _chatRooms.Keys.ToList();
        foreach (var room in _chatRooms.Values.ToList())
        {
            RemoveChannel(room);
        }
        
        foreach (var room in rooms)
        {
            await SubscribeChannelAsync(room);
        }
    }

    private async Task ProcessMessageAsync(IrcMessage message)
    {
        try
        {
            var channelName = message.Command.Arguments
                .FirstOrDefault(x => x.StartsWith('#'))?.TrimStart('#');

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
                    var user = await Client.Rest.GetUserByNameAsync(channelName!);
                    var room = new ChatRoom(this, user!);
                    _chatRooms.Add(room);
                }
            }

            if (message.Command.Command is "JOIN" or "PART" or "PRIVMSG" or "ROOMSTATE" or "USERSTATE" or "353")
            {
                await _chatRooms[channelName!].OnReceivedIrcMessageAsync(message);
            }

            if (message.Command.Command == "RECONNECT")
            {
                // We should close the connection if we receive this command
                // The event loop will and should start a new connection automatically
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Unexpected error occurred while processing IRC messages!");
            Logger.Error(ex);
        }
    }

    private async Task RunMessageLoopAsync()
    {
        while (!_disposed)
        {
            try
            {
                IrcMessage? message;
                try
                {
                    message = await _ircHandler.ReadMessageAsync();
                    if (message == null) return;
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions.OfType<WebSocketException>().Any()) return;
                    Logger.Error("Unexpected error occurred while processing IRC messages!");
                    Logger.Error(ex);
                    continue;
                }
                catch (WebSocketException ex)
                {
                    Logger.Error("Unexpected error occurred while processing IRC messages!");
                    Logger.Error(ex);
                    continue;
                }

                await ProcessMessageAsync(message);
            }
            catch (Exception ex)
            {
                Logger.Error("Unexpected error occurred while reading IRC messages!");
                Logger.Error(ex);
            }
        }
    }

    public Task<ChatRoom> SubscribeChannelAsync(string name)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        return SubscribeChannelAsync(name, cts.Token);
    }

    public async Task<ChatRoom> SubscribeChannelAsync(string name, CancellationToken token)
    {
        if (_chatRooms.TryGetValue(name, out var room)) return room;
        await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("JOIN", $"#{name}")));
        SpinWait.SpinUntil(() => token.IsCancellationRequested || _chatRooms.TryGetValue(name, out room));
        token.ThrowIfCancellationRequested();
        
        room.MessageReceived += RoomOnMessageReceived;
        return room;
    }

    private void RemoveChannel(ChatRoom room)
    {
        _chatRooms.Remove(room);
        room.MessageReceived -= RoomOnMessageReceived;
    }
    
    public async Task UnsubscribeChannelAsync(string name)
    {
        if (!_chatRooms.TryGetValue(name, out var room)) return;
        RemoveChannel(room);
        await _ircHandler.WriteMessageAsync(new IrcMessage(new GenericCommand("PART", $"#{name}")));
    }

    private async Task RoomOnMessageReceived(TwitchComment comment)
    {
        if (MessageReceived != null) 
            await MessageReceived.Invoke(comment);
    }

    public async Task WriteMessageAsync(IrcMessage message) => await _ircHandler.WriteMessageAsync(message);

    public void Dispose()
    {
        _disposed = true;
        _inputStream.Dispose();
        _outputStream.Dispose();
        _webSocket.Dispose();
        GC.SuppressFinalize(this);
    }
}