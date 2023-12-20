using System.Collections.Concurrent;
using System.Text.Json.Nodes;
using Mochi.Irc;
using Mochi.StreamKit.Twitch.Entities;
using Mochi.StreamKit.Twitch.Rest;
using Mochi.Texts;
using Mochi.Utils;

namespace Mochi.StreamKit.Twitch.Chat;

public class ChatRoom
{
    public TwitchChatClient Client { get; }
    private readonly ConcurrentDictionary<string, TwitchComment> _comments = new();
    private readonly ConcurrentDictionary<string, ChannelUser> _audiences = new();
    private readonly ConcurrentDictionary<string, ChannelUser> _users = new();
    private readonly ConcurrentQueue<string> _orderedCommentIds = new();
    private readonly ChannelBadgeStore _badges;
    private RoomState? _roomState;
    private readonly SemaphoreSlim _audiencesLock = new(1, 1);
    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly ConcurrentQueue<Guid> _joinPartQueue = new();

    public event Func<TwitchComment, Task>? MessageReceived;
    private event Func<IrcMessage, Task>? IrcMessageReceived;
        
    public string ChannelName { get; }
    public RoomState RoomState => _roomState == null ? null! : _roomState with {};
    public ICollection<ChannelUser> Audiences => _audiences.Values;

    internal ChatRoom(TwitchChatClient client, User user)
    {
        Client = client;
        ChannelName = user.Name;
        _roomState = RoomState.CreateDefault(user.Id);
        _badges = new ChannelBadgeStore(client.Client.Rest, this);
    }

    public async Task<Badge?> GetBadgeAsync(PartialBadge badge) =>
        await _badges.GetBadgeAsync(badge);

    internal async Task OnReceivedIrcMessageAsync(IrcMessage message)
    {
        await Task.Yield();
        var command = message.Command;

        if (command.Command == "ROOMSTATE")
        {
            _roomState = RoomState.CreateFromIrcMessage(message, _roomState);
        }

        if (command.Command == "USERSTATE")
        {
            AddOrUpdateCurrentUser(message);
        }
        
        if (command.Command == "GLOBALUSERSTATE")
        {
            AddOrUpdateGlobalCurrentUser(message);
        }
        
        if (command.Command == "353")
        {
            // Don't block the IRC message queue
            _ = Task.Run(async () =>
            {
                var guid = Guid.NewGuid();
                _joinPartQueue.Enqueue(guid);
                
                var globalUsers = await Client.Client.Rest
                    .GetUsersByNameAsync(message.Parameters!.Split(' ').ToArray());
                SpinWait.SpinUntil(() => _joinPartQueue.TryPeek(out var p) && p == guid);

                await _audiencesLock.WaitAsync();
                try
                {
                    foreach (var globalUser in globalUsers)
                    {
                        var user = AddOrUpdateUser(globalUser);
                        _audiences.AddOrUpdate(globalUser.Name.ToLower(), user, (_, _) => user);
                    }

                    Logger.Info(TranslateText.Of("%s Added %s users to audiences list")
                        .AddWith(Component.Literal($"[#{ChannelName}]").SetColor(TextColor.Aqua))
                        .AddWith(Component.Literal(globalUsers.Count + "").SetColor(TextColor.Aqua))
                    );

                    _joinPartQueue.TryDequeue(out _);
                }
                finally
                {
                    _audiencesLock.Release();
                }
            });
        }

        if (command.Command is "JOIN" or "PART")
        {
            var nick = message.Source?.Nick;
            if (nick != null)
            {
                // Don't block the IRC message queue
                _ = Task.Run(async () =>
                {
                    var guid = Guid.NewGuid();
                    _joinPartQueue.Enqueue(guid);
                    
                    var globalUser = await Client.Client.Rest.GetUserByNameAsync(nick);
                    SpinWait.SpinUntil(() => _joinPartQueue.TryPeek(out var p) && p == guid);

                    try
                    {
                        if (globalUser != null)
                        {

                            if (command.Command == "JOIN")
                            {
                                var user = AddOrUpdateUser(globalUser);
                                _audiences.AddOrUpdate(nick, user, (_, _) => user);
                                Logger.Info(TranslateText.Of("%s %s joined the chat")
                                    .AddWith(Component.Literal($"[#{ChannelName}]").SetColor(TextColor.Aqua))
                                    .AddWith(Component.Literal($"{user.DisplayName} ({user.Name})")
                                        .SetColor(TextColor.Gold))
                                );
                            }
                            else
                            {
                                if (_audiences.TryRemove(nick, out var left))
                                {
                                    Logger.Info(TranslateText.Of("%s %s left the chat")
                                        .AddWith(Component.Literal($"[#{ChannelName}]").SetColor(TextColor.Aqua))
                                        .AddWith(
                                            Component.Literal($"{left.DisplayName} ({left.Name})")
                                                .SetColor(TextColor.Gold))
                                    );
                                }
                            }
                        }

                        _joinPartQueue.TryDequeue(out _);
                    }
                    finally
                    {
                        _audiencesLock.Release();
                    }
                });
            }
        }
        
        if (command.Command == "PRIVMSG")
        {
            var id = message.Tags["id"]!.RawValue;
            var comment = GetOrCreateComment(id, message);
            if (MessageReceived != null) 
                await MessageReceived.Invoke(comment);
        }

        if (IrcMessageReceived != null) await IrcMessageReceived.Invoke(message);
    }

