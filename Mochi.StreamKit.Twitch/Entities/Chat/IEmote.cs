using Mochi.StreamKit.Twitch.API;

namespace Mochi.StreamKit.Twitch.Entities;

public interface IEmote : IEntity<string>
{
    public IEmoteSet EmoteSet { get; }
    public string Name { get; }
    public string? OwnerId { get; }
    public bool IsGlobal { get; }
    public EmoteFormatCollection Formats { get; }
    public EmoteScaleCollection Scales { get; }
    public EmoteThemeModeCollection ThemeModes { get; }
    public StringTemplate Template { get; }
    public Task<IUser?> GetOwnerAsync();
}