namespace Mochi.StreamKit.Twitch.Entities;

public interface IEmoteSet
{
    public ICollection<IEmote> Emotes { get; }
}