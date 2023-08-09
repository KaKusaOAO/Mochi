using Mochi.Irc;
using Mochi.StreamKit.Twitch.Rest;
using Mochi.Utils;

namespace Mochi.StreamKit.Twitch.Chat;

public class ChatRoom
{
    private readonly TwitchChatClient _client;
    public string ChannelName { get; }
    private RoomState? _roomState;
    public RoomState RoomState => _roomState == null ? null! : _roomState with {};
    public ChannelBadgeStore Badges { get; }

    public event Func<TwitchComment, Task>? MessageReceived;

    internal ChatRoom(TwitchChatClient client, string channelName)
    {
        _client = client;
        ChannelName = channelName;
        Badges = new ChannelBadgeStore(client.Client.Rest, this);
    }

    internal async Task OnReceivedIrcMessageAsync(IrcMessage message)
    {
        await Task.Yield();
        var command = message.Command;

        if (command.Command == "ROOMSTATE")
        {
            _roomState = RoomState.CreateFromIrcMessage(message);
        }
        
        if (command.Command == "PRIVMSG")
        {
            if (MessageReceived != null) 
                await MessageReceived.Invoke(new TwitchComment(this, message));
        }
    }

    public async Task SendMessageAsync(string content)
    {
        var message = new IrcMessage(new GenericCommand("PRIVMSG", $"#{ChannelName}"))
        {
            Parameters = content
        };
        await _client._ircHandler.WriteMessageAsync(message);
    }
}

public record RoomState(bool IsEmoteOnly, bool IsFollowersOnly, bool IsUniqueChat, string RoomId, bool IsSlowMode,
    bool IsSubscribersOnly)
{
    public static RoomState CreateFromIrcMessage(IrcMessage message)
    {
        if (message.Command.Command != "ROOMSTATE")
        {
            throw new ArgumentException("Invalid room state IRC message.");
        }

        if (!message.Tags.Any())
        {
            throw new ArgumentException("No tags defined in the IRC message.");
        }

        var emoteOnly = message.Tags["emote-only"].RawValue != "0";
        var followersOnly = message.Tags["followers-only"].RawValue != "0";
        var r9K = message.Tags["r9k"].RawValue != "0";
        var roomId = message.Tags["room-id"].RawValue;
        var slow = message.Tags["slow"].RawValue != "0";
        var subsOnly = message.Tags["subs-only"].RawValue != "0";

        return new RoomState(emoteOnly, followersOnly, r9K, roomId, slow, subsOnly);
    }
}