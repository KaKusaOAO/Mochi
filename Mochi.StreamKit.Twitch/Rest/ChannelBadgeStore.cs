using Mochi.StreamKit.Twitch.API;
using Mochi.StreamKit.Twitch.Chat;

namespace Mochi.StreamKit.Twitch.Rest;

public class ChannelBadgeStore : BadgeStore
{
    private readonly ChatRoom _chatRoom;

    public ChannelBadgeStore(TwitchRestApiClient client, ChatRoom chatRoom) : base(client)
    {
        _chatRoom = chatRoom;
    }

    protected override async Task<DataList<BadgeSet>> GetBadgeSetAsync() => 
        await _client.InternalGetChannelChatBadgesAsync(_chatRoom.RoomState.RoomId);
}