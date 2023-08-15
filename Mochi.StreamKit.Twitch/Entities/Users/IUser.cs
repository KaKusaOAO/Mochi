namespace Mochi.StreamKit.Twitch.Entities;

public interface IUser : IEntity<string>
{
    public string Name { get; }
    public string DisplayName { get; }
    public string ProfileImage { get; }
    public UserType Type { get; }
    public List<Badge> Badges { get; }
    public List<EmoteSet> EmoteSets { get; }
}