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

    public static IMutableComponent FromLegacyText(string message)
    {
        var texts = new List<IComponent>();
        var sb = new StringBuilder();
        var t = Component.Literal("");

        for (var i = 0; i < message.Length; i++)
        {
            var c = message[i];
            if (c == '\u00a7')
            {
                if (++i >= message.Length) break;
                c = message[i];

                // lower case
                if (c >= 'A' && c <= 'Z') c += (char)32;

                TextColor? color;
                if (c == 'x' && i + 12 < message.Length)
                {
                    StringBuilder hex = new("#");
                    for (var j = 0; j < 6; j++)
                    {
                        hex.Append(message[i + 2 + j * 2]);
                    }

                    try
                    {
                        color = TextColor.Of(hex.ToString());
                    }
                    catch (ArgumentException)
                    {
                        color = null;
                    }
                }
                else
                {
                    color = TextColor.Of(c);
                }
                
                if (color == null) continue;
                    
                // push old text to the list
                if (sb.Length > 0)
                {
                    var old = t;
                    t = old.Clone();
                    
                    ((LiteralContent) old.Content).Text = sb.ToString();
                    sb.Clear();
                    texts.Add(old);
                }

                t = Component.Literal("").SetColor(color);
                continue;
            }

            sb.Append(c);
        }

        ((LiteralContent) t.Content).Text = sb.ToString();
        texts.Add(t);

        var result = Component.Literal("");
        foreach (var text in texts)
        {
            result.AddExtra(text);
        }

        return result;
    }
}