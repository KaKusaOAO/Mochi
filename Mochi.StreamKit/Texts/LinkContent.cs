using System.Text.Json.Nodes;
using Mochi.Texts;

namespace Mochi.StreamKit.Texts;

public class LinkContent : IContent<LinkContent>
{
    public Uri Uri { get; }
    
    public IContentType<LinkContent> Type => LinkContentType.Shared;

    public LinkContent(Uri uri)
    {
        Uri = uri;
    }

    public LinkContent Clone() => new(Uri);

    public void Visit(IContentVisitor visitor, IStyle style)
    {
        visitor.Accept(this, style);
    }

    public void VisitLiteral(IContentVisitor visitor, IStyle style)
    {
        if (style is IColoredStyle colored)
        {
            style = colored.WithColor(TextColor.Aqua).ApplyTo(style);
        }
        visitor.Accept(new LiteralContent(@$"[{Uri}]"), style);
    }
}

public class LinkContentType : IContentType<LinkContent>
{
    public static readonly LinkContentType Shared = new();

    static LinkContentType()
    {
        TextContentTypes.Register("link", Shared);
    }

    public LinkContent CreateContent(JsonObject payload)
    {
        var uri = new Uri(payload["link"]!.GetValue<string>());
        return new LinkContent(uri);
    }

    public void InsertPayload(JsonObject target, LinkContent content)
    {
        target["link"] = content.Uri.ToString();
    }
}