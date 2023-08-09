using Mochi.Irc;

namespace Mochi.StreamKit.Twitch.Chat;

public class BadgeCollection : IIrcTagValue
{
    public List<PartialBadge> Badges { get; } = new();
    public string RawValue { get; }

    public BadgeCollection(string rawValue)
    {
        RawValue = rawValue;

        var pairs = rawValue.Split(',');
        foreach (var pair in pairs)
        {
            var arr = pair.Split('/');
            if (arr.Length < 2) continue;
            Badges.Add(new PartialBadge(arr[0], arr[1]));
        }
    } 
}