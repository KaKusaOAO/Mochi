using Mochi.Irc;

namespace Mochi.StreamKit.Twitch.Entities;

using Model = Mochi.StreamKit.Twitch.API.User;

public class GlobalUser : User
{
    public override string Name { get; internal set; }
    public override string DisplayName { get; internal set; }
    public override string ProfileImage { get; internal set; }
    public override UserType Type { get; internal set; }
    public override List<Badge> Badges { get; internal set; } = new();
    public override List<EmoteSet> EmoteSets { get; internal set; } = new();

    internal GlobalUser(TwitchClient client, string id) : base(client, id)
    {
    }

    internal static GlobalUser Create(TwitchClient client, string id, Model model)
    {
        var user = new GlobalUser(client, id);
        user.Update(model);
        return user;
    }
    
    internal static GlobalUser Create(TwitchClient client, string id, IrcMessage model)
    {
        var user = new GlobalUser(client, id);
        user.Update(model);
        return user;
    }
}