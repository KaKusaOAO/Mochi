using Mochi.Irc;
using Mochi.StreamKit.Twitch.Chat.Irc;

namespace Mochi.StreamKit.Twitch.Entities;

using Model = Mochi.StreamKit.Twitch.API.User;

public abstract class User : Entity<string>, IUser, ICacheable
{
    public abstract string Name { get; internal set; }
    public abstract string DisplayName { get; internal set; }
    public abstract string ProfileImage { get; internal set; }
    public abstract UserType Type { get; internal set; }

    public abstract List<Badge> Badges { get; internal set; }

    public abstract List<EmoteSet> EmoteSets { get; internal set; }
    public DateTimeOffset CacheExpireAt { get; private set; } = DateTimeOffset.Now;

    internal User(TwitchClient client, string id) : base(client, id)
    {
    }
    
    internal virtual void Update(Model model)
    {
        Name = model.Login;
        DisplayName = model.DisplayName;
        ProfileImage = model.ProfileImageUrl;
        Type = model.Type;
        CacheExpireAt = DateTimeOffset.Now + TimeSpan.FromMinutes(1);
    }

    internal virtual void Update(IrcMessage message)
    {
        var userId = message.Tags["user-id"]?.RawValue;
        if (userId != Id) return;

        var displayName = message.Tags["display-name"]?.RawValue;
        if (displayName != null)
            DisplayName = displayName;

        if (message.Command.Command.EndsWith("USERSTATE"))
        {
            var emoteSets = message.GetEmoteSets();
            // EmoteSets = emoteSets.EmoteSetIds.Select(e => Client.Rest.GetEmoteSetAsync(e).Result)
            //     .OfType<EmoteSet>().ToList();
            EmoteSets = Client.Rest.GetEmoteSetsAsync(emoteSets.EmoteSetIds.ToArray()).Result;

            if (message.Command.Command == "GLOBALUSERSTATE")
            {
                var badges = message.GetBadges();
                Badges = badges.Badges
                    .Select(b => Client.Rest.GetBadgeAsync(b).Result!)
                    .ToList();
            }
        }

        var source = message.Source;
        if (source != null && message.Command.Command == "PRIVMSG") Name = source.Nick!;
    }
}