namespace Mochi.StreamKit.Twitch.Entities;

public class EmoteSet : Entity<string>, IEmoteSet, ICacheable
{
    protected List<Emote> EmoteStore { get; set; } = new();
    public ICollection<Emote> Emotes => EmoteStore;
    ICollection<IEmote> IEmoteSet.Emotes => Emotes.OfType<IEmote>().ToList();
    
    public DateTimeOffset CacheExpireAt { get; private set; } = DateTimeOffset.Now;

    public EmoteSet(TwitchClient client, string id) : base(client, id)
    {
    }

    internal void Update()
    {
        var now = DateTimeOffset.Now;
        var duration = TimeSpan.FromMinutes(5);
        if (Id == "0") duration = TimeSpan.FromHours(12);

        CacheExpireAt = now + duration;
    }
}