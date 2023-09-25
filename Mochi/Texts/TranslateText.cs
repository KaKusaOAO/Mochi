using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Mochi.Texts;

public class TranslateContent : IContent<TranslateContent>
{
    public IContentType<TranslateContent> Type => TextContentTypes.Translate;
    public string Translate { get; set; }
    public ICollection<IComponent> With { get; set; } = new List<IComponent>();

    public TranslateContent(string translate, params IComponent[] texts)
    {
        Translate = translate;
        foreach (var t in texts)
        {
            With.Add(t);
        }
    }

    public TranslateContent AddWith(IComponent text)
    {
        With.Add(text);
        return this;
    }
    
    public TranslateContent Clone() => new(Translate, With.Select(t => (IComponent) t.Clone()).ToArray());

    private List<IComponent>? _decomposed;

    private List<IComponent> Decompose(IStyle style)
    {
        var offset = 0;
        var counter = 0;
        var fmt = Translate;
        var matches = new Regex("%(?:(?:(\\d*?)\\$)?)s").Matches(fmt);
        var parameters = With.ToList();
        
        var result = new List<IComponent>();
        foreach (Match m in matches)
        {
            var c = m.Groups[1].Value;
            var ci = c.Length == 0 ? counter++ : int.Parse(c) - 1;

            var front = fmt[offset..m.Index];
            if (front.Length > 0) 
                result.Add(new GenericMutableComponent(new LiteralContent(front), style.Clear()));

            result.Add(ci >= parameters.Count && ci < 0
                ? new GenericMutableComponent(new LiteralContent(m.Value), style.Clear())
                : parameters[ci].Clone());

            offset = m.Index + m.Length;
        }
        
        result.Add(new GenericMutableComponent(new LiteralContent(fmt[offset..]), style.Clear()));
        return result;
    }

    public void Visit(IContentVisitor visitor, IStyle style)
    {
        _decomposed ??= Decompose(style);
        foreach (var component in _decomposed)
        {
            component.Visit(visitor, style);
        }
    }
    
    public void VisitLiteral(IContentVisitor visitor, IStyle style)
    {
        _decomposed ??= Decompose(style);
        foreach (var component in _decomposed)
        {
            component.VisitLiteral(visitor, style);
        }
    }
}

public class TranslateContentType : IContentType<TranslateContent>
{
    public TranslateContent CreateContent(JsonObject payload)
    {
        var key = payload["translate"]!.GetValue<string>();
        var with = payload.TryGetPropertyValue("with", out var withNode) ? withNode!.AsArray() : new JsonArray();
        return new TranslateContent(key, with.Select(Component.FromJson).ToArray());
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
    public static MutableComponent Of(string format, params IComponent[] texts)
    {
        var content = new TranslateContent(format, texts);
        return new MutableComponent(content);
    }
}