using Mochi.Irc;
using Mochi.StreamKit.Texts;
using Mochi.Texts;

namespace Mochi.StreamKit.Twitch.Chat;

public class EmoteCollection : IIrcTagValue
{
    public List<EmoteMetadata> Emotes { get; set; }
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

    public IComponentResolver GetComponentResolver() => new EmoteComponentResolver(this);

    private class EmoteComponentResolver : IComponentResolver
    {
        private readonly EmoteCollection _collection;

        public EmoteComponentResolver(EmoteCollection collection)
        {
            _collection = collection;
        }
        
        public ICollection<IResolvedComponentEntry> GetResolvedEntries(string content)
        {
            return _collection.Emotes
                .SelectMany(x => x.Positions.Select(r => (x.Id, r)))
                .OrderBy(x => x.r.Start.Value)
                .Select(x => new EmoteResolvedComponentEntry(content, x.Id, x.r))
                .OfType<IResolvedComponentEntry>().ToList();
        }
    }

    private class EmoteResolvedComponentEntry : IResolvedComponentEntry
    {
        private readonly string _content;
        private readonly string _id;
        public Range Range { get; }

        public EmoteResolvedComponentEntry(string content, string id, Range range)
        {
            _content = content;
            _id = id;
            Range = range;
        }

        public IComponent? Resolve(IStyle style)
        {
            var name = _content[Range];
            var emote = Emote.CreateFromTwitch(name, _id);
            return new GenericMutableComponent(new EmoteContent(emote), style.Clear());
        }
    }
}