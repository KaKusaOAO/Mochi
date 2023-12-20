using Mochi.StreamKit.Twitch.API;
using Model = Mochi.StreamKit.Twitch.API.Emote;

namespace Mochi.StreamKit.Twitch.Entities;

public class Emote : Entity<string>, IEmote
{
    public EmoteSet EmoteSet { get; }
    IEmoteSet IEmote.EmoteSet => EmoteSet;

    public string Name { get; internal set; } = null!;

    public string? OwnerId { get; internal set; }

    public bool IsGlobal => OwnerId == "0";

    public EmoteFormatCollection Formats { get; internal set; } = new();
    public EmoteScaleCollection Scales { get; internal set; } = new();
    public EmoteThemeModeCollection ThemeModes { get; internal set; } = new();
    public StringTemplate Template { get; internal set; } = new();

    private Emote(TwitchClient client, string id, EmoteSet set) : base(client, id)
    {
        EmoteSet = set;
    }

    public static Emote Create(TwitchClient client, string id, EmoteSet set, Model model)
    {
        var emote = new Emote(client, id, set);
        emote.Update(model);
        return emote;
    }
    
    internal virtual void Update(Model model)
    {
        Name = model.Name;
        Formats = model.Formats;
        Scales = model.Scales;
        ThemeModes = model.ThemeModes;

        if (model is EmoteSetEntry entry)
        {
            OwnerId = entry.OwnerId;
        }
    }
    
    public async Task<IUser?> GetOwnerAsync()
    {
        var id = OwnerId;
        if (id is null or "0") return null;
        
        var user = Client.GetCachedUser(id);
        if (user != null) return user;

        return await Client.Rest.GetUserByIdAsync(id);
    }
}