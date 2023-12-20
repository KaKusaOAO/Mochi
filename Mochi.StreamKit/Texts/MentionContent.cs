using System.Text.Json.Nodes;
using Mochi.Texts;

namespace Mochi.StreamKit.Texts;

public class MentionContent : IContent<MentionContent>
{
    public string Text { get; }
    public string Prefix { get; }
    
    public IContentType<MentionContent> Type => MentionContentType.Shared;

    public MentionContent(string text, string prefix = "")
    {
        Text = text;
        Prefix = prefix;
    }

    public MentionContent Clone() => new(Text);

    public void Visit(IContentVisitor visitor, IStyle style)
    {
        visitor.Accept(this, style);
    }

    public void VisitLiteral(IContentVisitor visitor, IStyle style)
    {
        var original = style;
        if (style is IColoredStyle colored)
        {
            style = colored.WithColor(TextColor.Green).ApplyTo(style);
        }
        
        visitor.Accept(new LiteralContent(Prefix), original);
        visitor.Accept(new LiteralContent(@$"@{Text}"), style);
    }
}

public class MentionContentType : IContentType<MentionContent>
{
    public static readonly MentionContentType Shared = new();

    static MentionContentType()
    {
        TextContentTypes.Register("mention", Shared);
    }

    public MentionContent CreateContent(JsonObject payload)
    {
        var text = payload["mention"]!.GetValue<string>();
        var prefix = payload["prefix"]!.GetValue<string>();
        return new MentionContent(text, prefix);
    }

    public void InsertPayload(JsonObject target, MentionContent content)
    {
        target["mention"] = content.Text;
        target["prefix"] = content.Prefix;
    }
}