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
        if (visitor is IEmoteVisitor)
        {
            visitor.Accept(this, style);
        }
        else
        {
            if (style is IColoredStyle colored)
            {
                style = colored.WithColor(TextColor.Yellow).ApplyTo(style);
            }
            visitor.Accept(new LiteralContent(@$"[{Emote.Name}]"), style);
        }
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
        throw new NotImplementedException();
    }

    public void InsertPayload(JsonObject target, EmoteContent content)
    {
        throw new NotImplementedException();
    }
}