    private ChannelUser AddOrUpdateUser(GlobalUser user)
    {
        var id = user.Id;
        if (_users.TryGetValue(id, out var value))
        {
            return value;
        }
        
        value = ChannelUser.Create(this, user);
        _users[id] = value;
        return value;
    }
    
    private ChannelUser AddOrUpdateUser(IrcMessage model)
    {
        var id = model.Tags["user-id"]!.RawValue;
        if (_users.TryGetValue(id, out var value))
        {
            value.Update(model);
            return value;
        }
        
        value = ChannelUser.Create(this, model);
        _users[id] = value;
        return value;
    }
    
    private ChannelUser AddOrUpdateCurrentUser(IrcMessage model)
    {
        var id = Client.Client.Rest.CurrentUser.Id;
        model.Tags["user-id"] = new LiteralIrcTagValue(id);
        return AddOrUpdateUser(model);
    }
    
    private GlobalUser AddOrUpdateGlobalCurrentUser(IrcMessage model)
    {
        var id = Client.Client.Rest.CurrentUser.Id;
        model.Tags["user-id"] = new LiteralIrcTagValue(id);
        return Client.Client.Rest.AddOrUpdateUserByIrc(model);
    }

    public ICollection<ChannelUser> GetChannelUsers() => _users.Values;

    public ChannelUser? GetChannelUser(string id) => _users.GetValueOrDefault(id);

    public ICollection<TwitchComment> GetComments() => _comments.Values;

    public TwitchComment? GetComment(string id) => _comments.GetValueOrDefault(id);

    private TwitchComment GetOrCreateComment(string id, IrcMessage message)
    {
        var user = AddOrUpdateUser(message);
        var comment = _comments.GetOrAdd(id, _ =>
        {
            _orderedCommentIds.Enqueue(id);
            return TwitchComment.Create(Client.Client, this, user, message);
        });

        if (_orderedCommentIds.Count > 100)
        {
            if (_orderedCommentIds.TryDequeue(out var removeId))
            {
                _comments.TryRemove(removeId, out _);
            }
        }

        return comment;
    }

    public async Task SendMessageAsync(string content, CommentReference? commentReference = null)
    {
        await _sendLock.WaitAsync();
        try
        {
            var message = new IrcMessage(new GenericCommand("PRIVMSG", $"#{ChannelName}"))
            {
                Parameters = content
            };

            if (commentReference is {MessageId: not null} comRef)
            {
                message.Tags["reply-parent-msg-id"] = new LiteralIrcTagValue(comRef.MessageId);
            }

            var completed = false;
            var cts = new CancellationTokenSource(3000);
            string? id = null;

            async Task OnNextUserStateMessageReceived(IrcMessage msg)
            {
                if (msg.Command.Command != "USERSTATE") return;
                await Task.Yield();

                IrcMessageReceived -= OnNextUserStateMessageReceived;
                id = msg.Tags["id"]!.RawValue;
                completed = true;
            }

            IrcMessageReceived += OnNextUserStateMessageReceived;

            await Client.WriteMessageAsync(message);
            SpinWait.SpinUntil(() => completed || cts.IsCancellationRequested);
            cts.Token.ThrowIfCancellationRequested();

            message.Tags["id"] = new LiteralIrcTagValue(id!);
            message.Tags["user-id"] = new LiteralIrcTagValue(Client.Client.Rest.CurrentUser.Id);
            message.Tags["kaka-local"] = new LiteralIrcTagValue("1");
            MessageReceived?.Invoke(GetOrCreateComment(id!, message));
        }
        finally
        {
            _sendLock.Release();
        }
    }
}

public record RoomState(bool IsEmoteOnly, int FollowersOnlyMinutes, bool IsUniqueChat, string RoomId, bool IsSlowMode,
    bool IsSubscribersOnly)
{
    public static RoomState CreateDefault(string id) => 
        new(false, -1, false, id, false, false);

    public static RoomState CreateFromIrcMessage(IrcMessage message, RoomState? prev = null)
    {
        if (message.Command.Command != "ROOMSTATE")
        {
            throw new ArgumentException("Invalid room state IRC message.");
        }

        if (!message.Tags.Any())
        {
            throw new ArgumentException("No tags defined in the IRC message.");
        }

        var emoteOnly = Optional.OfNullable(message.Tags["emote-only"]).Select(x => (bool?) (x.RawValue != "0")).OrElseGet(() => null);
        var followersOnly = Optional.OfNullable(message.Tags["followers-only"]).Select(x => (int?) int.Parse(x.RawValue)).OrElseGet(() => null);
        var r9K = Optional.OfNullable(message.Tags["r9k"]).Select(x => (bool?) (x.RawValue != "0")).OrElseGet(() => null);
        var roomId = message.Tags["room-id"]!.RawValue;
        var slow = Optional.OfNullable(message.Tags["slow"]).Select(x => (bool?) (x.RawValue != "0")).OrElseGet(() => null);
        var subsOnly = Optional.OfNullable(message.Tags["subs-only"]).Select(x => (bool?) (x.RawValue != "0")).OrElseGet(() => null);

        prev ??= new RoomState(false, -1, false, roomId, false, false);
        return new RoomState(
            emoteOnly ?? prev.IsEmoteOnly, 
            followersOnly ?? prev.FollowersOnlyMinutes,
            r9K ?? prev.IsUniqueChat, roomId, 
            slow ?? prev.IsSlowMode, 
            subsOnly ?? prev.IsSubscribersOnly);
    }
}