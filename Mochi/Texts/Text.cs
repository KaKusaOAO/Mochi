using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Mochi.Texts;

public class Text : IMutableText, ITextGenericHelper<Text>
{
    public IContent Content { get; set; } = TextContentTypes.EmptyContent.Shared;
    public ICollection<IText> Extra { get; set; } = new List<IText>();
    public IText? Parent { get; set; }
    public TextColor? Color { get; set; }
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Obfuscated { get; set; }
    public bool Underline { get; set; }
    public bool Strikethrough { get; set; }
    public bool Reset { get; set; }

    private Text() {}

    public Text(IContent content)
    {
        Content = content;
        content.BindParent(this);
    }

    public bool ShouldSerializeExtra() => Extra.Count > 0;
        
    public TextColor? ParentColor
    {
        get
        {
            if (Parent == null) return null;
            return Parent.Color ?? Parent.ParentColor;
        }
    }

    public virtual string ToAnsi()
    {
        var color = (Color ?? ParentColor).GetAnsiCode();
        var extra = Extra.Aggregate("", (current, e) => 
            current + e.ToAnsi() + (Color ?? ParentColor).GetAnsiCode());
        return color + Content.ToAnsi() + extra + ParentColor.GetAnsiCode();
    }

    public virtual string ToPlainText()
    {
        var extra = Extra.Aggregate("", (current, e) => current + e.ToPlainText());
        return Content.ToPlainText() + extra;
    }

    public Text AddExtra(params IText[] texts)
    {
        foreach (var t in texts)
        {
            Extra.Add(t);
        }

        return this;
    }

    public Text SetColor(TextColor? color)
    {
        Color = color;
        return this;
    }

    public Text Clone()
    {
        var content = Content.Clone();
        var clone = new Text(content);
        
        clone.Extra.Clear();
        foreach (var extra in Extra)
        {
            clone.Extra.Add(extra);
        }
        
        clone.Color = Color;
        clone.Bold = Bold; 
        clone.Italic = Italic; 
        clone.Obfuscated = Obfuscated; 
        clone.Strikethrough = Strikethrough; 
        clone.Underline = Underline; 
        clone.Reset = Reset;
        return clone;
    }
    IMutableText IText.Clone() => Clone();

    public static IText Literal(string text) => LiteralText.Of(text);
    public static IText Translatable(string translate, params IText[] texts) => TranslateText.Of(translate, texts);

    public static IText RepresentType(Type t, TextColor? color = null)
        => TranslateText.Of($"%s.{t.Name}")
            .SetColor(color ?? TextColor.Gold)
            .AddWith(
                LiteralText.Of(t.Namespace)
                    .SetColor(TextColor.DarkGray)
            );
            
    private static IText RepresentInt(int val, TextColor? color = null)
        => LiteralText.Of(val.ToString())
            .SetColor(color ?? TextColor.Gold);

    private static IText RepresentDefGoldWithRedSuffix(string s, string suffix, TextColor? color = null)
        => TranslateText.Of($"{s}%s")
            .SetColor(color ?? TextColor.Gold)
            .AddWith(LiteralText.Of(suffix).SetColor(TextColor.Red));

    private static IText RepresentLong(long val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(), "L", color);

    private static IText RepresentFloat(float val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "f", color);

    private static IText RepresentDouble(double val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "d", color);

    private static IText RepresentDecimal(decimal val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "m", color);

    private static IText RepresentByte(byte val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(), "b", color);

    private static IText RepresentString(string val, TextColor? color = null)
        => TranslateText.Of(@"""%s""")
            .SetColor(color ?? TextColor.Green)
            .AddWith(LiteralText.Of(val));

    private static IText RepresentShort(short val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(), "s", color);
        
    private static IText RepresentBool(bool val, TextColor? color = null)
        => LiteralText.Of(val.ToString())
            .SetColor(color ?? (val ? TextColor.Green : TextColor.Red));
        
    public static IText Represent(object obj, TextColor? color = null)
    {
        switch (obj)
        {
            case null:
                return LiteralText.Of("null").SetColor(TextColor.Red);
            case int i:
                return RepresentInt(i, color);
            case long l:
                return RepresentLong(l, color);
            case float f:
                return RepresentFloat(f, color);
            case double d:
                return RepresentDouble(d, color);
            case decimal d:
                return RepresentDecimal(d, color);
            case byte b:
                return RepresentByte(b, color);
            case bool b:
                return RepresentBool(b, color);
            case string s:
                return RepresentString(s, color);
            case short s:
                return RepresentShort(s, color);
            case Type t:
                return RepresentType(t, color);
        }
        return TranslateText.Of("(%s)")
            .SetColor(color)
            .AddWith(RepresentType(obj.GetType()));
    }

    public static IText FromJson(string json) => FromJson(JsonSerializer.Deserialize<JsonNode>(json));

    public static IText FromJson(JsonNode? obj)
    {
        if (obj is JsonValue val)
        {
            return LiteralText.FromLegacyText(val.GetValue<string>());
        }

        if (obj is JsonArray arr)
        {
            return LiteralText.Of("")
                .AddExtra(arr.Select(FromJson).ToArray());
        }
        
        if (obj is JsonObject o)
        {
            var t = new Text();

            var content = TextContentTypes.CreateContent(o);
            t.Content = content;
            content.BindParent(t);
            
            var color = o.TryGetPropertyValue("color", out var colorNode) ? TextColor.Of(colorNode!.GetValue<string>()) : null;
            var bold = o.TryGetPropertyValue("bold", out var boldNode) && boldNode!.GetValue<bool>();
            var italic = o.TryGetPropertyValue("italic", out var italicNode) && italicNode!.GetValue<bool>();
            var obfuscated = o.TryGetPropertyValue("obfuscated", out var obfuscatedNode) && obfuscatedNode!.GetValue<bool>();
            var underline = o.TryGetPropertyValue("underline", out var underlineNode) && underlineNode!.GetValue<bool>();
            var strikethrough = o.TryGetPropertyValue("strikethrough", out var strikethroughNode) && strikethroughNode!.GetValue<bool>();
            var reset = o.TryGetPropertyValue("reset", out var resetNode) && resetNode!.GetValue<bool>();
            var extra = o.TryGetPropertyValue("extra", out var extraNode) ? extraNode!.AsArray() : new JsonArray();
            
            return t
                .SetColor(color)
                .SetBold(bold)
                .SetItalic(italic)
                .SetObfuscated(obfuscated)
                .SetUnderline(underline)
                .SetStrikethrough(strikethrough)
                .SetReset(reset)
                .AddExtra(extra.Select(FromJson).ToArray());
        }
        
        throw new ArgumentException("Invalid JSON");
    }
}