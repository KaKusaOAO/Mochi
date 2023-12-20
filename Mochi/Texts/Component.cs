using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Mochi.Texts;

public enum FlattenMode
{
    EmptyWithSiblings,
    OneAndSiblings
}

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

    public static IComponent ToFlattened(this IEnumerable<IComponent> components, FlattenMode mode = FlattenMode.OneAndSiblings)
    {
        var storage = components.ToList();
        storage.RemoveAll(x => x.Content is LiteralContent literal && string.IsNullOrEmpty(literal.Text));
        
        if (!storage.Any()) return Literal("");
        
        var first = storage.First();
        if (mode == FlattenMode.OneAndSiblings)
        {
            if (storage.Count == 1) return first;
            
            var x = first.Clone();
            foreach (var sibling in storage.Skip(1))
            {
                x.AddExtra(sibling);
            }

            return x;
        }

        if (mode == FlattenMode.EmptyWithSiblings)
        {
            var a = new GenericMutableComponent(new LiteralContent(""), first.Style.Clear());
            foreach (var comp in storage)
            {
                a.Siblings.Add(comp);
            }

            return a;
        }

        throw new ArgumentOutOfRangeException(nameof(mode));
    }

    public static IComponent Flatten(this IComponent component, FlattenMode mode = FlattenMode.OneAndSiblings)
    {
        var storage = new List<IComponent>();
        VisitFlatten(component, storage);
        return storage.ToFlattened(mode);
    }

    private static void VisitFlatten(IComponent component, List<IComponent> storage)
    {
        var x = component.Clone();
        x.Siblings.Clear();
        storage.Add(x);

        foreach (var sibling in component.Siblings)
        {
            VisitFlatten(sibling, storage);
        }
    }

    public static IComponent ResolveComponents(this IComponent component, IComponentResolver resolver)
    {
        var source = new List<IComponent>();
        var clone = component.Clone();
        source.AddRange(clone.Siblings);
        clone.Siblings.Clear();
        source.Insert(0, clone);

        var components = new List<IComponent>();
        foreach (var comp in source)
        {
            if (comp.Content is not LiteralContent literal)
            {
                components.Add(comp);
                continue;
            }

            var offset = 0;
            var content = literal.Text;
            var matches = resolver.GetResolvedEntries(content);
            foreach (var match in matches)
            {
                var range = match.Range;
                components.Add(new GenericMutableComponent(new LiteralContent(content[offset..range.Start]), comp.Style.Clear()));
                offset = range.End.Value;

                var produced = match.Resolve(comp.Style);
                if (produced != null) components.Add(produced);
            }

            var remaining = content[offset..];
            if (!string.IsNullOrEmpty(remaining))
                components.Add(new GenericMutableComponent(new LiteralContent(remaining), comp.Style.Clear()));
        }

        return components.ToFlattened();
    }

    public static IComponent
        ResolveComponents(this IComponent component, Regex regex, Func<Match, IStyle, IComponent?> factory) =>
        component.ResolveComponents(new RegexComponentResolver(regex, factory));

    private class PlainTextVisitor : IContentVisitor
    {
        private readonly StringBuilder _sb = new();

        public void Clear() => _sb.Clear();
        
        public void Accept(IContent content, IStyle style)
        {
            if (content is not LiteralContent literal)
            {
                content.VisitLiteral(this, style);
                return;
            }
            
            _sb.Append(literal.Text);
        }

        public string Result => _sb.ToString();
    }
    
    private class AnsiTextVisitor : IContentVisitor
    {
        private readonly StringBuilder _sb = new();

        public void Clear() => _sb.Clear();
        
        public void Accept(IContent content, IStyle style)
        {
            if (content is not LiteralContent literal)
            {
                content.VisitLiteral(this, style);
                return;
            }
            
            if (style is IColoredStyle colored)
            {
                var color = colored.Color == null ? LegacyAnsiColor.Reset : AnsiColor.FromTextColor(colored.Color);
                _sb.Append(color.ToAnsiCode());
            }
            
            _sb.Append(literal.Text);
        }

        public string Result => _sb.ToString();
    }
    
    public static IMutableComponent Literal(string? text) => 
        new MutableComponent(new LiteralContent(text));

    public static IMutableComponent Literal(string? text, IStyle style) => 
        new GenericMutableComponent(new LiteralContent(text), style);

    public static IMutableComponent<T> Literal<T>(string? text, T style) where T : IStyle<T> =>
        new MutableComponent<T>(new LiteralContent(text), style);

    private static IComponent[] FromObjects(object[] parameters)
    {
        return parameters.Select(p =>
        {
            if (p is IComponent comp) return comp;
            if (p is string str) return Literal(str);
            return Literal(p?.ToString() ?? "<null>");
        }).ToArray();
    }
    
    public static IMutableComponent Translatable(string text, params object[] parameters) => 
        new MutableComponent(new TranslateContent(text, FromObjects(parameters)));

    public static IMutableComponent Translatable(string text, IStyle style, params object[] parameters) => 
        new GenericMutableComponent(new TranslateContent(text, FromObjects(parameters)), style);

    public static IMutableComponent<T> Translatable<T>(string text, T style, params object[] parameters) where T : IStyle<T> =>
        new MutableComponent<T>(new TranslateContent(text, FromObjects(parameters)), style);
    
    public static IComponent FromJson(string json) => 
        FromJson(JsonSerializer.Deserialize<JsonNode>(json));
    public static IComponent FromJson(string json, Func<JsonObject, IStyle> parseStyle) => 
        FromJson(JsonSerializer.Deserialize<JsonNode>(json), parseStyle);

    public static Style ParseColorStyle(JsonObject o)
    {
        var color = o.TryGetPropertyValue("color", out var colorNode) ? TextColor.Of(colorNode!.GetValue<string>()) : null;
        return Style.Empty.WithColor(color);
    }

    public static IComponent FromJson(JsonNode? obj, Func<JsonObject, IStyle> parseStyle)
    {
        if (obj is JsonArray arr)
        {
            return Literal("")
                .AddExtra(arr.Select(n => FromJson(n, parseStyle)).ToArray());
        }
        
        if (obj is JsonObject o)
        {
            var content = TextContentTypes.CreateContent(o);
            var t = new GenericMutableComponent(content, parseStyle(o));
            var extra = o.TryGetPropertyValue("extra", out var extraNode) ? extraNode!.AsArray() : new JsonArray();
            return t.AddExtra(extra.Select(n => FromJson(n, parseStyle)).ToArray());
        }
        
        throw new ArgumentException("Invalid JSON");
    }
    
    public static IComponent FromJson(JsonNode? obj) => FromJson(obj, ParseColorStyle);

    public static IComponent RepresentType(Type t, TextColor? color = null)
    {
        var name = t.Name;
        var currentType = t;
        while (currentType.DeclaringType != null)
        {
            name = currentType.DeclaringType.Name + "." + name;
            currentType = currentType.DeclaringType;
        }
        
        return TranslateText.Of($"%s.{name}")
            .SetColor(color ?? TextColor.Gold)
            .AddWith(
                Literal(t.Namespace)
                    .SetColor(TextColor.DarkGray)
            );
    }

    private static IComponent RepresentInt(int val, TextColor? color = null)
        => Literal(val.ToString())
            .SetColor(color ?? TextColor.Gold);

    private static IComponent RepresentDefGoldWithRedSuffix(string s, string suffix, TextColor? color = null)
        => TranslateText.Of($"{s}%s")
            .SetColor(color ?? TextColor.Gold)
            .AddWith(Literal(suffix).SetColor(TextColor.Red));

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
            .AddWith(Literal(val));

    private static IComponent RepresentShort(short val, TextColor? color = null)
        => RepresentDefGoldWithRedSuffix(val.ToString(), "s", color);
        
    private static IComponent RepresentBool(bool val, TextColor? color = null)
        => Literal(val.ToString())
            .SetColor(color ?? (val ? TextColor.Green : TextColor.Red));
        
    public static IComponent Represent(object obj, TextColor? color = null)
    {
        switch (obj)
        {
            case null:
                return Literal("null").SetColor(TextColor.Red);
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