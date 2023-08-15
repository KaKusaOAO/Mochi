using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Mochi.Irc;
using Mochi.StreamKit.Texts;
using Mochi.StreamKit.Twitch.Chat;
using Mochi.StreamKit.Twitch.Chat.Irc;
using Mochi.StreamKit.Twitch.Texts;
using Mochi.Texts;
using Mochi.Utils;

namespace Mochi.StreamKit.Twitch.Entities;

public class TwitchComment : Entity<string>, IGenericComment, IChatMessage
{
    public TwitchComment(TwitchClient client, string id, ChannelUser user, ChatRoom chatRoom) : base(client, id)
    {
        ChatRoom = chatRoom;
        Author = user;
    }

    internal static TwitchComment Create(TwitchClient client, ChatRoom chatRoom, ChannelUser user, IrcMessage message)
    {
        var id = message.Tags["id"]!.RawValue;
        var comment = new TwitchComment(client, id, user, chatRoom);
        comment.Update(message);
        return comment;
    }
    
    internal void Update(IrcMessage message)
    {
        OriginalContent = message.Parameters ?? "";
        Content = ResolveContent(message);
        IsSentFromLocal = false;

        if (message.Tags.ContainsKey("kaka-local"))
        {
            IsSentFromLocal = true;
            Timestamp = DateTimeOffset.Now;
            return;
        }

        if (message.Tags["room-id"]!.RawValue != RoomId) return;
        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(message.Tags["tmi-sent-ts"]!.RawValue));
        IsFirstTime = message.Tags["first-msg"]!.RawValue != "0";
    }

    public List<Badge> Badges => Author.ChannelBadges;
    List<IUserBadge> IGenericComment.Badges => Badges.OfType<IUserBadge>().ToList();

    public string OriginalContent { get; private set; }

    public ChannelUser Author { get; }

    string IGenericComment.Id => "tw-" + Id;
    string IGenericComment.Name => Author.Name;
    public bool IsOwner => Author.Id == RoomId;

    public DateTimeOffset Timestamp { get; private set; }

    public bool IsFirstTime { get; private set; }
    public bool HasGift { get; private set; }
    public bool IsMeAction { get; private set; }

    public bool IsSentFromLocal { get; private set; }

    public CommentReference? CommentReference { get; private set; }

    public IComponent Content { get; private set; } = null!;

    string IGenericComment.DisplayName => Author.DisplayName;
    string IGenericComment.ProfileImage => Author.ProfileImage;
    string IGenericComment.UserId => Author.Id;
    IUser IChatMessage.Author => Author;

    private IComponent ResolveContent(IrcMessage message)
    {
        var content = message.Parameters ?? "";
        var actionMatch = Regex.Match(content, "^\x0001ACTION (.*?)\x0001$");
        IsMeAction = actionMatch.Success;
        if (IsMeAction)
        {
            content = actionMatch.Groups[1].Value;
        }

        var emotes = message.GetEmotes();
        if (Author.Id == Client.Rest.CurrentUser.Id)
        {
            // Parse the emotes manually
            var matches = Regex.Matches(content, "(?:^|\\s?)(?:([^\\s]+))");
            var metadata = new ConcurrentDictionary<string, List<Range>>();
            foreach (Match match in matches)
            {
                var group = match.Groups[1];
                var name = group.Value;
                var emote = Author.EmoteSets.SelectMany(set => set.Emotes).FirstOrDefault(e => e.Name == name);
                if (emote == null) continue;

                var ranges = metadata.GetOrAdd(emote.Id, _ => new List<Range>());
                ranges.Add(new Range(group.Index, group.Index + group.Length));
            }

            foreach (var (id, ranges) in metadata)
            {
                emotes.Emotes.Add(new EmoteMetadata
                {
                    Id = id,
                    Positions = ranges
                });
            }
        }

        var component = (IComponent) Component.Literal(content);
        component = component.ResolveComponents(emotes.GetComponentResolver());
        
        if (message.Tags["reply-parent-msg-id"] != null)
        {
            var parentId = message.Tags["reply-parent-msg-id"]!.RawValue;
            CommentReference = new CommentReference
            {
                MessageId = parentId
            };

            if (component.Content is LiteralContent literal)
            {
                // We only want to process the first part of the components
                var text = Regex.Replace(literal.Text, "^(?:@([^\\s]+))\\s?", "");
                var processed = Component.Literal(text, component.Style);
                foreach (var sibling in component.Siblings)
                {
                    processed.AddExtra(sibling);
                }
                component = processed;
            }
        }

        if (message.Tags.TryGetValue("bits", out var bits))
        {
            var amount = int.Parse(bits.RawValue);
            var newComponent = component.ResolveComponents(
                new Regex("Cheer(\\d+)(\\s?)"),
                (match, style) =>
                {
                    var num = int.Parse(match.Groups[1].Value);
                    var suffix = match.Groups[2].Value;
                    return new GenericMutableComponent(new CheerContent(num, suffix), style.Clear());
                }).Flatten(FlattenMode.EmptyWithSiblings);

            if (newComponent.Siblings.Select(x => x.Content)
                    .OfType<CheerContent>().Sum(x => x.Bits.Amount) != amount)
            {
                Logger.Warn("The total bits amount doesn't match the one tag provided");
            }
            else
            {
                component = newComponent;
            }
        }

        component = component.ResolveComponents(
            new Regex("(http(?:s)?):\\/\\/([\\w_-]+(?:(?:\\.[\\w_-]+)+))([\\w.,@?^=%&:\\/~+#-]*[\\w@?^=%&\\/~+#-])"),
            (match, style) =>
            {
                var uri = new Uri(match.Value);
                return new GenericMutableComponent(new LinkContent(uri), style.Clear());
            });
        component = component.ResolveComponents(
            new Regex("(^|\\s+?)(?:@([^\\s]+))"),
            (match, style) =>
            {
                var prefix = match.Groups[1].Value;
                var name = match.Groups[2].Value;
                var user = ChatRoom.GetChannelUsers().FirstOrDefault(x => x.DisplayName == name);
                if (user == null) return new GenericMutableComponent(new LiteralContent(prefix + match.Value), style.Clear());

                return new GenericMutableComponent(new MentionContent(name, prefix), style.Clear());
            });

        return component;
    }
    
    public ChatRoom ChatRoom { get; }

    public string RoomId => ChatRoom.RoomState.RoomId;
}