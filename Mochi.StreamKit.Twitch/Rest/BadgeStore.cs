using Mochi.StreamKit.Twitch.API;
using Mochi.StreamKit.Twitch.Chat;
using Mochi.StreamKit.Twitch.Entities;

namespace Mochi.StreamKit.Twitch.Rest;

public class BadgeStore
{
    protected readonly TwitchRestApiClient _client;
    private Lazy<Task<List<BadgeSet>>> _badges;
    
    public BadgeStore(TwitchRestApiClient client)
    {
        _client = client;
        _badges = CreateLazyBadgesResolver();
    }

    public async Task<Badge?> GetBadgeAsync(PartialBadge badge)
    {
        var badges = await _badges.Value;
        var badgeSet = badges.FirstOrDefault(x => x.SetId == badge.Name);
        var version = badgeSet?.Versions.FirstOrDefault(x => x.Id == badge.Version);
        return version == null ? null : new Badge(badge.Name, version);
    }

    public void InvalidateCache()
    {
        _badges = CreateLazyBadgesResolver();
    }

    private Lazy<Task<List<BadgeSet>>> CreateLazyBadgesResolver() => new(async () =>
    {
        var set = await GetBadgeSetAsync();
        return set.Data;
    });

    protected virtual async Task<DataList<BadgeSet>> GetBadgeSetAsync() => await _client.InternalGetGlobalChatBadgesAsync();
}