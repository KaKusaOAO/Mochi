using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using Mochi.Utils;

namespace Mochi.Texts;

public static class TextContentTypes
{
    private static readonly Dictionary<string, IContentType> _types = new();

    public static readonly LiteralContentType Literal = Register("text", new LiteralContentType());
    public static readonly TranslateContentType Translate = Register("translate", new TranslateContentType());

    public static T Register<T>(string key, T type) where T : IContentType
    {
        _types[key] = type;
        return type;
    }

    public static IOptional<IContentType> GetContentType(string key) => _types.TryGetValue(key, out var type)
        ? Optional.Of(type)
        : Optional.Empty<IContentType>();

    public static IContentType GetContentType(string key, IContentType def) => GetContentType(key).OrElse(def);

    public static IContent CreateContent(string key, JsonObject payload) =>
        GetContentType(key).Select(t => t.CreateContent(payload)).OrElse(EmptyContent.Shared);

    public static IContent CreateContent(JsonObject payload) => _types.Keys.Where(payload.ContainsKey)
        .Select(k => CreateContent(k, payload))
        .FirstOrDefault(c => c is not EmptyContent) ?? EmptyContent.Shared;

    internal class EmptyContent : IContent<EmptyContent>
    {
        private static EmptyContent? _instance;
        public static EmptyContent Shared => _instance ??= new EmptyContent();
        public IText? Parent { get; set; }
        public IContentType<EmptyContent> Type => EmptyContentType.Shared;
        public EmptyContent Clone() => this;
        public string ToAnsi() => "";
        public string ToPlainText() => "";
    }
    
    private class EmptyContentType : IContentType<EmptyContent>
    {
        private static EmptyContentType? _instance;
        public static EmptyContentType Shared => _instance ??= new EmptyContentType();
        
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public EmptyContent CreateContent(JsonObject payload) => EmptyContent.Shared;
        public void InsertPayload(JsonObject target, EmptyContent content) {}
    }
}