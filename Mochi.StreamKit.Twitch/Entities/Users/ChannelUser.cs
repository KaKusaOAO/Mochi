using Mochi.Irc;
using Mochi.StreamKit.Twitch.Chat;
using Mochi.StreamKit.Twitch.Chat.Irc;

namespace Mochi.StreamKit.Twitch.Entities;

using Model = Mochi.StreamKit.Twitch.API.User;

public class ChannelUser : User, IChannelUser
{
    public ChatRoom ChatRoom { get; }
    protected GlobalUser GlobalUser { get; }

    public List<Badge> ChannelBadges { get; private set; } = new();

    public bool IsModerator { get; private set; }

    public bool IsSubscriber { get; private set; }

    internal ChannelUser(ChatRoom chatRoom, GlobalUser globalUser) : base(chatRoom.Client.Client, globalUser.Id)
    {
        ChatRoom = chatRoom;
        GlobalUser = globalUser;
    }
    
    internal static ChannelUser Create(ChatRoom chatRoom, GlobalUser globalUser) => new(chatRoom, globalUser);

    internal static ChannelUser Create(ChatRoom chatRoom, Model model)
    {
        var user = new ChannelUser(chatRoom, chatRoom.Client.Client.GetOrCreateUser(model));
        user.Update(model);
        return user;
    }
    
    internal static ChannelUser Create(ChatRoom chatRoom, IrcMessage model)
    {
        var user = new ChannelUser(chatRoom, chatRoom.Client.Client.GetOrCreateUser(model));
        user.Update(model);
        return user;
    }

    public override string Name
    {
        get => GlobalUser.Name;
        internal set => GlobalUser.Name = value;
    }

    public override string DisplayName
    {
        get => GlobalUser.DisplayName;
        internal set => GlobalUser.DisplayName = value;
    }

    public override string ProfileImage
    {
        get => GlobalUser.ProfileImage;
        internal set => GlobalUser.ProfileImage = value;
    }

    public override UserType Type
    {
        get => GlobalUser.Type;
        internal set => GlobalUser.Type = value;
    }

    public override List<Badge> Badges
    {
        get => GlobalUser.Badges;
        internal set => GlobalUser.Badges = value;
    }

    public override List<EmoteSet> EmoteSets
    {
        get => GlobalUser.EmoteSets;
        internal set => GlobalUser.EmoteSets = value;
    }

    internal override void Update(Model model)
    {
        base.Update(model);
    }

    internal override void Update(IrcMessage message)
    {
        base.Update(message);
        
        if (message.Command.Arguments.Contains($"#{ChatRoom.ChannelName}"))
        {
            var badges = message.GetBadges();
            ChannelBadges = badges.Badges
                .Select(b => ChatRoom.GetBadgeAsync(b).Result ?? Client.Rest.GetBadgeAsync(b).Result!)
                .ToList();

            if (message.Command.Command == "USERSTATE")
            {
                IsModerator = message.Tags["mod"]!.RawValue == "1";
                IsSubscriber = message.Tags["subscriber"]!.RawValue == "1";
            }
        }
    }
}