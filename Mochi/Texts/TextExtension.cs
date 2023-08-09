using System.Text.Json.Nodes;

namespace Mochi.Texts;

public static class TextExtension
{
    public static T SetColor<T>(this T text, TextColor? color) where T : MutableComponent
    {
        text.Style = text.Style.WithColor(color);
        return text;
    }
    
    public static T AddExtra<T>(this T text, params IComponent[] texts) where T : IComponent
    {
        foreach (var t in texts)
        {
            text.Siblings.Add(t);
        }
        return text;
    }
    
    public static T AddWith<T>(this T text, params IComponent[] texts) where T : IComponent
    {
        var content = text.Content;
        if (content is not TranslateContent t) return text;
        
        foreach (var w in texts)
        {
            t.AddWith(w);
        }
        return text;
    }
    
    public static JsonObject ToJson(this IComponent text)
    {
        var obj = new JsonObject();
        var extras = new JsonArray();
        foreach (var e in text.Siblings)
        {
            extras.Add(e.ToJson());
        }

        obj["extra"] = extras;
        text.Style.SerializeInto(obj);
        text.Content.InsertPayload(obj);
        return obj;
    }
}