using System.Text.Json.Nodes;
using Mochi.StreamKit.Twitch.Chat;
using Mochi.Texts;

namespace Mochi.StreamKit.Twitch.Texts;

public class CheerContent : IContent<CheerContent>
{
    public IContentType<CheerContent> Type => CheerContentType.Shared;

    public CheerBits Bits { get; }
    public string Sibling { get; }
    
    public CheerContent(int amount, string sibling = "")
    {
        Bits = new CheerBits(amount);
        Sibling = sibling;
    }
    
    public CheerContent Clone() => new(Bits.Amount);

    public void Visit(IContentVisitor visitor, IStyle style)
    {
        visitor.Accept(this, style);
    }

    public void VisitLiteral(IContentVisitor visitor, IStyle style)
    {
        var originalStyle = style;
        if (style is IColoredStyle colored)
        {
            style = colored.WithColor(Bits.Color).ApplyTo(style);
        }
        visitor.Accept(new LiteralContent(@$"[Cheer{Bits.Amount}]"), style);
        visitor.Accept(new LiteralContent(Sibling), originalStyle);
    }
}

public class CheerContentType : IContentType<CheerContent>
{
    public static readonly CheerContentType Shared = new();
    
    static CheerContentType()
    {
        TextContentTypes.Register("cheerBits", Shared);
    }
    
    public CheerContent CreateContent(JsonObject payload)
    {
        var amount = payload["cheerBits"]!.GetValue<int>();
        return new CheerContent(amount);
    }

    public void InsertPayload(JsonObject target, CheerContent content)
    {
        target["cheerBits"] = content.Bits.Amount;
    }
}