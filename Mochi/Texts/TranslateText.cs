using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Mochi.Texts;

public class TranslateContent : IContent<TranslateContent>
{
    public IText? Parent { get; set; }
    public IContentType<TranslateContent> Type => TextContentTypes.Translate;
    public string Translate { get; set; }
    public ICollection<IText> With { get; set; } = new List<IText>();

    public TranslateContent(string translate, params IText[] texts)
    {
        Translate = translate;
        foreach (var t in texts)
        {
            With.Add(t);
        }
    }

    public TranslateContent AddWith(IText text)
    {
        With.Add(text);
        text.Parent = Parent;
        return this;
    }

    public void BindParent(IText? parent)
    {
        Parent = parent;
        foreach (var text in With)
        {
            text.Parent = Parent;
        }
    }

    public TranslateContent Clone() => new(Translate, With.Select(t => (IText) t.Clone()).ToArray());

    public string ToAnsi()
    {
        var color = (Parent?.Color ?? Parent?.ParentColor).GetAnsiCode();
        var withAscii = With.Select(text => (object) (text.ToAnsi() + color)).ToArray();
        return Format(Translate, withAscii);
    }

    public string ToPlainText()
    {
        var result = "";
        for (var i = 0; i < Translate.Length; i++)
        {
            var b = Translate;
            if (b[i] == TextColor.ColorChar && TextColor.McCodes().ToList().IndexOf(b[i + 1]) > -1)
            {
                i += 2;
            }
            else
            {
                result += b[i];
            }
        }

        var withAscii = With.Select(text => (object) text.ToPlainText()).ToArray();
        return Format(result, withAscii);
    }

    private string Format(string fmt, params object[] obj)
    {
        var offset = -1;
        var counter = 0;
        var matches = new Regex("%(?:(?:(\\d*?)\\$)?)s").Matches(fmt);
        foreach (Match m in matches)
        {
            var c = m.Groups[1].Value;
            if (c.Length == 0)
            {
                c = counter++ + "";
            }

            offset += c.Length + 2 - m.Value.Length;
            // fmt = fmt[..(m.Index + offset)] + "{" + c + "}" + fmt[(m.Index + offset + m.Value.Length)..];
            // Need to use legacy syntax to support older versions of .NET
            fmt = fmt.Substring(0, m.Index + offset) + $"{{{c}}}" +
                  fmt.Substring(m.Index + offset + m.Value.Length);
        }

        var o = obj.ToList();
        for (var i = 0; i < counter; i++) o.Add("");
        return string.Format(fmt, o.ToArray());
    }
}

public class TranslateContentType : IContentType<TranslateContent>
{
    public TranslateContent CreateContent(JsonObject payload)
    {
        var key = payload["translate"]!.GetValue<string>();
        var with = payload.TryGetPropertyValue("with", out var withNode) ? withNode!.AsArray() : new JsonArray();
        return new TranslateContent(key, with.Select(Text.FromJson).ToArray());
    }

    public void InsertPayload(JsonObject target, TranslateContent content)
    {
        target["translate"] = content.Translate;
        if (!content.With.Any()) return;
        
        var arr = new JsonArray();
        foreach (var text in content.With.Select(t => t.ToJson()))
        {
            arr.Add(text);
        }

        target["with"] = arr;
    }
}

public static class TranslateText
{
    public static IMutableText Of(string format, params IText[] texts)
    {
        var content = new TranslateContent(format, texts);
        return new Text(content);
    }
}