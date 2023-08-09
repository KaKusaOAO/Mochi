using Mochi.Irc;
using Mochi.StreamKit.Texts;
using Mochi.Texts;

namespace Mochi.StreamKit.Twitch.Chat;

public class EmoteCollection : IIrcTagValue
{
    public List<EmoteMetadata> Emotes { get; }
    public string RawValue { get; }

    public EmoteCollection(string rawValue)
    {
        RawValue = rawValue;
        Emotes = rawValue.Split('/').Where(s => s.Contains(':')).Select(s =>
        {
            var data = s.Split(':');
            return new EmoteMetadata
            {
                Id = data[0],
                Positions = data[1].Split(',').Select(p =>
                {
                    var posData = p.Split('-');
                    var start = int.Parse(posData[0]);
                    var end = int.Parse(posData[1]) + 1;
                    return start..end;
                }).ToList()
            };
        }).ToList();
    }

    public IComponent ParseContent(string content)
    {
        var components = new List<IComponent>();
        var offset = 0;
        foreach (var (id, range) in Emotes.SelectMany(x =>
                     x.Positions.OrderBy(a => a.Start.Value).Select(r => (x.Id, r))))
        {
            components.Add(Component.Literal(content[offset..range.Start]));

            var name = content[range];
            offset = range.End.Value;

            var emote = new Emote(name, id);
            components.Add(new MutableComponent(new EmoteContent(emote)));
        }
        components.Add(Component.Literal(content[offset..]));
        if (components.Count == 1) return components.First();

        var result = Component.Literal("");
        foreach (var comp in components)
        {
            result.AddExtra(comp);
        }

        return result;
    }
}