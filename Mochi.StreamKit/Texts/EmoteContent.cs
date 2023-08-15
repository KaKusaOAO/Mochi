using System.Text.Json.Nodes;
using Mochi.Texts;

namespace Mochi.StreamKit.Texts;

public class EmoteContent : IContent<EmoteContent>
{
    public Emote Emote { get; }
    
    public IContentType<EmoteContent> Type => EmoteContentType.Shared;

    public EmoteContent(Emote emote)
    {
        Emote = emote;
    }
    
    public EmoteContent Clone() => new(Emote);

    public void Visit(IContentVisitor visitor, IStyle style)
    {
        visitor.Accept(this, style);
    }

    public void VisitLiteral(IContentVisitor visitor, IStyle style)
    {
        if (style is IColoredStyle colored)
        {
            style = colored.WithColor(TextColor.Yellow).ApplyTo(style);
        }
        visitor.Accept(new LiteralContent(@$":{Emote.Name}:"), style);
    }
}

public class EmoteContentType : IContentType<EmoteContent>
{
    public static readonly EmoteContentType Shared = new();
    
    static EmoteContentType()
    {
        TextContentTypes.Register("emote", Shared);
    }
    
    public EmoteContent CreateContent(JsonObject payload)
    {
        var obj = payload["emote"]!.AsObject();
        var name = obj["name"]!.GetValue<string>();
        var url = new Uri(obj["url"]!.GetValue<string>());
        return new EmoteContent(new Emote(name, url));
    }

    public void InsertPayload(JsonObject target, EmoteContent content)
    {
        var emote = content.Emote;
        target["emote"] = new JsonObject
        {
            ["name"] = emote.Name,
            ["url"] = emote.ImageUrl.ToString()
        };
    }
}