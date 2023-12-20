using Mochi.Irc;
using Mochi.StreamKit.Twitch.Entities;

namespace Mochi.StreamKit.Twitch.Chat;

public class EmoteSetCollection : IIrcTagValue
{
    public string RawValue { get; }
    public List<string> EmoteSetIds { get; }

    public EmoteSetCollection(string value)
    {
        RawValue = value;
        EmoteSetIds = value.Split(',').Where(x => !string.IsNullOrEmpty(x)).ToList();
    }
}