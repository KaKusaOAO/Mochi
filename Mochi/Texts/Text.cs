using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Mochi.Texts;

public static class Text
{
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
            var t = LiteralText.Of("") as IMutableText;
            
            if (o.TryGetPropertyValue("text", out var textNode))
            {
                var text = textNode!.GetValue<string>();
                t = LiteralText.Of(text);
            }

            if (o.TryGetPropertyValue("translate", out var translateNode))
            {
                var key = translateNode!.GetValue<string>();
                var with = o.TryGetPropertyValue("with", out var withNode) ? withNode!.AsArray() : new JsonArray();
                var tt = TranslateText.Of(key);

                foreach (var w in with.Select(FromJson))
                {
                    tt.AddWith(w);
                }

                t = tt;
            }
            
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

public abstract class Text<T> : IText<T>, IMutableText where T : Text<T>
{
    public ICollection<IText> Extra { get; set; } = new List<IText>();
    public IText? Parent { get; set; }
    public TextColor? Color { get; set; }
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Obfuscated { get; set; }
    public bool Underline { get; set; }
    public bool Strikethrough { get; set; }
    public bool Reset { get; set; }

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
        var extra = "";
        foreach (var e in Extra)
        {
            extra += e.ToAnsi() + (Color ?? ParentColor).GetAnsiCode();
        }
        return extra + ParentColor.GetAnsiCode();
    }

    public virtual string ToPlainText()
    {
        var extra = "";
        foreach (var e in Extra)
        {
            extra += e.ToPlainText();
        }
        return extra;
    }
        
    protected abstract T ResolveThis();

    public abstract T Clone();

    protected T CloneToTarget(T clone)
    {
        clone.Extra.Clear();
        foreach (var extra in Extra) clone.Extra.Add(extra);
        clone.Color = Color;
        clone.Bold = Bold; 
        clone.Italic = Italic; 
        clone.Obfuscated = Obfuscated; 
        clone.Strikethrough = Strikethrough; 
        clone.Underline = Underline; 
        clone.Reset = Reset;
        return clone;
    }
    
    public IMutableText MutableCopy() => Clone();

    IText IText.Clone()
    {
        var clone = Clone();
        return CloneToTarget(clone);
    }

    public T AddExtra(params IText[] texts)
    {
        var t = ResolveThis();
        foreach (var text in texts)
        {
            Extra.Add(text);
            text.Parent = this;
        }
        return t;
    }

    public T SetColor(TextColor? color)
    {
        var t = ResolveThis();
        Color = color;
        return t;
    }

    public T Format(TextFormatFlag flags)
    {
        var t = ResolveThis();
        Bold = flags.HasFlag(TextFormatFlag.Bold);
        Italic = flags.HasFlag(TextFormatFlag.Italic);
        Obfuscated = flags.HasFlag(TextFormatFlag.Obfuscated);
        Strikethrough = flags.HasFlag(TextFormatFlag.Strikethrough);
        Underline = flags.HasFlag(TextFormatFlag.Underline);
        Reset = flags.HasFlag(TextFormatFlag.Reset);
        return t;
    }
}