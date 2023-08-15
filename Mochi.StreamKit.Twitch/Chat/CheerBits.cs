using Mochi.StreamKit.Twitch.API;
using Mochi.Structs;
using Mochi.Texts;

namespace Mochi.StreamKit.Twitch.Chat;

public class CheerBits
{
    private static readonly Dictionary<int, TextColor> _levelColor = new()
    {
        [1] = TextColor.Of(new Color(0x818181)),
        [100] = TextColor.Of(new Color(0x852bcf)),
        [1000] = TextColor.Of(new Color(0x12a598)),
        [5000] = TextColor.Of(new Color(0x0080d5)),
        [10000] = TextColor.Of(new Color(0xd92719))
    };
    private static readonly StringTemplate _template = StringTemplate
        .Parse("https://d3aqoihi2n8ty8.cloudfront.net/actions/cheer/dark/animated/{{level}}/4.gif");
    
    public int Amount { get; }

    public CheerBits(int amount)
    {
        Amount = amount;
    }

    public int Level => _levelColor.Keys.Last(x => x <= Amount);
    public TextColor Color => _levelColor.Last(x => x.Key <= Amount).Value;

    public Uri GetCheerEmoteUrl() => new(_template.Resolve(new Dictionary<string, string>
    {
        ["level"] = Level + ""
    }));
}