using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

namespace Mochi.Texts;

public class LiteralContent : IContent<LiteralContent>
{
    public IContentType<LiteralContent> Type => TextContentTypes.Literal;
    
    public string Text { get; set; }
    
    public LiteralContent Clone()
    {
        return new LiteralContent(Text);
    }

    public void Visit(IContentVisitor visitor, IStyle style) => 
        visitor.Accept(this, style);

    public void VisitLiteral(IContentVisitor visitor, IStyle style) => 
        visitor.Accept(this, style);

    public LiteralContent(string? text = null)
    {
        Text = text ?? "";
    }
}

public class LiteralContentType : IContentType<LiteralContent>
{
    public LiteralContent CreateContent(JsonObject payload)
    {
        var text = payload["text"]!.GetValue<string>();
        return new LiteralContent(text);
    }

    public void InsertPayload(JsonObject target, LiteralContent content)
    {
        target["text"] = content.Text;
    }
}

public static class LiteralText
{
    [Obsolete("Use Component.Literal()", true)]
    public static IMutableComponent Of(string? text) => Component.Literal(text);
}