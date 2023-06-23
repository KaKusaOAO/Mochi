using System.Text.Json.Nodes;

namespace Mochi.Texts;

public static class TextExtension
{
    public static T SetColor<T>(this T text, TextColor? color) where T : IMutableText
    {
        if (text is ITextGenericHelper<T> g) 
            return g.SetColor(color);
        
        text.Color = color;
        return text;
    }
    
    public static T AddExtra<T>(this T text, params IText[] texts) where T : IText
    {
        if (text is ITextGenericHelper<T> g) 
            return g.AddExtra(texts);
        
        foreach (var t in texts)
        {
            text.Extra.Add(t);
            t.Parent = text;
        }
        return text;
    }
    
    public static T AddWith<T>(this T text, params IText[] texts) where T : IText
    {
        var content = text.Content;
        if (content is not TranslateContent t) return text;
        
        foreach (var w in texts)
        {
            t.AddWith(w);
        }
        return text;
    }
    
    public static T SetBold<T>(this T text, bool val) where T : IMutableText
    {
        text.Bold = val;
        return text;
    }
    
    public static T SetItalic<T>(this T text, bool val) where T : IMutableText
    {
        text.Italic = val;
        return text;
    }
    
    public static T SetUnderline<T>(this T text, bool val) where T : IMutableText
    {
        text.Underline = val;
        return text;
    }
    
    public static T SetObfuscated<T>(this T text, bool val) where T : IMutableText
    {
        text.Obfuscated = val;
        return text;
    }
    
    public static T SetStrikethrough<T>(this T text, bool val) where T : IMutableText
    {
        text.Strikethrough = val;
        return text;
    }
    
    public static T SetReset<T>(this T text, bool val) where T : IMutableText
    {
        text.Reset = val;
        return text;
    }
    
    public static JsonObject ToJson(this IText text)
    {
        var obj = new JsonObject();
        var extras = new JsonArray();
        foreach (var e in text.Extra)
        {
            extras.Add(e.ToJson());
        }

        obj["extra"] = extras;

        if (text.Color != null)
        {
            obj["color"] = "#" + text.Color.Color.RGB.ToString("x6");
        }

        text.Content.InsertPayload(obj);
        return obj;
    }
}