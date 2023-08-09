using System;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Mochi.Texts;

public static class Component
{
    private static readonly PlainTextVisitor _plainTextVisitor = new();
    private static readonly AnsiTextVisitor _ansiTextVisitor = new();

    public static string ToPlainText(this IComponent component)
    {
        _plainTextVisitor.Clear();
        component.Visit(_plainTextVisitor, component.Style);
        return _plainTextVisitor.Result;
    }
    
    public static string ToAnsi(this IComponent component)
    {
        _ansiTextVisitor.Clear();
        component.Visit(_ansiTextVisitor, component.Style);
        return _ansiTextVisitor.Result;
    }

    private class PlainTextVisitor : IContentVisitor
    {
        private readonly StringBuilder _sb = new();

        public void Clear() => _sb.Clear();
        
        public void Accept(IContent content, IStyle style)
        {
            if (content is LiteralContent literal)
            {
                _sb.Append(literal.Text);
            }
        }

        public string Result => _sb.ToString();
    }
    
    private class AnsiTextVisitor : IContentVisitor
    {
        private readonly StringBuilder _sb = new();

        public void Clear() => _sb.Clear();
        
        public void Accept(IContent content, IStyle style)
        {
            if (style is IColoredStyle colored)
            {
                var color = colored.Color == null ? LegacyAnsiColor.Reset : AnsiColor.FromTextColor(colored.Color);
                _sb.Append(color.ToAnsiCode());
            }
            
            if (content is LiteralContent literal)
            {
                _sb.Append(literal.Text);
            }
        }

        public string Result => _sb.ToString();
    }
    
    public static MutableComponent Literal(string? text) => new(new LiteralContent(text));

    public static MutableComponent<T> Literal<T>(string? text, T style) where T : IStyle<T> =>
        new(new LiteralContent(text), style);
    
    public static IComponent FromJson(string json) => FromJson(JsonSerializer.Deserialize<JsonNode>(json));

    public static IComponent FromJson(JsonNode? obj)
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
            var content = TextContentTypes.CreateContent(o);
            var t = new MutableComponent(content);
            
            var color = o.TryGetPropertyValue("color", out var colorNode) ? TextColor.Of(colorNode!.GetValue<string>()) : null;
            var extra = o.TryGetPropertyValue("extra", out var extraNode) ? extraNode!.AsArray() : new JsonArray();
            
            return t
                .SetColor(color)
                .AddExtra(extra.Select(FromJson).ToArray());
        }
        
        throw new ArgumentException("Invalid JSON");
    }
    
    public static IComponent RepresentType(Type t, TextColor? color = null)
        => TranslateText.Of($"%s.{t.Name}")
            .SetColor(color ?? TextColor.Gold)
            .AddWith(
                LiteralText.Of(t.Namespace)
                    .SetColor(TextColor.DarkGray)
            );
            
    private static IComponent RepresentInt(int val, TextColor? color = null)
        => LiteralText.Of(val.ToString())
            .SetColor(color ?? TextColor.Gold);

    private static IComponent RepresentDefGoldWithRedSuffix(string s, string suffix, TextColor? color = null)
        => TranslateText.Of($"{s}%s")
            .SetColor(color ?? TextColor.Gold)
            .AddWith(LiteralText.Of(suffix).SetColor(TextColor.Red));

    private static IComponent RepresentLong(long val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(), "L", color);

    private static IComponent RepresentFloat(float val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "f", color);

    private static IComponent RepresentDouble(double val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "d", color);

    private static IComponent RepresentDecimal(decimal val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(CultureInfo.InvariantCulture), "m", color);

    private static IComponent RepresentByte(byte val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(), "b", color);

    private static IComponent RepresentString(string val, TextColor? color = null)
        => TranslateText.Of(@"""%s""")
            .SetColor(color ?? TextColor.Green)
            .AddWith(LiteralText.Of(val));

    private static IComponent RepresentShort(short val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(), "s", color);
        
    private static IComponent RepresentBool(bool val, TextColor? color = null)
        => LiteralText.Of(val.ToString())
            .SetColor(color ?? (val ? TextColor.Green : TextColor.Red));
        
    public static IComponent Represent(object obj, TextColor? color = null)
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
}